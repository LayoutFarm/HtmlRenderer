//BSD 2014, WinterDev

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
     

    static class SpecSetter
    {
        //=======================================================================================
        internal static void AssignPropertyValue(BoxSpec spec, BoxSpec parentSpec, WebDom.CssPropertyDeclaration decl)
        {
            if (decl.IsExpand)
            {
                return;
            }

            if (decl.MarkedAsInherit && parentSpec != null)
            {
                //use parent property 
                SetPropertyValueFromParent(spec, parentSpec, decl.WellknownPropertyName);
            }
            else
            {
                SetPropertyValue(spec, parentSpec, decl);
            }
        }

        
        static void SetPropertyValue(BoxSpec spec, BoxSpec parentSpec, WebDom.CssPropertyDeclaration decl)
        {
            //assign property  
            WebDom.CssCodeValueExpression cssValue = decl.GetPropertyValue(0);
            switch (decl.WellknownPropertyName)
            {
                case WebDom.WellknownCssPropertyName.Display:
                    {
                        CssDisplay display = UserMapUtil.GetDisplayType(cssValue);
                        switch (spec.WellknownTagName)
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
                        spec.CssDisplay = display;

                    } break;
                case WebDom.WellknownCssPropertyName.BorderBottomWidth:
                    spec.BorderBottomWidth = cssValue.AsBorderLength();
                    break;
                case WebDom.WellknownCssPropertyName.BorderLeftWidth:
                    spec.BorderLeftWidth = cssValue.AsBorderLength();
                    break;
                case WebDom.WellknownCssPropertyName.BorderRightWidth:
                    spec.BorderRightWidth = cssValue.AsBorderLength();
                    break;
                case WebDom.WellknownCssPropertyName.BorderTopWidth:
                    spec.BorderTopWidth = cssValue.AsBorderLength();
                    break;
                case WebDom.WellknownCssPropertyName.BorderBottomStyle:
                    spec.BorderBottomStyle = UserMapUtil.GetBorderStyle(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.BorderLeftStyle:
                    spec.BorderLeftStyle = UserMapUtil.GetBorderStyle(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.BorderRightStyle:
                    spec.BorderRightStyle = UserMapUtil.GetBorderStyle(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.BorderTopStyle:
                    spec.BorderTopStyle = UserMapUtil.GetBorderStyle(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.BorderBottomColor:
                    spec.BorderBottomColor = cssValue.AsColor();
                    break;
                case WebDom.WellknownCssPropertyName.BorderLeftColor:
                    spec.BorderLeftColor = cssValue.AsColor();
                    break;
                case WebDom.WellknownCssPropertyName.BorderRightColor:
                    spec.BorderRightColor = cssValue.AsColor();
                    break;
                case WebDom.WellknownCssPropertyName.BorderTopColor:
                    spec.BorderTopColor = cssValue.AsColor();
                    break;

                case WebDom.WellknownCssPropertyName.BorderSpacing:

                    spec.SetBorderSpacing(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.BorderCollapse:
                    spec.BorderCollapse = UserMapUtil.GetBorderCollapse(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.CornerRadius:
                    spec.SetCornerRadius(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.CornerNWRadius:
                    spec.CornerNWRadius = cssValue.AsLength();
                    break;
                case WebDom.WellknownCssPropertyName.CornerNERadius:
                    spec.CornerNERadius = cssValue.AsLength();
                    break;
                case WebDom.WellknownCssPropertyName.CornerSERadius:
                    spec.CornerSERadius = cssValue.AsLength();
                    break;
                case WebDom.WellknownCssPropertyName.CornerSWRadius:
                    spec.CornerSWRadius = cssValue.AsLength();
                    break;
                case WebDom.WellknownCssPropertyName.MarginBottom:
                    spec.MarginBottom = cssValue.AsTranslatedLength();
                    break;
                case WebDom.WellknownCssPropertyName.MarginLeft:
                    spec.MarginLeft = cssValue.AsTranslatedLength();
                    break;
                case WebDom.WellknownCssPropertyName.MarginRight:
                    spec.MarginRight = cssValue.AsTranslatedLength();
                    break;
                case WebDom.WellknownCssPropertyName.MarginTop:
                    spec.MarginTop = cssValue.AsTranslatedLength();
                    break;
                case WebDom.WellknownCssPropertyName.PaddingBottom:
                    spec.PaddingBottom = cssValue.AsTranslatedLength();
                    break;
                case WebDom.WellknownCssPropertyName.PaddingLeft:
                    spec.PaddingLeft = cssValue.AsTranslatedLength();
                    break;
                case WebDom.WellknownCssPropertyName.PaddingRight:
                    spec.PaddingRight = cssValue.AsTranslatedLength();
                    break;
                case WebDom.WellknownCssPropertyName.PaddingTop:
                    spec.PaddingTop = cssValue.AsTranslatedLength();
                    break;
                case WebDom.WellknownCssPropertyName.Left:
                    spec.Left = cssValue.AsLength();
                    break;
                case WebDom.WellknownCssPropertyName.Top:
                    spec.Top = cssValue.AsLength();
                    break;
                case WebDom.WellknownCssPropertyName.Width:
                    spec.Width = cssValue.AsLength();
                    break;
                case WebDom.WellknownCssPropertyName.MaxWidth:
                    spec.MaxWidth = cssValue.AsLength();
                    break;
                case WebDom.WellknownCssPropertyName.Height:
                    spec.Height = cssValue.AsLength();
                    break;
                case WebDom.WellknownCssPropertyName.BackgroundColor:
                    spec.BackgroundColor = cssValue.AsColor();
                    break;
                case WebDom.WellknownCssPropertyName.BackgroundImage:
                    spec.BackgroundImageBinder = new ImageBinder(cssValue.GetTranslatedStringValue());
                    break;
                case WebDom.WellknownCssPropertyName.BackgroundPosition:

                    spec.SetBackgroundPosition(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.BackgroundRepeat:
                    spec.BackgroundRepeat = UserMapUtil.GetBackgroundRepeat(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.BackgroundGradient:
                    spec.BackgroundGradient = cssValue.AsColor();
                    break;
                case WebDom.WellknownCssPropertyName.BackgroundGradientAngle:
                    {
                        spec.BackgroundGradientAngle = cssValue.AsNumber();

                        //float angle;
                        //if (float.TryParse(cssValue.GetTranslatedStringValue(), out angle))
                        //{
                        //    cssBox.BackgroundGradientAngle = angle;
                        //}
                    } break;
                case WebDom.WellknownCssPropertyName.Color:
                    spec.Color = cssValue.AsColor();
                    break;

                case WebDom.WellknownCssPropertyName.Direction:

                    spec.CssDirection = UserMapUtil.GetCssDirection(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.EmptyCells:
                    spec.EmptyCells = UserMapUtil.GetEmptyCell(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.Float:
                    spec.Float = UserMapUtil.GetFloat(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.Position:
                    spec.Position = UserMapUtil.GetCssPosition(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.LineHeight:
                    spec.LineHeight = cssValue.AsLength();

                    break;
                case WebDom.WellknownCssPropertyName.VerticalAlign:
                    spec.VerticalAlign = UserMapUtil.GetVerticalAlign(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.TextIndent:
                    spec.TextIndent = cssValue.AsLength();
                    break;
                case WebDom.WellknownCssPropertyName.TextAlign:

                    spec.CssTextAlign = UserMapUtil.GetTextAlign(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.TextDecoration:
                    spec.TextDecoration = UserMapUtil.GetTextDecoration(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.Whitespace:
                    spec.WhiteSpace = UserMapUtil.GetWhitespace(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.WordBreak:
                    spec.WordBreak = UserMapUtil.GetWordBreak(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.Visibility:
                    spec.Visibility = UserMapUtil.GetVisibility(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.WordSpacing:
                    spec.WordSpacing = cssValue.AsLength();
                    break;
                case WebDom.WellknownCssPropertyName.FontFamily:
                    spec.FontFamily = cssValue.GetTranslatedStringValue();
                    break;
                case WebDom.WellknownCssPropertyName.FontSize:
                    spec.SetFontSize(parentSpec, cssValue);

                    break;
                case WebDom.WellknownCssPropertyName.FontStyle:
                    spec.FontStyle = UserMapUtil.GetFontStyle(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.FontVariant:
                    spec.FontVariant = UserMapUtil.GetFontVariant(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.FontWeight:
                    spec.FontWeight = UserMapUtil.GetFontWeight(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.ListStyle:
                    spec.ListStyle = cssValue.GetTranslatedStringValue();
                    break;
                case WebDom.WellknownCssPropertyName.ListStylePosition:
                    spec.ListStylePosition = UserMapUtil.GetListStylePosition(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.ListStyleImage:
                    spec.ListStyleImage = cssValue.GetTranslatedStringValue();
                    break;
                case WebDom.WellknownCssPropertyName.ListStyleType:
                    spec.ListStyleType = UserMapUtil.GetListStyleType(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.Overflow:
                    spec.Overflow = UserMapUtil.GetOverflow(cssValue);
                    break;
            }
        }
        /// <summary>
        /// assign property value from parent
        /// </summary>
        /// <param name="spec"></param>
        /// <param name="propName"></param>
        static void SetPropertyValueFromParent(BoxSpec spec, BoxSpec parentSpec, HtmlRenderer.WebDom.WellknownCssPropertyName propName)
        {

            switch (propName)
            {
                case WebDom.WellknownCssPropertyName.BorderBottomWidth:
                    spec.BorderBottomWidth = parentSpec.BorderBottomWidth;
                    break;
                case WebDom.WellknownCssPropertyName.BorderLeftWidth:
                    spec.BorderLeftWidth = parentSpec.BorderLeftWidth;
                    break;
                case WebDom.WellknownCssPropertyName.BorderRightWidth:
                    spec.BorderRightWidth = parentSpec.BorderRightWidth;
                    break;
                case WebDom.WellknownCssPropertyName.BorderTopWidth:
                    spec.BorderTopWidth = parentSpec.BorderTopWidth;
                    break;
                case WebDom.WellknownCssPropertyName.BorderBottomStyle:
                    spec.BorderBottomStyle = parentSpec.BorderBottomStyle;
                    break;
                case WebDom.WellknownCssPropertyName.BorderLeftStyle:
                    spec.BorderLeftStyle = parentSpec.BorderBottomStyle;
                    break;
                case WebDom.WellknownCssPropertyName.BorderRightStyle:
                    spec.BorderRightStyle = parentSpec.BorderRightStyle;
                    break;
                case WebDom.WellknownCssPropertyName.BorderTopStyle:
                    spec.BorderTopStyle = parentSpec.BorderTopStyle;
                    break;
                case WebDom.WellknownCssPropertyName.BorderBottomColor:
                    spec.BorderBottomColor = parentSpec.BorderBottomColor;
                    break;
                case WebDom.WellknownCssPropertyName.BorderLeftColor:
                    spec.BorderLeftColor = parentSpec.BorderLeftColor;
                    break;
                case WebDom.WellknownCssPropertyName.BorderRightColor:
                    spec.BorderRightColor = parentSpec.BorderRightColor;
                    break;
                case WebDom.WellknownCssPropertyName.BorderTopColor:
                    spec.BorderTopColor = parentSpec.BorderTopColor;
                    break;
                case WebDom.WellknownCssPropertyName.BorderSpacing:

                    spec.BorderSpacingHorizontal = parentSpec.BorderSpacingHorizontal;
                    spec.BorderSpacingVertical = parentSpec.BorderSpacingVertical;
                    break;
                case WebDom.WellknownCssPropertyName.BorderCollapse:
                    spec.BorderCollapse = parentSpec.BorderCollapse;
                    break;
                case WebDom.WellknownCssPropertyName.CornerRadius:
                    spec.CornerNERadius = parentSpec.CornerNERadius;
                    spec.CornerNWRadius = parentSpec.CornerNWRadius;
                    spec.CornerSERadius = parentSpec.CornerSERadius;
                    spec.CornerSWRadius = parentSpec.CornerSWRadius;
                    break;
                case WebDom.WellknownCssPropertyName.CornerNWRadius:

                    spec.CornerNWRadius = parentSpec.CornerNWRadius;
                    break;
                case WebDom.WellknownCssPropertyName.CornerNERadius:
                    spec.CornerNERadius = parentSpec.CornerNERadius;
                    break;
                case WebDom.WellknownCssPropertyName.CornerSERadius:
                    spec.CornerSERadius = parentSpec.CornerSERadius;
                    break;
                case WebDom.WellknownCssPropertyName.CornerSWRadius:
                    spec.CornerSWRadius = parentSpec.CornerSWRadius;
                    break;
                case WebDom.WellknownCssPropertyName.MarginBottom:
                    spec.MarginBottom = parentSpec.MarginBottom;
                    break;
                case WebDom.WellknownCssPropertyName.MarginLeft:
                    spec.MarginLeft = parentSpec.MarginLeft;
                    break;
                case WebDom.WellknownCssPropertyName.MarginRight:
                    spec.MarginRight = parentSpec.MarginRight;
                    break;
                case WebDom.WellknownCssPropertyName.MarginTop:
                    spec.MarginTop = parentSpec.MarginTop;
                    break;
                case WebDom.WellknownCssPropertyName.PaddingBottom:
                    spec.PaddingBottom = parentSpec.MarginBottom;
                    break;
                case WebDom.WellknownCssPropertyName.PaddingLeft:
                    spec.PaddingLeft = parentSpec.PaddingLeft;
                    break;
                case WebDom.WellknownCssPropertyName.PaddingRight:
                    spec.PaddingRight = parentSpec.PaddingRight;
                    break;
                case WebDom.WellknownCssPropertyName.PaddingTop:
                    spec.PaddingTop = parentSpec.PaddingTop;
                    break;
                case WebDom.WellknownCssPropertyName.Left:
                    spec.Left = parentSpec.Left;
                    break;
                case WebDom.WellknownCssPropertyName.Top:
                    spec.Top = parentSpec.Top;
                    break;
                case WebDom.WellknownCssPropertyName.Width:
                    spec.Width = parentSpec.Width;
                    break;
                case WebDom.WellknownCssPropertyName.MaxWidth:
                    spec.MaxWidth = parentSpec.MaxWidth;
                    break;
                case WebDom.WellknownCssPropertyName.Height:
                    spec.Height = parentSpec.Height;
                    break;
                case WebDom.WellknownCssPropertyName.BackgroundColor:
                    spec.BackgroundColor = parentSpec.BackgroundColor;
                    break;
                case WebDom.WellknownCssPropertyName.BackgroundImage:
                    spec.BackgroundImageBinder = parentSpec.BackgroundImageBinder;
                    break;
                case WebDom.WellknownCssPropertyName.BackgroundPosition:
                    spec.BackgroundPositionX = parentSpec.BackgroundPositionX;
                    spec.BackgroundPositionY = parentSpec.BackgroundPositionY;
                    break;
                case WebDom.WellknownCssPropertyName.BackgroundRepeat:
                    spec.BackgroundRepeat = parentSpec.BackgroundRepeat;
                    break;
                case WebDom.WellknownCssPropertyName.BackgroundGradient:
                    spec.BackgroundGradient = parentSpec.BackgroundGradient;
                    break;
                case WebDom.WellknownCssPropertyName.BackgroundGradientAngle:
                    {
                        spec.BackgroundGradientAngle = parentSpec.BackgroundGradientAngle;
                        //float angle;
                        //if (float.TryParse(value, out angle))
                        //{
                        //    cssBox.BackgroundGradientAngle = angle;
                        //}
                    } break;
                case WebDom.WellknownCssPropertyName.Color:
                    spec.Color = parentSpec.Color;
                    break;
                case WebDom.WellknownCssPropertyName.Display:
                    spec.CssDisplay = parentSpec.CssDisplay;
                    break;
                case WebDom.WellknownCssPropertyName.Direction:
                    spec.CssDirection = parentSpec.CssDirection;
                    break;
                case WebDom.WellknownCssPropertyName.EmptyCells:
                    spec.EmptyCells = parentSpec.EmptyCells;
                    break;
                case WebDom.WellknownCssPropertyName.Float:
                    spec.Float = parentSpec.Float;
                    break;
                case WebDom.WellknownCssPropertyName.Position:
                    spec.Position = parentSpec.Position;
                    break;
                case WebDom.WellknownCssPropertyName.LineHeight:
                    spec.LineHeight = parentSpec.LineHeight;
                    break;
                case WebDom.WellknownCssPropertyName.VerticalAlign:
                    spec.VerticalAlign = parentSpec.VerticalAlign;
                    break;
                case WebDom.WellknownCssPropertyName.TextIndent:
                    spec.TextIndent = parentSpec.TextIndent;
                    break;
                case WebDom.WellknownCssPropertyName.TextAlign:
                    spec.CssTextAlign = parentSpec.CssTextAlign;
                    break;
                case WebDom.WellknownCssPropertyName.TextDecoration:
                    spec.TextDecoration = parentSpec.TextDecoration;
                    break;
                case WebDom.WellknownCssPropertyName.Whitespace:
                    spec.WhiteSpace = parentSpec.WhiteSpace;
                    break;
                case WebDom.WellknownCssPropertyName.WordBreak:
                    spec.WordBreak = parentSpec.WordBreak;
                    break;
                case WebDom.WellknownCssPropertyName.Visibility:
                    spec.Visibility = parentSpec.Visibility;
                    break;
                case WebDom.WellknownCssPropertyName.WordSpacing:
                    spec.WordSpacing = parentSpec.WordSpacing;
                    break;
                case WebDom.WellknownCssPropertyName.FontFamily:
                    spec.FontFamily = parentSpec.FontFamily;
                    //cssBox.FontFamily = value;
                    break;
                case WebDom.WellknownCssPropertyName.FontSize:
                    spec.FontSize = parentSpec.FontSize;
                    break;
                case WebDom.WellknownCssPropertyName.FontStyle:
                    spec.FontStyle = parentSpec.FontStyle;
                    break;
                case WebDom.WellknownCssPropertyName.FontVariant:
                    spec.FontVariant = parentSpec.FontVariant;

                    break;
                case WebDom.WellknownCssPropertyName.FontWeight:
                    spec.FontWeight = parentSpec.FontWeight;

                    break;
                case WebDom.WellknownCssPropertyName.ListStyle:

                    spec.ListStyle = parentSpec.ListStyle;
                    break;
                case WebDom.WellknownCssPropertyName.ListStylePosition:
                    spec.ListStylePosition = parentSpec.ListStylePosition;
                    break;
                case WebDom.WellknownCssPropertyName.ListStyleImage:

                    spec.ListStyleImage = parentSpec.ListStyleImage;
                    break;
                case WebDom.WellknownCssPropertyName.ListStyleType:

                    spec.ListStyleType = parentSpec.ListStyleType;
                    break;
                case WebDom.WellknownCssPropertyName.Overflow:

                    spec.Overflow = parentSpec.Overflow;
                    break;
            }
        }

    }

}