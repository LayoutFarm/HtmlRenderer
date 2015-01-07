//BSD 2014, WinterDev 
using System.Collections.Generic;
using System.Globalization;
using PixelFarm.Drawing;
using LayoutFarm.WebDom;
using LayoutFarm.Css;
using LayoutFarm.HtmlBoxes;

namespace LayoutFarm.Composers.BridgeHtml
{
    public struct BoxCreator
    {

        static List<CustomCssBoxGenerator> generators = new List<CustomCssBoxGenerator>();
        LayoutFarm.RootGraphic rootgfx;
        internal BoxCreator(LayoutFarm.RootGraphic rootgfx)
        {
            this.rootgfx = rootgfx;

        }

        public static void RegisterCustomCssBoxGenerator(CustomCssBoxGenerator generator)
        {
            generators.Add(generator);
        }
        static CssBox CreateImageBox(CssBox parent, HtmlElement childElement)
        {
            string imgsrc;
            ImageBinder imgBinder = null;
            if (childElement.TryGetAttribute(WellknownName.Src, out imgsrc))
            {
                imgBinder = new ImageBinder(imgsrc);
            }
            else
            {
                imgBinder = new ImageBinder(null);
            }
            CssBoxImage boxImage = new CssBoxImage(childElement, childElement.Spec, imgBinder);
            parent.AppendChild(boxImage);
            return boxImage;
        }


       
        internal void GenerateChildBoxes(HtmlElement parentElement, bool fullmode)
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
                        DomNode bridgeChild = parentElement.GetChildNode(0);
                        int newBox = 0;
                        switch (bridgeChild.NodeType)
                        {
                            case HtmlNodeType.TextNode:
                                {

                                    HtmlTextNode singleTextNode = (HtmlTextNode)bridgeChild;
                                    RunListHelper.AddRunList(hostBox, parentElement.Spec, singleTextNode);

                                } break;
                            case HtmlNodeType.ShortElement:
                            case HtmlNodeType.OpenElement:
                                {

                                    HtmlElement childElement = (HtmlElement)bridgeChild;
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
                                            GenerateChildBoxes(childElement, fullmode);
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
                                                GenerateChildBoxes(childElement, fullmode);
                                            }
                                        }
                                        else
                                        {
                                            //just insert                                                 
                                            hostBox.AppendChild(existing);
                                            if (!childElement.SkipPrincipalBoxEvalulation)
                                            {
                                                existing.Clear();
                                                GenerateChildBoxes(childElement, fullmode);
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
                                                GenerateChildBoxes(childElement, fullmode);
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
                                                    GenerateChildBoxes(childElement, fullmode);
                                                }
                                            }
                                            else
                                            {
                                                //just insert           
                                                hostBox.AppendChild(existingCssBox);
                                                if (!childElement.SkipPrincipalBoxEvalulation)
                                                {
                                                    existingCssBox.Clear();
                                                    GenerateChildBoxes(childElement, fullmode);
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

        internal CssBox CreateBox(CssBox parentBox, HtmlElement childElement, out bool alreadyHandleChildrenNodes)
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
                    newBox = new CssBox(childElement, childElement.Spec);
                    parentBox.AppendChild(newBox);

                    CssBox.SetAsBrBox(newBox);
                    CssBox.ChangeDisplayType(newBox, CssDisplay.Block);
                    return newBox;
                case WellKnownDomNodeName.img:

                    return CreateImageBox(parentBox, childElement);
                case WellKnownDomNodeName.hr:

                    newBox = new CssBoxHr(childElement, childElement.Spec);
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
                //test extension box
                case WellKnownDomNodeName.X:
                    {
                        alreadyHandleChildrenNodes = true;
                        newBox = CreateCustomBox(parentBox, childElement, childElement.Spec);
                        if (newBox == null)
                        {
                            goto default;
                        }
                        return newBox;
                    }
                //---------------------------------------------------
                case WellKnownDomNodeName.svg:
                    {
                        //1. create svg container node
                        alreadyHandleChildrenNodes = true;
                        return SvgCreator.CreateSvgBox(parentBox, childElement, childElement.Spec);
                    }
                //---------------------------------------------------
                default:
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
                            newBox = new CssBox(childElement, childSpec);
                            parentBox.AppendChild(newBox);
                            return newBox;
                    }
            }
        }

        CssBox CreateCustomBox(CssBox parent, object tag, BoxSpec boxspec)
        {
            for (int i = generators.Count - 1; i >= 0; --i)
            {
                var newbox = generators[i].CreateCssBox(tag, parent, boxspec, this.rootgfx);
                if (newbox != null)
                {
                    return newbox;
                }
            }
            return null;
        }

