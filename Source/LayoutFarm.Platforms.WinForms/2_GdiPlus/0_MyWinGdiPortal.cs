//Apache2, 2014-2016, WinterDev

using PixelFarm.Drawing;
using Win32;
using PixelFarm.Drawing.WinGdi;

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

            //text services:
            //
            TextServices.IFonts = new GdiPlusIFonts();
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

    class GdiPlusIFonts : LayoutFarm.IFonts
    {
        public float MeasureWhitespace(RequestFont f)
        {
            return WinGdiTextService.MeasureWhitespace(f);
        }
        public Size MeasureString(char[] buff, int startAt, int len, RequestFont font)
        {
            return WinGdiTextService.MeasureString(buff, startAt, len, font);
        }
        public Size MeasureString(char[] buff, int startAt, int len, RequestFont font, float maxWidth, out int charFit, out int charFitWidth)
        {
            throw new System.NotSupportedException();
        }
        public void Dispose()
        {


        }
    }

}