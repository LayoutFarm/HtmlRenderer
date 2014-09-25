//BSD 2014 ,WinterDev 
using System;
using System.Collections.Generic;
using LayoutFarm.Drawing;
using LayoutFarm.Drawing;

namespace HtmlRenderer.Boxes
{


    public class SelectionRange
    {
        //start line

        CssLineBox startHitHostLine;
        int startHitRunCharIndex;
        //--------------------- 
        //on end line  
        int endHitRunCharIndex;
        //---------------------      
        List<CssLineBox> selectedLines;

        public SelectionRange(BoxHitChain startChain, BoxHitChain endChain, IFonts ifonts)
        {


            if (IsOnTheSameLine(startChain, endChain))
            {
                //on the same line
                if (endChain.RootGlobalX < startChain.RootGlobalX)
                {
                    //swap
                    var tmp = endChain;
                    endChain = startChain;
                    startChain = tmp;
                }
            }
            else
            {
                //across line 
                if (endChain.RootGlobalY < startChain.RootGlobalY)
                {    //swap
                    var tmp = endChain;
                    endChain = startChain;
                    startChain = tmp;
                }
            }
            //Console.WriteLine(endChain.RootGlobalY);
            //if (endChain.RootGlobalY > 91)
            //{


            //}

            //1.
            this.SetupStartHitPoint(startChain, ifonts);
            //2. 
            this.SetupEndHitPoint(endChain, ifonts);

        }
        static bool IsOnTheSameLine(BoxHitChain startChain, BoxHitChain endChain)
        {
            CssLineBox startLineBox = GetLine(startChain.GetLastHit());
            CssLineBox endLineBox = GetLine(endChain.GetLastHit());
            return startLineBox != null && startLineBox == endLineBox;
        }
        static CssLineBox GetLine(HitInfo hitInfo)
        {
            switch (hitInfo.hitObjectKind)
            {
                default:
                case HitObjectKind.Unknown:
                    {
                        throw new NotSupportedException();
                    }
                case HitObjectKind.LineBox:
                    {
                        return (CssLineBox)hitInfo.hitObject;
                    }
                case HitObjectKind.Run:
                    {
                        return ((CssRun)hitInfo.hitObject).HostLine;
                    }
                case HitObjectKind.CssBox:
                    {
                        return null;
                    }
            }
        }

