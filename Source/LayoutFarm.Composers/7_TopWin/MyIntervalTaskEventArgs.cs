//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using LayoutFarm.Drawing;

namespace LayoutFarm.UI
{
    class MyIntervalTaskEventArgs : IntervalTaskEventArgs
    {
        internal void ClearForReuse()
        {
            this.NeedUpdate = false;
            this.GraphicUpdateArea = Rectangle.Empty;
        }
    }
}