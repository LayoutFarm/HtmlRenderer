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
        //start line

        CssLineBox startHitHostLine;
        int startHitRunCharIndex;
        //--------------------- 
        //on end line 
        CssLineBox endHitHostLine;
        int endHitRunCharIndex;
        //---------------------      
        List<CssLineBox> selectedLines;

        internal SelectionRange(BoxHitChain startChain, BoxHitChain endChain, IGraphics g)
        {

            //1.
            this.SetupStartHitPoint(startChain, g);
            //2.
            this.SetupEndHitPoint(endChain, g);
            //3. 

        }

        internal void ClearSelectionStatus()
        {
            if (this.selectedLines != null)
            {
                for (int i = selectedLines.Count - 1; i >= 0; --i)
                {
                    this.selectedLines[i].LineSelectionWidth = 0;
                }
                this.selectedLines.Clear();
            }
            else
            {
                if (this.startHitHostLine != null)
                {
                    this.startHitHostLine.LineSelectionWidth = 0;
                }
                if (this.endHitHostLine != null)
                {
                    this.endHitHostLine.LineSelectionWidth = 0;
                }
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
            //-----------------------------
            switch (startHit.hitObjectKind)
            {
                case HitObjectKind.Run:
                    {
                        CssRun run = (CssRun)startHit.hitObject;
                        int sel_index;
                        int sel_offset;

                        run.FindSelectionPoint(g,
                             startHit.localX,
                             true,
                             out sel_index,
                             out sel_offset);

                        this.startHitRunCharIndex = sel_index;

                        //modify hitpoint
                        CssLineBox hostLine = (CssLineBox)startChain.GetHitInfo(startChain.Count - 2).hitObject;

                        this.startHitHostLine = hostLine;

                        //make start hit point relative to line 

                        hostLine.LineSelectionStart = (int)(run.Left + sel_offset);
                        //------

                    } break;
                case HitObjectKind.LineBox:
                    {

                        this.startHitHostLine = (CssLineBox)startHit.hitObject;
                        //make global                         
                        startHitHostLine.LineSelectionStart = startHit.localX;

                    } break;
                case HitObjectKind.CssBox:
                    {
                        CssBox box = (CssBox)startHit.hitObject;

                        //find first nearest line at point  

                        CssLineBox startHitLine = FindNearestLine(box, startHit.localY);
                        if (startHitLine != null)
                        {
                            this.startHitHostLine = startHitLine;
                            startHitLine.LineSelectionStart = 0;
                        }
                        else
                        {
                            //if not found?
                            throw new NotSupportedException();
                        }

                    } break;
                default:
                    {
                        throw new NotSupportedException();
                    } break;
            }
        }

        void SetupEndHitPoint(BoxHitChain endChain, IGraphics g)
        {


            HitInfo endHit = endChain.GetLastHit();
            switch (endHit.hitObjectKind)
            {
                default:
                    {
                        throw new NotSupportedException();
                    } break;
                case HitObjectKind.Run:
                    {

                        CssRun run = (CssRun)endHit.hitObject;
                        if (run is CssTextRun)
                        {
                            CssTextRun tt = (CssTextRun)run;
                            if (!tt.Text.Contains("For"))
                            {

                            }
                        }

                        int sel_index;
                        int sel_offset;
                        run.FindSelectionPoint(g,
                             endHit.localX,
                             true,
                             out sel_index,
                             out sel_offset);
                        this.endHitRunCharIndex = sel_index;

                        //adjust
                        CssLineBox endline = (CssLineBox)endChain.GetHitInfo(endChain.Count - 2).hitObject;
                        int xposOnEndLine = (int)(run.Left + sel_offset);

                        if (startHitHostLine == endline)
                        {   
                            endline.LineSelectionWidth = xposOnEndLine - startHitHostLine.LineSelectionStart;
                        }
                        else
                        {
                            //select on different line 
                            //-------
                            this.selectedLines = new List<CssLineBox>();
                            //1. select all in start line      
                            CssLineBox startLineBox = this.startHitHostLine;
                            foreach (CssLineBox line in GetLineWalkIter(startLineBox, endline))
                            {
                                if (line == startLineBox)
                                {
                                    line.SelectPartialStart(line.LineSelectionStart);
                                    selectedLines.Add(line);
                                }
                                else if (line == endline)
                                {
                                    //-------
                                    //2. end line 
                                    line.SelectPartialEnd(xposOnEndLine);
                                    selectedLines.Add(line);
                                }
                                else
                                {
                                    //inbetween
                                    line.SelectFull();
                                    selectedLines.Add(line);
                                }
                            }
                        }
                    } break;
                case HitObjectKind.LineBox:
                    {

                        CssLineBox endline = (CssLineBox)endHit.hitObject; 
                        if (this.startHitHostLine == endline)
                        {   
                            endline.LineSelectionWidth = endHit.localX - startHitHostLine.LineSelectionStart;
                        }
                        else
                        {
                            //between line
                            //select on different line 
                            //-------
                            this.selectedLines = new List<CssLineBox>();

                            //1. select all in start line      
                            //1. select all in start line      
                            CssLineBox startLineBox = this.startHitHostLine; 
                            foreach (CssLineBox line in GetLineWalkIter(startLineBox, endline))
                            {
                                if (line == startLineBox)
                                { 
                                    line.SelectPartialStart(startLineBox.LineSelectionStart);
                                    selectedLines.Add(line);
                                }
                                else if (line == endline)
                                {
                                    endline.SelectPartialEnd(endHit.localX);
                                    selectedLines.Add(line);
                                }
                                else
                                {
                                    //between
                                    line.SelectFull();
                                    selectedLines.Add(line); 
                                }
                            }
                        }

                    } break;
                case HitObjectKind.CssBox:
                    {


                        CssLineBox latestLine = null;
                        this.selectedLines = new List<CssLineBox>();

                        float hitY = endHit.localY;
                        foreach (var line in GetLineWalkIter(this.startHitHostLine, (CssBox)endHit.hitObject))
                        {
                            if (line == startHitHostLine)
                            {
                                line.SelectPartialStart(startHitHostLine.LineSelectionStart);
                                selectedLines.Add(line);
                                latestLine = line;
                            }
                            else
                            {
                                if (line.CachedLineBottom < hitY)
                                {
                                    latestLine = line;
                                    line.SelectFull();
                                    selectedLines.Add(line);

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
                            //this.xposOnEndLine = endHit.localX;

                        }
                        else
                        {
                        }

                    } break;

            }
        }



    }

    static class CssLineBoxExtension
    {
        public static void SelectFull(this CssLineBox lineBox)
        {
            //full line selection
            lineBox.LineSelectionStart = 0;
            lineBox.LineSelectionWidth = (int)lineBox.CachedLineContentWidth; 
        }
        public static void SelectPartialStart(this CssLineBox lineBox, int startAt)
        {
            //from startAt to end of line
            lineBox.LineSelectionStart = startAt;
            lineBox.LineSelectionWidth = (int)lineBox.CachedLineContentWidth - startAt; 
        }
        public static void SelectPartialEnd(this CssLineBox lineBox, int endAt)
        {
            //from start of line to endAt
            lineBox.LineSelectionStart = 0;
            lineBox.LineSelectionWidth = endAt;
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