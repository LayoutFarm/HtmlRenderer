//Apache2, 2014-2018, WinterDev

using System;
using System.Collections.Generic;
using LayoutFarm.ContentManagers;
namespace LayoutFarm.CustomWidgets
{
    public class HtmlHostContentManager
    {
        HtmlBoxes.HtmlHost htmlHost;
        Dictionary<TextContentManager, int> textContentManList = new Dictionary<TextContentManager, int>();
        Dictionary<ImageContentManager, int> imageContentManList = new Dictionary<ImageContentManager, int>();
        public HtmlHostContentManager()
        {
        }
        public void Bind(HtmlBoxes.HtmlHost htmlHost)
        {
            this.htmlHost = htmlHost;
            this.htmlHost.AttachEssentailHandlers(
                //1. image req
                (s, e) =>
                {
                    foreach (ImageContentManager imgContentMx in imageContentManList.Keys)
                    {
                        imgContentMx.AddRequestImage(e.ImageBinder);
                    }
                },
                //2. stylesheet request
                (s, e) =>
                {


                });
        }

        public void AddTextContentMan(TextContentManager textMan)
        {
            if (this.textContentManList.ContainsKey(textMan))
            {
                return;
            }
            this.textContentManList.Add(textMan, 0);
        }
        public void AddImageContentMan(ImageContentManager imageMan)
        {
            if (this.imageContentManList.ContainsKey(imageMan))
            {
                return;
            }
            this.imageContentManList.Add(imageMan, 0);
        }
    }
}