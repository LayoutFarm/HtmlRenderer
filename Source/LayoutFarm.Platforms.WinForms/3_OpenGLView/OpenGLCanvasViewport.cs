//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using LayoutFarm.Drawing; 

namespace LayoutFarm.UI.OpenGLView
{
    class OpenGLCanvasViewport : CanvasViewport
    {
        public OpenGLCanvasViewport(TopWindowRenderBox wintop,
            Size viewportSize, int cachedPageNum)
            : base(wintop, viewportSize, cachedPageNum)
        {
        }
    }

}