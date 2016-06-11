// 2015,2014 ,MIT, WinterDev
//----------------------------------- 

using System;
using System.Collections.Generic;
namespace PixelFarm.Agg.Fonts
{
    class NativeFont : Font
    {
        NativeFontFace ownerFace;
        float fontSizeInPoint;
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

        public float SizeInPoint
        {
            get { return this.fontSizeInPoint; }
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
        public override bool IsAtlasFont
        {
            get { return false; }
        }
    }
}