//2014 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using LayoutFarm.Drawing;
using LayoutFarm.SampleControls;

namespace LayoutFarm
{
    [DemoNote("4.2 MixHtml and Text")]
    class Demo_MixHtml : DemoBase
    {
        protected override void OnStartDemo(UISurfaceViewportControl viewport)
        {

            ////==================================================
            //html box
            UIHtmlBox htmlBox = new UIHtmlBox(800, 400);
            viewport.AddContent(htmlBox);
            string html = @"<html><head></head><body><div>OK1</div><div>OK2</div></body></html>";
            htmlBox.LoadHtmlText(html);
            //================================================== 

            //textbox
            var textbox = new LayoutFarm.SampleControls.UITextBox(400, 100, true);
            textbox.SetLocation(0, 200);
            viewport.AddContent(textbox);
            textbox.Focus();

        }

    }
}