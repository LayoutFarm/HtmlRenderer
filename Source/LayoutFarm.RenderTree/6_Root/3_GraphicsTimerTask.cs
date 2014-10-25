//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using LayoutFarm.Drawing;

namespace LayoutFarm
{

    public abstract class GraphicsTimerTaskEventArgs : EventArgs
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

    public class GraphicsTimerTask
    {
        RootGraphic rootgfx;
        bool enable;
        object uniqueName;
        EventHandler<GraphicsTimerTaskEventArgs> tickHandler;
        public GraphicsTimerTask(RootGraphic rootgfx,
            object uniqueName,
            int internvalMs,
            EventHandler<GraphicsTimerTaskEventArgs> tickHandler)
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
        public void InvokeHandler(GraphicsTimerTaskEventArgs args)
        {
            this.tickHandler(this, args);
        }
    }
}