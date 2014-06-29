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


    static class CssBoxLayoutExtension
    {
        
        internal static void UseExpectedHeight(this CssBox box)
        {
            box.SetHeight(box.ExpectedHeight);
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

        /// <summary>
        /// Set the style/width/color for all 4 borders on the box.<br/>
        /// if null is given for a value it will not be set.
        /// </summary>
        /// <param name="style">optional: the style to set</param>
        /// <param name="width">optional: the width to set</param>
        /// <param name="color">optional: the color to set</param>
        internal static void SetAllBorders(this CssBox box, CssBorderStyle borderStyle, CssLength length, Color color)
        {
            //assign values

            box.BorderLeftStyle = box.BorderTopStyle = box.BorderRightStyle = box.BorderBottomStyle = borderStyle;

            box.BorderLeftWidth = box.BorderTopWidth = box.BorderRightWidth = box.BorderBottomWidth = length;

            box.BorderLeftColor = box.BorderTopColor = box.BorderRightColor = box.BorderBottomColor = color;

        }
    }
}