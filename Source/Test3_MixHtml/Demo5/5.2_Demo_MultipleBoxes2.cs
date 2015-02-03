// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;
using LayoutFarm.UI;

namespace LayoutFarm
{
    [DemoNote("5.2 MultpleBox2")]
    class Demo_MultipleBox2 : DemoBase
    {
        LayoutFarm.HtmlWidgets.CheckBox currentSingleCheckedBox;
        LayoutFarm.ContentManagers.ImageContentManager imageContentMan = new ContentManagers.ImageContentManager();

        protected override void OnStartDemo(SampleViewport viewport)
        {
            imageContentMan.ImageLoadingRequest += (s, e) =>
            {
                e.SetResultImage(LoadBitmap(e.ImagSource));
            };
            int boxHeight = 35;
            int boxY = 50;

            var islandHost = new HtmlBoxes.HtmlIslandHost(viewport.P);
            islandHost.BaseStylesheet = LayoutFarm.Composers.CssParserHelper.ParseStyleSheet(null, true);
            islandHost.RequestResource += (s, e) => this.imageContentMan.AddRequestImage(e.binder);
            //-------------------------------------------------------------------
            for (int i = 0; i < 2; ++i)
            {

                var lightBoxHost = new HtmlBoxes.LightHtmlBoxHost(islandHost, viewport.P, viewport.Root);

                var statedBox = new LayoutFarm.HtmlWidgets.CheckBox(lightBoxHost, 100, boxHeight);
                statedBox.SetLocation(10, boxY);
                boxY += boxHeight + 5;
                viewport.AddContent(statedBox);
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