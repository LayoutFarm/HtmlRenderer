//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing; 
using LayoutFarm;

namespace LayoutFarm
{
    public struct ToNotifySizeChangedEvent
    {
        public int xdiff;
        public int ydiff;
        public IEventListener ui;
        public AffectedElementSideFlags affectedSideFlags;

    }

}