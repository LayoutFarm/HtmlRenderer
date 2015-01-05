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
    [DemoNote("4.5 LightHtmlBox")]
    class Demo_LightHtmlBox : DemoBase
    {
        LightHtmlBoxHost lightBoxHost;
        protected override void OnStartDemo(SampleViewport viewport)
        {

            lightBoxHost = new LightHtmlBoxHost(viewport.P);
            lightBoxHost.SetRootGraphic(viewport.ViewportControl.WinTopRootGfx);

            ////==================================================
            //html box
            LightHtmlBox lightHtmlBox = lightBoxHost.CreateLightBox(800,400);
            viewport.AddContent(lightHtmlBox);
            
            //light box can't load full html
            //all light boxs of the same lightbox host share resource with the host
            string html = @"<div>OK1</div><div>OK2</div>";
            //if you want to use full html-> use HtmlBox instead 

            lightHtmlBox.LoadHtmlFragmentText(html);
            //================================================== 

            //textbox
            var textbox = new LayoutFarm.CustomWidgets.TextBox(400, 100, true);
            textbox.SetLocation(0, 200);
            viewport.AddContent(textbox);
            textbox.Focus();

        }

    }
}