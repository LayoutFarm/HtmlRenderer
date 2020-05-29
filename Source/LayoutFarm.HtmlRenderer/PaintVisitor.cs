//BSD, 2014-present, WinterDev 

using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
namespace LayoutFarm.HtmlBoxes
{
    //----------------------------------------------------------------------------
    public class PaintVisitor : BoxVisitor
    {

        PointF[] _borderPoints = new PointF[4];
        HtmlVisualRoot _htmlVisualRoot;
        DrawBoard _drawBoard;

        MultiLayerStack<CssBox> _latePaintStack = new MultiLayerStack<CssBox>();

        float _viewportWidth;
        float _viewportHeight;
        Color _cssBoxSelectionColor = KnownColors.LightGray;

        public PaintVisitor()
        {
        }
        public void Bind(HtmlVisualRoot htmlVisualRoot, DrawBoard drawBoard)
        {
            _htmlVisualRoot = htmlVisualRoot;
            _drawBoard = drawBoard;
        }
        public void EnterNewDrawboardBuffer(DrawboardBuffer attachToBackbuffer)
        {
            //save  
            _drawBoard.EnterNewDrawboardBuffer(attachToBackbuffer);
        }
        public void ExitCurrentDrawboardBuffer()
        {
            _drawBoard.ExitCurrentDrawboardBuffer();
        }
        public Color CssBoxSelectionColor => _cssBoxSelectionColor;

        public void UnBind()
        {
            _drawBoard = null;
            _htmlVisualRoot = null;
        }
        public void SetViewportSize(float width, float height)
        {
            _viewportWidth = width;
            _viewportHeight = height;
        }

        public float ViewportWidth => _viewportWidth;
        public float ViewportHeight => _viewportHeight;

        public DrawBoard InnerDrawBoard => _drawBoard;


        public bool AvoidGeometryAntialias { get; set; }
        //-----------------------------------------------------

        internal float ViewportTop => 0;//?

        internal float ViewportBottom => _viewportHeight;

        //=========================================================

        UpdateArea _u = new UpdateArea();
        /// <summary>
        /// push clip area relative to (0,0) of current CssBox
        /// </summary>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <returns></returns>
        internal bool PushLocalClipArea(float w, float h)
        {
            _u.CurrentRect = _drawBoard.CurrentClipRect;
            return _drawBoard.PushClipAreaRect((int)w, (int)h, _u);
        }


#if DEBUG
        public override string ToString()
        {
            return "clip:" + _drawBoard.CurrentClipRect + ",(ox,oy)=(" + _drawBoard.OriginX + "," + _drawBoard.OriginY + ")";
        }
#endif
        internal bool PushLocalClipArea(float left, float top, float w, float h)
        {
            _u.CurrentRect = _drawBoard.CurrentClipRect;
            return _drawBoard.PushClipAreaRect((int)left, (int)top, (int)w, (int)h, _u);
        }
        internal void PopLocalClipArea()
        {
            //return;
#if DEBUG
            if (this.dbugEnableLogRecord)
            {
                dbugLogRecords.Add(new string('<', dbugIndentLevel) + dbugIndentLevel.ToString() + " pop[]");
            }
#endif
            _drawBoard.PopClipAreaRect();
        }

        internal Rectangle CurrentClipRect => _drawBoard.CurrentClipRect;
        //
        /// <summary>
        /// async request for image
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="requestFrom"></param>
        public void RequestImageAsync(ImageBinder binder,
            CssImageRun imgRun,
            object requestFrom)
        {
            if (_htmlVisualRoot != null)
            {
                _htmlVisualRoot.RaiseImageRequest(
                    binder,
                    requestFrom,
                    false);
            }
            else
            {
                binder.LazyLoadImage();
            }

            //--------------------------------------------------
            if (binder.State == BinderState.Loaded)
            {
                Image img = binder.LocalImage;
                if (img != null)
                {
                    //set real image info
                    imgRun.ImageRectangle = new Rectangle(
                        (int)imgRun.Left, (int)imgRun.Top,
                        img.Width, img.Height);
                }
            }
        }

        //
        public int CanvasOriginX => _drawBoard.OriginX;
        //
        public int CanvasOriginY => _drawBoard.OriginY;
        //
        public void SetCanvasOrigin(int x, int y)
        {
            _drawBoard.SetCanvasOrigin(x, y);
        }
        //public void OffsetCanvasOrigin(int dx, int dy)
        //{
        //    _drawBoard.OffsetCanvasOrigin(dx, dy);
        //}

