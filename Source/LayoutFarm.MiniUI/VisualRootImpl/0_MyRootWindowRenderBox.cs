//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing; 

namespace LayoutFarm.Presentation
{



    public partial class MyRootWindowRenderBox : RootWindowRenderBox
    {
        bool layoutQueueClearing = false;

        List<RenderElement> layoutQueue = new List<RenderElement>();
        List<RenderElement> layoutQueue2 = new List<RenderElement>();

        List<ToNotifySizeChangedEvent> tobeNotifySizeChangedList = new List<ToNotifySizeChangedEvent>();

        RenderRootElement visualroot;
        CanvasEventsStock eventStock = new CanvasEventsStock();
        IEventListener currentMouseUIFocus = null;


        public MyRootWindowRenderBox(
            RenderRootElement visualroot, int width, int height)
            : base(width, height)
        {
            this.visualroot = visualroot;

            centralAnimationClock = new System.Timers.Timer();
            centralAnimationClock.Interval = 40;
            centralAnimationClock.Elapsed += new System.Timers.ElapsedEventHandler(centralAnimationClock_Elapsed);
            centralAnimationClock.Enabled = false;

            rootTasksTimer = new System.Timers.Timer();
            rootTasksTimer.Interval = 100;
            rootTasksTimer.Elapsed += new System.Timers.ElapsedEventHandler(rootTasksTimer_Elapsed);
            rootTasksTimer.Enabled = false;
            hoverMonitoringTask = new UIHoverMonitorTask(this, this.OnMouseHover);
#if DEBUG
            dbug_hide_objIden = true;
            dbug_Init();
#endif

        }
        public override Graphics CreateGraphics()
        {


            throw new NotImplementedException();
        }
        void rootTasksTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {


            LinkedListNode<VisualRootTimerTask> node = rootTimerTasks.First;
            while (node != null)
            {
                VisualRootTimerTask rootTask = node.Value;
                if (rootTask.Enabled)
                {
                    node.Value.Tick();
                }
                else
                {
                    tobeRemoveTasks.AddLast(node);

                }
                node = node.Next;
            }
            if (tobeRemoveTasks.Count > 0)
            {
                foreach (LinkedListNode<VisualRootTimerTask> tobeRemoveNode in tobeRemoveTasks)
                {
                    rootTimerTasks.Remove(tobeRemoveNode);
                    tobeRemoveNode.Value.IsInQueue = false;
                }
                tobeRemoveTasks.Clear();
            }
            if (rootTimerTasks.Count == 0)
            {
                rootTasksTimer.Enabled = false;
            }
        }

        void centralAnimationClock_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
        }

        public void SetAsCurrentMouseFocus(IEventListener ui)
        {
            if (this.currentMouseUIFocus != null &&
                this.currentMouseUIFocus != ui)
            {

                UIFocusEventArgs2 e = new UIFocusEventArgs2();
                e.SetWinRoot(this);
                e.ToBeFocusElement = ui;
                e.ToBeLostFocusElement = currentMouseUIFocus;
                currentMouseUIFocus = null;
            }

            this.currentMouseUIFocus = ui;
        }
#if DEBUG
        public override dbugRootElement dbugVisualRoot
        {
            get
            {
                return this.visualroot;
            }
        }
#endif
        public RenderRootElement MyVisualRoot
        {
            get
            {
                return this.visualroot;
            }
        }


        internal void AddTimerTask(VisualRootTimerTask task)
        {
            rootTimerTasks.AddLast(task);
        }
        internal void EnableTaskTimer()
        {
            if (!rootTasksTimer.Enabled)
            {
                rootTasksTimer.Enabled = true;
            }
        }

        void ChangeRootElementSize(int width, int height, VisualElementArgs vinv)
        {
            Size currentSize = this.Size;
            if (currentSize.Width != width || currentSize.Height != height)
            {
                this.SetSize(width, height, vinv);

                this.InvalidateContentArrangementFromContainerSizeChanged();
                this.TopDownReCalculateContentSize(vinv);
                this.TopDownReArrangeContentIfNeed(vinv);
            }
        }




