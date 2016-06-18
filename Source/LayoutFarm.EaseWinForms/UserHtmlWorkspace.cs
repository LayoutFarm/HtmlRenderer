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
    class UserHtmlWorkspace
    {
        HtmlBox htmlBox;
        string htmltext;
        string documentRootPath;
        public virtual void OnStartDemo(SampleViewport viewport)
        {
            //html box
            var contentMx = new LayoutFarm.ContentManagers.ImageContentManager();
            contentMx.ImageLoadingRequest += contentMx_ImageLoadingRequest;
            var host = HtmlHostCreatorHelper.CreateHtmlHost(viewport,
                (s, e) => contentMx.AddRequestImage(e.ImageBinder),
                contentMx_LoadStyleSheet);
            //1. htmlbox
            int viewportW = viewport.ViewportControl.Width;
            int viewportH = viewport.ViewportControl.Height;
            htmlBox = new HtmlBox(host, viewportW, viewportH);
            viewport.AddContent(htmlBox);
            if (htmltext == null)
            {
                htmltext = @"<html><head></head><body>NOT FOUND!</body></html>";
            }

            htmlBox.LoadHtmlString(htmltext);
        }
        public void LoadHtml(string documentRootPath, string htmltext)
        {
            this.documentRootPath = System.IO.Path.GetDirectoryName(documentRootPath);
            this.htmltext = htmltext;
            htmlBox.LoadHtmlString(htmltext);
        }
        public WebDom.WebDocument GetHtmlDom()
        {
            return htmlBox.HtmlContainer.WebDocument;
        }
        void contentMx_ImageLoadingRequest(object sender, LayoutFarm.ContentManagers.ImageRequestEventArgs e)
        {
            //load resource -- sync or async? 
            string absolutePath = documentRootPath + "\\" + e.ImagSource;
            if (!System.IO.File.Exists(absolutePath))
            {
                return;
            }
            //load
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(absolutePath);
            e.SetResultImage(new Bitmap(bmp.Width, bmp.Height, bmp));
        }
        void contentMx_LoadStyleSheet(object sender, LayoutFarm.ContentManagers.TextRequestEventArgs e)
        {
            string absolutePath = documentRootPath + "\\" + e.Src;
            if (!System.IO.File.Exists(absolutePath))
            {
                return;
            }
            //if found
            e.TextContent = System.IO.File.ReadAllText(absolutePath);
        }
    }
}