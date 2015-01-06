//2014,2015 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using LayoutFarm.Drawing;

using LayoutFarm.UI;

namespace LayoutFarm
{

    public static class SampleViewportExtension
    {

        public static void AddContent(this SampleViewport viewport, UIElement ui)
        {
            viewport.ViewportControl.AddContent(ui.GetPrimaryRenderElement(viewport.ViewportControl.WinTopRootGfx));             
        }
    }

}