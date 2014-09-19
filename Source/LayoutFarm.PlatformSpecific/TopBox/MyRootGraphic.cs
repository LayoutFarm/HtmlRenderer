//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace LayoutFarm
{

    public class MyRootGraphic : RootGraphic
    {
        List<RenderElementRequest> veReqList = new List<RenderElementRequest>();
        System.Windows.Forms.Timer graphicTimer1 = new System.Windows.Forms.Timer();

        Dictionary<object, GraphicIntervalTask> graphicIntervalTasks = new Dictionary<object, GraphicIntervalTask>();
        List<GraphicIntervalTask> intervalTasksList = new List<GraphicIntervalTask>();

        static MyTopWindowRenderBox currentTopWindowBox;

        public MyRootGraphic(int width, int height)
            : base(width, height)
        {

            graphicTimer1.Interval = 500; //300 ms
            graphicTimer1.Tick += new EventHandler(graphicTimer1_Tick);
            graphicTimer1.Enabled = true;
#if DEBUG
            dbugCurrentGlobalVRoot = this;
            dbug_Init();
#endif
        }
        public void CloseWinRoot()
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



        void graphicTimer1_Tick(object sender, EventArgs e)
        {

            //clear grahic timer
            int j = intervalTasksList.Count;
            for (int i = 0; i < j; ++i)
            {
                intervalTasksList[i].InvokeHandler();
            }
            if (j > 0 && currentTopWindowBox != null)
            {
                currentTopWindowBox.ForcePaint01();
            }

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
            int intervalMs, EventHandler<EventArgs> tickhandler)
        {
            GraphicIntervalTask existingTask;
            if (!graphicIntervalTasks.TryGetValue(uniqueName, out existingTask))
            {
                existingTask = new GraphicIntervalTask(this, uniqueName, intervalMs, tickhandler);
                graphicIntervalTasks.Add(uniqueName, existingTask);
                intervalTasksList.Add(existingTask);
            }
            return existingTask;
        }
        public override void RemoveIntervalTask(object uniqueName)
        {
            GraphicIntervalTask found;
            if (graphicIntervalTasks.TryGetValue(uniqueName, out found))
            {
                intervalTasksList.Remove(found);
                graphicIntervalTasks.Remove(uniqueName);
            }
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