//Apache2, 2014-present, WinterDev

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
                    //------
                    //only 1 manager that handle the img req
                    //------
                    //TODO: review 
                    foreach (ImageContentManager imgContentMx in imageContentManList.Keys)
                    {
                        if (imgContentMx.AddRequestImage(e.ImageBinder))
                        {
                            break;
                        }
                    }

                },
                //2. stylesheet request
                (s, e) =>
                {


                });
        }
        /// <summary>
        /// add text content manager
        /// </summary>
        /// <param name="textMan"></param>
        public void AddTextContentMan(TextContentManager textMan)
        {
            if (this.textContentManList.ContainsKey(textMan))
            {
                return;
            }
            this.textContentManList.Add(textMan, 0);
        }
        /// <summary>
        /// add image content manageer
        /// </summary>
        /// <param name="imageMan"></param>
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