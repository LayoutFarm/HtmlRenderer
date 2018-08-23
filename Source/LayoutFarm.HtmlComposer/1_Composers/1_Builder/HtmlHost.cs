//BSD, 2014-present, WinterDev 
//ArthurHub, Jose Manuel Menendez Poo

using System;
using System.Collections.Generic;
using System.Text;

using PixelFarm.Drawing;

using LayoutFarm.Composers;
using LayoutFarm.ContentManagers;
using LayoutFarm.Css;
using LayoutFarm.WebDom;

namespace LayoutFarm.HtmlBoxes
{

    public class HtmlHostCreationConfig
    {
        StringBuilder _validateResult;
        public HtmlHostCreationConfig()
        {

        }
        public CssActiveSheet ActiveSheet { get; set; }
        public ITextService TextService { get; set; }
        public RootGraphic RootGraphic { get; set; }
        public bool ValidateConfig()
        {
            _validateResult = new StringBuilder();
            bool validatePass = true;
            if (TextService == null)
            {
                _validateResult.AppendLine("No TextService");
                validatePass = false;
            }
            if (RootGraphic == null)
            {
                _validateResult.AppendLine("No RootGraphics");
                validatePass = false;
            }

            return validatePass;
        }
        public string GetValidationResult()
        {
            return (_validateResult == null) ? null : _validateResult.ToString();
        }
    }


    public class HtmlHost
    {

        //a HtmlHost 
        //  1. manages multiple HtmlVisual roots
        //  2. controls shared I/O

        List<CustomCssBoxGenerator> generators = new List<CustomCssBoxGenerator>();
        HtmlVisualRootUpdateHandler _visualHtmlRootUpdateHandler;
        EventHandler<ImageRequestEventArgs> _requestImage;
        EventHandler<TextRequestEventArgs> _requestStyleSheet;
        List<CssBox> waitForUpdateBoxes = new List<CssBox>();

        HtmlDocument commonHtmlDoc;
        RootGraphic rootgfx;
        Queue<LayoutVisitor> htmlLayoutVisitorStock = new Queue<LayoutVisitor>();
        RenderTreeBuilder renderTreeBuilder;

        ITextService _textservice;
        Svg.SvgCreator _svgCreator;


        public HtmlHost(HtmlHostCreationConfig config)
        {
            //use default style sheet

#if DEBUG
            if (!config.ValidateConfig())
            {
                throw new NotSupportedException();
            }
#endif

            this.commonHtmlDoc = new HtmlDocument(this);
            if (config.ActiveSheet != null)
            {
                this.commonHtmlDoc.CssActiveSheet = this.BaseStylesheet = config.ActiveSheet;
            }
            else
            {
                //use default
                this.commonHtmlDoc.CssActiveSheet = this.BaseStylesheet =
                     LayoutFarm.WebDom.Parser.CssParserHelper.ParseStyleSheet(null,
                     LayoutFarm.Composers.CssDefaults.DefaultCssData,
                    true);
            }
            this.rootgfx = config.RootGraphic;

            this._textservice = config.TextService;
            _svgCreator = new Svg.SvgCreator();


        }

        //----------------------
        public void ClearUpdateWaitingCssBoxes()
        {
            int j = waitForUpdateBoxes.Count;
            for (int i = 0; i < j; ++i)
            {
                CssBox cssbox = this.waitForUpdateBoxes[i];
                var controller = HtmlBoxes.CssBox.UnsafeGetController(cssbox) as UI.IUIEventListener;
                if (controller != null)
                {
                    controller.HandleElementUpdate();
                }
            }
            waitForUpdateBoxes.Clear();
        }

        public RootGraphic RootGfx { get { return this.rootgfx; } }

        //----------------------
        public void AttachEssentailHandlers(
            EventHandler<ImageRequestEventArgs> reqImageHandler,
            EventHandler<TextRequestEventArgs> reqStyleSheetHandler)
        {
            _requestImage = reqImageHandler;
            _requestStyleSheet = reqStyleSheetHandler;
        }
        internal void DetachEssentailHanlders()
        {
            _requestImage = null;
            _requestStyleSheet = null;
        }
        //----------------------

        public void SetHtmlVisualRootUpdateHandler(HtmlVisualRootUpdateHandler visualHtmlRootUpdateHandler)
        {
            this._visualHtmlRootUpdateHandler = visualHtmlRootUpdateHandler;
        }

