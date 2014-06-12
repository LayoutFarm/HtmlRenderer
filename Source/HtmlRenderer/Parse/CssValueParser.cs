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
using HtmlRenderer.Dom;
using HtmlRenderer.Entities;
using HtmlRenderer.Utils;

namespace HtmlRenderer.Parse
{
    /// <summary>
    /// Parse CSS properties values like numbers, urls, etc.
    /// </summary>
    internal static class CssValueParser
    {


        /// <summary>
        /// Evals a number and returns it. If number is a percentage, it will be multiplied by <see cref="hundredPercent"/>
        /// </summary>
        /// <param name="number">Number to be parsed</param>
        /// <param name="hundredPercent">Number that represents the 100% if parsed number is a percentage</param>
        /// <returns>Parsed number. Zero if error while parsing.</returns>

        public static float ParseNumber(CssLength number, float hundredPercent)
        {
            if (number.IsEmpty)
            {
                return 0;
            }
            if (number.IsPercentage)
            {
                return (number.Number / 100f) * hundredPercent;
            }
            else
            {
                return number.Number;
            }
        }

        /// <summary>
        ///  Parses a length. Lengths are followed by an unit identifier (e.g. 10px, 3.1em)
        /// </summary>
        /// <param name="length"></param>
        /// <param name="hundredPercent"></param>
        /// <param name="box"></param>
        /// <param name="fontAdjust"></param>
        /// <returns></returns>
        public static float ParseLength(CssLength length, float hundredPercent, CssBoxBase box, bool fontAdjust = false)
        {
            return ParseLengthImpl(length, hundredPercent, box, CssUnit.None, fontAdjust);
        }
        /// <summary>
        /// Parses a length. Lengths are followed by an unit identifier (e.g. 10px, 3.1em)
        /// </summary>
        /// <param name="length">Specified length</param>
        /// <param name="hundredPercent">Equivalent to 100 percent when length is percentage</param>
        /// <param name="box"></param>
        /// <param name="defaultUnit"></param>
        /// <returns>the parsed length value with adjustments</returns>
        public static float ParseLength(CssLength length, float hundredPercent, CssBoxBase box, CssUnit defaultUnit)
        {
            return ParseLengthImpl(length, hundredPercent, box, defaultUnit, false);
        }



        /// <summary>
        ///  Parses a length. Lengths are followed by an unit identifier (e.g. 10px, 3.1em)
        /// </summary>
        /// <param name="length"></param>
        /// <param name="hundredPercent"></param>
        /// <param name="emFactor"></param>
        /// <param name="defaultUnit"></param>
        /// <param name="fontAdjust"></param>
        /// <param name="returnPoints"></param>
        /// <returns></returns>
        static float ParseLengthImpl(CssLength length,
          float hundredPercent, CssBoxBase box,
          CssUnit defaultUnit, bool fontAdjust)
        {
            //Return zero if no length specified, zero specified    
            if (length.IsEmpty)
            {
                return 0;
            }
            //If percentage, use ParseNumber 
            if (length.IsPercentage)
            {
                return (length.Number / 100f) * hundredPercent;
            }

         
            switch (length.Unit)
            {
                case CssUnit.Ems:
                    return length.Number * box.GetEmHeight();
                case CssUnit.Ex:
                    return length.Number * (box.GetEmHeight() / 2);
                case CssUnit.Pixels:
                    //atodo: check support for hi dpi
                    return length.Number * (fontAdjust ? 72f / 96f : 1f); 
                case CssUnit.Milimeters:
                    return length.Number * 3.779527559f; //3 pixels per millimeter      
                case CssUnit.Centimeters:
                    return length.Number * 37.795275591f; //37 pixels per centimeter 
                case CssUnit.Inches:
                    return length.Number * 96f; //96 pixels per inch 
                case CssUnit.Points:
                    return length.Number * (96f / 72f); // 1 point = 1/72 of inch   
                case CssUnit.Picas:
                    return length.Number * 16f; // 1 pica = 12 points 
                default:
                    return 0;
            }             
        }
        /// <summary>
        /// Parses a color value in CSS style; e.g. #ff0000, red, rgb(255,0,0), rgb(100%, 0, 0)
        /// </summary>
        /// <param name="colorValue">color string value to parse</param>
        /// <returns>Color value</returns>
        public static Color GetActualColor(string colorValue)
        {
            Color color;
            TryGetColor(colorValue, 0, colorValue.Length, out color);
            return color;
        }
        public static float GetActualBorderWidth(CssLength borderValue, CssBoxBase b)
        {
            if (borderValue.IsEmpty)
            {
                //return as medium
                return 2f;
            }
            if (borderValue.IsMedium)
            {
                return 2f;
            }
            else if (borderValue.IsThin)
            {
                return 1f;
            }
            else if (borderValue.IsThick)
            {
                return 4f;
            }
            else
            {
                return Math.Abs(ParseLength(borderValue, 1, b));
            }

        }


        #region Private methods




        /// <summary>
        /// Parses a color value in CSS style; e.g. #ff0000, RED, RGB(255,0,0), RGB(100%, 0, 0)
        /// </summary>
        /// <param name="str">color substring value to parse</param>
        /// <param name="idx">substring start idx </param>
        /// <param name="length">substring length</param>
        /// <param name="color">return the parsed color</param>
        /// <returns>true - valid color, false - otherwise</returns>
        private static bool TryGetColor(string str, int idx, int length, out Color color)
        {
            try
            {
                if (!string.IsNullOrEmpty(str))
                {
                    if (length > 1 && str[idx] == '#')
                    {
                        return GetColorByHex(str, idx, length, out color);
                    }
                    else if (length > 10 && CommonUtils.SubStringEquals(str, idx, 4, "rgb(") && str[length - 1] == ')')
                    {
                        return GetColorByRgb(str, idx, length, out color);
                    }
                    else if (length > 13 && CommonUtils.SubStringEquals(str, idx, 5, "rgba(") && str[length - 1] == ')')
                    {
                        return GetColorByRgba(str, idx, length, out color);
                    }
                    else
                    {
                        return GetColorByName(str, idx, length, out color);
                    }
                }
            }
            catch
            { }
            color = Color.Black;
            return false;
        }

