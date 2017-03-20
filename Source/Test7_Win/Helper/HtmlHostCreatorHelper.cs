//Apache2, 2014-2017, WinterDev

using System;
using System.Collections.Generic;
namespace LayoutFarm
{
    static class HtmlHostCreatorHelper
    {
        public static HtmlBoxes.HtmlHost CreateHtmlHost(SampleViewport sampleViewport,
            EventHandler<ContentManagers.ImageRequestEventArgs> imageReqHandler,
            EventHandler<ContentManagers.TextRequestEventArgs> textReq)
        {
            HtmlBoxes.HtmlHost htmlhost = new HtmlBoxes.HtmlHost();
            htmlhost.SetRootGraphics(sampleViewport.Root);

            List<HtmlBoxes.HtmlContainer> updateList = new List<HtmlBoxes.HtmlContainer>();
            sampleViewport.Root.ClearingBeforeRender += (s, e) =>
            {
                int j = updateList.Count;
                for (int i = 0; i < j; ++i)
                {
                    HtmlBoxes.HtmlContainer htmlCont = updateList[i];
                    htmlCont.RefreshDomIfNeed();
                    htmlCont.IsInUpdateQueue = false; 
                }
                updateList.Clear();
            };

            htmlhost.RegisterCssBoxGenerator(new LayoutFarm.CustomWidgets.MyCustomCssBoxGenerator(htmlhost));
            htmlhost.AttachEssentailHandlers(imageReqHandler, textReq);
            htmlhost.SetHtmlContainerUpdateHandler(htmlCont =>
            {

                if (!htmlCont.IsInUpdateQueue)
                {
                    htmlCont.IsInUpdateQueue = true;
                    updateList.Add(htmlCont);
                }
                ////add that to waiting queue
                //sampleViewport.Root.AddToUpdateQueue(htmlCont);
            });




            return htmlhost;
        }
    }
}