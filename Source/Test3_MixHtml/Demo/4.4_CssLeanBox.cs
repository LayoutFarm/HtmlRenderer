//2014 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using LayoutFarm.Drawing;
using LayoutFarm.SampleControls;

namespace LayoutFarm
{
    [DemoNote("4.4 CssLeanBox")]
    class Demo_CssLeanBox : DemoBase
    {
        protected override void OnStartDemo(UISurfaceViewportControl viewport)
        {

            ////==================================================
            //html box
            UIHtmlBox htmlBox = new UIHtmlBox(800, 400);
            viewport.AddContent(htmlBox);
            StringBuilder stbuilder = new StringBuilder();
            stbuilder.Append("<html><head></head><body>");
            stbuilder.Append("<div>custom box1</div>");
            stbuilder.Append("<x id=\"my_custombox1\"></x>");
            stbuilder.Append("<div>custom box2</div>");
            stbuilder.Append("<x type=\"textbox\" id=\"my_custombox1\"></x>");
            stbuilder.Append("</body></html>");

            htmlBox.LoadHtmlText(stbuilder.ToString());

            //==================================================  
        }

    }
}