        public override void AddToLayoutQueue(RenderElement vs)
        {
#if DEBUG
            dbugRootElement dbugVisualRoot = this.dbugVRoot;
#endif

            if (layoutQueueClearing)
            {
                if (vs.IsInLayoutQueue)
                {
                    return;
                }
                else
                {
                    vs.IsInLayoutQueue = true;
                    layoutQueue2.Add(vs);
                    return;
                }
            }

#if DEBUG
            dbugVisualRoot.dbug_PushLayoutTraceMessage(dbugRootElement.dbugMsg_ADD_TO_LAYOUT_QUEUE, vs);
#endif

            vs.IsInLayoutQueue = true;
            layoutQueue.Add(vs);
        }


        static void ClearLayoutOn(VisualElementArgs vinv, MultiLayerRenderBox contvs, int i)
        {

            switch (contvs.GetReLayoutState())
            {
                case 0:
                    {
                        if (contvs.NeedReCalculateContentSize)
                        {

#if DEBUG
                            vinv.dbug_SetInitObject(contvs);
                            vinv.dbug_StartLayoutTrace(dbugVisualElementLayoutMsg.Clear_CAL_ARR, i);
#endif
                            if (!vinv.IsInTopDownReArrangePhase)
                            {
                                MultiLayerRenderBox topMostToBeCal = FindTopMostToBeRecalculate(contvs);
                                if (topMostToBeCal != null)
                                {
                                    topMostToBeCal.TopDownReCalculateContentSize(vinv);
                                }
                            }
                            contvs.TopDownReArrangeContentIfNeed(vinv);
#if DEBUG
                            vinv.dbug_EndLayoutTrace();
#endif
                        }
                        else
                        {

#if DEBUG
                            vinv.dbug_SetInitObject(contvs);
                            vinv.dbug_StartLayoutTrace(dbugVisualElementLayoutMsg.Clear_ARR_CAL, i);
#endif
                            contvs.TopDownReArrangeContentIfNeed(vinv);
#if DEBUG
                            vinv.dbug_EndLayoutTrace();
#endif
                        }

                    } break;
                case 1:
                    {


#if DEBUG
                        vinv.dbug_SetInitObject(contvs);
                        vinv.dbug_StartLayoutTrace(dbugVisualElementLayoutMsg.Clear_CAL, i);
#endif
                        if (!vinv.IsInTopDownReArrangePhase)
                        {
                            MultiLayerRenderBox topMostToBeCal = FindTopMostToBeRecalculate(contvs);
                            if (topMostToBeCal != null)
                            {
                                topMostToBeCal.TopDownReCalculateContentSize(vinv);
                            }
                        }
                        contvs.TopDownReArrangeContentIfNeed(vinv);

#if DEBUG
                        vinv.dbug_EndLayoutTrace();
#endif

                    } break;
                case 2:
                    {

#if DEBUG
                        vinv.dbug_SetInitObject(contvs);
                        vinv.dbug_StartLayoutTrace(dbugVisualElementLayoutMsg.Clear_ARR, i);
#endif
                        contvs.TopDownReArrangeContentIfNeed(vinv);
#if DEBUG
                        vinv.dbug_EndLayoutTrace();
#endif


                    } break;

            }

        }
        public override bool IsLayoutQueueClearing
        {
            get
            {
                return this.layoutQueueClearing;
            }
        }
        public void PrepareRender()
        {
            if (this.MyVisualRoot.VisualRequestCount > 0)
            {
                MyVisualRoot.ClearVisualRequests(this);
            }

            if (this.layoutQueue.Count > 0)
            {
                ClearLayoutQueue();
            }
        }

