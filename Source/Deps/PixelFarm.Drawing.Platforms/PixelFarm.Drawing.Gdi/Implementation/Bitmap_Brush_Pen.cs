//MIT, 2014-2016, WinterDev   

using System;

using PixelFarm.Drawing.Fonts;
namespace PixelFarm.Drawing.WinGdi
{
    //*** this class need System.Drawing , because 
    class WinGdiPlusFont : PlatformFont
    {
        System.Drawing.Font myFont;
        System.IntPtr hFont;
        public WinGdiPlusFont(System.Drawing.Font f)
        {
            this.myFont = f;
            this.hFont = f.ToHfont();
        }
        public override string Name
        {
            get { return this.myFont.Name; }
        }
        public override int Height
        {
            get { return this.myFont.Height; }
        }
        public System.IntPtr ToHfont()
        {   /// <summary>
            /// Set a resource (e.g. a font) for the specified device context.
            /// WARNING: Calling Font.ToHfont() many times without releasing the font handle crashes the app.
            /// </summary>
            return this.hFont;
        }
        public override float EmSize
        {
            get { return this.myFont.Size; }
        }
        public override FontStyle Style
        {
            get
            {
                return (FontStyle)this.myFont.Style;
            }
        }
        protected override void OnDispose()
        {
            if (myFont != null)
            {
                myFont.Dispose();
                myFont = null;
            }
        }
        public override FontGlyph GetGlyphByIndex(uint glyphIndex)
        {
            throw new NotImplementedException();
        }

        public override FontGlyph GetGlyph(char c)
        {
            throw new NotImplementedException();
        }

        public override void GetGlyphPos(char[] buffer, int start, int len, ProperGlyph[] properGlyphs)
        {
            throw new NotImplementedException();
        }

        public override int GetAdvanceForCharacter(char c)
        {
            throw new NotImplementedException();
        }

        public override int GetAdvanceForCharacter(char c, char next_c)
        {
            throw new NotImplementedException();
        }

        public System.Drawing.Font InnerFont
        {
            get { return this.myFont; }
        }

        public override FontFace FontFace
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

        public override double AscentInPixels
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override double DescentInPixels
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

        public override double CapHeightInPixels
        {
            get
            {
                throw new NotImplementedException();
            }
        }

    }
}