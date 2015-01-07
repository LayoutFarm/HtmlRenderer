// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;

namespace LayoutFarm.UI.GdiPlus
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
}