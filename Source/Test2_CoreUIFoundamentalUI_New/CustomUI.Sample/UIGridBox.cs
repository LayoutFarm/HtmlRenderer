//2014 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using LayoutFarm.Presentation.Text;
using LayoutFarm.Presentation.UI;

namespace LayoutFarm.Presentation.SampleControls
{
    public class UIGridBox : UIElement
    {
        CustomRenderElement simpleBox;
        public UIGridBox(int width, int height)
        {
            simpleBox = new CustomRenderElement(width, height);
        }
        public override RenderElement PrimaryRenderElement
        {
            get
            {
                return this.simpleBox;
            }
        }

    }
}