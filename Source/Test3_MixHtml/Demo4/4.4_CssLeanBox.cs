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
    [DemoNote("4.4 CssLeanBox")]
    class Demo_CssLeanBox : DemoBase
    {
        HtmlBoxes.HtmlIslandHost islandHost;
        HtmlBoxes.HtmlIslandHost GetIslandHost(SampleViewport viewport)
        {
            if (islandHost == null)
            {
                islandHost = new HtmlBoxes.HtmlIslandHost(viewport.P);
            } 
            return islandHost;             
        }
        protected override void OnStartDemo(SampleViewport viewport)
        {

            ////==================================================
            //html box
            var islandHost = GetIslandHost(viewport);
            HtmlBox htmlBox = new HtmlBox(islandHost, 800, 400);


            StringBuilder stbuilder = new StringBuilder();
            stbuilder.Append("<html><head></head><body>");
            stbuilder.Append("<div>custom box1</div>");
            stbuilder.Append("<x id=\"my_custombox1\"></x>");
            stbuilder.Append("<div>custom box2</div>");
            stbuilder.Append("<x type=\"textbox\" id=\"my_custombox1\"></x>");
            stbuilder.Append("</body></html>");

            htmlBox.LoadHtmlText(stbuilder.ToString());
            viewport.AddContent(htmlBox);
            //==================================================  

            //textbox
            var textbox = new LayoutFarm.CustomWidgets.TextBox(400, 100, true);
            textbox.SetLocation(0, 200);
            viewport.AddContent(textbox);
            textbox.Focus();

        }

    }
}