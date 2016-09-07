//Apache2, 2014-2016, WinterDev

using System;
namespace LayoutFarm.InternalUI
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