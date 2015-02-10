// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;
using LayoutFarm.UI;

namespace LayoutFarm.WebWidgets
{
    [DemoNote("5.7 ListView")]
    class Demo_ListView : DemoBase
    {

        LayoutFarm.ContentManagers.ImageContentManager imageContentMan = new ContentManagers.ImageContentManager();
        LayoutFarm.HtmlBoxes.HtmlHost myHtmlHost;
        SampleViewport sampleViewport;
        protected override void OnStartDemo(SampleViewport viewport)
        {
            this.sampleViewport = viewport;
            imageContentMan.ImageLoadingRequest += (s, e) =>
            {
                e.SetResultImage(LoadBitmap(e.ImagSource));
            };

            myHtmlHost = HtmlHostCreatorHelper.CreateHtmlHost(viewport,
              (s, e) => this.imageContentMan.AddRequestImage(e.ImageBinder),
              (s, e) => { });
            //-------------------------------------------------------------------


            var listview = new LayoutFarm.HtmlWidgets.ListView(100, 100);
            listview.SetLocation(30, 20);
            //add listview item 
            for (int i = 0; i < 10; ++i)
            {
                var listItem = new HtmlWidgets.ListItem(100, 20);
                listItem.Text = "item" + i;
                listview.AddItem(listItem);

            }
            AddToViewport(listview);
            //-------------------------------------------------------------------
            //add vertical scrollbar

            {
                //vertical scrollbar
                var vscbar = new LayoutFarm.CustomWidgets.ScrollBar(15,100);
                vscbar.SetLocation(10, 20);
                vscbar.MinValue = 0;
                vscbar.MaxValue = 170;
                vscbar.SmallChange = 20;
                viewport.AddContent(vscbar);
                //add relation between viewpanel and scroll bar 
                var scRelation = new LayoutFarm.CustomWidgets.ScrollingRelation(vscbar, listview);
            }
        }


        static void SetupImageList()
        {
            if (!LayoutFarm.CustomWidgets.ResImageList.HasImages)
            {
                //set imagelists
                var imgdic = new Dictionary<CustomWidgets.ImageName, Image>();
                imgdic[CustomWidgets.ImageName.CheckBoxUnChecked] = LoadBitmap("../../Demo/arrow_close.png");
                imgdic[CustomWidgets.ImageName.CheckBoxChecked] = LoadBitmap("../../Demo/arrow_open.png");
                LayoutFarm.CustomWidgets.ResImageList.SetImageList(imgdic);
            }
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
        void AddToViewport(HtmlWidgets.LightHtmlWidgetBase htmlWidget)
        {
            this.sampleViewport.AddContent(this.myHtmlHost, htmlWidget);
        }
    }
}