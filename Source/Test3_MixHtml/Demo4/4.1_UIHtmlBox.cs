//Apache2, 2014-2017, WinterDev

using PixelFarm.Drawing;
using LayoutFarm.CustomWidgets;
namespace LayoutFarm
{
    [DemoNote("4.1 UIHtmlBox")]
    class Demo_UIHtmlBox : DemoBase
    {
        HtmlBox htmlBox;
        string htmltext;
        string documentRootPath;
        protected override void OnStartDemo(SampleViewport viewport)
        {
            //html box
            var contentMx = new LayoutFarm.ContentManagers.ImageContentManager();
            contentMx.ImageLoadingRequest += contentMx_ImageLoadingRequest;
            var host = HtmlHostCreatorHelper.CreateHtmlHost(viewport,
                (s, e) => contentMx.AddRequestImage(e.ImageBinder),
                contentMx_LoadStyleSheet);
            htmlBox = new HtmlBox(host, 1024, 800);
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
             
            e.SetResultImage(LoadBitmap(absolutePath));
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