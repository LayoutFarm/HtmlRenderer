
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

        public void Paint(IGraphics g, PaintVisitor args)
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
        protected virtual void PaintImp(IGraphics g, PaintVisitor args)
        {
            if (this.CssDisplay != CssDisplay.None &&
               (this.CssDisplay != CssDisplay.TableCell ||
                 EmptyCells != CssEmptyCell.Hide || !IsSpaceOrEmpty))
            {

                bool hasPrevClip = false;
                RectangleF prevClip = RectangleF.Empty;
                if (this.Overflow == CssOverflow.Hidden)
                {

                    var expectedW = this.ExpectedWidth;
                    var expectedH = this.ExpectedHeight;
                    //clip width 
                    if (expectedH > 0)
                    {
                        hasPrevClip = true;
                        prevClip = RenderUtils.ClipGraphicsByOverflow(g, args);

                        if (prevClip.IsEmpty)
                        {
                            prevClip = new RectangleF(0, 0, this.SizeWidth, this.SizeHeight);
                            g.SetClip(prevClip);
                        }
                        else
                        {

                        }
                    }
                }

                var viewport = args.PeekGlobalViewportBound();
                //---------------------------------------------
                if (this.CssDisplay != CssDisplay.Inline)
                {
                    //var bound = this.GlobalBound;
                    RectangleF bound = new RectangleF(0, 0, this.SizeWidth, this.SizeHeight);
                    PaintBackground(g, bound, true, true);
                    BordersDrawHandler.DrawBoxBorders(g, this, bound, true, true);
                }

                if (this.LineBoxCount > 0)
                {
                    //viewport.Offset(-args.ContainerBlockGlobalX, -args.ContainerBlockGlobalY);

                    float viewport_top = viewport.Top - g.CanvasOriginY;
                    float viewport_bottom = viewport.Bottom - g.CanvasOriginY;

                    //---------------------------------------- 
                    if (this.ContainsSelectedRun)
                    {

                        //render with *** selection concern  
                        foreach (var line in this._clientLineBoxes)
                        {

                            if (line.CachedLineBottom >= viewport_top &&
                                line.CachedLineTop <= viewport_bottom)
                            {

                                g.OffsetCanvasOrigin(0, line.CachedLineTop);
                                //----------------------------------------
                                //1.                                 
                                line.PaintBackgroundAndBorder(g, args);

                                this.HtmlContainer.SelectionRange.Draw(g, args, line.CachedLineTop, line.CacheLineHeight, args.Offset);

                                //2.                                
                                line.PaintRuns(g, args);

                                //3. 
                                line.PaintDecoration(g, args);

#if DEBUG
                                line.dbugPaintRuns(g, args);
#endif
                                g.OffsetCanvasOrigin(0, -line.CachedLineTop);
                                //----------------------------------------
                                //Console.WriteLine("ok[" + line.dbugId + ",bottom=" + line.CachedLineBottom + " ,vw(top:bottom)=" +
                                //  viewport_top + ":" + viewport_bottom);
                            }
                            else
                            {
                                //Console.WriteLine("!![" + line.dbugId + ",bottom=" + line.CachedLineBottom + " ,vw(top:bottom)=" +
                                //  viewport_top + ":" + viewport_bottom);
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
                                //Console.WriteLine("ok[" + line.dbugId + ",bottom=" + line.CachedLineBottom + " ,vw(top:bottom)=" +
                                //   viewport_top + ":" + viewport_bottom);
                            }
                            else
                            {
                                //Console.WriteLine("!![" + line.dbugId + ",bottom=" + line.CachedLineBottom + " ,vw(top:bottom)=" +
                                //  viewport_top + ":" + viewport_bottom);
                            }
                        }
                    }
                }
                else
                {

                    float loc_x, loc_y;
                    if (this.HasContainingBlockProperty)
                    {
                        args.PushContaingBlock(this);
                        foreach (var b in this._boxes)
                        {
                            if (b.CssDisplay == CssDisplay.None)
                            {
                                continue;
                            }

                            g.OffsetCanvasOrigin(loc_x = b.LocalX, loc_y = b.LocalY);
                            b.Paint(g, args);
                            g.OffsetCanvasOrigin(-loc_x, -loc_y);
                        }
                        args.PopContainingBlock();
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
                            g.OffsetCanvasOrigin(loc_x = b.LocalX, loc_y = b.LocalY);
                            b.Paint(g, args);
                            g.OffsetCanvasOrigin(-loc_x, -loc_y);
                        }
                    }
                }
                //------------------------------------------
                //must! , 
                if (hasPrevClip)
                {
                    RenderUtils.ReturnClip(g, prevClip);
                }
                if (_listItemBox != null)
                {
                    _listItemBox.Paint(g, args);
                }
            }
        }




    }
}