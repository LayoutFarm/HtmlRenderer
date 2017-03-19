//MIT, 2017, WinterDev
using PixelFarm.Drawing;
using PixelFarm.Drawing.Fonts; 
using System.IO; 
namespace YourImplementation
{

    public static class BootStrapSkia
    {
        public static readonly IFontLoader myFontLoader = WindowsFontLoader.Default;
        static string lib_SKIA = "libSkiaSharp.dll";
        static BootStrapSkia()
        {

        }
        public static bool IsNativeLibAvailable()
        {
            if (!File.Exists(lib_SKIA))
            {
                return false;
            }
            return true;
        }
    }

}