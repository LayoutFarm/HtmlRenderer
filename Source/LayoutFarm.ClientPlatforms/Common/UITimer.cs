//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;


namespace LayoutFarm.UI
{

    public abstract class UITimer
    {   
        public event EventHandler Tick;
        public abstract int Interval { get; set; }
        public abstract bool Enabled { get; set; }

        protected void RaiseTick()
        {
            if (Tick != null)
            {
                Tick(this, EventArgs.Empty);
            }
        }
    }
}