//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace LayoutFarm
{

    public class GraphicIntervalTask
    {
        RootGraphic rootgfx;
        bool enable;
        object uniqueName;
        EventHandler<EventArgs> tickHandler;
        public GraphicIntervalTask(RootGraphic rootgfx,
            object uniqueName,
            int internvalMs,
            EventHandler<EventArgs> tickHandler)
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
        public void InvokeHandler()
        {
            this.tickHandler(this, EventArgs.Empty);
        }
    }
}