        /// <summary>
        /// Get color by parsing given hex value color string (#A28B34).
        /// </summary>
        /// <returns>true - valid color, false - otherwise</returns>
        private static bool GetColorByHex(string str, int idx, int length, out Color color)
        {
            int r = -1;
            int g = -1;
            int b = -1;
            if (length == 7)
            {
                r = ParseHexInt(str, idx + 1, 2);
                g = ParseHexInt(str, idx + 3, 2);
                b = ParseHexInt(str, idx + 5, 2);
            }
            else if (length == 4)
            {
                r = ParseHexInt(str, idx + 1, 1);
                r = r * 16 + r;
                g = ParseHexInt(str, idx + 2, 1);
                g = g * 16 + g;
                b = ParseHexInt(str, idx + 3, 1);
                b = b * 16 + b;
            }
            if (r > -1 && g > -1 && b > -1)
            {
                color = Color.FromArgb(r, g, b);
                return true;
            }
            color = Color.Empty;
            return false;
        }

        /// <summary>
        /// Get color by parsing given RGB value color string (RGB(255,180,90))
        /// </summary>
        /// <returns>true - valid color, false - otherwise</returns>
        private static bool GetColorByRgb(string str, int idx, int length, out Color color)
        {
            int r = -1;
            int g = -1;
            int b = -1;

            if (length > 10)
            {
                int s = idx + 4;
                r = ParseIntAtIndex(str, ref s);
                if (s < idx + length)
                {
                    g = ParseIntAtIndex(str, ref s);
                }
                if (s < idx + length)
                {
                    b = ParseIntAtIndex(str, ref s);
                }
            }

            if (r > -1 && g > -1 && b > -1)
            {
                color = Color.FromArgb(r, g, b);
                return true;
            }
            color = Color.Empty;
            return false;
        }

        /// <summary>
        /// Get color by parsing given RGBA value color string (RGBA(255,180,90,180))
        /// </summary>
        /// <returns>true - valid color, false - otherwise</returns>
        private static bool GetColorByRgba(string str, int idx, int length, out Color color)
        {
            int r = -1;
            int g = -1;
            int b = -1;
            int a = -1;

            if (length > 13)
            {
                int s = idx + 5;
                r = ParseIntAtIndex(str, ref s);

                if (s < idx + length)
                {
                    g = ParseIntAtIndex(str, ref s);
                }
                if (s < idx + length)
                {
                    b = ParseIntAtIndex(str, ref s);
                }
                if (s < idx + length)
                {
                    a = ParseIntAtIndex(str, ref s);
                }
            }

            if (r > -1 && g > -1 && b > -1 && a > -1)
            {
                color = Color.FromArgb(a, r, g, b);
                return true;
            }
            color = Color.Empty;
            return false;
        }

        /// <summary>
        /// Get color by given name, including .NET name.
        /// </summary>
        /// <returns>true - valid color, false - otherwise</returns>
        private static bool GetColorByName(string str, int idx, int length, out Color color)
        {
            color = Color.FromName(str.Substring(idx, length));
            return color.A > 0;
        }

        /// <summary>
        /// Parse the given decimal number string to positive int value.<br/>
        /// Start at given <paramref name="startIdx"/>, ignore whitespaces and take
        /// as many digits as possible to parse to int.
        /// </summary>
        /// <param name="str">the string to parse</param>
        /// <param name="startIdx">the index to start parsing at</param>
        /// <returns>parsed int or 0</returns>
        private static int ParseIntAtIndex(string str, ref int startIdx)
        {
            int len = 0;
            while (char.IsWhiteSpace(str, startIdx))
                startIdx++;
            while (char.IsDigit(str, startIdx + len))
                len++;
            var val = ParseInt(str, startIdx, len);
            startIdx = startIdx + len + 1;
            return val;
        }

        /// <summary>
        /// Parse the given decimal number string to positive int value.
        /// Assume given substring is not empty and all indexes are valid!<br/>
        /// </summary>
        /// <returns>int value, -1 if not valid</returns>
        private static int ParseInt(string str, int idx, int length)
        {
            if (length < 1)
                return -1;

            int num = 0;
            for (int i = 0; i < length; i++)
            {
                int c = str[idx + i];
                if (!(c >= 48 && c <= 57))
                    return -1;

                num = num * 10 + c - 48;
            }
            return num;
        }

        /// <summary>
        /// Parse the given hex number string to positive int value.
        /// Assume given substring is not empty and all indexes are valid!<br/>
        /// </summary>
        /// <returns>int value, -1 if not valid</returns>
        private static int ParseHexInt(string str, int idx, int length)
        {
            if (length < 1)
                return -1;

            int num = 0;
            for (int i = 0; i < length; i++)
            {
                int c = str[idx + i];
                if (!(c >= 48 && c <= 57) && !(c >= 65 && c <= 70) && !(c >= 97 && c <= 102))
                    return -1;

                num = num * 16 + (c <= 57 ? c - 48 : (10 + c - (c <= 70 ? 65 : 97)));
            }
            return num;
        }

        #endregion
    }
}