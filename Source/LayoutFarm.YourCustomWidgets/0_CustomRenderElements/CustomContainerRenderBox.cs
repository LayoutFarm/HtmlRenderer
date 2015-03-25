// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;

using LayoutFarm.Text;
using LayoutFarm.UI;
using LayoutFarm.RenderBoxes;

namespace LayoutFarm.CustomWidgets
{

    public class CustomContainerRenderBox : CustomRenderBox
    {

#if DEBUG
        public bool dbugBreak;
#endif
        public CustomContainerRenderBox(RootGraphic rootgfx, int width, int height)
            : base(rootgfx, width, height)
        {
            this.BackColor = Color.LightGray;
        }
        public override void ClearAllChildren()
        {
        }
        public void AddChildBox(RenderElement renderE)
        {

            if (this.Layer == null)
            {
                PlainLayer plain0 = new PlainLayer(this);
                plain0.AddChild(renderE);
                this.Layer = plain0;
            }
            else
            {
                var plainLayer = this.Layer as PlainLayer;
                if (plainLayer != null)
                {
                    plainLayer.AddChild(renderE);
                }
            }
            //VisualLayerCollection layers = this.Layers;
            //PlainLayer layer0 = null;
            //if (layers == null)
            //{
            //    layers = new VisualLayerCollection();
            //    layer0 = new PlainLayer(this);
            //    layers.AddLayer(layer0);
            //}
            //else
            //{
            //    layer0 = (PlainLayer)layers.GetLayer(0);
            //}
            //this.Layers = layers;

            //layer0.AddChild(renderE);
        }


    }



}