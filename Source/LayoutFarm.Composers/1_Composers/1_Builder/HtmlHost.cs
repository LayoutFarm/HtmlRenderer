//BSD 2014-2015 ,WinterDev
//ArthurHub  , Jose Manuel Menendez Poo

using System;
using System.Collections.Generic;

using System.Text;
using System.Diagnostics;
using PixelFarm.Drawing;
using LayoutFarm.WebDom;
using LayoutFarm.ContentManagers;
using LayoutFarm.UI;
using LayoutFarm.Css;

namespace LayoutFarm.HtmlBoxes
{

    public class HtmlHost
    {

        List<LayoutFarm.Composers.CustomCssBoxGenerator> generators = new List<LayoutFarm.Composers.CustomCssBoxGenerator>();

        HtmlContainerUpdateHandler htmlContainerUpdateHandler;

        EventHandler<ImageRequestEventArgs> requestImage;
        EventHandler<TextRequestEventArgs> requestStyleSheet;

        GraphicsPlatform gfxplatform;
        HtmlDocument commonHtmlDoc;
        RootGraphic rootgfx;

        public HtmlHost(GraphicsPlatform gfxplatform, WebDom.CssActiveSheet activeSheet)
        {

            this.gfxplatform = gfxplatform;
            this.BaseStylesheet = activeSheet;
            this.commonHtmlDoc = new HtmlDocument();
            this.commonHtmlDoc.CssActiveSheet = activeSheet;

        }
        public HtmlHost(GraphicsPlatform gfxplatform)
            : this(gfxplatform, LayoutFarm.WebDom.Parser.CssParserHelper.ParseStyleSheet(null,
              LayoutFarm.Composers.CssDefaults.DefaultCssData,
             true))
        {
            //use default style sheet
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
        public GraphicsPlatform GfxPlatform { get { return this.gfxplatform; } }
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

        public FragmentHtmlDocument CreateNewFragmentHtml()
        {
            return new FragmentHtmlDocument(this.commonHtmlDoc);
        }

        //------------------------         
        Queue<LayoutFarm.HtmlBoxes.LayoutVisitor> htmlLayoutVisitorStock = new Queue<LayoutVisitor>();
        LayoutFarm.Composers.RenderTreeBuilder renderTreeBuilder;

        public LayoutFarm.HtmlBoxes.LayoutVisitor GetSharedHtmlLayoutVisitor(HtmlContainer htmlCont)
        {
            LayoutFarm.HtmlBoxes.LayoutVisitor lay = null;
            if (htmlLayoutVisitorStock.Count == 0)
            {
                lay = new LayoutVisitor(this.gfxplatform);
            }
            else
            {
                lay = this.htmlLayoutVisitorStock.Dequeue();
            }
            lay.Bind(htmlCont);
            return lay;
        }
        public void ReleaseHtmlLayoutVisitor(LayoutFarm.HtmlBoxes.LayoutVisitor lay)
        {
            lay.UnBind();
            this.htmlLayoutVisitorStock.Enqueue(lay);
        }

        public HtmlInputEventAdapter GetNewInputEventAdapter()
        {
            return new HtmlInputEventAdapter(this.gfxplatform.SampleIFonts);
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

        //--------------------------------------------------- 

        public bool AlreadyRegisterCssBoxGen(Type t)
        {
            for (int i = generators.Count - 1; i >= 0; --i)
            {
                if (generators[i].GetType() == t)
                {
                    return true;
                }
            }
            return false;
        }
        public void RegisterCssBoxGenerator(LayoutFarm.Composers.CustomCssBoxGenerator cssBoxGenerator)
        {
            this.generators.Add(cssBoxGenerator);
        }

        public CssBox CreateCustomBox(CssBox parent,
            LayoutFarm.WebDom.DomElement tag,
            LayoutFarm.Css.BoxSpec boxspec,
            RootGraphic rootgfx,
            out bool alreadyHandleChildrenNodes)
        {

            for (int i = generators.Count - 1; i >= 0; --i)
            {
                var newbox = generators[i].CreateCssBox(tag, parent, boxspec, rootgfx, out alreadyHandleChildrenNodes);
                if (newbox != null)
                {

                    return newbox;
                }
            }
            alreadyHandleChildrenNodes = false;
            return null;
        }

        /// update some or generate all cssbox
        /// </summary>
        /// <param name="parentElement"></param>
        /// <param name="fullmode">update all nodes (true) or somenode (false)</param>
        public void UpdateChildBoxes(HtmlElement parentElement, bool fullmode)
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

                                } break;
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
                                        bool alreadyHandleChildrenNode;
                                        CssBox newbox = CreateBox(hostBox, childElement, out alreadyHandleChildrenNode);
                                        childElement.SetPrincipalBox(newbox);

                                        if (!alreadyHandleChildrenNode)
                                        {
                                            UpdateChildBoxes(childElement, fullmode);
                                        }
                                    }
                                    else
                                    {
                                        CssBox existing = HtmlElement.InternalGetPrincipalBox(childElement);
                                        if (existing == null)
                                        {
                                            bool alreadyHandleChildrenNode;
                                            CssBox box = CreateBox(hostBox, childElement, out alreadyHandleChildrenNode);
                                            childElement.SetPrincipalBox(box);
                                            if (!alreadyHandleChildrenNode)
                                            {
                                                UpdateChildBoxes(childElement, fullmode);
                                            }
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
                                } break;
                        }
                    } break;
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
                                                } break;
                                            case CssWhiteSpace.PreLine:
                                                {
                                                    if (newBox == 0 && textNode.IsWhiteSpace)
                                                    {
                                                        continue;//skip
                                                    }

                                                    RunListHelper.AddRunList(
                                                        CssBox.AddNewAnonInline(hostBox),
                                                        parentElement.Spec, textNode);

                                                } break;
                                            default:
                                                {
                                                    if (textNode.IsWhiteSpace)
                                                    {
                                                        continue;//skip
                                                    }
                                                    RunListHelper.AddRunList(
                                                        CssBox.AddNewAnonInline(hostBox),
                                                        parentElement.Spec, textNode);
                                                } break;
                                        }

                                        newBox++;

                                    } break;
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
                                            bool alreadyHandleChildrenNode;
                                            CssBox box = CreateBox(hostBox, childElement, out alreadyHandleChildrenNode);

                                            childElement.SetPrincipalBox(box);

                                            if (!alreadyHandleChildrenNode)
                                            {
                                                UpdateChildBoxes(childElement, fullmode);
                                            }
                                        }
                                        else
                                        {

                                            CssBox existingCssBox = HtmlElement.InternalGetPrincipalBox(childElement);
                                            if (existingCssBox == null)
                                            {
                                                bool alreadyHandleChildrenNode;
                                                CssBox box = CreateBox(hostBox, childElement, out alreadyHandleChildrenNode);

                                                childElement.SetPrincipalBox(box);

                                                if (!alreadyHandleChildrenNode)
                                                {
                                                    UpdateChildBoxes(childElement, fullmode);
                                                }
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
                                    } break;
                                default:
                                    {
                                    } break;
                            }
                        }

                    } break;
            }
            //----------------------------------
            //summary formatting context
            //that will be used on layout process 
            //----------------------------------

        }

        public CssBox CreateBox(CssBox parentBox, HtmlElement childElement, out bool alreadyHandleChildrenNodes)
        {

            alreadyHandleChildrenNodes = false;
            CssBox newBox = null;
            //----------------------------------------- 
            //1. create new box
            //----------------------------------------- 
            //some box has predefined behaviour
            switch (childElement.WellknownElementName)
            {

                case WellKnownDomNodeName.br:
                    //special treatment for br
                    newBox = new CssBox(childElement, childElement.Spec, parentBox.RootGfx);
                    parentBox.AppendChild(newBox);
                    CssBox.SetAsBrBox(newBox);
                    CssBox.ChangeDisplayType(newBox, CssDisplay.Block);
                    return newBox;

                case WellKnownDomNodeName.img:
                    return CreateImageBox(parentBox, childElement);
                case WellKnownDomNodeName.hr:

                    newBox = new CssBoxHr(childElement, childElement.Spec, parentBox.RootGfx);
                    parentBox.AppendChild(newBox);
                    return newBox;
                //-----------------------------------------------------
                //TODO: simplify this ...
                //table-display elements, fix display type
                case WellKnownDomNodeName.td:
                case WellKnownDomNodeName.th:
                    newBox = TableBoxCreator.CreateTableCell(parentBox, childElement, true);
                    return newBox;
                case WellKnownDomNodeName.col:
                    return TableBoxCreator.CreateTableColumnOrColumnGroup(parentBox, childElement, true, CssDisplay.TableColumn);
                case WellKnownDomNodeName.colgroup:
                    return TableBoxCreator.CreateTableColumnOrColumnGroup(parentBox, childElement, true, CssDisplay.TableColumnGroup);
                case WellKnownDomNodeName.tr:
                    return TableBoxCreator.CreateOtherPredefinedTableElement(parentBox, childElement, CssDisplay.TableRow);
                case WellKnownDomNodeName.tbody:
                    return TableBoxCreator.CreateOtherPredefinedTableElement(parentBox, childElement, CssDisplay.TableRowGroup);
                case WellKnownDomNodeName.table:
                    return TableBoxCreator.CreateOtherPredefinedTableElement(parentBox, childElement, CssDisplay.Table);
                case WellKnownDomNodeName.caption:
                    return TableBoxCreator.CreateOtherPredefinedTableElement(parentBox, childElement, CssDisplay.TableCaption);
                case WellKnownDomNodeName.thead:
                    return TableBoxCreator.CreateOtherPredefinedTableElement(parentBox, childElement, CssDisplay.TableHeaderGroup);
                case WellKnownDomNodeName.tfoot:
                    return TableBoxCreator.CreateOtherPredefinedTableElement(parentBox, childElement, CssDisplay.TableFooterGroup);
                //---------------------------------------------------
                case WellKnownDomNodeName.canvas:
                case WellKnownDomNodeName.input:
                    //----------------------------------------------- 
                    newBox = this.CreateCustomBox(parentBox, childElement, childElement.Spec, rootgfx, out alreadyHandleChildrenNodes);
                    if (newBox != null)
                    {
                        alreadyHandleChildrenNodes = true;
                        return newBox;
                    }
                    goto default; //else goto default *** 
                //---------------------------------------------------
                case WellKnownDomNodeName.svg:
                    {
                        //1. create svg container node
                        alreadyHandleChildrenNodes = true;
                        return Svg.SvgCreator.CreateSvgBox(parentBox, childElement, childElement.Spec);
                    }
                case WellKnownDomNodeName.NotAssign:
                case WellKnownDomNodeName.Unknown:
                    {
                        //custom tag
                        //check if this is tag is registered as custom element
                        //-----------------------------------------------
                        ExternalHtmlElement externalHtmlElement = childElement as ExternalHtmlElement;
                        if (externalHtmlElement != null)
                        {
                            newBox = externalHtmlElement.GetCssBox(rootgfx);
                            if (newBox != null)
                            {
                                parentBox.AppendChild(newBox);
                                return newBox;
                            }
                        }
                        //----------------------------------------------- 
                        LayoutFarm.WebDom.CreateCssBoxDelegate foundBoxGen;
                        if (((HtmlDocument)childElement.OwnerDocument).TryGetCustomBoxGenerator(childElement.Name, out foundBoxGen))
                        {
                            //create custom box
                            newBox = foundBoxGen(childElement, parentBox,
                                childElement.Spec, this, rootgfx,
                                out alreadyHandleChildrenNodes);
                        }
                        if (newBox == null)
                        {
                            goto default;
                        }
                        else
                        {
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
                                return TableBoxCreator.CreateTableCell(parentBox, childElement, false);
                            case CssDisplay.TableColumn:
                                return TableBoxCreator.CreateTableColumnOrColumnGroup(parentBox, childElement, false, CssDisplay.TableColumn);
                            case CssDisplay.TableColumnGroup:
                                return TableBoxCreator.CreateTableColumnOrColumnGroup(parentBox, childElement, false, CssDisplay.TableColumnGroup);
                            case CssDisplay.ListItem:
                                return ListItemBoxCreator.CreateListItemBox(parentBox, childElement);
                            default:
                                newBox = new CssBox(childElement, childSpec, parentBox.RootGfx);
                                parentBox.AppendChild(newBox);
                                return newBox;
                        }
                    }
            }
        }

        public CssBox CreateImageBox(CssBox parent, HtmlElement childElement)
        {
            string imgsrc;
            ImageBinder imgBinder = null;
            if (childElement.TryGetAttribute(WellknownName.Src, out imgsrc))
            {
                var clientImageBinder = new ClientImageBinder(imgsrc);
                imgBinder = clientImageBinder;
                clientImageBinder.SetOwner(childElement);
            }
            else
            {
                var clientImageBinder = new ClientImageBinder(null);
                imgBinder = clientImageBinder;
                clientImageBinder.SetOwner(childElement);
            }

            CssBoxImage boxImage = new CssBoxImage(childElement, childElement.Spec, parent.RootGfx, imgBinder);

            parent.AppendChild(boxImage);
            return boxImage;
        }
        internal static CssBox CreateCssRenderRoot(IFonts iFonts, LayoutFarm.RenderElement containerElement, RootGraphic rootgfx)
        {
            var spec = new BoxSpec();
            spec.CssDisplay = CssDisplay.Block;
            spec.Freeze();
            var box = new CssRenderRoot(spec, containerElement, rootgfx);
            //------------------------------------
            box.ReEvaluateFont(iFonts, 10);
            //------------------------------------
            return box;
        }
        internal static CssBox CreateCssIsolateBox(IFonts iFonts, LayoutFarm.RenderElement containerElement, RootGraphic rootgfx)
        {
            var spec = new BoxSpec();
            spec.CssDisplay = CssDisplay.Block;
            spec.Freeze();
            var box = new CssIsolateBox(spec, containerElement, rootgfx);
            //------------------------------------
            box.ReEvaluateFont(iFonts, 10);
            //------------------------------------
            return box;
        }


    }
}