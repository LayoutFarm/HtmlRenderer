//BSD 2014-2015 ,WinterDev 
using System;
using System.Collections.Generic;

using System.Text;
using System.Diagnostics;
using PixelFarm.Drawing;
using LayoutFarm.WebDom;

namespace LayoutFarm.HtmlBoxes
{
    class CssBoxDecorator
    {

#if DEBUG
        static int totaldbugId = 0;
        public readonly int dbugId = totaldbugId++;

#endif
        public CssBoxDecorator()
        {
            
        }
        public Color Color { get; set; }
        public int HBoxShadowOffset { get; set; }
        public int VBoxShadowOffset { get; set; }
        public int BlurRadius { get; set; }
        public int SpreadDistance { get; set; }
        public void Paint(CssBox box, PaintVisitor p)
        {

            p.FillRectangle(this.Color,
                box.LocalX + this.HBoxShadowOffset,
                box.LocalY + this.VBoxShadowOffset,
                box.VisualWidth,
                box.VisualHeight);


        }
    }
}
