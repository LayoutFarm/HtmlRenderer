// 2015,2014 ,MIT, WinterDev   

namespace PixelFarm.Drawing.WinGdi
{
    class MyFont : Font
    {
        System.Drawing.Font myFont;
        System.IntPtr hFont;
        FontInfo fontInfo;
        public MyFont(System.Drawing.Font f)
        {
            this.myFont = f;
            this.hFont = f.ToHfont();
        }
        public override FontInfo FontInfo
        {
            get { return this.fontInfo; }
        }
        public void SetFontInfo(FontInfo fontInfo)
        {
            this.fontInfo = fontInfo;
        }

        public override string Name
        {
            get { return this.myFont.Name; }
        }
        public override int Height
        {
            get { return this.myFont.Height; }
        }
        public override System.IntPtr ToHfont()
        {   /// <summary>
            /// Set a resource (e.g. a font) for the specified device context.
            /// WARNING: Calling Font.ToHfont() many times without releasing the font handle crashes the app.
            /// </summary>
            return this.hFont;
        }
        public override float Size
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
        public override void Dispose()
        {
            if (myFont != null)
            {
                myFont.Dispose();
                myFont = null;
            }
        }

        public override object InnerFont
        {
            get { return this.myFont; }
        }
    }
}