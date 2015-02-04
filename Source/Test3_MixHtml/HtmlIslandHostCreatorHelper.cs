// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing; 
using LayoutFarm.UI;

namespace LayoutFarm
{
    public static class HtmlIslandHostCreatorHelper
    {
        public static HtmlBoxes.HtmlIslandHost CreateHtmlIslandHost(SampleViewport sampleViewport)
        {

            HtmlBoxes.HtmlIslandHost htmlIslandHost = new HtmlBoxes.HtmlIslandHost(sampleViewport.P);
            htmlIslandHost.SetHtmlIslandUpdateHandler((htmlIsland) =>
            {
                sampleViewport.Root.AddToUpdateQueue(htmlIsland);
            });
            return htmlIslandHost;
        }
    }

}