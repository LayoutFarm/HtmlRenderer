//MIT 2014 ,WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;


namespace PixelFarm.Drawing
{
    
    public interface ITopWindowRenderBox
    {
        void DrawToThisPage(Canvas canvas, Rect r);
#if DEBUG
        void dbugShowRenderPart(Canvas canvas, Rect r);
#endif
    }

    public interface IVisualDrawingChain
    {
        void UpdateInvalidArea(Canvas targetCanvas,ITopWindowRenderBox rootbox);
    }
}