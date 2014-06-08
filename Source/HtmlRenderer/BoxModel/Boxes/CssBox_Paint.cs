
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
    //----------------------------------------------------------------------------
    partial class CssBox
    {

        public void Paint(IGraphics g, PaintingArgs args)
        {
            if (this.CssDisplay != CssDisplay.None &&
                this.CssVisibility == Dom.CssVisibility.Visible)
            {

                RectangleF clip = g.GetClip();
                RectangleF rect = args.LatestContaingBoxClientRect;
                //rect.X -= 2;
                //rect.Width += 2;
                rect.Offset(args.HtmlContainerScrollOffset);
                rect.Intersect(args.PeekViewportBound());

                clip.Intersect(rect);

                if (clip != RectangleF.Empty)
                {
                    PaintImp(g, args);
                }

            }
        }
        public void dbugPaint(IGraphics g)
        {
#if DEBUG
            //int rectCount = this.Rectangles.Count;
            //PointF offset = HtmlContainer.ScrollOffset;
            //var bound = this.Bounds;
            //bound.Offset(offset);

            ////draw diagonal
            //g.DrawLine(Pens.Blue, bound.X, bound.Y,
            //           bound.Right, bound.Bottom);
            //g.DrawLine(Pens.Blue, bound.X, bound.Bottom,
            //            bound.Right, bound.Y);

            //g.DrawRectangle(Pens.Pink,
            //      bound.X, bound.Y, bound.Width, bound.Height);
#endif
        }
        protected virtual void PaintImp(IGraphics g, PaintingArgs args)
        {
            if (this.CssDisplay != CssDisplay.None &&
               (this.CssDisplay != CssDisplay.TableCell ||
                 EmptyCells != CssEmptyCell.Hide || !IsSpaceOrEmpty))
            {

                var prevClip = RenderUtils.ClipGraphicsByOverflow(g, this);
                if (this.Overflow == CssOverflow.Hidden)
                {
                    var actualHeight = this.ActualHeight;
                    var actualWidth = this.ActualWidth;
                    if (actualHeight > 0)
                    {
                        if (prevClip.IsEmpty)
                        {
                            prevClip = this.Bounds;
                            g.SetClip(prevClip);
                        }
                        else
                        {

                        }
                    }
                }
                PointF offset = args.HtmlContainerScrollOffset;
                var viewport = args.PeekViewportBound();

                //---------------------------------------------
                if (this.CssDisplay != CssDisplay.Inline)
                {
                    var bound = this.Bounds;
                    bound.Offset(offset);
                    PaintBackground(g, bound, true, true);
                    BordersDrawHandler.DrawBoxBorders(g, this, bound, true, true);
                }

                if (this._clientLineBoxes != null && this._clientLineBoxes.Count > 0)
                {
                    viewport.Offset(offset.X, -offset.Y);
                    float viewport_top = viewport.Top;
                    float viewport_bottom = viewport.Bottom;
                    //---------------------------------------- 
                    if (this.ContainsSelectedRun)
                    {
                        //render with selection concern 
                        foreach (var line in this._clientLineBoxes)
                        {
                            if (line.CachedLineBottom >= viewport_top &&
                                line.CachedLineTop <= viewport_bottom)
                            {
                                //----------------------------------------
                                //1.
                                line.PaintBackgroundAndBorder(g, offset);

                                this.HtmlContainer.SelectionRange.Draw(g, args, line.CachedLineTop, line.CacheLineHeight, offset);

                                //2.
                                line.PaintRuns(g, offset);
                                //3.
                                line.PaintDecoration(g, offset);
#if DEBUG
                                line.dbugPaintRuns(g, offset);
#endif
                                //----------------------------------------
                            }
                            else
                            {

                            }
                        }

                    }
                    else
                    {
                        //render without selection concern
                        foreach (var line in this._clientLineBoxes)
                        {
                            if (line.CachedLineBottom >= viewport_top &&
                                line.CachedLineTop <= viewport_bottom)
                            {
                                //----------------------------------------
                                //1.
                                line.PaintBackgroundAndBorder(g, offset);
                                //2.
                                line.PaintRuns(g, offset);
                                //3.
                                line.PaintDecoration(g, offset);
#if DEBUG
                                line.dbugPaintRuns(g, offset);
#endif
                                //----------------------------------------
                            }
                            else
                            {

                            }
                        }
                    }
                }
                else
                {

                    if (this.HasContainingBlockProperty)
                    {
                        args.PushContainingBox(this);
                        foreach (var b in this._boxes)
                        {
                            if (b.CssDisplay == CssDisplay.None)
                            {
                                continue;
                            }
                            b.Paint(g, args);
                        }
                        args.PopContainingBox();
                    }
                    else
                    {
                        //if not
                        foreach (var b in this._boxes)
                        {
                            if (b.CssDisplay == CssDisplay.None)
                            {
                                continue;
                            }
                            b.Paint(g, args);
                        }
                    }
                }
                //------------------------------------------
                //must! , 

                RenderUtils.ReturnClip(g, prevClip);
                if (_listItemBox != null)
                {
                    _listItemBox.Paint(g, args);
                }
            }
        }




    }
}