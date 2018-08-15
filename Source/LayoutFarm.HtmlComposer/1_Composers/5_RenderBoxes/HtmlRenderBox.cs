//Apache2, 2014-present, WinterDev


using System.Collections.Generic;
using PixelFarm.Drawing;
using LayoutFarm.RenderBoxes;
namespace LayoutFarm.HtmlBoxes
{
    public class HtmlRenderBox : RenderBoxBase
    {
        MyHtmlContainer myHtmlCont;
        CssBox cssBox;

        public HtmlRenderBox(RootGraphic rootgfx,
            int width, int height)
            : base(rootgfx, width, height)
        {

             
        }

        public CssBox CssBox
        {
            get { return this.cssBox; }
        }
        public void SetHtmlContainer(MyHtmlContainer htmlCont, CssBox box)
        {
            this.myHtmlCont = htmlCont;
            this.cssBox = box;

        }
        public override void ClearAllChildren()
        {
        }
        protected override void DrawBoxContent(DrawBoard canvas, Rectangle updateArea)
        {
            //TODO: review here, 
            //
            if (myHtmlCont == null)
            {
                return;
            }

            myHtmlCont.CheckDocUpdate();
            PaintVisitor painter = PaintVisitorStock.GetSharedPaintVisitor(this.myHtmlCont, canvas);
            painter.SetViewportSize(this.Width, this.Height);
#if DEBUG
            painter.dbugDrawDiagonalBox(Color.Blue, this.X, this.Y, this.Width, this.Height);
#endif

            myHtmlCont.PerformPaint(painter);
            PaintVisitorStock.ReleaseSharedPaintVisitor(painter);
        }
        public override void ChildrenHitTestCore(HitChain hitChain)
        {
        }
        public int HtmlWidth
        {
            get { return (int)this.myHtmlCont.ActualWidth; }
        }
        public int HtmlHeight
        {
            get { return (int)this.myHtmlCont.ActualHeight; }
        }
    }

    static class PaintVisitorStock
    {
        internal static PaintVisitor GetSharedPaintVisitor(HtmlContainer htmlCont, DrawBoard canvas)
        {
            PaintVisitor painter = null;
            if (s_paintVisitorStock.Count == 0)
            {
                painter = new PaintVisitor();
            }
            else
            {
                painter = s_paintVisitorStock.Dequeue();
            }

            painter.Bind(htmlCont, canvas);
            return painter;
        }
        internal static void ReleaseSharedPaintVisitor(PaintVisitor p)
        {
            p.UnBind();
            s_paintVisitorStock.Enqueue(p);
        }
        static Queue<PaintVisitor> s_paintVisitorStock = new Queue<PaintVisitor>();
    }
}





