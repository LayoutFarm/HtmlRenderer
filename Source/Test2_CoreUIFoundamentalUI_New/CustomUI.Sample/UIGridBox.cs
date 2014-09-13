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
    public class UIGridBox : UIBox
    {
        CustomRenderBox simpleBox;

        public UIGridBox(int width, int height)
            : base(width, height)
        {
        }
        protected override RenderElement CurrentPrimaryRenderElement
        {
            get { return this.simpleBox; }
        }
        protected override bool HasReadyRenderElement
        {
            get { return this.simpleBox != null; }
        }
        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {
            if (simpleBox == null)
            {
                simpleBox = new CustomRenderBox(this.Width, this.Height);

            }
            return simpleBox;
        }
        public override void InvalidateGraphic()
        {
            if (simpleBox != null)
            {
                simpleBox.InvalidateGraphic();
            }
        }
    }
}