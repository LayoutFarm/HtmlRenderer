// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;
using LayoutFarm.RenderBoxes;

namespace LayoutFarm.UI
{


    public class MyRootGraphic : RootGraphic
    {
        List<RenderElement> layoutQueue = new List<RenderElement>();
        List<RenderElement> layoutQueue2 = new List<RenderElement>();
        List<ToNotifySizeChangedEvent> tobeNotifySizeChangedList = new List<ToNotifySizeChangedEvent>();


        List<RenderElementRequest> renderRequestList = new List<RenderElementRequest>();
        GraphicsTimerTaskManager graphicTimerTaskMan;
        GraphicsPlatform graphicsPlatform;


        static object normalUpdateTask = new object();
        UserInputEventAdapter userInputEventAdapter;

        TopWindowRenderBox topWindowRenderBox;
        public MyRootGraphic(UIPlatform uiPlatform, GraphicsPlatform gfxPlatform, int width, int height)
            : base(width, height)
        {
            this.graphicsPlatform = gfxPlatform;
            this.graphicTimerTaskMan = new GraphicsTimerTaskManager(this, uiPlatform);
#if DEBUG
            dbugCurrentGlobalVRoot = this;
            dbug_Init();
#endif

            //create default  render box
            this.topWindowRenderBox = new TopWindowRenderBox(this, width, height);
            this.userInputEventAdapter = CreateUserEventPortal();
            this.SubSccribeGraphicsIntervalTask(normalUpdateTask,
                TaskIntervalPlan.Animation,
                20,
                (s, e) =>
                {
                    //topWindowRenderBox.InvalidateGraphics();
                    this.FlushAccumGraphicUpdate();
                });
        }
        public IUserEventPortal UserInputEventAdapter
        {
            get { return this.userInputEventAdapter; }
        }
        public override TopWindowRenderBox TopWindowRenderBox
        {
            get
            {
                return this.topWindowRenderBox;
            }
            protected set
            {
                this.topWindowRenderBox = value;
            }
        }
        public override void PrepareRender()
        {
            this.ClearRenderRequests();
            //clear layoutqueue
            if (layoutQueue.Count == 0)
            {
                return;
            }
            ClearLayoutQueue();
            ClearNotificationSizeChangeList();
        }
        void ClearNotificationSizeChangeList()
        {
        }
        public override void ForcePaint()
        {
            this.paintToOutputHandler();
        }

        UserInputEventAdapter CreateUserEventPortal()
        {
            UserInputEventAdapter userInputEventBridge = new UserInputEventAdapter();
            userInputEventBridge.Bind(this.TopWindowRenderBox);
            return userInputEventBridge;
        }
        public override GraphicsPlatform P
        {
            get { return graphicsPlatform; }
        }

        public override void ClearRenderRequests()
        {
            if (this.VisualRequestCount > 0)
            {
                this.ClearVisualRequests();
            }
        }

        public override void CloseWinRoot()
        {
            this.graphicTimerTaskMan.CloseAllWorkers();
            this.graphicTimerTaskMan = null;
        }

        public override void CaretStartBlink()
        {

            graphicTimerTaskMan.StartCaretBlinkTask();
        }
        public override void CaretStopBlink()
        {
            graphicTimerTaskMan.StopCaretBlinkTask();

        }

        ~MyRootGraphic()
        {
            if (graphicTimerTaskMan != null)
            {
                this.graphicTimerTaskMan.CloseAllWorkers();
                this.graphicTimerTaskMan = null;
            }


#if DEBUG
            dbugHitTracker.Close();
#endif
        }

