//BSD 2014-2015 ,WinterDev 
using System;
using System.Collections.Generic;

using System.Text;
using System.Diagnostics;
using PixelFarm.Drawing;
using LayoutFarm.WebDom;

namespace LayoutFarm.HtmlBoxes
{
    public abstract class CssBoxDecorator
    {
        
        public virtual void DrawTopDecoration(PaintVisitor p)
        {
        }
    }
}
