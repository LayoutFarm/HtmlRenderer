//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using LayoutFarm.Drawing;

namespace LayoutFarm.Drawing
{
    public static class WinGdiPortal
    {
        public static void Start()
        {
            WinGdi.Start();
        }
        public static void End()
        {
            WinGdi.End();
        }
        public static GraphicPlatform P
        {
            get { return WinGdi.P; }
        }
    }
}