        public Rectangle GetCurrentClipRect() => _drawBoard.CurrentClipRect;

        internal void PaintBorders(CssBox box, RectangleF stripArea, bool isFirstLine, bool isLastLine)
        {
            LayoutFarm.HtmlBoxes.BorderPaintHelper.DrawBoxBorders(this, box, stripArea, isFirstLine, isLastLine);
        }
        internal void PaintBorders(CssBox box, RectangleF rect)
        {
            Color topColor = box.BorderTopColor;
            Color leftColor = box.BorderLeftColor;
            Color rightColor = box.BorderRightColor;
            Color bottomColor = box.BorderBottomColor;
            DrawBoard g = this.InnerDrawBoard;
            // var b1 = RenderUtils.GetSolidBrush(topColor);
            BorderPaintHelper.DrawBorder(CssSide.Top, _borderPoints, g, box, topColor, rect);
            // var b2 = RenderUtils.GetSolidBrush(leftColor);
            BorderPaintHelper.DrawBorder(CssSide.Left, _borderPoints, g, box, leftColor, rect);
            // var b3 = RenderUtils.GetSolidBrush(rightColor);
            BorderPaintHelper.DrawBorder(CssSide.Right, _borderPoints, g, box, rightColor, rect);
            //var b4 = RenderUtils.GetSolidBrush(bottomColor);
            BorderPaintHelper.DrawBorder(CssSide.Bottom, _borderPoints, g, box, bottomColor, rect);
        }
        internal void PaintBorder(CssBox box, CssSide border, Color solidColor, RectangleF rect)
        {

            BorderPaintHelper.DrawBorder(solidColor, border, _borderPoints, _drawBoard, box, rect);
        }
        //-------------------------------------
        //painting context for canvas , svg

        public bool UseCurrentContext { get; set; }
        public Color CurrentContextFillColor { get; set; }
        public Color CurrentContextPenColor { get; set; }
        public float CurrentContextPenWidth { get; set; }
        //-------------------------------------
#if DEBUG
        /// <summary>
        /// turn on/off wire frame
        /// </summary>
        public static bool dbugDrawWireFrame = false;
        public void dbugDrawDiagonalBox(Color color, float x1, float y1, float x2, float y2)
        {
            if (!dbugDrawWireFrame)
            {
                return;
            }
            //--

            DrawBoard g = _drawBoard;
            Color prevColor = g.StrokeColor;
            g.StrokeColor = color;
            g.DrawRectangle(color, x1, y1, x2 - x1, y2 - y1);
            g.DrawLine(x1, y1, x2, y2);
            g.DrawLine(x1, y2, x2, y1);
            g.StrokeColor = prevColor;
        }
        public void dbugDrawDiagonalBox(Color color, RectangleF rect)
        {
            if (!dbugDrawWireFrame)
            {
                return;
            }

            this.dbugDrawDiagonalBox(color, rect.Left, rect.Top, rect.Right, rect.Bottom);
        }
#endif
        //-------
        //public void FillPath(GraphicsPath path, Color fillColor)
        //{
        //    _drawBoard.FillPath(fillColor, path);
        //}
        //public void DrawPath(GraphicsPath path, Color strokeColor, float strokeW)
        //{
        //    var g = _drawBoard;
        //    var prevW = g.StrokeWidth;
        //    var prevColor = g.StrokeColor;
        //    g.StrokeColor = strokeColor;
        //    g.StrokeWidth = strokeW;
        //    g.DrawPath(path);
        //    g.StrokeWidth = prevW;
        //    g.StrokeColor = prevColor;
        //}
        public void DrawLine(float x1, float y1, float x2, float y2, Color strokeColor, float strokeW)
        {
            var g = _drawBoard;
            var prevW = g.StrokeWidth;
            g.StrokeWidth = strokeW;
            var prevColor = g.StrokeColor;
            g.DrawLine(x1, y1, x2, y2);
            g.StrokeWidth = prevW;
            g.StrokeColor = prevColor;
        }
        //------
        public void Clear(Color c)
        {
            _drawBoard.Clear(c);
        }
        public void FillRectangle(Color c, float x, float y, float w, float h)
        {
            _drawBoard.FillRectangle(c, x, y, w, h);
        }
        public void SetLatestFillAsTextBackgroundColorHint() => _drawBoard.SetLatestFillAsTextBackgroundColorHint();

