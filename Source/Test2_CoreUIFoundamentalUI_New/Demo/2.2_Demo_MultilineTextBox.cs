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
    [DemoNote("2.2 MultiLineTextBox")]
    class Demo_MultiLineTextBox : DemoBase
    {
        protected override void OnStartDemo(UISurfaceViewportControl viewport)
        {
            var textbox = new LayoutFarm.SampleControls.UIMultiLineTextBox(400, 500, true);
            viewport.AddContent(textbox);
            textbox.InvalidateGraphic();
        }
    }
}