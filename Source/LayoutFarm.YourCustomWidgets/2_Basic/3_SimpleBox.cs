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
    public enum BoxContentLayoutKind
    {
        Absolute,
        VerticalStack,
        HorizontalStack
    }

    public enum ContentStretch
    {
        None,
        Horizontal,
        Vertical,
        Both,
    }

    public sealed class SimpleBox : EaseBox
    {
        public SimpleBox(int w, int h)
            : base(w, h)
        {
        }
        public override void Walk(UIVisitor visitor)
        {
            visitor.BeginElement(this, "simplebox");
            this.Describe(visitor);
            //descrube child 
            visitor.EndElement();
        }
    }


}