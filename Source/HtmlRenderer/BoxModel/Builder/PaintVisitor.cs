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
        Stack<RectangleF> viewportBounds = new Stack<RectangleF>();
        Stack<RectangleF> clipStacks = new Stack<RectangleF>();

        PointF htmlContainerScrollOffset;
        HtmlContainer container;
        IGraphics ig;

        RectangleF latestClip = new RectangleF(0, 0, CssBoxConst.MAX_RIGHT, float.MaxValue);

        public PaintVisitor(HtmlContainer container, IGraphics ig)
        {
            this.container = container;
            this.htmlContainerScrollOffset = container.ScrollOffset;
            this.ig = ig;
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

        public void PushBound(float x, float y, float w, float h)
        {
            viewportBounds.Push(new RectangleF(x, y, w, h));
        }
        public void PopBound()
        {
            viewportBounds.Pop();
        }
        public RectangleF PeekGlobalViewportBound()
        {
            return this.viewportBounds.Peek();
        }
        public RectangleF PeekLocalViewportBound()
        {
            RectangleF bound = this.viewportBounds.Peek();
            bound.Offset(-ig.CanvasOriginX, -ig.CanvasOriginY);
            return bound;

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
            ig.SetClip(intersectResult);


            return !intersectResult.IsEmpty;
        }

        internal void PopLocalClipArea()
        {
            if (clipStacks.Count > 0)
            {
                RectangleF prevClip = this.latestClip = clipStacks.Pop();
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
            Color topColor = box.ActualBorderTopColor;
            Color leftColor = box.ActualBorderLeftColor;
            Color rightColor = box.ActualBorderRightColor;
            Color bottomColor = box.ActualBorderBottomColor;

            var g = this.Gfx;

            var b1 = RenderUtils.GetSolidBrush(topColor);
            BordersDrawHandler.DrawBorder(Border.Top, g, box, b1, rect);

            var b2 = RenderUtils.GetSolidBrush(leftColor);
            BordersDrawHandler.DrawBorder(Border.Left, g, box, b2, rect);

            var b3 = RenderUtils.GetSolidBrush(rightColor);
            BordersDrawHandler.DrawBorder(Border.Right, g, box, b3, rect);

            var b4 = RenderUtils.GetSolidBrush(bottomColor);
            BordersDrawHandler.DrawBorder(Border.Bottom, g, box, b4, rect);

        }
        internal void PaintBorder(CssBox box, Border border, Color solidColor, RectangleF rect)
        {
            var b = RenderUtils.GetSolidBrush(solidColor);
            BordersDrawHandler.DrawBorder(border, this.Gfx, box, b, rect);
        }
        
    }



}