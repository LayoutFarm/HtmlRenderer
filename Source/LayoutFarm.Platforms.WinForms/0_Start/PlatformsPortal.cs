//2014,2015 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;

namespace PixelFarm.Drawing
{
    public static class MyWinGdiPortal
    {
        public static GraphicsPlatform Start()
        {
            PixelFarm.Drawing.WinGdi.WinGdiPortal.Start();
            return PixelFarm.Drawing.WinGdi.WinGdiPortal.P;
        }
        public static void End()
        {
            PixelFarm.Drawing.WinGdi.WinGdiPortal.End();
        }
        public static GraphicsPlatform P
        {
            get { return PixelFarm.Drawing.WinGdi.WinGdiPortal.P; }
        }
    }
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