        static MultiLayerRenderBox FindTopMostToBeRecalculate(MultiLayerRenderBox veContainerBase)
        {

#if DEBUG
            dbugVisualLayoutTracer debugVisualLay = dbugRootElement.dbugCurrentGlobalVRoot.dbug_GetLastestVisualLayoutTracer();

#endif

            if (veContainerBase.IsLayoutSuspending)
            {
#if DEBUG
                dbug_WriteInfo(debugVisualLay, dbugVisitorMessage.E_RECAL_BUB_EARLY_EXIT, veContainerBase);

#endif
                return null;
            }

            if (!veContainerBase.NeedReCalculateContentSize)
            {
#if DEBUG
                dbug_WriteInfo(debugVisualLay, dbugVisitorMessage.NOT_NEED_RECAL, veContainerBase);
#endif
                return null;
            }
#if DEBUG

            dbug_BeginNewContext(debugVisualLay, dbugVisitorMessage.E_RECAL_BUB_0, veContainerBase);
#endif


            if (veContainerBase.IsWindowRoot)
            {
#if DEBUG

                dbug_EndCurrentContext(debugVisualLay, dbugVisitorMessage.E_RECAL_BUB_1, veContainerBase);
#endif
                return veContainerBase;
            }
            else
            {

                MultiLayerRenderBox ownerContainer = veContainerBase.GetOwnerContainer();

                if (ownerContainer != null && !ownerContainer.IsLayoutSuspending)
                {

                    if (ownerContainer.HasOwner)
                    {

                        MultiLayerRenderBox found = FindTopMostToBeRecalculate(ownerContainer);
                        if (found != null)
                        {
#if DEBUG

                            dbug_EndCurrentContext(debugVisualLay, dbugVisitorMessage.E_RECAL_BUB_1, veContainerBase);
#endif
                            return found;
                        }
                        else
                        {
#if DEBUG

                            dbug_EndCurrentContext(debugVisualLay, dbugVisitorMessage.E_RECAL_BUB_1, veContainerBase);
#endif
                            return veContainerBase;
                        }

                    }
                    else
                    {
#if DEBUG

                        dbug_EndCurrentContext(debugVisualLay, dbugVisitorMessage.E_RECAL_BUB_1, veContainerBase);
#endif
                        return ownerContainer;
                    }

                }
#if DEBUG
                else
                {
                    if (ownerContainer == null)
                    {
                        dbug_WriteInfo(debugVisualLay, dbugVisitorMessage.NO_OWNER_LAY, null);
                    }
                    else if (ownerContainer.IsLayoutSuspending)
                    {
                        dbug_WriteInfo(debugVisualLay, dbugVisitorMessage.OWNER_LAYER_SUSPEND_SO_EARLY_EXIT, null);
                    }
                }
#endif
            }

#if DEBUG

            dbug_EndCurrentContext(debugVisualLay, dbugVisitorMessage.E_RECAL_BUB_1, veContainerBase);
#endif
            return null;
        }

        internal void ClearLayoutQueue()
        {



            this.layoutQueueClearing = true;

#if DEBUG
            dbugRootElement visualroot = this.dbugVRoot;
            int total = layoutQueue.Count;
            visualroot.dbug_PushLayoutTraceMessage(dbugRootElement.dbugMsg_CLEAR_LAYOUT_enter, total);
#endif

            VisualElementArgs vinv = this.GetVInv();
            if (this.NeedReCalculateContentSize || this.NeedContentArrangement)
            {
                ClearLayoutOn(vinv, this, 0);
                this.IsInLayoutQueue = false;
            }
            else
            {
            }


            int preClear = layoutQueue.Count;
            for (int i = preClear - 1; i > -1; --i)
            {

                RenderElement elem = layoutQueue[i];
                if (elem.ParentLink != null && elem.IsVisualContainerBase)
                {
                    MultiLayerRenderBox contvs = (MultiLayerRenderBox)elem;
                    ClearLayoutOn(vinv, contvs, i);
                }
                elem.IsInLayoutQueue = false;
            }

            layoutQueue.Clear();
            this.layoutQueueClearing = false;
            if (layoutQueue2.Count > 0)
            {

                int lay2Count = layoutQueue2.Count;
                for (int i = 0; i < lay2Count; ++i)
                {
                    RenderElement elem = layoutQueue2[i];
                    if (elem.NeedContentArrangement)
                    {
                        elem.ResumeLayout(vinv);
                    }
                }

                for (int i = 0; i < lay2Count; ++i)
                {
                    RenderElement elem = layoutQueue2[i];
                    elem.IsInLayoutQueue = false;
                }

                layoutQueue2.Clear();
            }

            this.FreeVInv(vinv);
#if DEBUG

            visualroot.dbug_PushLayoutTraceMessage(dbugRootElement.dbugMsg_CLEAR_LAYOUT_exit);
#endif
        }
    }

}
