//Apache2, 2014-2017, WinterDev

using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
using LayoutFarm.RenderBoxes;
namespace LayoutFarm.UI
{
    public sealed class MyRootGraphic : RootGraphic, ITopWindowEventRootProvider
    {
        List<RenderElement> layoutQueue = new List<RenderElement>();
        List<ToNotifySizeChangedEvent> tobeNotifySizeChangedList = new List<ToNotifySizeChangedEvent>();
        List<RenderElementRequest> renderRequestList = new List<RenderElementRequest>();
        GraphicsTimerTaskManager graphicTimerTaskMan;

        static object normalUpdateTask = new object();
        readonly TopWindowEventRoot topWindowEventRoot;
        readonly RenderBoxBase topWindowRenderBox;

        UIPlatform uiPlatform;
        RequestFont _defaultTextEditFont; //TODO: review here
        IFonts _ifonts;
        public MyRootGraphic(
            UIPlatform uiPlatform,
            IFonts ifonts,
            int width, int height)
            : base(width, height)
        {
            this.uiPlatform = uiPlatform;
            this._ifonts = ifonts;
            this.graphicTimerTaskMan = new GraphicsTimerTaskManager(this, uiPlatform);
            _defaultTextEditFont = new RequestFont("tahoma", 10);

#if DEBUG
            dbugCurrentGlobalVRoot = this;
            dbug_Init(null, null, null);
#endif

            //create default render box***
            this.topWindowRenderBox = new TopWindowRenderBox(this, width, height);
            this.topWindowEventRoot = new TopWindowEventRoot(this.topWindowRenderBox);
            this.SubscribeGraphicsIntervalTask(normalUpdateTask,
                TaskIntervalPlan.Animation,
                20,
                (s, e) =>
                {
                    this.PrepareRender();
                    this.FlushAccumGraphics();
                });
        }
        public override IFonts IFonts
        {
            get
            {
                return this._ifonts;
            }
        }
        public void ChangeIFonts(IFonts ifonts)
        {
            this._ifonts = ifonts;
        }
        public override RootGraphic CreateNewOne(int w, int h)
        {
            return new MyRootGraphic(this.uiPlatform, this._ifonts, w, h);
        }
        public ITopWindowEventRoot TopWinEventPortal
        {
            get { return this.topWindowEventRoot; }
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


        public override RenderBoxBase TopWindowRenderBox
        {
            get
            {
                return this.topWindowRenderBox;
            }
        }
        public override void PrepareRender()
        {
            //clear layout queue before render*** 
            this.LayoutQueueClearing = true;
            InvokeClearingBeforeRender();
            this.LayoutQueueClearing = false;
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

        public override RequestFont DefaultTextEditFontInfo
        {
            get
            {
                return _defaultTextEditFont;
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
            if (graphicTimerTaskMan != null)
            {
                this.graphicTimerTaskMan.CloseAllWorkers();
                this.graphicTimerTaskMan = null;
            }
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
                        }
                        break;
                    case RequestCommand.DoFocus:
                        {
                            //RenderElement ve = req.ve;
                            //wintop.CurrentKeyboardFocusedElement = ve;
                            //ve.InvalidateGraphic();

                        }
                        break;
                    case RequestCommand.InvalidateArea:
                        {
                            Rectangle r = (Rectangle)req.parameters;
                            this.InvalidateGraphicArea(req.ve, ref r);
                        }
                        break;
                }
            }
            renderRequestList.Clear();
        }
        public override void SetCurrentKeyboardFocus(RenderElement renderElement)
        {
            if (renderElement == null)
            {
                this.topWindowEventRoot.CurrentKeyboardFocusedElement = null;
                return;
            }

            var owner = renderElement.GetController() as IEventListener;
            if (owner != null)
            {
                this.topWindowEventRoot.CurrentKeyboardFocusedElement = owner;
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

        ITopWindowEventRoot ITopWindowEventRootProvider.EventRoot
        {
            get { return this.topWindowEventRoot; }
        }


    }


}