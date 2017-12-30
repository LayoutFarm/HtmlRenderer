//BSD, 2014-2017, WinterDev
//ArthurHub  , Jose Manuel Menendez Poo

using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
using LayoutFarm.ContentManagers;
using LayoutFarm.Composers;
using LayoutFarm.Css;
using LayoutFarm.WebDom;
namespace LayoutFarm.HtmlBoxes
{
    public class HtmlHost
    {
        List<LayoutFarm.Composers.CustomCssBoxGenerator> generators = new List<LayoutFarm.Composers.CustomCssBoxGenerator>();
        HtmlContainerUpdateHandler htmlContainerUpdateHandler;
        EventHandler<ImageRequestEventArgs> requestImage;
        EventHandler<TextRequestEventArgs> requestStyleSheet;
        List<CssBox> waitForUpdateBoxes = new List<CssBox>();

        HtmlDocument commonHtmlDoc;
        RootGraphic rootgfx;
        Queue<LayoutFarm.HtmlBoxes.LayoutVisitor> htmlLayoutVisitorStock = new Queue<LayoutVisitor>();
        LayoutFarm.Composers.RenderTreeBuilder renderTreeBuilder;

        ITextService _textservice;

        private HtmlHost(WebDom.CssActiveSheet activeSheet)
        {

            this.BaseStylesheet = activeSheet;
            this.commonHtmlDoc = new HtmlDocument();
            this.commonHtmlDoc.CssActiveSheet = activeSheet;


            this._textservice = MyFontServices.GetTextService();
        }
        public HtmlHost()
            : this(
              LayoutFarm.WebDom.Parser.CssParserHelper.ParseStyleSheet(null,
              LayoutFarm.Composers.CssDefaults.DefaultCssData,
             true))
        {
            //use default style sheet
        }
        public ITextService GetTextService()
        {
            return _textservice;
        }
        public void EnqueueCssUpdate(CssBox box)
        {
            waitForUpdateBoxes.Add(box);
        }
        public void ClearUpdateWaitingCssBoxes()
        {
            int j = waitForUpdateBoxes.Count;
            for (int i = 0; i < j; ++i)
            {
                CssBox cssbox = this.waitForUpdateBoxes[i];
                var controller = HtmlBoxes.CssBox.UnsafeGetController(cssbox) as UI.IEventListener;
                controller.HandleElementUpdate();
            }
            waitForUpdateBoxes.Clear();
        }
        public void SetRootGraphics(RootGraphic rootgfx)
        {

            this.rootgfx = rootgfx;
        }
        public RootGraphic RootGfx { get { return this.rootgfx; } }
        public RenderBoxBase TopWindowRenderBox { get { return this.rootgfx.TopWindowRenderBox; } }


        public void AttachEssentailHandlers(
            EventHandler<ImageRequestEventArgs> reqImageHandler,
            EventHandler<TextRequestEventArgs> reqStyleSheetHandler)
        {
            this.requestImage = reqImageHandler;
            this.requestStyleSheet = reqStyleSheetHandler;
        }
        public void DetachEssentailHanlders()
        {
            this.requestImage = null;
            this.requestStyleSheet = null;
        }
        public void SetHtmlContainerUpdateHandler(HtmlContainerUpdateHandler htmlContainerUpdateHandler)
        {
            this.htmlContainerUpdateHandler = htmlContainerUpdateHandler;
        }

        public WebDom.CssActiveSheet BaseStylesheet { get; private set; }


        public void ChildRequestImage(ImageBinder binder, HtmlContainer htmlCont, object reqFrom, bool _sync)
        {
            if (this.requestImage != null)
            {
                ImageRequestEventArgs resReq = new ImageRequestEventArgs(binder);
                resReq.requestBy = reqFrom;
                requestImage(this, resReq);
            }
        }
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
        public LayoutVisitor GetSharedHtmlLayoutVisitor(HtmlContainer htmlCont)
        {
            LayoutVisitor lay = null;
            if (htmlLayoutVisitorStock.Count == 0)
            {
                RootGraphic rootgfx = (RootGraphic)htmlCont.RootCssBox.GetInternalRootGfx();
                lay = new LayoutVisitor(this.GetTextService());
            }
            else
            {
                lay = this.htmlLayoutVisitorStock.Dequeue();
            }
            lay.Bind(htmlCont);
            return lay;
        }
        public void ReleaseHtmlLayoutVisitor(LayoutVisitor lay)
        {
            lay.UnBind();
            this.htmlLayoutVisitorStock.Enqueue(lay);
        }

