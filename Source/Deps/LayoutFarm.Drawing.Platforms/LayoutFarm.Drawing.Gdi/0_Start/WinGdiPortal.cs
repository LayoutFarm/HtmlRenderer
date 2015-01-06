//2014,2015 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using LayoutFarm.Drawing.WinGdi;

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
            WinGdiPortal.platform = new WinGdiPlatform();
            GraphicsPlatform.GenericSerifFontName = System.Drawing.FontFamily.GenericSerif.Name;
             
        }
        public static void End()
        {

        }
        public static GraphicsPlatform P
        {
            get { return platform; }
        }
    }
}