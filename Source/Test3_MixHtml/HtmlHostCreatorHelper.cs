// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing; 
using LayoutFarm.UI;

namespace LayoutFarm
{
    public static class HtmlHostCreatorHelper
    {
        public static HtmlBoxes.HtmlHost CreateHtmlHost(SampleViewport sampleViewport)
        {   
            HtmlBoxes.HtmlHost htmlhost = new HtmlBoxes.HtmlHost(sampleViewport.P);
            htmlhost.SetHtmlContainerUpdateHandler((htmlCont) =>
            {
                sampleViewport.Root.AddToUpdateQueue(htmlCont);
            });
            return htmlhost;
        }
    }

}