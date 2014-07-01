
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
                this.BoxSpec.CssVisibility == Dom.CssVisibility.Visible)
            {   
                PaintImp(g, p);  
            }
        }
#if DEBUG
        public void dbugPaint(PaintVisitor p, RectangleF r)
        {
           // return;
            if (this.HtmlElement == null)
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
            //if (this.dbugId == 36)
            //{

            //}
            if (this.CssDisplay != CssDisplay.None &&
               (this.CssDisplay != CssDisplay.TableCell ||
                this.BoxSpec.EmptyCells != CssEmptyCell.Hide || !IsSpaceOrEmpty))
            {



                bool hasPrevClip = false;
                RectangleF prevClip = RectangleF.Empty;

                if (this.BoxSpec.Overflow == CssOverflow.Hidden)
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
                            if (b.CssDisplay == CssDisplay.None)
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
                            if (b.CssDisplay == CssDisplay.None)
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

                if (_listItemBox != null)
                {
                    _listItemBox.Paint(g, p);
                }
            }
        }




    }
}