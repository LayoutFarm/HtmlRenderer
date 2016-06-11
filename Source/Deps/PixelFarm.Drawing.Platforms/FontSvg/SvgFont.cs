// 2015,2014 ,MIT, WinterDev
//----------------------------------- 

using System;
using System.Collections.Generic;
using PixelFarm.Agg.Transform;
namespace PixelFarm.Agg.Fonts
{
    class SvgFont : Font
    {
        SvgFontFace fontface;
        int emSizeInPoints;
        const int POINTS_PER_INCH = 72;
        const int PIXEL_PER_INCH = 96;
        int emSizeInPixels;
        double currentEmScalling;
        Dictionary<char, FontGlyph> cachedGlyphs = new Dictionary<char, FontGlyph>();
        Affine scaleTx;
        PixelFarm.Agg.VertexSource.CurveFlattener curveFlattner = new VertexSource.CurveFlattener();
        public SvgFont(SvgFontFace fontface, int emSizeInPoints)
        {
            this.fontface = fontface;
            this.emSizeInPoints = emSizeInPoints;
            //------------------------------------
            emSizeInPixels = (int)(((float)emSizeInPoints / (float)POINTS_PER_INCH) * (float)PIXEL_PER_INCH);
            currentEmScalling = (float)emSizeInPixels / (float)fontface.UnitsPerEm;
            scaleTx = Affine.NewMatix(AffinePlan.Scale(currentEmScalling));
        }
        public override FontFace FontFace
        {
            get { return fontface; }
        }

        public override FontGlyph GetGlyphByIndex(uint glyphIndex)
        {
            FontGlyph glyph;
            //temp
            char c = (char)glyphIndex;
            if (!cachedGlyphs.TryGetValue(c, out glyph))
            {
                //create font glyph for this font size
                var originalGlyph = fontface.GetGlyphForCharacter(c);
                VertexStore characterGlyph = scaleTx.TransformToVxs(originalGlyph.originalVxs);
                glyph = new FontGlyph();
                glyph.originalVxs = characterGlyph;
                //then flatten it
                characterGlyph = curveFlattner.MakeVxs(characterGlyph);
                glyph.flattenVxs = characterGlyph;
                glyph.horiz_adv_x = originalGlyph.horiz_adv_x;
                cachedGlyphs.Add(c, glyph);
            }
            return glyph;
        }
        public override FontGlyph GetGlyph(char c)
        {
            FontGlyph glyph;
            if (!cachedGlyphs.TryGetValue(c, out glyph))
            {
                //create font glyph for this font size
                var originalGlyph = fontface.GetGlyphForCharacter(c);
                VertexStore characterGlyph = scaleTx.TransformToVxs(originalGlyph.originalVxs);
                glyph = new FontGlyph();
                glyph.horiz_adv_x = originalGlyph.horiz_adv_x;
                glyph.originalVxs = characterGlyph;
                //then flatten it
                characterGlyph = curveFlattner.MakeVxs(characterGlyph);
                glyph.flattenVxs = characterGlyph;
                cachedGlyphs.Add(c, glyph);
            }
            return glyph;
        }
        public override void GetGlyphPos(char[] buffer, int start, int len, ProperGlyph[] properGlyphs)
        {
            //find proper position for each glyph
            int j = buffer.Length;
            for (int i = 0; i < j; ++i)
            {
                FontGlyph f = this.GetGlyph(buffer[i]);
                properGlyphs[i].x_advance = f.horiz_adv_x >> 6; //64
                properGlyphs[i].codepoint = (uint)buffer[i];
            }
        }
        protected override void OnDispose()
        {
        }
        public override int EmSizeInPixels
        {
            get { return emSizeInPixels; }
        }
        public override int GetAdvanceForCharacter(char c)
        {
            return this.GetGlyph(c).horiz_adv_x >> 6;//64
        }
        public override int GetAdvanceForCharacter(char c, char next_c)
        {
            return this.GetGlyph(c).horiz_adv_x >> 6;//64
        }
        public override double AscentInPixels
        {
            get
            {
                return fontface.Ascent * currentEmScalling;
            }
        }

        public override double DescentInPixels
        {
            get
            {
                return fontface.Descent * currentEmScalling;
            }
        }

        public override double XHeightInPixels
        {
            get
            {
                return fontface.X_height * currentEmScalling;
            }
        }

        public override double CapHeightInPixels
        {
            get
            {
                return fontface.Cap_height * currentEmScalling;
            }
        }
        public override bool IsAtlasFont
        {
            get { return false; }
        }
    }
}
