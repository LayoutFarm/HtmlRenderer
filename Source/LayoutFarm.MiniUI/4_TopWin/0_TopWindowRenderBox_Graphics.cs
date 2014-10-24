//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using LayoutFarm.Drawing;

namespace LayoutFarm.UI
{

    public delegate void CanvasInvalidateRequestDelegate(ref Rectangle invalidateArea);

    partial class TopWindowRenderBox
    {

        CanvasInvalidateRequestDelegate canvasInvaliddateReqDel;

        Stack<VisualDrawingChain> renderingChainStock = new Stack<VisualDrawingChain>();
        LinkedList<UITimerTask> renderTaskList = new LinkedList<UITimerTask>();
        LinkedList<LinkedListNode<UITimerTask>> tobeRemoveTasks = new LinkedList<LinkedListNode<UITimerTask>>();

        public void SetCanvasInvalidateRequest(CanvasInvalidateRequestDelegate canvasInvaliddateReqDel)
        {
            this.canvasInvaliddateReqDel = canvasInvaliddateReqDel;
        }
        bool LayoutQueueClearing
        {
            get { return this.rootGraphic.LayoutQueueClearing; }
            set { this.rootGraphic.LayoutQueueClearing = value; }
        }
        bool DisableGraphicOutputFlush
        {
            get { return this.rootGraphic.DisableGraphicOutputFlush; }
            set { this.rootGraphic.DisableGraphicOutputFlush = value; }
        }
        int GraphicUpdateSuspendCount
        {
            get { return this.rootGraphic.GraphicUpdateBlockCount; }
            set { this.rootGraphic.GraphicUpdateBlockCount = value; }
        }
        void FlushAccumGraphicUpdate()
        {
            this.rootGraphic.FlushAccumGraphicUpdate(this);
        }
        
        public override void FlushGraphic(Rectangle rect)
        {
            canvasInvaliddateReqDel(ref rect);
        } 
        public override void RootBeginGraphicUpdate()
        {
            GraphicUpdateSuspendCount++;
            DisableGraphicOutputFlush = true;
        }
        public override void RootEndGraphicUpdate()
        {
            GraphicUpdateSuspendCount--;
            if (GraphicUpdateSuspendCount <= 0)
            {
                DisableGraphicOutputFlush = false;
                FlushAccumGraphicUpdate();
                GraphicUpdateSuspendCount = 0;
            }
        }
        public void ClearNotificationSizeChangeList()
        {
            if (tobeNotifySizeChangedList.Count > 0)
            {
                int sizeChangeCount = tobeNotifySizeChangedList.Count;
                for (int i = 0; i < sizeChangeCount; ++i)
                {
                    ToNotifySizeChangedEvent item = tobeNotifySizeChangedList[i];
                    UISizeChangedEventArgs sizeChangedEventArg = UISizeChangedEventArgs.GetFreeOne(
                        null, item.xdiff, item.ydiff, item.affectedSideFlags);
                    UISizeChangedEventArgs.ReleaseOne(sizeChangedEventArg);
                }
                tobeNotifySizeChangedList.Clear();
            }
        }

        
        public void ChangeVisualRootSize(int width, int height)
        {
            this.ChangeRootElementSize(width, height);
        }
        public void Dispose()
        {

        }
        public void ClearAllResources()
        {
            this.DisableTaskTimer();
            ClearAllChildren();
        }
    }
}