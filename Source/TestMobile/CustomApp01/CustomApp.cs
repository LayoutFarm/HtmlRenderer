//MIT, 2017, Zou Wei(github/zwcloud)
//MIT, 2017, WinterDev (modified from Xamarin's Android code template)

using System.IO;
using System;

using LayoutFarm;
using LayoutFarm.CustomWidgets;
using LayoutFarm.HtmlBoxes;

using PixelFarm.CpuBlit;
using PixelFarm.DrawingGL;
using PixelFarm.Drawing;

using YourImplementation;


namespace CustomApp01
{

    class Demo_UIHtmlBox : App
    {

        HtmlBox _htmlBox;
        string _htmltext;
        string _documentRootPath;
        AppHost _host;
        GLPainter _painter;
        RootGraphic _rootgfx;
        RenderElement _rootE;
        DrawBoard _drawBoard;
        protected override void OnStart(AppHost host)
        {
            //html box
            _host = host;
            _painter = (GLPainter)host.GetPainter();


            var loadingQueueMx = new LayoutFarm.ContentManagers.ImageLoadingQueueManager();
            loadingQueueMx.AskForImage += loadingQueue_AskForImg;

            HtmlHost htmlHost = HtmlHostCreatorHelper.CreateHtmlHost(host,
                 (s, e) => loadingQueueMx.AddRequestImage(e.ImageBinder),
                 contentMx_LoadStyleSheet);

            //
            _htmlBox = new HtmlBox(htmlHost, 1024, 800);
            _htmlBox.SetLocation(0, 300); //test
            _rootgfx = host.GetRootGraphics();
            _rootE = _htmlBox.GetPrimaryRenderElement(_rootgfx);

            _drawBoard = host.GetDrawBoard();

            host.AddChild(_htmlBox);


            //-------

            _htmltext = @"<html>
                    <head>
                    <style> 
                        .myfont1{font-size:30pt;background-color:yellow}
                        .myfont2{font-size:24pt;background-color:rgb(255,215,0)}
                    </style>
                    </head>
                    <body>
                           <div class='myfont1'>Hello</div>
                           <div class='myfont2'>... from HtmlRenderer</div>
                    </body>        
            </html>";

            //if (_htmltext == null)
            //{
            //    _htmltext = @"<html><head></head><body>NOT FOUND!</body></html>";
            //}
            _htmlBox.LoadHtmlString(_htmltext);
        }

        public override void RenderFrame()
        {
            _drawBoard.SetCanvasOrigin(_htmlBox.Left, _htmlBox.Top);
            _rootE.DrawToThisCanvas(_drawBoard, new Rectangle(0, 0, 1024, 800));

            base.RenderFrame();

            _drawBoard.SetCanvasOrigin(0, 0);
        }
        void loadingQueue_AskForImg(object sender, LayoutFarm.ContentManagers.ImageRequestEventArgs e)
        {
            //load resource -- sync or async? 
            //if we enable cache in loadingQueue (default=> enable)
            //if the loading queue dose not have the req img 
            //then it will raise event to here

            //we can resolve the req image to specific img
            //eg. 
            //1. built -in img from control may has special protocol
            //2. check if the req want a local file
            //3. or if req want to download from the network
            //

            //examples ...

            string absolutePath = null;
            if (e.ImagSource.StartsWith("built_in://imgs/"))
            {
                //substring
                absolutePath = _documentRootPath + "\\" + e.ImagSource.Substring("built_in://imgs/".Length);
            }
            else
            {
                absolutePath = _documentRootPath + "\\" + e.ImagSource;
            }

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
            string absolutePath = _documentRootPath + "\\" + e.Src;
            if (!System.IO.File.Exists(absolutePath))
            {
                return;
            }
            //if found
            e.TextContent = System.IO.File.ReadAllText(absolutePath);
        }
        public void LoadHtml(string documentRootPath, string htmltext)
        {
            _documentRootPath = System.IO.Path.GetDirectoryName(documentRootPath);
            _htmltext = htmltext;
        }
    }

}