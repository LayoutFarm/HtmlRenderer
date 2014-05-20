//BSD, 2014 WinterCore
using System;
using System.Drawing;
using System.Collections.Generic;

using HtmlRenderer.Entities;
using HtmlRenderer.Handlers;
using HtmlRenderer.Utils;

namespace HtmlRenderer.Dom
{

    public static class CssBoxUserUtilExtension
    {

        class CssValueMap<T>
        {


            readonly Dictionary<string, T> stringToValue;
            readonly Dictionary<T, string> valueToString;
            public CssValueMap()
            {

                LoadAndAssignCssValues(out stringToValue, out valueToString);
            }
            static void LoadAndAssignCssValues<T>(out Dictionary<string, T> stringToValue, out Dictionary<T, string> valueToString)
            {
                stringToValue = new Dictionary<string, T>();
                valueToString = new Dictionary<T, string>();

                var fields = typeof(T).GetFields();

                for (int i = fields.Length - 1; i >= 0; --i)
                {
                    var field = fields[i];
                    CssNameAttribute cssNameAttr = null;
                    var customAttrs = field.GetCustomAttributes(cssNameAttrType, false);
                    if (customAttrs != null && customAttrs.Length > 0 &&
                       (cssNameAttr = customAttrs[0] as CssNameAttribute) != null)
                    {
                        T value = (T)field.GetValue(null);
                        stringToValue.Add(cssNameAttr.CssName, value);//1.
                        valueToString.Add(value, cssNameAttr.CssName);//2.                    

                    }
                }

            }
            public string GetStringFromValue(T value)
            {
                string found;
                valueToString.TryGetValue(value, out found);
                return found;
            }
            public T GetValueFromString(string str, T defaultIfNotFound)
            {
                T found;
                if (stringToValue.TryGetValue(str, out found))
                {
                    return found;
                }
                return defaultIfNotFound;
            }

        }


        static Type cssNameAttrType = typeof(CssNameAttribute);
        static readonly CssValueMap<CssDisplay> _cssDisplayMap = new CssValueMap<CssDisplay>();
        static readonly CssValueMap<CssDirection> _cssDirectionMap = new CssValueMap<CssDirection>();
        static readonly CssValueMap<CssPosition> _cssPositionMap = new CssValueMap<CssPosition>();
        static readonly CssValueMap<CssWordBreak> _cssWordBreakMap = new CssValueMap<CssWordBreak>();
        static readonly CssValueMap<CssTextDecoration> _cssTextDecorationMap = new CssValueMap<CssTextDecoration>();
        static readonly CssValueMap<CssOverflow> _cssOverFlowMap = new CssValueMap<CssOverflow>();
        static readonly CssValueMap<CssTextAlign> _cssTextAlignMap = new CssValueMap<CssTextAlign>();
        static readonly CssValueMap<CssVerticalAlign> _cssVerticalAlignMap = new CssValueMap<CssVerticalAlign>();
        static readonly CssValueMap<CssVisibility> _cssVisibilityMap = new CssValueMap<CssVisibility>();
        static readonly CssValueMap<CssWhiteSpace> _cssWhitespaceMap = new CssValueMap<CssWhiteSpace>();
        static readonly CssValueMap<CssBorderCollapse> _cssCollapseBorderMap = new CssValueMap<CssBorderCollapse>();
        static readonly CssValueMap<CssBorderStyle> _cssBorderStyleMap = new CssValueMap<CssBorderStyle>();
        static readonly CssValueMap<CssEmptyCell> _cssEmptyCellMap = new CssValueMap<CssEmptyCell>();
        static readonly CssValueMap<CssFloat> _cssFloatMap = new CssValueMap<CssFloat>();
        static readonly CssValueMap<CssFontStyle> _cssFontStyleMap = new CssValueMap<CssFontStyle>();
        static readonly CssValueMap<CssFontVariant> _cssFontVariantMap = new CssValueMap<CssFontVariant>();
        static readonly CssValueMap<CssFontWeight> _cssFontWeightMap = new CssValueMap<CssFontWeight>();
        static readonly CssValueMap<CssListStylePoistion> _cssListStylePositionMap = new CssValueMap<CssListStylePoistion>();
        static readonly CssValueMap<CssListStyleType> _cssListStyleTypeMap = new CssValueMap<CssListStyleType>();


