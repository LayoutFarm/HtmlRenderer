//2014,2015 BSD, WinterDev

namespace PixelFarm.Drawing.WinGdi
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