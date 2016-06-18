// 2015,2014 ,Apache2, WinterDev

using System;
namespace LayoutFarm
{
    public static class HtmlHostCreatorHelper
    {
        public static HtmlBoxes.HtmlHost CreateHtmlHost(SampleViewport sampleViewport,
            EventHandler<ContentManagers.ImageRequestEventArgs> imageReqHandler,
            EventHandler<ContentManagers.TextRequestEventArgs> textReq)
        {
            HtmlBoxes.HtmlHost htmlhost = new HtmlBoxes.HtmlHost(sampleViewport.P);
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