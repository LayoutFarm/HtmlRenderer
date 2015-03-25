// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;

using LayoutFarm.Text;
using LayoutFarm.UI;
using LayoutFarm.RenderBoxes;

namespace LayoutFarm.CustomWidgets
{

    public static class RenderElementExtension
    {
        public static void AddChild(this RenderBoxBase renderBox, UIElement ui)
        {
            renderBox.AddChild(ui.GetPrimaryRenderElement(renderBox.Root));
        }
    }
}