        static CssBoxUserUtilExtension()
        {

        }
        public static CssBorderStyle GetBorderStyle(string value)
        {
            return _cssBorderStyleMap.GetValueFromString(value, CssBorderStyle.None);
        }
        public static void SetBorderCollapse(this CssBox box, string value)
        {
            box.BorderCollapse = _cssCollapseBorderMap.GetValueFromString(value, CssBorderCollapse.Separate);
        }
        public static string ToCssStringValue(this CssDisplay value)
        {
            return _cssDisplayMap.GetStringFromValue(value);
        }
        public static void SetDisplayType(this CssBox box, string cssdisplayValue)
        {
            box.CssDisplay = _cssDisplayMap.GetValueFromString(cssdisplayValue, CssDisplay.Inline);
        }
        //----------------------
        public static string ToCssStringValue(this CssDirection value)
        {
            return _cssDirectionMap.GetStringFromValue(value);
        }
        public static void SetCssDirection(this CssBox box, string value)
        {
            box.CssDirection = _cssDirectionMap.GetValueFromString(value, CssDirection.Ltl);
        }
        //----------------------
        public static void SetCssPosition(this CssBox box, string value)
        {
            box.Position = _cssPositionMap.GetValueFromString(value, CssPosition.Static);
        }
        public static void SetWordBreak(this CssBox box, string value)
        {
            box.WordBreak = _cssWordBreakMap.GetValueFromString(value, CssWordBreak.Normal);
        }
        public static void SetTextDecoration(this CssBox box, string value)
        {
            box.TextDecoration = _cssTextDecorationMap.GetValueFromString(value, CssTextDecoration.NotAssign);
        }
        public static void SetOverflow(this CssBox box, string value)
        {
            box.Overflow = _cssOverFlowMap.GetValueFromString(value, CssOverflow.Visible);

        }
        public static void SetTextAlign(this CssBox box, string value)
        {
            box.CssTextAlign = _cssTextAlignMap.GetValueFromString(value, CssTextAlign.NotAssign);
        }
        public static void SetVerticalAlign(this CssBox box, string value)
        {
            box.VerticalAlign = _cssVerticalAlignMap.GetValueFromString(value, CssVerticalAlign.Baseline);
        }
        public static void SetVisibility(this CssBox box, string value)
        {
            box.CssVisibility = _cssVisibilityMap.GetValueFromString(value, CssVisibility.Visible);
        }

        public static void SetWhitespace(this CssBox box, string value)
        {
            box.WhiteSpace = _cssWhitespaceMap.GetValueFromString(value, CssWhiteSpace.Normal);
        }
        public static void SetBorderSpacing(this CssBox box, string value)
        {
            System.Text.RegularExpressions.MatchCollection r =
             HtmlRenderer.Parse.RegexParserUtils.Match(HtmlRenderer.Parse.RegexParserUtils.CssLength, value);
            switch (r.Count)
            {
                case 1:
                    {
                        box.BorderSpacingHorizontal = box.BorderSpacingVertical = new CssLength(r[0].Value);
                    } break;
                case 2:
                    {
                        box.BorderSpacingHorizontal = new CssLength(r[0].Value);
                        box.BorderSpacingVertical = new CssLength(r[1].Value);
                    } break;
            }
        }
        public static string GetCornerRadius(this CssBox box)
        {

            System.Text.StringBuilder stbuilder = new System.Text.StringBuilder();
            stbuilder.Append(box.CornerNERadius);
            stbuilder.Append(' ');
            stbuilder.Append(box.CornerNWRadius);
            stbuilder.Append(' ');
            stbuilder.Append(box.CornerSERadius);
            stbuilder.Append(' ');
            stbuilder.Append(box.CornerSWRadius);
            return stbuilder.ToString();
        }
        public static void SetCornerRadius(this CssBox box, string value)
        {
            //throw new NotSupportedException();

            System.Text.RegularExpressions.MatchCollection r =
                HtmlRenderer.Parse.RegexParserUtils.Match(HtmlRenderer.Parse.RegexParserUtils.CssLength, value);
            switch (r.Count)
            {
                case 1:
                    box.CornerNERadius = box.CornerNWRadius =
                        box.CornerSERadius = box.CornerSWRadius = new CssLength(r[0].Value);
                    break;
                case 2:
                    box.CornerNERadius = box.CornerNWRadius = new CssLength(r[0].Value);
                    box.CornerSERadius = box.CornerSWRadius = new CssLength(r[1].Value);
                    break;
                case 3:
                    box.CornerNERadius = new CssLength(r[0].Value);
                    box.CornerNWRadius = new CssLength(r[1].Value);
                    box.CornerSERadius = new CssLength(r[2].Value);
                    break;
                case 4:
                    box.CornerNERadius = new CssLength(r[0].Value);
                    box.CornerNWRadius = new CssLength(r[1].Value);
                    box.CornerSERadius = new CssLength(r[2].Value);
                    box.CornerSWRadius = new CssLength(r[3].Value);
                    break;
            }
        }
        public static CssEmptyCell GetEmptyCell(string value)
        {
            return _cssEmptyCellMap.GetValueFromString(value, CssEmptyCell.Show);
        }

