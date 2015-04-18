// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections.Generic;
using PixelFarm.Drawing;

namespace LayoutFarm.HtmlBoxes
{
    interface IScrollable
    {
        void SetViewport(int x, int y);
        int ViewportX { get; }
        int ViewportY { get; }
        int ViewportWidth { get; }
        int ViewportHeight { get; }
        int DesiredHeight { get; }
        int DesiredWidth { get; }
        event EventHandler LayoutFinished;
    }


}