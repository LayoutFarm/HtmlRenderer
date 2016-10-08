//2016 MIT, WinterDev

using PixelFarm.Agg;
namespace PixelFarm.DrawingGL
{
    public class GLCanvasPainter : GLCanvasPainterBase
    {
        WinGdiFontPrinter _win32GdiPrinter;
        public GLCanvasPainter(CanvasGL2d canvas, int w, int h)
            : base(canvas, w, h)
        {
            _win32GdiPrinter = new WinGdiFontPrinter(w, h);
        }
        public override void DrawString(string text, double x, double y)
        {
            //TODO: review here
            //for small font size we use gdi+
            _win32GdiPrinter.DrawString(_canvas, text, (float)x, (float)y);
            //for large font size we use msdf font
            //base.DrawString(text, x, y);
        }
    }
}