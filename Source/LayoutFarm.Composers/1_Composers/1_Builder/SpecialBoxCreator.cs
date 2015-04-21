// 2015,2014 ,BSD, WinterDev 
using System.Collections.Generic;
using System.Globalization;
using PixelFarm.Drawing;
using LayoutFarm.WebDom;
using LayoutFarm.Css;
using LayoutFarm.Composers;


namespace LayoutFarm.HtmlBoxes
{
    static class TableBoxCreator
    {

        public static CssBox CreateOtherPredefinedTableElement(CssBox parent,
            HtmlElement childElement, CssDisplay selectedCssDisplayType)
        {
            var newBox = new CssBox(childElement.Spec, parent.RootGfx, selectedCssDisplayType);
            newBox.SetController(childElement);
            parent.AppendChild(newBox);
            return newBox;
        }
        public static CssBox CreateTableColumnOrColumnGroup(CssBox parent,
            HtmlElement childElement, bool fixDisplayType, CssDisplay selectedCssDisplayType)
        {
            CssBox col = null;
            if (fixDisplayType)
            {
                col = new CssBox(childElement.Spec, parent.RootGfx, selectedCssDisplayType);
            }
            else
            { 
                col = new CssBox(childElement.Spec, parent.RootGfx); 
            }
            col.SetController(childElement);
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
                tableCell = new CssBox(childElement.Spec, parent.RootGfx, CssDisplay.TableCell);
            }
            else
            {
                tableCell = new CssBox(childElement.Spec, parent.RootGfx);
            }
            tableCell.SetController(childElement);
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
        static Composers.ContentTextSplitter splitter = new Composers.ContentTextSplitter();

        public static CssBox CreateListItemBox(CssBox parent, HtmlElement childElement)
        {
            var spec = childElement.Spec;
            var newBox = new CssBoxListItem(spec, parent.RootGfx);
            newBox.SetController(childElement);
            parent.AppendChild(newBox);

            if (spec.ListStyleType != CssListStyleType.None)
            {

                //create sub item collection 
                var itemBulletBox = new CssBox(spec.GetAnonVersion(), parent.RootGfx);
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
                splitter.ParseWordContent(text_content, spec, itemBulletBox.IsBlock, out runlist, out  hasSomeCharacter);

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