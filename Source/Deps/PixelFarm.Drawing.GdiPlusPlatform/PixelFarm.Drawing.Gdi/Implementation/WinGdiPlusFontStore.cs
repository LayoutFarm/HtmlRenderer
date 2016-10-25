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
using PixelFarm.Drawing.Fonts;

namespace PixelFarm.Drawing.WinGdi
{

    class WinGdiPlusFontStore
    {

        /// <summary>
        /// Allow to map not installed fonts to different
        /// </summary>
        static readonly Dictionary<string, string> _fontsMapping = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
        /// <summary>
        /// collection of all installed and added font families to check if font exists
        /// </summary>
        static readonly Dictionary<string, System.Drawing.FontFamily> _existingFontFamilies = new Dictionary<string, System.Drawing.FontFamily>(StringComparer.InvariantCultureIgnoreCase);


        readonly Dictionary<FontKey, WinGdiPlusFont> resolvedWinGdiFont = new Dictionary<FontKey, WinGdiPlusFont>();

        public WinGdiPlusFontStore()
        {

        }
        public WinGdiPlusFont ResolveFont(PixelFarm.Drawing.RequestFont f)
        {
            WinGdiPlusFont currentFont = f.ActualFont as WinGdiPlusFont;
            if (currentFont != null)
            {
                return currentFont;
            }
            //check if we have cache this font 
            //if not then try create it
            //1. create font key
            FontKey fk = new FontKey(f.Name, f.SizeInPoints, f.Style);

            if (!resolvedWinGdiFont.TryGetValue(fk, out currentFont))
            {
                //not found 
                //then create it
                currentFont = CreateFont(f.Name, f.SizeInPoints, (System.Drawing.FontStyle)f.Style);

            }
            f.ActualFont = currentFont;
            return currentFont;

        }
        WinGdiPlusFont CreateFont(string family,
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

            return RegisterFont(newFont, new PixelFarm.Drawing.Fonts.FontKey(family, size, (PixelFarm.Drawing.FontStyle)style));
        }

        WinGdiPlusFont RegisterFont(System.Drawing.Font newFont, PixelFarm.Drawing.Fonts.FontKey fontKey)
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

            WinGdiPlusFont found;
            if (!resolvedWinGdiFont.TryGetValue(fontKey, out found))
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

                found = new WinGdiPlusFont(newFont);
                found.GdiPlusFontHeight = fontHeight;
                resolvedWinGdiFont.Add(fontKey, found);
                //myFont,
                //fontHeight,
                //(fontAscent * fontSize / fontEmHeight),
                //(descent * fontSize / fontEmHeight),
                //fontHeight * fontAscent / lineSpacing,
                //gdiFontHelper);
                //myFont.SetFontInfo(fontInfo);
                //lineSpacing * newFont.FontFamily.GetCellAscent(newFont.Style) / linespace); 
                //1. info

                //_fontsUnmanagedCache[newFont] = newFont.ToHfont();
                ////2. line cache
                //_fontHeightCache[newFont] = fontHeight;
            }
            return found;
        }


        /// <summary>
        /// Init the system installed fonts.
        /// </summary>
        static WinGdiPlusFontStore()
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


        /// <summary>
        /// Gets the line spacing of the font
        /// </summary>
        /// <param name="font"></param>
        /// <returns>pixel unit of line spacing</returns>
        /// <remarks>
        /// Font metrics from http://msdn.microsoft.com/en-us/library/xwf9s90b(VS.71).aspx
        /// </remarks>
        static float GetLineSpacing(System.Drawing.Font font)
        {
            return font.FontFamily.GetLineSpacing(font.Style) * font.Size / font.FontFamily.GetEmHeight(font.Style);
        }


        /// <summary>
        /// Check if the given font family exists by name
        /// </summary>
        /// <param name="family">the font to check</param>
        /// <returns>true - font exists by given family name, false - otherwise</returns>
        static bool IsFontExists(string family)
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
           PixelFarm.Drawing.IFonts gfx, PixelFarm.Drawing.RequestFont f)
        {

            WinGdiPlusFont winFont;
            if (!resolvedWinGdiFont.TryGetValue(f.FontKey, out winFont))
            {
                throw new NotSupportedException();
            }
            if (!winFont.HasWhiteSpaceLength)
            {
                return winFont.WhitespaceLength = gfx.MeasureString(new char[] { ' ' }, 0, 1, f).Width;
            }
            return winFont.WhitespaceLength;
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