        public static string ToCssStringValue(this CssEmptyCell value)
        {
            return _cssEmptyCellMap.GetStringFromValue(value);
        }
        public static string ToCssStringValue(this CssTextAlign value)
        {
            return _cssTextAlignMap.GetStringFromValue(value);
        }
        public static string ToCssStringValue(this CssTextDecoration value)
        {
            return _cssTextDecorationMap.GetStringFromValue(value);
        }
        public static string ToCssStringValue(this CssWordBreak value)
        {
            return _cssWordBreakMap.GetStringFromValue(value);
        }
        public static string ToCssStringValue(this CssWhiteSpace value)
        {
            return _cssWhitespaceMap.GetStringFromValue(value);

        }
        public static string ToCssStringValue(this CssVisibility value)
        {
            return _cssVisibilityMap.GetStringFromValue(value);

        }
        public static string ToCssStringValue(this CssVerticalAlign value)
        {
            return _cssVerticalAlignMap.GetStringFromValue(value);
        }
        public static string ToCssStringValue(this CssPosition value)
        {
            return _cssPositionMap.GetStringFromValue(value);
        }
        public static CssLength SetLineHeight(this CssBox box, string value)
        {

            // _lineHeight =
            //string.Format(NumberFormatInfo.InvariantInfo, "{0}px",
            //CssValueParser.ParseLength(value, Size.Height, this, CssConstants.Em));

            float lineHeight = HtmlRenderer.Parse.CssValueParser.ParseLength(value, box.Size.Height, box, CssConstants.Em);
            return CssLength.MakePixelLength(lineHeight);
        }

        public static CssFloat GetFloat(string value)
        {
            return _cssFloatMap.GetValueFromString(value, CssFloat.None);
        }

        public static string ToCssStringValue(this CssFloat cssFloat)
        {
            return _cssFloatMap.GetStringFromValue(cssFloat);
        }
        public static string ToCssStringValue(this CssBorderStyle borderStyle)
        {
            return _cssBorderStyleMap.GetStringFromValue(borderStyle);
        }
        public static string ToCssStringValue(this CssBorderCollapse borderCollapse)
        {
            return _cssCollapseBorderMap.GetStringFromValue(borderCollapse);
        }

