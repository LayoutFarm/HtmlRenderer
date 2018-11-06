//Apache2, 2014-present, WinterDev


using LayoutFarm.CustomWidgets;
namespace LayoutFarm
{
    [DemoNote("4.3.2 UIHtmlBox with ContentMx")]
    class Demo_UIHtmlBox_ContentMx : App
    {
        HtmlBoxes.HtmlHost _htmlHost;
        AppHost _host;
        string _imgFolderPath = null;
        HtmlBoxes.HtmlHost GetHtmlHost(AppHost host)
        {
            if (_htmlHost == null)
            {
                _htmlHost = HtmlHostCreatorHelper.CreateHtmlHost(host, null, null);
                var htmlBoxContentMx = new HtmlHostContentManager();
                var imgLoadingQ = new LayoutFarm.ContentManagers.ImageLoadingQueueManager();
                imgLoadingQ.AskForImage += contentMx_AskForImg;
                htmlBoxContentMx.ImgLoadingQueue = imgLoadingQ;
                htmlBoxContentMx.Bind(_htmlHost);
            }
            return _htmlHost;
        }


        protected override void OnStart(AppHost host)
        {
            _host = host;
            string appPath = System.Windows.Forms.Application.ExecutablePath;
            int pos = appPath.IndexOf("\\bin\\");
            if (pos > -1)
            {
                string sub01 = appPath.Substring(0, pos);
                _imgFolderPath = sub01 + "\\images";
            }
            //==================================================
            //html box
            var htmlBox = new HtmlBox(GetHtmlHost(host), 800, 600);
            host.AddChild(htmlBox);
            string html = "<html><head></head><body><div>OK1</div><div>3 Images</div><img src=\"sample01.png\"></img><img src=\"sample01.png\"></img><img src=\"sample01.png\"></img></body></html>";
            htmlBox.LoadHtmlString(html);
        }

        void contentMx_AskForImg(object sender, LayoutFarm.ContentManagers.ImageRequestEventArgs e)
        {
            //load resource -- sync or async? 
            string absolutePath = _imgFolderPath + "\\" + e.ImagSource;
            if (!System.IO.File.Exists(absolutePath))
            {
                return;
            }
            //load
            e.SetResultImage(_host.LoadImage(absolutePath));
        }
    }
}