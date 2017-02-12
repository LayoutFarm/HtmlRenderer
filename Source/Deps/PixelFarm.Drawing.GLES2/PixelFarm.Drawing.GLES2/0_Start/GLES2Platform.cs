//BSD, 2014-2017, WinterDev

using PixelFarm.Drawing.Fonts;
using PixelFarm.DrawingGL;

namespace PixelFarm.Drawing.GLES2
{

    public class GLES2Platform : GraphicsPlatform
    {

        public GLES2Platform()
        {

        }

        public override Canvas CreateCanvas(int left,
            int top, int width, int height,
            CanvasInitParameters reqPars = new CanvasInitParameters())
        {
            return new MyGLCanvas(CreateCanvasGL2d(width, height), 0, 0, width, height);
        }
        public static Canvas CreateCanvas2(int left,
            int top, int width, int height,
            CanvasGL2d canvas,
            GLCanvasPainter painter1,
            CanvasInitParameters reqPars = new CanvasInitParameters())
        {
            return new MyGLCanvas(canvas, painter1, 0, 0, width, height);
        }
        public static void AddTextureFont(string fontName, string xmlGlyphPos, string glypBitmap)
        {
            GLES2PlatformFontMx.AddTextureFontInfo(fontName, xmlGlyphPos, glypBitmap);
        }

        public static CanvasGL2d CreateCanvasGL2d(int w, int h)
        {
            Init();
            return new CanvasGL2d(w, h);
        }
        static bool s_isInit;
        static void Init()
        {
            if (s_isInit)
            {
                return;
            }
            s_isInit = true;
            if (!GLES2PlatformFontMx.DidLoadFonts)
            {
                GLES2PlatformFontMx.LoadInstalledFont(
                    new PixelFarm.Drawing.Win32.InstallFontsProviderWin32());
            }

        }

    }
}