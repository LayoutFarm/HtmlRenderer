// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;
using LayoutFarm.UI;

namespace LayoutFarm
{
    [DemoNote("2.2 MultiLineTextBox")]
    class Demo_MultiLineTextBox : DemoBase
    {
        protected override void OnStartDemo(SampleViewport viewport)
        {

            var textbox1 = new LayoutFarm.CustomWidgets.TextBox(400, 100, true);
            var style1 = new Text.TextSpanStyle();
            style1.FontInfo = viewport.P.GetFont("tahoma", 10, PixelFarm.Drawing.FontStyle.Regular);
            textbox1.DefaultSpanStyle = style1;
            viewport.AddContent(textbox1);
             
            var textbox2 = new LayoutFarm.CustomWidgets.TextBox(400, 500, true);
            textbox2.SetLocation(20, 120);
            viewport.AddContent(textbox2);
            textbox2.Text = "Hello World!";

        }
    }
}