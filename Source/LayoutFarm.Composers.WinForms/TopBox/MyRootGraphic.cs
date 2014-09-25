//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using LayoutFarm.Drawing;

namespace LayoutFarm
{

      class MyRootGraphic : RootGraphic
    {
        List<RenderElementRequest> veReqList = new List<RenderElementRequest>();
        System.Windows.Forms.Timer graphicTimer1 = new System.Windows.Forms.Timer();

        TimerTaskCollection timerTasks;
        static MyTopWindowRenderBox currentTopWindowBox;

        public MyRootGraphic(int width, int height)
            : base(width, height)
        {
            timerTasks = new TimerTaskCollection(this);

            graphicTimer1.Interval = 500; //300 ms
            graphicTimer1.Tick += new EventHandler(graphicTimer1_Tick);
            graphicTimer1.Enabled = true;
#if DEBUG
            dbugCurrentGlobalVRoot = this;
            dbug_Init();
#endif
        }
        public override void CloseWinRoot()
        {
            this.graphicTimer1.Enabled = false;
        }
        internal static MyTopWindowRenderBox CurrentTopWindowRenderBox
        {
            get { return currentTopWindowBox; }
            set
            {
                currentTopWindowBox = value;
            }
        }
        internal void TempRunCaret()
        {
            graphicTimer1.Enabled = true;
        }
        internal void TempStopCaret()
        {
            graphicTimer1.Enabled = false;
        }


        void graphicTimer1_Tick(object sender, EventArgs e)
        {
            if (currentTopWindowBox == null)
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
                currentTopWindowBox.ForcePaint();
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

            if (this.graphicTimer1 != null)
            {
                this.graphicTimer1.Enabled = false;
                this.graphicTimer1 = null;
            }

#if DEBUG
            dbugHitTracker.Close();
#endif
        }


        public override GraphicIntervalTask RequestGraphicInternvalTask(object uniqueName,
            int intervalMs, EventHandler<IntervalTaskEventArgs> tickhandler)
        {
            return this.timerTasks.RequestGraphicInternvalTask(uniqueName, intervalMs, tickhandler);
        }
        public override void RemoveIntervalTask(object uniqueName)
        {
            this.timerTasks.RemoveIntervalTask(uniqueName);
        }



        public const int IS_SHIFT_KEYDOWN = 1 << (1 - 1);
        public const int IS_ALT_KEYDOWN = 1 << (2 - 1);
        public const int IS_CTRL_KEYDOWN = 1 << (3 - 1);


        public int VisualRequestCount
        {
            get
            {
                return veReqList.Count;
            }
        }

        public void ClearVisualRequests(TopWindowRenderBox wintop)
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
                            RenderElement ve = req.ve;
                            wintop.CurrentKeyboardFocusedElement = ve;
                            ve.InvalidateGraphic();

                        } break;
                    case RequestCommand.InvalidateArea:
                        {
                            Rectangle r = (Rectangle)req.parameters;
                            TopWindowRenderBox wintop2;
                            this.InvalidateGraphicArea(req.ve, ref r, out wintop2);
                        } break;

                }
            }
            veReqList.Clear();
        }
    }
}