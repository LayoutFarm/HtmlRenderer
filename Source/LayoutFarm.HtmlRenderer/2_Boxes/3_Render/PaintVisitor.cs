//BSD, 2014-2017, WinterDev

using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
namespace LayoutFarm.HtmlBoxes
{
    //----------------------------------------------------------------------------
    public class PaintVisitor : BoxVisitor
    {
        Stack<Rectangle> clipStacks = new Stack<Rectangle>();
        PointF[] borderPoints = new PointF[4];
        HtmlContainer htmlContainer;
        DrawBoard canvas;
        Rectangle latestClip = new Rectangle(0, 0, CssBoxConstConfig.BOX_MAX_RIGHT, CssBoxConstConfig.BOX_MAX_BOTTOM);
        MultiLayerStack<CssBox> latePaintStack = new MultiLayerStack<CssBox>();
        float viewportWidth;
        float viewportHeight;
        public PaintVisitor()
        {
        }
        public void Bind(HtmlContainer htmlCont, DrawBoard canvas)
        {
            this.htmlContainer = htmlCont;
            this.canvas = canvas;
        }

        public void UnBind()
        {
            //clear
            this.canvas = null;
            this.htmlContainer = null;
            this.clipStacks.Clear();
            this.latestClip = new Rectangle(0, 0, CssBoxConstConfig.BOX_MAX_RIGHT, CssBoxConstConfig.BOX_MAX_BOTTOM);
        }

        public void SetViewportSize(float width, float height)
        {
            this.viewportWidth = width;
            this.viewportHeight = height;
        }

        public DrawBoard InnerCanvas
        {
            get
            {
                return this.canvas;
            }
        }
        public bool AvoidGeometryAntialias
        {
            get;
            set;
        }
        //-----------------------------------------------------

        internal float ViewportTop
        {
            get { return 0; }
        }
        internal float ViewportBottom
        {
            get { return this.viewportHeight; }
        }
        //=========================================================
        /// <summary>
        /// push clip area relative to (0,0) of current CssBox
        /// </summary>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <returns></returns>
        internal bool PushLocalClipArea(float w, float h)
        {
            //return true;
            //store lastest clip 
            this.latestClip = canvas.CurrentClipRect;
            clipStacks.Push(this.latestClip);
            ////make new clip global  
            Rectangle intersectResult = Rectangle.Intersect(
                latestClip,
                new Rectangle(0, 0, (int)w, (int)h));
            this.latestClip = intersectResult;
#if DEBUG
            if (this.dbugEnableLogRecord)
            {
                canvas.DrawRectangle(Color.DeepPink,
                    intersectResult.X, intersectResult.Y,
                    intersectResult.Width, intersectResult.Height);
                logRecords.Add(new string('>', dbugIndentLevel) + dbugIndentLevel.ToString() +
                   " clip[" + intersectResult + "] ");
            }
#endif

            canvas.SetClipRect(intersectResult);
            return !intersectResult.IsEmpty;
        }
        internal void PopLocalClipArea()
        {
            //return;
#if DEBUG
            if (this.dbugEnableLogRecord)
            {
                logRecords.Add(new string('<', dbugIndentLevel) + dbugIndentLevel.ToString() + " pop[]");
            }
#endif
            if (clipStacks.Count > 0)
            {
                Rectangle prevClip = this.latestClip = clipStacks.Pop();
                canvas.SetClipRect(prevClip);
            }
            else
            {
            }
        }
        internal Rectangle CurrentClipRect
        {
            get { return this.latestClip; }
        }
        /// <summary>
        /// async request for image
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="requestFrom"></param>
        public void RequestImageAsync(ImageBinder binder, CssImageRun imgRun, object requestFrom)
        {
            if (htmlContainer != null)
            {
                this.htmlContainer.RaiseImageRequest(
                    binder,
                    requestFrom,
                    false);
            }
            else
            {
                binder.LazyLoadImage();
            }

            //--------------------------------------------------
            if (binder.State == ImageBinderState.Loaded)
            {
                Image img = binder.Image;
                if (img != null)
                {
                    //set real image info
                    imgRun.ImageRectangle = new Rectangle(
                        (int)imgRun.Left, (int)imgRun.Top,
                        img.Width, img.Height);
                }
            }
        }
        //internal void RequestImage(ImageBinder binder, CssBox requestFrom, ReadyStateChangedHandler handler)
        //{
        //    HtmlRenderer.HtmlContainer.RaiseRequestImage(
        //           this.container,
        //           binder,
        //           requestFrom,
        //           false);
        //}
        //=========================================================

