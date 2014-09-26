//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using LayoutFarm.Drawing;

namespace LayoutFarm
{
    public static class WinGdiPlatform
    {
        public static void Start()
        {
            LayoutFarm.Drawing.WinGdiPlatform.WinGdi.Start();
        }
        public static void End()
        {
            LayoutFarm.Drawing.WinGdiPlatform.WinGdi.End();
        }
    }
}