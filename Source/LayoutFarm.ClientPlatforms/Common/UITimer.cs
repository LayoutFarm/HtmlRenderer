//Apache2, 2014-2017, WinterDev

using System;
namespace LayoutFarm.UI
{
    public abstract class UITimer
    {
        public event EventHandler Tick;
        public abstract int Interval { get; set; }
        public abstract bool Enabled { get; set; }

        protected void RaiseTick()
        {
            return;
            if (Tick != null)
            {
                Tick(this, EventArgs.Empty);
            }
        }
    }
}