//Apache2, 2014-present, WinterDev

using PixelFarm.Drawing;
using LayoutFarm.CustomWidgets;
using YourImplementation;

namespace LayoutFarm
{
    [DemoNote("4.1 UIHtmlBox")]
    class Demo_UIHtmlBox : App
    {

        HtmlBox htmlBox;
        string htmltext;
        string documentRootPath;
        AppHost _host;


        protected override void OnStart(AppHost host)
        {
            //html box
            _host = host;
            var loadingQueueMx = new LayoutFarm.ContentManagers.ImageLoadingQueueManager();
            loadingQueueMx.AskForImage += loadingQueue_AskForImg;

            HtmlBoxes.HtmlHost htmlHost = HtmlHostCreatorHelper.CreateHtmlHost(host,
                (s, e) => loadingQueueMx.AddRequestImage(e.ImageBinder),
                contentMx_LoadStyleSheet);

            //
            htmlBox = new HtmlBox(htmlHost, 1024, 800);
            //htmlBox.SetLocation(100, 0); //test
            host.AddChild(htmlBox);
            if (htmltext == null)
            {
                htmltext = @"<html><head></head><body>NOT FOUND!</body></html>";
            }
            htmlBox.LoadHtmlString(htmltext);
        }
        void loadingQueue_AskForImg(object sender, LayoutFarm.ContentManagers.ImageRequestEventArgs e)
        {
            //load resource -- sync or async? 
            string absolutePath = documentRootPath + "\\" + e.ImagSource;
            if (!System.IO.File.Exists(absolutePath))
            {
                return;
            }


            //load
            //lets host do img loading... 

            //we can do img resolve or caching here

            e.SetResultImage(_host.LoadImage(absolutePath));
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
        public void LoadHtml(string documentRootPath, string htmltext)
        {
            this.documentRootPath = System.IO.Path.GetDirectoryName(documentRootPath);
            this.htmltext = htmltext;
        }
    }
}