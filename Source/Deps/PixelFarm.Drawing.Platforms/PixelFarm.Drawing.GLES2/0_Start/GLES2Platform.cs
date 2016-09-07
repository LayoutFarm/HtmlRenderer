//BSD, 2014-2016, WinterDev

using System;
using PixelFarm.DrawingGL;
namespace PixelFarm.Drawing.GLES2
{
    class GLES2Platform : GraphicsPlatform
    {
        IFonts sampleIFonts;
        System.Drawing.Bitmap sampleBmp;
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
                    //System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(sampleBmp);
                    //TODO: review here, we need some platform-specific func
                    sampleIFonts = new PixelFarm.Drawing.WinGdi.MyScreenCanvas(this, 0, 0, 0, 0, 2, 2);
                }
                return this.sampleIFonts;
            }
        }

        public override Canvas CreateCanvas(int left, int top, int width, int height)
        {
            int max = Math.Max(width, height);
            System.Drawing.Bitmap backBmp = new System.Drawing.Bitmap(width, height);
            System.Drawing.Graphics gfx = System.Drawing.Graphics.FromImage(backBmp);
            CanvasGL2d canvas2d = new CanvasGL2d(max, max);
            MyGLCanvas myCanvas = new MyGLCanvas(this, canvas2d, gfx, 0, 0, width, height);
            return myCanvas;
        }
        public override Canvas CreateCanvas(object platformCanvas, int left, int top, int width, int height)
        {
            //create 
            int max = Math.Max(width, height);
            System.Drawing.Bitmap backBmp = new System.Drawing.Bitmap(width, height);
            System.Drawing.Graphics gfx = (System.Drawing.Graphics)platformCanvas;
            CanvasGL2d canvas2d = new CanvasGL2d(max, max);
            MyGLCanvas myCanvas = new MyGLCanvas(this, canvas2d, gfx, 0, 0, width, height);
            return myCanvas;
        }

        public override GraphicsPath CreateGraphicsPath()
        {
            throw new NotImplementedException();
        }

        public override Bitmap CreatePlatformBitmap(int w, int h, byte[] rawBuffer, bool isBottomUp)
        {
            throw new NotImplementedException();
        }

        public override PixelFarm.Drawing.Fonts.FontInfo GetFont(string fontfaceName, float emsize, FontStyle st)
        {
            throw new NotImplementedException();
        }
    }
}