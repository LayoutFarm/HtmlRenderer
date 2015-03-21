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
            {
                var textbox = new LayoutFarm.CustomWidgets.TextBox(400, 500, true); 
                var style = new Text.TextSpanStyle(); 
                style.FontInfo = viewport.P.GetFont("tahoma", 10, PixelFarm.Drawing.FontStyle.Regular);
                textbox.DefaultSpanStyle = style;

                viewport.AddContent(textbox);
            }

            {
                var textbox = new LayoutFarm.CustomWidgets.TextBox(400, 500, true);

                textbox.SetLocation(0, 120);
                viewport.AddContent(textbox);
            }
        }
    }
}