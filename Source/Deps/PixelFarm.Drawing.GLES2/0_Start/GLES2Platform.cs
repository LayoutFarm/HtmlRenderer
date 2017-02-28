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

            var painter1 = new GLCanvasPainter(CreateCanvasGL2d(width, height), width, height);
            return new MyGLCanvas(
                painter1,
                0, 0, width, height);
        }
        public static Canvas CreateCanvas2(int left,
            int top, int width, int height,
            CanvasGL2d canvas,
            GLCanvasPainter painter1,
            CanvasInitParameters reqPars = new CanvasInitParameters())
        {
            return new MyGLCanvas(painter1, 0, 0, width, height);
        }
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