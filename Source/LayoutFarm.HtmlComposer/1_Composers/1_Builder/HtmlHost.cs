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

        List<CustomCssBoxGenerator> _generators = new List<CustomCssBoxGenerator>();
        HtmlVisualRootUpdateHandler _visualHtmlRootUpdateHandler;
        EventHandler<ImageRequestEventArgs> _requestImage;
        EventHandler<TextRequestEventArgs> _requestStyleSheet;
        List<CssBox> _waitForUpdateBoxes = new List<CssBox>();

        HtmlDocument _commonHtmlDoc;
        RootGraphic _rootgfx;
        Queue<LayoutVisitor> _htmlLayoutVisitorStock = new Queue<LayoutVisitor>();
        RenderTreeBuilder _renderTreeBuilder;

        ITextService _textservice;
        PaintLab.Svg.SvgCreator _svgCreator;
        string _baseUrl;

        public HtmlHost(HtmlHostCreationConfig config)
        {
            //use default style sheet

#if DEBUG
            if (!config.ValidateConfig())
            {
                throw new NotSupportedException();
            }
#endif

            _commonHtmlDoc = new HtmlDocument(this);
            if (config.ActiveSheet != null)
            {
                _commonHtmlDoc.CssActiveSheet = this.BaseStylesheet = config.ActiveSheet;
            }
            else
            {
                //use default
                _commonHtmlDoc.CssActiveSheet = this.BaseStylesheet =
                     LayoutFarm.WebDom.Parser.CssParserHelper.ParseStyleSheet(null,
                     LayoutFarm.Composers.CssDefaults.DefaultCssData,
                    true);
            }
            _rootgfx = config.RootGraphic;

            _textservice = config.TextService;
            _svgCreator = new PaintLab.Svg.SvgCreator();


        }

        public string BaseUrl
        {
            get => _baseUrl;
            set => _baseUrl = value;
        }
        //----------------------
        public void ClearUpdateWaitingCssBoxes()
        {
            int j = _waitForUpdateBoxes.Count;
            for (int i = 0; i < j; ++i)
            {
                CssBox cssbox = _waitForUpdateBoxes[i];
                var controller = HtmlBoxes.CssBox.UnsafeGetController(cssbox) as UI.IUIEventListener;
                if (controller != null)
                {
                    controller.HandleElementUpdate();
                }
            }
            _waitForUpdateBoxes.Clear();
        }

        public RootGraphic RootGfx => _rootgfx;

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
            _visualHtmlRootUpdateHandler = visualHtmlRootUpdateHandler;
        }

        //---
        public HtmlDocument CreateNewDocumentFragment()
        {
            return new HtmlDocumentFragment(_commonHtmlDoc);
        }
        public HtmlDocument CreateNewSharedHtmlDoc()
        {
            //!!! this is my extension *** 
            HtmlDocument sharedDocument = new HtmlSharedDocument(_commonHtmlDoc);
            sharedDocument.SetDomUpdateHandler(_commonHtmlDoc.DomUpdateHandler);
            sharedDocument.CssActiveSheet = _commonHtmlDoc.CssActiveSheet;
            return sharedDocument;
        }

        public WebDom.CssActiveSheet BaseStylesheet { get; private set; }
        //---
        public LayoutVisitor GetSharedHtmlLayoutVisitor(HtmlVisualRoot htmlVisualRoot)
        {
            LayoutVisitor lay = null;
            if (_htmlLayoutVisitorStock.Count == 0)
            {
                lay = new LayoutVisitor(this.GetTextService());
            }
            else
            {
                lay = _htmlLayoutVisitorStock.Dequeue();
            }
            lay.Bind(htmlVisualRoot);
            return lay;
        }
        public void ReleaseHtmlLayoutVisitor(LayoutVisitor lay)
        {
            lay.UnBind();
            _htmlLayoutVisitorStock.Enqueue(lay);
        }

        public LayoutFarm.Composers.RenderTreeBuilder GetRenderTreeBuilder()
        {
            if (_renderTreeBuilder == null)
            {
                _renderTreeBuilder = new Composers.RenderTreeBuilder(this);
                _renderTreeBuilder.RequestStyleSheet += (e) =>
                {
                    if (_requestStyleSheet != null)
                    {
                        _requestStyleSheet(this, e);
                    }
                };
            }
            return _renderTreeBuilder;
        }
        public void RegisterCssBoxGenerator(LayoutFarm.Composers.CustomCssBoxGenerator cssBoxGenerator)
        {
            _generators.Add(cssBoxGenerator);
        }
        public void UpdateChildBoxes(WebDom.Impl.HtmlElement parentElement, bool fullmode)
        {
            //for public 
            UpdateChildBoxes((HtmlElement)parentElement, fullmode);
        }
        public CssBox CreateCssBox(CssBox parentBox, WebDom.Impl.HtmlElement childElement, bool fullmode)
        {
            return CreateCssBoxInternal(parentBox, (HtmlElement)childElement, fullmode);
        }
        //-------------

        //
        internal void ChildRequestImage(ImageBinder binder, object reqFrom)
        {
            if (_requestImage != null)
            {
                ImageRequestEventArgs resReq = new ImageRequestEventArgs(binder);
                resReq.requestBy = reqFrom;
                _requestImage(this, resReq);
            }
        }

        internal ITextService GetTextService() => _textservice;

        internal void EnqueueCssUpdate(CssBox box)
        {
            _waitForUpdateBoxes.Add(box);
        }
        internal void NotifyHtmlVisualRootUpdate(HtmlVisualRoot htmlVisualRoot)
        {
            _visualHtmlRootUpdateHandler?.Invoke(htmlVisualRoot);
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
                                        _renderTreeBuilder.UpdateTextNode(parentElement, singleTextNode, isblockContext);
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
                                            CssBox box = CreateCssBoxInternal(hostBox, childElement, fullmode);
                                        }
                                        else
                                        {
                                            CssBox existingCssBox = HtmlElement.InternalGetPrincipalBox(childElement);
                                            if (existingCssBox == null)
                                            {
                                                CreateCssBoxInternal(hostBox, childElement, fullmode);
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
            return CreateCssBoxInternal(HtmlElement.InternalGetPrincipalBox(parentElement), childElement, fullmode);
        }
        CssBox CreateCssBoxInternal(CssBox parentBox, HtmlElement childElement, bool fullmode)
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
                    newBox = new CssBox(childElement.Spec);
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
                    newBox = new CssBoxHr(childElement.Spec);
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
                case WellKnownDomNodeName.canvas:
                case WellKnownDomNodeName.input:
                case WellKnownDomNodeName.textarea:
                    newBox = CreateCustomCssBox(parentBox, childElement, childElement.Spec);
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
                                newBox = new CssBox(childSpec);
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
                var clientImageBinder = new ImageBinder(imgsrc);
                imgBinder = clientImageBinder;

            }
            else
            {
                var clientImageBinder = new ImageBinder(null as string);
                imgBinder = clientImageBinder;
            }

            CssBoxImage boxImage = new CssBoxImage(childElement.Spec,  imgBinder);
            boxImage.SetController(childElement);
            parent.AppendChild(boxImage);
            return boxImage;
        }


        CssBox CreateCustomCssBox(CssBox parent,
          HtmlElement htmlElem,
          BoxSpec boxspec)
        {
            for (int i = _generators.Count - 1; i >= 0; --i)
            {
                CssBox newbox = _generators[i].CreateCssBox(htmlElem, parent, boxspec, this);
                if (newbox != null)
                {
                    return newbox;
                }
            }
            return null;
        }


        internal static CssBox CreateBridgeBox(ITextService iFonts, LayoutFarm.RenderElement containerElement)
        {
            var spec = new BoxSpec();
            spec.CssDisplay = CssDisplay.Block;
            spec.Freeze();
            var box = new RenderElementBridgeCssBox(spec, containerElement);
            //------------------------------------
            box.ReEvaluateFont(iFonts, 10);
            //------------------------------------
            return box;
        }
        internal static CssBox CreateIsolateBox(ITextService iFonts)
        {
            var spec = new BoxSpec();
            spec.CssDisplay = CssDisplay.Block;
            spec.Freeze();
            var box = new CssIsolateBox(spec);
            //------------------------------------
            box.ReEvaluateFont(iFonts, 10);
            //------------------------------------
            return box;
        }
    }
}