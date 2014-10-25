//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using LayoutFarm.Drawing;
using LayoutFarm.UI;

namespace LayoutFarm.UI
{


    public partial class TopWindowRenderBox : TopWindowRenderBoxBase
    {
        List<RenderElement> layoutQueue = new List<RenderElement>();
        List<RenderElement> layoutQueue2 = new List<RenderElement>();
        List<ToNotifySizeChangedEvent> tobeNotifySizeChangedList = new List<ToNotifySizeChangedEvent>();      
        RootGraphic rootGraphic;
        public TopWindowRenderBox(
            RootGraphic rootGraphic,
            int width, int height)
            : base(rootGraphic, width, height)
        {

            this.rootGraphic = rootGraphic;
#if DEBUG
            dbug_hide_objIden = true;
#endif

        }

        public static TopWindowRenderBox CurrentTopWindowRenderBox
        {
            get;
            set;
        }
        public void MakeCurrent()
        {
            CurrentTopWindowRenderBox = this;
        }

       

        public void FlushRenderRequestQueue()
        {

            LinkedListNode<UITimerTask> node = renderTaskList.First;
            while (node != null)
            {
                UITimerTask rootTask = node.Value;
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
                foreach (LinkedListNode<UITimerTask> tobeRemoveNode in tobeRemoveTasks)
                {
                    renderTaskList.Remove(tobeRemoveNode);
                    tobeRemoveNode.Value.IsInQueue = false;
                }
                tobeRemoveTasks.Clear();
            }
            if (renderTaskList.Count == 0)
            {
                throw new NotSupportedException();
                //rootTasksTimer.Enabled = false;
            }
        }
        public void UpdateAnimation()
        {
            //update some animation request
        } 
        public void CloseWinRoot()
        {
            this.rootGraphic.CloseWinRoot();
        } 
        internal void AddTimerTask(UITimerTask task)
        {
            renderTaskList.AddLast(task);
        }
        internal void EnableTaskTimer()
        {
            throw new NotSupportedException();
            //if (!rootTasksTimer.Enabled)
            //{
            //    rootTasksTimer.Enabled = true;
            //}
        }
        internal void DisableTaskTimer()
        {
        }
        
        void ChangeRootElementSize(int width, int height)
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
        public override void AddToLayoutQueue(RenderElement vs)
        {
#if DEBUG
            RootGraphic dbugVisualRoot = this.dbugVRoot;
#endif

            if (LayoutQueueClearing)
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
            dbugVisualRoot.dbug_PushLayoutTraceMessage(RootGraphic.dbugMsg_ADD_TO_LAYOUT_QUEUE, vs);
#endif

            vs.IsInLayoutQueue = true;
            layoutQueue.Add(vs);
        }


