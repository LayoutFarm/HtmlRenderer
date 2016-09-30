//BSD, 2014-2016, WinterDev   
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
using System.Collections.Generic;
namespace PixelFarm.Drawing.WinGdi
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


    class WinGdiFontStore
    {
        // static PixelFarm.Drawing.WinGdi.BasicGdi32FontHelper gdiFontHelper = new PixelFarm.Drawing.WinGdi.BasicGdi32FontHelper();
        /// <summary>
        /// Allow to map not installed fonts to different
        /// </summary>
        static readonly Dictionary<string, string> _fontsMapping = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
        /// <summary>
        /// collection of all installed and added font families to check if font exists
        /// </summary>
        static readonly Dictionary<string, System.Drawing.FontFamily> _existingFontFamilies = new Dictionary<string, System.Drawing.FontFamily>(StringComparer.InvariantCultureIgnoreCase);
        readonly Dictionary<System.Drawing.Font, Font> _fontInfoCache = new Dictionary<System.Drawing.Font, Font>();
        readonly Dictionary<FontKey, Font> _fontInfoCacheByFontKey = new Dictionary<FontKey, Font>();
        static Dictionary<Font, WinGdiPlusFont> resolvedWinGdiFont = new Dictionary<Font, WinGdiPlusFont>();

        public WinGdiFontStore()
        {

        }
        /// <summary>
        /// Get cached font instance for the given font properties.<br/>
        /// Improve performance not to create same font multiple times.
        /// </summary>
        /// <returns>cached font instance</returns>
        public Font GetCachedFont(string family, float size, System.Drawing.FontStyle style)
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
        public Font GetCachedFont(System.Drawing.Font f)
        {
            Font found;
            if (!_fontInfoCache.TryGetValue(f, out found))
            {
                //if not found then create it
                return RegisterFont(f, new FontKey(f.Name, f.Size, f.Style));
            }
            return found;
        }
        /// <summary>
        /// Get cached font if it exists in cache or null if it is not.
        /// </summary>
        Font TryGetFont(string family, float size, System.Drawing.FontStyle style)
        {
            Font found;
            FontKey fontKey = new FontKey(family.ToLower(), size, style);
            _fontInfoCacheByFontKey.TryGetValue(fontKey, out found);
            return found;
        }

        /// <summary>
        // create font (try using existing font family to support custom fonts)
        /// </summary>
        Font CreateFont(string family,
          float size, System.Drawing.FontStyle style)
        {
            System.Drawing.FontFamily fontFamily;
            System.Drawing.Font newFont = null;
            if (_existingFontFamilies.TryGetValue(family, out fontFamily))
            {
                newFont = new System.Drawing.Font(fontFamily, size, style);
            }
            else
            {
                newFont = new System.Drawing.Font(family, size, style);
            }
            return RegisterFont(newFont, new FontKey(family, size, style));
        }

        Font RegisterFont(System.Drawing.Font newFont, FontKey fontKey)
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
            Font font;
            if (!_fontInfoCache.TryGetValue(newFont, out font))
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

                font = new Font(newFont.Name, newFont.SizeInPoints);
                resolvedWinGdiFont.Add(font, new WinGdiPlusFont(newFont));
                //myFont,
                //fontHeight,
                //(fontAscent * fontSize / fontEmHeight),
                //(descent * fontSize / fontEmHeight),
                //fontHeight * fontAscent / lineSpacing,
                //gdiFontHelper);
                //myFont.SetFontInfo(fontInfo);
                //lineSpacing * newFont.FontFamily.GetCellAscent(newFont.Style) / linespace); 
                //1. info
                _fontInfoCache.Add(newFont, font);
                _fontInfoCacheByFontKey.Add(fontKey, font);
                _fontsUnmanagedCache[newFont] = newFont.ToHfont();
                //2. line cache
                _fontHeightCache[newFont] = fontHeight;
                return font;
            }
            return font;
        }


        /// <summary>
        /// cache of H fonts for managed fonts
        /// </summary>
        static readonly Dictionary<System.Drawing.Font, IntPtr> _fontsUnmanagedCache = new Dictionary<System.Drawing.Font, IntPtr>();
        /// <summary>
        /// cache of fonts height.<br/>
        /// Not to call Font.GetHeight() each time it is required
        /// </summary>
        static readonly Dictionary<System.Drawing.Font, float> _fontHeightCache = new Dictionary<System.Drawing.Font, float>();
        static readonly Dictionary<WinGdiPlusFont, float> _fontWsCache = new Dictionary<WinGdiPlusFont, float>();
        /// <summary>
        /// Init the system installed fonts.
        /// </summary>
        static WinGdiFontStore()
        {
            _fontsMapping["monospace"] = "Courier New";
            _fontsMapping["Helvetica"] = "Arial";
            foreach (var family in System.Drawing.FontFamily.Families)
            {
                if (_existingFontFamilies.ContainsKey(family.Name) == false)
                {
                    _existingFontFamilies.Add(family.Name, family);
                }
            }
        }
        public WinGdiPlusFont GetResolvedFont(PixelFarm.Drawing.Font f)
        {
            return S_GetResolvedFont(f);
        }
        public static WinGdiPlusFont S_GetResolvedFont(PixelFarm.Drawing.Font f)
        {
            WinGdiPlusFont found;
            resolvedWinGdiFont.TryGetValue(f, out found);
            return found;
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

        public float MeasureWhitespace(
            PixelFarm.Drawing.IFonts gfx, PixelFarm.Drawing.Font f)
        {
            float ws;
            WinGdiPlusFont winFont;
            if (!resolvedWinGdiFont.TryGetValue(f, out winFont))
            {
                throw new NotSupportedException();
            }

            if (!_fontWsCache.TryGetValue(winFont, out ws))
            {
                ws = gfx.MeasureString(new char[] { ' ' }, 0, 1, f).Width;
                _fontWsCache[winFont] = ws;
            }
            return ws;
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
        public static void AddFontFamily(System.Drawing.FontFamily fontFamily)
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
    }
}