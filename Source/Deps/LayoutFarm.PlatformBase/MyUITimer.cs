//Apache2, 2014-2017, WinterDev

using System;
namespace LayoutFarm.UI
{
    class MyUITimer : UITimer
    {
        //platform specific

        System.Timers.Timer timer = new System.Timers.Timer();
        public MyUITimer()
        {
            timer.Elapsed += Timer_Elapsed;
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.RaiseTick();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            this.RaiseTick();
        }
        public override int Interval
        {
            get
            {
                return (int)this.timer.Interval;
            }
            set
            {
                this.timer.Interval = value;
            }
        }
        public override bool Enabled
        {
            get
            {
                return this.timer.Enabled;
            }
            set
            {
                this.timer.Enabled = value;
            }
        }
    }
}