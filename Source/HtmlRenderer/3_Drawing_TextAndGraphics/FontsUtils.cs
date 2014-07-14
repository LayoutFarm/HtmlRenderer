////BSD 2014,  
////ArthurHub

//// "Therefore those skilled at the unorthodox
//// are infinite as heaven and earth,
//// inexhaustible as the great rivers.
//// When they come to an end,
//// they begin again,
//// like the days and months;
//// they die and are reborn,
//// like the four seasons."
//// 
//// - Sun Tsu,
//// "The Art of War"

//using System;
//using System.Collections.Generic;
//using System.Drawing;

//namespace HtmlRenderer.Drawing
//{

  

//    /// <summary>
//    /// Utils for fonts and fonts families handling.
//    /// </summary>
//    public static class FontsUtils
//    {


//        #region Fields and Consts

//        /// <summary>
//        /// Allow to map not installed fonts to different
//        /// </summary>
//        private static readonly Dictionary<string, string> _fontsMapping = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

//        /// <summary>
//        /// collection of all installed and added font families to check if font exists
//        /// </summary>
//        private static readonly Dictionary<string, FontFamily> _existingFontFamilies = new Dictionary<string, FontFamily>(StringComparer.InvariantCultureIgnoreCase);

//        /// <summary>
//        /// cache of all the font used not to create same font again and again
//        /// </summary>
//        private static readonly Dictionary<string, Dictionary<float, Dictionary<FontStyle, Font>>> _fontsCache = new Dictionary<string, Dictionary<float, Dictionary<FontStyle, Font>>>(StringComparer.InvariantCultureIgnoreCase);


//        /// <summary>
//        /// cache of H fonts for managed fonts
//        /// </summary>
//        private static readonly Dictionary<Font, IntPtr> _fontsUnmanagedCache = new Dictionary<Font, IntPtr>();

//        /// <summary>
//        /// cache of fonts height.<br/>
//        /// Not to call Font.GetHeight() each time it is required
//        /// </summary>
//        private static readonly Dictionary<Font, float> _fontHeightCache = new Dictionary<Font, float>();

//        static readonly Dictionary<Font, float> _fontWhitespaceCache = new Dictionary<Font, float>();
//        static readonly Dictionary<Font, FontInfo> _fontInfoCache = new Dictionary<Font, FontInfo>();

//        #endregion





//        /// <summary>
//        /// Init the system installed fonts.
//        /// </summary>
//        static FontsUtils()
//        {
//            _fontsMapping["monospace"] = "Courier New";
//            _fontsMapping["Helvetica"] = "Arial";

//            foreach (var family in FontFamily.Families)
//            {
//                _existingFontFamilies.Add(family.Name, family);
//            } 
//        }

//        /// <summary>
//        /// Gets the ascent of the font
//        /// </summary>
//        /// <param name="font"></param>
//        /// <returns></returns>
//        /// <remarks>
//        /// Font metrics from http://msdn.microsoft.com/en-us/library/xwf9s90b(VS.71).aspx
//        /// </remarks>
//        public static float GetAscent(Font font)
//        {
//            return font.Size * font.FontFamily.GetCellAscent(font.Style) / font.FontFamily.GetEmHeight(font.Style);
//        }



//        /// <summary>
//        /// Gets the descent of the font
//        /// </summary>
//        /// <param name="font"></param>
//        /// <param name="graphics"></param>
//        /// <returns>pixel unit of descent</returns>
//        /// <remarks>
//        /// Font metrics from http://msdn.microsoft.com/en-us/library/xwf9s90b(VS.71).aspx
//        /// </remarks>
//        public static float GetDescentPx(Font font)
//        {
//            //http://msdn.microsoft.com/en-us/library/xwf9s90b%28v=vs.100%29.aspx

//            //cellHeight = font's ascent + descent
//            //linespacing = cellHeight + external leading
//            //em height (size)= cellHeight - internal leading  
//            return font.FontFamily.GetCellDescent(font.Style) * font.Size / font.FontFamily.GetEmHeight(font.Style);
//        }