        static void ClearLayoutOn(RenderBoxBase contvs, int i)
        {

            switch (contvs.GetReLayoutState())
            {
                case 0:
                    {
                        if (contvs.NeedReCalculateContentSize)
                        {

#if DEBUG
                            vinv_dbug_SetInitObject(contvs);
                            vinv_dbug_StartLayoutTrace(dbugVisualElementLayoutMsg.Clear_CAL_ARR, i);
#endif
                            if (!vinv_IsInTopDownReArrangePhase)
                            {
                                RenderElement topMostToBeCal = FindTopMostToBeRecalculate(contvs);
                                if (topMostToBeCal != null)
                                {
                                    topMostToBeCal.TopDownReCalculateContentSize();
                                }
                            }
                            contvs.TopDownReArrangeContentIfNeed();
#if DEBUG
                            vinv_dbug_EndLayoutTrace();
#endif
                        }
                        else
                        {

#if DEBUG
                            vinv_dbug_SetInitObject(contvs);
                            vinv_dbug_StartLayoutTrace(dbugVisualElementLayoutMsg.Clear_ARR_CAL, i);
#endif
                            contvs.TopDownReArrangeContentIfNeed();
#if DEBUG
                            vinv_dbug_EndLayoutTrace();
#endif
                        }

                    } break;
                case 1:
                    {
#if DEBUG
                        vinv_dbug_SetInitObject(contvs);
                        vinv_dbug_StartLayoutTrace(dbugVisualElementLayoutMsg.Clear_CAL, i);
#endif
                        if (!vinv_IsInTopDownReArrangePhase)
                        {
                            RenderElement topMostToBeCal = FindTopMostToBeRecalculate(contvs);
                            if (topMostToBeCal != null)
                            {
                                topMostToBeCal.TopDownReCalculateContentSize();
                            }
                        }
                        contvs.TopDownReArrangeContentIfNeed();

#if DEBUG
                        vinv_dbug_EndLayoutTrace();
#endif

                    } break;
                case 2:
                    {

#if DEBUG
                        vinv_dbug_SetInitObject(contvs);
                        vinv_dbug_StartLayoutTrace(dbugVisualElementLayoutMsg.Clear_ARR, i);
#endif
                        contvs.TopDownReArrangeContentIfNeed();
#if DEBUG
                        vinv_dbug_EndLayoutTrace();
#endif


                    } break;

            }
        }
        public void PrepareRender()
        {


            this.rootGraphic.ClearRenderRequests(this);
            if (this.layoutQueue.Count > 0)
            {
                ClearLayoutQueue();
            }
        }

        static RenderElement FindTopMostToBeRecalculate(RenderElement veContainerBase)
        {

#if DEBUG
            dbugVisualLayoutTracer debugVisualLay = RootGraphic.dbugCurrentGlobalVRoot.dbug_GetLastestVisualLayoutTracer();

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


            if (veContainerBase.IsTopWindow)
            {
#if DEBUG

                dbug_EndCurrentContext(debugVisualLay, dbugVisitorMessage.E_RECAL_BUB_1, veContainerBase);
#endif
                return veContainerBase;
            }
            else
            {

                RenderElement ownerContainer = veContainerBase.GetOwnerRenderElement();

                if (ownerContainer != null && !ownerContainer.IsLayoutSuspending)
                {

                    if (ownerContainer.HasOwner)
                    {

                        RenderElement found = FindTopMostToBeRecalculate(ownerContainer);
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
            this.LayoutQueueClearing = true;

#if DEBUG
            RootGraphic visualroot = this.dbugVRoot;
            int total = layoutQueue.Count;
            visualroot.dbug_PushLayoutTraceMessage(RootGraphic.dbugMsg_CLEAR_LAYOUT_enter, total);
#endif


            if (this.NeedReCalculateContentSize || this.NeedContentArrangement)
            {
                ClearLayoutOn(this, 0);
                this.IsInLayoutQueue = false;
            }
            else
            {
            }
            int preClear = layoutQueue.Count;
            for (int i = preClear - 1; i > -1; --i)
            {

                RenderElement elem = layoutQueue[i];
                if (elem.ParentLink != null && elem.MayHasChild)
                {
                    RenderBoxBase contvs = (RenderBoxBase)elem;
                    ClearLayoutOn(contvs, i);
                }
                elem.IsInLayoutQueue = false;
            }

            layoutQueue.Clear();
            this.LayoutQueueClearing = false;
            if (layoutQueue2.Count > 0)
            {

                int lay2Count = layoutQueue2.Count;
                for (int i = 0; i < lay2Count; ++i)
                {
                    RenderElement elem = layoutQueue2[i];
                    if (elem.NeedContentArrangement)
                    {
                        elem.ResumeLayout();
                    }
                }

                for (int i = 0; i < lay2Count; ++i)
                {
                    RenderElement elem = layoutQueue2[i];
                    elem.IsInLayoutQueue = false;
                }

                layoutQueue2.Clear();
            }

#if DEBUG

            visualroot.dbug_PushLayoutTraceMessage(RootGraphic.dbugMsg_CLEAR_LAYOUT_exit);
#endif
        }


    }

}
