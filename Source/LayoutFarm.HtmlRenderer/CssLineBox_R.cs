//BSD, 2014-present, WinterDev 
//ArthurHub, Jose Manuel Menendez Poo

// "Therefore those skilled at the unorthodox
// are infinite as heaven and earth,
// inexhaustible as the great rivers.
// When they come to an end,
// they begin again,
// like the days and months;
// they die and are reborn,
// like the four seasons."
// 
// - Sun Tsu,
// "The Art of War"

using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
namespace LayoutFarm.HtmlBoxes
{


    /// <summary>
    /// Represents a line of text.
    /// </summary>
    /// <remarks>
    /// To learn more about line-boxes see CSS spec:
    /// http://www.w3.org/TR/CSS21/visuren.html
    /// </remarks>
    sealed partial class CssLineBox
    {

#if DEBUG
        internal void dbugPaintRuns(PaintVisitor p)
        {
            if (!PaintVisitor.dbugDrawWireFrame)
            {
                return;
            }
            //linebox  
            float x1 = 0;
            float y1 = 0;
            float x2 = x1 + this.CachedLineContentWidth;
            float y2 = y1 + this.CacheLineHeight;
            //draw diagonal  
            p.dbugDrawDiagonalBox(Color.Blue, x1, y1, x2, y2);
            //g.DrawRectangle(Pens.Blue,
            //    this.OwnerBox.LocationX,
            //    this.CachedLineTop,
            //    this.CachedLineContentWidth,
            //    this.CacheLineHeight);
            //foreach (var strip in _boxStrips.Values)
            //{
            //    var bound = strip.Bound;
            //    bound.Offset(offset);
            //    dbugDrawDiagnalBox(g, Pens.Green, bound.X, bound.Y, bound.Right, bound.Bottom);
            //} 
            //return; 
            foreach (CssRun w in _runs)
            {
                p.DrawRectangle(KnownColors.DeepPink, w.Left, w.Top, w.Width, w.Height);
                //p.dbugDrawDiagonalBox(Color.DeepPink, w.Left, w.Top, w.Width, w.Height);
            }

            p.FillRectangle(Color.Red, 0, 0, 5, 5);
        }

#endif


