//Apache2, 2014-2017, WinterDev

using PixelFarm.Drawing;
namespace LayoutFarm.Text
{
    class CaretRenderElement : RenderElement
    {
        //implement caret for text edit
        public CaretRenderElement(RootGraphic g, int w, int h)
            : base(g, w, h)
        {
        }
        public override void CustomDrawToThisCanvas(Canvas canvas, Rectangle updateArea)
        {
        }
        public override void ResetRootGraphics(RootGraphic rootgfx)
        {
            DirectSetRootGraphics(this, rootgfx);
        }
        internal void DrawCaret(Canvas canvas, int x, int y)
        {
            canvas.FillRectangle(Color.Black, x, y, this.Width, this.Height);
        }
    }
}