using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;



using LayoutFarm.Presentation;

namespace LayoutFarm.Presentation
{
    public struct ToNotifySizeChangedEvent
    {
        public int xdiff;
        public int ydiff;
        public IVisualElementUI ui;
        public AffectedElementSideFlags affectedSideFlags;

    }
     
}