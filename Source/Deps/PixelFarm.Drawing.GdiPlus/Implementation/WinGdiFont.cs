//MIT, 2014-2017, WinterDev   

using System;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing.Fonts;
using Win32;

namespace PixelFarm.Drawing.WinGdi
{


    public class WinGdiFontFace : FontFace
    {
        FontFace nopenTypeFontFace;
        FontStyle style;
        static IFontLoader s_fontLoader;
        public WinGdiFontFace(string fontName, FontStyle style)
        {
            this.style = style;
            InstalledFontStyle installedStyle = InstalledFontStyle.Regular;
            switch (style)
            {
                default: break;
                case FontStyle.Bold:
                    installedStyle = InstalledFontStyle.Bold;
                    break;
                case FontStyle.Italic:
                    installedStyle = InstalledFontStyle.Italic;
                    break;
                case FontStyle.Bold | FontStyle.Italic:
                    installedStyle = InstalledFontStyle.Italic;
                    break;
            }

            //resolve
            InstalledFont foundInstalledFont = s_fontLoader.GetFont(fontName, installedStyle);
            //TODO: review 
            this.nopenTypeFontFace = OpenFontLoader.LoadFont(
                foundInstalledFont.FontPath,
                ScriptLangs.Latin,
                WriteDirection.LTR);
        }

        public static void SetFontLoader(IFontLoader fontLoader)
        {
            //set once
            if (s_fontLoader != null)
            {
                throw new NotSupportedException();
            }
            s_fontLoader = fontLoader;
        }
        protected override void OnDispose()
        {
            s_fontLoader = null;
        }
        public override string FontPath
        {
            get { return nopenTypeFontFace.FontPath; }
        }
        public override string Name
        {
            get { return nopenTypeFontFace.Name; }
        }
        public override ActualFont GetFontAtPointsSize(float pointSize)
        {
            //create WinGdiFont 
            return new WinGdiFont(this, pointSize, style);
        }
        public override float GetScale(float pointSize)
        {
            return nopenTypeFontFace.GetScale(pointSize);
        }
        public override int AscentInDzUnit
        {
            get { return nopenTypeFontFace.AscentInDzUnit; }
        }
        public override int DescentInDzUnit
        {
            get { return nopenTypeFontFace.DescentInDzUnit; }
        }
        public override int LineGapInDzUnit
        {
            get { return nopenTypeFontFace.LineGapInDzUnit; }
        }
        public override object GetInternalTypeface()
        {
            throw new NotImplementedException();
        }
    }

    public class WinGdiFont : ActualFont
    {
        /// <summary>
        /// font 'em' height?
        /// </summary>
        float fontSizeInPoints;
        float emSizeInPixels;
        float ascendInPixels;
        float descentInPixels;
        float linegapInPixels;

        WinGdiFontFace fontFace;



        int[] charWidths;
        NativeTextWin32.FontABC[] charAbcWidths;

