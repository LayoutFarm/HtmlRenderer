//Apache2, 2014-2017, WinterDev

using PixelFarm.Drawing;
using LayoutFarm.UI;
using LayoutFarm.CustomWidgets;
namespace LayoutFarm.DzBoardSample
{
    class GraphicShapeRenderElement : CustomRenderBox
    {
        public GraphicShapeRenderElement(RootGraphic root, int w, int h)
            : base(root, w, h)
        {
        }
        protected override void DrawBoxContent(DrawBoard canvas, Rectangle updateArea)
        {
            //draw this ...
            var penwidth = canvas.StrokeWidth;
            int borderWidth = 5;
            int halfBorder = borderWidth / 2;
            canvas.StrokeWidth = borderWidth;
            canvas.DrawRectangle(
                Color.OrangeRed,
                halfBorder, halfBorder,
                this.Width - borderWidth,
                this.Height - borderWidth);
            canvas.StrokeWidth = penwidth;
        }
    }
    class GraphicShapeBox : EaseBox, IDesignBox
    {
        GraphicShapeRenderElement renderShape;
        public GraphicShapeBox(int w, int h)
            : base(w, h)
        {
        }
        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {
            if (renderShape == null)
            {
                renderShape = new GraphicShapeRenderElement(rootgfx, this.Width, this.Height);
                renderShape.SetLocation(this.Left, this.Top);
                renderShape.SetController(this);
                this.SetPrimaryRenderElement(renderShape);
            }
            return this.renderShape;
        }

        public UIElement CloneDesignBox()
        {
            GraphicShapeBox newCloneShape = new GraphicShapeBox(this.Width, this.Height);
            return newCloneShape;
        }
        public void Describe(DzBoxSerializer writer)
        {
            DzBoxSerializerHelper.WriteElement(writer, this, "shapebox");
        }
        public override void Walk(UIVisitor visitor)
        {
            visitor.BeginElement(this, "gfxShape");
            this.Describe(visitor);
            visitor.EndElement();
        }
    }
}
