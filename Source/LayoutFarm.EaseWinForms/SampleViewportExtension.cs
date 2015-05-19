// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;

using LayoutFarm.UI;

namespace LayoutFarm
{

    static class SampleViewportExtension
    {

        public static void AddContent(this SampleViewport viewport, UIElement ui)
        {
            viewport.ViewportControl.AddContent(
                ui.GetPrimaryRenderElement(viewport.ViewportControl.RootGfx),
                ui);
        }
    }

}