//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using LayoutFarm.Drawing;

namespace LayoutFarm.Drawing
{
    public static class MyWinGdiPortal
    {
        

        public static void Start()
        {
            WinGdiPortal.Start();
        }
        public static void End()
        {
            WinGdiPortal.End();
        }
        public static GraphicPlatform P
        {
            get { return WinGdiPortal.P; }
        }
    }
}