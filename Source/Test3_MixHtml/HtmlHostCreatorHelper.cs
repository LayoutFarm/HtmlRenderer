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

            //1.
            HtmlBoxes.HtmlHost htmlhost = new HtmlBoxes.HtmlHost(config);  //create html host with config 
            appHost.RootGfx.ClearingBeforeRender += (s, e) =>
            {
                //
                htmlhost.ClearUpdateWaitingCssBoxes();
                //
                int j = htmlVisualRootUpdateList.Count;
                for (int i = 0; i < j; ++i)
                {

                    HtmlBoxes.HtmlVisualRoot htmlVisualRoot = htmlVisualRootUpdateList[i];
                    htmlVisualRoot.RefreshDomIfNeed();
                    htmlVisualRoot.IsInUpdateQueue = false;
                }
                htmlVisualRootUpdateList.Clear();
            };
            //2.
            htmlhost.RegisterCssBoxGenerator(new LayoutFarm.CustomWidgets.MyCustomCssBoxGenerator(htmlhost));
            //3.
            htmlhost.AttachEssentailHandlers(imageReqHandler, textReq);
            //4.
            htmlhost.SetHtmlVisualRootUpdateHandler(htmlVisualRoot =>
            {
                if (!htmlVisualRoot.IsInUpdateQueue)
                {
                    htmlVisualRoot.IsInUpdateQueue = true;
                    htmlVisualRootUpdateList.Add(htmlVisualRoot);
                }
            });

            //-----------------------------------------------------------------
            //each HtmlHost has its own image content manager
            var imgContentMx = new ContentManagers.ImageContentManager();
            imgContentMx.ImageLoadingRequest += (s, e) =>
            {
                //check loading policy here  
                //
                e.SetResultImage(LoadImgForSvgElem(e.ImagSource));
            };

            PaintLab.Svg.VgResourceIO.VgImgIOHandler = (LayoutFarm.ImageBinder binder, PaintLab.Svg.SvgRenderElement imgRun, object requestFrom) =>
            {
                imgContentMx.AddRequestImage(binder);
            };
            return htmlhost;
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