//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace LayoutFarm
{
    public abstract partial class RootGraphic
    {
        public int graphicUpdateBlockCount
        {
            get;
            set;
        }
        public bool disableGraphicOutputFlush
        {
            get;
            set;
        }
    }

}