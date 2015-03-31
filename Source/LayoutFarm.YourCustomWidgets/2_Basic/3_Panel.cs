// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;

using LayoutFarm.UI;
using LayoutFarm.RenderBoxes;
namespace LayoutFarm.CustomWidgets
{
    public enum PanelLayoutKind
    {
        Absolute,
        VerticalStack,
        HorizontalStack
    }
    public enum PanelStretch
    {
        None,
        Horizontal,
        Vertical,
        Both,
    }

    public sealed class Panel : EaseBox
    {
        public Panel(int w, int h)
            : base(w, h)
        {
        }
        public override void Walk(UIVisitor visitor)
        {
            visitor.BeginElement(this, "panel");
            this.DescribeDimension(visitor);
            visitor.EndElement();
        }
    }



}