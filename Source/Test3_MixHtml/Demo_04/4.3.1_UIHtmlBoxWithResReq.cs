//Apache2, 2014-present, WinterDev

using PixelFarm.Drawing;
using LayoutFarm.CustomWidgets;
namespace LayoutFarm
{
    [DemoNote("4.3.1 UIHtmlBox with Resource Request1")]
    class Demo_UIHtmlBox_WithResReq1 : App
    {
        HtmlBoxes.HtmlHost htmlHost;
        HtmlBoxes.HtmlHost GetHtmlHost(AppHost host)
        {
            if (htmlHost == null)
            {
                htmlHost = HtmlHostCreatorHelper.CreateHtmlHost(host,
                    //1. img request
                    (s, e) =>
                    {
                        //load resource -- sync or async? 
                        string absolutePath = imgFolderPath + "\\" + e.ImageBinder.ImageSource;
                        if (!System.IO.File.Exists(absolutePath))
                        {
                            return;
                        }

                        //load create and load bitmap                         
                        e.ImageBinder.SetLocalImage(host.LoadImage(absolutePath));
                    },
                    //2. stylesheet request
                    (s, e) =>
                    {
                    });
            }
            return htmlHost;
        }

        string imgFolderPath = null;
        protected override void OnStart(AppHost host)
        {
            string appPath = System.Windows.Forms.Application.ExecutablePath;
            int pos = appPath.IndexOf("\\bin\\");
            if (pos > -1)
            {
                string sub01 = appPath.Substring(0, pos);
                imgFolderPath = sub01 + "\\images";
            }
            //==================================================
            //html box
            var htmlBox = new HtmlBox(GetHtmlHost(host), 800, 600);
            host.AddChild(htmlBox);
            string html = "<html><head></head><body><div>OK1</div><div>3 Images</div><img src=\"sample01.png\"></img><img src=\"sample01.png\"></img><img src=\"sample01.png\"></img></body></html>";
            htmlBox.LoadHtmlString(html);
        }
    }
}