        //---
        public HtmlDocument CreateNewDocumentFragment()
        {
            return new HtmlDocumentFragment(this.commonHtmlDoc);
        }
        public HtmlDocument CreateNewSharedHtmlDoc()
        {
            //!!! this is my extension *** 
            HtmlDocument sharedDocument = new HtmlSharedDocument(this.commonHtmlDoc);
            sharedDocument.SetDomUpdateHandler(this.commonHtmlDoc.DomUpdateHandler);
            sharedDocument.CssActiveSheet = this.commonHtmlDoc.CssActiveSheet;
            return sharedDocument;
        }

        public WebDom.CssActiveSheet BaseStylesheet { get; private set; }
        //---
        public LayoutVisitor GetSharedHtmlLayoutVisitor(HtmlVisualRoot htmlVisualRoot)
        {
            LayoutVisitor lay = null;
            if (htmlLayoutVisitorStock.Count == 0)
            {
                lay = new LayoutVisitor(this.GetTextService());
            }
            else
            {
                lay = this.htmlLayoutVisitorStock.Dequeue();
            }
            lay.Bind(htmlVisualRoot);
            return lay;
        }
        public void ReleaseHtmlLayoutVisitor(LayoutVisitor lay)
        {
            lay.UnBind();
            this.htmlLayoutVisitorStock.Enqueue(lay);
        }
         
        public LayoutFarm.Composers.RenderTreeBuilder GetRenderTreeBuilder()
        {
            if (this.renderTreeBuilder == null)
            {
                renderTreeBuilder = new Composers.RenderTreeBuilder(this);
                this.renderTreeBuilder.RequestStyleSheet += (e) =>
                {
                    if (_requestStyleSheet != null)
                    {
                        _requestStyleSheet(this, e);
                    }
                };
            }
            return renderTreeBuilder;
        }
        public void RegisterCssBoxGenerator(LayoutFarm.Composers.CustomCssBoxGenerator cssBoxGenerator)
        {
            this.generators.Add(cssBoxGenerator);
        }
        public void UpdateChildBoxes(WebDom.Impl.HtmlElement parentElement, bool fullmode)
        {
            //for public 
            UpdateChildBoxes((HtmlElement)parentElement, fullmode);
        }
        public CssBox CreateBox(CssBox parentBox, WebDom.Impl.HtmlElement childElement, bool fullmode)
        {
            return CreateBoxInternal(parentBox, (HtmlElement)childElement, fullmode);
        }
        //-------------



