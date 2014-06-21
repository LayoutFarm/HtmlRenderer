//BSD 2014, WinterCore
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using HtmlRenderer.Entities;
using HtmlRenderer.Parse;
using HtmlRenderer.Utils;

namespace HtmlRenderer.Dom
{


    static class CssBoxExtension
    {
        internal static void UpdateIfHigher(this CssBox box, float newHeight)
        {
            if (newHeight > box.SizeHeight)
            {
                box.SetHeight(newHeight);
            }
        }
        internal static void UseExpectedHeight(this CssBox box)
        {
            box.SetHeight(box.ExpectedHeight);
        }
        internal static void SetHeightToZero(this CssBox box)
        {
            box.SetHeight(0);
        }
        internal static bool IsAbsolutePosition(this CssBox box)
        {
            return box.Position == CssPosition.Absolute;
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