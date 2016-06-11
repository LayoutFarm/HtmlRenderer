// 2015,2014 ,MIT, WinterDev

using System;
//TODO: revise PixelFarm.Drawing.WinGdi need here

namespace PixelFarm.Drawing.DrawingGL
{
    class CanvasGLPlatform : GraphicsPlatform
    {
        //font store is platform specific
        static PixelFarm.Drawing.WinGdi.FontStore fontStore = new PixelFarm.Drawing.WinGdi.FontStore();
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
            return new PixelFarm.Drawing.WinGdi.MyGraphicsPath();
        }

        public override FontInfo GetFont(string fontFaceName, float emsize, FontStyle fontstyle)
        {
            return PlatformGetFont(fontFaceName, emsize, FontLoadTechnique.GdiBitmapFont);
        }
        internal static FontInfo PlatformGetFont(string fontFaceName, float emsize, FontLoadTechnique fontLoadTechnique)
        {
            //create gdi font 
            System.Drawing.Font f = new System.Drawing.Font(fontFaceName, emsize);
            FontInfo fontInfo = fontStore.GetCachedFont(f);
            if (fontInfo.PlatformSpecificFont == null)
            {
                switch (fontLoadTechnique)
                {
                    case FontLoadTechnique.GdiBitmapFont:
                        {
                            //use gdi font board  

                            fontInfo.PlatformSpecificFont = new PixelFarm.Agg.Fonts.GdiTextureFont(800, 200, f, fontInfo);
                        }
                        break;
                    default:
                        {
                            fontInfo.PlatformSpecificFont = PixelFarm.Agg.Fonts.NativeFontStore.LoadFont(
                                "c:\\Windows\\Fonts\\" + fontFaceName + ".ttf", //sample only***
                                (int)emsize);
                        }
                        break;
                }
            }
            return fontInfo;
        }
        public override Canvas CreateCanvas(int left, int top, int width, int height)
        {
            return new MyCanvasGL(this, 0, 0, left, top, width, height);
        }
        public override Canvas CreateCanvas(object platformCanvas, int left, int top, int width, int height)
        {
            //current version is  not support printing
            throw new NotImplementedException();
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
                    sampleIFonts = new PixelFarm.Drawing.WinGdi.MyScreenCanvas(this, 0, 0, 0, 0, 2, 2);
                }
                return this.sampleIFonts;
            }
        }
        public override Bitmap CreatePlatformBitmap(int w, int h, byte[] rawBuffer, bool isBottomUp)
        {
            throw new NotImplementedException();
        }
    }
}