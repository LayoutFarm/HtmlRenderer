//BSD 2014, WinterDev
//ArthurHub

using System;
using System.Collections.Generic;
using LayoutFarm.Drawing;
 

namespace HtmlRenderer.Boxes
{

    partial class CssBox
    {

        public void Paint(IGraphics g, Painter p)
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
        public void dbugPaint(Painter p, RectangleF r)
        {
            //return; 
            var htmlE = CssBox.UnsafeGetController(this);
            if (htmlE == null)
            {
                //anonymous box
                //Font f = new Font("tahoma", 10);
                //p.Gfx.DrawString(this.__aa_dbugId.ToString(), f, System.Drawing.Color.Black,
                //     new PointF(r.Left + 10, r.Top + 10), new SizeF(r.Width, r.Height));
                //f.Dispose();

                p.dbugDrawDiagonalBox(Pens.Gray, r.Left, r.Top, r.Right, r.Bottom);
            }
            else
            {

                Pen selectedPens = null;
                switch (this._cssDisplay)
                {
                    case Css.CssDisplay.TableCell:
                        selectedPens = Pens.OrangeRed;
                        break;
                    //case Css.CssDisplay.BlockInsideInlineAfterCorrection:
                    //    selectedPens = Pens.Magenta;
                    //    break;
                    default:
                        selectedPens = Pens.Green;
                        break;
                }
                p.dbugDrawDiagonalBox(selectedPens, r.Left, r.Top, r.Right, r.Bottom);

            }
        }
#endif

        protected virtual void PaintImp(IGraphics g, Painter p)
        {



            Css.CssDisplay display = this.CssDisplay;
            if (display != Css.CssDisplay.TableCell ||
                this.EmptyCells != Css.CssEmptyCell.Hide || !IsSpaceOrEmpty)
            {

                bool hasPrevClip = false;
                RectangleF prevClip = RectangleF.Empty;

                if ((this._boxCompactFlags & BoxFlags.OVERFLOW_HIDDEN) != 0)
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

                    int drawState = 0;

                    var c_line = this._clientLineBoxes.First;

                    while (c_line != null)
                    {

                        var line = c_line.Value;

                        if (line.CachedLineBottom >= viewport_top &&
                            line.CachedLineTop <= viewport_bottom)
                        {
#if DEBUG
                            dbugCounter.dbugLinePaintCount++;
#endif

                            drawState = 1;//drawing in viewport area

                            float cX = g.CanvasOriginX;
                            float cy = g.CanvasOriginY;

                            g.SetCanvasOrigin(cX, cy + line.CachedLineTop);

                            //1.                                 
                            line.PaintBackgroundAndBorder(p);

                            if (line.LineSelectionWidth > 0)
                            {
                                line.PaintSelection(p);
                            }

                            //2.                                
                            line.PaintRuns(g, p);
                            //3. 
                            line.PaintDecoration(g, p);

#if DEBUG
                            line.dbugPaintRuns(g, p);
#endif

                            g.SetCanvasOrigin(cX, cy);//back

                        }
                        else if (drawState == 1)
                        {
                            //outof viewport -> break
                            break;
                        }

                        //----------------------------------------
                        c_line = c_line.Next;
                    }
                }
                else
                {


                    if (this.HasContainingBlockProperty)
                    {
                        p.PushContaingBlock(this);

                        float ox = g.CanvasOriginX;
                        float oy = g.CanvasOriginY;

                        var node = this._aa_boxes.GetFirstLinkedNode();
                        while (node != null)
                        {
                            CssBox b = node.Value;
                            if (b.CssDisplay == Css.CssDisplay.None)
                            {
                                node = node.Next;
                                continue;
                            }

                            g.SetCanvasOrigin(ox + b.LocalX, oy + b.LocalY);
                            b.Paint(g, p);
                            node = node.Next;
                        }
                        g.SetCanvasOrigin(ox, oy);
                        p.PopContainingBlock();
                    }
                    else
                    {
                        //if not
                        float ox = g.CanvasOriginX;
                        float oy = g.CanvasOriginY;

                        var node = this._aa_boxes.GetFirstLinkedNode();
                        while (node != null)
                        {
                            CssBox b = node.Value;
                            if (b.CssDisplay == Css.CssDisplay.None)
                            {
                                node = node.Next;
                                continue;
                            }
                            g.SetCanvasOrigin(ox + b.LocalX, oy + b.LocalY);
                            b.Paint(g, p);
                            node = node.Next;
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

            }
        }

        /// <summary>
        /// Paints the background of the box
        /// </summary>
        /// <param name="g">the device to draw into</param>
        /// <param name="rect">the bounding rectangle to draw in</param>
        /// <param name="isFirst">is it the first rectangle of the element</param>
        /// <param name="isLast">is it the last rectangle of the element</param>
        internal void PaintBackground(Painter p, RectangleF rect, bool isFirst, bool isLast)
        {
            if (!this.HasVisibleBgColor)
            {
                return;
            }

            if (rect.Width > 0 && rect.Height > 0)
            {
                Brush brush = null;
                bool dispose = false;

                if (BackgroundGradient != Color.Transparent)
                {
                    brush = p.Platform.CreateLinearGradientBrush(rect,
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

                        roundrect = RenderUtils.GetRoundRect(p.Platform ,
                            rect, 
                            ActualCornerNW,
                            ActualCornerNE, 
                            ActualCornerSE,
                            ActualCornerSW);
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
                        BackgroundImagePaintHelper.DrawBackgroundImage(g, this, bgImageBinder, rect);
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
                        //var h = g.MeasureString(" ", ActualFont).Height;
                        //float desc = FontsUtils.GetDescentPx(ActualFont);
                        //y = (float)Math.Round(rectangle.Top + h - desc + 0.5);
                        FontInfo fontInfo = g.GetFontInfo(ActualFont);
                        var h = fontInfo.LineHeight;
                        float desc = fontInfo.DescentPx;
                        y = (float)Math.Round(rectangle.Top + h - desc);

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


            //y -= ActualPaddingBottom - ActualBorderBottomWidth;
            y -= (ActualPaddingBottom + ActualBorderBottomWidth);

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