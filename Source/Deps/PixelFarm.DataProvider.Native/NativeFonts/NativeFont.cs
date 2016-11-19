//MIT, 2014-2016, WinterDev
//----------------------------------- 

using System;
using System.Collections.Generic;
namespace PixelFarm.Drawing.Fonts
{
    /// <summary>
    /// cross platform font
    /// </summary>
    class NativeFont : ActualFont
    {
        NativeFontFace ownerFace;
        float emSizeInPoints;
        int fontSizeInPixelUnit;
        float fontFaceAscentInPx;
        float fontFaceDescentInPx;
        float lineGapInPx;
        string fontName;
        FontStyle fontStyle;
        /// <summary>
        /// glyph
        /// </summary>
        Dictionary<char, FontGlyph> dicGlyphs = new Dictionary<char, FontGlyph>();
        Dictionary<uint, FontGlyph> dicGlyphs2 = new Dictionary<uint, FontGlyph>();
        internal NativeFont(NativeFontFace ownerFace, string fontName, FontStyle fontStyle, int pixelSize)
        {
            this.fontName = fontName;
            this.fontStyle = fontStyle;
            //store unmanage font file information
            this.ownerFace = ownerFace;
            this.fontSizeInPixelUnit = pixelSize;
            this.fontStyle = fontStyle;
            int ascentEmSize = ownerFace.Ascent / ownerFace.UnitPerEm;
            fontFaceAscentInPx = RequestFont.ConvEmSizeInPointsToPixels(ascentEmSize);

            int descentEmSize = ownerFace.Descent / ownerFace.UnitPerEm;
            fontFaceDescentInPx = RequestFont.ConvEmSizeInPointsToPixels(descentEmSize);


            int lineGap = ownerFace.LineGapInDzUnit / ownerFace.UnitPerEm;
            lineGapInPx = RequestFont.ConvEmSizeInPointsToPixels(lineGap);
        }

        public override string FontName
        {
            get { return fontName; }
        }
        public override FontStyle FontStyle
        {
            get { return fontStyle; }
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
        internal void SetEmSizeInPoint(float emSizeInPoints)
        {
            this.emSizeInPoints = emSizeInPoints;
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

        public override float AscentInPixels
        {
            get { return fontFaceAscentInPx; }
        }

        public override float DescentInPixels
        {
            get { return fontFaceDescentInPx; }
        }
        public override float SizeInPoints { get { return this.emSizeInPoints; } }
        public override float SizeInPixels { get { return fontSizeInPixelUnit; } }
        public override float LineGapInPixels
        {
            get { return lineGapInPx; }
        }
        public override float RecommendedLineSpacingInPixels
        {
            get { return AscentInPixels - DescentInPixels + LineGapInPixels; }
        }



    }
}