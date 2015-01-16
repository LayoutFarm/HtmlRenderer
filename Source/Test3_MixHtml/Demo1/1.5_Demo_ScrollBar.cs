// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;
using LayoutFarm.UI;

namespace LayoutFarm
{
    [DemoNote("1.5 ScrollBar")]
    class Demo_ScrollBar : DemoBase
    {
        protected override void OnStartDemo(SampleViewport viewport)
        {
            //----------------------------------------------------------------
            {   
                var scbar = new LayoutFarm.CustomWidgets.ScrollBar(15, 200);
                scbar.SetLocation(10, 10);
                scbar.MinValue = 0;
                scbar.MaxValue = 100;
                scbar.SmallChange = 50;
                viewport.AddContent(scbar);
            }
            //----------------------------------------------------------------
            {

                var scbar = new LayoutFarm.CustomWidgets.ScrollBar(15, 200);
                scbar.SetLocation(30, 10);
                scbar.MinValue = 0;
                scbar.MaxValue = 100;
                scbar.SmallChange = 25;
                viewport.AddContent(scbar);
            }
            //----------------------------------------------------------------
            {
                var scbar = new LayoutFarm.CustomWidgets.ScrollBar(15, 200);
                scbar.SetLocation(50, 10);
                scbar.MinValue = 0;
                scbar.MaxValue = 1000;
                scbar.SmallChange = 100;
                viewport.AddContent(scbar); 
            }

        }
    }
}