        //-------------------------------------------------------------------------------
        public override GraphicsTimerTask SubSccribeGraphicsIntervalTask(
            object uniqueName,
            TaskIntervalPlan planName,
            int intervalMs,
            EventHandler<GraphicsTimerTaskEventArgs> tickhandler)
        {
            return this.graphicTimerTaskMan.SubscribeGraphicsTimerTask(uniqueName, planName, intervalMs, tickhandler);
        }
        public override void RemoveIntervalTask(object uniqueName)
        {
            this.graphicTimerTaskMan.UnsubscribeTimerTask(uniqueName);
        }
        //-------------------------------------------------------------------------------
        int VisualRequestCount
        {
            get
            {
                return renderRequestList.Count;
            }
        }
        void ClearVisualRequests()
        {
            int j = renderRequestList.Count;
            for (int i = 0; i < j; ++i)
            {
                RenderElementRequest req = renderRequestList[i];
                switch (req.req)
                {

                    case RequestCommand.AddToWindowRoot:
                        {
                            this.TopWindowRenderBox.AddChild(req.ve);
                        } break;
                    case RequestCommand.DoFocus:
                        {
                            //RenderElement ve = req.ve;
                            //wintop.CurrentKeyboardFocusedElement = ve;
                            //ve.InvalidateGraphic();

                        } break;
                    case RequestCommand.InvalidateArea:
                        {
                            Rectangle r = (Rectangle)req.parameters;

                            this.InvalidateGraphicArea(req.ve, ref r);
                        } break;

                }
            }
            renderRequestList.Clear();
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
                            RenderElement.dbug_SetInitObject(contvs);
                            RenderElement.dbug_StartLayoutTrace(dbugVisualElementLayoutMsg.Clear_CAL_ARR, i);
#endif
                            if (!RenderElement.IsInTopDownReArrangePhase)
                            {
                                RenderElement topMostToBeCal = FindTopMostToBeRecalculate(contvs);
                                if (topMostToBeCal != null)
                                {
                                    topMostToBeCal.TopDownReCalculateContentSize();
                                }
                            }
                            contvs.TopDownReArrangeContentIfNeed();
#if DEBUG
                            RenderElement.dbug_EndLayoutTrace();
#endif
                        }
                        else
                        {

#if DEBUG

                            RenderElement.dbug_SetInitObject(contvs);
                            RenderElement.dbug_StartLayoutTrace(dbugVisualElementLayoutMsg.Clear_ARR_CAL, i);
#endif
                            contvs.TopDownReArrangeContentIfNeed();
#if DEBUG
                            RenderElement.dbug_EndLayoutTrace();
#endif
                        }

                    } break;
                case 1:
                    {
#if DEBUG
                        RenderElement.dbug_SetInitObject(contvs);
                        RenderElement.dbug_StartLayoutTrace(dbugVisualElementLayoutMsg.Clear_CAL, i);
#endif
                        if (!RenderElement.IsInTopDownReArrangePhase)
                        {
                            RenderElement topMostToBeCal = FindTopMostToBeRecalculate(contvs);
                            if (topMostToBeCal != null)
                            {
                                topMostToBeCal.TopDownReCalculateContentSize();
                            }
                        }
                        contvs.TopDownReArrangeContentIfNeed();

#if DEBUG
                        RenderElement.dbug_EndLayoutTrace();
#endif

                    } break;
                case 2:
                    {

#if DEBUG
                        RenderElement.dbug_SetInitObject(contvs);
                        RenderElement.dbug_StartLayoutTrace(dbugVisualElementLayoutMsg.Clear_ARR, i);
#endif
                        contvs.TopDownReArrangeContentIfNeed();
#if DEBUG
                        RenderElement.dbug_EndLayoutTrace();
#endif


                    } break;

            }
        }
        static RenderElement FindTopMostToBeRecalculate(RenderElement veContainerBase)
        {

#if DEBUG
            dbugVisualLayoutTracer debugVisualLay = RootGraphic.dbugCurrentGlobalVRoot.dbug_GetLastestVisualLayoutTracer();

#endif

            if (RenderElement.IsLayoutSuspending((RenderBoxBase)veContainerBase))
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

                var ownerContainer = veContainerBase.ParentRenderElement as RenderBoxBase;

                if (ownerContainer != null && !RenderBoxBase.IsLayoutSuspending(ownerContainer))
                {

                    if (ownerContainer.HasParent)
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
                    else if (RenderElement.IsLayoutSuspending(ownerContainer))
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

        public override void AddToLayoutQueue(RenderElement renderElement)
        {

#if DEBUG

            RootGraphic dbugVisualRoot = this;
#endif

            if (LayoutQueueClearing)
            {
                if (renderElement.IsInLayoutQueue)
                {
                    return;
                }
                else
                {
                    renderElement.IsInLayoutQueue = true;
                    layoutQueue2.Add(renderElement);
                    return;
                }
            }

#if DEBUG
            dbugVisualRoot.dbug_PushLayoutTraceMessage(RootGraphic.dbugMsg_ADD_TO_LAYOUT_QUEUE, renderElement);
#endif

            renderElement.IsInLayoutQueue = true;
            layoutQueue.Add(renderElement);
        }
        void ClearLayoutQueue()
        {
            this.LayoutQueueClearing = true;

#if DEBUG
            RootGraphic visualroot = this;
            int total = layoutQueue.Count;
            visualroot.dbug_PushLayoutTraceMessage(RootGraphic.dbugMsg_CLEAR_LAYOUT_enter, total);
#endif

            var topwin = this.TopWindowRenderBox;
            if (topwin.NeedReCalculateContentSize || topwin.NeedContentArrangement)
            {
                ClearLayoutOn(topwin, 0);
                this.TopWindowRenderBox.IsInLayoutQueue = false;
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

#if DEBUG

        static void dbug_WriteInfo(dbugVisualLayoutTracer debugVisualLay, dbugVisitorMessage msg, RenderElement ve)
        {
            if (debugVisualLay != null)
            {
                debugVisualLay.WriteInfo(msg.text, ve);
            }
        }
        static void dbug_BeginNewContext(dbugVisualLayoutTracer debugVisualLay, dbugVisitorMessage msg, RenderElement ve)
        {
            if (debugVisualLay != null)
            {
                debugVisualLay.BeginNewContext(); debugVisualLay.WriteInfo(msg.text, ve);
            }
        }
        static void dbug_EndCurrentContext(dbugVisualLayoutTracer debugVisualLay, dbugVisitorMessage msg, RenderElement ve)
        {
            if (debugVisualLay != null)
            {

                debugVisualLay.WriteInfo(msg.text, ve);
                debugVisualLay.EndCurrentContext();
            }
        }
        void dbug_DumpAllVisualElementProps(dbugLayoutMsgWriter writer)
        {
            //this.dbug_DumpVisualProps(writer);
            this.TopWindowRenderBox.dbug_DumpVisualProps(writer);
            writer.Add(new dbugLayoutMsg(this.TopWindowRenderBox, "FINISH"));
        }
        public void dbugShowRenderPart(Canvas canvasPage, Rectangle updateArea)
        {

            RootGraphic visualroot = this;
            if (visualroot.dbug_ShowRootUpdateArea)
            {
                canvasPage.FillRectangle(Color.FromArgb(50, Color.Black),
                     updateArea.Left, updateArea.Top,
                        updateArea.Width - 1, updateArea.Height - 1);
                canvasPage.FillRectangle(Color.White,
                     updateArea.Left, updateArea.Top, 5, 5);
                canvasPage.DrawRectangle(Color.Yellow,
                        updateArea.Left, updateArea.Top,
                        updateArea.Width - 1, updateArea.Height - 1);

                Color c_color = canvasPage.CurrentTextColor;
                canvasPage.CurrentTextColor = Color.White;
                canvasPage.DrawText(visualroot.dbug_RootUpdateCounter.ToString().ToCharArray(), updateArea.Left, updateArea.Top);
                if (updateArea.Height > 25)
                {
                    canvasPage.DrawText(visualroot.dbug_RootUpdateCounter.ToString().ToCharArray(), updateArea.Left, updateArea.Top + (updateArea.Height - 20));
                }
                canvasPage.CurrentTextColor = c_color;
                visualroot.dbug_RootUpdateCounter++;
            }
        }

#endif
    }
}