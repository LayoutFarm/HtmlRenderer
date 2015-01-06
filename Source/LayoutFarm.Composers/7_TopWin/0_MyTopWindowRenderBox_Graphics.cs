//2014,2015 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using LayoutFarm.Drawing;

namespace LayoutFarm.UI
{


    partial class MyTopWindowRenderBox
    {

        CanvasInvalidateRequestDelegate canvasInvaliddateReqDel;
        public override void SetCanvasInvalidateRequest(CanvasInvalidateRequestDelegate canvasInvaliddateReqDel)
        {
            this.canvasInvaliddateReqDel = canvasInvaliddateReqDel;
        }
        bool LayoutQueueClearing
        {
            get { return this.rootGraphic.LayoutQueueClearing; }
            set { this.rootGraphic.LayoutQueueClearing = value; }
        }
        //bool DisableGraphicOutputFlush
        //{
        //    get { return this.rootGraphic.DisableGraphicOutputFlush; }
        //    set { this.rootGraphic.DisableGraphicOutputFlush = value; }
        //}
        //int GraphicUpdateSuspendCount
        //{
        //    get { return this.rootGraphic.GraphicUpdateBlockCount; }
        //    set { this.rootGraphic.GraphicUpdateBlockCount = value; }
        //}
        //void FlushAccumGraphicUpdate()
        //{
        //    this.rootGraphic.FlushAccumGraphicUpdate(this);
        //}

        public override void FlushGraphic(Rectangle rect)
        {
            canvasInvaliddateReqDel(ref rect);
        }
        //public override void RootBeginGraphicUpdate()
        //{
        //    GraphicUpdateSuspendCount++;
        //    DisableGraphicOutputFlush = true;
        //}
        //public override void RootEndGraphicUpdate()
        //{
        //    GraphicUpdateSuspendCount--;
        //    if (GraphicUpdateSuspendCount <= 0)
        //    {
        //        DisableGraphicOutputFlush = false;
        //        FlushAccumGraphicUpdate();
        //        GraphicUpdateSuspendCount = 0;
        //    }
        //}
        void ClearNotificationSizeChangeList()
        {
            //if (tobeNotifySizeChangedList.Count > 0)
            //{
            //    int sizeChangeCount = tobeNotifySizeChangedList.Count;
            //    for (int i = 0; i < sizeChangeCount; ++i)
            //    {
            //        ToNotifySizeChangedEvent item = tobeNotifySizeChangedList[i];
            //        UISizeChangedEventArgs sizeChangedEventArg = UISizeChangedEventArgs.GetFreeOne(
            //            null, item.xdiff, item.ydiff, item.affectedSideFlags);
            //        UISizeChangedEventArgs.ReleaseOne(sizeChangedEventArg);
            //    }
            //    tobeNotifySizeChangedList.Clear();
            //}
        }
        public override void ChangeRootGraphicSize(int width, int height)
        {
            Size currentSize = this.Size;
            if (currentSize.Width != width || currentSize.Height != height)
            {
                this.SetSize(width, height);

                this.InvalidateContentArrangementFromContainerSizeChanged();
                this.TopDownReCalculateContentSize();
                this.TopDownReArrangeContentIfNeed();
            }
        }

        //public void ClearAllResources()
        //{
        //    this.DisableTaskTimer();
        //    ClearAllChildren();
        //}
    }
}