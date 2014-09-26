﻿//2014 Apache2, WinterDev
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

        protected override void OnStartDemo(UISurfaceViewportControl viewport)
        {
            var textbox = new LayoutFarm.SampleControls.UITextBox(400, 30, false);
            viewport.AddContent(textbox);
            textbox.InvalidateGraphic();
        }
    }
}