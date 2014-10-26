//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using LayoutFarm.Drawing;

namespace LayoutFarm.Drawing
{
    public static class WinGdiPortal
    {
        static bool isInit;
        static WinGdiPlatform platform;
        public static void Start()
        {

            if (isInit)
            {
                return;
            }
            isInit = true; 
            CurrentGraphicPlatform.SetCurrentPlatform(WinGdiPortal.platform = new WinGdiPlatform());
            CurrentGraphicPlatform.GenericSerifFontName = System.Drawing.FontFamily.GenericSerif.Name;

        }
        public static void End()
        {

        }
        public static GraphicPlatform P
        {
            get { return platform; }
        }
    }
}