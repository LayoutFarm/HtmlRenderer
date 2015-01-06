//2014,2015 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using LayoutFarm.Drawing;  
using LayoutFarm.UI;

namespace LayoutFarm
{
    [DemoNote("2.1 SingleLineText")]
    class Demo_SingleLineText : DemoBase
    {

        protected override void OnStartDemo(SampleViewport viewport)
        {
            var textbox = new LayoutFarm.CustomWidgets.UITextBox(400, 30, false);
            viewport.AddContent(textbox);
            textbox.InvalidateGraphic();
        }
    }
}