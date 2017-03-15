//BSD, 2014-2017, WinterDev

using PixelFarm.Drawing.Fonts;
using PixelFarm.DrawingGL;

namespace PixelFarm.Drawing.GLES2
{

    public static class GLES2Platform
    {

        public static void AddTextureFont(string fontName, string xmlGlyphPos, string glypBitmap)
        {
            GLES2PlatformFontMx.AddTextureFontInfo(fontName, xmlGlyphPos, glypBitmap);
        }

        public static CanvasGL2d CreateCanvasGL2d(int w, int h)
        {
            return new CanvasGL2d(w, h);
        }

        public static void SetFontLoader(IFontLoader fontLoader)
        {
            GLES2PlatformFontMx.SetFontLoader(fontLoader);
        }
        public static InstalledFont GetInstalledFont(string fontName, InstalledFontStyle style)
        {
            return GLES2PlatformFontMx.GetInstalledFont(fontName, style);
        }
    }
}