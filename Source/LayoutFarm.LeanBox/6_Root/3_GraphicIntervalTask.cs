//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace LayoutFarm
{

    public abstract class IntervalTaskEventArgs : EventArgs
    {
        public bool NeedUpdate
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
    public class GraphicIntervalTask
    {
        RootGraphic rootgfx;
        bool enable;
        object uniqueName;
        EventHandler<IntervalTaskEventArgs> tickHandler;
        public GraphicIntervalTask(RootGraphic rootgfx,
            object uniqueName,
            int internvalMs,
            EventHandler<IntervalTaskEventArgs> tickHandler)
        {
            this.uniqueName = uniqueName;
            this.enable = false;
            this.rootgfx = rootgfx;
            this.tickHandler = tickHandler;
        }
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
        public void InvokeHandler(IntervalTaskEventArgs args)
        {
            this.tickHandler(this, args);
        }
    }
}