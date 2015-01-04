//2014 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using LayoutFarm.Drawing;
using LayoutFarm.CustomWidgets;
using LayoutFarm.UI;

namespace LayoutFarm
{
    [DemoNote("4.1 UIHtmlBox")]
    class Demo_UIHtmlBox : DemoBase
    {
        HtmlBox htmlBox;
        string htmltext;
        protected override void OnStartDemo(SampleViewport viewport)
        {

            //html box
            htmlBox = new HtmlBox(viewport.P, 800, 600);
            viewport.AddContent(htmlBox);
            if (htmltext == null)
            {
                htmltext = @"<html><head></head><body><div>OK1</div><div>OK2</div></body></html>";
            }

            htmlBox.LoadHtmlText(htmltext);
        }
        public void LoadHtml(string htmltext)
        {
            this.htmltext = htmltext;
        }

    }
}