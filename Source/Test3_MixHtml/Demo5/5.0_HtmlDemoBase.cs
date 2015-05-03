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

<<<<<<< HEAD
        protected void AddToViewport(HtmlWidgets.LightHtmlBoxWidgetBase htmlWidget)
=======
        protected void AddToViewport(HtmlWidgets.OldHtmlWidgetBase htmlWidget)
>>>>>>> v_widget2
        {
            sampleViewport.AddContent(GetPrimaryUIElement(htmlWidget, myHtmlHost));
        }
        UIElement GetPrimaryUIElement(HtmlWidgets.LightHtmlBoxWidgetBase htmlWidget, HtmlHost htmlhost)
        {
            htmlWidget.HtmlHost = htmlhost;
            var lightHtmlBox = new HtmlBox(htmlhost, htmlWidget.Width, htmlWidget.Height);
            HtmlDocument htmldoc = htmlhost.CreateNewSharedHtmlDoc();
            var myPresentationDom = htmlWidget.GetPresentationDomNode(htmldoc);
            if (myPresentationDom != null)
            {
                htmldoc.RootNode.AddChild(myPresentationDom);
                lightHtmlBox.LoadHtmlDom(htmldoc);
            }
            lightHtmlBox.SetLocation(htmlWidget.Left, htmlWidget.Top);
            lightHtmlBox.LayoutFinished += (s, e) => this.RaiseEventLayoutFinished();

            this.lightHtmlBox = lightHtmlBox;
            //first time
            OnPrimaryUIElementCreated(htmlhost);

            return this.lightHtmlBox;
        }
    }
}