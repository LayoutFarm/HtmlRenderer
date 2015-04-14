//MIT ,2015,WinterDev

using System;
using System.Collections.Generic;
using PixelFarm.Drawing;

namespace LayoutFarm.HtmlBoxes
{
    class CssScrollView : CssBox
    {
        
        public CssScrollView(object controller,
            Css.BoxSpec boxSpec,
            IRootGraphics rootgfx)
            : base(controller, boxSpec, rootgfx)
        {

        }
        public CssBox InnerBox
        {
            get;
            set;
        }
    }

}