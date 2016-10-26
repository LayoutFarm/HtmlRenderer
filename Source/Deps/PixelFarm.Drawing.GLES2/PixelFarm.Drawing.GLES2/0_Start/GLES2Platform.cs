//BSD, 2014-2016, WinterDev

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
        FontSystem fontSystem = new FontSystem();
        public GLES2Platform()
        {
        }
        public override IFonts Fonts
        {
            get
            {
                return fontSystem;
            }
        }
        public override Canvas CreateCanvas(int left, int top, int width, int height, CanvasInitParameters reqPars = new CanvasInitParameters())
        {
            int max = Math.Max(width, height);
            CanvasGL2d canvas2d = new CanvasGL2d(max, max);
            MyGLCanvas myCanvas = new MyGLCanvas(this, canvas2d, 0, 0, width, height);
            return myCanvas;
        }

    }
}