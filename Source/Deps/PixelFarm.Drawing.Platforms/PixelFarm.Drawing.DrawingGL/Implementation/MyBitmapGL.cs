// 2015,2014 ,Apache2, WinterDev

using System.Drawing;
namespace PixelFarm.Drawing.DrawingGL
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
        public System.IntPtr ToHFont()
        {
            return this.hFont;
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
        {
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