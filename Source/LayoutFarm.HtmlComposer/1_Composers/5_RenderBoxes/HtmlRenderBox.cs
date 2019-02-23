//Apache2, 2014-present, WinterDev


using System.Collections.Generic;
using PixelFarm.Drawing;
using LayoutFarm.RenderBoxes;
namespace LayoutFarm.HtmlBoxes
{
    public class HtmlRenderBox : RenderBoxBase
    {
        MyHtmlVisualRoot _myHtmlVisualRoot;
        CssBox _cssBox;
        Backbuffer _builtInBackBuffer;


        public HtmlRenderBox(RootGraphic rootgfx,
            int width, int height)
            : base(rootgfx, width, height)
        {

        }
        public CssBox CssBox => _cssBox;

        public void SetHtmlVisualRoot(MyHtmlVisualRoot htmlVisualRoot, CssBox box)
        {
            _myHtmlVisualRoot = htmlVisualRoot;
            _cssBox = box;
            _myHtmlVisualRoot.RootRenderElement = this;
        }
        public override void ClearAllChildren()
        {
        }
        protected override void DrawBoxContent(DrawBoard canvas, Rectangle updateArea)
        {
            //TODO: review here, 
            //
            if (_myHtmlVisualRoot == null)
            {
                return;
            }

            _myHtmlVisualRoot.CheckDocUpdate();

            DrawBoard cpuDrawBoard = null;

            if (PreferSoftwareRenderer &&
                canvas.IsGpuDrawBoard &&
               (cpuDrawBoard = canvas.GetCpuBlitDrawBoard()) != null)
            {
                //TODO: review this again ***
                //test built-in 'shared' software rendering surface

                cpuDrawBoard.Clear(Color.White);
                PaintVisitor painter = PaintVisitorStock.GetSharedPaintVisitor(_myHtmlVisualRoot, cpuDrawBoard);

                painter.SetViewportSize(this.Width, this.Height);

#if DEBUG
                painter.dbugDrawDiagonalBox(Color.Blue, this.X, this.Y, this.Width, this.Height);
#endif


                _myHtmlVisualRoot.PerformPaint(painter);
                PaintVisitorStock.ReleaseSharedPaintVisitor(painter);

                //then copy from cpu to gpu 
                canvas.BlitFrom(cpuDrawBoard, X, Y, this.Width, this.Height, 0, 0);
            }
            else
            {
                //                PaintVisitor painter = PaintVisitorStock.GetSharedPaintVisitor(_myHtmlVisualRoot, canvas);
                //                painter.SetViewportSize(this.Width, this.Height);
                //#if DEBUG
                //                painter.dbugDrawDiagonalBox(Color.Blue, this.X, this.Y, this.Width, this.Height);
                //#endif

                //                _myHtmlVisualRoot.PerformPaint(painter);
                //                PaintVisitorStock.ReleaseSharedPaintVisitor(painter);

                //                if (i != 0)
                //                {
                //                    PaintVisitor painter = PaintVisitorStock.GetSharedPaintVisitor(_myHtmlVisualRoot, canvas);
                //                    painter.SetViewportSize(this.Width, this.Height);
                //#if DEBUG
                //                    painter.dbugDrawDiagonalBox(Color.Blue, this.X, this.Y, this.Width, this.Height);
                //#endif
                //                    //painter.FillRectangle(Color.Blue, 0, 0, 100, 100);
                //                   // painter.DrawText("B".ToCharArray(), 0, 1, new PointF(0, 0), new SizeF(100, 100));
                //                    _myHtmlVisualRoot.PerformPaint(painter);
                //                    PaintVisitorStock.ReleaseSharedPaintVisitor(painter);
                //                }
                //                else
                //                {

                PaintVisitor painter = PaintVisitorStock.GetSharedPaintVisitor(_myHtmlVisualRoot, canvas);
                if (_builtInBackBuffer == null)
                {
                    _builtInBackBuffer = painter.CreateOffscreenDrawBoard(this.Width, this.Height);
                }
                if (_builtInBackBuffer != null)
                {
                    painter.AttachTo(_builtInBackBuffer);
                }

                painter.SetViewportSize(this.Width, this.Height);
#if DEBUG
                painter.dbugDrawDiagonalBox(Color.Blue, this.X, this.Y, this.Width, this.Height);
#endif

                if (!_builtInBackBuffer.IsValid)
                {
                    painter.FillRectangle(Color.Red, 0, 0, 100, 100);
                    painter.DrawText(i.ToString().ToCharArray(), 0, 1, new PointF(0, 0), new SizeF(100, 100));
                    painter.OffsetCanvasOrigin(-this.X, -this.Y);
                    _myHtmlVisualRoot.PerformPaint(painter);
                    painter.OffsetCanvasOrigin(this.X, this.Y);
                    _builtInBackBuffer.IsValid = true;
                }

                if (_builtInBackBuffer != null)
                {
                    painter.AttachToNormalBuffer();
                    painter.DrawImage(_builtInBackBuffer.GetImage(), this.X, this.Y, this.Width, this.Height);
                }
                PaintVisitorStock.ReleaseSharedPaintVisitor(painter);
                //}

            }
        }
        public override void ChildrenHitTestCore(HitChain hitChain)
        {
        }
        //
        public int HtmlWidth => (int)_myHtmlVisualRoot.ActualWidth;
        //
        public int HtmlHeight => (int)_myHtmlVisualRoot.ActualHeight;
        //
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





