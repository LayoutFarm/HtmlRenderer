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
    public class PaintingArgs
    {
        Stack<RectangleF> viewportBounds = new Stack<RectangleF>();
        Stack<CssBox> containingBoxs = new Stack<CssBox>();
        HtmlContainer container;
        PointF htmlContainerScrollOffset;
        public PaintingArgs(HtmlContainer container)
        {
            this.container = container;
            this.htmlContainerScrollOffset = container.ScrollOffset;
        }
        //-----------------------------------------------------

        public void PushBound(float x, float y, float w, float h)
        {
            viewportBounds.Push(new RectangleF(x, y, w, h));
        }
        public void PopBound()
        {
            viewportBounds.Pop();
        }
        //-----------------------------------------------------

        public void PushContainingBox(CssBox containingBox)
        {
            this.containingBoxs.Push(containingBox);
        }
        public void PopContainingBox()
        {
            this.containingBoxs.Pop();

        }
        public RectangleF LatestContaingBoxClientRect
        {
            get
            {
                return this.containingBoxs.Peek().ClientRectangle;
            }
        }
        public PointF HtmlContainerScrollOffset
        {
            get
            {
                return this.htmlContainerScrollOffset;
            }
        }
        public RectangleF PeekViewportBound()
        {
            return this.viewportBounds.Peek();
        }
    }


}