        public void DrawRectangle(Color c, float x, float y, float w, float h)
        {
            _drawBoard.DrawRectangle(c, x, y, w, h);
        }
        //------
        public void DrawImage(Image img, float x, float y, float w, float h)
        {
            _drawBoard.DrawImage(img, new RectangleF(x, y, w, h));
        }
        public void DrawImage(Image img, RectangleF r)
        {
            _drawBoard.DrawImage(img, r);
        }
        //---------
        public void DrawText(char[] str, int startAt, int len, PointF point, SizeF size)
        {
#if DEBUG
            dbugCounter.dbugDrawStringCount++;
#endif

            _drawBoard.DrawText(str, startAt, len, new Rectangle(
                  (int)point.X, (int)point.Y,
                  (int)size.Width, (int)size.Height), 0
                  );
        }
        public void DrawText(RenderVxFormattedString renderVxFormattedString, float x, float y)
        {
#if DEBUG
            dbugCounter.dbugDrawStringCount++;
#endif
            switch (renderVxFormattedString.State)
            {
                case RenderVxFormattedString.VxState.Ready:
                    {
                        //int ox = _drawBoard.OriginX;
                        //int oy = _drawBoard.OriginY;
                        //_drawBoard.SetCanvasOrigin((int)point.X, (int)point.Y);
                        _drawBoard.DrawRenderVx(renderVxFormattedString, x, y);
                        //_drawBoard.SetCanvasOrigin(ox, oy);//restore
                    }
                    break;
                case RenderVxFormattedString.VxState.NoStrip:
                    {
                        //put this to the update queue system
                        //(TODO: add extension method for this)

                        //GlobalRootGraphic.CurrentRootGfx.EnqueueRenderRequest(
                        //    new RenderBoxes.RenderElementRequest(
                        //        s_currentRenderE,
                        //        RenderBoxes.RequestCommand.ProcessFormattedString,
                        //        renderVxFormattedString));
                    }
                    break;
            }


        }
        public RenderVxFormattedString CreateRenderVx(char[] str, int startAt, int len)
        {
            return _drawBoard.GetPainter().CreateRenderVx(str, startAt, len);
        }

        public DrawboardBuffer CreateOffscreenDrawBoard(int width, int height)
        {
            return _drawBoard.CreateBackbuffer(width, height);
        }
#if DEBUG
        int dbugIndentLevel;
        internal bool dbugEnableLogRecord;
        internal List<string> dbugLogRecords = new List<string>();
        public enum PaintVisitorContextName
        {
            Init
        }
        public void dbugResetLogRecords()
        {
            this.dbugIndentLevel = 0;
            dbugLogRecords.Clear();
        }
        public void dbugEnterNewContext(CssBox box, PaintVisitorContextName contextName)
        {
            if (this.dbugEnableLogRecord)
            {
                var controller = CssBox.UnsafeGetController(box);
                //if (box.__aa_dbugId == 7)
                //{
                //}
                dbugLogRecords.Add(new string('>', dbugIndentLevel) + dbugIndentLevel.ToString() +
                    "[" + _drawBoard.CurrentClipRect + "] " +
                    "(" + this.CanvasOriginX + "," + this.CanvasOriginY + ") " +
                    "x:" + box.Left + ",y:" + box.Top + ",w:" + box.VisualWidth + "h:" + box.VisualHeight +
                    " " + box.ToString() + ",id:" + box.__aa_dbugId);
                dbugIndentLevel++;
            }
        }
        public void dbugExitContext()
        {
            if (this.dbugEnableLogRecord)
            {
                dbugLogRecords.Add(new string('<', dbugIndentLevel) + dbugIndentLevel.ToString());
                dbugIndentLevel--;
                if (dbugIndentLevel < 0)
                {
                    throw new NotSupportedException();
                }
            }
        }
#endif

        //-----------------------------------------------------
        internal void AddToLatePaintList(CssBox box)
        {
            _latePaintStack.AddLayerItem(box);
        }
        internal int LatePaintItemCount => _latePaintStack.CurrentLayerItemCount;

        internal CssBox GetLatePaintItem(int index) => _latePaintStack.GetItem(index);

        internal void ClearLatePaintItems() => _latePaintStack.ClearLayerItems();

        internal void EnterNewLatePaintContext() => _latePaintStack.EnterNewContext();

        internal void ExitCurrentLatePaintContext() => _latePaintStack.ExitCurrentContext();

        //-----------------------------------------------------
    }
}