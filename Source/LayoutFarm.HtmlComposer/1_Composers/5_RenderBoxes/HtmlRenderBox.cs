//Apache2, 2014-present, WinterDev


using System.Collections.Generic;
using PixelFarm.Drawing;
using LayoutFarm.RenderBoxes;
namespace LayoutFarm.HtmlBoxes
{
    public class HtmlRenderBox : RenderBoxBase
    {
        MyHtmlVisualRoot _myHtmlCont;
        CssBox _cssBox;

        public HtmlRenderBox(RootGraphic rootgfx,
            int width, int height)
            : base(rootgfx, width, height)
        {

        }
        public CssBox CssBox
        {
            get { return this._cssBox; }
        }
        public void SetHtmlContainer(MyHtmlVisualRoot htmlCont, CssBox box)
        {
            this._myHtmlCont = htmlCont;
            this._cssBox = box;
            _myHtmlCont.RootRenderElement = this;
        }
        public override void ClearAllChildren()
        {
        }
        protected override void DrawBoxContent(DrawBoard canvas, Rectangle updateArea)
        {
            //TODO: review here, 
            //
            if (_myHtmlCont == null)
            {
                return;
            }

            _myHtmlCont.CheckDocUpdate();
            PaintVisitor painter = PaintVisitorStock.GetSharedPaintVisitor(this._myHtmlCont, canvas);
            painter.SetViewportSize(this.Width, this.Height);
#if DEBUG
            painter.dbugDrawDiagonalBox(Color.Blue, this.X, this.Y, this.Width, this.Height);
#endif

            _myHtmlCont.PerformPaint(painter);
            PaintVisitorStock.ReleaseSharedPaintVisitor(painter);
        }
        public override void ChildrenHitTestCore(HitChain hitChain)
        {
        }
        public int HtmlWidth
        {
            get { return (int)this._myHtmlCont.ActualWidth; }
        }
        public int HtmlHeight
        {
            get { return (int)this._myHtmlCont.ActualHeight; }
        }
    }

    static class PaintVisitorStock
    {
        internal static PaintVisitor GetSharedPaintVisitor(HtmlVisualRoot htmlVisualRoot, DrawBoard canvas)
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

            painter.Bind(htmlVisualRoot, canvas);
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





