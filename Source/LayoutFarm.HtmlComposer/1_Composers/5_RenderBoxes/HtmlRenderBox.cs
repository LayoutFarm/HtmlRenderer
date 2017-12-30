//Apache2, 2014-2017, WinterDev


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
            PaintVisitor painter = PainterStock.GetSharedPainter(this.myHtmlCont, canvas);
            painter.SetViewportSize(this.Width, this.Height);
#if DEBUG
            painter.dbugDrawDiagonalBox(Color.Blue, this.X, this.Y, this.Width, this.Height);
#endif

            myHtmlCont.PerformPaint(painter);
            PainterStock.ReleaseSharedPainter(painter);
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

    static class PainterStock
    {
        internal static PaintVisitor GetSharedPainter(HtmlContainer htmlCont, DrawBoard canvas)
        {
            PaintVisitor painter = null;
            if (painterStock.Count == 0)
            {
                painter = new PaintVisitor();
            }
            else
            {
                painter = painterStock.Dequeue();
            }

            painter.Bind(htmlCont, canvas);
            return painter;
        }
        internal static void ReleaseSharedPainter(PaintVisitor p)
        {
            p.UnBind();
            painterStock.Enqueue(p);
        }
        static Queue<PaintVisitor> painterStock = new Queue<PaintVisitor>();
    }
}





