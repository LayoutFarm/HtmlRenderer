//BSD, 2014-2016, WinterDev 

using System.Collections.Generic;
using PixelFarm.Drawing.Fonts;
namespace PixelFarm.Drawing.Skia
{
    public class SkiaGraphicsPlatform : GraphicsPlatform
    {
        static InstalledFontCollection s_installFontCollection = new InstalledFontCollection();
        static Dictionary<InstalledFont, SkiaSharp.SKTypeface> skTypeFaces = new Dictionary<InstalledFont, SkiaSharp.SKTypeface>();

        static SkiaGraphicsPlatform()
        {
            //TODO: review here again about font provider ***
            var installFontsWin32 = new PixelFarm.Drawing.Win32.InstallFontsProviderWin32();
            s_installFontCollection.LoadInstalledFont(installFontsWin32.GetInstalledFontIter());
        }
        public SkiaGraphicsPlatform()
        {
        }
        public override Canvas CreateCanvas(int left, int top, int width, int height, CanvasInitParameters canvasInitPars = new CanvasInitParameters())
        {
            return new MySkiaCanvas(0, 0, left, top, width, height);
        }
        public static void SetFontNotFoundHandler(FontNotFoundHandler fontNotFoundHandler)
        {
            s_installFontCollection.SetFontNotFoundHandler(fontNotFoundHandler);
        }
        internal static SkiaSharp.SKTypeface GetInstalledFont(string typefaceName)
        {

            InstalledFont installedFont = s_installFontCollection.GetFont(typefaceName, InstalledFontStyle.Regular);
            if (installedFont == null)
            {
                return null;
            }
            else
            {
                SkiaSharp.SKTypeface loadedTypeFace;
                if (!skTypeFaces.TryGetValue(installedFont, out loadedTypeFace))
                {
                    loadedTypeFace = SkiaSharp.SKTypeface.FromFile(installedFont.FontPath);
                    skTypeFaces.Add(installedFont, loadedTypeFace);
                }
                return loadedTypeFace;
            }
        }
    }



}