//MIT, 2014-2016, WinterDev   

using System;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;
using PixelFarm.Drawing.Fonts;
using Win32;

namespace PixelFarm.Drawing.WinGdi
{


    class WinGdiFont : ActualFont
    {
        float emSize;
        float emSizeInPixels;
        float ascendInPixels;
        float descentInPixels;

        static BasicGdi32FontHelper basGdi32FontHelper = new BasicGdi32FontHelper();

        int[] charWidths;
        NativeTextWin32.FontABC[] charAbcWidths;
        NativeWin32MemoryDc nativeWin32MemDc;
        IntPtr hfont;
        int bmpWidth = 200;
        int bmpHeight = 50;

        //eg.
        Encoding fontEncoding = Encoding.GetEncoding(874);
        PixelFarm.Drawing.RequestFont f;
        public WinGdiFont(PixelFarm.Drawing.RequestFont f)
        {

            this.f = f;
            bmpWidth = 10;
            bmpHeight = 10;
            nativeWin32MemDc = new NativeWin32MemoryDc(bmpWidth, bmpHeight);
            //this will create 
            InitFont(f.Name, (int)f.SizeInPoints);
            nativeWin32MemDc.SetTextColor(0);
            //------------------------------------------------------------------
            //create gdi font from font data
            this.emSize = f.SizeInPoints;
            this.emSizeInPixels = PixelFarm.Drawing.RequestFont.ConvEmSizeInPointsToPixels(this.emSize);
            //
            //build font matrix
            basGdi32FontHelper.MeasureCharWidths(hfont, out charWidths, out charAbcWidths);
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
            get { return f.Name; }
        }
        public override FontStyle FontStyle
        {
            get { return f.Style; }
        }
        void InitFont(string fontName, int emHeight)
        {
            MyWin32.LOGFONT logFont = new MyWin32.LOGFONT();
            MyWin32.SetFontName(ref logFont, fontName);
            logFont.lfHeight = emHeight;
            logFont.lfCharSet = 1;//default
            logFont.lfQuality = 0;//default
            hfont = MyWin32.CreateFontIndirect(ref logFont);
            MyWin32.SelectObject(this.nativeWin32MemDc.DC, hfont);
        }

        public System.IntPtr ToHfont()
        {   /// <summary>
            /// Set a resource (e.g. a font) for the specified device context.
            /// WARNING: Calling Font.ToHfont() many times without releasing the font handle crashes the app.
            /// </summary>
            return this.hfont;
        }
        public override float SizeInPoints
        {
            get { return emSize; }
        }
        public override float SizeInPixels
        {
            get { return emSizeInPixels; }
        }
        protected override void OnDispose()
        {

            //TODO: review here 
            Win32Utils.DeleteObject(hfont);
            hfont = IntPtr.Zero;
            nativeWin32MemDc.Dispose();
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

        //public override void GetGlyphPos(char[] buffer, int start, int len, ProperGlyph[] properGlyphs)
        //{
        //    //get gyph pos
        //    throw new NotImplementedException();
        //}

        //char[] singleCharArray = new char[1];
        //byte[] codePoints = new byte[2];

        //public override float GetAdvanceForCharacter(char c)
        //{
        //    //check if we have width got this char or not
        //    //temp fix
        //    //TODO: review here again ***
        //    singleCharArray[0] = c;
        //    fontEncoding.GetBytes(singleCharArray, 0, 1, codePoints, 0);
        //    return charWidths[codePoints[0]];
        //}

      



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

        public override float DescentInPixels
        {
            get
            {
                return descentInPixels;
            }
        }
    }


    class WinGdiFontSystem
    {

        static RequestFont latestFont;
        static WinGdiFont latestWinFont;
        static Dictionary<RequestFont, WinGdiFont> registerFonts = new Dictionary<RequestFont, WinGdiFont>();
        static public WinGdiFont GetWinGdiFont(RequestFont f)
        {
            if (f == null)
            {
                throw new NotSupportedException();
            }
            if (f == latestFont)
            {
                return latestWinFont;
            }
            //-----
            //get register font or create the new one
            WinGdiFont found;
            if (!registerFonts.TryGetValue(f, out found))
            {
                //create the new one and register                  
                found = new WinGdiFont(f);
            }
            latestFont = f;
            RequestFont.SetCacheActualFont(f, latestWinFont);
            return latestWinFont = found;
        }
    }

    public static class WinGdiTextService
    {
        static NativeWin32MemoryDc win32MemDc;

        //=====================================
        //static 
        static readonly int[] _charFit = new int[1];
        static readonly int[] _charFitWidth = new int[1000];

        static float whitespaceSize = -1;
        static WinGdiTextService()
        {
            win32MemDc = new NativeWin32MemoryDc(2, 2);
        }
        public static float MeasureWhitespace(RequestFont f)
        {
            return whitespaceSize = MeasureString(new char[] { ' ' }, 0, 1, f).Width;
        }
        static void SetFont(RequestFont font)
        {
            WinGdiFont winFont = WinGdiFontSystem.GetWinGdiFont(font);
            Win32Utils.SelectObject(win32MemDc.DC, winFont.ToHfont());
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

        public static ActualFont GetWinGdiFont(RequestFont f)
        {
            return WinGdiFontSystem.GetWinGdiFont(f);
        }


    }
}