//        /// <summary>
//        /// Gets the line spacing of the font
//        /// </summary>
//        /// <param name="font"></param>
//        /// <returns>pixel unit of line spacing</returns>
//        /// <remarks>
//        /// Font metrics from http://msdn.microsoft.com/en-us/library/xwf9s90b(VS.71).aspx
//        /// </remarks>
//        public static float GetLineSpacing(Font font)
//        {
//            return font.FontFamily.GetLineSpacing(font.Style) * font.Size / font.FontFamily.GetEmHeight(font.Style);
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="g"></param>
//        /// <param name="str"></param>
//        /// <param name="font"></param>
//        /// <returns></returns>
//        public static float MeasureStringWidth(IGraphics g, string str, Font font)
//        {
//            return g.MeasureString(str, font).Width;
//        }
//        public static float MeasureStringWidth(IGraphics g, char[] buffer, int startAt, int len, Font font)
//        {
//            return g.MeasureString2(buffer, startAt, len, font).Width;
//        }

//        /// <summary>
//        /// Measure regions for specific font empty space size.
//        /// </summary>
//        /// <param name="g">the graphics instance to use if calculation required</param>
//        /// <param name="font">the font to calculate for</param>
//        /// <returns>the calculated regions</returns>
//        public static float MeasureWhitespace(IGraphics g, Font font)
//        {
//            float width;
//            if (!_fontWhitespaceCache.TryGetValue(font, out width))
//            {
//                return RegisterFont(font).WhitespaceWidth;
//            }
//            return width;
//        }
//        /// <summary>
//        /// Check if the given font family exists by name
//        /// </summary>
//        /// <param name="family">the font to check</param>
//        /// <returns>true - font exists by given family name, false - otherwise</returns>
//        public static bool IsFontExists(string family)
//        {
//            bool exists = _existingFontFamilies.ContainsKey(family);
//            if (!exists)
//            {
//                string mappedFamily;
//                if (_fontsMapping.TryGetValue(family, out mappedFamily))
//                {
//                    exists = _existingFontFamilies.ContainsKey(mappedFamily);
//                }
//            }
//            return exists;
//        }

//        /// <summary>
//        /// Get cached font instance for the given font properties.<br/>
//        /// Improve performance not to create same font multiple times.
//        /// </summary>
//        /// <returns>cached font instance</returns>
//        public static Font GetCachedFont(string family, float size, FontStyle style)
//        {
//            var font = TryGetFont(family, size, style);
//            if (font == null)
//            {
//                if (!_existingFontFamilies.ContainsKey(family))
//                {
//                    string mappedFamily;
//                    if (_fontsMapping.TryGetValue(family, out mappedFamily))
//                    {
//                        font = TryGetFont(mappedFamily, size, style);
//                        if (font == null)
//                        {
//                            font = CreateFont(mappedFamily, size, style);
//                            _fontsCache[mappedFamily][size][style] = font;
//                        }
//                    }
//                }

//                if (font == null)
//                {
//                    font = CreateFont(family, size, style);
//                }

//                _fontsCache[family][size][style] = font;
//                _fontsUnmanagedCache[font] = font.ToHfont();
//            }
//            return font;
//        }

//        /// <summary>
//        /// Get pointer to unmanaged Hfont object for the given managed font object.
//        /// </summary>
//        /// <param name="font">the font to get unmanaged font for</param>
//        /// <returns>Hfont pointer</returns>
//        public static IntPtr GetCachedHFont(Font font)
//        {
//            IntPtr hFont;
//            if (!_fontsUnmanagedCache.TryGetValue(font, out hFont))
//            {
//                _fontsUnmanagedCache[font] = hFont = font.ToHfont();
//            }
//            return hFont;
//        }

//        /// <summary>
//        /// Get cached font height for the given font.<br/>
//        /// Improve performance not to access the GetHeight property of a font as it is expensive.<br/>
//        /// Should be used with <see cref="GetCachedFont"/> as the cache uses the font object itself as key.
//        /// </summary>
//        /// <param name="font">the font to get its height</param>
//        /// <returns>the height of the font</returns>
//        public static float GetFontHeight(Font font)
//        {
//            float height;
//            if (!_fontHeightCache.TryGetValue(font, out height))
//            {
//                return RegisterFont(font).LineHeight;
//            }
//            return height;
//        }

