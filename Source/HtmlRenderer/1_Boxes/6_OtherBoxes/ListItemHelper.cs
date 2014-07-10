//BSD 2014,WinterDev
//ArthurHub

using HtmlRenderer.Utils;
using System.Globalization;

namespace HtmlRenderer.Dom
{
    static class ListItemHelper
    {
        static readonly char[] discItem = new[] { '•' };
        static readonly char[] circleItem = new[] { 'o' };
        static readonly char[] squareItem = new[] { '♠' };


        public static CssBox CreateListItem(CssBox forBox, BoxSpec spec, LayoutVisitor lay)
        {

            CssBox listItemBox = new CssBox(null, null, spec);
            CssBox.UnsafeSetParent(listItemBox, forBox, forBox.HtmlContainer);


            listItemBox.ReEvaluateFont(forBox.ActualFont.Size);
            listItemBox.ReEvaluateComputedValues(forBox);


            CssBox.ChangeDisplayType(listItemBox, Dom.CssDisplay.Inline);


            char[] text_content = null;
            switch (forBox.ListStyleType)
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
                        text_content = (GetIndexForList(forBox).ToString(CultureInfo.InvariantCulture) + ".").ToCharArray();
                    } break;
                case CssListStyleType.DecimalLeadingZero:
                    {
                        text_content = (GetIndexForList(forBox).ToString("00", CultureInfo.InvariantCulture) + ".").ToCharArray();
                    } break;
                default:
                    {
                        text_content = (CommonUtils.ConvertToAlphaNumber(GetIndexForList(forBox), forBox.ListStyleType) + ".").ToCharArray();
                    } break;
            }

            //---------------------

            ContentTextSplitter splitter = lay.GetSplitter();
<<<<<<< HEAD
            bool hasSomeCharacter;
            var splitParts = splitter.ParseWordContent(text_content, out  hasSomeCharacter);
            RunListCreator.AddRunList(listItemBox, spec, splitParts, text_content);
=======
            List<CssRun> runlist;
            bool hasSomeCharacter; 
            listItemBox.SetTextBuffer(text_content); 
            splitter.ParseWordContent(text_content, spec, out runlist, out  hasSomeCharacter);
            RunListHelper.AddRunList(listItemBox, spec, runlist, text_content,false );
>>>>>>> v1.7perf
            //--------------------- 

            var prevSibling = lay.LatestSiblingBox;
            lay.LatestSiblingBox = null;//reset
            listItemBox.PerformLayout(lay);
            lay.LatestSiblingBox = prevSibling;


            var fRun = listItemBox.FirstRun;

            listItemBox.FirstRun.SetSize(fRun.Width, fRun.Height);


            return listItemBox;
        }

        /// <summary>
        /// Gets the index of the box to be used on a (ordered) list
        /// </summary>
        /// <returns></returns>
        static int GetIndexForList(CssBox box)
        {
            bool reversed = !string.IsNullOrEmpty(box.ParentBox.GetAttribute("reversed"));
            int index;
            if (!int.TryParse(box.ParentBox.GetAttribute("start"), out index))
            {
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
                if (b.Equals(box))
                    return index;
                if (b.CssDisplay == CssDisplay.ListItem)
                    index += reversed ? -1 : 1;
            }

            return index;
        }
    }


}