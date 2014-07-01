//BSD 2014, WinterCore

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using HtmlRenderer.Entities;
using HtmlRenderer.Handlers;
using HtmlRenderer.Parse;
using HtmlRenderer.Utils;

namespace HtmlRenderer.Dom
{
    //----------------------------------------------------------------------------
    public class PaintVisitor : BoxVisitor
    {



        Stack<RectangleF> clipStacks = new Stack<RectangleF>();

        PointF htmlContainerScrollOffset;
        HtmlContainer container;
        IGraphics ig;

        RectangleF latestClip = new RectangleF(0, 0, CssBoxConst.MAX_RIGHT, CssBoxConst.MAX_BOTTOM);

        float physicalViewportWidth;
        float physicalViewportHeight;
        float physicalViewportX;
        float physicalViewportY;

        public PaintVisitor(HtmlContainer container, IGraphics ig)
        {
            this.container = container;
            this.htmlContainerScrollOffset = container.ScrollOffset;
            this.ig = ig;
        }
        internal void SetPhysicalViewportBound(float x, float y, float width, float height)
        {
            this.physicalViewportX = x;
            this.physicalViewportY = y;
            this.physicalViewportWidth = width;
            this.physicalViewportHeight = height;
        }

        internal IGraphics Gfx
        {
            get
            {
                return this.ig;
            }
        }
        internal bool AvoidGeometryAntialias
        {
            get { return this.container.AvoidGeometryAntialias; }
        }
        //-----------------------------------------------------
        internal HtmlContainer HtmlContainer
        {
            get { return this.container; }
        }
        internal float LocalViewportTop
        {
            get { return this.physicalViewportY - ig.CanvasOriginY; }
        }
        internal float LocalViewportBottom
        {
            get { return (this.physicalViewportY + this.physicalViewportHeight) - ig.CanvasOriginY; }
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
            RectangleF intersectResult = RectangleF.Intersect(
                latestClip,
                new RectangleF(0, 0, w, h));
            this.latestClip = intersectResult;

            //ig.DrawRectangle(Pens.Red, intersectResult.X, intersectResult.Y, intersectResult.Width, intersectResult.Height);
            ig.SetClip(intersectResult);
            return !intersectResult.IsEmpty;
        }

        internal void PopLocalClipArea()
        {
            if (clipStacks.Count > 0)
            {
                RectangleF prevClip = this.latestClip = clipStacks.Pop();
                //ig.DrawRectangle(Pens.Green, prevClip.X, prevClip.Y, prevClip.Width, prevClip.Height);
                ig.SetClip(prevClip);
            }
        }

        internal void RequestImage(ImageBinder binder, CssBox requestFrom)
        {
            this.container.RequestImage(binder, requestFrom);
        }
        internal void RequestImage(ImageBinder binder, CssBox requestFrom, ReadyStateChangedHandler handler)
        {
            this.container.RequestImage(binder, requestFrom, handler);
        }
        //=========================================================

        internal void PaintBorders(CssBox box, RectangleF stripArea, bool isFirstLine, bool isLastLine)
        {
            HtmlRenderer.Handlers.BordersDrawHandler.DrawBoxBorders(this, box, stripArea, isFirstLine, isLastLine);
        }
        internal void PaintBorders(CssBox box, RectangleF rect)
        {
            var spec = box.BoxSpec;
            Color topColor = spec.BorderTopColor;
            Color leftColor = spec.BorderLeftColor;
            Color rightColor = spec.BorderRightColor;
            Color bottomColor = spec.BorderBottomColor;


            var g = this.Gfx;

            var b1 = RenderUtils.GetSolidBrush(topColor);
            BordersDrawHandler.DrawBorder(RenderBorderSide.Top, g, box, b1, rect);

            var b2 = RenderUtils.GetSolidBrush(leftColor);
            BordersDrawHandler.DrawBorder(RenderBorderSide.Left, g, box, b2, rect);

            var b3 = RenderUtils.GetSolidBrush(rightColor);
            BordersDrawHandler.DrawBorder(RenderBorderSide.Right, g, box, b3, rect);

            var b4 = RenderUtils.GetSolidBrush(bottomColor);
            BordersDrawHandler.DrawBorder(RenderBorderSide.Bottom, g, box, b4, rect);

        }
        internal void PaintBorder(CssBox box, RenderBorderSide border, Color solidColor, RectangleF rect)
        {
            var b = RenderUtils.GetSolidBrush(solidColor);
            BordersDrawHandler.DrawBorder(border, this.Gfx, box, b, rect);
        }

#if DEBUG
        public void dbugDrawDiagonalBox(Pen pen, float x1, float y1, float x2, float y2)
        {
            var g = this.Gfx;
            g.DrawRectangle(pen, x1, y1, x2 - x1, y2 - y1);
            g.DrawLine(pen, x1, y1, x2, y2);
            g.DrawLine(pen, x1, y2, x2, y1);
        }
        public void dbugDrawDiagonalBox(Pen pen, RectangleF rect)
        {
            var g = this.Gfx;
            this.dbugDrawDiagonalBox(pen, rect.Left, rect.Top, rect.Right, rect.Bottom);

        }
#endif

    }



}