        //
        internal void ChildRequestImage(ImageBinder binder, HtmlVisualRoot visualRoot, object reqFrom, bool _sync)
        {
            if (this._requestImage != null)
            {
                ImageRequestEventArgs resReq = new ImageRequestEventArgs(binder);
                resReq.requestBy = reqFrom;
                _requestImage(this, resReq);
            }
        }
        internal ITextService GetTextService()
        {
            return _textservice;
        }
        internal void EnqueueCssUpdate(CssBox box)
        {
            waitForUpdateBoxes.Add(box);
        }
        internal void NotifyHtmlVisualRootUpdate(HtmlVisualRoot htmlVisualRoot)
        {
            if (_visualHtmlRootUpdateHandler != null)
            {
                _visualHtmlRootUpdateHandler(htmlVisualRoot);
            }
        }
        /// update some or generate all cssbox
        /// </summary>
        /// <param name="parentElement"></param>
        /// <param name="fullmode">update all nodes (true) or somenode (false)</param>
        internal void UpdateChildBoxes(HtmlElement parentElement, bool fullmode)
        {
            //recursive ***  
            //first just generate into primary pricipal box
            //layout process  will correct it later  
            switch (parentElement.ChildrenCount)
            {
                case 0: { } break;
                case 1:
                    {
                        CssBox hostBox = HtmlElement.InternalGetPrincipalBox(parentElement);
                        //only one child -- easy 
                        DomNode child = parentElement.GetChildNode(0);
                        int newBox = 0;
                        switch (child.NodeKind)
                        {
                            case HtmlNodeKind.TextNode:
                                {
                                    HtmlTextNode singleTextNode = (HtmlTextNode)child;
                                    if (!singleTextNode.HasSetSplitPart)
                                    {
                                        CssBox principalCssBox = parentElement.CurrentPrincipalBox;
                                        bool isblockContext = (principalCssBox != null) ? principalCssBox.IsBlock : false;
                                        renderTreeBuilder.UpdateTextNode(parentElement, singleTextNode, isblockContext);
                                    }
                                    RunListHelper.AddRunList(hostBox, parentElement.Spec, singleTextNode);
                                }
                                break;
                            case HtmlNodeKind.ShortElement:
                            case HtmlNodeKind.OpenElement:
                                {
                                    HtmlElement childElement = (HtmlElement)child;
                                    var spec = childElement.Spec;
                                    if (spec.CssDisplay == CssDisplay.None)
                                    {
                                        return;
                                    }

                                    newBox++;
                                    //-------------------------------------------------- 
                                    if (fullmode)
                                    {
                                        CssBox newbox = CreateBoxInternal(parentElement, childElement, fullmode);
                                        childElement.SetPrincipalBox(newbox);
                                    }
                                    else
                                    {
                                        CssBox existing = HtmlElement.InternalGetPrincipalBox(childElement);
                                        if (existing == null)
                                        {
                                            CssBox box = CreateBoxInternal(parentElement, childElement, fullmode);
                                            childElement.SetPrincipalBox(box);
                                        }
                                        else
                                        {
                                            //just insert                                                 
                                            hostBox.AppendChild(existing);
                                            if (!childElement.SkipPrincipalBoxEvalulation)
                                            {
                                                existing.Clear();
                                                UpdateChildBoxes(childElement, fullmode);
                                                childElement.SkipPrincipalBoxEvalulation = true;
                                            }
                                        }
                                    }
                                }
                                break;
                        }
                    }
                    break;
                default:
                    {

                        CssBox hostBox = HtmlElement.InternalGetPrincipalBox(parentElement);
                        CssWhiteSpace ws = parentElement.Spec.WhiteSpace;
                        int childCount = parentElement.ChildrenCount;

                        int newBox = 0;
                        for (int i = 0; i < childCount; ++i)
                        {
                            DomNode childNode = parentElement.GetChildNode(i);
                            switch (childNode.NodeKind)
                            {
                                case HtmlNodeKind.TextNode:
                                    {
                                        HtmlTextNode textNode = (HtmlTextNode)childNode;
                                        switch (ws)
                                        {
                                            case CssWhiteSpace.Pre:
                                            case CssWhiteSpace.PreWrap:
                                                {
                                                    RunListHelper.AddRunList(
                                                        CssBox.AddNewAnonInline(hostBox),
                                                        parentElement.Spec, textNode);
                                                }
                                                break;
                                            case CssWhiteSpace.PreLine:
                                                {
                                                    if (newBox == 0 && textNode.IsWhiteSpace)
                                                    {
                                                        continue;//skip
                                                    }

                                                    RunListHelper.AddRunList(
                                                        CssBox.AddNewAnonInline(hostBox),
                                                        parentElement.Spec, textNode);
                                                }
                                                break;
                                            default:
                                                {
                                                    if (textNode.IsWhiteSpace)
                                                    {
                                                        continue;//skip
                                                    }
                                                    RunListHelper.AddRunList(
                                                        CssBox.AddNewAnonInline(hostBox),
                                                        parentElement.Spec, textNode);
                                                }
                                                break;
                                        }

                                        newBox++;
                                    }
                                    break;
                                case HtmlNodeKind.ShortElement:
                                case HtmlNodeKind.OpenElement:
                                    {
                                        HtmlElement childElement = (HtmlElement)childNode;
                                        BoxSpec spec = childElement.Spec;
                                        if (spec.CssDisplay == CssDisplay.None)
                                        {
                                            continue;
                                        }

                                        if (fullmode)
                                        {
                                            CssBox box = CreateBoxInternal(hostBox, childElement, fullmode);
                                        }
                                        else
                                        {
                                            CssBox existingCssBox = HtmlElement.InternalGetPrincipalBox(childElement);
                                            if (existingCssBox == null)
                                            {
                                                CreateBoxInternal(hostBox, childElement, fullmode);
                                            }
                                            else
                                            {
                                                //just insert           
                                                hostBox.AppendChild(existingCssBox);
                                                if (!childElement.SkipPrincipalBoxEvalulation)
                                                {
                                                    if (childElement.HasSpecialPresentation)
                                                    {
                                                        childElement.SpecialPresentationUpdate(null);
                                                    }
                                                    else
                                                    {
                                                        existingCssBox.Clear();
                                                        UpdateChildBoxes(childElement, fullmode);
                                                    }
                                                    childElement.SkipPrincipalBoxEvalulation = true;
                                                }
                                            }
                                        }
                                        newBox++;
                                    }
                                    break;
                                default:
                                    {
                                    }
                                    break;
                            }
                        }
                    }
                    break;
            }
            //----------------------------------
            //summary formatting context
            //that will be used on layout process 
            //---------------------------------- 
        }

