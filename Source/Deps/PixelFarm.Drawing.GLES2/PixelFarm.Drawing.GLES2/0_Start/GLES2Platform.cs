//BSD, 2014-2017, WinterDev

using System;
using System.IO;
using System.Collections.Generic;
using PixelFarm.Drawing.Fonts;
using PixelFarm.DrawingGL;

using System.Text;
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