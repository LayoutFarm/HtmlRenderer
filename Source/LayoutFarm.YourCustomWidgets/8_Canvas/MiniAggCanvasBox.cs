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

    public class MiniAggCanvasBox : UIBox
    {
        MiniAggCanvasRenderElement canvasRenderElement; 
        public MiniAggCanvasBox(int w, int h)
            : base(w, h)
        {


        }
        public override RenderElement CurrentPrimaryRenderElement
        {
            get { return this.canvasRenderElement; }
        }
        protected override bool HasReadyRenderElement
        {
            get
            {
                return this.canvasRenderElement != null;
            }

        }
        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {
            if (canvasRenderElement == null)
            {
                var canvas = new MiniAggCanvasRenderElement(rootgfx, this.Width, this.Height);
                canvas.HasSpecificHeight = this.HasSpecificHeight;
                canvas.HasSpecificWidth = this.HasSpecificWidth;
                canvas.SetLocation(this.Left, this.Top);

                canvas.Painter.StrokeWidth = 1;
                canvas.Painter.StrokeColor = PixelFarm.Agg.ColorRGBA.Black;


                this.canvasRenderElement = canvas;
                canvas.SetController(this);
            }
            return canvasRenderElement;
        }

        public PixelFarm.Agg.CanvasPainter Painter
        {
            get { return this.canvasRenderElement.Painter; }
        }

        protected void InvalidateCanvasContent()
        {
            this.canvasRenderElement.InvalidateCanvasContent();
        }
        public override void Walk(UIVisitor visitor)
        {
            visitor.BeginElement(this, "canvas");
            this.DescribeDimension(visitor);
            visitor.EndElement();
        }
    }
}