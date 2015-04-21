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
    [DemoNote("4.6 dynamic dom1")]
    class Demo_DynamicDom1 : DemoBase
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
 

        }

    }

}