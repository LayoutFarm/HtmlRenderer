//MIT, 2017, WinterDev

using PixelFarm.Drawing;
using PixelFarm.Drawing.Fonts;

namespace YourImplementation
{
    public static class BootStrapWinGdi
    {
        public static readonly IFontLoader myFontLoader = WindowsFontLoader.Default;
    }

}