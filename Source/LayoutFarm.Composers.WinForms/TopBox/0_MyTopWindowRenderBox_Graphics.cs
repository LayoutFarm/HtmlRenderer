//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using LayoutFarm.Drawing;


namespace LayoutFarm
{

    partial class MyTopWindowRenderBox
    {

        public event EventHandler<UIInvalidateEventArgs> CanvasInvalidatedEvent;
        public event EventHandler<EventArgs> CanvasForcePaint;

        Stack<VisualDrawingChain> renderingChainStock = new Stack<VisualDrawingChain>();
        LinkedList<VisualRootTimerTask> rootTimerTasks = new LinkedList<VisualRootTimerTask>();
        System.Timers.Timer rootTasksTimer;
        LinkedList<LinkedListNode<VisualRootTimerTask>> tobeRemoveTasks = new LinkedList<LinkedListNode<VisualRootTimerTask>>();


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
        public override void FlushGraphic(Rectangle rect)
        {
            UIInvalidateEventArgs e = this.eventStock.GetFreeCanvasInvalidatedEventArgs();
            e.InvalidArea = rect;
            CanvasInvalidatedEvent(this, e);
            eventStock.ReleaseEventArgs(e);
        }

        internal void FlushAccumGraphicUpdate()
        {
            this.rootGraphic.FlushAccumGraphicUpdate(this);
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

        public void BeginRenderPhase()
        {
            this.rootGraphic.IsInRenderPhase = true;
#if DEBUG
            RootGraphic myroot = this.dbugVRoot;
            myroot.dbug_rootDrawingMsg.Clear();
            myroot.dbug_drawLevel = 0;
#endif
        }
        public void EndRenderPhase()
        {
            this.rootGraphic.IsInRenderPhase = false;
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
            if (centralAnimationClock != null)
            {
                centralAnimationClock.Stop();
            } 
            ClearAllChildren();
        }
    }
}