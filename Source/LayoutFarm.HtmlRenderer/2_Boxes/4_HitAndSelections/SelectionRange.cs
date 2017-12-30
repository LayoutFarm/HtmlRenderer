//MIT, 2014-2017, WinterDev

using System;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;
namespace LayoutFarm.HtmlBoxes
{
    public class SelectionRange
    {
        //start line

        CssLineBox startHitHostLine;
        List<CssLineBox> selectedLines;
        bool isValid = true;
        CssRun startHitRun;
        int startHitRunCharIndex;
        int startLineBeginSelectionAtPixel;
        CssRun endHitRun;
        int endHitRunCharIndex;
        Rectangle snapSelectionArea;
        public SelectionRange(
            CssBoxHitChain startChain,
            CssBoxHitChain endChain,
            ITextService ifonts)
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

            //1.
            this.SetupStartHitPoint(startChain, ifonts);
            //2. 
            if (this.startHitHostLine == null)
            {
                this.isValid = false;
                return;
            }


            this.SetupEndHitPoint(startChain, endChain, ifonts);
            this.snapSelectionArea = this.GetSelectionRectArea();
        }
        public Rectangle SnapSelectionArea { get { return this.snapSelectionArea; } }
        public bool IsValid
        {
            get { return this.isValid; }
        }

        public void ClearSelection()
        {
            if (this.selectedLines != null)
            {
                for (int i = selectedLines.Count - 1; i >= 0; --i)
                {
                    this.selectedLines[i].SelectionSegment = null;
                }
                this.selectedLines.Clear();
            }
            else
            {
                if (this.startHitHostLine != null)
                {
                    this.startHitHostLine.SelectionSegment = null;
                }
            }
            this.startHitRun = this.endHitRun = null;
            this.startHitRunCharIndex = this.endHitRunCharIndex = 0;
        }

        public void CopyText(StringBuilder stbuilder)
        {
            //copy selected text to stbuilder 
            //this version just copy a plain text
            int j = selectedLines.Count;
            for (int i = 0; i < j; ++i)
            {
                var selLine = selectedLines[i];
                var selSeg = selLine.SelectionSegment;
                switch (selSeg.Kind)
                {
                    case SelectionSegmentKind.Partial:
                        {
                            var startRun = selSeg.StartHitRun;
                            var endHitRun = selSeg.EndHitRun;
                            bool autoFirstRun = false;
                            bool autoLastRun = false;
                            if (startRun == null)
                            {
                                startRun = selLine.GetFirstRun();
                                autoFirstRun = true;
                            }
                            if (endHitRun == null)
                            {
                                endHitRun = selLine.GetLastRun();
                                autoLastRun = true;
                            }

                            if (startRun == endHitRun)
                            {
                                var rr = startRun as CssTextRun;
                                if (this.startHitRunCharIndex >= 0)
                                {
                                    var alltext = rr.Text;
                                    var sub1 = alltext.Substring(this.startHitRunCharIndex, this.endHitRunCharIndex - this.startHitRunCharIndex);
                                    stbuilder.Append(sub1);
                                }
                            }
                            else
                            {
                                int runCount = selLine.RunCount;
                                for (int n = 0; n < runCount; ++n)
                                {
                                    //temp fix here!
                                    //TODO: review this for other cssrun type
                                    var rr = selLine.GetRun(n) as CssTextRun;
                                    if (rr == null)
                                    {
                                        continue;
                                    }
                                    if (rr == startRun)
                                    {
                                        var alltext = rr.Text;
                                        if (autoFirstRun)
                                        {
                                            stbuilder.Append(alltext);
                                        }
                                        else
                                        {
                                            if (this.startHitRunCharIndex >= 0)
                                            {
                                                var sub1 = alltext.Substring(this.startHitRunCharIndex);
                                                stbuilder.Append(sub1);
                                            }
                                        }
                                    }
                                    else if (rr == endHitRun)
                                    {
                                        var alltext = rr.Text;
                                        if (autoLastRun)
                                        {
                                            stbuilder.Append(alltext);
                                        }
                                        else
                                        {
                                            if (this.endHitRunCharIndex >= 0)
                                            {
                                                var sub1 = alltext.Substring(0, this.endHitRunCharIndex);
                                                stbuilder.Append(sub1);
                                            }
                                        }
                                        //stop
                                        break;
                                    }
                                    else
                                    {
                                        stbuilder.Append(rr.Text);
                                    }
                                }
                            }
                        }
                        break;
                    default:
                        {
                            int runCount = selLine.RunCount;
                            for (int n = 0; n < runCount; ++n)
                            {
                                var r = selLine.GetRun(n) as CssTextRun;
                                if (r != null)
                                {
                                    stbuilder.Append(r.Text);
                                }
                            }
                        }
                        break;
                }

                if (i < j - 1)
                {
                    //if not lastline
                    stbuilder.AppendLine();
                }
            }
        }

