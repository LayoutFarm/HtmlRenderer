//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;

namespace LayoutFarm
{
    public static class HtmlHostCreatorHelper
    {
        public static HtmlBoxes.HtmlHost CreateHtmlHost(AppHost appHost,
            EventHandler<ContentManagers.ImageRequestEventArgs> imageReqHandler,
            EventHandler<ContentManagers.TextRequestEventArgs> textReq)
        {
            HtmlBoxes.HtmlHost htmlhost = new HtmlBoxes.HtmlHost();
            htmlhost.SetRootGraphics(appHost.RootGfx);

            List<HtmlBoxes.HtmlContainer> htmlContUpdateList = new List<HtmlBoxes.HtmlContainer>();
            appHost.RootGfx.ClearingBeforeRender += (s, e) =>
            {

                //1.
                htmlhost.ClearUpdateWaitingCssBoxes();
                //1.
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