//        /// <summary>
//        /// Adds a font family to be used.
//        /// </summary>
//        /// <param name="fontFamily">The font family to add.</param>
//        public static void AddFontFamily(FontFamily fontFamily)
//        {
//            _existingFontFamilies[fontFamily.Name] = fontFamily;
//        }

//        /// <summary>
//        /// Adds a font mapping from <paramref name="fromFamily"/> to <paramref name="toFamily"/> iff the <paramref name="fromFamily"/> is not found.<br/>
//        /// When the <paramref name="fromFamily"/> font is used in rendered html and is not found in existing 
//        /// fonts (installed or added) it will be replaced by <paramref name="toFamily"/>.<br/>
//        /// </summary>
//        /// <param name="fromFamily">the font family to replace</param>
//        /// <param name="toFamily">the font family to replace with</param>
//        public static void AddFontFamilyMapping(string fromFamily, string toFamily)
//        {
//            _fontsMapping[fromFamily] = toFamily;
//        }


//        #region Private methods

//        /// <summary>
//        /// Get cached font if it exists in cache or null if it is not.
//        /// </summary>
//        static Font TryGetFont(string family, float size, FontStyle style)
//        {
//            Font font = null;
//            if (_fontsCache.ContainsKey(family))
//            {
//                var a = _fontsCache[family];
//                if (a.ContainsKey(size))
//                {
//                    var b = a[size];
//                    if (b.ContainsKey(style))
//                    {
//                        font = b[style];
//                    }
//                }
//                else
//                {
//                    _fontsCache[family][size] = new Dictionary<FontStyle, Font>();
//                }
//            }
//            else
//            {
//                _fontsCache[family] = new Dictionary<float, Dictionary<FontStyle, Font>>();
//                _fontsCache[family][size] = new Dictionary<FontStyle, Font>();
//            }
//            return font;
//        }

//        /// <summary>
//        // create font (try using existing font family to support custom fonts)
//        /// </summary>
//        static Font CreateFont(string family, float size, FontStyle style)
//        {

//            FontFamily fontFamily;
//            Font newFont = null;
//            if (_existingFontFamilies.TryGetValue(family, out fontFamily))
//            {
//                newFont = new Font(fontFamily, size, style);
//            }
//            else
//            {
//                newFont = new Font(family, size, style);
//            }

//            RegisterFont(newFont);

//            return newFont;
//        }
//        static FontInfo RegisterFont(Font newFont)
//        {

//            //from ...
//            //http://msdn.microsoft.com/en-us/library/xwf9s90b%28v=vs.100%29.aspx

//            //http://stackoverflow.com/questions/1006069/how-do-i-get-the-position-of-the-text-baseline-in-a-label-and-a-numericupdown


//            //-------------------
//            //evaluate this font, collect font matrix in pixel mode
//            FontInfo fontInfo;
//            if (!_fontInfoCache.TryGetValue(newFont, out fontInfo))
//            {
//                fontInfo = new FontInfo(newFont);
//                FontFamily ff = newFont.FontFamily;

//                int linespace = ff.GetLineSpacing(newFont.Style);
//                int ascent = ff.GetCellAscent(newFont.Style);

//                //font height is expensive call ****
//                int lineSpacing = newFont.Height;

//                //in design unit
//                fontInfo.LineHeight = lineSpacing;//?


//                float baseline = lineSpacing * ascent / linespace;

//                //1. info
//                _fontInfoCache.Add(newFont, fontInfo);
//                //2. line cache
//                _fontHeightCache.Add(newFont, lineSpacing);
//                //3. whitespace 

//                //SizeF whitespaceSize = _myInternalGfx.MeasureString(" ", newFont);
//                //_fontWhitespaceCache.Add(newFont, whitespaceSize.Width);


//                return fontInfo;
//            }
//            return fontInfo;
//        }
//        #endregion
//    }
//}