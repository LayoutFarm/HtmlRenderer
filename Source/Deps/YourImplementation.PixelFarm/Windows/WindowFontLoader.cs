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

            SetFontNotFoundHandler(
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
        }
        public InstalledFont GetFont(string fontName, InstalledFontStyle style)
        {
            return installFontCollection.GetFont(fontName, style);
        }
        public void SetFontNotFoundHandler(FontNotFoundHandler fontNotFoundHandler)
        {
            installFontCollection.SetFontNotFoundHandler(fontNotFoundHandler);
        }
    }
}