//Apache2, 2014-2017, WinterDev

using LayoutFarm.UI;
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
                canvas.Painter.StrokeColor = PixelFarm.Drawing.Color.Black;
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
            this.Describe(visitor);
            visitor.EndElement();
        }
    }
}