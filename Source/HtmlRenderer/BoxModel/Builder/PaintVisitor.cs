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
        PointF htmlContainerScrollOffset;
        HtmlContainer container;
        IGraphics ig;
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
        public PointF Offset
        {
            get { return this.htmlContainerScrollOffset; }
        }
    }


}