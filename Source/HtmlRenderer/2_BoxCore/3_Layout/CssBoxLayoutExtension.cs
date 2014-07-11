//BSD 2014, WinterDev 

namespace HtmlRenderer.Dom
{


    static class CssBoxLayoutExtension
    {

        internal static void UseExpectedHeight(this CssBox box)
        {
            box.SetHeight(box.ExpectedHeight);
        }
        internal static bool IsAbsolutePosition(this CssBox box)
        {
            return box.Position == Css.CssPosition.Absolute;
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