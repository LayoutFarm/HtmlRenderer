// "Therefore those skilled at the unorthodox
// are infinite as heaven and earth,
// inexhaustible as the great rivers.
// When they come to an end,
// they begin again,
// like the days and months;
// they die and are reborn,
// like the four seasons."
// 
// - Sun Tsu,
// "The Art of War"

using System;
using System.Drawing;
using HtmlRenderer.Dom;
using HtmlRenderer.Entities;
using HtmlRenderer.Parse;

namespace HtmlRenderer.Utils
{
    /// <summary>
    /// Utility method for handling CSS stuff.
    /// </summary>
    internal static class CssUtils
    {
        #region Fields and Consts

        /// <summary>
        /// Brush for selection background
        /// </summary>
        private static readonly Brush _defaultSelectionBackcolor = new SolidBrush(Color.FromArgb(0xa9, 0x33, 0x99, 0xFF));

        /// <summary>
        /// default CSS parsed data singleton
        /// </summary>
        private static CssActiveSheet _defaultCssData;


        #endregion


        /// <summary>
        /// default CSS parsed data singleton
        /// </summary>
        public static CssActiveSheet DefaultCssData
        {
            get
            {
                if (_defaultCssData == null)
                {
                    _defaultCssData = new CssActiveSheet();
                    CssParser.ParseStyleSheet(_defaultCssData, CssDefaults.DefaultStyleSheet);
                }
                return _defaultCssData;
            }
        }


        /// <summary>
        /// Brush for selection background
        /// </summary>
        public static Brush DefaultSelectionBackcolor
        {
            get { return _defaultSelectionBackcolor; }
        }

        /// <summary>
        /// Gets the white space width of the specified box
        /// </summary>
        /// <param name="g"></param>
        /// <param name="box"></param>
        /// <returns></returns>
        public static float MeasureWhiteSpace(LayoutVisitor lay, CssBoxBase box)
        {
            //depends on Font of this box
            float w = FontsUtils.MeasureWhitespace(lay.Gfx, box.ActualFont);
            if (!(box.WordSpacing.IsEmpty || box.WordSpacing.IsNormalWordSpacing))
            {
                w += CssValueParser.ParseLength(box.WordSpacing, 0, box, true);
            }

            return w;
        }

