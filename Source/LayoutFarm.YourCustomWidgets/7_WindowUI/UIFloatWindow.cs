// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;
using LayoutFarm.UI;

namespace LayoutFarm.CustomWidgets
{
    public class UIFloatWindow : EaseBox
    {  
        public UIFloatWindow(int w, int h)
            : base(w, h)
        { 
        } 
        public override void Walk(UIVisitor visitor)
        {
            //TODO: implement this 
        }
        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {
            return base.GetPrimaryRenderElement(rootgfx);
        }
    }

}