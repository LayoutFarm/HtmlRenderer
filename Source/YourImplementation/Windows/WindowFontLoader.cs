//MIT, 2014-2017, WinterDev   
 
using PixelFarm.Drawing.Fonts;

namespace PixelFarm.Drawing
{
    class WindowsFontLoader : IFontLoader
    {
        InstalledFontCollection installFontCollection = new InstalledFontCollection();
        public WindowsFontLoader()
        {
            //iterate
            var installFontsWin32 = new InstallFontsProviderWin32();
            installFontCollection.LoadInstalledFont(installFontsWin32.GetInstalledFontIter());
        }
        public InstalledFont GetFont(string fontName, InstalledFontStyle style)
        {
            return installFontCollection.GetFont(fontName, style);
        }
        public void SetFontHotFoundHandler(FontNotFoundHandler fontNotFoundHandler)
        {
            installFontCollection.SetFontNotFoundHandler(fontNotFoundHandler);
        }
    }
}