        public void ClearSelectionStatus()
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

            }
        }

        static CssLineBox FindNearestLine(CssBox startBox, int globalY, int yRange)
        {
            CssLineBox latestLine = null;
            CssBox latestLineBoxOwner = null;
            float latestLineBoxGlobalYPos = 0;

            foreach (CssLineBox lineBox in BoxUtils.GetDeepDownLineBoxIter(startBox))
            {
                if (lineBox.CacheLineHeight == 0)
                {
                    continue;
                }

                if (latestLineBoxOwner != lineBox.OwnerBox)
                {
                    //find global position of box
                    latestLineBoxOwner = lineBox.OwnerBox;
                    PointF boxGlobalPoint = GetGlobalLocation(latestLineBoxOwner);
                    latestLineBoxGlobalYPos = boxGlobalPoint.Y;
                }

                float lineGlobalBottom = lineBox.CachedLineBottom + latestLineBoxGlobalYPos;

                if (lineGlobalBottom <= globalY)
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

            foreach (var lineOrBox in GetLineOrBoxIterWalk(startLine))
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
            bool foundEndBox = false;
            foreach (var visit in GetLineOrBoxIterWalk(startLine))
            {
                if (visit.isLine)
                {
                    yield return visit.lineBox;
                }
                else if (visit.box == endBox)
                {
                    foundEndBox = true;
                    foreach (var visit2 in GetDeepBoxOrLineIter(endBox))
                    {
                        if (visit2.isLine)
                        {
                            yield return visit2.lineBox;
                        }
                    }
                }
                else
                {
                    if (foundEndBox)
                    {
                        break;
                    }
                }
            }
        }
        internal static IEnumerable<LineOrBoxVisit> GetDeepBoxOrLineIter(CssBox box)
        {
            yield return new LineOrBoxVisit(box);
            if (box.LineBoxCount > 0)
            {
                foreach (CssLineBox linebox in box.GetLineBoxIter())
                {
                    yield return new LineOrBoxVisit(linebox);
                }
            }
            else
            {
                if (box.ChildCount > 0)
                {
                    foreach (CssBox child in box.GetChildBoxIter())
                    {
                        foreach (var visit in GetDeepBoxOrLineIter(child))
                        {
                            yield return visit;
                        }
                    }
                }
            }
        }

        void SetupStartHitPoint(BoxHitChain startChain, IFonts ifonts)
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

                        run.FindSelectionPoint(ifonts,
                             startHit.localX,
                             out sel_index,
                             out sel_offset);

                        this.startHitRunCharIndex = sel_index;
                        //modify hitpoint
                        CssLineBox hostLine = (CssLineBox)startChain.GetHitInfo(startChain.Count - 2).hitObject;
                        hostLine.LineSelectionStart = (int)(run.Left + sel_offset);
                        this.startHitHostLine = hostLine;

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
                        CssLineBox startHitLine = FindNearestLine(box, startChain.RootGlobalY, 5);
                        if (startHitLine != null)
                        {
                            this.startHitHostLine = startHitLine;
                            startHitLine.LineSelectionStart = 0;
                        }
                        else
                        {
                            //if not found?
                            this.startHitHostLine = null;
                        }
                    } break;
                default:
                    {
                        throw new NotSupportedException();
                    } break;
            }
        }
        internal static IEnumerable<LineOrBoxVisit> GetLineOrBoxIterWalk(CssLineBox startLine)
        {
            //start at line
            //1. start line
            yield return new LineOrBoxVisit(startLine);
            CssLineBox curLine = startLine;

            //walk up and down the tree
            CssLineBox nextline = startLine.NextLine;
            while (nextline != null)
            {
                yield return new LineOrBoxVisit(nextline);
                nextline = nextline.NextLine;
            }
            //--------------------
            //no next line 
            //then step up  
            CssBox curBox = startLine.OwnerBox;
        RETRY:
            //ask for sibling
            CssBox level1Sibling = BoxUtils.GetNextSibling(curBox);
            while (level1Sibling != null)
            {
                foreach (var visit in GetDeepBoxOrLineIter(level1Sibling))
                {
                    yield return visit;
                }

                level1Sibling = BoxUtils.GetNextSibling(level1Sibling);
            }
            //--------------------
            //other further sibling
            //then step to parent of lineOwner
            if (curBox.ParentBox != null)
            {
                //if has parent                  
                curBox = curBox.ParentBox;
                goto RETRY;
            }
        }

        static PointF GetGlobalLocation(CssBox box)
        {
            float localX = box.LocalX;
            float localY = box.LocalY;
            CssBox parentBox = box.ParentBox;
            while (parentBox != null)
            {
                localX += parentBox.LocalX;
                localY += parentBox.LocalY;
                parentBox = parentBox.ParentBox;
            }
            return new PointF(localX, localY);
        }

        //static int dbugCounter = 0;

        void SetupEndHitPoint(BoxHitChain endChain, IFonts ifonts)
        {

            //dbugCounter++;
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

                        //if (run is CssTextRun)
                        //{
                        //    CssTextRun tt = (CssTextRun)run;
                        //    Console.WriteLine(dbugCounter + "r:" + run.dbugId + tt.Text + " (line:" + run.HostLine.dbugId + ",top=" + run.HostLine.CachedLineTop + ")");
                        //}
                        //else
                        //{
                        //    Console.WriteLine(dbugCounter + "r:" + run.dbugId + "(line:" + run.HostLine.dbugId + ",top=" + run.HostLine.CachedLineTop + ")");
                        //}

                        int sel_index;
                        int sel_offset;
                        run.FindSelectionPoint(ifonts,
                             endHit.localX,
                             out sel_index,
                             out sel_offset);
                        this.endHitRunCharIndex = sel_index;


                        CssLineBox endline = run.HostLine;
                        int xposOnEndLine = (int)(run.Left + sel_offset);

                        //find selection direction

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
                        //find selection direction 
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
                        CssBox hitBox = (CssBox)endHit.hitObject;
                        //find selection direction

                        //Console.WriteLine(dbugCounter + "B:" + hitBox.dbugId);

                        CssLineBox latestLine = null;
                        this.selectedLines = new List<CssLineBox>();
                        //convert to global position
                        float globalHitY = endChain.RootGlobalY;
                        //check if should use first line of this box                         
                        //or last line box this box


                        foreach (var line in GetLineWalkIter(this.startHitHostLine, hitBox))
                        {
                            if (line == startHitHostLine)
                            {
                                line.SelectPartialStart(startHitHostLine.LineSelectionStart);
                                selectedLines.Add(line);
                                latestLine = line;
                            }
                            else
                            {

                                //----------------------
                                //find cssbox
                                var ownerBox = line.OwnerBox;
                                PointF globalLocation = GetGlobalLocation(ownerBox);
                                float lineGlobalBottom = line.CachedLineBottom + globalLocation.Y;

                                //----------------------
                                if (lineGlobalBottom < globalHitY)
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