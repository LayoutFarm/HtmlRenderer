// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;
using LayoutFarm.UI;

using LayoutFarm.Composers;
using LayoutFarm.WebDom;
using LayoutFarm.WebDom.Extension;
using LayoutFarm.HtmlBoxes;
using LayoutFarm.CustomWidgets;

namespace LayoutFarm.WebWidgets
{

    abstract class HtmlDemoBase : DemoBase
    {
        LayoutFarm.ContentManagers.ImageContentManager imageContentMan;
        protected LayoutFarm.HtmlBoxes.HtmlHost myHtmlHost;
        protected SampleViewport sampleViewport;
        protected override void OnStartDemo(SampleViewport viewport)
        {
            this.sampleViewport = viewport;


            //-----------
            this.sampleViewport = viewport;
            imageContentMan = new ContentManagers.ImageContentManager();
            imageContentMan.ImageLoadingRequest += (s, e) =>
            {
                e.SetResultImage(LoadBitmap(e.ImagSource));
            };

            //init host 
            myHtmlHost = HtmlHostCreatorHelper.CreateHtmlHost(viewport,
              (s, e) => this.imageContentMan.AddRequestImage(e.ImageBinder),
              (s, e) => { });
            //-----------

            OnHtmlHostCreated();
        }
        protected virtual void OnHtmlHostCreated()
        {

        }
        protected virtual Bitmap LoadBitmap(string filename)
        {
            System.Drawing.Bitmap gdiBmp = new System.Drawing.Bitmap(filename);
            Bitmap bmp = new Bitmap(gdiBmp.Width, gdiBmp.Height, gdiBmp);
            return bmp;
        }

        protected void AddToViewport(HtmlWidgets.OldHtmlWidgetBase htmlWidget)
        {
           
            //1.  
            sampleViewport.AddContent(htmlWidget.GetPrimaryUIElement(myHtmlHost));
        }
    }
}