        /// <summary>
        /// Get CSS box property value by the CSS name.<br/>
        /// Used as a mapping between CSS property and the class property.
        /// </summary>
        /// <param name="cssBox">the CSS box to get it's property value</param>
        /// <param name="propName">the name of the CSS property</param>
        /// <returns>the value of the property, null if no such property exists</returns>
        public static string GetPropertyValue(CssBox cssBox, string propName)
        {
            switch (propName)
            {
                case "border-bottom-width":
                    return cssBox.BorderBottomWidth.ToString();
                case "border-left-width":
                    return cssBox.BorderLeftWidth.ToString();
                case "border-right-width":
                    return cssBox.BorderRightWidth.ToString();
                case "border-top-width":
                    return cssBox.BorderTopWidth.ToString();

                case "border-bottom-style":
                    return cssBox.BorderBottomStyle.ToCssStringValue();
                case "border-left-style":
                    return cssBox.BorderLeftStyle.ToCssStringValue();
                case "border-right-style":
                    return cssBox.BorderRightStyle.ToCssStringValue();
                case "border-top-style":
                    return cssBox.BorderTopStyle.ToCssStringValue();

                case "border-bottom-color":
                    return cssBox.BorderBottomColor.ToHexColor();
                case "border-left-color":
                    return cssBox.BorderLeftColor.ToHexColor();
                case "border-right-color":
                    return cssBox.BorderRightColor.ToHexColor();
                case "border-top-color":
                    return cssBox.BorderTopColor.ToHexColor();

                case "border-spacing":
                    return cssBox.BorderSpacingHorizontal.ToString() + " " +
                           cssBox.BorderSpacingVertical;

                case "border-collapse":
                    return cssBox.BorderCollapse.ToCssStringValue();
                case "corner-radius":
                    return cssBox.GetCornerRadius();

                case "corner-nw-radius":
                    return cssBox.CornerNWRadius.ToString();
                case "corner-ne-radius":
                    return cssBox.CornerNERadius.ToString();
                case "corner-se-radius":
                    return cssBox.CornerSERadius.ToString();
                case "corner-sw-radius":
                    return cssBox.CornerSWRadius.ToString();

                case "margin-bottom":
                    return cssBox.MarginBottom.ToString();
                case "margin-left":
                    return cssBox.MarginLeft.ToString();
                case "margin-right":
                    return cssBox.MarginRight.ToString();
                case "margin-top":
                    return cssBox.MarginTop.ToString();
                case "padding-bottom":

                    return cssBox.PaddingBottom.ToString();
                case "padding-left":
                    return cssBox.PaddingLeft.ToString();
                case "padding-right":
                    return cssBox.PaddingRight.ToString();
                case "padding-top":
                    return cssBox.PaddingTop.ToString();

                case "left":
                    return cssBox.Left.ToString();
                case "top":
                    return cssBox.Top.ToString();
                case "width":
                    return cssBox.Width.ToString();
                case "max-width":
                    return cssBox.MaxWidth.ToString();
                case "height":
                    return cssBox.Height.ToString();

                case "background-color":
                    return cssBox.BackgroundColor.ToHexColor();
                case "background-image":
                    return cssBox.BackgroundImage;
                case "background-position":
                    return cssBox.BackgroundPosition;
                case "background-repeat":
                    return cssBox.BackgroundRepeat;
                case "background-gradient":
                    return cssBox.BackgroundGradient.ToHexColor();
                case "background-gradient-angle":
                    return cssBox.BackgroundGradientAngle.ToString();
                case "color":
                    return cssBox.Color.ToHexColor();
                case "display":
                    return cssBox.CssDisplay.ToCssStringValue();
                case "direction":
                    return cssBox.CssDirection.ToCssStringValue();
                case "empty-cells":
                    return cssBox.EmptyCells.ToCssStringValue();
                case "float":
                    return cssBox.Float.ToCssStringValue();
                case "position":
                    return cssBox.Position.ToCssStringValue();
                case "line-height":
                    return cssBox.LineHeight.ToString();
                case "vertical-align":
                    return cssBox.VerticalAlign.ToCssStringValue();
                case "text-indent":
                    return cssBox.TextIndent.ToString();
                case "text-align":
                    return cssBox.CssTextAlign.ToCssStringValue();
                case "text-decoration":
                    return cssBox.TextDecoration.ToCssStringValue();
                case "white-space":
                    return cssBox.WhiteSpace.ToCssStringValue();
                case "word-break":
                    return cssBox.WordBreak.ToCssStringValue();
                case "visibility":
                    return cssBox.CssVisibility.ToCssStringValue();
                case "word-spacing":
                    return cssBox.WordSpacing.ToString();
                case "font-family":
                    return cssBox.FontFamily;
                case "font-size":
                    return cssBox.FontSize.ToFontSizeString();
                case "font-style":
                    return cssBox.FontStyle.ToCssStringValue();
                case "font-variant":
                    return cssBox.FontVariant.ToCssStringValue();
                case "font-weight":
                    return cssBox.FontWeight.ToCssStringValue();
                case "list-style":
                    return cssBox.ListStyle;
                case "list-style-position":
                    return cssBox.ListStylePosition.ToCssStringValue();
                case "list-style-image":
                    return cssBox.ListStyleImage;
                case "list-style-type":
                    return cssBox.ListStyleType.ToCssStringValue();
                case "overflow":
                    return cssBox.Overflow.ToString();
            }
            return null;
        }




    }
}
