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
        DrawboardBuffer _builtInBackBuffer;
        bool _useBackbuffer;
        bool _hasAccumRect;
        Rectangle _invalidateRect;

#if DEBUG
        public readonly int dbugHtmlRenderBoxId = dbugTotalId++;
        static int dbugTotalId;
#endif
        public HtmlRenderBox(RootGraphic rootgfx,
            int width, int height)
            : base(rootgfx, width, height)
        {

            NeedInvalidateRectEvent = true;
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
            if (canvas.IsGpuDrawBoard)
            {
                _useBackbuffer = true;
            }
            //_useBackbuffer = false;

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
            else if (_useBackbuffer)
            {

                PaintVisitor painter = PaintVisitorStock.GetSharedPaintVisitor(_myHtmlVisualRoot, canvas);
                Rectangle rect1 = painter.CurrentClipRect;
                //rect1.Offset(this.X, this.Y);

                if (_builtInBackBuffer == null)
                {
                    _builtInBackBuffer = painter.CreateOffscreenDrawBoard(this.Width, this.Height);
                }
                if (_builtInBackBuffer != null)
                {
                    painter.AttachTo(_builtInBackBuffer);
                }

                painter.SetViewportSize(this.Width, this.Height);
                painter.dbugDrawDiagonalBox(Color.Blue, this.X, this.Y, this.Width, this.Height);

                if (!_builtInBackBuffer.IsValid)
                {
                    //painter.FillRectangle(Color.Red, 0, 0, 100, 100);//debug 
                    //painter.DrawText(i.ToString().ToCharArray(), 0, 1, new PointF(0, 0), new SizeF(100, 100)); //debug


                    Rectangle currentClipRect = painter.GetCurrentClipRect();
                    if (_hasAccumRect)
                    {

                        _invalidateRect.OffsetY(-this.Y);
                        System.Diagnostics.Debug.WriteLine(_invalidateRect.ToString());

                        int ox2 = painter.CanvasOriginX;
                        int oy2 = painter.CanvasOriginY;

                        painter.SetCanvasOrigin(0, 0);
                        painter.SetClipRect(currentClipRect);

                        painter.PushLocalClipArea(
                            _invalidateRect.Left, _invalidateRect.Top,
                            _invalidateRect.Width, _invalidateRect.Height);
                        //painter.OffsetCanvasOrigin(-this.X, -this.Y);
                        painter.FillRectangle(Color.Red, 0, 0, this.Width, this.Height);
                        _myHtmlVisualRoot.PerformPaint(painter);
                        //painter.OffsetCanvasOrigin(this.X, this.Y);
                        painter.PopLocalClipArea();

                        painter.SetCanvasOrigin(ox2, oy2);
                        painter.SetClipRect(currentClipRect);
                    }
                    else
                    {
                        painter.OffsetCanvasOrigin(-this.X, -this.Y);
                        painter.FillRectangle(Color.Yellow, 0, 0, this.Width, this.Height);
                        _myHtmlVisualRoot.PerformPaint(painter);
                        painter.OffsetCanvasOrigin(this.X, this.Y);
                    }

                    _builtInBackBuffer.IsValid = true;
                    _hasAccumRect = false;
                }

                if (_builtInBackBuffer != null)
                {
                    painter.AttachToNormalBuffer();
                    //System.Diagnostics.Debug.WriteLine(rect1.ToString());
                    if (dbugHtmlRenderBoxId > 1)
                    {
                    }

                    //painter.FillRectangle(Color.Red, 0, 0, this.Width, this.Height);
                    //int ox2 = painter.CanvasOriginX;
                    //int oy2 = painter.CanvasOriginY;
                    //painter.SetCanvasOrigin(0, 0);
                    painter.SetClipRect(rect1);
                    //painter.SetCanvasOrigin(ox2, oy2);

                    painter.DrawImage(_builtInBackBuffer.GetImage(), this.X, this.Y, this.Width, this.Height);
                }
                PaintVisitorStock.ReleaseSharedPaintVisitor(painter);

            }
            else
            {

                PaintVisitor painter = PaintVisitorStock.GetSharedPaintVisitor(_myHtmlVisualRoot, canvas);
                painter.SetViewportSize(this.Width, this.Height);
#if DEBUG
                //System.Diagnostics.Debug.WriteLine(">> 500x500");
                painter.dbugDrawDiagonalBox(Color.Blue, this.X, this.Y, this.Width, this.Height);
#endif

                //painter.SetClipRect(new Rectangle(0, 0, 200, 200));
                _myHtmlVisualRoot.PerformPaint(painter);
#if DEBUG
                //System.Diagnostics.Debug.WriteLine("<< 500x500");
                //painter.dbugDrawDiagonalBox(Color.Blue, this.X, this.Y, this.Width, this.Height);
#endif
                PaintVisitorStock.ReleaseSharedPaintVisitor(painter);

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
        protected override void OnInvalidateParentGraphics(Rectangle totalBounds)
        {
            if (!_hasAccumRect)
            {
                _invalidateRect = totalBounds;
                _hasAccumRect = true;
            }
            else
            {
                _invalidateRect = Rectangle.Union(_invalidateRect, totalBounds);
            }
            //------
            if (_builtInBackBuffer != null)
            {
                _builtInBackBuffer.IsValid = false;
            }
            base.OnInvalidateParentGraphics(totalBounds);
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





