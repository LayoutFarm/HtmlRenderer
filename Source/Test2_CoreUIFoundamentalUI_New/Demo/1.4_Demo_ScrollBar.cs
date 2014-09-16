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
    [DemoNote("1.4 ScrollBar")]
    class Demo_ScrollBar : DemoBase
    {
        protected override void OnStartDemo(UISurfaceViewportControl viewport)
        {

            var scbar = new LayoutFarm.SampleControls.UIScrollBar(15, 200);
            scbar.SetLocation(10, 10);
            viewport.AddContent(scbar);
            viewport.WinTop.TopDownReCalculateContentSize();
            scbar.InvalidateGraphic();

        }
    }
}