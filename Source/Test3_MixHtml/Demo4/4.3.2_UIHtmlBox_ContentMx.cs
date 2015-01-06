//2014,2015 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using LayoutFarm.Drawing;
using LayoutFarm.CustomWidgets;
using LayoutFarm.UI;

namespace LayoutFarm
{
    [DemoNote("4.3.2 UIHtmlBox with ContentMx")]
    class Demo_UIHtmlBox_ContentMx : DemoBase
    {

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
            var htmlBox = new HtmlBox(800, 600);
            var htmlBoxContentMx = new HtmlBoxContentManager();
            var contentMx = new HtmlRenderer.ContentManagers.ImageContentManager();


            htmlBoxContentMx.AddImageContentMan(contentMx);
            htmlBoxContentMx.Bind(htmlBox);


            contentMx.ImageLoadingRequest += new EventHandler<HtmlRenderer.ContentManagers.ImageRequestEventArgs>(contentMx_ImageLoadingRequest);


            viewport.AddContent(htmlBox);
            string html = "<html><head></head><body><div>OK1</div><div>3 Images</div><img src=\"sample01.png\"></img><img src=\"sample01.png\"></img><img src=\"sample01.png\"></img></body></html>";
            htmlBox.LoadHtmlText(html);
        }

        void contentMx_ImageLoadingRequest(object sender, HtmlRenderer.ContentManagers.ImageRequestEventArgs e)
        {
            //load resource -- sync or async? 
            string absolutePath = imgFolderPath + "\\" + e.ImagSource;
            if (!System.IO.File.Exists(absolutePath))
            {
                return;
            }
            //load
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(absolutePath);
            e.SetResultImage(new Bitmap(bmp.Width, bmp.Height, bmp));
        }

    }
}