        internal void PaintRuns(PaintVisitor p)
        {
            List<CssRun> tmpRuns = _runs;
            int j = tmpRuns.Count;
            if (j < 1) { return; }
            //-----------------------


            //iterate from each words 
            CssBox latestOwner = null;
            DrawBoard innerCanvas = p.InnerDrawBoard;
            RequestFont enterFont = innerCanvas.CurrentFont;
            Color enterColor = innerCanvas.CurrentTextColor;

            for (int i = 0; i < j; ++i)
            {
                //-----------------
#if DEBUG
                dbugCounter.dbugRunPaintCount++;
#endif
                //-----------------

                CssRun w = tmpRuns[i];
                switch (w.Kind)
                {
                    case CssRunKind.SolidContent:
                        {
#if DEBUG
                            //System.Diagnostics.Debug.WriteLine("ox,oy=" + p.CanvasOriginX + "," + p.CanvasOriginY);
                            //System.Diagnostics.Debug.WriteLine("clip=" + p.CurrentClipRect); 
#endif

                            Rectangle currentClipRect = p.CurrentClipRect;
                            Rectangle wRect = new Rectangle((int)w.Left, (int)w.Top, (int)w.Width, (int)w.Height);
                            wRect.Intersect(currentClipRect);
#if DEBUG
                            //System.Diagnostics.Debug.WriteLine("empty_clip=" + (wRect.Height == 0 || wRect.Width == 0));
#endif

                            if (wRect.Height != 0 && wRect.Width != 0)
                            {
                                w.OwnerBox.Paint(p, new RectangleF(w.Left, w.Top, w.Width, w.Height));
                            }
                        }
                        break;
                    case CssRunKind.BlockRun:
                        {
                            //Console.WriteLine("blockrun"); 
                            CssBlockRun blockRun = (CssBlockRun)w;
                            int ox = p.CanvasOriginX;
                            int oy = p.CanvasOriginY;
                            //p.SetCanvasOrigin(ox + (int)(blockRun.Left + blockRun.ContentBox.LocalX), oy + (int)blockRun.Top);
                            p.SetCanvasOrigin(ox + (int)(blockRun.Left), oy + (int)blockRun.Top);

                            CssBox.Paint(blockRun.ContentBox, p);

                            p.SetCanvasOrigin(ox, oy);
                        }
                        break;
                    case CssRunKind.Text:
                        {
                            if (latestOwner != w.OwnerBox)
                            {
                                //change
                                latestOwner = w.OwnerBox;
                                //change font when change owner 

                                p.InnerDrawBoard.CurrentFont = latestOwner.ResolvedFont;
                                p.InnerDrawBoard.CurrentTextColor = latestOwner.ActualColor;
                            }

                            CssTextRun textRun = (CssTextRun)w;
                            p.DrawText(CssBox.UnsafeGetTextBuffer(w.OwnerBox),
                                                          textRun.TextStartIndex,
                                                          textRun.TextLength,
                                                          new PointF(w.Left, w.Top),
                                                          new SizeF(w.Width, w.Height));


                            //RenderVxFormattedString formattedStr = CssTextRun.GetCachedFormatString(textRun);
                            //if (formattedStr == null)
                            //{
                            //    formattedStr = p.CreateRenderVx(CssBox.UnsafeGetTextBuffer(w.OwnerBox),
                            //                                textRun.TextStartIndex,
                            //                                textRun.TextLength);

                            //    CssTextRun.SetCachedFormattedString(textRun, formattedStr);
                            //}
                            //if (formattedStr != null)
                            //{
                            //    p.DrawText(formattedStr,
                            //                               new PointF(w.Left, w.Top),
                            //                               new SizeF(w.Width, w.Height));
                            //}
                            //else
                            //{
                            //    p.DrawText(CssBox.UnsafeGetTextBuffer(w.OwnerBox),
                            //                               textRun.TextStartIndex,
                            //                               textRun.TextLength,
                            //                               new PointF(w.Left, w.Top),
                            //                               new SizeF(w.Width, w.Height));
                            //}

                        }
                        break;
                    default:
                        {
#if DEBUG
                            // w.OwnerBox.dbugPaintTextWordArea(g, offset, w);
#endif
                        }
                        break;
                }
            }


            innerCanvas.CurrentFont = enterFont;
            innerCanvas.CurrentTextColor = enterColor;

        }

        internal void PaintBackgroundAndBorder(PaintVisitor p)
        {
            //iterate each strip
            //if (_bottomUpBoxStrips == null)
            //{
            //    return;
            //}
            for (int i = _bottomUpBoxStrips.Length - 1; i >= 0; --i)
            {
                PartialBoxStrip strip = _bottomUpBoxStrips[i];
                CssBox stripOwner = strip.owner;
                if (!stripOwner.HasVisibleBgColor)
                {
                    continue;
                }
                //-----------------------------------------------------------------
                RectangleF stripArea = strip.Bound;
                bool isFirstLine, isLastLine;
                CssBox.GetSplitInfo(stripOwner, this, out isFirstLine, out isLastLine);
                stripOwner.PaintBackground(p, stripArea, isFirstLine, isLastLine);
                //if (stripOwner.CssDisplay != Css.CssDisplay.TableCell
                //    && stripOwner.HasSomeVisibleBorder)
                //{
                //    p.PaintBorders(stripOwner, stripArea, isFirstLine, isLastLine);
                //}

            }
        }

        internal void PaintDecoration(PaintVisitor p)
        {
            //if (_bottomUpBoxStrips == null)
            //{
            //    return;
            //}
            for (int i = _bottomUpBoxStrips.Length - 1; i >= 0; --i)
            {
                PartialBoxStrip strip = _bottomUpBoxStrips[i];
                CssBox ownerBox = strip.owner;
                bool isFirstLine, isLastLine;
                CssBox.GetSplitInfo(ownerBox, this, out isFirstLine, out isLastLine);
                ownerBox.PaintDecoration(p.InnerDrawBoard, strip.Bound, isFirstLine, isLastLine);
            }
        }

    }
}
