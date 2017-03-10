//MIT, 2017, WinterDev
using System;
using PixelFarm.Drawing;
using PixelFarm.Drawing.Fonts;

namespace YourImplementation
{
    public static class BootStrapWinGdi
    {
        public static readonly IFontLoader myFontLoader = new WindowsFontLoader();
    }
}