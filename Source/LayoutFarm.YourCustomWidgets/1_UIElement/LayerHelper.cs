// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;

using LayoutFarm.Text;
using LayoutFarm.UI;

namespace LayoutFarm.CustomWidgets
{   


    static class LayerHelpers
    {   
        public static void AddUI(this VisualPlainLayer plainLayer, UIElement ui)
        {
            plainLayer.AddChild(ui.GetPrimaryRenderElement(plainLayer.Root));
        }
    }

}