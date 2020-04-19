//MIT, 2020, WinterDev
using System;
using System.Collections.Generic;
using PixelFarm.Drawing; 

namespace LayoutFarm.Css
{
    static class FontDefaultConfig
    {

        internal static string DEFAULT_FONT_NAME = "Tahoma";
        /// <summary>
        /// Default font size in points. Change this value to modify the default font size.
        /// </summary>
        public const float DEFAULT_FONT_SIZE = 10f;
    }

}

namespace LayoutFarm.HtmlBoxes
{
    partial class CssBox
    {
        public void InvalidateGraphics() { }
        public void InvalidateGraphics(RectangleF r) { }
    }

}