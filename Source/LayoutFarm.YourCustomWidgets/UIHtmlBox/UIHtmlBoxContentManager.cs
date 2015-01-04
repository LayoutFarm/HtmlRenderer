//2014 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using HtmlRenderer;
using HtmlRenderer.ContentManagers;
using LayoutFarm.UI;

namespace LayoutFarm.SampleControls
{
    public class UIHtmlBoxContentManager
    {
        UIHtmlBox htmlBox;
        Dictionary<TextContentManager, int> textContentManList = new Dictionary<TextContentManager, int>();
        Dictionary<ImageContentManager, int> imageContentManList = new Dictionary<ImageContentManager, int>();

         
        bool isBinded;

        public UIHtmlBoxContentManager()
        {
            
        }

        
        public void Bind(UIHtmlBox htmlBox)
        {


            this.htmlBox = htmlBox;
            this.htmlBox.RequestImage += new EventHandler<ImageRequestEventArgs>(htmlBox_RequestImage);
            this.htmlBox.RequestStylesheet += new EventHandler<TextLoadRequestEventArgs>(htmlBox_RequestStylesheet);
            this.isBinded = true; 

        }
        void htmlBox_RequestStylesheet(object sender, TextLoadRequestEventArgs e)
        {

        }

        void htmlBox_RequestImage(object sender, ImageRequestEventArgs e)
        {
            foreach (ImageContentManager key in imageContentManList.Keys)
            {
                key.AddRequestImage(new ImageContentRequest(e.ImageBinder, null, htmlBox.HtmlIsland));
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