// 2015,2014 ,BSD, WinterDev 
//ArthurHub  , Jose Manuel Menendez Poo

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
using PixelFarm.Drawing;
using LayoutFarm.WebDom;
using LayoutFarm.Css;
using LayoutFarm.HtmlBoxes;

namespace LayoutFarm.Composers
{


    public static class SpecSetter
    {
        internal static Color AsColor(WebDom.CssCodeValueExpression value)
        {
            return CssColorConv.ConvertToActualColor(UserMapUtil.AsColor(value));
        }
        //=======================================================================================
        public static void AssignPropertyValue(BoxSpec spec, BoxSpec parentSpec, CssPropertyDeclaration decl)
        {
            if (decl.IsExpand)
            {
                return;
            }

            if (decl.MarkedAsInherit && parentSpec != null)
            {
                SetPropertyValueFromParent(spec, parentSpec, decl.WellknownPropertyName);
            }
            else
            {
                SetPropertyValue(spec, decl);
            }
        }
        static void SetBackgroundPosition(this CssBox box, WebDom.CssCodeValueExpression value)
        {
            //TODO: implement background position from combination value
            throw new NotSupportedException();
        }
        static void SetBackgroundPosition(this BoxSpec box, WebDom.CssCodeValueExpression value)
        {
            //TODO: implement background position from combination value
            throw new NotSupportedException();
        }
        static void SetBorderSpacing(BoxSpec box, WebDom.CssCodeValueExpression value)
        {
            WebDom.CssCodePrimitiveExpression primValue = value as WebDom.CssCodePrimitiveExpression;
            if (primValue == null)
            {
                //2 values?
                //box.BorderSpacingHorizontal = new CssLength(r[0].Value);
                //box.BorderSpacingVertical = new CssLength(r[1].Value);
                throw new NotSupportedException();
            }
            else
            {
                //primitive value 
                box.BorderSpacingHorizontal = box.BorderSpacingVertical = UserMapUtil.AsLength(primValue);
            }
        }
        static void SetPropertyValue(BoxSpec spec, WebDom.CssPropertyDeclaration decl)
        {
            //assign property  
            CssCodeValueExpression cssValue = decl.GetPropertyValue(0);
            switch (decl.WellknownPropertyName)
            {
                case WellknownCssPropertyName.Display:
                    spec.CssDisplay = UserMapUtil.GetDisplayType(cssValue);
                    break;
                //------------------------------------------------
                case WellknownCssPropertyName.BorderBottomWidth:
                    spec.BorderBottomWidth = cssValue.AsBorderLength();
                    break;
                case WellknownCssPropertyName.BorderLeftWidth:
                    spec.BorderLeftWidth = cssValue.AsBorderLength();
                    break;
                case WellknownCssPropertyName.BorderRightWidth:
                    spec.BorderRightWidth = cssValue.AsBorderLength();
                    break;
                case WellknownCssPropertyName.BorderTopWidth:
                    spec.BorderTopWidth = cssValue.AsBorderLength();
                    break;
                //------------------------------------------------
                case WellknownCssPropertyName.BorderBottomStyle:
                    spec.BorderBottomStyle = UserMapUtil.GetBorderStyle(cssValue);
                    break;
                case WellknownCssPropertyName.BorderLeftStyle:
                    spec.BorderLeftStyle = UserMapUtil.GetBorderStyle(cssValue);
                    break;
                case WellknownCssPropertyName.BorderRightStyle:
                    spec.BorderRightStyle = UserMapUtil.GetBorderStyle(cssValue);
                    break;
                case WellknownCssPropertyName.BorderTopStyle:
                    spec.BorderTopStyle = UserMapUtil.GetBorderStyle(cssValue);
                    break;
                //------------------------------------------------
                case WellknownCssPropertyName.BorderBottomColor:
                    spec.BorderBottomColor = AsColor(cssValue);
                    break;
                case WellknownCssPropertyName.BorderLeftColor:
                    spec.BorderLeftColor = AsColor(cssValue);
                    break;
                case WellknownCssPropertyName.BorderRightColor:
                    spec.BorderRightColor = AsColor(cssValue);
                    break;
                case WellknownCssPropertyName.BorderTopColor:
                    spec.BorderTopColor = AsColor(cssValue); ;
                    break;
                //------------------------------------------------
                case WellknownCssPropertyName.BorderSpacing:

                    SetBorderSpacing(spec, cssValue);
                    break;
                case WellknownCssPropertyName.BorderCollapse:
                    spec.BorderCollapse = UserMapUtil.GetBorderCollapse(cssValue);
                    break;
                //------------------------------------------------
                case WellknownCssPropertyName.BoxShadow:
                    SetBoxShadow(spec, decl);
                    break;
                case WellknownCssPropertyName.CornerRadius:

                    SetCornerRadius(spec, cssValue);
                    break;
                case WellknownCssPropertyName.CornerNWRadius:
                    spec.CornerNWRadius = cssValue.AsLength();
                    break;
                case WellknownCssPropertyName.CornerNERadius:
                    spec.CornerNERadius = cssValue.AsLength();
                    break;
                case WellknownCssPropertyName.CornerSERadius:
                    spec.CornerSERadius = cssValue.AsLength();
                    break;
                case WellknownCssPropertyName.CornerSWRadius:
                    spec.CornerSWRadius = cssValue.AsLength();
                    break;

                //------------------------------------------------
                case WellknownCssPropertyName.MarginBottom:
                    spec.MarginBottom = cssValue.AsTranslatedLength();
                    break;
                case WellknownCssPropertyName.MarginLeft:
                    spec.MarginLeft = cssValue.AsTranslatedLength();
                    break;
                case WellknownCssPropertyName.MarginRight:
                    spec.MarginRight = cssValue.AsTranslatedLength();
                    break;
                case WellknownCssPropertyName.MarginTop:
                    spec.MarginTop = cssValue.AsTranslatedLength();
                    break;
                //------------------------------------------------
                case WellknownCssPropertyName.PaddingBottom:
                    spec.PaddingBottom = cssValue.AsTranslatedLength();
                    break;
                case WellknownCssPropertyName.PaddingLeft:
                    spec.PaddingLeft = cssValue.AsTranslatedLength();
                    break;
                case WellknownCssPropertyName.PaddingRight:
                    spec.PaddingRight = cssValue.AsTranslatedLength();
                    break;
                case WellknownCssPropertyName.PaddingTop:
                    spec.PaddingTop = cssValue.AsTranslatedLength();
                    break;
                //------------------------------------------------
                case WellknownCssPropertyName.Left:
                    spec.Left = cssValue.AsLength();
                    break;
                case WellknownCssPropertyName.Top:
                    spec.Top = cssValue.AsLength();
                    break;
                case WellknownCssPropertyName.Width:
                    spec.Width = cssValue.AsLength();
                    break;
                case WellknownCssPropertyName.MaxWidth:
                    spec.MaxWidth = cssValue.AsLength();
                    break;
                case WellknownCssPropertyName.Height:
                    spec.Height = cssValue.AsLength();
                    break;
                case WellknownCssPropertyName.BackgroundColor:
                    spec.BackgroundColor = AsColor(cssValue);
                    break;
                case WellknownCssPropertyName.BackgroundImage:
                    spec.BackgroundImageBinder = new ClientImageBinder(cssValue.GetTranslatedStringValue());
                    break;
                case WellknownCssPropertyName.BackgroundPosition:
                    SetBackgroundPosition(spec, cssValue);
                    break;
                case WellknownCssPropertyName.BackgroundRepeat:
                    spec.BackgroundRepeat = UserMapUtil.GetBackgroundRepeat(cssValue);
                    break;
                case WellknownCssPropertyName.BackgroundGradient:
                    spec.BackgroundGradient = AsColor(cssValue);
                    break;
                case WellknownCssPropertyName.BackgroundGradientAngle:
                    spec.BackgroundGradientAngle = cssValue.AsNumber();
                    break;
                case WellknownCssPropertyName.Color:
                    spec.Color = AsColor(cssValue);
                    break;

                case WellknownCssPropertyName.Direction:

                    spec.CssDirection = UserMapUtil.GetCssDirection(cssValue);
                    break;
                case WellknownCssPropertyName.EmptyCells:
                    spec.EmptyCells = UserMapUtil.GetEmptyCell(cssValue);
                    break;
                case WellknownCssPropertyName.Float:
                    spec.Float = UserMapUtil.GetFloat(cssValue);
                    break;
                case WellknownCssPropertyName.Position:
                    spec.Position = UserMapUtil.GetCssPosition(cssValue);
                    break;
                case WellknownCssPropertyName.LineHeight:
                    spec.LineHeight = cssValue.AsLength();

                    break;
                case WellknownCssPropertyName.VerticalAlign:
                    spec.VerticalAlign = UserMapUtil.GetVerticalAlign(cssValue);
                    break;
                case WellknownCssPropertyName.TextIndent:
                    spec.TextIndent = cssValue.AsLength();
                    break;
                case WellknownCssPropertyName.TextAlign:

                    spec.CssTextAlign = UserMapUtil.GetTextAlign(cssValue);
                    break;
                case WellknownCssPropertyName.TextDecoration:
                    spec.TextDecoration = UserMapUtil.GetTextDecoration(cssValue);
                    break;
                case WellknownCssPropertyName.Whitespace:
                    spec.WhiteSpace = UserMapUtil.GetWhitespace(cssValue);
                    break;
                case WellknownCssPropertyName.WordBreak:
                    spec.WordBreak = UserMapUtil.GetWordBreak(cssValue);
                    break;
                case WellknownCssPropertyName.Visibility:
                    spec.Visibility = UserMapUtil.GetVisibility(cssValue);
                    break;
                case WellknownCssPropertyName.WordSpacing:
                    spec.WordSpacing = cssValue.AsLength();
                    break;
                case WellknownCssPropertyName.FontFamily:
                    spec.FontFamily = cssValue.GetTranslatedStringValue();
                    break;
                case WellknownCssPropertyName.FontSize:

                    SetFontSize(spec, cssValue);
                    break;
                case WellknownCssPropertyName.FontStyle:
                    spec.FontStyle = UserMapUtil.GetFontStyle(cssValue);
                    break;
                case WellknownCssPropertyName.FontVariant:
                    spec.FontVariant = UserMapUtil.GetFontVariant(cssValue);
                    break;
                case WellknownCssPropertyName.FontWeight:
                    spec.FontWeight = UserMapUtil.GetFontWeight(cssValue);
                    break;
                case WellknownCssPropertyName.ListStyle:
                    spec.ListStyle = cssValue.GetTranslatedStringValue();
                    break;
                case WellknownCssPropertyName.ListStylePosition:
                    spec.ListStylePosition = UserMapUtil.GetListStylePosition(cssValue);
                    break;
                case WellknownCssPropertyName.ListStyleImage:
                    spec.ListStyleImage = cssValue.GetTranslatedStringValue();
                    break;
                case WellknownCssPropertyName.ListStyleType:
                    spec.ListStyleType = UserMapUtil.GetListStyleType(cssValue);
                    break;
                case WellknownCssPropertyName.Overflow:
                    spec.Overflow = UserMapUtil.GetOverflow(cssValue);
                    break;
                //------------------------------------------------


            }
        }
        static CssLength SetLineHeight(this CssBox box, CssLength len)
        {
            //2014,2015,
            //from www.w3c.org/wiki/Css/Properties/line-height

            //line height in <percentage> : 
            //The computed value if the property is percentage multiplied by the 
            //element's computed font size. 
            return CssLength.MakePixelLength(
                 CssValueParser.ConvertToPx(len, box.GetEmHeight(), box));
        }
        static void SetCornerRadius(BoxSpec box, WebDom.CssCodeValueExpression value)
        {
            WebDom.CssCodePrimitiveExpression prim = value as WebDom.CssCodePrimitiveExpression;
            if (prim == null)
            {
                //combinator values?
                throw new NotSupportedException();
                return;
            }
            box.CornerNERadius = box.CornerNWRadius =
               box.CornerSERadius = box.CornerSWRadius = UserMapUtil.AsLength(prim);
        }
        static void SetBoxShadow(BoxSpec box, WebDom.CssPropertyDeclaration decl)
        {
            switch (decl.ValueCount)
            {
                case 2:
                    {
                        //
                        var v0 = decl.GetPropertyValue(0);
                        var v1 = decl.GetPropertyValue(1);

                        box.BoxShadowHOffset = v0.AsLength();
                        box.BoxShadowVOffset = v1.AsLength();
                    } break;
                case 3:
                    {
                        var v0 = decl.GetPropertyValue(0);
                        var v1 = decl.GetPropertyValue(1);
                        var v2 = decl.GetPropertyValue(2);
                        box.BoxShadowHOffset = v0.AsLength();
                        box.BoxShadowVOffset = v1.AsLength();

                        box.BoxShadowColor = AsColor(v2);

                    } break;
            } 
        }
        static void SetFontSize(this BoxSpec box, WebDom.CssCodeValueExpression value)
        {
            //number + value
            WebDom.CssCodePrimitiveExpression primValue = value as WebDom.CssCodePrimitiveExpression;
            if (primValue == null)
            {
                return;
            }
            switch (primValue.Hint)
            {
                case WebDom.CssValueHint.Number:
                    {
                        //has unit or not
                        //?
                        //or percent ? 

                        CssLength len = UserMapUtil.AsLength(primValue);
                        if (len.HasError)
                        {
                            len = CssLength.FontSizeMedium;
                        }
                        box.FontSize = len;

                    } break;
                case WebDom.CssValueHint.Iden:
                    {
                        switch (primValue.GetTranslatedStringValue())
                        {
                            default:
                                {
                                    throw new NotSupportedException();
                                }
                            case CssConstants.Medium:
                                box.FontSize = CssLength.FontSizeMedium;
                                break;
                            case CssConstants.Small:
                                box.FontSize = CssLength.FontSizeSmall;
                                break;
                            case CssConstants.XSmall:
                                box.FontSize = CssLength.FontSizeXSmall;
                                break;
                            case CssConstants.XXSmall:
                                box.FontSize = CssLength.FontSizeXXSmall;
                                break;
                            case CssConstants.Large:
                                box.FontSize = CssLength.FontSizeLarge;
                                break;
                            case CssConstants.XLarge:
                                box.FontSize = CssLength.FontSizeLarge;
                                break;
                            case CssConstants.XXLarge:
                                box.FontSize = CssLength.FontSizeLarger;
                                break;
                            case CssConstants.Smaller:
                                box.FontSize = CssLength.FontSizeSmaller;
                                break;
                            case CssConstants.Larger:
                                box.FontSize = CssLength.FontSizeLarger;
                                break;
                        }
                    } break;
            }
        }
        /// <summary>
        /// assign property value from parent
        /// </summary>
        /// <param name="spec"></param>
        /// <param name="propName"></param>
        static void SetPropertyValueFromParent(BoxSpec spec, BoxSpec parentSpec, WellknownCssPropertyName propName)
        {

            switch (propName)
            {
                case WellknownCssPropertyName.BorderBottomWidth:
                    spec.BorderBottomWidth = parentSpec.BorderBottomWidth;
                    break;
                case WellknownCssPropertyName.BorderLeftWidth:
                    spec.BorderLeftWidth = parentSpec.BorderLeftWidth;
                    break;
                case WellknownCssPropertyName.BorderRightWidth:
                    spec.BorderRightWidth = parentSpec.BorderRightWidth;
                    break;
                case WellknownCssPropertyName.BorderTopWidth:
                    spec.BorderTopWidth = parentSpec.BorderTopWidth;
                    break;
                //------------------------------------------------
                case WellknownCssPropertyName.BorderBottomStyle:
                    spec.BorderBottomStyle = parentSpec.BorderBottomStyle;
                    break;
                case WellknownCssPropertyName.BorderLeftStyle:
                    spec.BorderLeftStyle = parentSpec.BorderBottomStyle;
                    break;
                case WellknownCssPropertyName.BorderRightStyle:
                    spec.BorderRightStyle = parentSpec.BorderRightStyle;
                    break;
                case WellknownCssPropertyName.BorderTopStyle:
                    spec.BorderTopStyle = parentSpec.BorderTopStyle;
                    break;
                //------------------------------------------------
                case WellknownCssPropertyName.BorderBottomColor:
                    spec.BorderBottomColor = parentSpec.BorderBottomColor;
                    break;
                case WellknownCssPropertyName.BorderLeftColor:
                    spec.BorderLeftColor = parentSpec.BorderLeftColor;
                    break;
                case WellknownCssPropertyName.BorderRightColor:
                    spec.BorderRightColor = parentSpec.BorderRightColor;
                    break;
                case WellknownCssPropertyName.BorderTopColor:
                    spec.BorderTopColor = parentSpec.BorderTopColor;
                    break;
                //------------------------------------------------
                case WellknownCssPropertyName.BorderSpacing:

                    spec.BorderSpacingHorizontal = parentSpec.BorderSpacingHorizontal;
                    spec.BorderSpacingVertical = parentSpec.BorderSpacingVertical;
                    break;
                case WellknownCssPropertyName.BorderCollapse:
                    spec.BorderCollapse = parentSpec.BorderCollapse;
                    break;
                //------------------------------------------------
                case WellknownCssPropertyName.CornerRadius:
                    spec.CornerNERadius = parentSpec.CornerNERadius;
                    spec.CornerNWRadius = parentSpec.CornerNWRadius;
                    spec.CornerSERadius = parentSpec.CornerSERadius;
                    spec.CornerSWRadius = parentSpec.CornerSWRadius;
                    break;
                case WellknownCssPropertyName.CornerNWRadius:

                    spec.CornerNWRadius = parentSpec.CornerNWRadius;
                    break;
                case WellknownCssPropertyName.CornerNERadius:
                    spec.CornerNERadius = parentSpec.CornerNERadius;
                    break;
                case WellknownCssPropertyName.CornerSERadius:
                    spec.CornerSERadius = parentSpec.CornerSERadius;
                    break;
                case WellknownCssPropertyName.CornerSWRadius:
                    spec.CornerSWRadius = parentSpec.CornerSWRadius;
                    break;
                //------------------------------------------------
                case WellknownCssPropertyName.MarginBottom:
                    spec.MarginBottom = parentSpec.MarginBottom;
                    break;
                case WellknownCssPropertyName.MarginLeft:
                    spec.MarginLeft = parentSpec.MarginLeft;
                    break;
                case WellknownCssPropertyName.MarginRight:
                    spec.MarginRight = parentSpec.MarginRight;
                    break;
                case WellknownCssPropertyName.MarginTop:
                    spec.MarginTop = parentSpec.MarginTop;
                    break;
                //------------------------------------------------
                case WellknownCssPropertyName.PaddingBottom:
                    spec.PaddingBottom = parentSpec.PaddingBottom;
                    break;
                case WellknownCssPropertyName.PaddingLeft:
                    spec.PaddingLeft = parentSpec.PaddingLeft;
                    break;
                case WellknownCssPropertyName.PaddingRight:
                    spec.PaddingRight = parentSpec.PaddingRight;
                    break;
                case WellknownCssPropertyName.PaddingTop:
                    spec.PaddingTop = parentSpec.PaddingTop;
                    break;
                //------------------------------------------------
                case WellknownCssPropertyName.Left:
                    spec.Left = parentSpec.Left;
                    break;
                case WellknownCssPropertyName.Top:
                    spec.Top = parentSpec.Top;
                    break;
                case WellknownCssPropertyName.Width:
                    spec.Width = parentSpec.Width;
                    break;
                case WellknownCssPropertyName.MaxWidth:
                    spec.MaxWidth = parentSpec.MaxWidth;
                    break;
                case WellknownCssPropertyName.Height:
                    spec.Height = parentSpec.Height;
                    break;
                case WellknownCssPropertyName.BackgroundColor:
                    spec.BackgroundColor = parentSpec.BackgroundColor;
                    break;
                case WellknownCssPropertyName.BackgroundImage:
                    spec.BackgroundImageBinder = parentSpec.BackgroundImageBinder;
                    break;
                case WellknownCssPropertyName.BackgroundPosition:
                    spec.BackgroundPositionX = parentSpec.BackgroundPositionX;
                    spec.BackgroundPositionY = parentSpec.BackgroundPositionY;
                    break;
                case WellknownCssPropertyName.BackgroundRepeat:
                    spec.BackgroundRepeat = parentSpec.BackgroundRepeat;
                    break;
                case WellknownCssPropertyName.BackgroundGradient:
                    spec.BackgroundGradient = parentSpec.BackgroundGradient;
                    break;
                case WellknownCssPropertyName.BackgroundGradientAngle:
                    {
                        spec.BackgroundGradientAngle = parentSpec.BackgroundGradientAngle;
                        //float angle;
                        //if (float.TryParse(value, out angle))
                        //{
                        //    cssBox.BackgroundGradientAngle = angle;
                        //}
                    } break;
                case WellknownCssPropertyName.Color:
                    spec.Color = parentSpec.Color;
                    break;
                case WellknownCssPropertyName.Display:
                    spec.CssDisplay = parentSpec.CssDisplay;
                    break;
                case WellknownCssPropertyName.Direction:
                    spec.CssDirection = parentSpec.CssDirection;
                    break;
                case WellknownCssPropertyName.EmptyCells:
                    spec.EmptyCells = parentSpec.EmptyCells;
                    break;
                case WellknownCssPropertyName.Float:
                    spec.Float = parentSpec.Float;
                    break;
                case WellknownCssPropertyName.Position:
                    spec.Position = parentSpec.Position;
                    break;
                case WellknownCssPropertyName.LineHeight:
                    spec.LineHeight = parentSpec.LineHeight;
                    break;
                case WellknownCssPropertyName.VerticalAlign:
                    spec.VerticalAlign = parentSpec.VerticalAlign;
                    break;
                case WellknownCssPropertyName.TextIndent:
                    spec.TextIndent = parentSpec.TextIndent;
                    break;
                case WellknownCssPropertyName.TextAlign:
                    spec.CssTextAlign = parentSpec.CssTextAlign;
                    break;
                case WellknownCssPropertyName.TextDecoration:
                    spec.TextDecoration = parentSpec.TextDecoration;
                    break;
                case WellknownCssPropertyName.Whitespace:
                    spec.WhiteSpace = parentSpec.WhiteSpace;
                    break;
                case WellknownCssPropertyName.WordBreak:
                    spec.WordBreak = parentSpec.WordBreak;
                    break;
                case WellknownCssPropertyName.Visibility:
                    spec.Visibility = parentSpec.Visibility;
                    break;
                case WellknownCssPropertyName.WordSpacing:
                    spec.WordSpacing = parentSpec.WordSpacing;
                    break;
                case WellknownCssPropertyName.FontFamily:
                    spec.FontFamily = parentSpec.FontFamily;
                    //cssBox.FontFamily = value;
                    break;
                case WellknownCssPropertyName.FontSize:
                    spec.FontSize = parentSpec.FontSize;
                    break;
                case WellknownCssPropertyName.FontStyle:
                    spec.FontStyle = parentSpec.FontStyle;
                    break;
                case WellknownCssPropertyName.FontVariant:
                    spec.FontVariant = parentSpec.FontVariant;

                    break;
                case WellknownCssPropertyName.FontWeight:
                    spec.FontWeight = parentSpec.FontWeight;

                    break;
                case WellknownCssPropertyName.ListStyle:

                    spec.ListStyle = parentSpec.ListStyle;
                    break;
                case WellknownCssPropertyName.ListStylePosition:
                    spec.ListStylePosition = parentSpec.ListStylePosition;
                    break;
                case WellknownCssPropertyName.ListStyleImage:

                    spec.ListStyleImage = parentSpec.ListStyleImage;
                    break;
                case WellknownCssPropertyName.ListStyleType:

                    spec.ListStyleType = parentSpec.ListStyleType;
                    break;
                case WellknownCssPropertyName.Overflow:

                    spec.Overflow = parentSpec.Overflow;
                    break;
            }
        }

    }

}