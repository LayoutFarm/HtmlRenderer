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
    public class HtmlBoxContentManager
    {
        HtmlBox htmlBox;
        Dictionary<TextContentManager, int> textContentManList = new Dictionary<TextContentManager, int>();
        Dictionary<ImageContentManager, int> imageContentManList = new Dictionary<ImageContentManager, int>();


        bool isBinded;

        public HtmlBoxContentManager()
        {

        }


        public void Bind(HtmlBox htmlBox)
        {
            this.htmlBox = htmlBox;
            this.htmlBox.RequestImage += htmlBox_RequestImage;
            this.htmlBox.RequestStylesheet += htmlBox_RequestStylesheet;
            this.isBinded = true;

        }
        void htmlBox_RequestStylesheet(object sender, TextLoadRequestEventArgs e)
        {

        }

        void htmlBox_RequestImage(object sender, ImageRequestEventArgs e)
        {
            foreach (ImageContentManager key in imageContentManList.Keys)
            {
                key.AddRequestImage(
                    new ImageContentRequest(e.ImageBinder,
                    null, htmlBox));
            }
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