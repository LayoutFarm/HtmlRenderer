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
        int lastX, lastY;
        MiniAggCanvasRenderElement canvasRenderElement;
        List<Point> pointList = new List<Point>();
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

                this.canvasRenderElement = canvas;
                canvas.SetController(this);
            }
            return canvasRenderElement;
        }
        protected override void OnMouseDown(UIMouseEventArgs e)
        {
            //test
            //let down on this canva

            this.lastX = e.X;
            this.lastY = e.Y;
            canvasRenderElement.Painter.StrokeWidth = 1;
            canvasRenderElement.Painter.StrokeColor = PixelFarm.Agg.ColorRGBA.Black;

            pointList.Add(new Point(lastX, lastY));
            base.OnMouseDown(e);
        }
        protected override void OnMouseMove(UIMouseEventArgs e)
        {
            if (e.IsDragging)
            {
                //test
                //draw on this canvas
                this.lastX = e.X;
                this.lastY = e.Y;

                //temp fix here -> need converter
                canvasRenderElement.Painter.Clear(PixelFarm.Agg.ColorRGBA.White);
                pointList.Add(new Point(lastX, lastY));
                //clear and render again
                int j = pointList.Count;
                for (int i = 1; i < j; ++i)
                {
                    var p0 = pointList[i - 1];
                    var p1 = pointList[i];
                    canvasRenderElement.Painter.Line(
                        p0.X, p0.Y,
                        p1.X, p1.Y);
                }
                canvasRenderElement.InvalidateCanvasContent();
            }
            base.OnMouseMove(e);
        }


    }
}