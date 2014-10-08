using System;
using System.Collections.Generic;
using System.Text;
using LayoutFarm.Drawing;

namespace LayoutFarm.UI
{
    class TimerTaskCollection
    {
        Dictionary<object, GraphicIntervalTask> graphicIntervalTasks = new Dictionary<object, GraphicIntervalTask>();
        List<GraphicIntervalTask> intervalTasksList = new List<GraphicIntervalTask>();
        RootGraphic rootgfx;
        public TimerTaskCollection(RootGraphic rootgfx)
        {
            this.rootgfx = rootgfx;
        } 
        public GraphicIntervalTask RequestGraphicInternvalTask(object uniqueName, 
            int intervalMs, 
            EventHandler<IntervalTaskEventArgs> tickhandler)
        {
            GraphicIntervalTask existingTask;
            if (!graphicIntervalTasks.TryGetValue(uniqueName, out existingTask))
            {
                existingTask = new GraphicIntervalTask(this.rootgfx, uniqueName, intervalMs, tickhandler);
                graphicIntervalTasks.Add(uniqueName, existingTask);
                intervalTasksList.Add(existingTask);
            }
            return existingTask;
        }
        public void RemoveIntervalTask(object uniqueName)
        {
            GraphicIntervalTask found;
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
        public GraphicIntervalTask GetTask(int index)
        {
            return intervalTasksList[index];
        }
    }
}