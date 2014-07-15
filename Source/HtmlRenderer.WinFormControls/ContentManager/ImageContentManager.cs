
//BSD 2014, WinterDev 
using System;
using System.Collections.Generic;
using System.Drawing;
using HtmlRenderer.Drawing;
using HtmlRenderer.Boxes;
using System.Threading;


namespace HtmlRenderer.ContentManagers
{

    public struct ImageContentRequest
    {
        public readonly ImageBinder binder;
        public readonly CssBox box;
        public ImageContentRequest(ImageBinder binder, CssBox box)
        {
            this.binder = binder;
            this.box = box;
        }
    }


    public class ImageContentManager
    {

        public event EventHandler<HtmlImageRequestEventArgs> ImageLoadingRequest;

        LinkedList<ImageContentRequest> inputList = new LinkedList<ImageContentRequest>();

        LinkedList<ImageBinder> outputList = new LinkedList<ImageBinder>();
        HtmlContainer parentHtmlContainer;

        System.Timers.Timer timImageLoadMonitor = new System.Timers.Timer();

        bool hasSomeInputHint;
        bool hasSomeOutputHint;


        object outputListSync = new object();
        object inputListSync = new object();
        Thread imageLoadingThread;

        public ImageContentManager(HtmlContainer parentHtmlContainer)
        {
            this.parentHtmlContainer = parentHtmlContainer;
            timImageLoadMonitor.Interval = 50;//30 ms check state
            timImageLoadMonitor.Elapsed += new System.Timers.ElapsedEventHandler(timImageLoadMonitor_Elapsed);
            timImageLoadMonitor.Start();

        }
        void timImageLoadMonitor_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (hasSomeInputHint)
            {

                lock (inputListSync)
                {
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

                        this.ImageLoadingRequest(
                            this,
                            new HtmlImageRequestEventArgs(
                            binder));
                        //....
                        //process image infomation
                        //....



                        //send ready image notification to
                        //parent html container
                        this.parentHtmlContainer.AddRequestImageBinderUpdate(binder);



                        //next image
                    }
                    if (j == 0)
                    {
                        hasSomeInputHint = false;
                    }
                }
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