//Apache2, 2014-2017, WinterDev

using System;
using System.Collections.Generic;

namespace LayoutFarm
{
    public static class HtmlHostCreatorHelper
    {
        public static HtmlBoxes.HtmlHost CreateHtmlHost(SampleViewport sampleViewport,
            EventHandler<ContentManagers.ImageRequestEventArgs> imageReqHandler,
            EventHandler<ContentManagers.TextRequestEventArgs> textReq)
        {
            HtmlBoxes.HtmlHost htmlhost = new HtmlBoxes.HtmlHost();
            htmlhost.SetRootGraphics(sampleViewport.Root);

            List<HtmlBoxes.HtmlContainer> htmlContUpdateList = new List<HtmlBoxes.HtmlContainer>();
            sampleViewport.Root.ClearingBeforeRender += (s, e) =>
            {
                int j = htmlContUpdateList.Count;
                for (int i = 0; i < j; ++i)
                {

                    var htmlCont = htmlContUpdateList[i]; 
                    htmlCont.RefreshDomIfNeed();
                    htmlCont.IsInUpdateQueue = false;
                }
                htmlContUpdateList.Clear();
            };
            htmlhost.RegisterCssBoxGenerator(new LayoutFarm.CustomWidgets.MyCustomCssBoxGenerator(htmlhost));
            htmlhost.AttachEssentailHandlers(imageReqHandler, textReq);
            htmlhost.SetHtmlContainerUpdateHandler(htmlCont =>
            {
                if (!htmlCont.IsInUpdateQueue)
                {
                    htmlCont.IsInUpdateQueue = true;
                    htmlContUpdateList.Add(htmlCont);
                }
            });
            return htmlhost;
        }
    }
}