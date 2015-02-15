// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections.Generic;
using PixelFarm.Drawing;

namespace LayoutFarm.UI
{

    public class UIContext
    {
        public UIContext(RootGraphic rootgfx)
        {
            this.Root = rootgfx;
        }
        public RootGraphic Root
        {
            get;
            private set;
        }    
    }
    

}