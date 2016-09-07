//MIT,2016, WinterDev
//----------------------------------- 
using System;
using PixelFarm.DrawingGL;
namespace PixelFarm.Drawing.Fonts
{

    public static class TextureFontBuilder
    {
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
    public class TextureFont : Font
    {
        SimpleFontAtlas fontAtlas;
        string name;
        GLBitmap glBmp;
        internal TextureFont(string name, SimpleFontAtlas fontAtlas)
        {
            this.fontAtlas = fontAtlas;
            this.name = name;
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

        internal GLBitmap GLBmp
        {
            get { return glBmp; }
            set { glBmp = value; }
        }
        internal SimpleFontAtlas FontAtlas
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

        public override FontInfo FontInfo
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

        public override object InnerFont
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
        }

        public override void GetGlyphPos(char[] buffer, int start, int len, ProperGlyph[] properGlyphs)
        {
            throw new NotImplementedException();
        }

        protected override void OnDispose()
        {
            if(glBmp != null )
            {
                glBmp.Dispose();
                glBmp = null;
            }
        }
    }
}