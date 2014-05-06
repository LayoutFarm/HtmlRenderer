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
        static readonly CssValueMap<CssBoxDisplayType> _cssDisplayMap = new CssValueMap<CssBoxDisplayType>();
        static readonly CssValueMap<CssDirection> _cssDirectionMap = new CssValueMap<CssDirection>();
        static readonly CssValueMap<CssPositionType> _cssPositionMap = new CssValueMap<CssPositionType>();
        static readonly CssValueMap<CssWordBreak> _cssWordBreakMap = new CssValueMap<CssWordBreak>();
        static readonly CssValueMap<CssTextDecoration> _cssTextDecorationMap = new CssValueMap<CssTextDecoration>();
        static readonly CssValueMap<CssOverflow> _cssOverFlowMap = new CssValueMap<CssOverflow>();
        static readonly CssValueMap<CssTextAlign> _csstextAlignMap = new CssValueMap<CssTextAlign>();
        static readonly CssValueMap<CssVerticalAlign> _cssVerticalAlignMap = new CssValueMap<CssVerticalAlign>();
        static readonly CssValueMap<CssVisibility> _cssVisibilityMap = new CssValueMap<CssVisibility>();
        static readonly CssValueMap<CssWhiteSpace> _cssWhitespaceMap = new CssValueMap<CssWhiteSpace>();
        static readonly CssValueMap<CssBorderCollapse> _cssCollapseBorderMap = new CssValueMap<CssBorderCollapse>();
        static readonly CssValueMap<CssBorderStyle> _cssBorderStyleMap = new CssValueMap<CssBorderStyle>();
        static readonly CssValueMap<CssEmptyCell> _cssEmptyCellMap = new CssValueMap<CssEmptyCell>();
        static readonly CssValueMap<CssFloat> _cssFloatMap = new CssValueMap<CssFloat>();

        static CssBoxUserUtilExtension()
        {

        }
        public static CssBorderStyle GetBorderStyle(string value)
        {
            return _cssBorderStyleMap.GetValueFromString(value, CssBorderStyle.None);
        }
        public static void SetBorderCollapse(this CssBox box, string value)
        {
            box.BorderCollapse = _cssCollapseBorderMap.GetValueFromString(value, CssBorderCollapse.Sepatate);
        }
        public static string GetDisplayString(this CssBox box)
        {
            return _cssDisplayMap.GetStringFromValue(box.CssDisplay);
        }
        public static void SetDisplayType(this CssBox box, string cssdisplayValue)
        {
            box.CssDisplay = _cssDisplayMap.GetValueFromString(cssdisplayValue, CssBoxDisplayType.Inline);
        }
        //----------------------
        public static string GetCssDirection(this CssBox box)
        {
            return _cssDirectionMap.GetStringFromValue(box.CssDirection);
        }
        public static void SetCssDirection(this CssBox box, string value)
        {
            box.CssDirection = _cssDirectionMap.GetValueFromString(value, CssDirection.Ltl);
        }
        //----------------------
        public static void SetCssPosition(this CssBox box, string value)
        {
            box.Position = _cssPositionMap.GetValueFromString(value, CssPositionType.Static);
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
            box.CssTextAlign = _csstextAlignMap.GetValueFromString(value, CssTextAlign.NotAssign);
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
            // CornerNERadius = r[0].Value;
            //                CornerNWRadius = r[1].Value;
            //                CornerSERadius = r[2].Value;
            //                CornerSWRadius = r[3].Value;
            //CssLength[] corners = box.CornerRadius;
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
        public static string GetEmptyCellString(CssEmptyCell value)
        {
            return _cssEmptyCellMap.GetStringFromValue(value);
        }
        public static CssLength SetLineHeight(this CssBox box, string value)
        {
            float lineHeight = HtmlRenderer.Parse.CssValueParser.ParseLength(value, box.Size.Height, box, CssConstants.Em);
            return CssLength.MakePixelLength(lineHeight);
        }
        public static CssFloat GetFloat(string value)
        {
            return _cssFloatMap.GetValueFromString(value, CssFloat.None);
        }
        public static string GetFloatString(CssFloat floatString)
        {
            return _cssFloatMap.GetStringFromValue(floatString);
        }


    }
}