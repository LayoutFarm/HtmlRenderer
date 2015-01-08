// 2015,2014 ,MIT, WinterDev
using System; 

namespace PixelFarm.Drawing
{
    
    public interface ITopWindowRenderBox
    {
        void DrawToThisPage(Canvas canvas, Rectangle r);
#if DEBUG
        void dbugShowRenderPart(Canvas canvas, Rectangle r);
#endif
    }

    public interface IVisualDrawingChain
    {
        void UpdateInvalidArea(Canvas targetCanvas, ITopWindowRenderBox rootbox);
    }
}