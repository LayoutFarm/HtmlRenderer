// 2015,2014 ,Apache2, WinterDev
using System;
using System.Text; 

namespace PixelFarm.Drawing.DrawingGL
{
    public static class CanvasGLPortal
    {
        //static bool isInit;

        static CanvasGLPlatform platform;
        static CanvasGLPortal()
        {
            CanvasGLPortal.platform = new CanvasGLPlatform();
            GraphicsPlatform.GenericSerifFontName = System.Drawing.FontFamily.GenericSerif.Name;
            //lock (syncLock)
            //{
            //    if (isInit)
            //    {
            //        return;
            //    }
            //    isInit = true;
            //    CanvasGLPortal.platform = new CanvasGLPlatform();
            //    GraphicsPlatform.GenericSerifFontName = System.Drawing.FontFamily.GenericSerif.Name;

            //}
        }
        public static void Start()
        {

        }
        public static void End()
        {

        }
        public static GraphicsPlatform P
        {
            get { return platform; }
        }
        public static FontInfo GetFontInfo(string fontname, float emsize, FontLoadTechnique fontLoadTechnique)
        {
            return CanvasGLPlatform.PlatformGetFont(fontname, emsize, fontLoadTechnique);
            
        }
    }
    public enum FontLoadTechnique
    {
        NativeFreeType,
        GdiBitmapFont
    }

}