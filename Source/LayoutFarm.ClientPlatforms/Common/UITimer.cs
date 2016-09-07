//Apache2, 2014-2016, WinterDev

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
            if (Tick != null)
            {
                Tick(this, EventArgs.Empty);
            }
        }
    }
}