        void SetupStartHitPoint(CssBoxHitChain startChain, ITextService ifonts)
        {
            //find global location of start point
            HitInfo startHit = startChain.GetLastHit();
            //-----------------------------
            this.startHitRun = null;
            this.startHitRunCharIndex = 0;
            switch (startHit.hitObjectKind)
            {
                case HitObjectKind.Run:
                    {
                        CssRun run = (CssRun)startHit.hitObject;
                        //-------------------------------------------------------
                        int sel_index;
                        int sel_offset;
                        run.FindSelectionPoint(ifonts,
                             startHit.localX,
                             out sel_index,
                             out sel_offset);
                        this.startHitRunCharIndex = sel_index;
                        //modify hitpoint
                        this.startHitHostLine = (CssLineBox)startChain.GetHitInfo(startChain.Count - 2).hitObject;
                        this.startLineBeginSelectionAtPixel = (int)(run.Left + sel_offset);
                        this.startHitRun = run;
                    }
                    break;
                case HitObjectKind.LineBox:
                    {
                        this.startHitHostLine = (CssLineBox)startHit.hitObject;
                        this.startLineBeginSelectionAtPixel = startHit.localX;
                        //make global            
                    }
                    break;
                case HitObjectKind.CssBox:
                    {
                        CssBox box = (CssBox)startHit.hitObject;
                        //find first nearest line at point   
                        CssLineBox startHitLine = FindNearestLine(box, startChain.RootGlobalY, 5);
                        this.startLineBeginSelectionAtPixel = 0;
                        if (startHitLine != null)
                        {
                            this.startHitHostLine = startHitLine;
                        }
                        else
                        {
                            //if not found?
                            this.startHitHostLine = null;
                        }
                    }
                    break;
                default:
                    {
                        throw new NotSupportedException();
                    }
            }
        }


