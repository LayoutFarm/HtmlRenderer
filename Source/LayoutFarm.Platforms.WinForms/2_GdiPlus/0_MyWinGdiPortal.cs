//Apache2, 2014-2016, WinterDev

using PixelFarm.Drawing;
namespace LayoutFarm.UI.GdiPlus
{
    public static class MyWinGdiPortal
    {
        static PixelFarm.Drawing.WinGdi.WinGdiPlusPlatform _winGdiPlatform;
        
        static bool isInit;
        public static GraphicsPlatform Start()
        {
            if (isInit)
            {
                return _winGdiPlatform;
            }
            isInit = true;
            return _winGdiPlatform = new PixelFarm.Drawing.WinGdi.WinGdiPlusPlatform();

        }
        public static void End()
        {

        }
        public static GraphicsPlatform P
        {
            get { return _winGdiPlatform; }
        }
    }
}