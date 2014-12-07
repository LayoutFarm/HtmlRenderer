//BSD 2014,  
//ArthurHub

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
using System.Collections.Generic;

using System.Drawing;
using System.Drawing.Drawing2D;

namespace LayoutFarm
{

    struct FontKey
    {
        public readonly string FontName;
        public readonly float FontSize;
        public readonly System.Drawing.FontStyle FontStyle;
        public FontKey(string lowerFontName, float fontSize, System.Drawing.FontStyle fs)
        {
            this.FontName = lowerFontName;
            this.FontSize = fontSize;
            this.FontStyle = fs;
        }
    }

    /// <summary>
    /// Utils for fonts and fonts families handling.
    /// </summary>
    static class FontsUtils
    {
        static LayoutFarm.Drawing.BasicGdi32FontHelper gdiFontHelper = new Drawing.BasicGdi32FontHelper();

        /// <summary>
        /// Allow to map not installed fonts to different
        /// </summary>
        static readonly Dictionary<string, string> _fontsMapping = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

        /// <summary>
        /// collection of all installed and added font families to check if font exists
        /// </summary>
        static readonly Dictionary<string, System.Drawing.FontFamily> _existingFontFamilies = new Dictionary<string, System.Drawing.FontFamily>(StringComparer.InvariantCultureIgnoreCase);


        /// <summary>
        /// cache of H fonts for managed fonts
        /// </summary>
        static readonly Dictionary<System.Drawing.Font, IntPtr> _fontsUnmanagedCache = new Dictionary<System.Drawing.Font, IntPtr>();

        /// <summary>
        /// cache of fonts height.<br/>
        /// Not to call Font.GetHeight() each time it is required
        /// </summary>
        static readonly Dictionary<System.Drawing.Font, float> _fontHeightCache = new Dictionary<System.Drawing.Font, float>();


        static readonly Dictionary<System.Drawing.Font, LayoutFarm.Drawing.FontInfo> _fontInfoCache = new Dictionary<System.Drawing.Font, LayoutFarm.Drawing.FontInfo>();
        static readonly Dictionary<FontKey, LayoutFarm.Drawing.FontInfo> _fontsCache = new Dictionary<FontKey, LayoutFarm.Drawing.FontInfo>();
        static readonly Dictionary<System.Drawing.Font, float> _fontWsCache = new Dictionary<System.Drawing.Font, float>();



        /// <summary>
        /// Init the system installed fonts.
        /// </summary>
        static FontsUtils()
        {
            _fontsMapping["monospace"] = "Courier New";
            _fontsMapping["Helvetica"] = "Arial";

            foreach (var family in System.Drawing.FontFamily.Families)
            {
                _existingFontFamilies.Add(family.Name, family);
            }
        }


