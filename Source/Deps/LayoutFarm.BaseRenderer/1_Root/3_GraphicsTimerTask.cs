//Apache2, 2014-2017, WinterDev

using System;
using PixelFarm.Drawing;
namespace LayoutFarm.RenderBoxes
{
    public abstract class GraphicsTimerTaskEventArgs : EventArgs
    {
        public int NeedUpdate
        {
            get;
            set;
        }
        public Rectangle GraphicUpdateArea
        {
            get;
            set;
        }
    }
    public enum TaskIntervalPlan
    {
        FastUpDate,
        Animation,
        CaretBlink
    }
    public class GraphicsTimerTask
    {
        RootGraphic rootgfx;
        bool enable;
        object uniqueName;
        EventHandler<GraphicsTimerTaskEventArgs> tickHandler;
        public GraphicsTimerTask(RootGraphic rootgfx,
            TaskIntervalPlan planName,
            object uniqueName,
            int internvalMs,
            EventHandler<GraphicsTimerTaskEventArgs> tickHandler)
        {
            this.PlanName = planName;
            this.uniqueName = uniqueName;
            this.enable = false;
            this.rootgfx = rootgfx;
            this.tickHandler = tickHandler;
        }

        public TaskIntervalPlan PlanName { get; private set; }
        public bool Enable
        {
            get
            {
                return this.enable;
            }
            set
            {
                this.enable = value;
            }
        }
        public void RemoveSelf()
        {
            if (this.rootgfx != null)
            {
                this.rootgfx.RemoveIntervalTask(this.uniqueName);
            }
        }
        public void InvokeHandler(GraphicsTimerTaskEventArgs args)
        {
            this.tickHandler(this, args);
        }
    }
}