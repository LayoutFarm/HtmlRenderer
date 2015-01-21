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
            var renderE = ui.GetPrimaryRenderElement(plainLayer.Root);
            plainLayer.AddChild(renderE);
        }
        public static void RemoveUI(this PlainLayer plainLayer, UIElement ui)
        {
            var currentRenderElement = UIElement.GetCurrentPrimaryRenderElement(ui);
            if (currentRenderElement != null)
            {
                currentRenderElement.InvalidateGraphics();
                plainLayer.RemoveChild(currentRenderElement);
            }

        }
    }

}