        CssBox CreateBoxInternal(HtmlElement parentElement, HtmlElement childElement, bool fullmode)
        {
            CssBox hostBox = HtmlElement.InternalGetPrincipalBox(parentElement);
            return CreateBoxInternal(hostBox, childElement, fullmode);
        }
        CssBox CreateBoxInternal(CssBox parentBox, HtmlElement childElement, bool fullmode)
        {
            //----------------------------------------- 
            //1. create new box
            //----------------------------------------- 
            //some box has predefined behaviour 
            CssBox newBox = null;
            switch (childElement.WellknownElementName)
            {
                case WellKnownDomNodeName.br:
                    //special treatment for br
                    newBox = new CssBox(childElement.Spec, parentBox.RootGfx);
                    newBox.SetController(childElement);
                    CssBox.SetAsBrBox(newBox);
                    CssBox.ChangeDisplayType(newBox, CssDisplay.Block);
                    parentBox.AppendChild(newBox);
                    childElement.SetPrincipalBox(newBox);
                    return newBox;
                case WellKnownDomNodeName.img:

                    //auto append newBox to parentBox
                    newBox = CreateImageBox(parentBox, childElement);
                    childElement.SetPrincipalBox(newBox);
                    return newBox;
                case WellKnownDomNodeName.hr:
                    newBox = new CssBoxHr(childElement.Spec, parentBox.RootGfx);
                    newBox.SetController(childElement);
                    parentBox.AppendChild(newBox);
                    childElement.SetPrincipalBox(newBox);
                    return newBox;
                //-----------------------------------------------------
                //TODO: simplify this ...
                //table-display elements, fix display type
                case WellKnownDomNodeName.td:
                case WellKnownDomNodeName.th:
                    newBox = TableBoxCreator.CreateTableCell(parentBox, childElement, true);
                    break;
                case WellKnownDomNodeName.col:
                    newBox = TableBoxCreator.CreateTableColumnOrColumnGroup(parentBox, childElement, true, CssDisplay.TableColumn);
                    break;
                case WellKnownDomNodeName.colgroup:
                    newBox = TableBoxCreator.CreateTableColumnOrColumnGroup(parentBox, childElement, true, CssDisplay.TableColumnGroup);
                    break;
                case WellKnownDomNodeName.tr:
                    newBox = TableBoxCreator.CreateOtherPredefinedTableElement(parentBox, childElement, CssDisplay.TableRow);
                    break;
                case WellKnownDomNodeName.tbody:
                    newBox = TableBoxCreator.CreateOtherPredefinedTableElement(parentBox, childElement, CssDisplay.TableRowGroup);
                    break;
                case WellKnownDomNodeName.table:
                    newBox = TableBoxCreator.CreateOtherPredefinedTableElement(parentBox, childElement, CssDisplay.Table);
                    break;
                case WellKnownDomNodeName.caption:
                    newBox = TableBoxCreator.CreateOtherPredefinedTableElement(parentBox, childElement, CssDisplay.TableCaption);
                    break;
                case WellKnownDomNodeName.thead:
                    newBox = TableBoxCreator.CreateOtherPredefinedTableElement(parentBox, childElement, CssDisplay.TableHeaderGroup);
                    break;
                case WellKnownDomNodeName.tfoot:
                    newBox = TableBoxCreator.CreateOtherPredefinedTableElement(parentBox, childElement, CssDisplay.TableFooterGroup);
                    break;
                //---------------------------------------------------
                case WellKnownDomNodeName.select:
                    newBox = this.CreateCustomCssBox(parentBox, childElement, childElement.Spec);
                    if (newBox != null)
                    {
                        childElement.SetPrincipalBox(newBox);
                        return newBox;
                    }
                    goto default; //else goto default ***  
                case WellKnownDomNodeName.canvas:
                case WellKnownDomNodeName.input:

                    newBox = this.CreateCustomCssBox(parentBox, childElement, childElement.Spec);
                    if (newBox != null)
                    {
                        childElement.SetPrincipalBox(newBox);
                        return newBox;
                    }
                    goto default; //else goto default *** 
                //---------------------------------------------------
                case WellKnownDomNodeName.svg:
                    {
                        //1. create svg container node 
                        newBox = _svgCreator.CreateSvgBox(parentBox, childElement, childElement.Spec);
                        childElement.SetPrincipalBox(newBox);
                        return newBox;
                    }
                case WellKnownDomNodeName.NotAssign:
                case WellKnownDomNodeName.Unknown:
                    {
                        //custom tag
                        //check if this is tag is registered as custom element
                        //-----------------------------------------------
                        if (childElement.HasCustomPrincipalBoxGenerator)
                        {
                            var childbox = childElement.GetPrincipalBox(parentBox, this);
                            parentBox.AppendChild(childbox);
                            return childbox;
                        }

                        //----------------------------------------------- 
                        LayoutFarm.Composers.CreateCssBoxDelegate foundBoxGen;
                        if (((HtmlDocument)childElement.OwnerDocument).TryGetCustomBoxGenerator(childElement.Name, out foundBoxGen))
                        {
                            //create custom box 
                            newBox = foundBoxGen(childElement, parentBox, childElement.Spec, this);
                        }
                        if (newBox == null)
                        {
                            goto default;
                        }
                        else
                        {
                            childElement.SetPrincipalBox(newBox);
                            return newBox;
                        }
                    }
                default:
                    {
                        BoxSpec childSpec = childElement.Spec;
                        switch (childSpec.CssDisplay)
                        {
                            //not fixed display type
                            case CssDisplay.TableCell:
                                newBox = TableBoxCreator.CreateTableCell(parentBox, childElement, false);
                                break;
                            case CssDisplay.TableColumn:
                                newBox = TableBoxCreator.CreateTableColumnOrColumnGroup(parentBox, childElement, false, CssDisplay.TableColumn);
                                break;
                            case CssDisplay.TableColumnGroup:
                                newBox = TableBoxCreator.CreateTableColumnOrColumnGroup(parentBox, childElement, false, CssDisplay.TableColumnGroup);
                                break;
                            case CssDisplay.ListItem:
                                newBox = ListItemBoxCreator.CreateListItemBox(parentBox, childElement);
                                break;
                            default:
                                newBox = new CssBox(childSpec, parentBox.RootGfx);
                                newBox.SetController(childElement);
                                parentBox.AppendChild(newBox);
                                break;
                        }
                    }
                    break;
            }

            if (childElement.WellknownElementName == WellKnownDomNodeName.body)
            {
                newBox.IsBody = true;
            }

            childElement.SetPrincipalBox(newBox);
            UpdateChildBoxes(childElement, fullmode);
            return newBox;
        }

