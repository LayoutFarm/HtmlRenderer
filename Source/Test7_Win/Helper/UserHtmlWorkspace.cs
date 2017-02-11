//Apache2, 2014-2017, WinterDev

using PixelFarm.Drawing;
using LayoutFarm.CustomWidgets;
namespace LayoutFarm
{
    class UserHtmlWorkspace
    {
        HtmlBox htmlBox;
        string htmltext;
        string documentRootPath;
        public virtual void OnViewportReady(SampleViewport viewport)
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
            this.documentRootPath = documentRootPath;
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
            e.SetResultImage(DemoBitmap.CreateFromGdiPlusBitmap(bmp));
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

    sealed class DemoBitmap : Image
    {
        int width;
        int height;

        public DemoBitmap(int w, int h, System.Drawing.Bitmap innerImage)
        {
            this.width = w;
            this.height = h;
            SetCacheInnerImage(this, innerImage);
        }
        public override int Width
        {
            get { return this.width; }
        }
        public override int Height
        {
            get { return this.height; }
        }

        public override void Dispose()
        {
        }
        public override bool IsReferenceImage
        {
            get { return false; }
        }
        public override int ReferenceX
        {
            get { return 0; }
        }
        public override int ReferenceY
        {
            get { return 0; }
        }


        public static DemoBitmap CreateFromGdiPlusBitmap(System.Drawing.Bitmap bmp)
        {
            return new DemoBitmap(bmp.Width, bmp.Height, bmp);
        }
    }

}