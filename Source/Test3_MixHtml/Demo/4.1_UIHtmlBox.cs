//2014 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using LayoutFarm.Drawing;

using LayoutFarm.UI;

namespace LayoutFarm
{
    [DemoNote("4.1 UIHtmlBox")]
    class Demo_UIHtmlBox : DemoBase
    {
        protected override void OnStartDemo(UISurfaceViewportControl viewport)
        {    
            //==================================================
            //html box
            UIHtmlBox htmlBox = new UIHtmlBox(800, 600);

            viewport.AddContent(htmlBox);
            string html = @"<html><head></head><body><div>OK1</div><div>OK2</div></body></html>";
            htmlBox.LoadHtmlText(html);
        }

    }
}