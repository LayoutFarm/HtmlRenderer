//BSD, 2014-2017, WinterDev
//ArthurHub  , Jose Manuel Menendez Poo

using System;
using PixelFarm.Drawing;
using PixelFarm.Drawing.Fonts;
namespace LayoutFarm.HtmlBoxes
{
    partial class CssBox
    {
        public void InvalidateGraphics()
        {
            //bubble invalidate area to to parent?
            var parentBox = this.ParentBox;
            if (parentBox != null)
            {
                parentBox.InvalidateGraphics(new Rectangle(
                    (int)this.LocalX,
                    (int)this.LocalY,
                    (int)this.VisualWidth,
                    (int)this.VisualHeight));
            }
        }
        public virtual void InvalidateGraphics(Rectangle clientArea)
        {
            //bubble up to parent
            //clientArea => area relative to this element
            //adjust to 
            //adjust client area 
            var parentBox = this.ParentBox;
            if (parentBox != null)
            {
                clientArea.Offset((int)this.LocalX, (int)this.LocalY);
                parentBox.InvalidateGraphics(clientArea);
            }
        }
        public void Paint(PaintVisitor p)
        {
#if DEBUG
            dbugCounter.dbugBoxPaintCount++;
#endif
            if (this._isVisible)
            {
                //offset 

                if (this.mayHasViewport)
                {
                    p.OffsetCanvasOrigin(-this.ViewportX, -this.ViewportY);
                    PaintImp(p);
                    p.OffsetCanvasOrigin(this.ViewportX, this.ViewportY);
                }
                else
                {
                    PaintImp(p);
                }
            }
        }
#if DEBUG
        public void dbugPaint(PaintVisitor p, RectangleF r)
        {
            if (!PaintVisitor.dbugDrawWireFrame)
            {
                return;
            }
            //
            var htmlE = CssBox.UnsafeGetController(this);
            if (htmlE == null)
            {
                //anonymous box
                //Font f = new Font("tahoma", 10);
                //p.Gfx.DrawString(this.__aa_dbugId.ToString(), f, System.Drawing.Color.Black,
                //     new PointF(r.Left + 10, r.Top + 10), new SizeF(r.Width, r.Height));
                //f.Dispose();

                p.dbugDrawDiagonalBox(Color.Gray, r.Left, r.Top, r.Right, r.Bottom);
            }
            else
            {
                Color color = Color.Green;
                switch (this._cssDisplay)
                {
                    case Css.CssDisplay.TableCell:
                        color = Color.OrangeRed;
                        break;
                }
                p.dbugDrawDiagonalBox(color, r.Left, r.Top, r.Right, r.Bottom);
            }
        }
#endif
        protected virtual void PaintImp(PaintVisitor p)
        {
            //if (this.dbugMark2 == 10 || this.dbugMark2 == 12)
            //{

            //}

            Css.CssDisplay display = this.CssDisplay;
            if (display == Css.CssDisplay.TableCell &&
                this.EmptyCells == Css.CssEmptyCell.Hide &&
                this.IsSpaceOrEmpty)
            {
                return;
            }
#if DEBUG
            p.dbugEnterNewContext(this, PaintVisitor.PaintVisitorContextName.Init);
#endif

            //----------------------------------------------- 
            bool hasPrevClip = false;
            RectangleF prevClip = RectangleF.Empty;
            p.EnterNewLatePaintContext();
            //---------------------------------------------
            //if (display != Css.CssDisplay.Inline ||
            //    this.Position == Css.CssPosition.Absolute ||
            //    this.Position == Css.CssPosition.Fixed)
            if (this._renderBGAndBorder)
            {
                RectangleF bound = new RectangleF(0, 0, this.VisualWidth, this.VisualHeight);
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
                float viewport_top = p.ViewportTop;
                float viewport_bottom = p.ViewportBottom;
                int drawState = 0;
                var c_lineNode = this._clientLineBoxes.First;
                while (c_lineNode != null)
                {
                    CssLineBox line = c_lineNode.Value;
                    if (line.CachedLineBottom >= viewport_top &&
                        line.CachedLineTop <= viewport_bottom)
                    {
#if DEBUG
                        dbugCounter.dbugLinePaintCount++;
#endif

                        drawState = 1;//drawing in viewport area
                        int cX = p.CanvasOriginX;
                        int cy = p.CanvasOriginY;
                        p.SetCanvasOrigin(cX, cy + (int)line.CachedLineTop);
                        //1.                                 
                        line.PaintBackgroundAndBorder(p);
                        if (line.SelectionSegment != null)
                        {
                            line.SelectionSegment.PaintSelection(p, line);
                        }
                        //2.                                
                        line.PaintRuns(p);
                        //3. 
                        line.PaintDecoration(p);
#if DEBUG
                        line.dbugPaintRuns(p);
#endif

                        p.SetCanvasOrigin(cX, cy);//back
                    }
                    else if (drawState == 1)
                    {
                        //outof viewport -> break
                        break;
                    }

                    //----------------------------------------
                    c_lineNode = c_lineNode.Next;
                }
            }
            else
            {
                if (this.HasContainingBlockProperty)
                {
                    p.PushContaingBlock(this);
                    int ox = p.CanvasOriginX;
                    int oy = p.CanvasOriginY;
                    var node = this._aa_boxes.GetFirstLinkedNode();
                    while (node != null)
                    {
                        CssBox b = node.Value;
                        if (b.CssDisplay == Css.CssDisplay.None)
                        {
                            node = node.Next;
                            continue;
                        }
                        else if (b.IsOutOfFlowBox)
                        {
                            //
                            p.AddToLatePaintList(b);
                            node = node.Next;
                            continue;
                        }
                        //move to left-top of client box 
                        p.SetCanvasOrigin(ox + (int)b.LocalX, oy + (int)b.LocalY);
                        if (b.decorator != null)
                        {
                            b.decorator.Paint(b, p);
                        }

                        if (b.HasClipArea)
                        {
                            if (p.PushLocalClipArea(b.VisualWidth, b.VisualHeight))
                            {
                                b.Paint(p);
                            }
                            p.PopLocalClipArea();
                        }
                        else
                        {
                            b.Paint(p);
                        }

                        node = node.Next;
                    }
                    p.SetCanvasOrigin(ox, oy);
                    p.PopContainingBlock();
                }
                else
                {
                    //if not
                    int ox = p.CanvasOriginX;
                    int oy = p.CanvasOriginY;
                    var node = this._aa_boxes.GetFirstLinkedNode();
                    while (node != null)
                    {
                        CssBox b = node.Value;
                        if (b.CssDisplay == Css.CssDisplay.None)
                        {
                            node = node.Next;
                            continue;
                        }
                        p.SetCanvasOrigin(ox + (int)b.LocalX, oy + (int)b.LocalY);
                        b.Paint(p);
                        node = node.Next;
                    }
                    p.SetCanvasOrigin(ox, oy);
                }
            }
            //------------------------------------------
            //debug
            //var clientLeft = this.ClientLeft;
            //g.DrawRectangle(Pens.GreenYellow, 0, 0, 5, 10);
            //g.DrawRectangle(Pens.HotPink, this.ClientRight - 5, 0, 5, 10);
            //------------------------------------------   

            if (this.HasAbsoluteLayer)
            {
                p.PushContaingBlock(this);
                int ox = p.CanvasOriginX;
                int oy = p.CanvasOriginY;
                var node = this._absPosLayer.GetFirstLinkedNode();
                while (node != null)
                {
                    CssBox b = node.Value;
                    if (b.CssDisplay == Css.CssDisplay.None)
                    {
                        node = node.Next;
                        continue;
                    }
                    p.SetCanvasOrigin(ox + (int)b.LocalX, oy + (int)b.LocalY);
                    b.Paint(p);
                    node = node.Next;
                }
                p.SetCanvasOrigin(ox, oy);
                p.PopContainingBlock();
            }

            if (p.LatePaintItemCount > 0)
            {
                //late paint -> floatBox 
                Rectangle latestClipRect = p.CurrentClipRect;
                p.PopLocalClipArea(); //temp
                p.PushContaingBlock(this);
                int j = p.LatePaintItemCount;
                int ox = p.CanvasOriginX;
                int oy = p.CanvasOriginY;
                for (int i = 0; i < j; ++i)
                {
                    CssBox box = p.GetLatePaintItem(i);
                    if (box.CssDisplay == Css.CssDisplay.None)
                    {
                        continue;
                    }
                    p.SetCanvasOrigin(ox + (int)box.LocalX, oy + (int)box.LocalY);
                    box.Paint(p);
                    p.SetCanvasOrigin(ox, oy);
                }
                p.PopContainingBlock();
                p.PushLocalClipArea(latestClipRect.Width, latestClipRect.Height);//push back
            }
            p.ExitCurrentLatePaintContext();
            //must! , 
            if (hasPrevClip)
            {
                p.PopLocalClipArea();
            }

#if DEBUG
            p.dbugExitContext();
#endif
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
            //return;
            //if (this.dbugMark1 > 0)
            //{
            //    Console.WriteLine(this.dbugMark1);
            //    if ((this.dbugMark1 % 2) == 0)
            //    {
            //    }
            //    else
            //    {
            //    }
            //}
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
                    //use bg gradient 

                    brush = LinearGradientBrush.CreateLinearGradientBrush(rect,
                        ActualBackgroundColor,
                        ActualBackgroundGradient,
                        ActualBackgroundGradientAngle);
                    dispose = true;
                }
                else if (RenderUtils.IsColorVisible(ActualBackgroundColor))
                {
                    brush = new SolidBrush(this.ActualBackgroundColor);
                    dispose = true;
                }


                DrawBoard g = p.InnerCanvas;
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
                        roundrect = RenderUtils.GetRoundRect( rect, ActualCornerNW, ActualCornerNE, ActualCornerSE, ActualCornerSW);
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
        internal void PaintDecoration(DrawBoard g, RectangleF rectangle, bool isFirst, bool isLast)
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

                        RequestFont afont = ResolvedFont;
                        
                            
                        //PixelFarm.Drawing.Fonts.ActualFont ff = afont.ActualFont;
                        var h = afont.SizeInPixels;
                        float desc = (float)afont.DescentInPixels;// fontInfo.DescentPx;
                        y = (float)Math.Round(rectangle.Top + h - desc);
                    }
                    break;
                case Css.CssTextDecoration.LineThrough:
                    {
                        y = rectangle.Top + rectangle.Height / 2f;
                    }
                    break;
                case Css.CssTextDecoration.Overline:
                    {
                        y = rectangle.Top;
                    }
                    break;
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

            var prevColor = g.StrokeColor;
            g.StrokeColor = ActualColor;
            g.DrawLine(x1, y, x2, y);
            g.StrokeColor = prevColor;
        }

        public virtual void Paint(PaintVisitor p, RectangleF r)
        {
        }
    }
}