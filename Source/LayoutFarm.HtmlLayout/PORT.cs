//MIT, 2020, WinterDev
using System;
using System.Collections.Generic;
using PixelFarm.Drawing;


using Typography.OpenFont;
using Typography.OpenFont.Extensions;

using Typography.TextLayout;
using Typography.TextServices;
using Typography.FontManagement;
using Typography.TextBreak;


namespace LayoutFarm.HtmlBoxes
{

    static class CssBoxRenderExt
    {
        public static void InvalidateGraphics(this CssBox box) { }
        public static void InvalidateGraphics(this CssBox box, RectangleF r) { }
    }
}

 