        public int CanvasOriginX
        {
            get { return this.canvas.OriginX; }
        }
        public int CanvasOriginY
        {
            get { return this.canvas.OriginY; }
        }
        public void SetCanvasOrigin(int x, int y)
        {
            this.canvas.SetCanvasOrigin(x, y);
        }
        public void OffsetCanvasOrigin(int dx, int dy)
        {
            this.canvas.OffsetCanvasOrigin(dx, dy);
        }
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
            var g = this.InnerCanvas;
            // var b1 = RenderUtils.GetSolidBrush(topColor);
            BorderPaintHelper.DrawBorder(CssSide.Top, borderPoints, g, box, topColor, rect);
            // var b2 = RenderUtils.GetSolidBrush(leftColor);
            BorderPaintHelper.DrawBorder(CssSide.Left, borderPoints, g, box, leftColor, rect);
            // var b3 = RenderUtils.GetSolidBrush(rightColor);
            BorderPaintHelper.DrawBorder(CssSide.Right, borderPoints, g, box, rightColor, rect);
            //var b4 = RenderUtils.GetSolidBrush(bottomColor);
            BorderPaintHelper.DrawBorder(CssSide.Bottom, borderPoints, g, box, bottomColor, rect);
        }
        internal void PaintBorder(CssBox box, CssSide border, Color solidColor, RectangleF rect)
        {
            PointF[] borderPoints = new PointF[4];
            BorderPaintHelper.DrawBorder(solidColor, border, borderPoints, this.canvas, box, rect);
        }
        //-------------------------------------
        //painting context for canvas , svg
        Color currentContextFillColor = Color.Black;
        Color currentContextPenColor = Color.Transparent;
        float currentContextPenWidth = 1;
        public bool UseCurrentContext
        {
            get;
            set;
        }
        public Color CurrentContextFillColor
        {
            get { return this.currentContextFillColor; }
            set { this.currentContextFillColor = value; }
        }
        public Color CurrentContextPenColor
        {
            get { return this.currentContextPenColor; }
            set { this.currentContextPenColor = value; }
        }
        public float CurrentContextPenWidth
        {
            get { return this.currentContextPenWidth; }
            set { this.currentContextPenWidth = value; }
        }

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
            var g = this.canvas;
            var prevColor = g.StrokeColor;
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
            var g = this.canvas;
            this.dbugDrawDiagonalBox(color, rect.Left, rect.Top, rect.Right, rect.Bottom);
        }
#endif
        //-------
        public void FillPath(GraphicsPath path, Color fillColor)
        {
            this.canvas.FillPath(fillColor, path);
        }
        public void DrawPath(GraphicsPath path, Color strokeColor, float strokeW)
        {
            var g = this.canvas;
            var prevW = g.StrokeWidth;
            var prevColor = g.StrokeColor;
            g.StrokeColor = strokeColor;
            g.StrokeWidth = strokeW;
            g.DrawPath(path);
            g.StrokeWidth = prevW;
            g.StrokeColor = prevColor;
        }
        public void DrawLine(float x1, float y1, float x2, float y2, Color strokeColor, float strokeW)
        {
            var g = this.canvas;
            var prevW = g.StrokeWidth;
            g.StrokeWidth = strokeW;
            var prevColor = g.StrokeColor;
            g.DrawLine(x1, y1, x2, y2);
            g.StrokeWidth = prevW;
            g.StrokeColor = prevColor;
        }
        //------
        public void FillRectangle(Color c, float x, float y, float w, float h)
        {
            this.canvas.FillRectangle(c, x, y, w, h);
        }
        public void DrawRectangle(Color c, float x, float y, float w, float h)
        {
            this.canvas.DrawRectangle(c, x, y, w, h);
        }
        //------
        public void DrawImage(Image img, float x, float y, float w, float h)
        {
            this.canvas.DrawImage(img, new RectangleF(x, y, w, h));
        }
        public void DrawImage(Image img, RectangleF r)
        {
            this.canvas.DrawImage(img, r);
        }
        //---------
        public void DrawText(char[] str, int startAt, int len, PointF point, SizeF size)
        {
#if DEBUG
            dbugCounter.dbugDrawStringCount++;
#endif
            var g = this.canvas;
            g.DrawText(str, startAt, len, new Rectangle(
                  (int)point.X, (int)point.Y,
                  (int)size.Width, (int)size.Height), 0
                  );
        }
#if DEBUG
        int dbugIndentLevel;
        internal bool dbugEnableLogRecord;
        internal List<string> logRecords = new List<string>();
        public enum PaintVisitorContextName
        {
            Init
        }
        public void dbugResetLogRecords()
        {
            this.dbugIndentLevel = 0;
            logRecords.Clear();
        }
        public void dbugEnterNewContext(CssBox box, PaintVisitorContextName contextName)
        {
            if (this.dbugEnableLogRecord)
            {
                var controller = CssBox.UnsafeGetController(box);
                //if (box.__aa_dbugId == 7)
                //{
                //}
                logRecords.Add(new string('>', dbugIndentLevel) + dbugIndentLevel.ToString() +
                    "[" + this.canvas.CurrentClipRect + "] " +
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
                logRecords.Add(new string('<', dbugIndentLevel) + dbugIndentLevel.ToString());
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
            this.latePaintStack.AddLayerItem(box);
        }
        internal int LatePaintItemCount
        {
            get { return this.latePaintStack.CurrentLayerItemCount; }
        }
        internal CssBox GetLatePaintItem(int index)
        {
            return this.latePaintStack.GetItem(index);
        }
        internal void ClearLatePaintItems()
        {
            this.latePaintStack.ClearLayerItems();
        }
        internal void EnterNewLatePaintContext()
        {
            this.latePaintStack.EnterNewContext();
        }
        internal void ExitCurrentLatePaintContext()
        {
            this.latePaintStack.ExitCurrentContext();
        }
        //-----------------------------------------------------
    }
}