//Apache2, 2014-2017, WinterDev

using System;
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
            htmlhost.RegisterCssBoxGenerator(new LayoutFarm.CustomWidgets.MyCustomCssBoxGenerator(htmlhost));
            htmlhost.AttachEssentailHandlers(imageReqHandler, textReq);
            htmlhost.SetHtmlContainerUpdateHandler(htmlCont =>
            {
                sampleViewport.Root.AddToUpdateQueue(htmlCont);
            });
            return htmlhost;
        }
    }
}