        public static string ToHexColor(this Color color)
        {
            return string.Concat("#", color.R.ToString("X"), color.G.ToString("X"), color.B.ToString("X"));
        }
        public static string ToCssStringValue(this CssFontStyle fontstyle)
        {
            return _cssFontStyleMap.GetStringFromValue(fontstyle);
        }
        public static CssFontStyle GetFontStyle(string fontstyle)
        {
            return _cssFontStyleMap.GetValueFromString(fontstyle, CssFontStyle.Normal);
        }
        public static string ToCssStringValue(this CssFontVariant fontVariant)
        {
            return _cssFontVariantMap.GetStringFromValue(fontVariant);
        }
        public static CssFontVariant GetFontVariant(string fontVariant)
        {
            return _cssFontVariantMap.GetValueFromString(fontVariant, CssFontVariant.Normal);
        }
        public static string ToFontSizeString(this CssLength length)
        {
            if (length.IsFontSizeName)
            {
                switch (length.FontSizeName)
                {
                    case CssFontSizeConst.FONTSIZE_MEDIUM:
                        return CssConstants.Medium;
                    case CssFontSizeConst.FONTSIZE_SMALL:
                        return CssConstants.Small;
                    case CssFontSizeConst.FONTSIZE_X_SMALL:
                        return CssConstants.XSmall;
                    case CssFontSizeConst.FONTSIZE_XX_SMALL:
                        return CssConstants.XXSmall;
                    case CssFontSizeConst.FONTSIZE_LARGE:
                        return CssConstants.Large;
                    case CssFontSizeConst.FONTSIZE_X_LARGE:
                        return CssConstants.XLarge;
                    case CssFontSizeConst.FONTSIZE_XX_LARGE:
                        return CssConstants.XXLarge;
                    case CssFontSizeConst.FONTSIZE_LARGER:
                        return CssConstants.Larger;
                    case CssFontSizeConst.FONTSIZE_SMALLER:
                        return CssConstants.Smaller;
                    default:
                        return "";
                }
            }
            else
            {
                return length.ToString();
            }
        }
        public static void SetFontSize(this CssBox box, string value)
        {
            string length = HtmlRenderer.Parse.RegexParserUtils.Search(HtmlRenderer.Parse.RegexParserUtils.CssLength, value);
            if (length != null)
            {

                CssLength len = new CssLength(length);
                CssBox parentBox = null;
                if (len.HasError)
                {

                    len = CssLength.FontSizeMedium;
                }
                else if (len.Unit == CssUnit.Ems && ((parentBox = box.GetParentBox()) != null))
                {
                    len = len.ConvertEmToPoints(parentBox.ActualFont.SizeInPoints);
                }
                else
                {

                }
                box.FontSize = len;
            }
            else
            {
                switch (value)
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

            }
        }


        public static CssFontWeight GetFontWeight(string fontWeight)
        {
            return _cssFontWeightMap.GetValueFromString(fontWeight, CssFontWeight.NotAssign);
        }
        public static string ToCssStringValue(this CssFontWeight weight)
        {
            return _cssFontWeightMap.GetStringFromValue(weight);
        }

        public static string ToCssStringValue(this CssListStylePoistion listStylePosition)
        {
            return _cssListStylePositionMap.GetStringFromValue(listStylePosition);
        }
        public static CssListStylePoistion GetListStylePosition(string value)
        {
            return _cssListStylePositionMap.GetValueFromString(value, CssListStylePoistion.Outside);
        }

        public static string ToCssStringValue(this CssListStyleType listStyleType)
        {
            return _cssListStyleTypeMap.GetStringFromValue(listStyleType);
        }
        public static CssListStyleType GetListStyleType(string value)
        {

            return _cssListStyleTypeMap.GetValueFromString(value, CssListStyleType.Disc);
        }


        public static WellknownHtmlTagName EvaluateTagName(string name)
        {  
            switch (name)
            {
                default:
                    return WellknownHtmlTagName.Unknown;
                case "hr":
                    return WellknownHtmlTagName.HR;
                case "a":
                    return WellknownHtmlTagName.A;
                case "script":
                    return WellknownHtmlTagName.SCRIPT;
                case "style":
                    return WellknownHtmlTagName.STYLE;
                case "div":
                    return WellknownHtmlTagName.DIV;
                case "span":
                    return WellknownHtmlTagName.SPAN;
                case "img":
                    return WellknownHtmlTagName.IMG;
                case "link":
                    return WellknownHtmlTagName.LINK;
                case "p":
                    return WellknownHtmlTagName.P;
                case "table":
                    return WellknownHtmlTagName.TABLE;
                case "td":
                    return WellknownHtmlTagName.TD;
                case "tr":
                    return WellknownHtmlTagName.TR;
                case "br":
                    return WellknownHtmlTagName.BR;
                case "html":
                    return WellknownHtmlTagName.HTML;
                case "iframe":
                    return WellknownHtmlTagName.IFREAME;
                case "font":
                    return WellknownHtmlTagName.FONT;
                case "x":
                    return WellknownHtmlTagName.X; //test for extension                     
            }
        }
    }


}