//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using LayoutFarm.Drawing;

namespace LayoutFarm.UI
{
    class MyWinTimer : WinTimer
    {
        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        public MyWinTimer()
        {
            timer.Tick += (o, s) =>
            {
                this.RaiseTick();
            };
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