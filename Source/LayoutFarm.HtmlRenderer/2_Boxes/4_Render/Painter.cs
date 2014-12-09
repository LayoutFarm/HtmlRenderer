//BSD 2014, WinterDev

using System;
using System.Collections.Generic;
using LayoutFarm.Drawing;

using HtmlRenderer.Css;
using HtmlRenderer.Boxes;

namespace HtmlRenderer
{
    //----------------------------------------------------------------------------
    public class Painter : BoxVisitor
    {
        Stack<Rectangle> clipStacks = new Stack<Rectangle>();

        PointF[] borderPoints = new PointF[4];
        PointF htmlContainerScrollOffset;
        HtmlIsland visualRootBox;
        Canvas canvas;

        Rectangle latestClip = new Rectangle(0, 0, CssBoxConstConfig.BOX_MAX_RIGHT, CssBoxConstConfig.BOX_MAX_BOTTOM);

        float physicalViewportWidth;
        float physicalViewportHeight;
        float physicalViewportX;
        float physicalViewportY;

        bool aviodGeometyAntialias;
        public Painter(HtmlIsland container, Canvas ig)
        {
            this.visualRootBox = container;
            this.htmlContainerScrollOffset = container.ScrollOffset;
            this.aviodGeometyAntialias = container.AvoidGeometryAntialias;
            this.canvas = ig;
        }

        public GraphicsPlatform Platform
        {
            get { return this.canvas.Platform; }
        }
        internal void SetPhysicalViewportBound(float x, float y, float width, float height)
        {
            this.physicalViewportX = x;
            this.physicalViewportY = y;
            this.physicalViewportWidth = width;
            this.physicalViewportHeight = height;
        }

        public Canvas InnerCanvas
        {
            get
            {
                return this.canvas;
            }
        }
        internal bool AvoidGeometryAntialias
        {
            get { return this.aviodGeometyAntialias; }
        }
        //-----------------------------------------------------

        internal float LocalViewportTop
        {
            get { return this.physicalViewportY - canvas.CanvasOriginY; }
        }
        internal float LocalViewportBottom
        {
            get { return (this.physicalViewportY + this.physicalViewportHeight) - canvas.CanvasOriginY; }
        }

        public PointF Offset
        {
            get { return this.htmlContainerScrollOffset; }
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

            //store lastest clip 
            clipStacks.Push(this.latestClip);
            ////make new clip global 
            Rectangle intersectResult = Rectangle.Intersect(
                latestClip,
                new Rectangle(0, 0, (int)w, (int)h));
            this.latestClip = intersectResult;

            //ig.DrawRectangle(Pens.Red, intersectResult.X, intersectResult.Y, intersectResult.Width, intersectResult.Height);
            canvas.SetClipRect(intersectResult);
            return !intersectResult.IsEmpty;
        }
        internal void PopLocalClipArea()
        {
            if (clipStacks.Count > 0)
            {
                Rectangle prevClip = this.latestClip = clipStacks.Pop();
                //ig.DrawRectangle(Pens.Green, prevClip.X, prevClip.Y, prevClip.Width, prevClip.Height);
                canvas.SetClipRect(prevClip);

            }
        }
        /// <summary>
        /// async request for image
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="requestFrom"></param>
        public void RequestImageAsync(ImageBinder binder, CssImageRun imgRun, object requestFrom)
        {
            HtmlIsland.RaiseRequestImage(
                this.visualRootBox,
                binder,
                requestFrom,
                false);
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
            get { return this.canvas.CanvasOriginX; }
        }
        public int CanvasOriginY
        {
            get { return this.canvas.CanvasOriginY; }
        }
        public void SetCanvasOrigin(int x, int y)
        {
            this.canvas.SetCanvasOrigin(x, y);
        }
        internal void PaintBorders(CssBox box, RectangleF stripArea, bool isFirstLine, bool isLastLine)
        {
            HtmlRenderer.Boxes.BorderPaintHelper.DrawBoxBorders(this, box, stripArea, isFirstLine, isLastLine);
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
            var g = this.canvas;
            var prevColor = g.FillColor;
            g.FillColor = solidColor;
            BorderPaintHelper.DrawBorder(border, borderPoints, g, box, rect);
            g.FillColor = prevColor;

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
        public void dbugDrawDiagonalBox(Color color, float x1, float y1, float x2, float y2)
        {
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
            var g = this.canvas;
            this.dbugDrawDiagonalBox(color, rect.Left, rect.Top, rect.Right, rect.Bottom);

        }
#endif
        //-------
        public void FillPath(GraphicsPath path, Color fillColor)
        {
            var g = this.canvas;
            var prevColor = g.FillColor;

            g.FillColor = fillColor;
            g.FillPath(path);
            g.FillColor = prevColor;
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
            var g = this.canvas;
            var prevColor = g.FillColor;
            g.FillRectangle(c, x, y, w, h);
            g.FillColor = prevColor;
        }
        public void DrawRectangle(Color c, float x, float y, float w, float h)
        {
            var g = this.canvas;
            var prevColor = g.FillColor;
            g.DrawRectangle(c, x, y, w, h);
            g.FillColor = prevColor;

        }
        //------
        public void DrawImage(Image img, float x, float y, float w, float h)
        {
            var g = this.canvas;
            g.DrawImage(img, new RectangleF(x, y, w, h));
        }
        public void DrawImage(Image img, RectangleF r)
        {
            var g = this.canvas;
            g.DrawImage(img, r);

        }
        //---------
        public void DrawText(char[] str, int startAt, int len, Font font, Color color, PointF point, SizeF size)
        {

#if DEBUG
            dbugCounter.dbugDrawStringCount++;
#endif
            var g = this.canvas;
            if (color.A == 255)
            {
                g.CurrentFont = font;
                g.CurrentTextColor = color;
                g.DrawText(str, startAt, len, new Rectangle(
                    (int)point.X, (int)point.Y,
                    (int)size.Width, (int)size.Height), 0
                    );


            }
            else
            {
                g.CurrentFont = font;
                g.CurrentTextColor = color;
                g.DrawText(str, startAt, len, new Rectangle(
                    (int)point.X, (int)point.Y,
                    (int)size.Width, (int)size.Height), 0
                    );

                ////translucent / transparent text
                //g.CurrentFont = font;
                //g.CurrentTextColor = color;
                //unsafe
                //{
                //    fixed (char* startAddr = &str[0])
                //    {
                //        Win32Utils.TextOut2(_hdc, (int)Math.Round(point.X + canvasOriginX),
                //            (int)Math.Round(point.Y + canvasOriginY), (startAddr + startAt), len);
                //    }
                //}

                //DrawTransparentText(_hdc, str, font, new Point((int)Math.Round(point.X), (int)Math.Round(point.Y)), Size.Round(size), color);
            }
        }


    }



}