//MIT, 2014-2016, WinterDev
//----------------------------------- 

using System;
using System.Collections.Generic;
namespace PixelFarm.Drawing.Fonts
{
    /// <summary>
    /// cross platform font
    /// </summary>
    public class NativeFont : ActualFont
    {
        NativeFontFace ownerFace;
        int fontSizeInPixelUnit;
        /// <summary>
        /// glyph
        /// </summary>
        Dictionary<char, FontGlyph> dicGlyphs = new Dictionary<char, FontGlyph>();
        Dictionary<uint, FontGlyph> dicGlyphs2 = new Dictionary<uint, FontGlyph>();
        internal NativeFont(NativeFontFace ownerFace, int pixelSize)
        {
            //store unmanage font file information
            this.ownerFace = ownerFace;
            this.fontSizeInPixelUnit = pixelSize;
        }
        protected override void OnDispose()
        {
            //TODO: clear resource here 

        }
        public override FontGlyph GetGlyph(char c)
        {
            FontGlyph found;
            if (!dicGlyphs.TryGetValue(c, out found))
            {
                found = ownerFace.ReloadGlyphFromChar(c, fontSizeInPixelUnit);
                this.dicGlyphs.Add(c, found);
            }
            return found;
        }
        public override FontGlyph GetGlyphByIndex(uint glyphIndex)
        {
            FontGlyph found;
            if (!dicGlyphs2.TryGetValue(glyphIndex, out found))
            {
                //not found glyph 
                found = ownerFace.ReloadGlyphFromIndex(glyphIndex, fontSizeInPixelUnit);
                this.dicGlyphs2.Add(glyphIndex, found);
            }
            return found;
        }

        /// <summary>
        /// owner font face
        /// </summary>
        public override FontFace FontFace
        {
            get { return this.ownerFace; }
        }
        internal NativeFontFace NativeFontFace
        {
            get { return this.ownerFace; }
        }
        public override void GetGlyphPos(char[] buffer, int start, int len, ProperGlyph[] properGlyphs)
        {
            unsafe
            {
                fixed (ProperGlyph* propGlyphH = &properGlyphs[0])
                fixed (char* head = &buffer[0])
                {
                    //we use font shaping engine here
                    NativeMyFontsLib.MyFtShaping(
                        this.NativeFontFace.HBFont,
                        head,
                        buffer.Length,
                        propGlyphH);
                }
            }
        }
        public override double AscentInPixels
        {
            get { throw new NotImplementedException(); }
        }
        public override double CapHeightInPixels
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        public override double DescentInPixels
        {
            get { throw new NotImplementedException(); }
        }
        public override int EmSizeInPixels
        {
            get { throw new NotImplementedException(); }
        }

        public override double XHeightInPixels
        {
            get { throw new NotImplementedException(); }
        }
        public override int GetAdvanceForCharacter(char c)
        {
            throw new NotImplementedException();
        }
        public override int GetAdvanceForCharacter(char c, char next_c)
        {
            throw new NotImplementedException();
        }

        public override string Name
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override int Height
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override float EmSize
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override FontStyle Style
        {
            get
            {
                throw new NotImplementedException();
            }
        }


        //---------------------------------------------------------------------------
        public static GlyphImage BuildMsdfFontImage(FontGlyph fontGlyph)
        {
            return NativeFontGlyphBuilder.BuildMsdfFontImage(fontGlyph);
        }
       
        public static void SwapColorComponentFromBigEndianToWinGdi(int[] bitbuffer)
        {
            unsafe
            {
                int j = bitbuffer.Length;
                fixed (int* p0 = &(bitbuffer[j - 1]))
                {
                    int* p = p0;
                    for (int i = j - 1; i >= 0; --i)
                    {
                        int color = *p;
                        int a = color >> 24;
                        int b = (color >> 16) & 0xff;
                        int g = (color >> 8) & 0xff;
                        int r = color & 0xff;
                        *p = (a << 24) | (r << 16) | (g << 8) | b;
                        p--;
                    }
                }
            }
        }
    }
}