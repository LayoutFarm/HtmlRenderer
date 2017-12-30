//BSD, 2014-2017, WinterDev

using PixelFarm.Drawing;
using PixelFarm.Drawing.Fonts;
namespace LayoutFarm.Composers
{

    //temp
    static class MyFontServices
    {

        static ITextService s_textService;
        public static PixelFarm.Drawing.ITextService GetTextService()
        {
            if (s_textService == null)
            {
                s_textService = PixelFarm.Drawing.WinGdi.WinGdiPlusPlatform.GetIFonts();
            }
            return s_textService;
        }

    }
}