//MIT, 2020, WinterDev
using System;
using PixelFarm.Drawing;

namespace LayoutFarm.HtmlBoxes
{

    static class CssBoxRenderExt
    {
        public static void InvalidateGraphics(this CssBox box) { }
        public static void InvalidateGraphics(this CssBox box, RectangleF r) { }
    }
}

