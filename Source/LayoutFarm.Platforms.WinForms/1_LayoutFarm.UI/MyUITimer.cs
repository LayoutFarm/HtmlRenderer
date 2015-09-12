// 2015,2014 ,Apache2, WinterDev
using System;
namespace LayoutFarm.UI
{
    class MyUITimer : UITimer
    {
        //platform specific
        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        public MyUITimer()
        {
            timer.Tick += new EventHandler(timer_Tick);
        }
        void timer_Tick(object sender, EventArgs e)
        {
            this.RaiseTick();
        }
        public override int Interval
        {
            get
            {
                return this.timer.Interval;

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