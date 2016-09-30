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
        Font nativeFont;
        static NativeFontStore s_nativeFontStore = new NativeFontStore();
        internal TextureFont(string name, SimpleFontAtlas fontAtlas)
        {
            this.fontAtlas = fontAtlas;
            this.name = name;
            //string fontfile = @"D:\WImageTest\THSarabunNew\THSarabunNew.ttf";
            string fontfile = @"C:\Windows\Fonts\Tahoma.ttf";
            nativeFont = new Font("tahoma", 28);
            s_nativeFontStore.LoadFont(nativeFont, fontfile);
        }
        public override double AscentInPixels
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override double CapHeightInPixels
        {
            get
            {
                throw new NotImplementedException();
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
        public override double DescentInPixels
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

        public override int EmSizeInPixels
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override FontFace FontFace
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



        public override string Name
        {
            get
            {
                return this.name;
            }
        }

        public override FontStyle Style
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override double XHeightInPixels
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override int GetAdvanceForCharacter(char c)
        {
            throw new NotImplementedException();
        }

        public override int GetAdvanceForCharacter(char c, char next_c)
        {
            throw new NotImplementedException();
        }

        public override FontGlyph GetGlyph(char c)
        {
            throw new NotImplementedException();
        }

        public override FontGlyph GetGlyphByIndex(uint glyphIndex)
        {
            throw new NotImplementedException();
            //return nativeFont.ActualFont.GetGlyphByIndex(glyphIndex);
        }

        public override void GetGlyphPos(char[] buffer, int start, int len, ProperGlyph[] properGlyphs)
        {
            throw new NotImplementedException();
            //nativeFont.ActualFont.GetGlyphPos(buffer, start, len, properGlyphs);
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
        public static TextureFont CreateFont(string fontName, string xmlFontInfo, string imgAtlas)
        {
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
            return new TextureFont(fontName, fontAtlas);
        }
    }
}