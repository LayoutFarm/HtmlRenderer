//BSD 2014, WinterDev
//ArthurHub

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using HtmlRenderer.Utils;


namespace HtmlRenderer.RenderDom
{

    partial class CssBox
    {

        public void Paint(IGraphics g, PaintVisitor p)
        {
#if DEBUG
            dbugCounter.dbugBoxPaintCount++;
#endif
            if (this._isVisible)
            {
                PaintImp(g, p);
            }
        }
#if DEBUG
        public void dbugPaint(PaintVisitor p, RectangleF r)
        {
            return;

            var htmlE = CssBox.debugGetController(this);
            if (htmlE == null)
            {
                p.dbugDrawDiagonalBox(Pens.Gray, r.Left, r.Top, r.Right, r.Bottom);

            }
            else
            {
                p.dbugDrawDiagonalBox(Pens.Green, r.Left, r.Top, r.Right, r.Bottom);
            }
        }
#endif

        protected virtual void PaintImp(IGraphics g, PaintVisitor p)
        {
            Css.CssDisplay display = this.CssDisplay;

            if (display != Css.CssDisplay.None &&
               (display != Css.CssDisplay.TableCell ||
                this.EmptyCells != Css.CssEmptyCell.Hide || !IsSpaceOrEmpty))
            {

                bool hasPrevClip = false;
                RectangleF prevClip = RectangleF.Empty;

                if (this._isHiddenOverflow)
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
                if (display != Css.CssDisplay.Inline)
                {

                    RectangleF bound = new RectangleF(0, 0, this.SizeWidth, this.SizeHeight);

                    PaintBackground(p, bound, true, true);

                    if (this.HasSomeVisibleBorder)
                    {
                        p.PaintBorders(this, bound, true, true);
                    }
#if DEBUG
                    dbugPaint(p, bound);
#endif
                }
                //---------------------------------------------
                if (this.LineBoxCount > 0)
                {

                    float viewport_top = p.LocalViewportTop;
                    float viewport_bottom = p.LocalViewportBottom;

                    foreach (var line in this._clientLineBoxes)
                    {

                        if (line.CachedLineBottom >= viewport_top &&
                            line.CachedLineTop <= viewport_bottom)
                        {


#if DEBUG
                            dbugCounter.dbugLinePaintCount++;
#endif

                            float cX = g.CanvasOriginX;
                            float cy = g.CanvasOriginY;

                            g.SetCanvasOrigin(cX, cy + line.CachedLineTop);
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

                            g.SetCanvasOrigin(cX, cy);//back
                            //---------------------------------------- 
                        }
                    }

                }
                else
                {


                    if (this.HasContainingBlockProperty)
                    {
                        p.PushContaingBlock(this);

                        float ox = g.CanvasOriginX;
                        float oy = g.CanvasOriginY;

                        foreach (var b in this._aa_boxes)
                        {
                            if (b.CssDisplay == Css.CssDisplay.None)
                            {
                                continue;
                            }
                            g.SetCanvasOrigin(ox + b.LocalX, oy + b.LocalY);
                            b.Paint(g, p);
                        }

                        g.SetCanvasOrigin(ox, oy);


                        p.PopContainingBlock();
                    }
                    else
                    {
                        //if not
                        float ox = g.CanvasOriginX;
                        float oy = g.CanvasOriginY;

                        foreach (var b in this._aa_boxes)
                        {
                            if (b.CssDisplay == Css.CssDisplay.None)
                            {
                                continue;
                            }
                            g.SetCanvasOrigin(ox + b.LocalX, oy + b.LocalY);
                            b.Paint(g, p);
                        }
                        g.SetCanvasOrigin(ox, oy);

                    }
                }
                //------------------------------------------
                //debug
                //var clientLeft = this.ClientLeft;
                //g.DrawRectangle(Pens.GreenYellow, 0, 0, 5, 10);
                //g.DrawRectangle(Pens.HotPink, this.ClientRight - 5, 0, 5, 10);
                //------------------------------------------  
                //must! , 
                if (hasPrevClip)
                {
                    p.PopLocalClipArea();
                }
                //------------------------------------------   
                if (_subBoxes != null && _subBoxes.ListItemBulletBox != null)
                {
                    _subBoxes.ListItemBulletBox.Paint(g, p);
                }

            }
        }

