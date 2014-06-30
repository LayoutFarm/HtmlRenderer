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

        internal static void AssignPropertyValue(BoxSpec target, BoxSpec parent, CssPropertyDeclaration decl)
        {
            if (decl.IsExpand)
            {
                return;
            }

            if (decl.MarkedAsInherit && parent != null)
            {
                //use parent property 
                SetPropertyValueFromParent(target, parent, decl.WellknownPropertyName);
            }
            else
            {
                SetPropertyValue(target, parent, decl);
            }
        }

        static void SetPropertyValue(BoxSpec target, BoxSpec parent, CssPropertyDeclaration decl)
        {
            //assign property  
            WebDom.CssCodeValueExpression cssValue = decl.GetPropertyValue(0);
            switch (decl.WellknownPropertyName)
            {
                case WebDom.WellknownCssPropertyName.Display:
                    {
                        target.SetCssDisplay(UserMapUtil.GetDisplayType(cssValue));
                        //switch (target.WellknownTagName)
                        //{
                        //    //------------------------
                        //    //fix definition
                        //    case WellknownHtmlTagName.table:
                        //        display = CssDisplay.Table;
                        //        break;
                        //    case WellknownHtmlTagName.tr:
                        //        display = CssDisplay.TableRow;
                        //        break;
                        //    case WellknownHtmlTagName.tbody:
                        //        display = CssDisplay.TableRowGroup;
                        //        break;
                        //    case WellknownHtmlTagName.thead:
                        //        display = CssDisplay.TableHeaderGroup;
                        //        break;
                        //    case WellknownHtmlTagName.tfoot:
                        //        display = CssDisplay.TableFooterGroup;
                        //        break;
                        //    case WellknownHtmlTagName.col:
                        //        display = CssDisplay.TableColumn;
                        //        break;
                        //    case WellknownHtmlTagName.colgroup:
                        //        display = CssDisplay.TableColumnGroup;
                        //        break;
                        //    case WellknownHtmlTagName.td:
                        //    case WellknownHtmlTagName.th:
                        //        display = CssDisplay.TableCell;
                        //        break;
                        //    case WellknownHtmlTagName.caption:
                        //        display = CssDisplay.TableCaption;
                        //        break;
                        //    //------------------------
                        //}

                    } break;
                case WebDom.WellknownCssPropertyName.BorderBottomWidth:
                    target.SetBorderWidth(CssSide.Bottom, cssValue.AsBorderLength());
                    break;
                case WebDom.WellknownCssPropertyName.BorderLeftWidth:
                    target.SetBorderWidth(CssSide.Left, cssValue.AsBorderLength());
                    break;
                case WebDom.WellknownCssPropertyName.BorderRightWidth:
                    target.SetBorderWidth(CssSide.Right, cssValue.AsBorderLength());
                    break;
                case WebDom.WellknownCssPropertyName.BorderTopWidth:
                    target.SetBorderWidth(CssSide.Top, cssValue.AsBorderLength());
                    break;
                case WebDom.WellknownCssPropertyName.BorderBottomStyle:
                    target.SetBorderStyle(CssSide.Bottom, UserMapUtil.GetBorderStyle(cssValue));
                    break;
                case WebDom.WellknownCssPropertyName.BorderLeftStyle:
                    target.SetBorderStyle(CssSide.Left, UserMapUtil.GetBorderStyle(cssValue));
                    break;
                case WebDom.WellknownCssPropertyName.BorderRightStyle:
                    target.SetBorderStyle(CssSide.Right, UserMapUtil.GetBorderStyle(cssValue));
                    break;
                case WebDom.WellknownCssPropertyName.BorderTopStyle:
                    target.SetBorderStyle(CssSide.Top, UserMapUtil.GetBorderStyle(cssValue));
                    break;
                case WebDom.WellknownCssPropertyName.BorderBottomColor:
                    target.SetBorderColor(CssSide.Bottom, cssValue.AsColor());
                    break;
                case WebDom.WellknownCssPropertyName.BorderLeftColor:
                    target.SetBorderColor(CssSide.Left, cssValue.AsColor());
                    break;
                case WebDom.WellknownCssPropertyName.BorderRightColor:
                    target.SetBorderColor(CssSide.Right, cssValue.AsColor());
                    break;
                case WebDom.WellknownCssPropertyName.BorderTopColor:
                    target.SetBorderColor(CssSide.Top, cssValue.AsColor());
                    break;
                case WebDom.WellknownCssPropertyName.BorderSpacing:
                    target.SetBorderSpacing(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.BorderCollapse:
                    target.BorderCollapse = UserMapUtil.GetBorderCollapse(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.CornerRadius:
                    target.SetCornerRadius(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.CornerNWRadius:
                    target.CornerNWRadius = cssValue.AsLength();
                    break;
                case WebDom.WellknownCssPropertyName.CornerNERadius:
                    target.CornerNERadius = cssValue.AsLength();
                    break;
                case WebDom.WellknownCssPropertyName.CornerSERadius:
                    target.CornerSERadius = cssValue.AsLength();
                    break;
                case WebDom.WellknownCssPropertyName.CornerSWRadius:
                    target.CornerSWRadius = cssValue.AsLength();
                    break;

                case WebDom.WellknownCssPropertyName.MarginBottom:
                    target.SetMargin(CssSide.Bottom, cssValue.AsTranslatedLength());
                    break;
                case WebDom.WellknownCssPropertyName.MarginLeft:
                    target.SetMargin(CssSide.Left, cssValue.AsTranslatedLength());
                    break;
                case WebDom.WellknownCssPropertyName.MarginRight:
                    target.SetMargin(CssSide.Right, cssValue.AsTranslatedLength());
                    break;
                case WebDom.WellknownCssPropertyName.MarginTop:
                    target.SetMargin(CssSide.Top, cssValue.AsTranslatedLength());
                    break;

                case WebDom.WellknownCssPropertyName.PaddingBottom:
                    target.SetPadding(CssSide.Bottom, cssValue.AsTranslatedLength());
                    break;
                case WebDom.WellknownCssPropertyName.PaddingLeft:
                    target.SetPadding(CssSide.Left, cssValue.AsTranslatedLength());
                    break;
                case WebDom.WellknownCssPropertyName.PaddingRight:
                    target.SetPadding(CssSide.Right, cssValue.AsTranslatedLength());
                    break;
                case WebDom.WellknownCssPropertyName.PaddingTop:
                    target.SetPadding(CssSide.Top, cssValue.AsTranslatedLength());
                    break;

                case WebDom.WellknownCssPropertyName.Left:
                    target.Left = cssValue.AsLength();
                    break;
                case WebDom.WellknownCssPropertyName.Top:
                    target.Top = cssValue.AsLength();
                    break;
                case WebDom.WellknownCssPropertyName.Width:
                    target.Width = cssValue.AsLength();
                    break;
                case WebDom.WellknownCssPropertyName.MaxWidth:
                    target.MaxWidth = cssValue.AsLength();
                    break;
                case WebDom.WellknownCssPropertyName.Height:
                    target.Height = cssValue.AsLength();
                    break;
                case WebDom.WellknownCssPropertyName.BackgroundColor:
                    target.BackgroundColor = cssValue.AsColor();
                    break;
                case WebDom.WellknownCssPropertyName.BackgroundImage:
                    target.BackgroundImageBinder = new ImageBinder(cssValue.GetTranslatedStringValue());
                    break;
                case WebDom.WellknownCssPropertyName.BackgroundPosition:

                    target.SetBackgroundPosition(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.BackgroundRepeat:
                    target.BackgroundRepeat = UserMapUtil.GetBackgroundRepeat(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.BackgroundGradient:
                    target.BackgroundGradient = cssValue.AsColor();
                    break;
                case WebDom.WellknownCssPropertyName.BackgroundGradientAngle:
                    {
                        target.BackgroundGradientAngle = cssValue.AsNumber();

                        //float angle;
                        //if (float.TryParse(cssValue.GetTranslatedStringValue(), out angle))
                        //{
                        //    cssBox.BackgroundGradientAngle = angle;
                        //}
                    } break;
                case WebDom.WellknownCssPropertyName.Color:
                    target.Color = cssValue.AsColor();
                    break;

                case WebDom.WellknownCssPropertyName.Direction:

                    target.SetCssDirection(UserMapUtil.GetCssDirection(cssValue));
                    break;
                case WebDom.WellknownCssPropertyName.EmptyCells:
                    target.EmptyCells = UserMapUtil.GetEmptyCell(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.Float:
                    target.Float = UserMapUtil.GetFloat(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.Position:
                    target.Position = UserMapUtil.GetCssPosition(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.LineHeight:
                    //2014,
                    //from www.w3c.org/wiki/Css/Properties/line-height

                    //line height in <percentage> : 
                    //The computed value if the property is percentage multiplied by the 
                    //element's computed font size. 

                    target.LineHeight = cssValue.AsLength();
                    break;
                case WebDom.WellknownCssPropertyName.VerticalAlign:
                    target.VerticalAlign = UserMapUtil.GetVerticalAlign(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.TextIndent:
                    target.TextIndent = cssValue.AsLength();
                    break;
                case WebDom.WellknownCssPropertyName.TextAlign:

                    target.CssTextAlign = UserMapUtil.GetTextAlign(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.TextDecoration:
                    target.TextDecoration = UserMapUtil.GetTextDecoration(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.Whitespace:
                    target.WhiteSpace = UserMapUtil.GetWhitespace(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.WordBreak:
                    target.WordBreak = UserMapUtil.GetWordBreak(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.Visibility:
                    target.CssVisibility = UserMapUtil.GetVisibility(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.WordSpacing:
                    target.WordSpacing = cssValue.AsLength();
                    break;
                case WebDom.WellknownCssPropertyName.FontFamily:
                    target.FontFamily = cssValue.GetTranslatedStringValue();
                    break;
                case WebDom.WellknownCssPropertyName.FontSize:
                    target.SetFontSize(parent, cssValue);

                    break;
                case WebDom.WellknownCssPropertyName.FontStyle:
                    target.FontStyle = UserMapUtil.GetFontStyle(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.FontVariant:
                    target.FontVariant = UserMapUtil.GetFontVariant(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.FontWeight:
                    target.FontWeight = UserMapUtil.GetFontWeight(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.ListStyle:
                    target.ListStyle = cssValue.GetTranslatedStringValue();
                    break;
                case WebDom.WellknownCssPropertyName.ListStylePosition:
                    target.ListStylePosition = UserMapUtil.GetListStylePosition(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.ListStyleImage:
                    target.ListStyleImage = cssValue.GetTranslatedStringValue();
                    break;
                case WebDom.WellknownCssPropertyName.ListStyleType:
                    target.ListStyleType = UserMapUtil.GetListStyleType(cssValue);
                    break;
                case WebDom.WellknownCssPropertyName.Overflow:
                    target.Overflow = UserMapUtil.GetOverflow(cssValue);
                    break;
            }
        }


        /// <summary>
        /// assign property value from parent
        /// </summary>
        /// <param name="target"></param>
        /// <param name="propName"></param>
        static void SetPropertyValueFromParent(BoxSpec target, BoxSpec parent, WellknownCssPropertyName propName)
        {

            switch (propName)
            {
                case WebDom.WellknownCssPropertyName.BorderBottomWidth:
                    target.SetBorderWidth(CssSide.Bottom, parent.BorderBottomWidth);
                    break;
                case WebDom.WellknownCssPropertyName.BorderLeftWidth:
                    target.SetBorderWidth(CssSide.Left, parent.BorderLeftWidth);
                    break;
                case WebDom.WellknownCssPropertyName.BorderRightWidth:
                    target.SetBorderWidth(CssSide.Right, parent.BorderRightWidth);
                    break;
                case WebDom.WellknownCssPropertyName.BorderTopWidth:
                    target.SetBorderWidth(CssSide.Top, parent.BorderTopWidth);
                    break;

                case WebDom.WellknownCssPropertyName.BorderBottomStyle:
                    target.SetBorderStyle(CssSide.Bottom, parent.BorderBottomStyle);
                    break;
                case WebDom.WellknownCssPropertyName.BorderLeftStyle:
                    target.SetBorderStyle(CssSide.Left, parent.BorderLeftStyle);
                    break;
                case WebDom.WellknownCssPropertyName.BorderRightStyle:
                    target.SetBorderStyle(CssSide.Right, parent.BorderRightStyle);
                    break;
                case WebDom.WellknownCssPropertyName.BorderTopStyle:
                    target.SetBorderStyle(CssSide.Top, parent.BorderTopStyle);
                    break;

                case WebDom.WellknownCssPropertyName.BorderBottomColor:
                    target.SetBorderColor(CssSide.Bottom, parent.BorderTopColor);
                    break;
                case WebDom.WellknownCssPropertyName.BorderLeftColor:
                    target.SetBorderColor(CssSide.Left, parent.BorderLeftColor);
                    break;
                case WebDom.WellknownCssPropertyName.BorderRightColor:
                    target.SetBorderColor(CssSide.Right, parent.BorderRightColor);
                    break;
                case WebDom.WellknownCssPropertyName.BorderTopColor:
                    target.SetBorderColor(CssSide.Top, parent.BorderTopColor);

                    break;
                case WebDom.WellknownCssPropertyName.BorderSpacing:

                    target.BorderSpacingHorizontal = parent.BorderSpacingHorizontal;
                    target.BorderSpacingVertical = parent.BorderSpacingVertical;
                    break;
                case WebDom.WellknownCssPropertyName.BorderCollapse:
                    target.BorderCollapse = parent.BorderCollapse;
                    break;
                case WebDom.WellknownCssPropertyName.CornerRadius:
                    target.CornerNERadius = parent.CornerNERadius;
                    target.CornerNWRadius = parent.CornerNWRadius;
                    target.CornerSERadius = parent.CornerSERadius;
                    target.CornerSWRadius = parent.CornerSWRadius;
                    break;
                case WebDom.WellknownCssPropertyName.CornerNWRadius:

                    target.CornerNWRadius = parent.CornerNWRadius;
                    break;
                case WebDom.WellknownCssPropertyName.CornerNERadius:
                    target.CornerNERadius = parent.CornerNERadius;
                    break;
                case WebDom.WellknownCssPropertyName.CornerSERadius:
                    target.CornerSERadius = parent.CornerSERadius;
                    break;
                case WebDom.WellknownCssPropertyName.CornerSWRadius:
                    target.CornerSWRadius = parent.CornerSWRadius;
                    break;

                case WebDom.WellknownCssPropertyName.MarginBottom:
                    target.SetMargin(CssSide.Bottom, parent.MarginBottom);
                    break;
                case WebDom.WellknownCssPropertyName.MarginLeft:
                    target.SetMargin(CssSide.Left, parent.MarginLeft);
                    break;
                case WebDom.WellknownCssPropertyName.MarginRight:
                    target.SetMargin(CssSide.Right, parent.MarginRight);
                    break;
                case WebDom.WellknownCssPropertyName.MarginTop:
                    target.SetMargin(CssSide.Top, parent.MarginTop);

                    break;
                case WebDom.WellknownCssPropertyName.PaddingBottom:
                    target.SetPadding(CssSide.Bottom, parent.PaddingBottom);
                    break;
                case WebDom.WellknownCssPropertyName.PaddingLeft:
                    target.SetPadding(CssSide.Left, parent.PaddingLeft);
                    break;
                case WebDom.WellknownCssPropertyName.PaddingRight:
                    target.SetPadding(CssSide.Right, parent.PaddingRight);
                    break;
                case WebDom.WellknownCssPropertyName.PaddingTop:
                    target.SetPadding(CssSide.Top, parent.PaddingTop);
                    break;
                case WebDom.WellknownCssPropertyName.Left:
                    target.Left = parent.Left;
                    break;
                case WebDom.WellknownCssPropertyName.Top:
                    target.Top = parent.Top;
                    break;
                case WebDom.WellknownCssPropertyName.Width:
                    target.Width = parent.Width;
                    break;
                case WebDom.WellknownCssPropertyName.MaxWidth:
                    target.MaxWidth = parent.MaxWidth;
                    break;
                case WebDom.WellknownCssPropertyName.Height:
                    target.Height = parent.Height;
                    break;
                case WebDom.WellknownCssPropertyName.BackgroundColor:
                    target.BackgroundColor = parent.BackgroundColor;
                    break;
                case WebDom.WellknownCssPropertyName.BackgroundImage:
                    target.BackgroundImageBinder = parent.BackgroundImageBinder;
                    break;
                case WebDom.WellknownCssPropertyName.BackgroundPosition:
                    target.BackgroundPositionX = parent.BackgroundPositionX;
                    target.BackgroundPositionY = parent.BackgroundPositionY;
                    break;
                case WebDom.WellknownCssPropertyName.BackgroundRepeat:
                    target.BackgroundRepeat = parent.BackgroundRepeat;
                    break;
                case WebDom.WellknownCssPropertyName.BackgroundGradient:
                    target.BackgroundGradient = parent.BackgroundGradient;
                    break;
                case WebDom.WellknownCssPropertyName.BackgroundGradientAngle:
                    {
                        target.BackgroundGradientAngle = parent.BackgroundGradientAngle;
                        //float angle;
                        //if (float.TryParse(value, out angle))
                        //{
                        //    cssBox.BackgroundGradientAngle = angle;
                        //}
                    } break;
                case WebDom.WellknownCssPropertyName.Color:
                    target.Color = parent.Color;
                    break;
                case WebDom.WellknownCssPropertyName.Display:
                    //target.CssDisplay = parent.CssDisplay;
                    target.SetCssDisplay(parent.CssDisplay);
                    break;
                case WebDom.WellknownCssPropertyName.Direction:

                    target.SetCssDirection(parent.CssDirection);
                    break;
                case WebDom.WellknownCssPropertyName.EmptyCells:
                    target.EmptyCells = parent.EmptyCells;
                    break;
                case WebDom.WellknownCssPropertyName.Float:
                    target.Float = parent.Float;
                    break;
                case WebDom.WellknownCssPropertyName.Position:
                    target.Position = parent.Position;
                    break;
                case WebDom.WellknownCssPropertyName.LineHeight:
                    target.LineHeight = parent.LineHeight;
                    break;
                case WebDom.WellknownCssPropertyName.VerticalAlign:
                    target.VerticalAlign = parent.VerticalAlign;
                    break;
                case WebDom.WellknownCssPropertyName.TextIndent:
                    target.TextIndent = parent.TextIndent;
                    break;
                case WebDom.WellknownCssPropertyName.TextAlign:
                    target.CssTextAlign = parent.CssTextAlign;
                    break;
                case WebDom.WellknownCssPropertyName.TextDecoration:
                    target.TextDecoration = parent.TextDecoration;
                    break;
                case WebDom.WellknownCssPropertyName.Whitespace:
                    target.WhiteSpace = parent.WhiteSpace;
                    break;
                case WebDom.WellknownCssPropertyName.WordBreak:
                    target.WordBreak = parent.WordBreak;
                    break;
                case WebDom.WellknownCssPropertyName.Visibility:
                    target.CssVisibility = parent.CssVisibility;
                    break;
                case WebDom.WellknownCssPropertyName.WordSpacing:
                    target.WordSpacing = parent.WordSpacing;
                    break;
                case WebDom.WellknownCssPropertyName.FontFamily:
                    target.FontFamily = parent.FontFamily;
                    //cssBox.FontFamily = value;
                    break;
                case WebDom.WellknownCssPropertyName.FontSize:
                    target.FontSize = parent.FontSize;
                    break;
                case WebDom.WellknownCssPropertyName.FontStyle:
                    target.FontStyle = parent.FontStyle;
                    break;
                case WebDom.WellknownCssPropertyName.FontVariant:
                    target.FontVariant = parent.FontVariant;

                    break;
                case WebDom.WellknownCssPropertyName.FontWeight:
                    target.FontWeight = parent.FontWeight;

                    break;
                case WebDom.WellknownCssPropertyName.ListStyle:

                    target.ListStyle = parent.ListStyle;
                    break;
                case WebDom.WellknownCssPropertyName.ListStylePosition:
                    target.ListStylePosition = parent.ListStylePosition;
                    break;
                case WebDom.WellknownCssPropertyName.ListStyleImage:

                    target.ListStyleImage = parent.ListStyleImage;
                    break;
                case WebDom.WellknownCssPropertyName.ListStyleType:

                    target.ListStyleType = parent.ListStyleType;
                    break;
                case WebDom.WellknownCssPropertyName.Overflow:

                    target.Overflow = parent.Overflow;
                    break;
            }
        }

    }



}