        IntPtr hfont;
        FontStyle fontStyle;
        public WinGdiFont(WinGdiFontFace fontFace, float sizeInPoints, FontStyle style)
        {

            this.fontFace = fontFace;
            this.fontSizeInPoints = sizeInPoints;
            this.fontStyle = style;

            this.fontSizeInPoints = sizeInPoints;
            this.emSizeInPixels = PixelFarm.Drawing.RequestFont.ConvEmSizeInPointsToPixels(this.fontSizeInPoints);
            this.hfont = InitFont(fontFace.Name, sizeInPoints, style);
            //------------------------------------------------------------------
            //create gdi font from font data
            //build font matrix             ;
            WinGdiTextService.MeasureCharWidths(hfont, out charWidths, out charAbcWidths);
            float scale = fontFace.GetScale(sizeInPoints);
            ascendInPixels = fontFace.AscentInDzUnit * scale;
            descentInPixels = fontFace.DescentInDzUnit * scale;
            linegapInPixels = fontFace.LineGapInDzUnit * scale;

            //------------------------------------------------------------------


            //int emHeightInDzUnit = f.FontFamily.GetEmHeight(f.Style);
            //this.ascendInPixels = Font.ConvEmSizeInPointsToPixels((f.FontFamily.GetCellAscent(f.Style) / emHeightInDzUnit));
            //this.descentInPixels = Font.ConvEmSizeInPointsToPixels((f.FontFamily.GetCellDescent(f.Style) / emHeightInDzUnit));

            ////--------------
            ////we build font glyph, this is just win32 glyph
            ////
            //int j = charAbcWidths.Length;
            //fontGlyphs = new FontGlyph[j];
            //for (int i = 0; i < j; ++i)
            //{
            //    FontGlyph glyph = new FontGlyph();
            //    glyph.horiz_adv_x = charWidths[i] << 6;
            //    fontGlyphs[i] = glyph;
            //}

        }
        public override string FontName
        {
            get { return fontFace.Name; }
        }
        public override FontStyle FontStyle
        {
            get { return fontStyle; }
        }
        static IntPtr InitFont(string fontName, float emHeight, FontStyle style)
        {
            //see: MSDN, LOGFONT structure
            //https://msdn.microsoft.com/en-us/library/windows/desktop/dd145037(v=vs.85).aspx
            MyWin32.LOGFONT logFont = new MyWin32.LOGFONT();
            MyWin32.SetFontName(ref logFont, fontName);
            logFont.lfHeight = -(int)PixelFarm.Drawing.RequestFont.ConvEmSizeInPointsToPixels(emHeight);//minus **
            logFont.lfCharSet = 1;//default
            logFont.lfQuality = 0;//default

            MyWin32.LOGFONT_FontWeight weight =
                ((style & FontStyle.Bold) == FontStyle.Bold) ?
                MyWin32.LOGFONT_FontWeight.FW_BOLD :
                MyWin32.LOGFONT_FontWeight.FW_REGULAR;
            logFont.lfWeight = (int)weight;
            //
            logFont.lfItalic = (byte)(((style & FontStyle.Italic) == FontStyle.Italic) ? 1 : 0);
            return MyWin32.CreateFontIndirect(ref logFont);
        }


        public System.IntPtr ToHfont()
        {
            return this.hfont;
        }
        public override float SizeInPoints
        {
            get { return fontSizeInPoints; }
        }
        public override float SizeInPixels
        {
            get { return emSizeInPixels; }
        }
        protected override void OnDispose()
        {

            //TODO: review here 
            MyWin32.DeleteObject(hfont);
            hfont = IntPtr.Zero;

        }


        internal NativeTextWin32.FontABC[] GetInteralABCArray()
        {
            return this.charAbcWidths;
        }


        public override FontGlyph GetGlyphByIndex(uint glyphIndex)
        {
            throw new NotImplementedException();
        }
        public override FontGlyph GetGlyph(char c)
        {
            //convert c to glyph index
            //temp fix  
            throw new NotImplementedException();
        }
        public override FontFace FontFace
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        public override float AscentInPixels
        {
            get
            {
                return ascendInPixels;
            }
        }
        public override float LineGapInPixels
        {
            get { return linegapInPixels; }
        }
        public override float RecommendedLineSpacingInPixels
        {
            get { return AscentInPixels - DescentInPixels + LineGapInPixels; }
        }
        public override float DescentInPixels
        {
            get
            {
                return descentInPixels;
            }
        }
        public void AssignToRequestFont(RequestFont r)
        {
            SetCacheActualFont(r, this);
        }
        public static WinGdiFont GetCacheFontAsWinGdiFont(RequestFont r)
        {
            return GetCacheActualFont(r) as WinGdiFont;
        }
    }

    struct FontFaceKey
    {
        public readonly int FontNameIndex;
        public readonly FontStyle FontStyle;
        public FontFaceKey(FontKey fontKey)
        {
            this.FontNameIndex = fontKey.FontNameIndex;
            this.FontStyle = fontKey.FontStyle;
        }
    }
    public class WinGdiFontSystem
    {

        static RequestFont latestFont;
        static WinGdiFont latestWinFont;
        static Dictionary<FontKey, WinGdiFont> registerFonts = new Dictionary<FontKey, WinGdiFont>();
        static Dictionary<FontFaceKey, WinGdiFontFace> winGdiFonFaces = new Dictionary<FontFaceKey, WinGdiFontFace>();

