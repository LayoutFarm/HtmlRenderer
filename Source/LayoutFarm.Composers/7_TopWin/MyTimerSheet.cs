using System;
using System.Collections.Generic;
using System.Text;
using LayoutFarm.Drawing;

namespace LayoutFarm.UI
{
    class TimerTaskCollection
    {
        Dictionary<object, GraphicsTimerTask> graphicIntervalTasks = new Dictionary<object, GraphicsTimerTask>();
        List<GraphicsTimerTask> intervalTasksList = new List<GraphicsTimerTask>();
        RootGraphic rootgfx;
        public TimerTaskCollection(RootGraphic rootgfx)
        {
            this.rootgfx = rootgfx;
        } 
        public GraphicsTimerTask RequestGraphicInternvalTask(object uniqueName, 
            int intervalMs, 
            EventHandler<GraphicsTimerTaskEventArgs> tickhandler)
        {
            GraphicsTimerTask existingTask;
            if (!graphicIntervalTasks.TryGetValue(uniqueName, out existingTask))
            {
                existingTask = new GraphicsTimerTask(this.rootgfx, uniqueName, intervalMs, tickhandler);
                graphicIntervalTasks.Add(uniqueName, existingTask);
                intervalTasksList.Add(existingTask);
            }
            return existingTask;
        }
        public void RemoveIntervalTask(object uniqueName)
        {
            GraphicsTimerTask found;
            if (graphicIntervalTasks.TryGetValue(uniqueName, out found))
            {
                intervalTasksList.Remove(found);
                graphicIntervalTasks.Remove(uniqueName);
            }
        }
        public int TaskCount
        {
            get { return this.intervalTasksList.Count; }
        }
        public GraphicsTimerTask GetTask(int index)
        {
            return intervalTasksList[index];
        }
    }
}