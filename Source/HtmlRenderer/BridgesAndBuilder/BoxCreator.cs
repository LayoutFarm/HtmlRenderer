//BSD 2014, WinterDev


using System.Collections.Generic;
using System.Globalization;
using HtmlRenderer.Utils;

namespace HtmlRenderer.Dom
{

    public abstract class CustomCssBoxGenerator
    {
        public abstract CssBox CreateCssBox(IHtmlElement tag, CssBox parentBox, BoxSpec spec);

    }

    public static class BoxCreator
    {
        static ContentTextSplitter splitter = new ContentTextSplitter();

        static List<CustomCssBoxGenerator> generators = new List<CustomCssBoxGenerator>();
        public static void RegisterCustomCssBoxGenerator(CustomCssBoxGenerator generator)
        {
            generators.Add(generator);
        }
        static CssBox CreateCustomCssBox(IHtmlElement tag, CssBox parentBox, BoxSpec spec)
        {
            int j = generators.Count;
            if (j > 0)
            {
                for (int i = j - 1; i >= 0; --i)
                {
                    var box = generators[i].CreateCssBox(tag, parentBox, spec);
                    if (box != null)
                    {
                        return box;
                    }
                }
            }
            return null;
        }

        static CssBox CreateOtherPredefinedTableElement(CssBox parent,
           BridgeHtmlElement childElement, CssDisplay selectedCssDisplayType)
        {
            return new CssBox(parent, childElement, childElement.Spec, selectedCssDisplayType);

        }

        static CssBox CreateTableColumnOrColumnGroup(CssBox parent,
            BridgeHtmlElement childElement, bool fixDisplayType, CssDisplay selectedCssDisplayType)
        {
            CssBox col = null;
            if (fixDisplayType)
            {
                col = new CssBox(parent, childElement, childElement.Spec, selectedCssDisplayType);
            }
            else
            {
                col = new CssBox(parent, childElement, childElement.Spec);

            }
            string spanValue;
            int spanNum = 1;//default
            if (childElement.TryGetAttribute2("span", out spanValue))
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

            col.SetRowColSpan(1, spanNum);
            return col;
        }
        static CssBox CreateTableCell(CssBox parent, BridgeHtmlElement childElement, bool fixDisplayType)
        {
            CssBox tableCell = null;
            if (fixDisplayType)
            {
                tableCell = new CssBox(parent, childElement, childElement.Spec, CssDisplay.TableCell);
            }
            else
            {
                tableCell = new CssBox(parent, childElement, childElement.Spec);
            }
            //get rowspan and colspan here 
            int nRowSpan = 1;
            int nColSpan = 1;
            string rowspan;
            if (childElement.TryGetAttribute2("rowspan", out rowspan))
            {
                if (!int.TryParse(rowspan, out nRowSpan))
                {
                    nRowSpan = 1;
                }
            }
            string colspan;
            if (childElement.TryGetAttribute2("colspan", out colspan))
            {
                if (!int.TryParse(colspan, out nColSpan))
                {
                    nColSpan = 1;
                }
            }
            //---------------------------------------------------------- 
            tableCell.SetRowColSpan(nRowSpan, nColSpan);
            return tableCell;
        }


        static CssBox CreateListItemBox(CssBox parent, BridgeHtmlElement childElement)
        {
            var spec = childElement.Spec;
            var newBox = new CssBox(parent, childElement, spec);

            if (spec.ListStyleType != CssListStyleType.None)
            {
                //create sub item collection
                var subBoxs = new SubBoxCollection();
                newBox.SubBoxes = subBoxs;
                var itemBulletBox = new CssBox(null, null, spec.GetAnonVersion());
                subBoxs.ListItemBulletBox = itemBulletBox;

                CssBox.UnsafeSetParent(itemBulletBox, newBox, newBox.HtmlContainer);
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
                            text_content = (CommonUtils.ConvertToAlphaNumber(GetIndexForList(newBox, childElement), spec.ListStyleType) + ".").ToCharArray();
                        } break;
                }
                //---------------------------------------------------------------
                itemBulletBox.SetTextBuffer(text_content);


                List<CssRun> runlist;
                bool hasSomeCharacter;
                itemBulletBox.SetTextBuffer(text_content);
                splitter.ParseWordContent(text_content, spec, out runlist, out  hasSomeCharacter);

