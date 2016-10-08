//MIT, 2014-2016, WinterDev   

using System;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing.Fonts;
using PixelFarm.Agg;
using Win32;
namespace PixelFarm.Drawing.WinGdi
{
    //*** this class need System.Drawing  
    class WinGdiPlusFont : ActualFont
    {
        System.Drawing.Font myFont;
        System.IntPtr hFont;
        float emSize;
        float emSizeInPixels;
        float ascendInPixels;
        float descentInPixels;

        int[] charWidths;
        Win32.NativeTextWin32.FontABC[] charAbcWidths;
        FontGlyph[] fontGlyphs;
        Encoding fontEncoding = Encoding.GetEncoding(874);
        bool hasWhitespaceLength;
        float whitespaceLength;
       

        public WinGdiPlusFont(System.Drawing.Font f)
        {
            this.myFont = f;
            this.hFont = f.ToHfont();

            this.emSize = f.SizeInPoints;
            this.emSizeInPixels = Font.ConvEmSizeInPointsToPixels(this.emSize);
            //
            //build font matrix
            basGdi32FontHelper.MeasureCharWidths(hFont, out charWidths, out charAbcWidths);
            int emHeightInDzUnit = f.FontFamily.GetEmHeight(f.Style);

            this.ascendInPixels = Font.ConvEmSizeInPointsToPixels((f.FontFamily.GetCellAscent(f.Style) / emHeightInDzUnit));
            this.descentInPixels = Font.ConvEmSizeInPointsToPixels((f.FontFamily.GetCellDescent(f.Style) / emHeightInDzUnit));

            //--------------
            //we build font glyph, this is just win32 glyph
            //
            int j = charAbcWidths.Length;
#if DEBUG
            if (j > 256)
            {
                throw new NotSupportedException();
            }
#endif
            fontGlyphs = new FontGlyph[j];
            for (int i = 0; i < j; ++i)
            {
                FontGlyph glyph = new FontGlyph();
                glyph.codePoint = i;

                codePoints[0] = (byte)i;
                fontEncoding.GetChars(codePoints, 0, 1, singleCharArray, 0);
                glyph.unicode = singleCharArray[0];
                glyph.horiz_adv_x = charWidths[i] << 6;
                fontGlyphs[i] = glyph;
            }
        }
        public bool HasWhiteSpaceLength
        {
            get { return hasWhitespaceLength; }
        }
        public float WhitespaceLength
        {
            get
            {
                return whitespaceLength;
            }
            set
            {
                whitespaceLength = value;
                hasWhitespaceLength = true;
            }
        }
        public float GdiPlusFontHeight
        {
            get;
            set;
        }
            
        public System.IntPtr ToHfont()
        {   /// <summary>
            /// Set a resource (e.g. a font) for the specified device context.
            /// WARNING: Calling Font.ToHfont() many times without releasing the font handle crashes the app.
            /// </summary>
            return this.hFont;
        }
       
        
        
        public override float EmSize
        {
            get { return emSize; }
        }
        public override float EmSizeInPixels
        {
            get { return emSizeInPixels; }
        }
        protected override void OnDispose()
        {
            if (myFont != null)
            {
                myFont.Dispose();
                myFont = null;
            }
        }
      
        public override FontGlyph GetGlyphByIndex(uint glyphIndex)
        {
            FontGlyph glyph = fontGlyphs[glyphIndex];
            //build glyph vertex
            if (glyph.flattenVxs == null)
            {
                BuildVxs(glyph, (char)glyph.unicode);
            }

            return glyph;
        }
        public override FontGlyph GetGlyph(char c)
        {
            //convert c to glyph index
            //temp fix  
            singleCharArray[0] = c;
            fontEncoding.GetBytes(singleCharArray, 0, 1, codePoints, 0);
            FontGlyph glyph = fontGlyphs[codePoints[0]];
            //build glyph vertex
            if (glyph.flattenVxs == null)
            {
                BuildVxs(glyph, c);
            }
            return glyph;
        }
        void BuildVxs(FontGlyph glyph, char c)
        {
            //------------------------
            //create vector version, using Path
            VertexStore vxs = new VertexStore();
            PixelFarm.Agg.GdiPathConverter.ConvertCharToVertexGlyph(this.myFont, c, vxs);
            glyph.originalVxs = vxs;
            //create flatten version 
            glyph.flattenVxs = curveFlattener.MakeVxs(vxs);//?
            //-------------------------
        }
        FontGlyph InternalGetGlyph(char c)
        {
            singleCharArray[0] = c;
            fontEncoding.GetBytes(singleCharArray, 0, 1, codePoints, 0);
            return fontGlyphs[codePoints[0]];
        }
        public override void GetGlyphPos(char[] buffer, int start, int len, ProperGlyph[] properGlyphs)
        {
            //simple 
            //TODO: review here again ***
            int j = buffer.Length;
            int offset = 0;
            for (int i = 0; i < j; ++i)
            {
                FontGlyph glyph = InternalGetGlyph(buffer[i]);
                var propGlyph = new ProperGlyph();
                propGlyph.codepoint = (uint)glyph.codePoint;
                propGlyph.x_offset = offset;
                propGlyph.x_advance = glyph.horiz_adv_x >> 6;
                offset += propGlyph.x_advance;
                properGlyphs[i] = propGlyph;
            }
        }
        char[] singleCharArray = new char[1];
        byte[] codePoints = new byte[2];
        public override float GetAdvanceForCharacter(char c)
        {
            //check if we have width got this char or not
            //temp fix
            //TODO: review here again ***
            singleCharArray[0] = c;
            fontEncoding.GetBytes(singleCharArray, 0, 1, codePoints, 0);
            return charWidths[codePoints[0]];
        }

        public override float GetAdvanceForCharacter(char c, char next_c)
        {
            //TODO: review here
            return GetAdvanceForCharacter(c);
            //throw new NotImplementedException();
        }

        public System.Drawing.Font InnerFont
        {
            get { return this.myFont; }
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

        public override float DescentInPixels
        {
            get
            {
                return descentInPixels;
            }
        }

        static BasicGdi32FontHelper basGdi32FontHelper = new BasicGdi32FontHelper();
        static Agg.VertexSource.CurveFlattener curveFlattener = new Agg.VertexSource.CurveFlattener();


    }
}