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
        
        UITimer uiTimer1;

        TimerTaskCollection timerTasks;
        GraphicPlatform graphicsPlatform;

        public MyRootGraphic(GraphicPlatform graphicsPlatform,
            UITimer uiTimer, int width, int height)
            : base(width, height)
        {
            this.graphicsPlatform = graphicsPlatform;

            timerTasks = new TimerTaskCollection(this);
            this.uiTimer1 = uiTimer;

            uiTimer1.Interval = 500; //300 ms
            uiTimer1.Tick += new EventHandler(graphicTimer1_Tick);
            uiTimer1.Enabled = true;
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
            this.uiTimer1.Enabled = false;
        }

        public override void CaretStartBlink()
        {
            uiTimer1.Enabled = true;
        }
        public override void CaretStopBlink()
        {
            uiTimer1.Enabled = false;
        }


        void graphicTimer1_Tick(object sender, EventArgs e)
        {
            if (TopWindowRenderBox.CurrentTopWindowRenderBox == null)
            {
                return;
            }
            //clear grahic timer
            int j = timerTasks.TaskCount;
            MyIntervalTaskEventArgs args = GetTaskEventArgs();
            bool needForcePaint = false;
            for (int i = 0; i < j; ++i)
            {
                timerTasks.GetTask(i).InvokeHandler(args);
                if (!needForcePaint)
                {
                    needForcePaint = args.NeedUpdate;
                }
                args.ClearForReuse();
            }
            if (needForcePaint)
            {
                TopWindowRenderBox.CurrentTopWindowRenderBox.ForcePaint();
            }
            FreeTaskEventArgs(args);
        }

        Stack<MyIntervalTaskEventArgs> taskEventPools = new Stack<MyIntervalTaskEventArgs>();
        MyIntervalTaskEventArgs GetTaskEventArgs()
        {
            if (taskEventPools.Count > 0)
            {
                return taskEventPools.Pop();
            }
            else
            {
                return new MyIntervalTaskEventArgs();
            }
        }
        void FreeTaskEventArgs(MyIntervalTaskEventArgs args)
        {
            //clear for reues
            args.ClearForReuse();
            taskEventPools.Push(args);
        }


        ~MyRootGraphic()
        {

            if (this.uiTimer1 != null)
            {
                this.uiTimer1.Enabled = false;
                this.uiTimer1 = null;
            }

#if DEBUG
            dbugHitTracker.Close();
#endif
        }


        public override GraphicsTimerTask RequestGraphicsIntervalTask(object uniqueName,
            int intervalMs, EventHandler<GraphicsTimerTaskEventArgs> tickhandler)
        {
            return this.timerTasks.RequestGraphicInternvalTask(uniqueName, intervalMs, tickhandler);
        }
        public override void RemoveIntervalTask(object uniqueName)
        {
            this.timerTasks.RemoveIntervalTask(uniqueName);
        }

 


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