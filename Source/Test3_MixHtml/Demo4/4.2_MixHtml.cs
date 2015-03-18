// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;
using LayoutFarm.CustomWidgets;
using LayoutFarm.UI;

namespace LayoutFarm
{
    [DemoNote("4.2 MixHtml and Text")]
    class Demo_MixHtml : DemoBase
    {

        protected override void OnStartDemo(SampleViewport viewport)
        {

            var htmlhost = HtmlHostCreatorHelper.CreateHtmlHost(viewport, null, null);
            ////==================================================
            //html box
            var htmlBox = new HtmlBox(htmlhost, 800, 400);
            htmlBox.SetLocation(30, 30);
            viewport.AddContent(htmlBox);
            string html = @"<html><head></head><body><div>OK1</div><div>OK2</div></body></html>";
            htmlBox.LoadHtmlString(html);
            //================================================== 

            //textbox
            var textbox = new LayoutFarm.CustomWidgets.TextBox(400, 100, true);
            textbox.SetLocation(0, 200);
            viewport.AddContent(textbox);
            textbox.Focus();

        }

    }

}