        internal static CssBox CreateCssRenderRoot(IFonts iFonts, LayoutFarm.RenderElement containerElement)
        {
            var spec = new BoxSpec();
            spec.CssDisplay = CssDisplay.Block;
            spec.Freeze();
            var box = new CssRenderRoot(spec, containerElement);
            //------------------------------------
            box.ReEvaluateFont(iFonts, 10);
            //------------------------------------
            return box;
        }
    }

    class CssRenderRoot : CssBox
    {
        LayoutFarm.RenderElement containerElement;
        public CssRenderRoot(BoxSpec spec, LayoutFarm.RenderElement containerElement)
            : base(null, spec)
        {
            this.containerElement = containerElement;
        }
        public LayoutFarm.RenderElement ContainerElement
        {
            get { return this.containerElement; }
        } 
    }

    static class TableBoxCreator
    {

        public static CssBox CreateOtherPredefinedTableElement(CssBox parent,
            HtmlElement childElement, CssDisplay selectedCssDisplayType)
        {
            var newBox = new CssBox(childElement, childElement.Spec, selectedCssDisplayType);
            parent.AppendChild(newBox);
            return newBox;
        }
        public static CssBox CreateTableColumnOrColumnGroup(CssBox parent,
            HtmlElement childElement, bool fixDisplayType, CssDisplay selectedCssDisplayType)
        {
            CssBox col = null;
            if (fixDisplayType)
            {
                col = new CssBox(childElement, childElement.Spec, selectedCssDisplayType);
            }
            else
            {
                col = new CssBox(childElement, childElement.Spec);

            }
            parent.AppendChild(col);

            string spanValue;
            int spanNum = 1;//default
            if (childElement.TryGetAttribute(WellknownName.Span, out spanValue))
            {
                if (!int.TryParse(spanValue, out spanNum))
                {
                    spanNum = 1;
                }
                if (spanNum < 0)
                {
                    spanNum = -spanNum;
                }
            }

            col.SetRowSpanAndColSpan(1, spanNum);
            return col;
        }
        public static CssBox CreateTableCell(CssBox parent, HtmlElement childElement, bool fixDisplayType)
        {
            CssBox tableCell = null;
            if (fixDisplayType)
            {
                tableCell = new CssBox(childElement, childElement.Spec, CssDisplay.TableCell);
            }
            else
            {
                tableCell = new CssBox(childElement, childElement.Spec);
            }
            parent.AppendChild(tableCell);
            //----------------------------------------------------------------------------------------------


            //----------------------------------------------------------------------------------------------
            //get rowspan and colspan here 
            int nRowSpan = 1;
            int nColSpan = 1;
            string rowspan;
            if (childElement.TryGetAttribute(WellknownName.RowSpan, out rowspan))
            {
                if (!int.TryParse(rowspan, out nRowSpan))
                {
                    nRowSpan = 1;
                }
            }
            string colspan;
            if (childElement.TryGetAttribute(WellknownName.ColSpan, out colspan))
            {
                if (!int.TryParse(colspan, out nColSpan))
                {
                    nColSpan = 1;
                }
            }
            //---------------------------------------------------------- 
            tableCell.SetRowSpanAndColSpan(nRowSpan, nColSpan);
            return tableCell;
        }
    }

    static class ListItemBoxCreator
    {
        static readonly char[] discItem = new[] { '•' };
        static readonly char[] circleItem = new[] { 'o' };
        static readonly char[] squareItem = new[] { '♠' };
        static ContentTextSplitter splitter = new ContentTextSplitter();

        public static CssBox CreateListItemBox(CssBox parent, HtmlElement childElement)
        {


            var spec = childElement.Spec;
            var newBox = new CssBoxListItem(childElement, spec);

            parent.AppendChild(newBox);

            if (spec.ListStyleType != CssListStyleType.None)
            {

                //create sub item collection 
                var itemBulletBox = new CssBox(null, spec.GetAnonVersion());
                newBox.BulletBox = itemBulletBox;

                CssBox.UnsafeSetParent(itemBulletBox, newBox);
                CssBox.ChangeDisplayType(itemBulletBox, CssDisplay.Inline);
                //---------------------------------------------------------------
                //set content of bullet 
                char[] text_content = null;
                switch (spec.ListStyleType)
                {
                    case CssListStyleType.Disc:
                        {
                            text_content = discItem;
                        } break;
                    case CssListStyleType.Circle:
                        {
                            text_content = circleItem;
                        } break;
                    case CssListStyleType.Square:
                        {
                            text_content = squareItem;
                        } break;
                    case CssListStyleType.Decimal:
                        {
                            text_content = (GetIndexForList(newBox, childElement).ToString(CultureInfo.InvariantCulture) + ".").ToCharArray();
                        } break;
                    case CssListStyleType.DecimalLeadingZero:
                        {
                            text_content = (GetIndexForList(newBox, childElement).ToString("00", CultureInfo.InvariantCulture) + ".").ToCharArray();
                        } break;
                    default:
                        {
                            text_content = (BulletNumberFormatter.ConvertToAlphaNumber(GetIndexForList(newBox, childElement), spec.ListStyleType) + ".").ToCharArray();
                        } break;
                }
                //--------------------------------------------------------------- 
                CssBox.UnsafeSetTextBuffer(itemBulletBox, text_content);

                List<CssRun> runlist;
                bool hasSomeCharacter;
                splitter.ParseWordContent(text_content, spec, out runlist, out  hasSomeCharacter);

                RunListHelper.AddRunList(itemBulletBox, spec, runlist, text_content, false);

            }
            return newBox;
        }
        /// <summary>
        /// Gets the index of the box to be used on a (ordered) list
        /// </summary>
        /// <returns></returns>
        static int GetIndexForList(CssBox box, HtmlElement childElement)
        {

            HtmlElement parentNode = childElement.ParentNode as HtmlElement;
            int index = 1;

            string reversedAttrValue;
            bool reversed = false;
            if (parentNode.TryGetAttribute(WellknownName.Reversed, out reversedAttrValue))
            {
                reversed = true;
            }
            string startAttrValue;
            if (!parentNode.TryGetAttribute(WellknownName.Start, out startAttrValue))
            {
                //if not found
                //TODO: not to loop count ?

                if (reversed)
                {
                    index = 0;
                    foreach (CssBox b in box.ParentBox.GetChildBoxIter())
                    {
                        if (b.CssDisplay == CssDisplay.ListItem)
                        {
                            index++;
                        }
                    }
                }
                else
                {
                    index = 1;
                }
            }
            foreach (CssBox b in box.ParentBox.GetChildBoxIter())
            {
                if (b == box)
                    return index;
                if (b.CssDisplay == CssDisplay.ListItem)
                    index += reversed ? -1 : 1;
            }
            return index;
        }
    }
}