        void SetupEndHitPoint(CssBoxHitChain startChain, CssBoxHitChain endChain, ITextService ifonts)
        {
            //find global location of end point 
            HitInfo endHit = endChain.GetLastHit();
            int xposOnEndLine = 0;
            CssLineBox endline = null;
            int run_sel_offset = 0;
            //find endline first
            this.endHitRunCharIndex = 0;
            this.endHitRun = null;
            switch (endHit.hitObjectKind)
            {
                default:
                    {
                        throw new NotSupportedException();
                    }
                case HitObjectKind.Run:
                    {
                        CssRun endRun = (CssRun)endHit.hitObject;
                        //if (endRun.Text != null && endRun.Text.Contains("Jose"))
                        //{
                        //}

                        int run_sel_index;
                        endRun.FindSelectionPoint(ifonts,
                             endHit.localX,
                             out run_sel_index,
                             out run_sel_offset);
                        endline = endRun.HostLine;
                        xposOnEndLine = (int)(endRun.Left + run_sel_offset);
                        this.endHitRunCharIndex = run_sel_index;
                        this.endHitRun = endRun;
                    }
                    break;
                case HitObjectKind.LineBox:
                    {
                        endline = (CssLineBox)endHit.hitObject;
                        xposOnEndLine = endHit.localX;
                    }
                    break;
                case HitObjectKind.CssBox:
                    {
                        CssBox hitBox = (CssBox)endHit.hitObject;
                        endline = FindNearestLine(hitBox, endChain.RootGlobalY, 5);
                        xposOnEndLine = endHit.localX;
                    }
                    break;
            }

#if DEBUG
            if (xposOnEndLine == 0)
            {
            }
#endif

            //----------------------------------
            this.selectedLines = new List<CssLineBox>();
            if (startHitHostLine == endline)
            {
                this.selectedLines.Add(endline);
                startHitHostLine.Select(startLineBeginSelectionAtPixel, xposOnEndLine,
                        this.startHitRun, this.startHitRunCharIndex,
                        this.endHitRun, this.endHitRunCharIndex);
                return; //early exit here ***
            }
            //---------------------------------- 
            //select on different line 
            LineWalkVisitor lineWalkVisitor = null;
            int breakAtLevel;
            if (FindCommonGround(startChain, endChain, out breakAtLevel) && breakAtLevel > 0)
            {
                var hit1 = endChain.GetHitInfo(breakAtLevel).hitObject;
                var hitBlockRun = hit1 as CssBlockRun;
                //multiple select 
                //1. first part        
                if (hitBlockRun != null)
                {
                    startHitHostLine.Select(startLineBeginSelectionAtPixel, (int)hitBlockRun.Left,
                     this.startHitRun, this.startHitRunCharIndex,
                     this.endHitRun, this.endHitRunCharIndex);
                    selectedLines.Add(this.startHitHostLine);
                    lineWalkVisitor = new LineWalkVisitor(hitBlockRun);
                }
                else
                {
                    startHitHostLine.SelectPartialToEnd(startLineBeginSelectionAtPixel, this.startHitRun, this.startHitRunCharIndex);
                    selectedLines.Add(this.startHitHostLine);
                    lineWalkVisitor = new LineWalkVisitor(startHitHostLine);
                }
            }
            else
            {
                startHitHostLine.SelectPartialToEnd(startLineBeginSelectionAtPixel, this.startHitRun, this.startHitRunCharIndex);
                selectedLines.Add(this.startHitHostLine);
                lineWalkVisitor = new LineWalkVisitor(startHitHostLine);
            }

            lineWalkVisitor.SetWalkTargetPosition(endChain.RootGlobalX, endChain.RootGlobalY);
            lineWalkVisitor.Walk(endline, (lineCoverage, linebox, partialLineRun) =>
            {
                switch (lineCoverage)
                {
                    case LineCoverage.EndLine:
                        {
                            //found end line  
                            linebox.SelectPartialFromStart(xposOnEndLine, this.endHitRun, this.endHitRunCharIndex);
                            selectedLines.Add(linebox);
                        }
                        break;
                    case LineCoverage.PartialLine:
                        {
                            linebox.SelectPartialFromStart((int)partialLineRun.Right, this.endHitRun, this.endHitRunCharIndex);
                            selectedLines.Add(linebox);
                        }
                        break;
                    case LineCoverage.FullLine:
                        {
                            //check if hitpoint is in the line area
                            linebox.SelectFull();
                            selectedLines.Add(linebox);
                        }
                        break;
                }
            });
        }
        static bool FindCommonGround(CssBoxHitChain startChain, CssBoxHitChain endChain, out int breakAtLevel)
        {
            //find common ground of startChain and endChain
            int startChainCount = startChain.Count;
            int endChainCount = endChain.Count;
            int lim = Math.Min(startChainCount, endChainCount);
            //from root to leave
            breakAtLevel = 0;
            for (int i = 0; i < lim; ++i)
            {
                var startHitInfo = startChain.GetHitInfo(i);
                var endHitInfo = endChain.GetHitInfo(i);
                if (startHitInfo.hitObject != endHitInfo.hitObject)
                {
                    //found diff here
                    breakAtLevel = i;
                    break;
                }
            }
            //----------------------------
            //check  
            //return isDeepDown = endChainCount > startChainCount && (breakAtLevel == startChainCount - 1);
            return endChainCount > startChainCount && (breakAtLevel == startChainCount - 1);
        }

