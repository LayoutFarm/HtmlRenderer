//Apache2, 2014-2016, WinterDev

using PixelFarm.Drawing;
using LayoutFarm.CustomWidgets;
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
                htmlHost = HtmlHostCreatorHelper.CreateHtmlHost(viewport,
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
                        e.ImageBinder.SetImage(LoadBitmap(absolutePath));
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
            htmlBox.LoadHtmlString(html);
        }
    }
}