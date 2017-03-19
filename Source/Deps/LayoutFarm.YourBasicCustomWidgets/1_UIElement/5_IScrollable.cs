//Apache2, 2014-2017, WinterDev

using System;
namespace LayoutFarm.UI
{
    public interface IScrollable
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