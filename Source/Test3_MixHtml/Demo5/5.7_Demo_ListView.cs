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
        LayoutFarm.HtmlBoxes.HtmlHost htmlHost;
        protected override void OnStartDemo(SampleViewport viewport)
        {
            imageContentMan.ImageLoadingRequest += (s, e) =>
            {
                e.SetResultImage(LoadBitmap(e.ImagSource));
            };

            htmlHost = HtmlHostCreatorHelper.CreateHtmlHost(viewport,
              (s, e) => this.imageContentMan.AddRequestImage(e.ImageBinder),
              (s, e) => { });
            //-------------------------------------------------------------------
             
            int boxX = 0;
            for (int i = 0; i < 1; ++i)
            {
                var hingeBox = new LayoutFarm.HtmlWidgets.HingeBox(htmlHost, 100, 30);
                hingeBox.SetLocation(boxX, 20);
                boxX += 100 + 2;
                viewport.AddContent(hingeBox);
            }
        }
        LayoutFarm.HtmlWidgets.HingeBox CreateHingeBox(int w, int h)
        {
            var hingeBox = new LayoutFarm.HtmlWidgets.HingeBox(htmlHost, 100, 30); 
            //1. set land part detail
            //2. set float part detail

            return hingeBox;
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
    }
}