// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;


using LayoutFarm.UI;
using LayoutFarm.RenderBoxes;
namespace LayoutFarm.CustomWidgets
{


    static class LayerHelpers
    {
        public static void AddUI(this PlainLayer plainLayer, UIElement ui)
        {
            plainLayer.AddChild(ui.GetPrimaryRenderElement(plainLayer.Root));
        }
        public static void RemoveUI(this PlainLayer plainLayer, UIElement ui)
        {
            var currentRenderElement = UIElement.GetCurrentPrimaryRenderElement(ui);
            if (currentRenderElement != null)
            {
                plainLayer.AddChild(currentRenderElement);
            }

        }
    }

}