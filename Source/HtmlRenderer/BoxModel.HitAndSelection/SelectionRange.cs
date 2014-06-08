//BSD 2014 ,WinterCore 
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

using HtmlRenderer.Entities;
using HtmlRenderer.Parse;

namespace HtmlRenderer.Dom
{

    public class SelectionRange
    {


        //--------------------- 
        Point startHitPoint;
        CssLineBox startHitHostLine;
        CssBox startHitCssBox;
        int startHitRunCharIndex;

        //--------------------- 
        Point endHitPoint;
        CssLineBox endHitHostLine;
        CssBox endHitCssBox;
        int endHitRunCharIndex;
        //---------------------       

        List<RectangleF> selStrips;
        List<CssLineBox> selLineBoxes;

        internal SelectionRange(BoxHitChain startChain, BoxHitChain endChain, IGraphics g)
        {

            //1.
            this.SetupStartHitPoint(startChain, g);
            //2.
            this.SetupEndHitPoint(endChain, g);
            //3.

            startHitCssBox.ContainsSelectedRun = true;
            endHitCssBox.ContainsSelectedRun = true;
        }

        internal void ClearSelectionStatus()
        {

            this.startHitCssBox.ContainsSelectedRun = false;
            this.endHitCssBox.ContainsSelectedRun = false;
            if (this.selLineBoxes != null)
            {
                for (int i = selLineBoxes.Count - 1; i >= 0; --i)
                {
                    this.selLineBoxes[i].OwnerBox.ContainsSelectedRun = false;
                }
                this.selLineBoxes.Clear();
            }


        }


        static CssLineBox FindNearestLine(CssBox box, int y)
        {
            CssLineBox latestLine = null;
            foreach (CssLineBox lineBox in Utils.DomUtils.GetDeepLineBoxIter(box))
            {
                if (lineBox.CacheLineHeight == 0)
                {
                    continue;
                }
                if (lineBox.CachedLineBottom <= y)
                {
                    latestLine = lineBox;
                }
                else
                {
                    latestLine = lineBox;
                    break;
                }
            }
            return latestLine;

        }

        static IEnumerable<CssLineBox> GetLineWalkIter(CssLineBox startLine, CssLineBox endLine)
        {

            foreach (var lineOrBox in Utils.DomUtils.GetLineOrBoxIterWalk(startLine))
            {
                if (lineOrBox.isLine)
                {
                    yield return lineOrBox.lineBox;
                    if (lineOrBox.lineBox == endLine)
                    {
                        break;
                    }
                }
            }

        }
        static IEnumerable<CssLineBox> GetLineWalkIter(CssLineBox startLine, CssBox endBox)
        {
            foreach (var visit in Utils.DomUtils.GetLineOrBoxIterWalk(startLine))
            {
                if (visit.isLine)
                {
                    yield return visit.lineBox;
                }
                else if (visit.box == endBox)
                {
                    break;
                }
            }
        }

        void SetupStartHitPoint(BoxHitChain startChain, IGraphics g)
        {
            HitInfo startHit = startChain.GetLastHit();
            this.startHitPoint = new Point(startHit.x, startHit.y);
            //-----------------------------
            switch (startHit.hitObjectKind)
            {
                case HitObjectKind.Run:
                    {
                        CssRun run = (CssRun)startHit.hitObject;
                        int sel_index;
                        int sel_offset;
                        run.FindSelectionPoint(g,
                            (int)(startHit.x - run.Left),
                             true,
                             out sel_index,
                             out sel_offset);

                        this.startHitRunCharIndex = sel_index;

                        //modify hitpoint
                        CssLineBox hostLine = (CssLineBox)startChain.GetHitInfo(startChain.Count - 2).hitObject;
                        this.startHitHostLine = hostLine;
                        this.startHitPoint = new Point((int)(run.Left + sel_offset), (int)hostLine.CachedLineTop);

                        //------
                        this.startHitCssBox = hostLine.OwnerBox;
                    } break;
                case HitObjectKind.LineBox:
                    {
                        this.startHitHostLine = (CssLineBox)startHit.hitObject;
                        this.startHitCssBox = startHitHostLine.OwnerBox;
                    } break;
                case HitObjectKind.CssBox:
                    {
                        CssBox box = (CssBox)startHit.hitObject;
                        //find first nearest line at point  
                        this.startHitCssBox = box;

                        CssLineBox startHitLine = FindNearestLine(box, startHit.y);
                        if (startHitLine != null)
                        {
                            this.startHitCssBox = startHitLine.OwnerBox;
                            this.startHitHostLine = startHitLine;
                        }

                    } break;
                default:
                    {
                    } break;
            }
        }

