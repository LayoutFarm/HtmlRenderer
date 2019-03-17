//Apache2, 2014-present, WinterDev


using System.Collections.Generic;
using PixelFarm.Drawing;
using LayoutFarm.RenderBoxes;
namespace LayoutFarm.HtmlBoxes
{
    public sealed class HtmlRenderBox : RenderBoxBase
    {
        MyHtmlVisualRoot _myHtmlVisualRoot;
        CssBox _cssBox;
        DrawboardBuffer _builtInBackBuffer;

        bool _hasAccumRect;
        Rectangle _invalidateRect;

#if DEBUG
        public bool dbugBreak;
        System.Random dbugRandom = new System.Random();
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
            if (_myHtmlVisualRoot == null) { return; }

            bool useBackbuffer = canvas.IsGpuDrawBoard;
            useBackbuffer = false;

            //... TODO: review here, check doc update here?
            _myHtmlVisualRoot.CheckDocUpdate();

            if (useBackbuffer)
            {
                PaintVisitor painter = PaintVisitorStock.GetSharedPaintVisitor(_myHtmlVisualRoot, canvas);

                if (_builtInBackBuffer == null)
                {
                    _builtInBackBuffer = painter.CreateOffscreenDrawBoard(this.Width, this.Height);
                }

#if DEBUG
                painter.dbugDrawDiagonalBox(Color.Blue, this.X, this.Y, this.Width, this.Height);
#endif
                if (!_builtInBackBuffer.IsValid)
                {
                    //painter.FillRectangle(Color.Red, 0, 0, 100, 100);//debug 
                    //painter.DrawText(i.ToString().ToCharArray(), 0, 1, new PointF(0, 0), new SizeF(100, 100)); //debug


                    float backupViewportW = painter.ViewportWidth; //backup
                    float backupViewportH = painter.ViewportHeight; //backup

                    painter.AttachTo(_builtInBackBuffer); //*** switch to builtInBackbuffer 
                    painter.SetViewportSize(this.Width, this.Height);

                    if (!_hasAccumRect)
                    {
                        _invalidateRect = new Rectangle(0, 0, Width, Height);
                    }

#if DEBUG
                    //System.Diagnostics.Debug.WriteLine("inv_rect:" + _invalidateRect + "," + painter.ToString());
#endif
                    painter.PushLocalClipArea(
                        _invalidateRect.Left, _invalidateRect.Top,
                        _invalidateRect.Width, _invalidateRect.Height);


#if DEBUG
                    //for debug , test clear with random color
                    //another useful technique to see latest clear area frame-by-frame => use random color
                    painter.Clear(Color.FromArgb(255, dbugRandom.Next(0, 255), dbugRandom.Next(0, 255), dbugRandom.Next(0, 255)));
#endif
                    //painter.Clear(Color.White);

                    _myHtmlVisualRoot.PerformPaint(painter);

                    painter.PopLocalClipArea();

                    _builtInBackBuffer.IsValid = true;
                    _hasAccumRect = false;

                    painter.AttachToNormalBuffer();//*** switch back
                    painter.SetViewportSize(backupViewportW, backupViewportH);//restore viewport size
                }

                painter.DrawImage(_builtInBackBuffer.GetImage(), 0, 0, this.Width, this.Height);

                PaintVisitorStock.ReleaseSharedPaintVisitor(painter);
            }
            else if (PreferSoftwareRenderer &&
                 canvas.IsGpuDrawBoard)
            {
                //TODO: review this again ***
                //test built-in 'shared' software rendering surface
                DrawBoard cpuDrawBoard = null;
                if ((cpuDrawBoard = canvas.GetCpuBlitDrawBoard()) != null)
                {
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
            }
            else
            {

                PaintVisitor painter = PaintVisitorStock.GetSharedPaintVisitor(_myHtmlVisualRoot, canvas);
                painter.SetViewportSize(this.Width, this.Height);
#if DEBUG
                //System.Diagnostics.Debug.WriteLine(">> 500x500");
                painter.dbugDrawDiagonalBox(Color.Blue, this.X, this.Y, this.Width, this.Height);

                //for debug , test clear with random color
                //another useful technique to see latest clear area frame-by-frame => use random color
                //painter.Clear(Color.FromArgb(255, dbugRandom.Next(0, 255), dbugRandom.Next(0, 255), dbugRandom.Next(0, 255)));
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
        protected override void OnInvalidateGraphicsNoti(bool fromMe, Rectangle totalBounds)
        {
            if (_builtInBackBuffer != null)
            {
                if (!fromMe)
                {
                    totalBounds.Offset(-this.X, this.Y);
                }

                _builtInBackBuffer.IsValid = false;

                if (!_hasAccumRect)
                {
                    _invalidateRect = totalBounds;
                    _hasAccumRect = true;
                }
                else
                {
                    _invalidateRect = Rectangle.Union(_invalidateRect, totalBounds);
                }

            }

            //base.OnInvalidateGraphicsNoti(totalBounds);//skip
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