        CssBox CreateImageBox(CssBox parent, HtmlElement childElement)
        {
            string imgsrc;
            ImageBinder imgBinder = null;
            if (childElement.TryGetAttribute(WellknownName.Src, out imgsrc))
            {
                var clientImageBinder = new ClientImageBinder(imgsrc);
                imgBinder = clientImageBinder;
                //clientImageBinder.SetOwner(childElement);
            }
            else
            {
                var clientImageBinder = new ClientImageBinder(null);
                imgBinder = clientImageBinder;
                //clientImageBinder.SetOwner(childElement);
            }

            CssBoxImage boxImage = new CssBoxImage(childElement.Spec, parent.RootGfx, imgBinder);
            boxImage.SetController(childElement);
            parent.AppendChild(boxImage);
            return boxImage;
        }


        CssBox CreateCustomCssBox(CssBox parent,
          LayoutFarm.WebDom.DomElement tag,
          BoxSpec boxspec)
        {
            for (int i = generators.Count - 1; i >= 0; --i)
            {
                CssBox newbox = generators[i].CreateCssBox(tag, parent, boxspec, this);
                if (newbox != null)
                {
                    return newbox;
                }
            }
            return null;
        }


        internal static CssBox CreateBridgeBox(ITextService iFonts, LayoutFarm.RenderElement containerElement, RootGraphic rootgfx)
        {
            var spec = new BoxSpec();
            spec.CssDisplay = CssDisplay.Block;
            spec.Freeze();
            var box = new RenderElementBridgeCssBox(spec, containerElement, rootgfx);
            //------------------------------------
            box.ReEvaluateFont(iFonts, 10);
            //------------------------------------
            return box;
        }
        internal static CssBox CreateIsolateBox(ITextService iFonts, RootGraphic rootgfx)
        {
            var spec = new BoxSpec();
            spec.CssDisplay = CssDisplay.Block;
            spec.Freeze();
            var box = new CssIsolateBox(spec, rootgfx);
            //------------------------------------
            box.ReEvaluateFont(iFonts, 10);
            //------------------------------------
            return box;
        }
    }
}