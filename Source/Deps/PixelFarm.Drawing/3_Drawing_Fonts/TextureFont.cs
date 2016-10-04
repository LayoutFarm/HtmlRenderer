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
        Dictionary<Font, TextureFont> registerFonts = new Dictionary<Font, TextureFont>();
        public void RegisterFont(Font f, TextureFont textureFont)
        {
            registerFonts.Add(f, textureFont);
        }
        public TextureFont GetResolvedFont(Font f)
        {
            TextureFont found;
            registerFonts.TryGetValue(f, out found);
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
            var font = new Font(fontName, fontSizeInPts);
            s_nativeFontStore.LoadFont(font, fontfile);
            nativeFont = s_nativeFontStore.GetResolvedNativeFont(font);
        }
        internal TextureFont(string fontName, float fontSizeInPts, SimpleFontAtlas fontAtlas)
        {
            //not support font 
            this.fontAtlas = fontAtlas;
            this.name = fontName;
            var font = new Font(fontName, fontSizeInPts);
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
        public override float EmSize
        {
            get
            {
                return nativeFont.EmSize;
            }
        }

        public override float EmSizeInPixels
        {
            get
            {
                return nativeFont.EmSizeInPixels;
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
        public static TextureFont CreateFont(string fontName, float fontSizeInPoints, string xmlFontInfo, string imgAtlas)
        {
            //for msdf font
            //1 font atlas may support mutliple font size 
            SimpleFontAtlasBuilder atlasBuilder = new SimpleFontAtlasBuilder();
            SimpleFontAtlas fontAtlas = atlasBuilder.LoadFontInfo(xmlFontInfo);
            //2. load glyph image
            using (Bitmap bmp = new Bitmap(imgAtlas))
            {
                var glyImage = new GlyphImage(bmp.Width, bmp.Height);
                var buffer = new int[bmp.Width * bmp.Height];
                System.Runtime.InteropServices.Marshal.Copy(bmp.GetNativeHImage(), buffer, 0, buffer.Length);
                glyImage.SetImageBuffer(buffer, true);
                fontAtlas.TotalGlyph = glyImage;
            }
            return new TextureFont(fontName, fontSizeInPoints, fontAtlas);
        }
    }
}