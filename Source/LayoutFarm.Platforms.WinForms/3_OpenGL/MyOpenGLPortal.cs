// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;
namespace LayoutFarm.UI.OpenGL
{
    public static class MyOpenGLPortal
    {
        public static GraphicsPlatform Start()
        {

            PixelFarm.DrawingGL.CanvasOptions.DefaultOrientation = CanvasOrientation.LeftTop;

            PixelFarm.Drawing.DrawingGL.CanvasGLPortal.Start();
            return PixelFarm.Drawing.DrawingGL.CanvasGLPortal.P;
        }
        public static void End()
        {
            PixelFarm.Drawing.DrawingGL.CanvasGLPortal.End();
        }
        public static GraphicsPlatform P
        {
            get { return PixelFarm.Drawing.DrawingGL.CanvasGLPortal.P; }
        }
    }

}