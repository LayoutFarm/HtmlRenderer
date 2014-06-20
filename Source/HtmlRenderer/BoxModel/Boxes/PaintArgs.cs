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
        CssBox latestContainingBlock;

        PointF htmlContainerScrollOffset;
        public PaintingArgs(HtmlContainer container)
        {
            this.container = container;
            this.htmlContainerScrollOffset = container.ScrollOffset;
        }
        //-----------------------------------------------------
        public HtmlContainer HtmlContainer
        {
            get { return this.container; }

        }
        public CssBox LatestContainingBlock
        {
            get
            {
                return this.latestContainingBlock;
            }
        }
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
            this.latestContainingBlock = containingBox;
            this.containingBoxs.Push(containingBox);
        }
        public void PopContainingBox()
        {
            this.containingBoxs.Pop();
            if (containingBoxs.Count == 0)
            {
                this.latestContainingBlock = null;
            }
            else
            {
                this.latestContainingBlock = this.containingBoxs.Peek();
            }
        }
        public RectangleF LatestContaingBoxClientRect
        {
            get
            {
                return this.containingBoxs.Peek().GlobalClientRectangle;
            }
        }
       
        public RectangleF PeekViewportBound()
        {
            return this.viewportBounds.Peek();
        }


        public PointF Offset
        {
            get { return this.htmlContainerScrollOffset; }
        }
    }


}