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
}