        /// <summary>
        /// Paints the background of the box
        /// </summary>
        /// <param name="g">the device to draw into</param>
        /// <param name="rect">the bounding rectangle to draw in</param>
        /// <param name="isFirst">is it the first rectangle of the element</param>
        /// <param name="isLast">is it the last rectangle of the element</param>
        internal void PaintBackground(PaintVisitor p, RectangleF rect, bool isFirst, bool isLast)
        {
            if (!this.HasVisibleBgColor)
            {
                return;
            }

            if (rect.Width > 0 && rect.Height > 0)
            {
                Brush brush = null;
                bool dispose = false;

                if (BackgroundGradient != System.Drawing.Color.Transparent)
                {
                    brush = new LinearGradientBrush(rect,
                        ActualBackgroundColor,
                        ActualBackgroundGradient,
                        ActualBackgroundGradientAngle);

                    dispose = true;
                }
                else if (RenderUtils.IsColorVisible(ActualBackgroundColor))
                {

                    brush = RenderUtils.GetSolidBrush(ActualBackgroundColor);
                }


                IGraphics g = p.Gfx;
                SmoothingMode smooth = g.SmoothingMode;

                if (brush != null)
                {
                    // atodo: handle it correctly (tables background)
                    // if (isLast)
                    //  rectangle.Width -= ActualWordSpacing + CssUtils.GetWordEndWhitespace(ActualFont);

                    GraphicsPath roundrect = null;
                    bool hasSomeRoundCorner = this.HasSomeRoundCorner;
                    if (hasSomeRoundCorner)
                    {
                        roundrect = RenderUtils.GetRoundRect(rect, ActualCornerNW, ActualCornerNE, ActualCornerSE, ActualCornerSW);
                    }

                    if (!p.AvoidGeometryAntialias && hasSomeRoundCorner)
                    {
                        g.SmoothingMode = SmoothingMode.AntiAlias;
                    }

                    if (roundrect != null)
                    {
                        g.FillPath(brush, roundrect);
                    }
                    else
                    {
                        g.FillRectangle(brush, (float)Math.Ceiling(rect.X), (float)Math.Ceiling(rect.Y), rect.Width, rect.Height);
                    }

                    g.SmoothingMode = smooth;

                    if (roundrect != null) roundrect.Dispose();
                    if (dispose) brush.Dispose();
                }

                if (isFirst)
                {
                    var bgImageBinder = this.BackgroundImageBinder;
                    if (bgImageBinder != null && bgImageBinder.Image != null)
                    {
                        BackgroundImageDrawHandler.DrawBackgroundImage(g, this, bgImageBinder, rect);
                    }
                }
            }
        }

        internal void PaintDecoration(IGraphics g, RectangleF rectangle, bool isFirst, bool isLast)
        {
            float y = 0f;
            switch (this.TextDecoration)
            {
                default:
                    return;
                case Css.CssTextDecoration.Underline:
                    {
                        //TODO: correct this ...
                        var h = g.MeasureString(" ", ActualFont).Height;
                        float desc = FontsUtils.GetDescent(ActualFont, g);
                        y = (float)Math.Round(rectangle.Top + h - desc + 0.5);
                    } break;
                case Css.CssTextDecoration.LineThrough:
                    {
                        y = rectangle.Top + rectangle.Height / 2f;
                    } break;
                case Css.CssTextDecoration.Overline:
                    {
                        y = rectangle.Top;
                    } break;
            }


            y -= ActualPaddingBottom - ActualBorderBottomWidth;

            float x1 = rectangle.X;
            if (isFirst)
            {
                x1 += ActualPaddingLeft + ActualBorderLeftWidth;
            }


            float x2 = rectangle.Right;
            if (isLast)
            {
                x2 -= ActualPaddingRight + ActualBorderRightWidth;
            }

            var pen = RenderUtils.GetPen(ActualColor);

            g.DrawLine(pen, x1, y, x2, y);
        }



    }
}