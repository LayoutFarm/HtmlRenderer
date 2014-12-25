//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using LayoutFarm.Drawing;

namespace LayoutFarm.UI.OpenGLView
{
    class OpenGLCanvasViewport : CanvasViewport
    {
        Canvas canvas;
        public OpenGLCanvasViewport(TopWindowRenderBox wintop,
            Size viewportSize, int cachedPageNum)
            : base(wintop, viewportSize, cachedPageNum)
        {        }
        public void NotifyWindowControlBinding()
        {
            this.canvas = LayoutFarm.Drawing.DrawingGL.CanvasGLPortal.P.CreateCanvas(0, 0, this.ViewportWidth, this.ViewportHeight);
        }
        public void PaintMe()
        {

            //----------------------------------
            //gl paint here
            canvas.ClearSurface(Color.White);
            //test draw rect
            canvas.StrokeColor = LayoutFarm.Drawing.Color.Blue;
            canvas.DrawRectangle(Color.Blue, 20, 20, 200, 200);

            //------------------------
        }
    }

}