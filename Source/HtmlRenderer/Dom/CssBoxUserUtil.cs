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
    }
}