//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using LayoutFarm.Drawing;

namespace LayoutFarm.Drawing
{
    public static class MyWinGdiPortal
    {
        public static GraphicsPlatform Start()
        {
            WinGdiPortal.Start();
            return WinGdiPortal.P;
        }
        public static void End()
        {
            WinGdiPortal.End();
        }
        public static GraphicsPlatform P
        {
            get { return WinGdiPortal.P; }
        }
    }
    public static class MyOpenGLPortal
    {
        public static GraphicsPlatform Start()
        {

            LayoutFarm.DrawingGL.CanvasOptions.DefaultOrientation = CanvasOrientation.LeftTop;

            LayoutFarm.Drawing.DrawingGL.CanvasGLPortal.Start();
            return LayoutFarm.Drawing.DrawingGL.CanvasGLPortal.P;
        }
        public static void End()
        {
            LayoutFarm.Drawing.DrawingGL.CanvasGLPortal.End();
        }
        public static GraphicsPlatform P
        {
            get { return LayoutFarm.Drawing.DrawingGL.CanvasGLPortal.P; }
        }
    }

}