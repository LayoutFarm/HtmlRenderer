//MIT, 2014-2016, WinterDev   

using System;
using System.Collections.Generic;

using PixelFarm.Agg;
namespace PixelFarm.Drawing.Fonts
{
    class GdiPathFont : Font
    {
        GdiPathFontFace fontface;
        int emSizeInPoints;
        const int POINTS_PER_INCH = 72;
        const int PIXEL_PER_INCH = 96;
        int emSizeInPixels;

        Agg.VertexSource.CurveFlattener curveFlattener = new Agg.VertexSource.CurveFlattener();
        Dictionary<char, FontGlyph> cachedGlyphs = new Dictionary<char, FontGlyph>();
        System.Drawing.Font gdiFont;
        public GdiPathFont(GdiPathFontFace fontface, int emSizeInPoints)
        {
            this.fontface = fontface;
            this.emSizeInPoints = emSizeInPoints;
            //--------------------------------------
            emSizeInPixels = (int)(((float)emSizeInPoints / (float)POINTS_PER_INCH) * (float)PIXEL_PER_INCH);
            //currentEmScalling = (float)emSizeInPixels / (float)fontface.UnitsPerEm;

            //-----------------
            //implementation
            gdiFont = new System.Drawing.Font(fontface.FaceName, emSizeInPoints);
        }
        public override int GetAdvanceForCharacter(char c)
        {
            throw new NotImplementedException();
        }
        public override int GetAdvanceForCharacter(char c, char next_c)
        {
            throw new NotImplementedException();
        }
        public override double AscentInPixels
        {
            get { throw new NotImplementedException(); }
        }
        public override double CapHeightInPixels
        {
            get { throw new NotImplementedException(); }
        }
        public override double DescentInPixels
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
        public override int EmSizeInPixels
        {
            get { throw new NotImplementedException(); }
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
        public override double XHeightInPixels
        {
            get { throw new NotImplementedException(); }
        }
        protected override void OnDispose()
        {
        }
       

        public override FontInfo FontInfo
        {
            get
            {
                throw new NotImplementedException();
            }
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

        public override object InnerFont
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}