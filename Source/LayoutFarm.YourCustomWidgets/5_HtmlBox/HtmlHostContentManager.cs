// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using LayoutFarm;
using LayoutFarm.ContentManagers;
using LayoutFarm.UI;

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
                    foreach (ImageContentManager key in imageContentManList.Keys)
                    {
                        key.AddRequestImage(e.ImageBinder);
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