        public static WinGdiFont GetWinGdiFont(RequestFont f)
        {
            if (f == null)
            {
                throw new NotSupportedException();
            }
            if (f == latestFont)
            {
                return latestWinFont;
            }
            WinGdiFont actualFontInside = WinGdiFont.GetCacheFontAsWinGdiFont(f);
            if (actualFontInside != null)
            {
                return actualFontInside;
            }
            //-----
            //need to create a new one
            //get register font or create the new one
            FontKey key = f.FontKey;
            WinGdiFont found;
            if (!registerFonts.TryGetValue(key, out found))
            {
                //create the new one and register                  
                //create fontface
                FontFaceKey fontfaceKey = new FontFaceKey(key);
                WinGdiFontFace fontface;
                if (!winGdiFonFaces.TryGetValue(fontfaceKey, out fontface))
                {
                    //create new 
                    fontface = new WinGdiFontFace(f.Name, f.Style);
                    winGdiFonFaces.Add(fontfaceKey, fontface);
                }

                found = (WinGdiFont)fontface.GetFontAtPointsSize(f.SizeInPoints);
                registerFonts.Add(key, found);//cache here
            }
            latestFont = f;
            found.AssignToRequestFont(f);
            return latestWinFont = found;
        }
    }

    public static class WinGdiTextService
    {
        //TODO: consider use uniscribe

        static NativeWin32MemoryDc win32MemDc;

        //=====================================
        //static 
        static readonly int[] _charFit = new int[1];
        static readonly int[] _charFitWidth = new int[1000];

        static float whitespaceSize = -1;
        static char[] whitespace = new char[1];
        static Encoding s_en;
        static WinGdiTextService()
        {
            win32MemDc = new NativeWin32MemoryDc(2, 2);
            whitespace[0] = ' ';
        }
        public static void SetDefaultEncoding(Encoding en)
        {
            s_en = en;
        }

        const int MAX_CODEPOINT_NO = 255;
        internal static void MeasureCharWidths(IntPtr hFont,
            out int[] charWidths,
            out NativeTextWin32.FontABC[] abcSizes)
        {

            //only in ascii range
            //current version
            charWidths = new int[MAX_CODEPOINT_NO + 1]; // 
            MyWin32.SelectObject(win32MemDc.DC, hFont);
            unsafe
            {
                //see: https://msdn.microsoft.com/en-us/library/ms404377(v=vs.110).aspx
                //A code page contains 256 code points and is zero-based.
                //In most code pages, code points 0 through 127 represent the ASCII character set,
                //and code points 128 through 255 differ significantly between code pages
                abcSizes = new NativeTextWin32.FontABC[MAX_CODEPOINT_NO + 1];
                fixed (NativeTextWin32.FontABC* abc = abcSizes)
                {
                    NativeTextWin32.GetCharABCWidths(win32MemDc.DC, (uint)0, (uint)MAX_CODEPOINT_NO, abc);
                }
                for (int i = 0; i < (MAX_CODEPOINT_NO + 1); ++i)
                {
                    charWidths[i] = abcSizes[i].Sum;
                }

            }
        }
        public static float MeasureWhitespace(RequestFont f)
        {
            return whitespaceSize = MeasureString(whitespace, 0, 1, f).Width;
        }
        static WinGdiFont SetFont(RequestFont font)
        {
            WinGdiFont winFont = WinGdiFontSystem.GetWinGdiFont(font);

            MyWin32.SelectObject(win32MemDc.DC, winFont.ToHfont());
            return winFont;
        }
        public static Size MeasureString(char[] buff, int startAt, int len, RequestFont font)
        {
            //if (_useGdiPlusTextRendering)
            //{
            //    ReleaseHdc();
            //    _characterRanges[0] = new System.Drawing.CharacterRange(0, len);
            //    _stringFormat.SetMeasurableCharacterRanges(_characterRanges);
            //    System.Drawing.Font font2 = (System.Drawing.Font)font.InnerFont;

            //    var size = gx.MeasureCharacterRanges(
            //        new string(buff, startAt, len),
            //        font2,
            //        System.Drawing.RectangleF.Empty,
            //        _stringFormat)[0].GetBounds(gx).Size;
            //    return new PixelFarm.Drawing.Size((int)Math.Round(size.Width), (int)Math.Round(size.Height));
            //}
            //else
            //{
            SetFont(font);
            PixelFarm.Drawing.Size size = new Size();
            if (buff.Length > 0)
            {
                unsafe
                {
                    fixed (char* startAddr = &buff[0])
                    {
                        NativeTextWin32.UnsafeGetTextExtentPoint32(win32MemDc.DC, startAddr + startAt, len, ref size);
                    }
                }
            }

            return size;
            //}
        }
        /// <summary>
        /// Measure the width and height of string <paramref name="str"/> when drawn on device context HDC
        /// using the given font <paramref name="font"/>.<br/>
        /// Restrict the width of the string and get the number of characters able to fit in the restriction and
        /// the width those characters take.
        /// </summary>
        /// <param name="str">the string to measure</param>
        /// <param name="font">the font to measure string with</param>
        /// <param name="maxWidth">the max width to render the string in</param>
        /// <param name="charFit">the number of characters that will fit under <see cref="maxWidth"/> restriction</param>
        /// <param name="charFitWidth"></param>
        /// <returns>the size of the string</returns>
        public static Size MeasureString(char[] buff, int startAt, int len, RequestFont font, float maxWidth, out int charFit, out int charFitWidth)
        {
            //if (_useGdiPlusTextRendering)
            //{
            //    ReleaseHdc();
            //    throw new NotSupportedException("Char fit string measuring is not supported for GDI+ text rendering");
            //}
            //else
            //{
            SetFont(font);
            if (buff.Length == 0)
            {
                charFit = 0;
                charFitWidth = 0;
                return Size.Empty;
            }
            var size = new PixelFarm.Drawing.Size();
            unsafe
            {
                fixed (char* startAddr = &buff[0])
                {
                    NativeTextWin32.UnsafeGetTextExtentExPoint(
                        win32MemDc.DC, startAddr + startAt, len,
                        (int)Math.Round(maxWidth), _charFit, _charFitWidth, ref size);
                }
            }
            charFit = _charFit[0];
            charFitWidth = charFit > 0 ? _charFitWidth[charFit - 1] : 0;
            return size;
            //}
        }
        //==============================================