                RunListHelper.AddRunList(itemBulletBox, spec, runlist, text_content, false);

            }
            return newBox;
        }
        /// <summary>
        /// Gets the index of the box to be used on a (ordered) list
        /// </summary>
        /// <returns></returns>
        static int GetIndexForList(CssBox box, BridgeHtmlElement childElement)
        {

            BridgeHtmlElement parentNode = childElement.ParentNode as BridgeHtmlElement;
            int index = 1;

            string reversedAttrValue;
            bool reversed = false;
            if (parentNode.TryGetAttribute2("reversed", out reversedAttrValue))
            {
                reversed = true;
            }
            string startAttrValue;
            if (!parentNode.TryGetAttribute2("start", out startAttrValue))
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


        static CssBox CreateImageBox(CssBox parent, BridgeHtmlElement childElement)
        {
            string imgsrc;
            ImageBinder imgBinder = null;
            if (childElement.TryGetAttribute2("src", out imgsrc))
            {
                imgBinder = new ImageBinder(imgsrc);
            }
            else
            {
                imgBinder = new ImageBinder(null);
            }
            return new CssBoxImage(parent, childElement, childElement.Spec, imgBinder);
        }

        internal static CssBox CreateBox(CssBox parentBox, BridgeHtmlElement childElement)
        {


            //case WellknownHtmlTagName.table:

            //        break;
            //    case WellknownHtmlTagName.tr:

            //        break;
            //    case WellknownHtmlTagName.tbody:

            //        break;
            //    case WellknownHtmlTagName.thead:

            //        break;
            //    case WellknownHtmlTagName.tfoot:

            //        break;
            //    case WellknownHtmlTagName.col:
            //        newdisplay = CssDisplay.TableColumn;
            //        break;
            //    case WellknownHtmlTagName.colgroup:
            //        newdisplay = CssDisplay.TableColumnGroup;
            //        break;
            //    case WellknownHtmlTagName.td:
            //    case WellknownHtmlTagName.th:
            //        newdisplay = CssDisplay.TableCell;
            //        break;
            //    case WellknownHtmlTagName.caption:
            //        newdisplay = CssDisplay.TableCaption;
            //        break;


            CssBox newBox = null;
            //----------------------------------------- 
            //1. create new box
            //----------------------------------------- 
            //some box has predefined behaviour
            switch (childElement.WellknownTagName)
            {

                case WellknownHtmlTagName.br:
                    //special treatment for br
                    newBox = new CssBox(parentBox, childElement, childElement.Spec);
                    CssBox.SetAsBrBox(newBox);
                    CssBox.ChangeDisplayType(newBox, CssDisplay.BlockInsideInlineAfterCorrection);
                    return newBox;

                case WellknownHtmlTagName.img:
                    return CreateImageBox(parentBox, childElement);

                case WellknownHtmlTagName.hr:
                    return new CssBoxHr(parentBox, childElement, childElement.Spec);

                //-----------------------------------------------------
                //TODO: simplify this ...
                //table-display elements, fix display type
                case WellknownHtmlTagName.td:
                case WellknownHtmlTagName.th:
                    newBox = CreateTableCell(parentBox, childElement, true);
                    return newBox;
                case WellknownHtmlTagName.col:
                    return CreateTableColumnOrColumnGroup(parentBox, childElement, true, CssDisplay.TableColumn);
                case WellknownHtmlTagName.colgroup:
                    return CreateTableColumnOrColumnGroup(parentBox, childElement, true, CssDisplay.TableColumnGroup);
                case WellknownHtmlTagName.tr:
                    return CreateOtherPredefinedTableElement(parentBox, childElement, CssDisplay.TableRow);
                case WellknownHtmlTagName.tbody:
                    return CreateOtherPredefinedTableElement(parentBox, childElement, CssDisplay.TableRowGroup);
                case WellknownHtmlTagName.table:
                    return CreateOtherPredefinedTableElement(parentBox, childElement, CssDisplay.Table);
                case WellknownHtmlTagName.caption:
                    return CreateOtherPredefinedTableElement(parentBox, childElement, CssDisplay.TableCaption);
                case WellknownHtmlTagName.thead:
                    return CreateOtherPredefinedTableElement(parentBox, childElement, CssDisplay.TableHeaderGroup);
                case WellknownHtmlTagName.tfoot:
                    return CreateOtherPredefinedTableElement(parentBox, childElement, CssDisplay.TableFooterGroup);
                //---------------------------------------------------
                //test extension box
                case WellknownHtmlTagName.X:
                    {
                        newBox = CreateCustomBox(parentBox, childElement, childElement.Spec);
                        if (newBox == null)
                        {
                            goto default;
                        }
                        return newBox;
                    }
                //---------------------------------------------------
                default:
                    BoxSpec childSpec = childElement.Spec;
                    switch (childSpec.CssDisplay)
                    {
                        //not fixed display type
                        case CssDisplay.TableCell:
                            return CreateTableCell(parentBox, childElement, false);
                        case CssDisplay.TableColumn:
                            return CreateTableColumnOrColumnGroup(parentBox, childElement, false, CssDisplay.TableColumn);
                        case CssDisplay.TableColumnGroup:
                            return CreateTableColumnOrColumnGroup(parentBox, childElement, false, CssDisplay.TableColumnGroup);
                        case CssDisplay.ListItem:
                            return CreateListItemBox(parentBox, childElement);
                        default:
                            return newBox = new CssBox(parentBox, childElement, childElement.Spec);
                    }
            }
        }

        static CssBox CreateCustomBox(CssBox parent, IHtmlElement tag, BoxSpec boxspec)
        {
            for (int i = generators.Count - 1; i >= 0; --i)
            {
                var newbox = generators[i].CreateCssBox(tag, parent, boxspec);
                if (newbox != null)
                {
                    return newbox;
                }
            }
            return null;
        }

        /// <summary>
        /// Create new css block box.
        /// </summary>
        /// <returns>the new block box</returns>
        internal static CssBox CreateRootBlock()
        {
            var spec = new BoxSpec();
            spec.CssDisplay = CssDisplay.Block;
            spec.Freeze();

            var box = new CssBox(null, null, spec);
            //------------------------------------
            box.ReEvaluateFont(10);
            //------------------------------------
            return box;
        }

        static readonly char[] discItem = new[] { '•' };
        static readonly char[] circleItem = new[] { 'o' };
        static readonly char[] squareItem = new[] { '♠' };

    }
}