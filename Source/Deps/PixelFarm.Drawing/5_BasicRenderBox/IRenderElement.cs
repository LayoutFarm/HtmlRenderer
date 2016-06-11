// 2015,2014 ,MIT, WinterDev

using System;
namespace PixelFarm.Drawing
{
    public interface IRenderElement
    {
        void DrawToThisCanvas(Canvas canvas, Rectangle updateArea);
#if DEBUG
        void dbugShowRenderPart(Canvas canvas, Rectangle r);
#endif
    }

    public interface IRootGraphics
    {
    }
}