        public static void CalculateGlyphAdvancePos(char[] str, int startAt, int len, RequestFont font, int[] glyphXAdvances)
        {
            unsafe
            {
                if (len == 0)
                {
                    return;
                }

                WinGdiFont winfont = SetFont(font);
                //ushort* glyhIndics = stackalloc ushort[len];
                //fixed (char* s = &str[startAt])
                //{
                //    NativeTextWin32.GetGlyphIndices(
                //        win32MemDc.DC,
                //        s,
                //        len,
                //        glyhIndics,
                //        0);
                //}
                byte[] encoding = s_en.GetBytes(str, startAt, len);
                NativeTextWin32.FontABC[] abcWidths = winfont.GetInteralABCArray();
                for (int i = 0; i < len; ++i)
                {
                    //ushort glyphIndex = *(glyhIndics + i);
                    int enc_index = encoding[i];
                    if (enc_index == 0)
                    {
                        break;//?
                    }
                    glyphXAdvances[i] = abcWidths[enc_index].Sum;
                }
            }
            //unsafe
            //{
            //    SetFont(font);
            //    NativeTextWin32.GCP_RESULTS gpcResult = new NativeTextWin32.GCP_RESULTS();
            //    int[] caretpos = new int[len];
            //    uint[] lpOrder = new uint[len];
            //    int[] lpDx = new int[len];
            //    fixed (int* lpdx_h = &lpDx[0])
            //    fixed (uint* lpOrder_h = &lpOrder[0])
            //    fixed (int* caretpos_h = &caretpos[0])
            //    fixed (char* str_h = &str[startAt])
            //    {
            //        gpcResult.lpCaretPos = caretpos_h;
            //        gpcResult.lpOrder = lpOrder_h;
            //        gpcResult.lpDx = lpdx_h;
            //        //gpcResult.
            //        ////gpcResult.lpCaretPos = 
            //        NativeTextWin32.GetCharacterPlacement(
            //            win32MemDc.DC,
            //            str_h,
            //            len,
            //            len, ref gpcResult, 0);

            //    }

            //}

        }
        public static ActualFont GetWinGdiFont(RequestFont f)
        {
            return WinGdiFontSystem.GetWinGdiFont(f);
        }



    }
}