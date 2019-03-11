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

                    //
                    htmlVisualRootUpdateList.Add(htmlVisualRoot);
                }
            });

            //-----------------------------------------------------------------

            if (PaintLab.Svg.VgResourceIO.VgImgIOHandler == null)
            {
                var imgLoadingQ = new ContentManagers.ImageLoadingQueueManager();
                imgLoadingQ.AskForImage += (s, e) =>
                {
                    //check loading policy here  
                    //
                    e.SetResultImage(LoadImage(e.ImagSource));
                };
                PaintLab.Svg.VgResourceIO.VgImgIOHandler = (LayoutFarm.ImageBinder binder, PaintLab.Svg.VgVisualElement imgRun, object requestFrom) =>
                {
                    imgLoadingQ.AddRequestImage(binder);
                };
            }

            return htmlhost;
        }

        public static PixelFarm.CpuBlit.MemBitmap LoadImage(string imgName)
        {
            //TODO: review here again
            //1. do not access the system file directly
            //2.  ask for a file from 'host'
            if (!System.IO.File.Exists(imgName))
            {
                return null;
            }

            using (System.Drawing.Bitmap gdiBmp = new System.Drawing.Bitmap(imgName))
            {
                int w = gdiBmp.Width;
                int h = gdiBmp.Height;

                var bmpData = gdiBmp.LockBits(new System.Drawing.Rectangle(0, 0, w, h),
                    System.Drawing.Imaging.ImageLockMode.ReadOnly,
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                PixelFarm.CpuBlit.MemBitmap newBmp = PixelFarm.CpuBlit.MemBitmap.CreateFromCopy(w, h,
                     w * h * 4,
                    bmpData.Scan0);
                gdiBmp.UnlockBits(bmpData);

                return newBmp;
            }

        }
    }
}