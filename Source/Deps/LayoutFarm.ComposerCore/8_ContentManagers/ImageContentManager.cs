//BSD, 2014-2017, WinterDev 
//ArthurHub  , Jose Manuel Menendez Poo

using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
using LayoutFarm.UI;

namespace LayoutFarm.ContentManagers
{
    public class ImageRequestEventArgs : EventArgs
    {
        public ImageRequestEventArgs(ImageBinder binder)
        {
            this.ImageBinder = binder;
        }

        //TODO: review here
        public object requestBy;
        public ImageBinder ImageBinder { get; private set; }
        public string ImagSource
        {
            get { return this.ImageBinder.ImageSource; }
        }
        public void SetResultImage(Image img)
        {
            this.ImageBinder.SetImage(img);
        }
    }

    public class ImageContentManager
    {
        public event EventHandler<ImageRequestEventArgs> ImageLoadingRequest;
        LinkedList<ImageBinder> inputList = new LinkedList<ImageBinder>();
        LinkedList<ImageBinder> outputList = new LinkedList<ImageBinder>();
        ImageCacheSystem imageCacheLevel0 = new ImageCacheSystem();
        UITimer timImageLoadMonitor;
        bool hasSomeInputHint;
        bool hasSomeOutputHint;
        object outputListSync = new object();
        object inputListSync = new object();
        bool working = false;
        public ImageContentManager(UIPlatform platform)
        {
            timImageLoadMonitor = platform.CreateUITimer();
            //TODO: review here****
            timImageLoadMonitor.Interval = 50;//30 ms check state             
            timImageLoadMonitor.Tick += TimImageLoadMonitor_Tick;
            timImageLoadMonitor.Enabled = true;
        }
        private void TimImageLoadMonitor_Tick(object sender, EventArgs e)
        {
            lock (inputListSync)
            {
                if (working)
                {
                    return;
                }
                if (!hasSomeInputHint)
                {
                    return;
                }
                working = true;
            }



            int j = inputList.Count;
            //load image in this list
            List<ImageBinder> tmploadingList = new List<ImageBinder>();
            //copy data out 
            for (int i = 0; i < j; ++i)
            {
                var firstNode = inputList.First;
                inputList.RemoveFirst();
                ImageBinder binder = firstNode.Value;
                //wait until finish this  .... 


                //1. check from cache if not found
                //then send request to external ... 

                Image foundImage;
                if (!this.imageCacheLevel0.TryGetCacheImage(
                    binder.ImageSource,
                    out foundImage))
                {
                    this.ImageLoadingRequest(
                        this,
                        new ContentManagers.ImageRequestEventArgs(
                        binder));
                    //....
                    //process image infomation
                    //.... 
                    if (binder.State == ImageBinderState.Loaded)
                    {
                        //store to cache 
                        //TODO: implement caching policy  
                        imageCacheLevel0.AddCacheImage(binder.ImageSource, binder.Image);
                    }
                }
                else
                {
                    //process image infomation
                    //....  
                    binder.SetImage(foundImage);
                }

                //next image
            }
            if (j == 0)
            {
                hasSomeInputHint = false;
            }


            if (hasSomeOutputHint)
            {
                lock (outputListSync)
                {
                    if (outputList.Count > 0)
                    {
                    }
                }
            }
            working = false;
        }



        public void AddRequestImage(ImageBinder contentReq)
        {
            if (contentReq.ImageSource == null && !contentReq.HasLazyFunc)
            {
                contentReq.State = ImageBinderState.NoImage;
                return;
            }
            //binder and req box 
            //1. 
            contentReq.State = ImageBinderState.Loading;
            //2.
            inputList.AddLast(contentReq);
            //another thread will manage this request 
            //and store in outputlist         
            hasSomeInputHint = true;
        }
    }
}