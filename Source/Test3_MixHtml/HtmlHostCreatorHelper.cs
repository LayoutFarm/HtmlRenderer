//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;

namespace LayoutFarm
{
    //YOUR IMPLEMENTATION ...

    public static class HtmlHostCreatorHelper
    {
        public static HtmlBoxes.HtmlHost CreateHtmlHost(AppHost appHost,
            EventHandler<ContentManagers.ImageRequestEventArgs> imageReqHandler,
            EventHandler<ContentManagers.TextRequestEventArgs> textReq)
        {
            List<HtmlBoxes.HtmlVisualRoot> htmlVisualRootUpdateList = new List<HtmlBoxes.HtmlVisualRoot>();

            var config = new HtmlBoxes.HtmlHostCreationConfig()
            {
                RootGraphic = appHost.RootGfx,
                TextService = appHost.RootGfx.TextServices
            };

            HtmlBoxes.HtmlHost htmlhost = new HtmlBoxes.HtmlHost(config);  //create html host with config 
            appHost.RootGfx.ClearingBeforeRender += (s, e) =>
            {
                //1.
                htmlhost.ClearUpdateWaitingCssBoxes();
                //2. remaining 
                int j = htmlVisualRootUpdateList.Count;
                for (int i = 0; i < j; ++i)
                {

                    HtmlBoxes.HtmlVisualRoot htmlVisualRoot = htmlVisualRootUpdateList[i];
                    htmlVisualRoot.RefreshDomIfNeed();
                    htmlVisualRoot.IsInUpdateQueue = false;
                }
                htmlVisualRootUpdateList.Clear();
            };
            htmlhost.RegisterCssBoxGenerator(new LayoutFarm.CustomWidgets.MyCustomCssBoxGenerator(htmlhost));
            htmlhost.AttachEssentailHandlers(imageReqHandler, textReq);
            htmlhost.SetHtmlVisualRootUpdateHandler(htmlVisualRoot =>
            {
                if (!htmlVisualRoot.IsInUpdateQueue)
                {
                    htmlVisualRoot.IsInUpdateQueue = true;
                    htmlVisualRootUpdateList.Add(htmlVisualRoot);
                }
            });



            PaintLab.Svg.VgResourceIO.VgImgIOHandler = RequestImgAync;
            return htmlhost;
        }


        static ContentManagers.ImageContentManager _contentMx;
        static void RequestImgAync(LayoutFarm.ImageBinder binder, PaintLab.Svg.SvgRenderElement imgRun, object requestFrom)
        {
            //create default
            if (_contentMx == null)
            {
                _contentMx = new ContentManagers.ImageContentManager();
                _contentMx.ImageLoadingRequest += (s, e) =>
                {
                    e.SetResultImage(LoadImgForSvgElem(e.ImagSource));
                };
            }
            _contentMx.AddRequestImage(binder);
        }
        static PixelFarm.Drawing.Image LoadImgForSvgElem(string imgName)
        {
            //temp fix
            if (imgName == "html32.png")
            {
                //TODO: review document root
                imgName = @"D:\projects\HtmlRenderer\Source\Test8_HtmlRenderer.Demo\Samples\SvgSamples\" + imgName;
            }
            //handle resource load
            if (!System.IO.File.Exists(imgName))
            {

            }

            using (System.Drawing.Bitmap gdiBmp = new System.Drawing.Bitmap(imgName))
            {
                int w = gdiBmp.Width;
                int h = gdiBmp.Height;

                var bmpData = gdiBmp.LockBits(new System.Drawing.Rectangle(0, 0, w, h), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                int[] buffer = new int[w * h];
                System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, buffer, 0, w * h);
                gdiBmp.UnlockBits(bmpData);

                return new PixelFarm.CpuBlit.ActualBitmap(gdiBmp.Width, gdiBmp.Height, buffer);
            }

        }
    }
}