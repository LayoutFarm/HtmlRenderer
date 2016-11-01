//MIT,2016, WinterDev
//----------------------------------- 
using System;
using System.Collections.Generic;
namespace PixelFarm.Drawing.Fonts
{

    class TextureFontFace : FontFace
    {
        GlyphImage glyphImg;
        SimpleFontAtlasBuilder atlasBuilder;
        SimpleFontAtlas fontAtlas;
        string fontName;
        FontFace nOpenTypeFontFace;
        public TextureFontFace(string fontName, string xmlFontInfo, GlyphImage glyphImg)
        {
            //for msdf font
            //1 font atlas may support mutliple font size 
            atlasBuilder = new SimpleFontAtlasBuilder();
            fontAtlas = atlasBuilder.LoadFontInfo(xmlFontInfo);
            fontAtlas.TotalGlyph = glyphImg;

            //----------
            InstalledFont installedFont = GLES2PlatformFontMx.GetInstalledFont(fontName, InstalledFontStyle.Regular);
            nOpenTypeFontFace = NOpenTypeFontLoader.LoadFont(installedFont.FontPath,
                GLES2PlatformFontMx.defaultLang,
                GLES2PlatformFontMx.defaultHbDirection,
                GLES2PlatformFontMx.defaultScriptCode);
            //----------
        }
        public override string FontPath
        {
            get { return nOpenTypeFontFace.Name; }
        }
        
        public FontFace InnerFontFace
        {
            get { return nOpenTypeFontFace; }
        }
        public override ActualFont GetFontAtPointsSize(float pointSize)
        {
            TextureFont t = new TextureFont(this, pointSize);
            return t;
        }
        protected override void OnDispose()
        {

        }
        public override string Name
        {
            get { return fontName; }
        }
        public SimpleFontAtlas FontAtlas
        {
            get { return fontAtlas; }
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
            actualFont = typeface.InnerFontFace.GetFontAtPointsSize(sizeInPoints);

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
        public override float GetAdvanceForCharacter(char c)
        {
            return actualFont.GetAdvanceForCharacter(c);
        }

        public override float GetAdvanceForCharacter(char c, char next_c)
        {
            return actualFont.GetAdvanceForCharacter(c, next_c);
        }

        public override FontGlyph GetGlyph(char c)
        {
            return actualFont.GetGlyph(c);
        }

        public override FontGlyph GetGlyphByIndex(uint glyphIndex)
        {
            return actualFont.GetGlyphByIndex(glyphIndex);
        }

        //public override void GetGlyphPos(char[] buffer, int start, int len, ProperGlyph[] properGlyphs)
        //{
        //    nativeFont.GetGlyphPos(buffer, start, len, properGlyphs);
        //}

        protected override void OnDispose()
        {
            if (glBmp != null)
            {
                glBmp.Dispose();
                glBmp = null;
            }
        }

    }
}