// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using PixelFarm.Drawing;
using LayoutFarm.UI;

namespace LayoutFarm.CustomWidgets
{
    public sealed class SimpleBox : EaseBox
    {
        public SimpleBox(int w, int h)
            : base(w, h)
        {
        }
        public override void Walk(UIVisitor visitor)
        {
            visitor.BeginElement(this, "simplebox");
            this.DescribeDimension(visitor);
            visitor.EndElement();
        }
    }
}