        void SetupEndHitPoint(BoxHitChain endChain, IGraphics g)
        {
            HitInfo endHit = endChain.GetLastHit();
            this.endHitPoint = new Point(endHit.x, endHit.y);

            switch (endHit.hitObjectKind)
            {
                case HitObjectKind.Run:
                    {

                        CssRun run = (CssRun)endHit.hitObject;
                        int sel_index;
                        int sel_offset;
                        run.FindSelectionPoint(g,
                            (int)(endHit.x - run.Left),
                             true,
                             out sel_index,
                             out sel_offset);
                        this.endHitRunCharIndex = sel_index;


                        //adjust
                        CssLineBox hostLine = (CssLineBox)endChain.GetHitInfo(endChain.Count - 2).hitObject;
                        this.endHitHostLine = hostLine;
                        this.endHitPoint = new Point((int)(run.Left + sel_offset), (int)hostLine.CachedLineBottom);
                        this.endHitCssBox = hostLine.OwnerBox;


                        if (this.startHitHostLine != this.endHitHostLine)
                        {
                            //select on different line 
                            //-------
                            this.selStrips = new List<RectangleF>();
                            this.selLineBoxes = new List<CssLineBox>();

                            //1. select all in start line      
                            CssLineBox startLineBox = this.startHitHostLine;
                            CssLineBox endLineBox = this.endHitHostLine;


                            foreach (CssLineBox line in GetLineWalkIter(startLineBox, endLineBox))
                            {

                                if (line == startLineBox)
                                {
                                    this.selStrips.Add(new RectangleF(
                                        this.startHitPoint.X,
                                        line.CachedLineTop,
                                        line.CachedLineContentWidth - this.startHitPoint.X,
                                        line.CacheLineHeight));
                                    this.selLineBoxes.Add(line);
                                }
                                else if (line == endLineBox)
                                {

                                    //-------
                                    //2. end line 
                                    CssBox endHostLineOwner = this.endHitHostLine.OwnerBox;
                                    this.selStrips.Add(new RectangleF(
                                        endHostLineOwner.LocationX,
                                        endLineBox.CachedLineTop,
                                        (int)(run.Left + sel_offset) - endHostLineOwner.LocationX,
                                        endLineBox.CacheLineHeight));

                                    this.selLineBoxes.Add(line);
                                }
                                else
                                {
                                    //inbetween
                                    CssBox lineOwner = line.OwnerBox;


                                    this.selStrips.Add(new RectangleF(
                                        lineOwner.LocationX,
                                        line.CachedLineTop,
                                        line.CachedLineContentWidth,
                                        line.CacheLineHeight));
                                    this.selLineBoxes.Add(line);
                                }
                            }
                        }
                    } break;
                case HitObjectKind.LineBox:
                    {

                        CssLineBox linebox = (CssLineBox)endHit.hitObject;
                        this.endHitHostLine = linebox;
                        this.endHitPoint = new Point(endHit.x, (int)linebox.CachedLineBottom);
                        this.endHitCssBox = linebox.OwnerBox;

                        if (this.startHitHostLine != this.endHitHostLine)
                        {
                            //between line
                            //select on different line 
                            //-------
                            this.selStrips = new List<RectangleF>();
                            this.selLineBoxes = new List<CssLineBox>();

                            //1. select all in start line      
                            //1. select all in start line      
                            CssLineBox startLineBox = this.startHitHostLine;
                            CssLineBox endLineBox = this.endHitHostLine;


                            foreach (CssLineBox line in GetLineWalkIter(startLineBox, endLineBox))
                            {

                                if (line == startLineBox)
                                {
                                    this.selStrips.Add(new RectangleF(
                                       this.startHitPoint.X,
                                       line.CachedLineTop,
                                       line.CachedLineContentWidth - this.startHitPoint.X,
                                       line.CacheLineHeight));
                                    this.selLineBoxes.Add(line);
                                }
                                else if (line == endLineBox)
                                {

                                    CssBox endHostLineOwner = this.endHitHostLine.OwnerBox;
                                    this.selStrips.Add(new RectangleF(
                                        endHostLineOwner.LocationX,
                                        linebox.CachedLineTop,
                                        endHitPoint.X - endHostLineOwner.LocationX,
                                        line.CacheLineHeight));
                                    this.selLineBoxes.Add(line);
                                }
                                else
                                {
                                    //between

                                    this.selStrips.Add(new RectangleF(
                                        line.OwnerBox.LocationX,
                                        line.CachedLineTop,
                                        line.CachedLineContentWidth,
                                        line.CacheLineHeight));
                                    this.selLineBoxes.Add(line);
                                }
                            }
                        }

                    } break;
                case HitObjectKind.CssBox:
                    {

                        this.endHitCssBox = (CssBox)endHit.hitObject;
                        CssLineBox latestLine = null;
                        this.selStrips = new List<RectangleF>();
                        this.selLineBoxes = new List<CssLineBox>();

                        float hitY = endHit.y;
                        foreach (var line in GetLineWalkIter(this.startHitHostLine, this.endHitCssBox))
                        {
                            if (line == startHitHostLine)
                            {
                                this.selStrips.Add(new RectangleF(
                                   this.startHitPoint.X,
                                   line.CachedLineTop,
                                   line.CachedLineContentWidth - this.startHitPoint.X,
                                   line.CacheLineHeight));
                                this.selLineBoxes.Add(line);
                                latestLine = line;
                            }
                            else
                            {
                                if (line.CachedLineBottom < hitY)
                                {
                                    latestLine = line;
                                    this.selStrips.Add(new RectangleF(
                                           line.OwnerBox.LocationX,
                                            line.CachedLineTop,
                                            line.CachedLineContentWidth,
                                            line.CacheLineHeight));
                                    this.selLineBoxes.Add(line);
                                }
                                else if (line.CacheLineHeight > 0)
                                {
                                    break;
                                }
                                else
                                {
                                    //if cache line height ==0
                                    //skip this line
                                }
                            }
                        }

                        //------------------------------------------------------
                        if (latestLine != null)
                        {
                            this.endHitHostLine = latestLine;
                            this.endHitPoint = new Point(endHit.x, (int)latestLine.CachedLineBottom);
                            this.endHitCssBox = latestLine.OwnerBox;
                        }
                        else
                        {
                        }

                    } break;
                default:
                    {
                    } break;
            }
        }
        internal void ActivateSelection()
        {
            this.endHitCssBox.ContainsSelectedRun = true;
            this.startHitCssBox.ContainsSelectedRun = true;

            if (this.selLineBoxes != null)
            {
                for (int i = selLineBoxes.Count - 1; i >= 0; --i)
                {
                    this.selLineBoxes[i].OwnerBox.ContainsSelectedRun = true;
                }
            }
        }

