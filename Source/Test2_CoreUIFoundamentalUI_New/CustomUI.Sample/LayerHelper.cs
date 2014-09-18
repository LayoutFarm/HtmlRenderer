//2014 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using LayoutFarm.Text;
using LayoutFarm.UI;

namespace LayoutFarm.SampleControls
{   


    static class LayerHelpers
    {   
        public static void AddUI(this VisualPlainLayer plainLayer, UIElement ui)
        {
            plainLayer.AddTop(ui.GetPrimaryRenderElement(plainLayer.Root));
        }
    }

}