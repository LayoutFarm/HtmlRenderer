//2014 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using LayoutFarm.Text;
using LayoutFarm.UI;

namespace LayoutFarm
{
    [DemoNote("1.3 Grid")]
    class Demo_Grid : DemoBase
    {
        protected override void OnStartDemo(UISurfaceViewportControl viewport)
        {

            var gridBox = new LayoutFarm.SampleControls.UIGridBox(100, 100);
            gridBox.SetLocation(50, 50); 
            viewport.AddContent(gridBox);
            gridBox.InvalidateGraphic();
        }
    }
}