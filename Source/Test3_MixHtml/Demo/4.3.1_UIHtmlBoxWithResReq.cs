//2014 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using LayoutFarm.Drawing;
using LayoutFarm.SampleControls;

namespace LayoutFarm
{
    [DemoNote("4.3.1 UIHtmlBox with Resource Request1")]
    class Demo_UIHtmlBox_WithResReq1 : DemoBase
    {


        string imgFolderPath = null;
        protected override void OnStartDemo(UISurfaceViewportControl viewport)
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
            var htmlBox = new UIHtmlBox(800, 600);
            htmlBox.RequestImage += new EventHandler<HtmlRenderer.ContentManagers.ImageRequestEventArgs>(html_ImageReq);

            viewport.AddContent(htmlBox);
            string html = "<html><head></head><body><div>OK1</div><div>3 Images</div><img src=\"sample01.png\"></img><img src=\"sample01.png\"></img><img src=\"sample01.png\"></img></body></html>";
            htmlBox.LoadHtmlText(html);
        }
        void html_ImageReq(object sender, HtmlRenderer.ContentManagers.ImageRequestEventArgs e)
        {
            //load resource -- sync or async?

            string absolutePath = imgFolderPath + "\\" + e.ImagSource;
            if (!System.IO.File.Exists(absolutePath))
            {
                return;
            }

            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(absolutePath);
            e.SetResultImage(CurrentGraphicPlatform.P.CreateBitmap(bmp));
            
        }

    }
}