        /// <summary>
        /// Gets the line spacing of the font
        /// </summary>
        /// <param name="font"></param>
        /// <returns>pixel unit of line spacing</returns>
        /// <remarks>
        /// Font metrics from http://msdn.microsoft.com/en-us/library/xwf9s90b(VS.71).aspx
        /// </remarks>
        public static float GetLineSpacing(System.Drawing.Font font)
        {
            return font.FontFamily.GetLineSpacing(font.Style) * font.Size / font.FontFamily.GetEmHeight(font.Style);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="g"></param>
        /// <param name="str"></param>
        /// <param name="font"></param>
        /// <returns></returns>
        public static float MeasureStringWidth(LayoutFarm.Drawing.IFonts g, string str, LayoutFarm.Drawing.Font font)
        {
            return g.MeasureString(str, font).Width;
        }
        public static float MeasureStringWidth(LayoutFarm.Drawing.IFonts g, char[] buffer, int startAt, int len, LayoutFarm.Drawing.Font font)
        {
            return g.MeasureString(buffer, startAt, len, font).Width;
        }


        /// <summary>
        /// Check if the given font family exists by name
        /// </summary>
        /// <param name="family">the font to check</param>
        /// <returns>true - font exists by given family name, false - otherwise</returns>
        public static bool IsFontExists(string family)
        {
            bool exists = _existingFontFamilies.ContainsKey(family);
            if (!exists)
            {
                string mappedFamily;
                if (_fontsMapping.TryGetValue(family, out mappedFamily))
                {
                    exists = _existingFontFamilies.ContainsKey(mappedFamily);
                }
            }
            return exists;
        }

        public static float MeasureWhitespace(
            LayoutFarm.Drawing.IFonts gfx, LayoutFarm.Drawing.Font f)
        {
            float ws;
            var f2 = f.InnerFont as System.Drawing.Font;
            if (!_fontWsCache.TryGetValue(f2, out ws))
            {
                ws = gfx.MeasureString(new char[] { ' ' }, 0, 1, f).Width;
                _fontWsCache[f2] = ws;
            }
            return ws;
        }

        /// <summary>
        /// Get cached font instance for the given font properties.<br/>
        /// Improve performance not to create same font multiple times.
        /// </summary>
        /// <returns>cached font instance</returns>
        public static LayoutFarm.Drawing.FontInfo GetCachedFont(string family, float size, FontStyle style)
        {
            var font = TryGetFont(family, size, style);
            if (font == null)
            {
                //check if font exist
                if (!_existingFontFamilies.ContainsKey(family))
                {
                    //if not then check from font map
                    string mappedFamily;
                    if (_fontsMapping.TryGetValue(family, out mappedFamily))
                    {
                        //if has map then try get from existing 
                        font = TryGetFont(mappedFamily, size, style);

                        if (font == null)
                        {
                            //if not found then
                            //create and register
                            font = CreateFont(mappedFamily, size, style);
                        }
                    }
                }
                if (font == null)
                {
                    //if still null
                    font = CreateFont(family, size, style);
                }
            }
            return font;
        }
        public static LayoutFarm.Drawing.FontInfo GetCachedFont(Font f)
        {
            LayoutFarm.Drawing.FontInfo found;

            if (!_fontInfoCache.TryGetValue(f, out found))
            {
                //if not found then create it
                return RegisterFont(f, new FontKey(f.Name, f.Size, f.Style));
            }
            return found;
        }
        /// <summary>
        /// Get pointer to unmanaged Hfont object for the given managed font object.
        /// </summary>
        /// <param name="font">the font to get unmanaged font for</param>
        /// <returns>Hfont pointer</returns>
        public static IntPtr GetCachedHFont(System.Drawing.Font font)
        {
            IntPtr hFont;
            if (!_fontsUnmanagedCache.TryGetValue(font, out hFont))
            {
                _fontsUnmanagedCache[font] = hFont = font.ToHfont();
            }
            return hFont;
        }


        /// <summary>
        /// Adds a font family to be used.
        /// </summary>
        /// <param name="fontFamily">The font family to add.</param>
        public static void AddFontFamily(FontFamily fontFamily)
        {
            _existingFontFamilies[fontFamily.Name] = fontFamily;
        }

        /// <summary>
        /// Adds a font mapping from <paramref name="fromFamily"/> to <paramref name="toFamily"/> iff the <paramref name="fromFamily"/> is not found.<br/>
        /// When the <paramref name="fromFamily"/> font is used in rendered html and is not found in existing 
        /// fonts (installed or added) it will be replaced by <paramref name="toFamily"/>.<br/>
        /// </summary>
        /// <param name="fromFamily">the font family to replace</param>
        /// <param name="toFamily">the font family to replace with</param>
        public static void AddFontFamilyMapping(string fromFamily, string toFamily)
        {
            _fontsMapping[fromFamily] = toFamily;
        }


        #region Private methods

        /// <summary>
        /// Get cached font if it exists in cache or null if it is not.
        /// </summary>
        static LayoutFarm.Drawing.FontInfo TryGetFont(string family, float size, FontStyle style)
        {
            LayoutFarm.Drawing.FontInfo fontInfo = null;
            FontKey fontKey = new FontKey(family.ToLower(), size, style);
            _fontsCache.TryGetValue(fontKey, out fontInfo);
            return fontInfo;

            //if (_fontsCache.ContainsKey(family))
            //{
            //    var a = _fontsCache[family];
            //    if (a.ContainsKey(size))
            //    {
            //        var b = a[size];
            //        if (b.ContainsKey(style))
            //        {
            //            font = b[style];
            //        }
            //    }
            //    else
            //    {
            //        _fontsCache[family][size] = new Dictionary<FontStyle, Font>();
            //    }
            //}
            //else
            //{
            //    _fontsCache[family] = new Dictionary<float, Dictionary<FontStyle, Font>>();
            //    _fontsCache[family][size] = new Dictionary<FontStyle, Font>();
            //}
            //return font;
        }

        /// <summary>
        // create font (try using existing font family to support custom fonts)
        /// </summary>
        static LayoutFarm.Drawing.FontInfo CreateFont(string family,
            float size, FontStyle style)
        {

            FontFamily fontFamily;
            System.Drawing.Font newFont = null;
            if (_existingFontFamilies.TryGetValue(family, out fontFamily))
            {
                newFont = new Font(fontFamily, size, style);
            }
            else
            {
                newFont = new Font(family, size, style);
            }


            return RegisterFont(newFont, new FontKey(family, size, style));
        }

        static LayoutFarm.Drawing.FontInfo RegisterFont(System.Drawing.Font newFont, FontKey fontKey)
        {

            //from ...
            //1. http://msdn.microsoft.com/en-us/library/xwf9s90b%28v=vs.100%29.aspx
            //3.  Font metrics from http://msdn.microsoft.com/en-us/library/xwf9s90b(VS.71).aspx

            //2. http://stackoverflow.com/questions/1006069/how-do-i-get-the-position-of-the-text-baseline-in-a-label-and-a-numericupdown
            //cellHeight = font's ascent + descent
            //linespacing = cellHeight + external leading
            //em height (size)= cellHeight - internal leading  

            //-------------------
            //evaluate this font, collect font matrix in pixel mode
            LayoutFarm.Drawing.FontInfo fontInfo;
            if (!_fontInfoCache.TryGetValue(newFont, out fontInfo))
            {

                System.Drawing.FontFamily ff = newFont.FontFamily;

                int lineSpacing = ff.GetLineSpacing(newFont.Style);
                //font height is expensive call ****
                int fontHeight = newFont.Height;

                //convert descent 

                float fontSize = newFont.Size;
                int fontEmHeight = newFont.FontFamily.GetEmHeight(newFont.Style);
                int fontAscent = newFont.FontFamily.GetCellAscent(newFont.Style);
                float descent = newFont.FontFamily.GetCellDescent(newFont.Style);


                fontInfo = new LayoutFarm.Drawing.MyFontInfo(

                    new LayoutFarm.Drawing.MyFont(newFont),
                    fontHeight,
                    (fontAscent * fontSize / fontEmHeight),
                    (descent * fontSize / fontEmHeight),
                    fontHeight * fontAscent / lineSpacing,
                    gdiFontHelper);

                //lineSpacing * newFont.FontFamily.GetCellAscent(newFont.Style) / linespace);


                //1. info
                _fontInfoCache.Add(newFont, fontInfo);
                _fontsCache.Add(fontKey, fontInfo);

                _fontsUnmanagedCache[newFont] = newFont.ToHfont();
                //2. line cache
                _fontHeightCache.Add(newFont, fontHeight);

                return fontInfo;
            }
            return fontInfo;
        }
        #endregion
    }



}