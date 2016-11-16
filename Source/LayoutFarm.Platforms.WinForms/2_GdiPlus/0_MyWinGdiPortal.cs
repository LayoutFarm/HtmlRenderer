//Apache2, 2014-2016, WinterDev

using PixelFarm.Drawing;
using Win32;
using PixelFarm.Drawing.WinGdi;
using PixelFarm.Drawing.Fonts;
using PixelFarm.Drawing.Text;
namespace LayoutFarm.UI.GdiPlus
{
    public class MyWinGdiPortalSetupParameters
    {
        public string IcuDataFile { get; set; }
        public RootGraphic.TextBreakGenDel TextBreakGenerator { get; set; }
        public IFonts TextServiceInstance { get; set; }
        public IActualFontResolver ActualFontResolver { get; set; }
    }


    public static class MyWinGdiPortal
    {
        static PixelFarm.Drawing.WinGdi.WinGdiPlusPlatform _winGdiPlatform;
        static bool isInit;


        public static GraphicsPlatform Start(MyWinGdiPortalSetupParameters initParams)
        {
            if (isInit)
            {
                return _winGdiPlatform;
            }
            isInit = true;

            //text services:            
            TextServices.IFonts = initParams.TextServiceInstance ?? new GdiPlusIFonts();
            ActualFontResolver.Resolver = initParams.ActualFontResolver ?? new GdiFontResolver();
            //set if we use pixelfarm's native myft.dll
            //or use managed text break
            //-------------------------------------
            //if we use ICU text breaker
            //1. load icu data
            if (initParams.IcuDataFile != null)
            {
                //check icu file is exist 
                //TODO: review  file/resource load mechanism again ***
                //NativeTextBreaker.SetICUDataFile(initParams.IcuDataFile);
            }
            //2. text breaker
            //RootGraphic.SetTextBreakerGenerator(
            //    initParams.TextBreakGenerator ??
            //    (locale => new NativeTextBreaker(TextBreakKind.Word, locale))
            //    );

            //-------------------------------------
            //config encoding
            WinGdiPlusPlatform.SetFontEncoding(System.Text.Encoding.GetEncoding(874));
            //-------------------------------------
            WinGdiPlusPlatform.SetFontNotFoundHandler(
                (fontCollection, fontName, style) =>
                {
                    //TODO: implement font not found mapping here
                    //_fontsMapping["monospace"] = "Courier New";
                    //_fontsMapping["Helvetica"] = "Arial";
                    fontName = fontName.ToUpper();
                    switch (fontName)
                    {
                        case "MONOSPACE":
                            return fontCollection.GetFont("Courier New", style);
                        case "HELVETICA":
                            return fontCollection.GetFont("Arial", style);
                        case "TAHOMA":
                            //default font must found
                            //if not throw err 
                            //this prevent infinit loop
                            throw new System.NotSupportedException();
                        default:
                            return fontCollection.GetFont("tahoma", style);
                    }

                });
            _winGdiPlatform = new PixelFarm.Drawing.WinGdi.WinGdiPlusPlatform();

            return _winGdiPlatform;
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
        public Size MeasureString(char[] buff, int startAt, int len, RequestFont font,
            float maxWidth,
            out int charFit,
            out int charFitWidth)
        {
            return WinGdiTextService.MeasureString(buff, startAt, len, font, maxWidth, out charFit, out charFitWidth);
        }
        public void CalculateGlyphAdvancePos(char[] str, int startAt, int len, RequestFont font, int[] glyphXAdvances)
        {
            WinGdiTextService.CalculateGlyphAdvancePos(str, startAt, len, font, glyphXAdvances);
        }
        public void Dispose()
        {
        }
    }
    class GdiFontResolver : LayoutFarm.IActualFontResolver
    {
        public PixelFarm.Drawing.Fonts.ActualFont Resolve(RequestFont font)
        {
            return WinGdiTextService.GetWinGdiFont(font);
        }
    }

}