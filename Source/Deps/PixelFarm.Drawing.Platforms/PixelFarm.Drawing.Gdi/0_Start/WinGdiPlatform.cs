//2014,2015 BSD, WinterDev
using System;
namespace PixelFarm.Drawing.WinGdi
{
    class WinGdiPlatform : GraphicsPlatform
    {
        static FontStore fontStore = new FontStore();
        System.Drawing.Bitmap sampleBmp;
        IFonts sampleIFonts;
        public WinGdiPlatform()
        {
        }

        ~WinGdiPlatform()
        {
            if (sampleBmp != null)
            {
                sampleBmp.Dispose();
                sampleBmp = null;
            }
            if (sampleIFonts != null)
            {
                sampleIFonts.Dispose();
                sampleIFonts = null;
            }
        }

        public override GraphicsPath CreateGraphicsPath()
        {
            return new MyGraphicsPath();
        }
        public override FontInfo GetFont(string fontfaceName, float emsize, FontStyle fontStyle)
        {
            //System.Drawing.Font nativeFont = new System.Drawing.Font(fontfaceName, emsize,fonts);
            return fontStore.GetCachedFont(fontfaceName, emsize, (System.Drawing.FontStyle)fontStyle);
        }

        public override Canvas CreateCanvas(int left, int top, int width, int height)
        {
            return new MyCanvas(this, 0, 0, left, top, width, height);
        }
        public override Canvas CreateCanvas(object platformCanvas, int left, int top, int width, int height)
        {
            var g = platformCanvas as System.Drawing.Graphics;
            return new MyPrintingCanvas(this, g, left, top, width, height);
        }
        public override IFonts SampleIFonts
        {
            get
            {
                if (sampleIFonts == null)
                {
                    if (sampleBmp == null)
                    {
                        sampleBmp = new System.Drawing.Bitmap(2, 2);
                    }

                    System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(sampleBmp);
                    sampleIFonts = new MyCanvas(this, 0, 0, 0, 0, 2, 2);
                }
                return this.sampleIFonts;
            }
        }
    }
}