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


         
        public HtmlBoxContentManager()
        {

        } 
        public void Bind(HtmlBox htmlBox)
        {
            this.htmlBox = htmlBox;
            this.htmlBox.HtmlHost.RequestImage += (s, e) =>
                {
                    foreach (ImageContentManager key in imageContentManList.Keys)
                    {
                        key.AddRequestImage(e.binder);
                    }
                };
            this.htmlBox.HtmlHost.RequestStyleSheet += (s, e) =>
            {

            };
             
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