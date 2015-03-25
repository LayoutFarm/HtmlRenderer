// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;
using LayoutFarm.RenderBoxes;

namespace LayoutFarm.UI
{


    public sealed class MyRootGraphic : RootGraphic
    {

        List<RenderElement> layoutQueue = new List<RenderElement>();
        List<HtmlBoxes.MyHtmlContainer> htmlContainerUpdateQueue = new List<HtmlBoxes.MyHtmlContainer>();


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
            this.userInputEventAdapter = new UserInputEventAdapter(this);
            this.SubscribeGraphicsIntervalTask(normalUpdateTask,
                TaskIntervalPlan.Animation,
                20,
                (s, e) =>
                {
                    this.PrepareRender();
                    this.FlushAccumGraphics();
                });
        }
        public override bool GfxTimerEnabled
        {
            get
            {
                return this.graphicTimerTaskMan.Enabled;
            }
            set
            {
                this.graphicTimerTaskMan.Enabled = value;
            }
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
            //clear layout queue before render*** 
            this.ClearLayoutQueue();

            this.ClearRenderRequests();

            if (layoutQueue.Count == 0)
            {
                return;
            }
            ClearNotificationSizeChangeList();
        }
        void ClearNotificationSizeChangeList()
        {

        }
        public override GraphicsPlatform P
        {
            get { return graphicsPlatform; }
        }
        public override FontInfo DefaultTextEditFontInfo
        {
            get
            {
                return graphicsPlatform.TextEditFontInfo;
            }

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
        public override GraphicsTimerTask SubscribeGraphicsIntervalTask(
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

        public override void TakeKeyboardFocus(RenderElement renderElement)
        {
            var owner = renderElement.GetController() as IEventListener;
            if (owner != null)
            {
                this.userInputEventAdapter.CurrentKeyboardFocusedElement = owner;
            }

        }
        public override void AddToLayoutQueue(RenderElement renderElement)
        {

#if DEBUG
            RootGraphic dbugVisualRoot = this;
#endif
            if (renderElement.IsInLayoutQueue)
            {
                return;
            }
            renderElement.IsInLayoutQueue = true;
#if DEBUG
            dbugVisualRoot.dbug_PushLayoutTraceMessage(RootGraphic.dbugMsg_ADD_TO_LAYOUT_QUEUE, renderElement);
#endif

            renderElement.IsInLayoutQueue = true;
            layoutQueue.Add(renderElement);
        }
        public override void AddToUpdateQueue(object toupdateObj)
        {
            var htmlCont = toupdateObj as HtmlBoxes.MyHtmlContainer;
            if (htmlCont != null && !htmlCont.IsInUpdateQueue)
            {
                htmlCont.IsInUpdateQueue = true;
                htmlContainerUpdateQueue.Add(htmlCont);
            }
        }

        void ClearLayoutQueue()
        {

            this.LayoutQueueClearing = true;
            int j = this.layoutQueue.Count;
            for (int i = this.layoutQueue.Count - 1; i >= 0; --i)
            {
                //clear
                var renderE = this.layoutQueue[i];
                var controller = renderE.GetController() as IEventListener;
                if (controller != null)
                {
                    controller.HandleContentLayout();
                }
                renderE.IsInLayoutQueue = false;
                this.layoutQueue.RemoveAt(i);
            }
            //-------------------------------- 
            j = this.htmlContainerUpdateQueue.Count;
            for (int i = 0; i < j; ++i)
            {
                var htmlCont = htmlContainerUpdateQueue[i];
                htmlCont.IsInUpdateQueue = false;
                htmlCont.RefreshDomIfNeed();
            }
            for (int i = j - 1; i >= 0; --i)
            {
                htmlContainerUpdateQueue.RemoveAt(i);
            }
            //-------------------------------- 
            this.LayoutQueueClearing = false;
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