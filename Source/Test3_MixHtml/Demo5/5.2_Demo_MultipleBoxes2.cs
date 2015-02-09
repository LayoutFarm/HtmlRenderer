// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;
using LayoutFarm.UI;

namespace LayoutFarm.WebWidgets
{
    [DemoNote("5.2 MultpleBox2")]
    class Demo_MultipleBox2 : DemoBase
    {
        LayoutFarm.HtmlWidgets.CheckBox currentSingleCheckedBox;
        LayoutFarm.ContentManagers.ImageContentManager imageContentMan = new ContentManagers.ImageContentManager();

        HtmlBoxes.HtmlHost myHtmlHost;
        SampleViewport sampleViewport;

        protected override void OnStartDemo(SampleViewport viewport)
        {
            this.sampleViewport = viewport;
            imageContentMan.ImageLoadingRequest += (s, e) =>
            {
                e.SetResultImage(LoadBitmap(e.ImagSource));
            };
            int boxHeight = 35;


            var htmlHost = myHtmlHost = HtmlHostCreatorHelper.CreateHtmlHost(viewport,
                (s, e) => this.imageContentMan.AddRequestImage(e.ImageBinder),
                (s, e) => { });
            //-------------------------------------------------------------------
            int boxX = 0;
            for (int i = 0; i < 5; ++i)
            {
                var button = new LayoutFarm.HtmlWidgets.Button(100, boxHeight);
                button.SetLocation(boxX, 20);
                button.Text = "button" + i;
                boxX += 100 + 2;
                AddToViewport(button);

            }
            //-------------------------------------------------------------------
            int boxY = 70;
            for (int i = 0; i < 10; ++i)
            {
                var statedBox = new LayoutFarm.HtmlWidgets.CheckBox( 100, boxHeight);
                statedBox.Text = "chk" + i;
                statedBox.SetLocation(10, boxY);
                boxY += boxHeight + 5;

                AddToViewport(statedBox);
                statedBox.WhenChecked += (s, e) =>
                {
                    var selectedBox = (LayoutFarm.HtmlWidgets.CheckBox)s;
                    if (selectedBox != currentSingleCheckedBox)
                    {
                        if (currentSingleCheckedBox != null)
                        {
                            currentSingleCheckedBox.Checked = false;
                        }
                        currentSingleCheckedBox = selectedBox;
                    }
                };
            }
        }

        void AddToViewport(HtmlWidgets.LightHtmlWidgetBase htmlWidget)
        {
            this.sampleViewport.AddContent(this.myHtmlHost, htmlWidget);
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