        Point StartHitPoint
        {
            get
            {
                return this.startHitPoint;
            }
        }
        Point EndHitPoint
        {
            get
            {
                return this.endHitPoint;
            }
        }

        internal void Draw(IGraphics g, PaintingArgs args, float lineTop, float lineHeight, PointF offset)
        {

            if (this.selStrips != null)
            {
                foreach (RectangleF rr in this.selStrips)
                {

                    if (rr.Top >= lineTop &&
                        rr.Bottom <= (lineTop + lineHeight))
                    {
                        rr.Offset(offset);

                        g.FillRectangle(Brushes.LightGray, rr.X, rr.Y,
                         rr.Width,
                         rr.Height);
                    }
                    else
                    {
                        //rr.Offset(offset);
                        //////dbug draw diagonal rect                        
                        //g.DrawRectangle(Pens.Red, rr.X, rr.Y, rr.Width, rr.Height);
                        //g.DrawLine(Pens.Red, rr.X, rr.Y, rr.Right, rr.Bottom);
                        //g.DrawLine(Pens.Red, rr.X, rr.Bottom, rr.Right, rr.Y);
                    }

                }
                return;
            }
            else
            {

                Point p1 = this.StartHitPoint;
                Point p2 = this.EndHitPoint;
                p1.Offset((int)offset.X, (int)offset.Y);
                p2.Offset((int)offset.X, (int)offset.Y);
                

                g.FillRectangle(Brushes.LightGray, p1.X, p1.Y,
                    p2.X - p1.X,
                    p2.Y - p1.Y);
            }
        }
    }


    struct LineOrBoxVisit
    {
        internal readonly CssLineBox lineBox;
        internal readonly CssBox box;
        internal readonly bool isLine;
        public LineOrBoxVisit(CssLineBox lineBox)
        {
            this.isLine = true;
            this.lineBox = lineBox;
            this.box = null;
        }
        public LineOrBoxVisit(CssBox box)
        {
            this.isLine = false;
            this.lineBox = null;
            this.box = box;
        }

    }
}