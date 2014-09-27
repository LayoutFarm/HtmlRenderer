//2014 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using LayoutFarm.Drawing;  
using LayoutFarm.UI;

namespace LayoutFarm
{
    [DemoNote("1.5 ScrollBar")]
    class Demo_ScrollBar : DemoBase
    {
        protected override void OnStartDemo(UISurfaceViewportControl viewport)
        {

            var scbar = new LayoutFarm.SampleControls.UIScrollBar(15, 200);
            scbar.SetLocation(10, 10);

            scbar.MinValue = 0;
            scbar.MaxValue = 170;
            scbar.SmallChange = 100;


            viewport.AddContent(scbar);


        }
    }
}