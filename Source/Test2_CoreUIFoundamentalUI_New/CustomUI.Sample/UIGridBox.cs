//2014 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using LayoutFarm.Text;
using LayoutFarm.UI;
using LayoutFarm.Grids;

namespace LayoutFarm.SampleControls
{
    public class UIGridBox : UIBox
    {
        CustomRenderBox gridBox;

        public UIGridBox(int width, int height)
            : base(width, height)
        {
        }
        protected override RenderElement CurrentPrimaryRenderElement
        {
            get { return this.gridBox; }
        }
        protected override bool HasReadyRenderElement
        {
            get { return this.gridBox != null; }
        }
        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {
            if (gridBox == null)
            {
                gridBox = new CustomRenderBox(this.Width, this.Height);
                var layers = new VisualLayerCollection();
                //1. create grid layer


            }
            return gridBox;
        }

    }
}