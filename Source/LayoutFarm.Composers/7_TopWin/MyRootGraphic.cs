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
       
       

        List<RenderElementRequest> renderRequestList = new List<RenderElementRequest>();
        GraphicsTimerTaskManager graphicTimerTaskMan;
        GraphicsPlatform graphicsPlatform;
        

        static object normalUpdateTask = new object();

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
            this.topWindowRenderBox = CreateTopWindowRenderBox(width, height);

            this.RequestGraphicsIntervalTask(normalUpdateTask,
                TaskIntervalPlan.Animation,
                20,
                (s, e) =>
                {
                    topWindowRenderBox.InvalidateGraphics();
                });



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
        
        public override void ForcePaint()
        {
            this.paintToOutputHandler();
        }
        TopWindowRenderBox CreateTopWindowRenderBox(int w, int h)
        {
            return new MyTopWindowRenderBox(this, w, h);
        }
        public IUserEventPortal CreateUserEventPortal(TopWindowRenderBox topwin)
        {
            UserInputEventAdapter userInputEventBridge = new UserInputEventAdapter();
            userInputEventBridge.Bind(topwin);
            return userInputEventBridge;
        }
        public override GraphicsPlatform P
        {
            get { return graphicsPlatform; }
        }

        public override void ClearRenderRequests(TopWindowRenderBox topwin)
        {
            if (this.VisualRequestCount > 0)
            {
                this.ClearVisualRequests(topwin);
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
        public override GraphicsTimerTask RequestGraphicsIntervalTask(
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
        void ClearVisualRequests(TopWindowRenderBox wintop)
        {
            int j = renderRequestList.Count;
            for (int i = 0; i < j; ++i)
            {
                RenderElementRequest req = renderRequestList[i];
                switch (req.req)
                {

                    case RequestCommand.AddToWindowRoot:
                        {
                            wintop.AddChild(req.ve);

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
                            TopWindowRenderBox wintop2;
                            this.InvalidateGraphicArea(req.ve, ref r, out wintop2);
                        } break;

                }
            }
            renderRequestList.Clear();
        }
    }
}