
////Apache2, 2014-present, WinterDev
//using LayoutFarm.RenderBoxes;
//using PixelFarm.Drawing;
//namespace LayoutFarm
//{
//    public class TopWindowRenderBox2 : RenderBoxBase
//    {
//        public TopWindowRenderBox2(RootGraphic rootGfx, int width, int height)
//            : base(rootGfx, width, height)
//        {
//            this.IsTopWindow = true;
//            this.HasSpecificWidthAndHeight = true;
//        }
//        protected override void DrawBoxContent(DrawBoard canvas, Rectangle updateArea)
//        {
//            canvas.FillRectangle(Color.White, 0, 0, this.Width, this.Height);
//            this.DrawDefaultLayer(canvas, ref updateArea);
//        }
//        protected override PlainLayer CreateDefaultLayer()
//        {
//            throw new System.NotImplementedException();
//        }
//    }
//}