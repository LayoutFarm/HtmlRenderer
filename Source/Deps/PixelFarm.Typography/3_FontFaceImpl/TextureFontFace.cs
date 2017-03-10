//MIT, 2016-2017, WinterDev
//----------------------------------- 
using System;
namespace PixelFarm.Drawing.Fonts
{

    class TextureFontFace : FontFace
    {


        SimpleFontAtlas fontAtlas;
        FontFace primFontFace;

        public TextureFontFace(FontFace primFontFace, SimpleFontAtlas fontAtlas)
        {
            //for msdf font
            //1 font atlas may support mutliple font size  
            this.fontAtlas = fontAtlas;
            this.primFontFace = primFontFace;
        }

        public override float GetScale(float pointSize)
        {
            return primFontFace.GetScale(pointSize);
        }
        public override string FontPath
        {
            get { return primFontFace.Name; }
        }
        public override int AscentInDzUnit
        {
            get { return primFontFace.AscentInDzUnit; }
        }
        public override int DescentInDzUnit
        {
            get { return primFontFace.DescentInDzUnit; }
        }
        public override int LineGapInDzUnit
        {
            get { return primFontFace.LineGapInDzUnit; }
        }
        public FontFace InnerFontFace
        {
            get { return primFontFace; }
        }
        public override ActualFont GetFontAtPointSize(float pointSize)
        {
            TextureFont t = new TextureFont(this, pointSize);
            return t;
        }
        protected override void OnDispose()
        {

        }
        public override string Name
        {
            get { return primFontFace.Name; }
        }
        public SimpleFontAtlas FontAtlas
        {
            get { return fontAtlas; }
        }
        public override object GetInternalTypeface()
        {
            return primFontFace.GetInternalTypeface();
        }

    }
    class TextureFont : ActualFont
    {
        SimpleFontAtlas fontAtlas;
        IDisposable glBmp;
        ActualFont actualFont;
        TextureFontFace typeface;

        internal TextureFont(TextureFontFace typeface, float sizeInPoints)
        {
            this.typeface = typeface;
            this.fontAtlas = typeface.FontAtlas;
            actualFont = typeface.InnerFontFace.GetFontAtPointSize(sizeInPoints);

        }
        public override string FontName
        {
            get { return typeface.Name; }
        }
        public override FontStyle FontStyle
        {
            get { return Drawing.FontStyle.Regular; }
        }
        public override float AscentInPixels
        {
            get
            {
                return actualFont.AscentInPixels;
            }
        }
        public override float DescentInPixels
        {
            get
            {
                return actualFont.DescentInPixels;
            }
        }
        public IDisposable GLBmp
        {
            get { return glBmp; }
            set { glBmp = value; }
        }
        public SimpleFontAtlas FontAtlas
        {
            get { return fontAtlas; }
        }
        public override float SizeInPoints
        {
            get
            {
                return actualFont.SizeInPoints;
            }
        }

        public override float SizeInPixels
        {
            get
            {
                return actualFont.SizeInPixels;
            }
        }

        public override FontFace FontFace
        {
            get
            {
                return actualFont.FontFace;
            }
        }
        public override float LineGapInPixels
        {
            get { return actualFont.LineGapInPixels; }
        }
        public override float RecommendedLineSpacingInPixels
        {
            get { return actualFont.RecommendedLineSpacingInPixels; }
        }


        public override FontGlyph GetGlyph(char c)
        {
            return actualFont.GetGlyph(c);
        }

        public override FontGlyph GetGlyphByIndex(uint glyphIndex)
        {
            return actualFont.GetGlyphByIndex(glyphIndex);
        }
        protected override void OnDispose()
        {
            if (glBmp != null)
            {
                glBmp.Dispose();
                glBmp = null;
            }
        }

        public void AssignToRequestFont(RequestFont r)
        {
            SetCacheActualFont(r, this);
        }
        public static TextureFont GetCacheFontAsTextureFont(RequestFont r)
        {
            return GetCacheActualFont(r) as TextureFont;
        }
    }
}