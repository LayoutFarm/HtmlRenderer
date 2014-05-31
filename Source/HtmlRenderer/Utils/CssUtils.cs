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
        public static CssBorderStyle GetBorderStyle(string value)
        {
            return CssBoxUserUtilExtension.GetBorderStyle(value);
        }


        public static void SetPropertyValue2(CssBox cssBox, WebDom.CssPropertyDeclaration decl)
        {
            //assign property  
            WebDom.CssCodePropertyValue cssValue = decl.GetPropertyValue(0);
            string value = cssValue.ToString();
            //--------------------------------------------------------------
            switch (decl.PropertyName)
            {
                case "border-bottom-width":
                    cssBox.BorderBottomWidth = cssValue.AsBorderLength();
                    break;
                case "border-left-width":
                    cssBox.BorderLeftWidth = cssValue.AsBorderLength();
                    break;
                case "border-right-width":
                    cssBox.BorderRightWidth = cssValue.AsBorderLength();
                    break;
                case "border-top-width":
                    cssBox.BorderTopWidth = cssValue.AsBorderLength();
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
                    cssBox.BorderBottomColor = cssValue.AsColor();// CssValueParser.GetActualColor(value);
                    break;
                case "border-left-color":
                    cssBox.BorderLeftColor = cssValue.AsColor();// CssValueParser.GetActualColor(value);
                    break;
                case "border-right-color":
                    cssBox.BorderRightColor = cssValue.AsColor();//CssValueParser.GetActualColor(value);
                    break;
                case "border-top-color":
                    cssBox.BorderTopColor = cssValue.AsColor();//CssValueParser.GetActualColor(value);
                    break;
                case "border-spacing":
                     
                    cssBox.SetBorderSpacing(value);
                    break;
                case "border-collapse":
                    cssBox.SetBorderCollapse(value);
                    break;
                case "corner-radius":
                     
                    cssBox.SetCornerRadius(value);
                    break;
                case "corner-nw-radius":
                    cssBox.CornerNWRadius = cssValue.AsLength();
                    break;
                case "corner-ne-radius":
                    cssBox.CornerNERadius = cssValue.AsLength();
                    break;
                case "corner-se-radius":
                    cssBox.CornerSERadius = cssValue.AsLength();
                    break;
                case "corner-sw-radius":
                    cssBox.CornerSWRadius = cssValue.AsLength();
                    break;
                case "margin-bottom":
                    cssBox.MarginBottom = cssValue.AsTranslatedLength();
                    break;
                case "margin-left":
                    cssBox.MarginLeft = cssValue.AsTranslatedLength();
                    break;
                case "margin-right":
                    cssBox.MarginRight = cssValue.AsTranslatedLength();
                    break;
                case "margin-top":
                    cssBox.MarginTop = cssValue.AsTranslatedLength();
                    break;
                case "padding-bottom":
                    cssBox.PaddingBottom = cssValue.AsTranslatedLength();
                    break;
                case "padding-left":
                    cssBox.PaddingLeft = cssValue.AsTranslatedLength();
                    break;
                case "padding-right":
                    cssBox.PaddingRight = cssValue.AsTranslatedLength();
                    break;
                case "padding-top":
                    cssBox.PaddingTop = cssValue.AsTranslatedLength();
                    break;
                case "left":
                    cssBox.Left = cssValue.AsLength();
                    break;
                case "top":
                    cssBox.Top = cssValue.AsLength();
                    break;
                case "width":
                    cssBox.Width = cssValue.AsLength();
                    break;
                case "max-width":
                    cssBox.MaxWidth = cssValue.AsLength();
                    break;
                case "height":
                    cssBox.Height = cssValue.AsLength();
                    break;
                case "background-color":
                    cssBox.BackgroundColor = cssValue.AsColor();// CssValueParser.GetActualColor(value);
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
                    cssBox.BackgroundGradient = cssValue.AsColor();// CssValueParser.GetActualColor(value);
                    break;
                case "background-gradient-angle":
                    {
                        float angle;
                        if (float.TryParse(value, out angle))
                        {
                            cssBox.BackgroundGradientAngle = angle;
                        }
                    } break;
                case "color":
                    cssBox.Color = cssValue.AsColor();//CssValueParser.GetActualColor(value);
                    break;
                case "display":
                    cssBox.SetDisplayType(value);
                    break;
                case "direction":
                    cssBox.SetCssDirection(value);
                    break;
                case "empty-cells":
                    cssBox.EmptyCells = CssBoxUserUtilExtension.GetEmptyCell(value);
                    break;
                case "float":
                    cssBox.Float = CssBoxUserUtilExtension.GetFloat(value);
                    break;
                case "position":
                    cssBox.SetCssPosition(value);
                    break;
                case "line-height":
                    cssBox.SetLineHeight(value);
                    break;
                case "vertical-align":
                    cssBox.SetVerticalAlign(value.ToLower());
                    break;
                case "text-indent":
                    cssBox.TextIndent = cssValue.AsLength();
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
                    cssBox.SetFontSize(value);
                    break;
                case "font-style":
                    cssBox.FontStyle = CssBoxUserUtilExtension.GetFontStyle(value);
                    break;
                case "font-variant":
                    cssBox.FontVariant = CssBoxUserUtilExtension.GetFontVariant(value);
                    break;
                case "font-weight":
                    cssBox.FontWeight = CssBoxUserUtilExtension.GetFontWeight(value);
                    break;
                case "list-style":
                    cssBox.ListStyle = value;
                    break;
                case "list-style-position":
                    cssBox.ListStylePosition = CssBoxUserUtilExtension.GetListStylePosition(value);
                    break;
                case "list-style-image":
                    cssBox.ListStyleImage = value;
                    break;
                case "list-style-type":
                    cssBox.ListStyleType = CssBoxUserUtilExtension.GetListStyleType(value);
                    break;
                case "overflow":
                    cssBox.SetOverflow(value);
                    break;
            }
        }


        /// <summary>
        /// assign property value from parent
        /// </summary>
        /// <param name="cssBox"></param>
        /// <param name="propName"></param>
        public static void SetPropertyValueFromParent(CssBox cssBox, string propName)
        {
            CssBox parentCssBox = cssBox.ParentBox;

            switch (propName)
            {
                case "border-bottom-width":
                    cssBox.BorderBottomWidth = parentCssBox.BorderBottomWidth;//  CssLength.MakeBorderLength(value);
                    break;
                case "border-left-width":
                    cssBox.BorderLeftWidth = parentCssBox.BorderLeftWidth;//CssLength.MakeBorderLength(value);
                    break;
                case "border-right-width":
                    cssBox.BorderRightWidth = parentCssBox.BorderRightWidth; //CssLength.MakeBorderLength(value);
                    break;
                case "border-top-width":
                    cssBox.BorderTopWidth = parentCssBox.BorderTopWidth; //CssLength.MakeBorderLength(value);
                    break;
                case "border-bottom-style":
                    cssBox.BorderBottomStyle = parentCssBox.BorderBottomStyle;//GetBorderStyle(value); 
                    break;
                case "border-left-style":
                    cssBox.BorderLeftStyle = parentCssBox.BorderBottomStyle;//GetBorderStyle(value);
                    break;
                case "border-right-style":
                    cssBox.BorderRightStyle = parentCssBox.BorderRightStyle; //GetBorderStyle(value);
                    break;
                case "border-top-style":
                    cssBox.BorderTopStyle = parentCssBox.BorderTopStyle;
                    break;
                case "border-bottom-color":
                    cssBox.BorderBottomColor = parentCssBox.BorderBottomColor;// CssValueParser.GetActualColor(value);
                    break;
                case "border-left-color":
                    cssBox.BorderLeftColor = parentCssBox.BorderLeftColor;//CssValueParser.GetActualColor(value);
                    break;
                case "border-right-color":
                    cssBox.BorderRightColor = parentCssBox.BorderRightColor;// CssValueParser.GetActualColor(value);
                    break;
                case "border-top-color":
                    cssBox.BorderTopColor = parentCssBox.BorderTopColor;//  CssValueParser.GetActualColor(value);
                    break;
                case "border-spacing":
                     
                    cssBox.BorderSpacingHorizontal = parentCssBox.BorderSpacingHorizontal;
                    cssBox.BorderSpacingVertical = parentCssBox.BorderSpacingVertical; 
                    break;
                case "border-collapse": 
                    cssBox.BorderCollapse = parentCssBox.BorderCollapse;
                    break;
                case "corner-radius": 
                    cssBox.CornerNERadius = parentCssBox.CornerNERadius;
                    cssBox.CornerNWRadius = parentCssBox.CornerNWRadius;
                    cssBox.CornerSERadius = parentCssBox.CornerSERadius;
                    cssBox.CornerSWRadius = parentCssBox.CornerSWRadius;
                    break;
                case "corner-nw-radius":
                    
                    cssBox.CornerNWRadius = parentCssBox.CornerNWRadius;
                    break;
                case "corner-ne-radius":
                    cssBox.CornerNERadius = parentCssBox.CornerNERadius;// new CssLength(value);
                    break;
                case "corner-se-radius":
                    cssBox.CornerSERadius = parentCssBox.CornerSERadius;//new CssLength(value);
                    break;
                case "corner-sw-radius":
                    cssBox.CornerSWRadius = parentCssBox.CornerSWRadius;//new CssLength(value);
                    break;
                case "margin-bottom":
                    cssBox.MarginBottom = parentCssBox.MarginBottom; // new CssLength(DomParser.TranslateLength(value));
                    break;
                case "margin-left":
                    cssBox.MarginLeft = parentCssBox.MarginLeft; //new CssLength(DomParser.TranslateLength(value));
                    break;
                case "margin-right":
                    cssBox.MarginRight = parentCssBox.MarginRight;// new CssLength(DomParser.TranslateLength(value));
                    break;
                case "margin-top":
                    cssBox.MarginTop = parentCssBox.MarginTop;// new CssLength(DomParser.TranslateLength(value));
                    break;
                case "padding-bottom":
                    cssBox.PaddingBottom = parentCssBox.MarginBottom;// new CssLength(DomParser.TranslateLength(value));
                    break;
                case "padding-left":
                    cssBox.PaddingLeft = parentCssBox.PaddingLeft;// new CssLength(DomParser.TranslateLength(value));
                    break;
                case "padding-right":
                    cssBox.PaddingRight = parentCssBox.PaddingRight;//new CssLength(DomParser.TranslateLength(value));
                    break;
                case "padding-top":
                    cssBox.PaddingTop = parentCssBox.PaddingTop;// new CssLength(DomParser.TranslateLength(value));
                    break;
                case "left":
                    cssBox.Left = parentCssBox.Left;//  new CssLength(value);
                    break;
                case "top":
                    cssBox.Top = parentCssBox.Top;// new CssLength(value);
                    break;
                case "width":
                    cssBox.Width = parentCssBox.Width;// new CssLength(value);
                    break;
                case "max-width":
                    cssBox.MaxWidth = parentCssBox.MaxWidth;// new CssLength(value);
                    break;
                case "height":
                    cssBox.Height = parentCssBox.Height;//new CssLength(value);
                    break;
                case "background-color":
                    cssBox.BackgroundColor = parentCssBox.BackgroundColor;// CssValueParser.GetActualColor(value);
                    break;
                case "background-image":
                    cssBox.BackgroundImage = parentCssBox.BackgroundImage;// value;
                    break;
                case "background-position":
                    cssBox.BackgroundPosition = parentCssBox.BackgroundPosition;// value;
                    break;
                case "background-repeat":
                    cssBox.BackgroundRepeat = parentCssBox.BackgroundRepeat;// value;
                    break;
                case "background-gradient":
                    cssBox.BackgroundGradient = parentCssBox.BackgroundGradient;// CssValueParser.GetActualColor(value);
                    break;
                case "background-gradient-angle":
                    {
                        cssBox.BackgroundGradientAngle = parentCssBox.BackgroundGradientAngle;
                        //float angle;
                        //if (float.TryParse(value, out angle))
                        //{
                        //    cssBox.BackgroundGradientAngle = angle;
                        //}
                    } break;
                case "color": 
                    cssBox.Color = parentCssBox.Color;
                    break;
                case "display": 
                    cssBox.CssDisplay = parentCssBox.CssDisplay;
                    break;
                case "direction": 
                    cssBox.CssDirection = parentCssBox.CssDirection;
                    break;
                case "empty-cells": 
                    cssBox.EmptyCells = parentCssBox.EmptyCells;
                    break;
                case "float": 
                    cssBox.Float = parentCssBox.Float;
                    break;
                case "position": 
                    cssBox.Position = parentCssBox.Position;
                    break;
                case "line-height": 
                    cssBox.LineHeight = parentCssBox.LineHeight;
                    break;
                case "vertical-align": 
                    cssBox.VerticalAlign = parentCssBox.VerticalAlign;
                    break;
                case "text-indent": 
                    cssBox.TextIndent = parentCssBox.TextIndent;
                    break;
                case "text-align": 
                    cssBox.CssTextAlign = parentCssBox.CssTextAlign;
                    break;
                case "text-decoration":
                    //cssBox.SetTextDecoration(value);
                    cssBox.TextDecoration = parentCssBox.TextDecoration;
                    break;
                case "white-space": 
                    cssBox.WhiteSpace = parentCssBox.WhiteSpace;
                    break;
                case "word-break": 
                    cssBox.WordBreak = parentCssBox.WordBreak;
                    break;
                case "visibility": 
                    cssBox.CssVisibility = parentCssBox.CssVisibility;
                    break;
                case "word-spacing":
                    cssBox.WordSpacing = parentCssBox.WordSpacing; 
                    break;
                case "font-family":
                    cssBox.FontFamily = parentCssBox.FontFamily;
                    //cssBox.FontFamily = value;
                    break;
                case "font-size":
                    cssBox.FontSize = parentCssBox.FontSize; 
                    break;
                case "font-style": 
                    cssBox.FontStyle = parentCssBox.FontStyle;
                    break;
                case "font-variant":
                    cssBox.FontVariant = parentCssBox.FontVariant;
                   
                    break;
                case "font-weight":
                    cssBox.FontWeight = parentCssBox.FontWeight;
                  
                    break;
                case "list-style":
                    
                    cssBox.ListStyle = parentCssBox.ListStyle;
                    break;
                case "list-style-position":
                    cssBox.ListStylePosition = parentCssBox.ListStylePosition;// CssBoxUserUtilExtension.GetListStylePosition(value);
                    break;
                case "list-style-image":
                     
                    cssBox.ListStyleImage = parentCssBox.ListStyleImage;
                    break;
                case "list-style-type":
                     
                    cssBox.ListStyleType = parentCssBox.ListStyleType;
                    break;
                case "overflow":
                   
                    cssBox.Overflow = parentCssBox.Overflow;
                    break;
            }
        }
    }
}
