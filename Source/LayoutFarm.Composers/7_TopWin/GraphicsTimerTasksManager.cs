using System;
using System.Collections.Generic;
using System.Text;
using LayoutFarm.Drawing;

namespace LayoutFarm.UI
{
    class GraphicsTimerTaskManager
    {
        Dictionary<object, GraphicsTimerTask> registeredTasks = new Dictionary<object, GraphicsTimerTask>();
        List<GraphicsTimerTask> fastIntervalTaskList = new List<GraphicsTimerTask>();
        List<GraphicsTimerTask> caretIntervalTaskList = new List<GraphicsTimerTask>();

        RootGraphic rootgfx;
        UITimer uiTimer1;

        int fastPlanInterval = 25;//ms 
        int caretBlinkInterval = 500;//ms (2 fps)
        int tickAccum = 0;

        bool enableCaretPlan = true;
        public GraphicsTimerTaskManager(RootGraphic rootgfx, UIPlatform platform)
        {
            this.rootgfx = rootgfx;
            this.uiTimer1 = platform.CreateUITimer();
            //--------------------------------------
            uiTimer1.Interval = fastPlanInterval; //fast task plan
            uiTimer1.Tick += new EventHandler(graphicTimer1_Tick);
            uiTimer1.Enabled = true;
            //--------------------------------------
        }
        public void CloseAllWorkers()
        {
            this.uiTimer1.Enabled = false;
        }
        public void StartCaretBlinkTask()
        {
            enableCaretPlan = true;
        }
        public void StopCaretBlinkTask()
        {
            enableCaretPlan = false;
        }

        public GraphicsTimerTask SubscribeGraphicsTimerTask(
            object uniqueName,
            TaskIntervalPlan planName,
            int intervalMs,
            EventHandler<GraphicsTimerTaskEventArgs> tickhandler)
        {
            GraphicsTimerTask existingTask;
            if (!registeredTasks.TryGetValue(uniqueName, out existingTask))
            {

                existingTask = new GraphicsTimerTask(this.rootgfx, planName, uniqueName, intervalMs, tickhandler);
                registeredTasks.Add(uniqueName, existingTask);
                switch (planName)
                {
                    case TaskIntervalPlan.CaretBlink:
                        {
                            caretIntervalTaskList.Add(existingTask);
                        } break;
                    default:
                        {
                            fastIntervalTaskList.Add(existingTask);
                        } break;
                }

            }
            return existingTask;
        }
        public void UnsubscribeTimerTask(object uniqueName)
        {
            GraphicsTimerTask found;
            if (registeredTasks.TryGetValue(uniqueName, out found))
            {
                registeredTasks.Remove(uniqueName);

                switch (found.PlanName)
                {
                    case TaskIntervalPlan.CaretBlink:
                        {
                            caretIntervalTaskList.Remove(found);
                        } break;
                    default:
                        {
                            fastIntervalTaskList.Remove(found);
                        } break;
                }
            }
        }



        void graphicTimer1_Tick(object sender, EventArgs e)
        {
            if (TopWindowRenderBox.CurrentTopWindowRenderBox == null)
            {
                return;
            }
            //-------------------------------------------------
            tickAccum += fastPlanInterval;
            bool doCaretPlan = false;
            if (tickAccum > caretBlinkInterval)
            {
                tickAccum = 0;//reset
                doCaretPlan = true;
            }
            //-------------------------------------------------
            int needUpdate = 0;
            if (doCaretPlan && enableCaretPlan)
            {
                //-------------------------------------------------
                //1. fast and animation plan
                //------------------------------------------------- 
                MyIntervalTaskEventArgs args = GetTaskEventArgs();
                int j = this.fastIntervalTaskList.Count;
                if (j > 0)
                {                       
                    for (int i = 0; i < j; ++i)
                    {
                        fastIntervalTaskList[i].InvokeHandler(args);
                        needUpdate |= args.NeedUpdate;
                    }
                }
                //-------------------------------------------------
                //2. caret plan  
                //------------------------------------------------- 
                j = this.caretIntervalTaskList.Count;
                for (int i = 0; i < j; ++i)
                {
                    caretIntervalTaskList[i].InvokeHandler(args);
                    needUpdate |= args.NeedUpdate;
                }
                FreeTaskEventArgs(args);
            }
            else
            {
                int j = this.fastIntervalTaskList.Count;
                MyIntervalTaskEventArgs args = GetTaskEventArgs();
                if (j > 0)
                {
                    for (int i = 0; i < j; ++i)
                    {
                        fastIntervalTaskList[i].InvokeHandler(args);
                        needUpdate |= args.NeedUpdate;
                    }
                }
                FreeTaskEventArgs(args); 
            } 
            if (needUpdate > 0)
            {
                TopWindowRenderBox.CurrentTopWindowRenderBox.ForcePaint();
            }

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


    }
}