        static bool IsOnTheSameLine(CssBoxHitChain startChain, CssBoxHitChain endChain)
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
                        return null;
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
        static CssLineBox FindNearestLine(CssBox startBox, int globalY, int yRange)
        {
            CssLineBox latestLine = null;
            CssBox latestLineBoxOwner = null;
            float latestLineBoxGlobalYPos = 0;
            foreach (CssLineBox lineBox in BoxHitUtils.GetDeepDownLineBoxIter(startBox))
            {
                if (lineBox.CacheLineHeight == 0)
                {
                    continue;
                }
                if (latestLineBoxOwner != lineBox.OwnerBox)
                {
                    //find global position of box
                    latestLineBoxOwner = lineBox.OwnerBox;
                    //TODO: review here , duplicate GetGlobalLocation 
                    float gx, gy;
                    latestLineBoxOwner.GetGlobalLocation(out gx, out gy);
                    latestLineBoxGlobalYPos = gy;
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

        Rectangle GetSelectionRectArea()
        {
            if (selectedLines != null)
            {
                int j = selectedLines.Count;
                //first 
                if (j > 0)
                {
                    CssBox ownerCssBox = null;
                    CssBox rootbox = null;
                    float fx1 = 0, fy1 = 0; //left top
                    RectangleF selArea = RectangleF.Empty;
                    //if (j ==1)
                    //{ 
                    //}
                    for (int i = 0; i < j; ++i)
                    {
                        var line = selectedLines[i];
                        if (line.OwnerBox != ownerCssBox)
                        {
                            ownerCssBox = line.OwnerBox;
                            rootbox = ownerCssBox.GetGlobalLocationRelativeToRoot(out fx1, out fy1);
                        }
                        if (i == 0)
                        {
                            selArea = new RectangleF(fx1,
                                fy1 + line.CachedLineTop,
                                line.CachedLineContentWidth,
                                line.CacheLineHeight);
                        }
                        else
                        {
                            selArea = RectangleF.Union(selArea,
                                  new RectangleF(fx1,
                                  fy1 + line.CachedLineTop,
                                  line.CachedLineContentWidth,
                                  line.CacheLineHeight));
                        }
                    }
                    //if want to debug then send a big rect

                    //Console.WriteLine(new Rectangle((int)selArea.X, (int)selArea.Y, (int)selArea.Width, (int)selArea.Height).ToString());
                    //return new Rectangle(0, 0, 800, 600);
                    return new Rectangle((int)selArea.X, (int)selArea.Y, (int)selArea.Width, (int)selArea.Height);
                }
            }
            return Rectangle.Empty;
        }
        //======================================================================================
        class LineWalkVisitor
        {
            readonly CssBlockRun startBlockRun;
            readonly CssLineBox startLineBox;
            public float globalX;
            public float globalY;
            CssLineBox currentVisitLineBox;
            float targetX;
            float targetY;
            public LineWalkVisitor(CssLineBox startLineBox)
            {
                this.startLineBox = startLineBox;
                float endElemX = 0, endElemY = 0;
                startLineBox.OwnerBox.GetGlobalLocation(out endElemX, out endElemY);
                this.globalX = endElemX;
                this.globalY = endElemY + startLineBox.CachedLineTop;
            }
            public LineWalkVisitor(CssBlockRun startBlockRun)
            {
                float endElemX = 0, endElemY = 0;
                startBlockRun.ContentBox.GetGlobalLocation(out endElemX, out endElemY);
                this.globalX = endElemX;
                this.globalY = endElemY;
                this.startBlockRun = startBlockRun;
            }
            public void SetWalkTargetPosition(float x, float y)
            {
                this.targetX = x;
                this.targetY = y;
            }
            public void Walk(CssLineBox endLineBox, VisitLineDelegate del)
            {
                //2 cases :
                //1. start with BlockRun 
                //2. start with LineBox 
                InnerWalk(endLineBox,
                          del,
                          (startBlockRun != null) ?
                                    GetLineWalkDownIter(this, startBlockRun.ContentBox) :
                                    GetLineWalkDownAndUpIter(this, startLineBox));
            }
            void InnerWalk(CssLineBox endLineBox, VisitLineDelegate del, IEnumerable<CssLineBox> lineIter)
            {
                //recursive


                foreach (var ln in lineIter)
                {
                    this.currentVisitLineBox = ln;
                    if (ln == endLineBox)
                    {
                        del(LineCoverage.EndLine, ln, null);
                        //found endline 
                        return;
                    }
                    else if (this.IsWalkTargetInCurrentLineArea())
                    {
                        int j = ln.RunCount;
                        bool isOK = false;
                        for (int i = 0; i < j && !isOK; ++i)
                        {
                            var run3 = ln.GetRun(i) as CssBlockRun;
                            if (run3 == null) continue;
                            //recursive here 
                            InnerWalk(endLineBox, del, GetLineWalkDownIter(this, run3.ContentBox));
                            if (i > 0)
                            {
                                del(LineCoverage.PartialLine, ln, ln.GetRun(i - 1));
                            }
                            isOK = true;
                            break;
                        }
                    }
                    else
                    {
                        del(LineCoverage.FullLine, ln, null);
                    }
                }
            }

            public bool IsWalkTargetInCurrentLineArea()
            {
                return targetY >= this.globalY &&
                        targetY < this.globalY + currentVisitLineBox.CacheLineHeight &&
                        targetX >= this.globalX &&
                        targetX < this.globalX + currentVisitLineBox.CachedLineContentWidth;
            }
            /// walk down and up
            /// </summary>
            /// <param name="startLine"></param>
            /// <returns></returns>
            static IEnumerable<CssLineBox> GetLineWalkDownAndUpIter(LineWalkVisitor visitor, CssLineBox startLine)
            {
                float sx, sy;
                startLine.OwnerBox.GetGlobalLocation(out sx, out sy);
                CssLineBox curLine = startLine;
                //walk up and down the tree
                CssLineBox nextline = curLine.NextLine;
                while (nextline != null)
                {
                    visitor.globalY = sy + startLine.CachedLineTop;
                    yield return nextline;
                    nextline = nextline.NextLine;
                }
                //--------------------
                //no next line 
                //then walk up  ***
                CssBox curBox = startLine.OwnerBox;
            RETRY:
                CssBox level1Sibling = BoxHitUtils.GetNextSibling(curBox);
                while (level1Sibling != null)
                {
                    level1Sibling.GetGlobalLocation(out sx, out sy);
                    visitor.globalY = sy;
                    //walk down
                    foreach (var line in GetLineWalkDownIter(visitor, level1Sibling))
                    {
                        yield return line;
                    }

                    level1Sibling = BoxHitUtils.GetNextSibling(level1Sibling);
                }
                //--------------------
                //other further sibling
                //then step to parent of lineOwner
                if (curBox.ParentBox != null)
                {
                    //if has parent    
                    //walk up***
                    curBox = curBox.ParentBox;
                    goto RETRY;
                }
            }

            static IEnumerable<CssLineBox> GetLineWalkDownIter(LineWalkVisitor visitor, CssBox box)
            {
                //recursive
                float y = visitor.globalY;
                if (box.LineBoxCount > 0)
                {
                    foreach (var linebox in box.GetLineBoxIter())
                    {
                        visitor.globalY = y + linebox.CachedLineTop;
                        yield return linebox;
                    }
                }
                else
                {
                    //element based
                    foreach (var childbox in box.GetChildBoxIter())
                    {
                        visitor.globalY = y + childbox.LocalY;
                        //recursive
                        foreach (var linebox in GetLineWalkDownIter(visitor, childbox))
                        {
                            yield return linebox;
                        }
                    }
                }

                visitor.globalY = y;
            }
        }

        delegate void VisitLineDelegate(LineCoverage lineCoverage, CssLineBox linebox, CssRun partialRun);
        enum LineCoverage
        {
            FullLine,
            EndLine,
            PartialLine
        }
    }



    static class CssLineBoxExtension
    {
        public static void SelectFull(this CssLineBox lineBox)
        {
            //full line selection 
            //lineBox.SelectionStartAt = 0;
            //lineBox.SelectionWidth = (int)lineBox.CachedLineContentWidth;
            lineBox.SelectionSegment = SelectionSegment.FullLine;
        }

        public static void SelectPartialToEnd(this CssLineBox lineBox, int startAtPx, CssRun startRun, int startRunIndex)
        {
            //from startAt to end of line

            //lineBox.SelectionStartAt = startAtPx;
            //lineBox.SelectionWidth = (int)lineBox.CachedLineContentWidth - startAtPx;

            lineBox.SelectionSegment = new SelectionSegment(startAtPx, (int)lineBox.CachedLineContentWidth - startAtPx)
            {
                StartHitRun = startRun,
                StartHitCharIndex = startRunIndex
            };
        }
        public static void SelectPartialFromStart(this CssLineBox lineBox, int endAtPx, CssRun endRun, int endRunIndex)
        {
            //from start of line to endAt              

            //lineBox.SelectionStartAt = 0;
            //lineBox.SelectionWidth = endAtPx;

            lineBox.SelectionSegment = new SelectionSegment(0, endAtPx)
            {
                EndHitRun = endRun,
                EndHitCharIndex = endRunIndex
            };
        }

        public static void Select(this CssLineBox lineBox, int startAtPx, int endAt,
            CssRun startRun, int startRunIndex,
            CssRun endRun, int endRunIndex)
        {
            lineBox.SelectionSegment = new SelectionSegment(startAtPx, endAt - startAtPx)
            {
                StartHitRun = startRun,
                StartHitCharIndex = startRunIndex,
                EndHitRun = endRun,
                EndHitCharIndex = endRunIndex
            };
        }
    }
}