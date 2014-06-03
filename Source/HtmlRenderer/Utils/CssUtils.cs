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
        public static float WhiteSpace(IGraphics g, CssBoxBase box)
        {
            //depends on Font of this box
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


        public static void SetPropertyValue(CssBox cssBox, WebDom.CssPropertyDeclaration decl)
        {
            //assign property  
            WebDom.CssCodeValueExpression cssValue = decl.GetPropertyValue(0);

            switch (decl.WellknownPropertyName)
            {
                case WebDom.WellknownCssPropertyName.BorderBottomWidth:
                    cssBox.BorderBottomWidth = cssValue.AsBorderLength();
                    break;
                case WebDom.WellknownCssPropertyName.BorderLeftWidth:
                    cssBox.BorderLeftWidth = cssValue.AsBorderLength();
                    break;
                case WebDom.WellknownCssPropertyName.BorderRightWidth:
                    cssBox.BorderRightWidth = cssValue.AsBorderLength();
                    break;
                case WebDom.WellknownCssPropertyName.BorderTopWidth:
                    cssBox.BorderTopWidth = cssValue.AsBorderLength();
                    break;

                case WebDom.WellknownCssPropertyName.BorderBottomStyle:
                    cssBox.BorderBottomStyle = CssBoxUserUtilExtension.GetBorderStyle(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.BorderLeftStyle:
                    cssBox.BorderLeftStyle = CssBoxUserUtilExtension.GetBorderStyle(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.BorderRightStyle:
                    cssBox.BorderRightStyle = CssBoxUserUtilExtension.GetBorderStyle(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.BorderTopStyle:
                    cssBox.BorderTopStyle = CssBoxUserUtilExtension.GetBorderStyle(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.BorderBottomColor:
                    cssBox.BorderBottomColor = cssValue.AsColor();
                    break;
                case WebDom.WellknownCssPropertyName.BorderLeftColor:
                    cssBox.BorderLeftColor = cssValue.AsColor();
                    break;
                case WebDom.WellknownCssPropertyName.BorderRightColor:
                    cssBox.BorderRightColor = cssValue.AsColor();
                    break;
                case WebDom.WellknownCssPropertyName.BorderTopColor:
                    cssBox.BorderTopColor = cssValue.AsColor();
                    break;

                case WebDom.WellknownCssPropertyName.BorderSpacing:

                    cssBox.SetBorderSpacing(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.BorderCollapse:
                    cssBox.SetBorderCollapse(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.CornerRadius:
                    cssBox.SetCornerRadius(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.CornerNWRadius:
                    cssBox.CornerNWRadius = cssValue.AsLength();
                    break;
                case WebDom.WellknownCssPropertyName.CornerNERadius:
                    cssBox.CornerNERadius = cssValue.AsLength();
                    break;
                case WebDom.WellknownCssPropertyName.CornerSERadius:
                    cssBox.CornerSERadius = cssValue.AsLength();
                    break;
                case WebDom.WellknownCssPropertyName.CornerSWRadius:
                    cssBox.CornerSWRadius = cssValue.AsLength();
                    break;
                case WebDom.WellknownCssPropertyName.MarginBottom:
                    cssBox.MarginBottom = cssValue.AsTranslatedLength();
                    break;
                case WebDom.WellknownCssPropertyName.MarginLeft:
                    cssBox.MarginLeft = cssValue.AsTranslatedLength();
                    break;
                case WebDom.WellknownCssPropertyName.MarginRight:
                    cssBox.MarginRight = cssValue.AsTranslatedLength();
                    break;
                case WebDom.WellknownCssPropertyName.MarginTop:
                    cssBox.MarginTop = cssValue.AsTranslatedLength();
                    break;
                case WebDom.WellknownCssPropertyName.PaddingBottom:
                    cssBox.PaddingBottom = cssValue.AsTranslatedLength();
                    break;
                case WebDom.WellknownCssPropertyName.PaddingLeft:
                    cssBox.PaddingLeft = cssValue.AsTranslatedLength();
                    break;
                case WebDom.WellknownCssPropertyName.PaddingRight:
                    cssBox.PaddingRight = cssValue.AsTranslatedLength();
                    break;
                case WebDom.WellknownCssPropertyName.PaddingTop:
                    cssBox.PaddingTop = cssValue.AsTranslatedLength();
                    break;
                case WebDom.WellknownCssPropertyName.Left:
                    cssBox.Left = cssValue.AsLength();
                    break;
                case WebDom.WellknownCssPropertyName.Top:
                    cssBox.Top = cssValue.AsLength();
                    break;
                case WebDom.WellknownCssPropertyName.Width:
                    cssBox.Width = cssValue.AsLength();
                    break;
                case WebDom.WellknownCssPropertyName.MaxWidth:
                    cssBox.MaxWidth = cssValue.AsLength();
                    break;
                case WebDom.WellknownCssPropertyName.Height:
                    cssBox.Height = cssValue.AsLength();
                    break;
                case WebDom.WellknownCssPropertyName.BackgroundColor:
                    cssBox.BackgroundColor = cssValue.AsColor();
                    break;
                case WebDom.WellknownCssPropertyName.BackgroundImage:
                    cssBox.BackgroundImage = cssValue.GetTranslatedStringValue();
                    break;
                case WebDom.WellknownCssPropertyName.BackgroundPosition:
                    cssBox.BackgroundPosition = cssValue.GetTranslatedStringValue();
                    break;
                case WebDom.WellknownCssPropertyName.BackgroundRepeat:
                    cssBox.BackgroundRepeat = cssValue.GetTranslatedStringValue();
                    break;
                case WebDom.WellknownCssPropertyName.BackgroundGradient:
                    cssBox.BackgroundGradient = cssValue.AsColor();
                    break;
                case WebDom.WellknownCssPropertyName.BackgroundGradientAngle:
                    {
                        cssBox.BackgroundGradientAngle = cssValue.AsNumber();

                        //float angle;
                        //if (float.TryParse(cssValue.GetTranslatedStringValue(), out angle))
                        //{
                        //    cssBox.BackgroundGradientAngle = angle;
                        //}
                    } break;
                case WebDom.WellknownCssPropertyName.Color:
                    cssBox.Color = cssValue.AsColor();
                    break;
                case WebDom.WellknownCssPropertyName.Display:
                    cssBox.SetDisplayType(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.Direction:
                    cssBox.SetCssDirection(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.EmptyCells:
                    cssBox.EmptyCells = CssBoxUserUtilExtension.GetEmptyCell(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.Float:
                    cssBox.Float = CssBoxUserUtilExtension.GetFloat(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.Position:
                    cssBox.SetCssPosition(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.LineHeight:
                    cssBox.SetLineHeight(cssValue.GetTranslatedStringValue());
                    break;
                case WebDom.WellknownCssPropertyName.VerticalAlign:
                    cssBox.SetVerticalAlign(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.TextIndent:
                    cssBox.TextIndent = cssValue.AsLength();
                    break;
                case WebDom.WellknownCssPropertyName.TextAlign:
                    cssBox.SetTextAlign(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.TextDecoration:
                    cssBox.SetTextDecoration(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.Whitespace:
                    cssBox.SetWhitespace(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.WordBreak:
                    cssBox.SetWordBreak(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.Visibility:
                    cssBox.SetVisibility(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.WordSpacing:
                    cssBox.WordSpacing = cssValue.AsLength();
                    break;
                case WebDom.WellknownCssPropertyName.FontFamily:
                    cssBox.FontFamily = cssValue.GetTranslatedStringValue();
                    break;
                case WebDom.WellknownCssPropertyName.FontSize:
                    cssBox.SetFontSize(cssValue);

                    break;
                case WebDom.WellknownCssPropertyName.FontStyle:
                    cssBox.FontStyle = CssBoxUserUtilExtension.GetFontStyle(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.FontVariant:
                    cssBox.FontVariant = CssBoxUserUtilExtension.GetFontVariant(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.FontWeight:
                    cssBox.FontWeight = CssBoxUserUtilExtension.GetFontWeight(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.ListStyle:
                    cssBox.ListStyle = cssValue.GetTranslatedStringValue();
                    break;
                case WebDom.WellknownCssPropertyName.ListStylePosition:
                    cssBox.ListStylePosition = CssBoxUserUtilExtension.GetListStylePosition(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.ListStyleImage:
                    cssBox.ListStyleImage = cssValue.GetTranslatedStringValue();
                    break;
                case WebDom.WellknownCssPropertyName.ListStyleType:
                    cssBox.ListStyleType = CssBoxUserUtilExtension.GetListStyleType(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.Overflow:
                    cssBox.SetOverflow(cssValue);
                    break;
            }
        }


        /// <summary>
        /// assign property value from parent
        /// </summary>
        /// <param name="cssBox"></param>
        /// <param name="propName"></param>
        public static void SetPropertyValueFromParent(CssBox cssBox, HtmlRenderer.WebDom.WellknownCssPropertyName propName)
        {
            CssBox parentCssBox = cssBox.ParentBox;

            switch (propName)
            {
                case WebDom.WellknownCssPropertyName.BorderBottomWidth:
                    cssBox.BorderBottomWidth = parentCssBox.BorderBottomWidth;//  CssLength.MakeBorderLength(value);
                    break;
                case WebDom.WellknownCssPropertyName.BorderLeftWidth:
                    cssBox.BorderLeftWidth = parentCssBox.BorderLeftWidth;//CssLength.MakeBorderLength(value);
                    break;
                case WebDom.WellknownCssPropertyName.BorderRightWidth:
                    cssBox.BorderRightWidth = parentCssBox.BorderRightWidth; //CssLength.MakeBorderLength(value);
                    break;
                case WebDom.WellknownCssPropertyName.BorderTopWidth:
                    cssBox.BorderTopWidth = parentCssBox.BorderTopWidth; //CssLength.MakeBorderLength(value);
                    break;
                case WebDom.WellknownCssPropertyName.BorderBottomStyle:
                    cssBox.BorderBottomStyle = parentCssBox.BorderBottomStyle;//GetBorderStyle(value); 
                    break;
                case WebDom.WellknownCssPropertyName.BorderLeftStyle:
                    cssBox.BorderLeftStyle = parentCssBox.BorderBottomStyle;//GetBorderStyle(value);
                    break;
                case WebDom.WellknownCssPropertyName.BorderRightStyle:
                    cssBox.BorderRightStyle = parentCssBox.BorderRightStyle; //GetBorderStyle(value);
                    break;
                case WebDom.WellknownCssPropertyName.BorderTopStyle:
                    cssBox.BorderTopStyle = parentCssBox.BorderTopStyle;
                    break;
                case WebDom.WellknownCssPropertyName.BorderBottomColor:
                    cssBox.BorderBottomColor = parentCssBox.BorderBottomColor;// CssValueParser.GetActualColor(value);
                    break;
                case WebDom.WellknownCssPropertyName.BorderLeftColor:
                    cssBox.BorderLeftColor = parentCssBox.BorderLeftColor;//CssValueParser.GetActualColor(value);
                    break;
                case WebDom.WellknownCssPropertyName.BorderRightColor:
                    cssBox.BorderRightColor = parentCssBox.BorderRightColor;// CssValueParser.GetActualColor(value);
                    break;
                case WebDom.WellknownCssPropertyName.BorderTopColor:
                    cssBox.BorderTopColor = parentCssBox.BorderTopColor;//  CssValueParser.GetActualColor(value);
                    break;
                case WebDom.WellknownCssPropertyName.BorderSpacing:

                    cssBox.BorderSpacingHorizontal = parentCssBox.BorderSpacingHorizontal;
                    cssBox.BorderSpacingVertical = parentCssBox.BorderSpacingVertical;
                    break;
                case WebDom.WellknownCssPropertyName.BorderCollapse:
                    cssBox.BorderCollapse = parentCssBox.BorderCollapse;
                    break;
                case WebDom.WellknownCssPropertyName.CornerRadius:
                    cssBox.CornerNERadius = parentCssBox.CornerNERadius;
                    cssBox.CornerNWRadius = parentCssBox.CornerNWRadius;
                    cssBox.CornerSERadius = parentCssBox.CornerSERadius;
                    cssBox.CornerSWRadius = parentCssBox.CornerSWRadius;
                    break;
                case WebDom.WellknownCssPropertyName.CornerNWRadius:

                    cssBox.CornerNWRadius = parentCssBox.CornerNWRadius;
                    break;
                case WebDom.WellknownCssPropertyName.CornerNERadius:
                    cssBox.CornerNERadius = parentCssBox.CornerNERadius;// new CssLength(value);
                    break;
                case WebDom.WellknownCssPropertyName.CornerSERadius:
                    cssBox.CornerSERadius = parentCssBox.CornerSERadius;//new CssLength(value);
                    break;
                case WebDom.WellknownCssPropertyName.CornerSWRadius:
                    cssBox.CornerSWRadius = parentCssBox.CornerSWRadius;//new CssLength(value);
                    break;
                case WebDom.WellknownCssPropertyName.MarginBottom:
                    cssBox.MarginBottom = parentCssBox.MarginBottom; // new CssLength(DomParser.TranslateLength(value));
                    break;
                case WebDom.WellknownCssPropertyName.MarginLeft:
                    cssBox.MarginLeft = parentCssBox.MarginLeft; //new CssLength(DomParser.TranslateLength(value));
                    break;
                case WebDom.WellknownCssPropertyName.MarginRight:
                    cssBox.MarginRight = parentCssBox.MarginRight;// new CssLength(DomParser.TranslateLength(value));
                    break;
                case WebDom.WellknownCssPropertyName.MarginTop:
                    cssBox.MarginTop = parentCssBox.MarginTop;// new CssLength(DomParser.TranslateLength(value));
                    break;
                case WebDom.WellknownCssPropertyName.PaddingBottom:
                    cssBox.PaddingBottom = parentCssBox.MarginBottom;// new CssLength(DomParser.TranslateLength(value));
                    break;
                case WebDom.WellknownCssPropertyName.PaddingLeft:
                    cssBox.PaddingLeft = parentCssBox.PaddingLeft;// new CssLength(DomParser.TranslateLength(value));
                    break;
                case WebDom.WellknownCssPropertyName.PaddingRight:
                    cssBox.PaddingRight = parentCssBox.PaddingRight;//new CssLength(DomParser.TranslateLength(value));
                    break;
                case WebDom.WellknownCssPropertyName.PaddingTop:
                    cssBox.PaddingTop = parentCssBox.PaddingTop;// new CssLength(DomParser.TranslateLength(value));
                    break;
                case WebDom.WellknownCssPropertyName.Left:
                    cssBox.Left = parentCssBox.Left;//  new CssLength(value);
                    break;
                case WebDom.WellknownCssPropertyName.Top:
                    cssBox.Top = parentCssBox.Top;// new CssLength(value);
                    break;
                case WebDom.WellknownCssPropertyName.Width:
                    cssBox.Width = parentCssBox.Width;// new CssLength(value);
                    break;
                case WebDom.WellknownCssPropertyName.MaxWidth:
                    cssBox.MaxWidth = parentCssBox.MaxWidth;// new CssLength(value);
                    break;
                case WebDom.WellknownCssPropertyName.Height:
                    cssBox.Height = parentCssBox.Height;//new CssLength(value);
                    break;
                case WebDom.WellknownCssPropertyName.BackgroundColor:
                    cssBox.BackgroundColor = parentCssBox.BackgroundColor;// CssValueParser.GetActualColor(value);
                    break;
                case WebDom.WellknownCssPropertyName.BackgroundImage:
                    cssBox.BackgroundImage = parentCssBox.BackgroundImage;// value;
                    break;
                case WebDom.WellknownCssPropertyName.BackgroundPosition:
                    cssBox.BackgroundPosition = parentCssBox.BackgroundPosition;// value;
                    break;
                case WebDom.WellknownCssPropertyName.BackgroundRepeat:
                    cssBox.BackgroundRepeat = parentCssBox.BackgroundRepeat;// value;
                    break;
                case WebDom.WellknownCssPropertyName.BackgroundGradient:
                    cssBox.BackgroundGradient = parentCssBox.BackgroundGradient;// CssValueParser.GetActualColor(value);
                    break;
                case WebDom.WellknownCssPropertyName.BackgroundGradientAngle:
                    {
                        cssBox.BackgroundGradientAngle = parentCssBox.BackgroundGradientAngle;
                        //float angle;
                        //if (float.TryParse(value, out angle))
                        //{
                        //    cssBox.BackgroundGradientAngle = angle;
                        //}
                    } break;
                case WebDom.WellknownCssPropertyName.Color:
                    cssBox.Color = parentCssBox.Color;
                    break;
                case WebDom.WellknownCssPropertyName.Display:
                    cssBox.CssDisplay = parentCssBox.CssDisplay;
                    break;
                case WebDom.WellknownCssPropertyName.Direction:
                    cssBox.CssDirection = parentCssBox.CssDirection;
                    break;
                case WebDom.WellknownCssPropertyName.EmptyCells:
                    cssBox.EmptyCells = parentCssBox.EmptyCells;
                    break;
                case WebDom.WellknownCssPropertyName.Float:
                    cssBox.Float = parentCssBox.Float;
                    break;
                case WebDom.WellknownCssPropertyName.Position:
                    cssBox.Position = parentCssBox.Position;
                    break;
                case WebDom.WellknownCssPropertyName.LineHeight:
                    cssBox.LineHeight = parentCssBox.LineHeight;
                    break;
                case WebDom.WellknownCssPropertyName.VerticalAlign:
                    cssBox.VerticalAlign = parentCssBox.VerticalAlign;
                    break;
                case WebDom.WellknownCssPropertyName.TextIndent:
                    cssBox.TextIndent = parentCssBox.TextIndent;
                    break;
                case WebDom.WellknownCssPropertyName.TextAlign:
                    cssBox.CssTextAlign = parentCssBox.CssTextAlign;
                    break;
                case WebDom.WellknownCssPropertyName.TextDecoration:
                    //cssBox.SetTextDecoration(value);
                    cssBox.TextDecoration = parentCssBox.TextDecoration;
                    break;
                case WebDom.WellknownCssPropertyName.Whitespace:
                    cssBox.WhiteSpace = parentCssBox.WhiteSpace;
                    break;
                case WebDom.WellknownCssPropertyName.WordBreak:
                    cssBox.WordBreak = parentCssBox.WordBreak;
                    break;
                case WebDom.WellknownCssPropertyName.Visibility:
                    cssBox.CssVisibility = parentCssBox.CssVisibility;
                    break;
                case WebDom.WellknownCssPropertyName.WordSpacing:
                    cssBox.WordSpacing = parentCssBox.WordSpacing;
                    break;
                case WebDom.WellknownCssPropertyName.FontFamily:
                    cssBox.FontFamily = parentCssBox.FontFamily;
                    //cssBox.FontFamily = value;
                    break;
                case WebDom.WellknownCssPropertyName.FontSize:
                    cssBox.FontSize = parentCssBox.FontSize;
                    break;
                case WebDom.WellknownCssPropertyName.FontStyle:
                    cssBox.FontStyle = parentCssBox.FontStyle;
                    break;
                case WebDom.WellknownCssPropertyName.FontVariant:
                    cssBox.FontVariant = parentCssBox.FontVariant;

                    break;
                case WebDom.WellknownCssPropertyName.FontWeight:
                    cssBox.FontWeight = parentCssBox.FontWeight;

                    break;
                case WebDom.WellknownCssPropertyName.ListStyle:

                    cssBox.ListStyle = parentCssBox.ListStyle;
                    break;
                case WebDom.WellknownCssPropertyName.ListStylePosition:
                    cssBox.ListStylePosition = parentCssBox.ListStylePosition;// CssBoxUserUtilExtension.GetListStylePosition(value);
                    break;
                case WebDom.WellknownCssPropertyName.ListStyleImage:

                    cssBox.ListStyleImage = parentCssBox.ListStyleImage;
                    break;
                case WebDom.WellknownCssPropertyName.ListStyleType:

                    cssBox.ListStyleType = parentCssBox.ListStyleType;
                    break;
                case WebDom.WellknownCssPropertyName.Overflow:

                    cssBox.Overflow = parentCssBox.Overflow;
                    break;
            }
        }
    }
}
