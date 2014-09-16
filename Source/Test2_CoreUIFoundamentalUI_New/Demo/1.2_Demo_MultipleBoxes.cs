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
    [DemoNote("1.2 MultpleBox")]
    class Demo_MultipleBox : DemoBase
    {
        protected override void OnStartDemo(UISurfaceViewportControl viewport)
        {
            for (int i = 0; i < 5; ++i)
            {
                var textbox = new LayoutFarm.SampleControls.UIButton(30, 30);
                textbox.SetLocation(i * 40, i * 40);

                viewport.AddContent(textbox);


            }
        }
    }
}