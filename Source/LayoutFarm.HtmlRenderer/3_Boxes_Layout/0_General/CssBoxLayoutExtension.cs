// 2015,2014 ,BSD, WinterDev 

namespace LayoutFarm.HtmlBoxes
{


    static class CssBoxLayoutExtension
    {
        //--------------------------------
        public static float GetClientLeft(this CssBox box)
        {
            return box.ActualBorderLeftWidth + box.ActualPaddingLeft;
        }
        public static float GetClientRight(this CssBox box)
        {
            return box.SizeWidth - box.ActualPaddingRight - box.ActualBorderRightWidth;
        }
        //--------------------------------
        public static float GetClientTop(this CssBox box)
        {
            return box.ActualBorderTopWidth + box.ActualPaddingTop;
        }
        //------------------------------------------
        public static float GetClientWidth(this CssBox box)
        {
            return box.SizeWidth - (box.ActualBorderLeftWidth +
                box.ActualPaddingLeft + box.ActualPaddingRight + box.ActualBorderRightWidth);
        }
        public static  float GetClientHeight(this CssBox box)
        {
            return box.SizeHeight - (box.ActualBorderTopWidth + box.ActualPaddingTop  + box.ActualPaddingBottom + box.ActualBorderBottomWidth);
        }
        internal static float CalculateInnerContentHeight(this CssBox startBox)
        {
            //calculate inner content height
            if (startBox.LineBoxCount > 0)
            {
                var lastLine = startBox.GetLastLineBox();
                return lastLine.CachedLineBottom;
            }
            else
            {
                float maxBottom = 0;
                foreach (var childBox in startBox.GetChildBoxIter())
                {
                    float top = childBox.LocalY;
                    float contentH = CalculateInnerContentHeight(childBox);
                    if ((top + contentH) > maxBottom)
                    {
                        maxBottom = top + contentH;
                    }
                }
                return maxBottom;
            }
        }
    }
}