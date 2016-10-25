//MIT,2016, WinterDev
//----------------------------------- 
using System;
using System.Collections.Generic;
namespace PixelFarm.Drawing.Fonts
{
    /// <summary>
    /// cache texture font
    /// </summary>
    public class TextureFontStore
    {
        Dictionary<FontKey, TextureFont> registerFonts = new Dictionary<FontKey, TextureFont>();
        public void RegisterFont(RequestFont f, TextureFont textureFont)
        {
            registerFonts.Add(f.FontKey, textureFont);
        }
        public TextureFont GetResolvedFont(RequestFont f)
        {
            TextureFont found;
            registerFonts.TryGetValue(f.FontKey, out found);
            return found;
        }
    }

    public class TextureFont : ActualFont
    {
        SimpleFontAtlas fontAtlas;
        string name;
        IDisposable glBmp;

        NativeFont nativeFont;
        static NativeFontStore s_nativeFontStore = new NativeFontStore();

        internal TextureFont(string fontName, float fontSizeInPts, string fontfile, SimpleFontAtlas fontAtlas)
        {
            this.fontAtlas = fontAtlas;
            this.name = fontName;
            var font = new RequestFont(fontName, fontSizeInPts);
            s_nativeFontStore.LoadFont(font, fontfile);
            nativeFont = s_nativeFontStore.GetResolvedNativeFont(font);
        }
        internal TextureFont(string fontName, float fontSizeInPts, SimpleFontAtlas fontAtlas)
        {
            //not support font 
            this.fontAtlas = fontAtlas;
            this.name = fontName;
            var font = new RequestFont(fontName, fontSizeInPts);
            s_nativeFontStore.LoadFont(font);
            nativeFont = s_nativeFontStore.GetResolvedNativeFont(font);
        }
        public override float AscentInPixels
        {
            get
            {
                return nativeFont.AscentInPixels;
            }
        }
        public override float DescentInPixels
        {
            get
            {
                return nativeFont.DescentInPixels;
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
                return nativeFont.SizeInPoints;
            }
        }

        public override float SizeInPixels
        {
            get
            {
                return nativeFont.SizeInPixels;
            }
        }

        public override FontFace FontFace
        {
            get
            {
                return nativeFont.FontFace;
            }
        }
        public override float GetAdvanceForCharacter(char c)
        {
            return nativeFont.GetAdvanceForCharacter(c);
        }

        public override float GetAdvanceForCharacter(char c, char next_c)
        {
            return nativeFont.GetAdvanceForCharacter(c, next_c);
        }

        public override FontGlyph GetGlyph(char c)
        {
            return nativeFont.GetGlyph(c);
        }

        public override FontGlyph GetGlyphByIndex(uint glyphIndex)
        {
            return nativeFont.GetGlyphByIndex(glyphIndex);
        }

        public override void GetGlyphPos(char[] buffer, int start, int len, ProperGlyph[] properGlyphs)
        {
            nativeFont.GetGlyphPos(buffer, start, len, properGlyphs);
        }

        protected override void OnDispose()
        {
            if (glBmp != null)
            {
                glBmp.Dispose();
                glBmp = null;
            }
        }
        //----------------------------------------------------
        /// <summary>
        /// this method always create new TextureFont, 
        /// user should do caching by themself 
        /// </summary>
        /// <param name="fontName"></param>
        /// <param name="xmlFontInfo"></param>
        /// <param name="imgAtlas"></param>
        /// <returns></returns>
        public static TextureFont CreateFont(string fontName, float fontSizeInPoints, string xmlFontInfo, GlyphImage glyphImg)
        {
            //for msdf font
            //1 font atlas may support mutliple font size 
            SimpleFontAtlasBuilder atlasBuilder = new SimpleFontAtlasBuilder();
            SimpleFontAtlas fontAtlas = atlasBuilder.LoadFontInfo(xmlFontInfo);
            fontAtlas.TotalGlyph = glyphImg;


            return new TextureFont(fontName, fontSizeInPoints, fontAtlas);
        }
    }
}