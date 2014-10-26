//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using LayoutFarm.Drawing;

namespace LayoutFarm.UI
{
    public abstract class UIPlatform
    {
        public abstract UITimer CreateUITimer();
        public abstract GraphicPlatform GraphicsPlatform { get; }      
        public TopWindowRenderBox CreateTopWindowRenderBox(RootGraphic rootgfx, int w, int h)
        {
            return new MyTopWindowRenderBox(rootgfx, w, h);
        }
    }
}