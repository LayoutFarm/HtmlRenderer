//Apache2, 2014-2017, WinterDev
#if GL_ENABLE
using PixelFarm.Drawing;
namespace LayoutFarm.UI.OpenGL
{
    public static class MyOpenGLPortal
    {
        public static GraphicsPlatform Start()
        {

            return null;
            //PixelFarm.DrawingGL.CanvasOptions.DefaultOrientation = CanvasOrientation.LeftTop;
            //PixelFarm.Drawing.DrawingGL.CanvasGLPortal.Start();
            //return PixelFarm.Drawing.DrawingGL.CanvasGLPortal.P;
        }
        public static void End()
        {
            //PixelFarm.Drawing.DrawingGL.CanvasGLPortal.End();
        }
        public static GraphicsPlatform P
        {
            get
            {
                return null;
                  //PixelFarm.Drawing.DrawingGL.CanvasGLPortal.P;
            }
        }
    }

}
#endif