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
        //-----------------------------------------------------
        public HtmlContainer HtmlContainer
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

            if (intersectResult.IsEmpty)
            {

            }

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
    }


}