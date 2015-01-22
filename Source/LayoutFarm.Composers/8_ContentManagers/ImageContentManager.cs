// 2015,2014 ,BSD, WinterDev 
//ArthurHub

using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
using System.Threading;
using LayoutFarm.UI;

namespace LayoutFarm.ContentManagers
{



    public struct ImageContentRequest
    {
        internal readonly ImageBinder binder;
        internal readonly object requestBy;
        internal readonly IEventListener listener;

        public ImageContentRequest(ImageBinder binder,
            object requestBy,
            IEventListener listener)
        {
            this.binder = binder;
            this.requestBy = requestBy;
            this.listener = listener;
        }

    }


    class ImageCacheSystem
    {
        Dictionary<string, Image> cacheImages = new Dictionary<string, Image>();
        public ImageCacheSystem()
        {
        }
        public bool TryGetCacheImage(string url, out Image img)
        {
            return cacheImages.TryGetValue(url, out img);
        }
        public void AddCacheImage(string url, Image img)
        {
            this.cacheImages[url] = img;
        }
    }


    public class ImageContentManager
    {

        public event EventHandler<ImageRequestEventArgs> ImageLoadingRequest;

        LinkedList<ImageContentRequest> inputList = new LinkedList<ImageContentRequest>();
        LinkedList<ImageBinder> outputList = new LinkedList<ImageBinder>();


        ImageCacheSystem imageCacheLevel0 = new ImageCacheSystem();

        System.Timers.Timer timImageLoadMonitor = new System.Timers.Timer();

        bool hasSomeInputHint;
        bool hasSomeOutputHint;


        object outputListSync = new object();
        object inputListSync = new object();
        bool working = false;

        public ImageContentManager()
        {

            timImageLoadMonitor.Interval = 50;//30 ms check state
            timImageLoadMonitor.Elapsed += new System.Timers.ElapsedEventHandler(timImageLoadMonitor_Elapsed);
            timImageLoadMonitor.Start();

        }
        void timImageLoadMonitor_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
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

                ImageContentRequest req = firstNode.Value;
                //wait until finish this  .... 
                var binder = req.binder;

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
                        //send ready image notification to
                        //parent html container
                        //send data update to owner 
                        //req.listener.AddUpdatedImageBinder(binder);
                        if (req.listener != null)
                        {
                            req.listener.HandleContentUpdate();
                        }
                    }
                }
                else
                {
                    //process image infomation
                    //.... 

                    binder.SetImage(foundImage);
                    //send ready image notification to
                    //parent html container                             
                    if (req.listener != null)
                    {
                        req.listener.HandleContentUpdate();
                    }
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
        public void AddRequestImage(ImageContentRequest contentReq)
        {
            //binder and req box 

            //1. 
            contentReq.binder.State = ImageBinderState.Loading;
            //2.
            inputList.AddLast(contentReq);

            //another thread will manage this request 
            //and store in outputlist         
            hasSomeInputHint = true;
        }
    }
}