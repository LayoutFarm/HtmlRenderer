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
        private static CssData _defaultCssData;

        #endregion


        /// <summary>
        /// default CSS parsed data singleton
        /// </summary>
        public static CssData DefaultCssData
        {
            get
            {
                if (_defaultCssData == null)
                {
                    _defaultCssData = new CssData();
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
        public static float WhiteSpace(IGraphics g, CssBoxBase box)
        {
            float w = FontsUtils.MeasureWhitespace(g, box.ActualFont);             
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
                    return cssBox.BorderBottomStyle.ToString();
                case "border-left-style":
                    return cssBox.BorderLeftStyle.ToString();
                case "border-right-style":
                    return cssBox.BorderRightStyle.ToString();
                case "border-top-style":
                    return cssBox.BorderTopStyle.ToString();
                case "border-bottom-color":
                    return cssBox.BorderBottomColor;
                case "border-left-color":
                    return cssBox.BorderLeftColor;
                case "border-right-color":
                    return cssBox.BorderRightColor;
                case "border-top-color":
                    return cssBox.BorderTopColor;
                case "border-spacing":
                    return cssBox.BorderSpacing;
                case "border-collapse":
                    return cssBox.BorderCollapse.ToString();
                case "corner-radius":
                    return cssBox.CornerRadius;
                case "corner-nw-radius":
                    return cssBox.CornerNWRadius;
                case "corner-ne-radius":
                    return cssBox.CornerNERadius;
                case "corner-se-radius":
                    return cssBox.CornerSERadius;
                case "corner-sw-radius":
                    return cssBox.CornerSWRadius;
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
                    return cssBox.Left;
                case "top":
                    return cssBox.Top;
                case "width":
                    return cssBox.Width.ToString();
                case "max-width":
                    return cssBox.MaxWidth.ToString();
                case "height":
                    return cssBox.Height.ToString();
                case "background-color":
                    return cssBox.BackgroundColor;
                case "background-image":
                    return cssBox.BackgroundImage;
                case "background-position":
                    return cssBox.BackgroundPosition;
                case "background-repeat":
                    return cssBox.BackgroundRepeat;
                case "background-gradient":
                    return cssBox.BackgroundGradient;
                case "background-gradient-angle":
                    return cssBox.BackgroundGradientAngle;
                case "color":
                    return cssBox.Color;
                case "display":
                    return cssBox.GetDisplayString();
                case "direction":
                    return cssBox.GetCssDirection();
                case "empty-cells":
                    return cssBox.EmptyCells;
                case "float":
                    return cssBox.Float;
                case "position":
                    return cssBox.Position.ToString();
                case "line-height":
                    return cssBox.LineHeight;
                case "vertical-align":
                    return cssBox.VerticalAlign.ToString();
                case "text-indent":
                    return cssBox.TextIndent.ToString();
                case "text-align":
                    return cssBox.CssTextAlign.ToString();
                case "text-decoration":
                    return cssBox.TextDecoration.ToString();
                case "white-space":
                    return cssBox.WhiteSpace.ToString();
                case "word-break":
                    return cssBox.WordBreak.ToString();
                case "visibility":
                    return cssBox.CssVisibility.ToString();
                case "word-spacing":
                    return cssBox.WordSpacing.ToString();
                case "font-family":
                    return cssBox.FontFamily;
                case "font-size":
                    return cssBox.FontSize;
                case "font-style":
                    return cssBox.FontStyle;
                case "font-variant":
                    return cssBox.FontVariant;
                case "font-weight":
                    return cssBox.FontWeight;
                case "list-style":
                    return cssBox.ListStyle;
                case "list-style-position":
                    return cssBox.ListStylePosition;
                case "list-style-image":
                    return cssBox.ListStyleImage;
                case "list-style-type":
                    return cssBox.ListStyleType;
                case "overflow":
                    return cssBox.Overflow.ToString();
            }
            return null;
        }
        public static CssBorderStyle GetBorderStyle(string value)
        {
            return CssBoxUserUtilExtension.GetBorderStyle(value); 
        }
        //public static CssBorderCollapse GetBorderCollapse(string str)
        //{
        //    switch (str)
        //    {
        //        default:
        //            return CssBorderCollapse.Sepatate;
        //        case CssConstants.Collapse:
        //            return CssBorderCollapse.Collapse;
        //    }
        //}

        /// <summary>
        /// Set CSS box property value by the CSS name.<br/>
        /// Used as a mapping between CSS property and the class property.
        /// </summary>
        /// <param name="cssBox">the CSS box to set it's property value</param>
        /// <param name="propName">the name of the CSS property</param>
        /// <param name="value">the value to set</param>
        public static void SetPropertyValue(CssBox cssBox, string propName, string value)
        {
            switch (propName)
            {
                case "border-bottom-width":
                    cssBox.BorderBottomWidth = CssLength.MakeBorderLength(value);
                    break;
                case "border-left-width":
                    cssBox.BorderLeftWidth = CssLength.MakeBorderLength(value);
                    break;
                case "border-right-width":
                    cssBox.BorderRightWidth = CssLength.MakeBorderLength(value);
                    break;
                case "border-top-width":
                    cssBox.BorderTopWidth = CssLength.MakeBorderLength(value);
                    break;
                case "border-bottom-style":
                    cssBox.BorderBottomStyle = GetBorderStyle(value);

                    break;
                case "border-left-style":
                    cssBox.BorderLeftStyle = GetBorderStyle(value);
                    break;
                case "border-right-style":
                    cssBox.BorderRightStyle = GetBorderStyle(value);
                    break;
                case "border-top-style":
                    cssBox.BorderTopStyle = GetBorderStyle(value);
                    break;
                case "border-bottom-color":
                    cssBox.BorderBottomColor = value;
                    break;
                case "border-left-color":
                    cssBox.BorderLeftColor = value;
                    break;
                case "border-right-color":
                    cssBox.BorderRightColor = value;
                    break;
                case "border-top-color":
                    cssBox.BorderTopColor = value;
                    break;
                case "border-spacing":
                    cssBox.BorderSpacing = value;
                    break;
                case "border-collapse":                     
                    cssBox.SetBorderCollapse(value);
                    break;
                case "corner-radius":
                    cssBox.CornerRadius = value;
                    break;
                case "corner-nw-radius":
                    cssBox.CornerNWRadius = value;
                    break;
                case "corner-ne-radius":
                    cssBox.CornerNERadius = value;
                    break;
                case "corner-se-radius":
                    cssBox.CornerSERadius = value;
                    break;
                case "corner-sw-radius":
                    cssBox.CornerSWRadius = value;
                    break;
                case "margin-bottom":
                    cssBox.MarginBottom = new CssLength(DomParser.TranslateLength(value));
                    break;
                case "margin-left":
                    cssBox.MarginLeft = new CssLength(DomParser.TranslateLength(value));
                    break;
                case "margin-right":
                    cssBox.MarginRight = new CssLength(DomParser.TranslateLength(value));
                    break;
                case "margin-top":
                    cssBox.MarginTop = new CssLength(DomParser.TranslateLength(value));
                    break;
                case "padding-bottom":
                    cssBox.PaddingBottom = new CssLength(DomParser.TranslateLength(value));
                    break;
                case "padding-left":
                    cssBox.PaddingLeft = new CssLength(DomParser.TranslateLength(value));
                    break;
                case "padding-right":
                    cssBox.PaddingRight = new CssLength(DomParser.TranslateLength(value));
                    break;
                case "padding-top":
                    cssBox.PaddingTop = new CssLength(DomParser.TranslateLength(value));
                    break;
                case "left":
                    cssBox.Left = value;
                    break;
                case "top":
                    cssBox.Top = value;
                    break;
                case "width":
                    cssBox.Width = new CssLength(value);
                    break;
                case "max-width":
                    cssBox.MaxWidth = new CssLength(value);
                    break;
                case "height":
                    cssBox.Height = new CssLength(value);
                    break;
                case "background-color":
                    cssBox.BackgroundColor = value;
                    break;
                case "background-image":
                    cssBox.BackgroundImage = value;
                    break;
                case "background-position":
                    cssBox.BackgroundPosition = value;
                    break;
                case "background-repeat":
                    cssBox.BackgroundRepeat = value;
                    break;
                case "background-gradient":
                    cssBox.BackgroundGradient = value;
                    break;
                case "background-gradient-angle":
                    cssBox.BackgroundGradientAngle = value;
                    break;
                case "color":
                    cssBox.Color = value;
                    break;
                case "display":
                    cssBox.SetDisplayType(value);
                    break;
                case "direction":
                    cssBox.SetCssDirection(value);
                    break;
                case "empty-cells":
                    cssBox.EmptyCells = value;
                    break;
                case "float":
                    cssBox.Float = value;
                    break;
                case "position":
                    cssBox.SetCssPosition(value);
                    break;
                case "line-height":
                    cssBox.LineHeight = value;
                    break;
                case "vertical-align":
                    cssBox.SetVerticalAlign(value.ToLower());
                    break;
                case "text-indent":
                    cssBox.TextIndent = new CssLength(value);
                    break;
                case "text-align":
                    cssBox.SetTextAlign(value);
                    break;
                case "text-decoration":
                    cssBox.SetTextDecoration(value);
                    break;
                case "white-space":
                    cssBox.SetWhitespace(value);
                    break;
                case "word-break":
                    cssBox.SetWordSpacing(value);
                    break;
                case "visibility":
                    cssBox.SetVisibility(value);
                    break;
                case "word-spacing":
                    cssBox.SetWordSpacing(value);
                    break;
                case "font-family":
                    cssBox.FontFamily = value;
                    break;
                case "font-size":
                    cssBox.FontSize = value;
                    break;
                case "font-style":
                    cssBox.FontStyle = value;
                    break;
                case "font-variant":
                    cssBox.FontVariant = value;
                    break;
                case "font-weight":
                    cssBox.FontWeight = value;
                    break;
                case "list-style":
                    cssBox.ListStyle = value;
                    break;
                case "list-style-position":
                    cssBox.ListStylePosition = value;
                    break;
                case "list-style-image":
                    cssBox.ListStyleImage = value;
                    break;
                case "list-style-type":
                    cssBox.ListStyleType = value;
                    break;
                case "overflow":
                    cssBox.SetOverflow(value);
                    break;
            }
        }
    }
}
