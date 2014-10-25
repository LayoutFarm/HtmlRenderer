//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using LayoutFarm.Drawing;

namespace LayoutFarm.UI
{


    public class MyRootGraphic : RootGraphic
    {
        List<RenderElementRequest> veReqList = new List<RenderElementRequest>();


        GraphicsTimerTaskManager graphicTaskMan;
        GraphicPlatform graphicsPlatform;

        public MyRootGraphic(UIPlatform uiPlatform, int width, int height)
            : base(width, height)
        {

            this.graphicsPlatform = uiPlatform.GraphicsPlatform;
            graphicTaskMan = new GraphicsTimerTaskManager(this, uiPlatform);

#if DEBUG
            dbugCurrentGlobalVRoot = this;
            dbug_Init();
#endif
        }
        public override GraphicPlatform P
        {
            get { return graphicsPlatform; }
        }
        public override void ClearRenderRequests(TopWindowRenderBoxBase topwin)
        {
            if (this.VisualRequestCount > 0)
            {
                this.ClearVisualRequests(topwin);
            }
        }

        public override void CloseWinRoot()
        {
            this.graphicTaskMan.CloseAllWorkers();
            this.graphicTaskMan = null;
        }

        public override void CaretStartBlink()
        {

            graphicTaskMan.StartCaretBlinkTask();
        }
        public override void CaretStopBlink()
        {
            graphicTaskMan.StopCaretBlinkTask();

        }

        ~MyRootGraphic()
        {
            if (graphicTaskMan != null)
            {
                this.graphicTaskMan.CloseAllWorkers();
                this.graphicTaskMan = null;
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
            return this.graphicTaskMan.SubscribeGraphicsTimerTask(uniqueName, planName, intervalMs, tickhandler);
        }
        public override void RemoveIntervalTask(object uniqueName)
        {
            this.graphicTaskMan.UnsubscribeTimerTask(uniqueName);
        }
        //-------------------------------------------------------------------------------
        int VisualRequestCount
        {
            get
            {
                return veReqList.Count;
            }
        }
        void ClearVisualRequests(TopWindowRenderBoxBase wintop)
        {
            int j = veReqList.Count;
            for (int i = 0; i < j; ++i)
            {
                RenderElementRequest req = veReqList[i];
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
                            TopWindowRenderBoxBase wintop2;
                            this.InvalidateGraphicArea(req.ve, ref r, out wintop2);
                        } break;

                }
            }
            veReqList.Clear();
        }
    }
}