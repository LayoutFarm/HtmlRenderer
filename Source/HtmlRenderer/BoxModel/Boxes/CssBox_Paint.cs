
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

        public void Paint(IGraphics g, PaintVisitor p)
        {
            if (this.CssDisplay != CssDisplay.None &&
                this.CssVisibility == Dom.CssVisibility.Visible)
            {
                //----------------
                PaintImp(g, p);

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
        protected virtual void PaintImp(IGraphics g, PaintVisitor p)
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
                        if (!(hasPrevClip = p.PushLocalClipArea(expectedW, expectedH)))
                        {
                            p.PopLocalClipArea();
                            return;
                        }
                    }
                }

                //---------------------------------------------
                if (this.CssDisplay != CssDisplay.Inline)
                {
                    RectangleF bound = new RectangleF(0, 0, this.SizeWidth, this.SizeHeight);
                    PaintBackground(p, bound, true, true);
                    p.PaintBorders(this, bound, true, true);
                }

                if (this.LineBoxCount > 0)
                {
                    var viewport = p.PeekLocalViewportBound();

                    float viewport_top = viewport.Top;
                    float viewport_bottom = viewport.Bottom;
                     

                    
                    foreach (var line in this._clientLineBoxes)
                    {

                        if (line.CachedLineBottom >= viewport_top &&
                            line.CachedLineTop <= viewport_bottom)
                        {

                            g.OffsetCanvasOrigin(0, line.CachedLineTop);
                            //----------------------------------------
                            //1.                                 
                            line.PaintBackgroundAndBorder(p);

                            if (line.LineSelectionWidth > 0)
                            {
                                line.PaintSelection(p);
                            }           
                            //------------
                            //2.                                
                            line.PaintRuns(g, p);

                            //3. 
                            line.PaintDecoration(g, p);

#if DEBUG
                            line.dbugPaintRuns(g, p);
#endif
                            g.OffsetCanvasOrigin(0, -line.CachedLineTop);
                            //----------------------------------------

                        }
                        else
                        {
                        }
                    }

                }
                else
                {

                    float loc_x, loc_y;
                    if (this.HasContainingBlockProperty)
                    {
                        p.PushContaingBlock(this);
                        foreach (var b in this._boxes)
                        {
                            if (b.CssDisplay == CssDisplay.None)
                            {
                                continue;
                            }
                            g.OffsetCanvasOrigin(loc_x = b.LocalX, loc_y = b.LocalY);
                            b.Paint(g, p);
                            g.OffsetCanvasOrigin(-loc_x, -loc_y);
                        }
                        p.PopContainingBlock();
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
                            b.Paint(g, p);
                            g.OffsetCanvasOrigin(-loc_x, -loc_y);
                        }
                    }
                }
                //------------------------------------------
                //must! , 
                if (hasPrevClip)
                {
                    p.PopLocalClipArea();
                }

                if (_listItemBox != null)
                {
                    _listItemBox.Paint(g, p);
                }
            }
        }




    }
}