        public HtmlInputEventAdapter GetNewInputEventAdapter()
        {
            return new HtmlInputEventAdapter();
        }


        public LayoutFarm.Composers.RenderTreeBuilder GetRenderTreeBuilder()
        {
            if (this.renderTreeBuilder == null)
            {
                renderTreeBuilder = new Composers.RenderTreeBuilder(this);
                this.renderTreeBuilder.RequestStyleSheet += (e) =>
                {
                    if (requestStyleSheet != null)
                    {
                        requestStyleSheet(this, e);
                    }
                };
            }
            return renderTreeBuilder;
        }
        internal void NotifyHtmlContainerUpdate(HtmlContainer htmlCont)
        {
            if (htmlContainerUpdateHandler != null)
            {
                htmlContainerUpdateHandler(htmlCont);
            }
        }
        public void RegisterCssBoxGenerator(LayoutFarm.Composers.CustomCssBoxGenerator cssBoxGenerator)
        {
            this.generators.Add(cssBoxGenerator);
        }

        CssBox CreateCustomCssBox(CssBox parent,
          LayoutFarm.WebDom.DomElement tag,
          LayoutFarm.Css.BoxSpec boxspec)
        {
            for (int i = generators.Count - 1; i >= 0; --i)
            {
                var newbox = generators[i].CreateCssBox(tag, parent, boxspec, this);
                if (newbox != null)
                {
                    return newbox;
                }
            }
            return null;
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
                        switch (child.NodeType)
                        {
                            case HtmlNodeType.TextNode:
                                {
                                    HtmlTextNode singleTextNode = (HtmlTextNode)child;
                                    RunListHelper.AddRunList(hostBox, parentElement.Spec, singleTextNode);
                                }
                                break;
                            case HtmlNodeType.ShortElement:
                            case HtmlNodeType.OpenElement:
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
                                        CssBox newbox = CreateBox(hostBox, childElement, fullmode);
                                        childElement.SetPrincipalBox(newbox);
                                    }
                                    else
                                    {
                                        CssBox existing = HtmlElement.InternalGetPrincipalBox(childElement);
                                        if (existing == null)
                                        {
                                            CssBox box = CreateBox(hostBox, childElement, fullmode);
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
                            var childNode = parentElement.GetChildNode(i);
                            switch (childNode.NodeType)
                            {
                                case HtmlNodeType.TextNode:
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
                                case HtmlNodeType.ShortElement:
                                case HtmlNodeType.OpenElement:
                                    {
                                        HtmlElement childElement = (HtmlElement)childNode;
                                        var spec = childElement.Spec;
                                        if (spec.CssDisplay == CssDisplay.None)
                                        {
                                            continue;
                                        }

                                        if (fullmode)
                                        {
                                            CssBox box = CreateBox(hostBox, childElement, fullmode);
                                        }
                                        else
                                        {
                                            CssBox existingCssBox = HtmlElement.InternalGetPrincipalBox(childElement);
                                            if (existingCssBox == null)
                                            {
                                                CssBox box = CreateBox(hostBox, childElement, fullmode);
                                            }
                                            else
                                            {
                                                //just insert           
                                                hostBox.AppendChild(existingCssBox);
                                                if (!childElement.SkipPrincipalBoxEvalulation)
                                                {
                                                    existingCssBox.Clear();
                                                    UpdateChildBoxes(childElement, fullmode);
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
        public CssBox CreateBox2(CssBox parentBox, WebDom.Impl.HtmlElement childElement, bool fullmode)
        {
            return CreateBox(parentBox, (HtmlElement)childElement, fullmode);
        }
        internal CssBox CreateBox(CssBox parentBox, HtmlElement childElement, bool fullmode)
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
                        newBox = Svg.SvgCreator.CreateSvgBox(parentBox, childElement, childElement.Spec);
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

        public LayoutFarm.WebDom.Impl.HtmlDocument CreatePresentationHtmlDoc()
        {
            return new HtmlDocument();
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