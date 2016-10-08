//BSD, 2014-2016, WinterDev

namespace PixelFarm.Drawing.WinGdi
{
    public static class WinGdiPortal
    {
        static bool isInit;
        static WinGdiPlusPlatform platform;
        public static void Start()
        {
            if (isInit)
            {
                return;
            }
            isInit = true;
            //we have only 1 WinGdiPlusPlatform per process
            WinGdiPortal.platform = new WinGdiPlusPlatform(); 
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