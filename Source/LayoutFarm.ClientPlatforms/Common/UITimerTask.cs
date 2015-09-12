// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;


namespace LayoutFarm.UI
{

    public abstract class UITimerTask
    {

        public UITimerTask()
        {

        }
        public abstract bool Enabled
        {
            get;
            set;
        }
        public abstract bool IsInQueue
        {
            get;
            set;
        }

        public int TickInterval { get; set; }
        public virtual void Tick()
        {
        }
        public virtual void Reset()
        {
        }
    }




}