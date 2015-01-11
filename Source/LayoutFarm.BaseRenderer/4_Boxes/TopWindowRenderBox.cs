// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;
using LayoutFarm.RenderBoxes;

namespace LayoutFarm 
{

    public sealed class TopWindowRenderBox : RenderBoxBase
    {
        PlainLayer groundLayer;
        public TopWindowRenderBox(RootGraphic rootGfx, int width, int height)
            : base(rootGfx, width, height)
        {
            groundLayer = new PlainLayer(this);
            this.MyLayers = new VisualLayerCollection();
            this.MyLayers.AddLayer(groundLayer);

            this.IsTopWindow = true;
            this.HasSpecificSize = true;
        }
        public void AddChild(RenderElement renderE)
        {
            groundLayer.AddChild(renderE);
        }
        //---------------------------------------------------------------------------- 
        public override void ClearAllChildren()
        {
            this.groundLayer.Clear();
        }
        public PlainLayer Layer0
        {
            get { return this.groundLayer; }
        }
    }

}