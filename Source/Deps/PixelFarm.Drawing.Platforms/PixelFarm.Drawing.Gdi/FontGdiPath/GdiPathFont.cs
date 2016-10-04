//MIT, 2014-2016, WinterDev   

using System;
using System.Collections.Generic;

using PixelFarm.Agg;
namespace PixelFarm.Drawing.Fonts
{
    //this is experiment only***
    class GdiPathFont : ActualFont
    {
        GdiPathFontFace fontface;
        int emSizeInPoints;
        float emSizeInPixels;

        Agg.VertexSource.CurveFlattener curveFlattener = new Agg.VertexSource.CurveFlattener();
        Dictionary<char, FontGlyph> cachedGlyphs = new Dictionary<char, FontGlyph>();
        System.Drawing.Font gdiFont;
        public GdiPathFont(GdiPathFontFace fontface, int emSizeInPoints)
        {
            this.fontface = fontface;
            this.emSizeInPoints = emSizeInPoints;
            //--------------------------------------
            emSizeInPixels = Font.ConvEmSizeInPointsToPixels(emSizeInPoints);
            //--------------------------------------
            //implementation
            gdiFont = new System.Drawing.Font(fontface.FaceName, emSizeInPoints);
        }
        public override float GetAdvanceForCharacter(char c)
        {
            throw new NotImplementedException();
        }
        public override float GetAdvanceForCharacter(char c, char next_c)
        {
            throw new NotImplementedException();
        }
        public override float AscentInPixels
        {
            get { throw new NotImplementedException(); }
        }

        public override float DescentInPixels
        {
            get { throw new NotImplementedException(); }
        }
        public override FontGlyph GetGlyphByIndex(uint glyphIndex)
        {
            throw new NotImplementedException();
        }
        public override FontFace FontFace
        {
            get { return this.fontface; }
        }
        public override float EmSizeInPixels
        {
            get { return emSizeInPixels; }
        }
        public override FontGlyph GetGlyph(char c)
        {
            FontGlyph found;
            if (!this.cachedGlyphs.TryGetValue(c, out found))
            {
                //if not found then create new one
                found = new FontGlyph();
                //------------------------
                //create vector version, using Path
                VertexStore vxs = new VertexStore();
                PixelFarm.Agg.GdiPathConverter.ConvertCharToVertexGlyph(gdiFont, c, vxs);
                found.originalVxs = vxs;
                //create flatten version 
                found.flattenVxs = curveFlattener.MakeVxs(vxs);//?
                //-------------------------
                //create bmp version 
                //find vxs bound 
                this.cachedGlyphs.Add(c, found);
            }
            return found;
        }
        public override void GetGlyphPos(char[] buffer, int start, int len, ProperGlyph[] properGlyphs)
        {
            throw new NotImplementedException();
        }

        protected override void OnDispose()
        {
        }
        public override float EmSize
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}