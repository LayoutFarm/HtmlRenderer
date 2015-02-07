// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;
using LayoutFarm.CustomWidgets;
using LayoutFarm.UI;

namespace LayoutFarm
{
    [DemoNote("4.3.1 UIHtmlBox with Resource Request1")]
    class Demo_UIHtmlBox_WithResReq1 : DemoBase
    {
        HtmlBoxes.HtmlHost htmlHost;
        HtmlBoxes.HtmlHost GetHtmlHost(SampleViewport viewport)
        {
            if (htmlHost == null)
            {
                htmlHost = HtmlHostCreatorHelper.CreateHtmlHost(viewport);
                htmlHost.AttachEssentailHandlers(
                    //1. img request
                    (s, e) =>
                    {
                        //load resource -- sync or async? 
                        string absolutePath = imgFolderPath + "\\" + e.binder.ImageSource;
                        if (!System.IO.File.Exists(absolutePath))
                        {
                            return;
                        }

                        //load create and load bitmap
                        System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(absolutePath);
                        e.binder.SetImage(new Bitmap(bmp.Width, bmp.Height, bmp));
                    },
                    //2. stylesheet request
                    (s, e) =>
                    {
                    });

            }
            return htmlHost;
        }

        string imgFolderPath = null;
        protected override void OnStartDemo(SampleViewport viewport)
        {
            var appPath = System.Windows.Forms.Application.ExecutablePath;
            int pos = appPath.IndexOf("\\bin\\");
            if (pos > -1)
            {
                string sub01 = appPath.Substring(0, pos);
                imgFolderPath = sub01 + "\\images";
            }
            //==================================================
            //html box
            var htmlBox = new HtmlBox(GetHtmlHost(viewport), 800, 600);


            viewport.AddContent(htmlBox);
            string html = "<html><head></head><body><div>OK1</div><div>3 Images</div><img src=\"sample01.png\"></img><img src=\"sample01.png\"></img><img src=\"sample01.png\"></img></body></html>";
            htmlBox.LoadHtmlText(html);
        }

    }
}