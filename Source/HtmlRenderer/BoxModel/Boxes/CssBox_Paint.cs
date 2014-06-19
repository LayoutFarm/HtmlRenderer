
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

    partial class CssBox
    {

        public void Paint(IGraphics g, PaintingArgs args)
        {
            if (this.CssDisplay != CssDisplay.None &&
                this.CssVisibility == Dom.CssVisibility.Visible)
            {
                //----------------
                PaintImp(g, args);

                //v2
                //----------------
                //RectangleF clip = g.GetClip();
                //RectangleF rect = args.LatestContaingBoxClientRect;


                ////rect.X -= 2;
                ////rect.Width += 2;
                //rect.Offset(args.Offset);
                //rect.Intersect(args.PeekViewportBound());

                //clip.Intersect(rect);

                //if (clip != RectangleF.Empty)
                //{
                //    PaintImp(g, args);
                //}

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

                var prevClip = RenderUtils.ClipGraphicsByOverflow(g, args);
                if (this.Overflow == CssOverflow.Hidden)
                {
                    var expectedW = this.ExpectedWidth;
                    var expectedH = this.ExpectedHeight;
                    //clip width 
                    if (expectedH > 0)
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

                if (this.CssDisplay == Dom.CssDisplay.TableCell)
                {

                }

                var viewport = args.PeekViewportBound();
                //---------------------------------------------
                if (this.CssDisplay != CssDisplay.Inline)
                {
                    var bound = this.Bounds;
                    bound.Offset(args.Offset);
                    PaintBackground(g, bound, true, true);
                    BordersDrawHandler.DrawBoxBorders(g, this, bound, true, true);
                }

                if (this.LineBoxCount > 0)
                {
                    PointF offset = args.Offset;
                    viewport.Offset(offset.X, -offset.Y);
                    float viewport_top = viewport.Top;
                    float viewport_bottom = viewport.Bottom;
                    //---------------------------------------- 
                    if (this.ContainsSelectedRun)
                    {
                        //render with *** selection concern 
                        g.OffsetCanvasOrigin(0, this.GlobalY);
                        foreach (var line in this._clientLineBoxes)
                        {
                            if (line.CachedLineBottom >= viewport_top &&
                                line.CachedLineTop <= viewport_bottom)
                            {

                                g.OffsetCanvasOrigin(0, line.CachedLineTop);
                                //----------------------------------------
                                //1.                                 
                                line.PaintBackgroundAndBorder(g, args);

                                this.HtmlContainer.SelectionRange.Draw(g, args, line.CachedLineTop, line.CacheLineHeight, offset);

                                //2.                                
                                line.PaintRuns(g, args);

                                //3. 
                                line.PaintDecoration(g, args);

#if DEBUG
                                line.dbugPaintRuns(g, args);
#endif
                                g.OffsetCanvasOrigin(0, -line.CachedLineTop);
                                //----------------------------------------
                            }
                            else
                            {

                            }
                        }
                        g.OffsetCanvasOrigin(0, -this.GlobalY);

                    }
                    else
                    {
                        //render without selection concern
                        g.OffsetCanvasOrigin(0, this.GlobalY);
                        foreach (var line in this._clientLineBoxes)
                        {
                            if (line.CachedLineBottom >= viewport_top &&
                                line.CachedLineTop <= viewport_bottom)
                            {
                                //----------------------------------------
                                g.OffsetCanvasOrigin(0, line.CachedLineTop);

                                //1. 
                                line.PaintBackgroundAndBorder(g, args);
                                //2. 
                                line.PaintRuns(g, args);
                                //3. 
                                line.PaintDecoration(g, args);
#if DEBUG

                                line.dbugPaintRuns(g, args);

#endif
                                g.OffsetCanvasOrigin(0, -line.CachedLineTop);
                                //----------------------------------------
                            }
                            else
                            {

                            }
                        }
                        g.OffsetCanvasOrigin(0, -this.GlobalY);
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