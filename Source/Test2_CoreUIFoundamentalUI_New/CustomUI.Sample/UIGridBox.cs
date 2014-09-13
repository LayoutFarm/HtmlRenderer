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

                gridBox.Layers = layers;
                //1. create grid layer
                GridLayer gridLayer = new GridLayer(gridBox,
                    10, 5, GridType.UniformCell);
                layers.AddLayer(gridLayer);

                //2. add some small box to the grid
                UIButton simpleButton1 = new UIButton(10, 10);
                GridCell gridCell = gridLayer.GetCell(1, 1);
                gridCell.ContentElement = simpleButton1.GetPrimaryRenderElement(rootgfx);
            }
            return gridBox;
        }

    }
}