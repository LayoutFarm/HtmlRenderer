//MIT, 2014-2016, WinterDev 
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using NOpenType;
using System.IO;
using PixelFarm.Agg;
namespace PixelFarm.Drawing.Fonts
{
    class NOpenTypeFontFace : FontFace
    {
        Typeface ntypeface;
        string name, path;
        PixelFarm.Agg.GlyphPathBuilderVxs glyphPathBuilder;

        public NOpenTypeFontFace(Typeface ntypeface, string fontName, string fontPath)
        {
            this.ntypeface = ntypeface;
            this.name = fontName;
            this.path = fontPath;
            //----
            glyphPathBuilder = new Agg.GlyphPathBuilderVxs(ntypeface);
        }
        public override string Name
        {
            get { return name; }
        }
        public override string FontPath
        {
            get { return path; }
        }
        protected override void OnDispose() { }
        public override ActualFont GetFontAtPointsSize(float pointSize)
        {
            NOpenTypeActualFont actualFont = new NOpenTypeActualFont(this, pointSize, FontStyle.Regular);
            return actualFont;
        }
        public Typeface Typeface { get { return this.ntypeface; } }

        internal PixelFarm.Agg.GlyphPathBuilderVxs VxsBuilder
        {
            get { return this.glyphPathBuilder; }
        }
        public override float GetScale(float pointSize)
        {
            return ntypeface.CalculateScale(pointSize);
        }
        public override int AscentInDzUnit
        {
            get { return ntypeface.Ascender; }
        }
        public override int DescentInDzUnit
        {
            get { return ntypeface.Descender; }
        }
        public override int LineGapInDzUnit
        {
            get { return ntypeface.LineGap; }
        }

    }
    class NOpenTypeActualFont : ActualFont
    {
        NOpenTypeFontFace ownerFace;
        float sizeInPoints;
        FontStyle style;
        Typeface typeFace;
        float scale;
        Dictionary<uint, VertexStore> glyphVxs = new Dictionary<uint, VertexStore>();
        public NOpenTypeActualFont(NOpenTypeFontFace ownerFace, float sizeInPoints, FontStyle style)
        {
            this.ownerFace = ownerFace;
            this.sizeInPoints = sizeInPoints;
            this.style = style;
            this.typeFace = ownerFace.Typeface;
            //calculate scale *** 
            scale = typeFace.CalculateScale(sizeInPoints);
        }
        public override float SizeInPoints
        {
            get { return this.sizeInPoints; }
        }
        public override float SizeInPixels
        {
            //font height 
            get { return sizeInPoints * scale; }
        }
        public override float AscentInPixels
        {
            get { return typeFace.Ascender * scale; }
        }
        public override float DescentInPixels
        {
            get { return typeFace.Descender * scale; }
        }
        public override FontFace FontFace
        {
            get { return ownerFace; }
        }
        public override string FontName
        {
            get { return typeFace.Name; }
        }
        public override FontStyle FontStyle
        {
            get { return style; }
        }


        public override FontGlyph GetGlyph(char c)
        {
            return GetGlyphByIndex((uint)typeFace.LookupIndex(c));
        }
        public override FontGlyph GetGlyphByIndex(uint glyphIndex)
        {
            //1.  
            FontGlyph fontGlyph = new FontGlyph();
            fontGlyph.flattenVxs = GetGlyphVxs(glyphIndex);
            fontGlyph.horiz_adv_x = typeFace.GetAdvanceWidthFromGlyphIndex((int)glyphIndex);
            return fontGlyph;
        }
        protected override void OnDispose()
        {

        }
        public VertexStore GetGlyphVxs(uint codepoint)
        {
            VertexStore found;
            if (glyphVxs.TryGetValue(codepoint, out found))
            {
                return found;
            }
            //not found
            //then build it
            ownerFace.VxsBuilder.BuildFromGlyphIndex((ushort)codepoint, this.sizeInPoints);
            found = ownerFace.VxsBuilder.GetVxs();
            glyphVxs.Add(codepoint, found);
            return found;
        }
    }


}