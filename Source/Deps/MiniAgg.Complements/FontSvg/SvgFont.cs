//MIT, 2014-2016, WinterDev
//----------------------------------- 

using System;
using System.Collections.Generic;
using PixelFarm.Agg;
using PixelFarm.Agg.Transform;

namespace PixelFarm.Drawing.Fonts
{
    class SvgFont : OutlineFont
    {
        SvgFontFace fontface;
        int emSizeInPoints;
        int emSizeInPixels;
        double currentEmScalling;
        Dictionary<char, FontGlyph> cachedGlyphs = new Dictionary<char, FontGlyph>();
        Dictionary<uint, FontGlyph> cachedGlyphsByIndex = new Dictionary<uint, FontGlyph>();
        Affine scaleTx;
        PixelFarm.Agg.VertexSource.CurveFlattener curveFlattner = new PixelFarm.Agg.VertexSource.CurveFlattener();
        public SvgFont(SvgFontFace fontface, int emSizeInPoints)
        {
            this.fontface = fontface;
            this.emSizeInPoints = emSizeInPoints;
            //------------------------------------
            emSizeInPixels = (int)Font.ConvEmSizeInPointsToPixels(emSizeInPoints);
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
            if (!cachedGlyphsByIndex.TryGetValue(glyphIndex, out glyph))
            {
                //create font glyph for this font size
                FontGlyph originalGlyph = fontface.GetGlyphByIndex((int)glyphIndex);
                VertexStore characterGlyph = scaleTx.TransformToVxs(originalGlyph.originalVxs);
                glyph = new FontGlyph();
                glyph.originalVxs = characterGlyph;
                //then flatten it
                characterGlyph = curveFlattner.MakeVxs(characterGlyph);
                glyph.flattenVxs = characterGlyph;
                glyph.horiz_adv_x = originalGlyph.horiz_adv_x;
                cachedGlyphsByIndex.Add(glyphIndex, glyph);
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


        //public override FontSpec FontInfo
        //{
        //    get
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

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


    }
}
