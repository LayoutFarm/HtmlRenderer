// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;
using LayoutFarm.UI;

namespace LayoutFarm
{
    [DemoNote("5.9 DemoTabPage")]
    class DemoTabPage : DemoBase
    {
        LayoutFarm.ContentManagers.ImageContentManager imageContentMan = new ContentManagers.ImageContentManager();
        LayoutFarm.HtmlBoxes.HtmlHost myHtmlHost;
        SampleViewport sampleViewport;
        protected override void OnStartDemo(SampleViewport viewport)
        {
            this.sampleViewport = viewport;
            LayoutFarm.ContentManagers.ImageContentManager imageContentMan = new ContentManagers.ImageContentManager();
            imageContentMan.ImageLoadingRequest += (s, e) =>
            {
                e.SetResultImage(LoadBitmap(e.ImagSource));
            };

            myHtmlHost = HtmlHostCreatorHelper.CreateHtmlHost(viewport,
              (s, e) => this.imageContentMan.AddRequestImage(e.ImageBinder),
              (s, e) => { });


            var tabContainer = new LayoutFarm.HtmlWidgets.TabPageContainer(300, 400);
            tabContainer.SetLocation(10, 10);
            AddToViewport(tabContainer);

            //tabContainer.BackColor = KnownColors.FromKnownColor(KnownColor.LightGray);
            //viewport.AddContent(tabContainer); 
            ////add 
            for (int i = 0; i < 10; ++i)
            {
                var tabPage = new LayoutFarm.HtmlWidgets.TabPage(400, 20);
                if ((i % 2) == 0)
                {
                    //tabPage.BackColor = KnownColors.FromKnownColor(KnownColor.OrangeRed);
                }
                else
                {
                    //tabPage.BackColor = KnownColors.FromKnownColor(KnownColor.Yellow);
                }
                tabContainer.AddItem(tabPage);
            }
            //tabContainer.SelectedIndex = 1;
            

        }
        void AddToViewport(HtmlWidgets.LightHtmlWidgetBase htmlWidget)
        {
            this.sampleViewport.AddContent(this.myHtmlHost, htmlWidget);
        }
        static Bitmap LoadBitmap(string filename)
        {
            System.Drawing.Bitmap gdiBmp = new System.Drawing.Bitmap(filename);
            Bitmap bmp = new Bitmap(gdiBmp.Width, gdiBmp.Height, gdiBmp);
            return bmp;
        }
        static ImageBinder LoadImage(string filename)
        {
            System.Drawing.Bitmap gdiBmp = new System.Drawing.Bitmap(filename);
            Bitmap bmp = new Bitmap(gdiBmp.Width, gdiBmp.Height, gdiBmp);

            ImageBinder binder = new ClientImageBinder(null);
            binder.SetImage(bmp);
            binder.State = ImageBinderState.Loaded;
            return binder;
        }
    }
}