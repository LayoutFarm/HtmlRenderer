// 2015,2014 ,Apache2, WinterDev

using PixelFarm.Drawing; 

namespace LayoutFarm.UI.GdiPlus
{
    public static class MyWinGdiPortal
    {
        public static GraphicsPlatform Start()
        {
            PixelFarm.Drawing.WinGdi.WinGdiPortal.Start();
            var platform = PixelFarm.Drawing.WinGdi.WinGdiPortal.P;
            platform.TextEditFontInfo = platform.GetFont("tahoma", 10, PixelFarm.Drawing.FontStyle.Regular);
            return platform;
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