//BSD 2014, WinterCore

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
using System.Globalization;
using System.Collections.Generic;

using HtmlRenderer.Entities;
using HtmlRenderer.Handlers;
using HtmlRenderer.Utils;
using HtmlRenderer.Parse;

using HtmlRenderer.WebDom;


namespace HtmlRenderer.Dom
{

    static class CssPropSetter
    {
        static void SetPropertyValue(CssBoxBase cssBox, CssBoxBase parentBox, WebDom.CssPropertyDeclaration decl)
        {
            //assign property  
            WebDom.CssCodeValueExpression cssValue = decl.GetPropertyValue(0);
            switch (decl.WellknownPropertyName)
            {
                case WebDom.WellknownCssPropertyName.Display:
                    {
                        CssDisplay display = UserMapUtil.GetDisplayType(cssValue);
                        switch (cssBox.WellknownTagName)
                        {
                            //------------------------
                            //fix definition
                            case WellknownHtmlTagName.table:
                                display = CssDisplay.Table;
                                break;
                            case WellknownHtmlTagName.tr:
                                display = CssDisplay.TableRow;
                                break;
                            case WellknownHtmlTagName.tbody:
                                display = CssDisplay.TableRowGroup;
                                break;
                            case WellknownHtmlTagName.thead:
                                display = CssDisplay.TableHeaderGroup;
                                break;
                            case WellknownHtmlTagName.tfoot:
                                display = CssDisplay.TableFooterGroup;
                                break;
                            case WellknownHtmlTagName.col:
                                display = CssDisplay.TableColumn;
                                break;
                            case WellknownHtmlTagName.colgroup:
                                display = CssDisplay.TableColumnGroup;
                                break;
                            case WellknownHtmlTagName.td:
                            case WellknownHtmlTagName.th:
                                display = CssDisplay.TableCell;
                                break;
                            case WellknownHtmlTagName.caption:
                                display = CssDisplay.TableCaption;
                                break;
                            //------------------------
                        }
                        cssBox.CssDisplay = display;

                    } break;
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
                    cssBox.BorderBottomStyle = UserMapUtil.GetBorderStyle(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.BorderLeftStyle:
                    cssBox.BorderLeftStyle = UserMapUtil.GetBorderStyle(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.BorderRightStyle:
                    cssBox.BorderRightStyle = UserMapUtil.GetBorderStyle(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.BorderTopStyle:
                    cssBox.BorderTopStyle = UserMapUtil.GetBorderStyle(cssValue);
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
                    cssBox.BorderCollapse = UserMapUtil.GetBorderCollapse(cssValue);
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
                    cssBox.BackgroundImageBinder = new ImageBinder(cssValue.GetTranslatedStringValue());
                    break;
                case WebDom.WellknownCssPropertyName.BackgroundPosition:

                    cssBox.SetBackgroundPosition(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.BackgroundRepeat:
                    cssBox.BackgroundRepeat = UserMapUtil.GetBackgroundRepeat(cssValue);
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

                case WebDom.WellknownCssPropertyName.Direction:

                    cssBox.CssDirection = UserMapUtil.GetCssDirection(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.EmptyCells:
                    cssBox.EmptyCells = UserMapUtil.GetEmptyCell(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.Float:
                    cssBox.Float = UserMapUtil.GetFloat(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.Position:
                    cssBox.Position = UserMapUtil.GetCssPosition(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.LineHeight:

                    cssBox.SetLineHeight(cssValue.AsLength());
                    break;
                case WebDom.WellknownCssPropertyName.VerticalAlign:
                    cssBox.VerticalAlign = UserMapUtil.GetVerticalAlign(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.TextIndent:
                    cssBox.TextIndent = cssValue.AsLength();
                    break;
                case WebDom.WellknownCssPropertyName.TextAlign:

                    cssBox.CssTextAlign = UserMapUtil.GetTextAlign(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.TextDecoration:
                    cssBox.TextDecoration = UserMapUtil.GetTextDecoration(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.Whitespace:
                    cssBox.WhiteSpace = UserMapUtil.GetWhitespace(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.WordBreak:
                    cssBox.WordBreak = UserMapUtil.GetWordBreak(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.Visibility:
                    cssBox.CssVisibility = UserMapUtil.GetVisibility(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.WordSpacing:
                    cssBox.WordSpacing = cssValue.AsLength();
                    break;
                case WebDom.WellknownCssPropertyName.FontFamily:
                    cssBox.FontFamily = cssValue.GetTranslatedStringValue();
                    break;
                case WebDom.WellknownCssPropertyName.FontSize:
                    cssBox.SetFontSize(parentBox, cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.FontStyle:
                    cssBox.FontStyle = UserMapUtil.GetFontStyle(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.FontVariant:
                    cssBox.FontVariant = UserMapUtil.GetFontVariant(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.FontWeight:
                    cssBox.FontWeight = UserMapUtil.GetFontWeight(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.ListStyle:
                    cssBox.ListStyle = cssValue.GetTranslatedStringValue();
                    break;
                case WebDom.WellknownCssPropertyName.ListStylePosition:
                    cssBox.ListStylePosition = UserMapUtil.GetListStylePosition(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.ListStyleImage:
                    cssBox.ListStyleImage = cssValue.GetTranslatedStringValue();
                    break;
                case WebDom.WellknownCssPropertyName.ListStyleType:
                    cssBox.ListStyleType = UserMapUtil.GetListStyleType(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.Overflow:
                    cssBox.Overflow = UserMapUtil.GetOverflow(cssValue);
                    break;
            }
        }
        /// <summary>
        /// assign property value from parent
        /// </summary>
        /// <param name="cssBox"></param>
        /// <param name="propName"></param>
        static void SetPropertyValueFromParent(CssBoxBase cssBox, CssBoxBase parentCssBox, HtmlRenderer.WebDom.WellknownCssPropertyName propName)
        {

            switch (propName)
            {
                case WebDom.WellknownCssPropertyName.BorderBottomWidth:
                    cssBox.BorderBottomWidth = parentCssBox.BorderBottomWidth;
                    break;
                case WebDom.WellknownCssPropertyName.BorderLeftWidth:
                    cssBox.BorderLeftWidth = parentCssBox.BorderLeftWidth;
                    break;
                case WebDom.WellknownCssPropertyName.BorderRightWidth:
                    cssBox.BorderRightWidth = parentCssBox.BorderRightWidth;
                    break;
                case WebDom.WellknownCssPropertyName.BorderTopWidth:
                    cssBox.BorderTopWidth = parentCssBox.BorderTopWidth;
                    break;
                case WebDom.WellknownCssPropertyName.BorderBottomStyle:
                    cssBox.BorderBottomStyle = parentCssBox.BorderBottomStyle;
                    break;
                case WebDom.WellknownCssPropertyName.BorderLeftStyle:
                    cssBox.BorderLeftStyle = parentCssBox.BorderBottomStyle;
                    break;
                case WebDom.WellknownCssPropertyName.BorderRightStyle:
                    cssBox.BorderRightStyle = parentCssBox.BorderRightStyle;
                    break;
                case WebDom.WellknownCssPropertyName.BorderTopStyle:
                    cssBox.BorderTopStyle = parentCssBox.BorderTopStyle;
                    break;
                case WebDom.WellknownCssPropertyName.BorderBottomColor:
                    cssBox.BorderBottomColor = parentCssBox.BorderBottomColor;
                    break;
                case WebDom.WellknownCssPropertyName.BorderLeftColor:
                    cssBox.BorderLeftColor = parentCssBox.BorderLeftColor;
                    break;
                case WebDom.WellknownCssPropertyName.BorderRightColor:
                    cssBox.BorderRightColor = parentCssBox.BorderRightColor;
                    break;
                case WebDom.WellknownCssPropertyName.BorderTopColor:
                    cssBox.BorderTopColor = parentCssBox.BorderTopColor;
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
                    cssBox.CornerNERadius = parentCssBox.CornerNERadius;
                    break;
                case WebDom.WellknownCssPropertyName.CornerSERadius:
                    cssBox.CornerSERadius = parentCssBox.CornerSERadius;
                    break;
                case WebDom.WellknownCssPropertyName.CornerSWRadius:
                    cssBox.CornerSWRadius = parentCssBox.CornerSWRadius;
                    break;
                case WebDom.WellknownCssPropertyName.MarginBottom:
                    cssBox.MarginBottom = parentCssBox.MarginBottom;
                    break;
                case WebDom.WellknownCssPropertyName.MarginLeft:
                    cssBox.MarginLeft = parentCssBox.MarginLeft;
                    break;
                case WebDom.WellknownCssPropertyName.MarginRight:
                    cssBox.MarginRight = parentCssBox.MarginRight;
                    break;
                case WebDom.WellknownCssPropertyName.MarginTop:
                    cssBox.MarginTop = parentCssBox.MarginTop;
                    break;
                case WebDom.WellknownCssPropertyName.PaddingBottom:
                    cssBox.PaddingBottom = parentCssBox.MarginBottom;
                    break;
                case WebDom.WellknownCssPropertyName.PaddingLeft:
                    cssBox.PaddingLeft = parentCssBox.PaddingLeft;
                    break;
                case WebDom.WellknownCssPropertyName.PaddingRight:
                    cssBox.PaddingRight = parentCssBox.PaddingRight;
                    break;
                case WebDom.WellknownCssPropertyName.PaddingTop:
                    cssBox.PaddingTop = parentCssBox.PaddingTop;
                    break;
                case WebDom.WellknownCssPropertyName.Left:
                    cssBox.Left = parentCssBox.Left;
                    break;
                case WebDom.WellknownCssPropertyName.Top:
                    cssBox.Top = parentCssBox.Top;
                    break;
                case WebDom.WellknownCssPropertyName.Width:
                    cssBox.Width = parentCssBox.Width;
                    break;
                case WebDom.WellknownCssPropertyName.MaxWidth:
                    cssBox.MaxWidth = parentCssBox.MaxWidth;
                    break;
                case WebDom.WellknownCssPropertyName.Height:
                    cssBox.Height = parentCssBox.Height;
                    break;
                case WebDom.WellknownCssPropertyName.BackgroundColor:
                    cssBox.BackgroundColor = parentCssBox.BackgroundColor;
                    break;
                case WebDom.WellknownCssPropertyName.BackgroundImage:
                    cssBox.BackgroundImageBinder = parentCssBox.BackgroundImageBinder;
                    break;
                case WebDom.WellknownCssPropertyName.BackgroundPosition:
                    cssBox.BackgroundPositionX = parentCssBox.BackgroundPositionX;
                    cssBox.BackgroundPositionY = parentCssBox.BackgroundPositionY;
                    break;
                case WebDom.WellknownCssPropertyName.BackgroundRepeat:
                    cssBox.BackgroundRepeat = parentCssBox.BackgroundRepeat;
                    break;
                case WebDom.WellknownCssPropertyName.BackgroundGradient:
                    cssBox.BackgroundGradient = parentCssBox.BackgroundGradient;
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
                    cssBox.ListStylePosition = parentCssBox.ListStylePosition;
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

        internal static void AssignPropertyValue(CssBoxBase box, CssBoxBase boxParent, WebDom.CssPropertyDeclaration decl)
        {
            if (decl.IsExpand)
            {
                return;
            }

            if (decl.MarkedAsInherit && boxParent != null)
            {
                //use parent property 
                SetPropertyValueFromParent(box, boxParent, decl.WellknownPropertyName);
            }
            else
            {
                SetPropertyValue(box, boxParent, decl);
            }
        }


        //=======================================================================================
        internal static void AssignPropertyValue(CssBoxBase.BoxSpecBase box, CssBoxBase.BoxSpecBase boxParent, WebDom.CssPropertyDeclaration decl)
        {
            if (decl.IsExpand)
            {
                return;
            }

            if (decl.MarkedAsInherit && boxParent != null)
            {
                //use parent property 
                SetPropertyValueFromParent(box, boxParent, decl.WellknownPropertyName);
            }
            else
            {
                SetPropertyValue(box, boxParent, decl);
            }
        }
        //internal static void AssignPropertyValue(CssBoxBase.BoxSpecBase box, CssBoxBase boxParent, WebDom.CssPropertyDeclaration decl)
        //{
        //    if (decl.IsExpand)
        //    {
        //        return;
        //    }

        //    if (decl.MarkedAsInherit && boxParent != null)
        //    {
        //        //use parent property 
        //        SetPropertyValueFromParent(box, boxParent, decl.WellknownPropertyName);
        //    }
        //    else
        //    {
        //        SetPropertyValue(box, boxParent, decl);
        //    }
        //}
      
        static void SetPropertyValue(CssBoxBase.BoxSpecBase cssBox, CssBoxBase.BoxSpecBase parentBox, WebDom.CssPropertyDeclaration decl)
        {
            //assign property  
            WebDom.CssCodeValueExpression cssValue = decl.GetPropertyValue(0);
            switch (decl.WellknownPropertyName)
            {
                case WebDom.WellknownCssPropertyName.Display:
                    {
                        CssDisplay display = UserMapUtil.GetDisplayType(cssValue);
                        switch (cssBox.WellknownTagName)
                        {
                            //------------------------
                            //fix definition
                            case WellknownHtmlTagName.table:
                                display = CssDisplay.Table;
                                break;
                            case WellknownHtmlTagName.tr:
                                display = CssDisplay.TableRow;
                                break;
                            case WellknownHtmlTagName.tbody:
                                display = CssDisplay.TableRowGroup;
                                break;
                            case WellknownHtmlTagName.thead:
                                display = CssDisplay.TableHeaderGroup;
                                break;
                            case WellknownHtmlTagName.tfoot:
                                display = CssDisplay.TableFooterGroup;
                                break;
                            case WellknownHtmlTagName.col:
                                display = CssDisplay.TableColumn;
                                break;
                            case WellknownHtmlTagName.colgroup:
                                display = CssDisplay.TableColumnGroup;
                                break;
                            case WellknownHtmlTagName.td:
                            case WellknownHtmlTagName.th:
                                display = CssDisplay.TableCell;
                                break;
                            case WellknownHtmlTagName.caption:
                                display = CssDisplay.TableCaption;
                                break;
                            //------------------------
                        }
                        cssBox.CssDisplay = display;

                    } break;
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
                    cssBox.BorderBottomStyle = UserMapUtil.GetBorderStyle(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.BorderLeftStyle:
                    cssBox.BorderLeftStyle = UserMapUtil.GetBorderStyle(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.BorderRightStyle:
                    cssBox.BorderRightStyle = UserMapUtil.GetBorderStyle(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.BorderTopStyle:
                    cssBox.BorderTopStyle = UserMapUtil.GetBorderStyle(cssValue);
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
                    cssBox.BorderCollapse = UserMapUtil.GetBorderCollapse(cssValue);
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
                    cssBox.BackgroundImageBinder = new ImageBinder(cssValue.GetTranslatedStringValue());
                    break;
                case WebDom.WellknownCssPropertyName.BackgroundPosition:

                    cssBox.SetBackgroundPosition(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.BackgroundRepeat:
                    cssBox.BackgroundRepeat = UserMapUtil.GetBackgroundRepeat(cssValue);
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

                case WebDom.WellknownCssPropertyName.Direction:

                    cssBox.CssDirection = UserMapUtil.GetCssDirection(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.EmptyCells:
                    cssBox.EmptyCells = UserMapUtil.GetEmptyCell(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.Float:
                    cssBox.Float = UserMapUtil.GetFloat(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.Position:
                    cssBox.Position = UserMapUtil.GetCssPosition(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.LineHeight:

                    cssBox.SetLineHeight(cssValue.AsLength());
                    break;
                case WebDom.WellknownCssPropertyName.VerticalAlign:
                    cssBox.VerticalAlign = UserMapUtil.GetVerticalAlign(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.TextIndent:
                    cssBox.TextIndent = cssValue.AsLength();
                    break;
                case WebDom.WellknownCssPropertyName.TextAlign:

                    cssBox.CssTextAlign = UserMapUtil.GetTextAlign(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.TextDecoration:
                    cssBox.TextDecoration = UserMapUtil.GetTextDecoration(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.Whitespace:
                    cssBox.WhiteSpace = UserMapUtil.GetWhitespace(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.WordBreak:
                    cssBox.WordBreak = UserMapUtil.GetWordBreak(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.Visibility:
                    cssBox.CssVisibility = UserMapUtil.GetVisibility(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.WordSpacing:
                    cssBox.WordSpacing = cssValue.AsLength();
                    break;
                case WebDom.WellknownCssPropertyName.FontFamily:
                    cssBox.FontFamily = cssValue.GetTranslatedStringValue();
                    break;
                case WebDom.WellknownCssPropertyName.FontSize:
                    cssBox.SetFontSize(parentBox, cssValue);

                    break;
                case WebDom.WellknownCssPropertyName.FontStyle:
                    cssBox.FontStyle = UserMapUtil.GetFontStyle(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.FontVariant:
                    cssBox.FontVariant = UserMapUtil.GetFontVariant(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.FontWeight:
                    cssBox.FontWeight = UserMapUtil.GetFontWeight(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.ListStyle:
                    cssBox.ListStyle = cssValue.GetTranslatedStringValue();
                    break;
                case WebDom.WellknownCssPropertyName.ListStylePosition:
                    cssBox.ListStylePosition = UserMapUtil.GetListStylePosition(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.ListStyleImage:
                    cssBox.ListStyleImage = cssValue.GetTranslatedStringValue();
                    break;
                case WebDom.WellknownCssPropertyName.ListStyleType:
                    cssBox.ListStyleType = UserMapUtil.GetListStyleType(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.Overflow:
                    cssBox.Overflow = UserMapUtil.GetOverflow(cssValue);
                    break;
            }
        }
        /// <summary>
        /// assign property value from parent
        /// </summary>
        /// <param name="cssBox"></param>
        /// <param name="propName"></param>
        static void SetPropertyValueFromParent(CssBoxBase.BoxSpecBase cssBox, CssBoxBase.BoxSpecBase parentCssBox, HtmlRenderer.WebDom.WellknownCssPropertyName propName)
        {

            switch (propName)
            {
                case WebDom.WellknownCssPropertyName.BorderBottomWidth:
                    cssBox.BorderBottomWidth = parentCssBox.BorderBottomWidth;
                    break;
                case WebDom.WellknownCssPropertyName.BorderLeftWidth:
                    cssBox.BorderLeftWidth = parentCssBox.BorderLeftWidth;
                    break;
                case WebDom.WellknownCssPropertyName.BorderRightWidth:
                    cssBox.BorderRightWidth = parentCssBox.BorderRightWidth;
                    break;
                case WebDom.WellknownCssPropertyName.BorderTopWidth:
                    cssBox.BorderTopWidth = parentCssBox.BorderTopWidth;
                    break;
                case WebDom.WellknownCssPropertyName.BorderBottomStyle:
                    cssBox.BorderBottomStyle = parentCssBox.BorderBottomStyle;
                    break;
                case WebDom.WellknownCssPropertyName.BorderLeftStyle:
                    cssBox.BorderLeftStyle = parentCssBox.BorderBottomStyle;
                    break;
                case WebDom.WellknownCssPropertyName.BorderRightStyle:
                    cssBox.BorderRightStyle = parentCssBox.BorderRightStyle;
                    break;
                case WebDom.WellknownCssPropertyName.BorderTopStyle:
                    cssBox.BorderTopStyle = parentCssBox.BorderTopStyle;
                    break;
                case WebDom.WellknownCssPropertyName.BorderBottomColor:
                    cssBox.BorderBottomColor = parentCssBox.BorderBottomColor;
                    break;
                case WebDom.WellknownCssPropertyName.BorderLeftColor:
                    cssBox.BorderLeftColor = parentCssBox.BorderLeftColor;
                    break;
                case WebDom.WellknownCssPropertyName.BorderRightColor:
                    cssBox.BorderRightColor = parentCssBox.BorderRightColor;
                    break;
                case WebDom.WellknownCssPropertyName.BorderTopColor:
                    cssBox.BorderTopColor = parentCssBox.BorderTopColor;
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
                    cssBox.CornerNERadius = parentCssBox.CornerNERadius;
                    break;
                case WebDom.WellknownCssPropertyName.CornerSERadius:
                    cssBox.CornerSERadius = parentCssBox.CornerSERadius;
                    break;
                case WebDom.WellknownCssPropertyName.CornerSWRadius:
                    cssBox.CornerSWRadius = parentCssBox.CornerSWRadius;
                    break;
                case WebDom.WellknownCssPropertyName.MarginBottom:
                    cssBox.MarginBottom = parentCssBox.MarginBottom;
                    break;
                case WebDom.WellknownCssPropertyName.MarginLeft:
                    cssBox.MarginLeft = parentCssBox.MarginLeft;
                    break;
                case WebDom.WellknownCssPropertyName.MarginRight:
                    cssBox.MarginRight = parentCssBox.MarginRight;
                    break;
                case WebDom.WellknownCssPropertyName.MarginTop:
                    cssBox.MarginTop = parentCssBox.MarginTop;
                    break;
                case WebDom.WellknownCssPropertyName.PaddingBottom:
                    cssBox.PaddingBottom = parentCssBox.MarginBottom;
                    break;
                case WebDom.WellknownCssPropertyName.PaddingLeft:
                    cssBox.PaddingLeft = parentCssBox.PaddingLeft;
                    break;
                case WebDom.WellknownCssPropertyName.PaddingRight:
                    cssBox.PaddingRight = parentCssBox.PaddingRight;
                    break;
                case WebDom.WellknownCssPropertyName.PaddingTop:
                    cssBox.PaddingTop = parentCssBox.PaddingTop;
                    break;
                case WebDom.WellknownCssPropertyName.Left:
                    cssBox.Left = parentCssBox.Left;
                    break;
                case WebDom.WellknownCssPropertyName.Top:
                    cssBox.Top = parentCssBox.Top;
                    break;
                case WebDom.WellknownCssPropertyName.Width:
                    cssBox.Width = parentCssBox.Width;
                    break;
                case WebDom.WellknownCssPropertyName.MaxWidth:
                    cssBox.MaxWidth = parentCssBox.MaxWidth;
                    break;
                case WebDom.WellknownCssPropertyName.Height:
                    cssBox.Height = parentCssBox.Height;
                    break;
                case WebDom.WellknownCssPropertyName.BackgroundColor:
                    cssBox.BackgroundColor = parentCssBox.BackgroundColor;
                    break;
                case WebDom.WellknownCssPropertyName.BackgroundImage:
                    cssBox.BackgroundImageBinder = parentCssBox.BackgroundImageBinder;
                    break;
                case WebDom.WellknownCssPropertyName.BackgroundPosition:
                    cssBox.BackgroundPositionX = parentCssBox.BackgroundPositionX;
                    cssBox.BackgroundPositionY = parentCssBox.BackgroundPositionY;
                    break;
                case WebDom.WellknownCssPropertyName.BackgroundRepeat:
                    cssBox.BackgroundRepeat = parentCssBox.BackgroundRepeat;
                    break;
                case WebDom.WellknownCssPropertyName.BackgroundGradient:
                    cssBox.BackgroundGradient = parentCssBox.BackgroundGradient;
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
                    cssBox.ListStylePosition = parentCssBox.ListStylePosition;
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