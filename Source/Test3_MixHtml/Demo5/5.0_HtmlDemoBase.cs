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
        protected LayoutFarm.ContentManagers.ImageContentManager imageContentMan = new ContentManagers.ImageContentManager();
        protected LayoutFarm.HtmlBoxes.HtmlHost myHtmlHost;
        protected SampleViewport sampleViewport;
        protected override void OnStartDemo(SampleViewport viewport)
        {
            this.sampleViewport = viewport;
            //init host
            myHtmlHost = HtmlHostCreatorHelper.CreateHtmlHost(viewport, null, null);
            //-----------
            this.sampleViewport = viewport;
            LayoutFarm.ContentManagers.ImageContentManager imageContentMan = new ContentManagers.ImageContentManager();
            imageContentMan.ImageLoadingRequest += (s, e) =>
            {
                e.SetResultImage(LoadBitmap(e.ImagSource));
            };
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
        protected virtual ImageBinder LoadImage(string filename)
        {
            System.Drawing.Bitmap gdiBmp = new System.Drawing.Bitmap(filename);
            Bitmap bmp = new Bitmap(gdiBmp.Width, gdiBmp.Height, gdiBmp);

            ImageBinder binder = new ClientImageBinder(null);
            binder.SetImage(bmp);
            binder.State = ImageBinderState.Loaded;
            return binder;
        }
        protected void AddToViewport(HtmlWidgets.LightHtmlWidgetBase htmlWidget)
        {
            this.sampleViewport.AddContent(this.myHtmlHost, htmlWidget);
        }
    }
}