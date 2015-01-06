//2014,2015 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;

using LayoutFarm;

namespace LayoutFarm.UI
{
    public struct ToNotifySizeChangedEvent
    {
        public int xdiff;
        public int ydiff;
        public IEventListener ui;
        public AffectedElementSideFlags affectedSideFlags; 
    }

}