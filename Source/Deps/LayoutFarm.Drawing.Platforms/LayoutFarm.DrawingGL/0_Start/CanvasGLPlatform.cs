using System;

using LayoutFarm.Drawing.WinGdi;

namespace LayoutFarm.Drawing.DrawingGL
{

    class CanvasGLPlatform : GraphicsPlatform
    {

        System.Drawing.Bitmap sampleBmp;
        IFonts sampleIFonts;
        public CanvasGLPlatform()
        {
        }

        ~CanvasGLPlatform()
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
      
        public override FontInfo CreateNativeFontWrapper(object nativeFont)
        {
            return  FontsUtils.GetCachedFont((System.Drawing.Font)nativeFont);

        }
        public override Canvas CreateCanvas(int left, int top, int width, int height)
        {
            return new MyCanvasGL(this, 0, 0, left, top, width, height); 
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