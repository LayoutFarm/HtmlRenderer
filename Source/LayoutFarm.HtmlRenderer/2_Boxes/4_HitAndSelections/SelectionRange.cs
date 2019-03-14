//MIT, 2014-present, WinterDev

using System;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;
namespace LayoutFarm.HtmlBoxes
{
    public class SelectionRange
    {
        //start line

        CssLineBox _startHitHostLine;
        List<CssLineBox> _selectedLines;
        bool _isValid = true;
        CssRun _startHitRun;
        int _startHitRunCharIndex;
        int _startLineBeginSelectionAtPixel;
        CssRun _endHitRun;
        int _endHitRunCharIndex;
        Rectangle _snapSelectionArea;


        internal SelectionRange(
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
                    CssBoxHitChain tmp = endChain;
                    endChain = startChain;
                    startChain = tmp;
                }
            }
            else
            {
                //across line 
                if (endChain.RootGlobalY < startChain.RootGlobalY)
                {    //swap
                    CssBoxHitChain tmp = endChain;
                    endChain = startChain;
                    startChain = tmp;
                }
            }

            //1.
            this.SetupStartHitPoint(startChain, ifonts);
            //2. 
            if (_startHitHostLine == null)
            {
                _isValid = false;
                return;
            }

            this.SetupEndHitPoint(startChain, endChain, ifonts);
            _snapSelectionArea = this.GetSelectionRectArea();
        }
        public Rectangle SnapSelectionArea => _snapSelectionArea;
        public bool IsValid => _isValid;
        //

        internal void ClearSelection()
        {
            if (_selectedLines != null)
            {
                for (int i = _selectedLines.Count - 1; i >= 0; --i)
                {
                    _selectedLines[i].SelectionSegment = null;
                }
                _selectedLines.Clear();
            }
            else
            {
                if (_startHitHostLine != null)
                {
                    _startHitHostLine.SelectionSegment = null;
                }
            }
            _startHitRun = _endHitRun = null;
            _startHitRunCharIndex = _endHitRunCharIndex = 0;
        }

        internal void CopyText(StringBuilder stbuilder)
        {
            //copy selected text to stbuilder 
            //this version just copy a plain text
            int j = _selectedLines.Count;
            for (int i = 0; i < j; ++i)
            {
                CssLineBox selLine = _selectedLines[i];
                SelectionSegment selSeg = selLine.SelectionSegment;
                switch (selSeg.Kind)
                {
                    case SelectionSegmentKind.Partial:
                        {
                            CssRun startRun = selSeg.StartHitRun;
                            CssRun endHitRun = selSeg.EndHitRun;
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
                                if (rr != null && _startHitRunCharIndex >= 0)
                                {
                                    string alltext = rr.Text;
                                    string sub1 = alltext.Substring(_startHitRunCharIndex, _endHitRunCharIndex - _startHitRunCharIndex);
                                    stbuilder.Append(sub1);
                                }
                            }
                            else
                            {
                                int runCount = selLine.RunCount;
                                bool foundStartRun = false;
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
                                        foundStartRun = true;
                                        string alltext = rr.Text;
                                        if (autoFirstRun)
                                        {
                                            stbuilder.Append(alltext);
                                        }
                                        else
                                        {
                                            if (_startHitRunCharIndex >= 0)
                                            {
                                                string sub1 = alltext.Substring(_startHitRunCharIndex);
                                                stbuilder.Append(sub1);
                                            }
                                        }
                                    }
                                    else if (rr == endHitRun)
                                    {
                                        string alltext = rr.Text;
                                        if (autoLastRun)
                                        {
                                            stbuilder.Append(alltext);
                                        }
                                        else
                                        {
                                            if (_endHitRunCharIndex >= 0)
                                            {
                                                string sub1 = alltext.Substring(0, _endHitRunCharIndex);
                                                stbuilder.Append(sub1);
                                            }
                                        }
                                        //stop
                                        break;
                                    }
                                    else
                                    {
                                        if (foundStartRun)
                                        {
                                            stbuilder.Append(rr.Text);
                                        }
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

        void SetupStartHitPoint(CssBoxHitChain startChain, ITextService textService)
        {
            //find global location of start point
            HitInfo startHit = startChain.GetLastHit();
            //-----------------------------
            _startHitRun = null;
            _startHitRunCharIndex = 0;
            switch (startHit.hitObjectKind)
            {
                case HitObjectKind.Run:
                    {
                        CssRun run = (CssRun)startHit.hitObject;
                        //-------------------------------------------------------
                        int sel_index;
                        int sel_offset;
                        run.FindSelectionPoint(textService,
                             startHit.localX,
                             out sel_index,
                             out sel_offset);
                        _startHitRunCharIndex = sel_index;
                        //modify hitpoint
                        _startHitHostLine = (CssLineBox)startChain.GetHitInfo(startChain.Count - 2).hitObject;
                        _startLineBeginSelectionAtPixel = (int)(run.Left + sel_offset);
                        _startHitRun = run;
                    }
                    break;
                case HitObjectKind.LineBox:
                    {
                        _startHitHostLine = (CssLineBox)startHit.hitObject;
                        _startLineBeginSelectionAtPixel = startHit.localX;
                        //make global            
                    }
                    break;
                case HitObjectKind.CssBox:
                    {
                        CssBox box = (CssBox)startHit.hitObject;
                        //find first nearest line at point   
                        CssLineBox startHitLine = FindNearestLine(box, startChain.RootGlobalY, 5);
                        _startLineBeginSelectionAtPixel = 0;
                        if (startHitLine != null)
                        {
                            _startHitHostLine = startHitLine;
                        }
                        else
                        {
                            //if not found?
                            _startHitHostLine = null;
                        }
                    }
                    break;
                default:
                    {
                        throw new NotSupportedException();
                    }
            }
        }


        void SetupEndHitPoint(CssBoxHitChain startChain, CssBoxHitChain endChain, ITextService textService)
        {
            //find global location of end point 
            HitInfo endHit = endChain.GetLastHit();
            int xposOnEndLine = 0;
            CssLineBox endline = null;
            int run_sel_offset = 0;
            //find endline first
            _endHitRunCharIndex = 0;
            _endHitRun = null;
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
                        endRun.FindSelectionPoint(textService,
                             endHit.localX,
                             out run_sel_index,
                             out run_sel_offset);
                        endline = endRun.HostLine;
                        xposOnEndLine = (int)(endRun.Left + run_sel_offset);
                        _endHitRunCharIndex = run_sel_index;
                        _endHitRun = endRun;
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
            _selectedLines = new List<CssLineBox>();
            if (_startHitHostLine == endline)
            {
                _selectedLines.Add(endline);
                _startHitHostLine.Select(_startLineBeginSelectionAtPixel, xposOnEndLine,
                        _startHitRun, _startHitRunCharIndex,
                        _endHitRun, _endHitRunCharIndex);
                return; //early exit here ***
            }
            //---------------------------------- 
            //select on different line 
            LineWalkVisitor lineWalkVisitor = null;

            if (FindCommonGround(startChain, endChain, out int breakAtLevel) && breakAtLevel > 0)
            {

                CssBlockRun hitBlockRun = endChain.GetHitInfo(breakAtLevel).hitObject as CssBlockRun;
                //multiple select 
                //1. first part        
                if (hitBlockRun != null)
                {
                    _startHitHostLine.Select(_startLineBeginSelectionAtPixel, (int)hitBlockRun.Left,
                     _startHitRun, _startHitRunCharIndex,
                     _endHitRun, _endHitRunCharIndex);
                    _selectedLines.Add(_startHitHostLine);
                    lineWalkVisitor = new LineWalkVisitor(hitBlockRun);
                }
                else
                {
                    _startHitHostLine.SelectPartialToEnd(_startLineBeginSelectionAtPixel, _startHitRun, _startHitRunCharIndex);
                    _selectedLines.Add(_startHitHostLine);
                    lineWalkVisitor = new LineWalkVisitor(_startHitHostLine);
                }
            }
            else
            {
                _startHitHostLine.SelectPartialToEnd(_startLineBeginSelectionAtPixel, _startHitRun, _startHitRunCharIndex);
                _selectedLines.Add(_startHitHostLine);
                lineWalkVisitor = new LineWalkVisitor(_startHitHostLine);
            }

            lineWalkVisitor.SetWalkTargetPosition(endChain.RootGlobalX, endChain.RootGlobalY);

#if DEBUG
            int dbugExpectedId = 1;
#endif
            lineWalkVisitor.Walk(endline, (lineCoverage, linebox, partialLineRun) =>
            {
#if DEBUG                
                //System.Diagnostics.Debug.WriteLine("sel:" + linebox.dbugId);
                if (dbugExpectedId != linebox.dbugId)
                {

                }
                dbugExpectedId++;
#endif
                switch (lineCoverage)
                {
                    case LineCoverage.EndLine:
                        {
                            //found end line  
                            linebox.SelectPartialFromStart(xposOnEndLine, _endHitRun, _endHitRunCharIndex);
                            _selectedLines.Add(linebox);
                        }
                        break;
                    case LineCoverage.PartialLine:
                        {
                            linebox.SelectPartialFromStart((int)partialLineRun.Right, _endHitRun, _endHitRunCharIndex);
                            _selectedLines.Add(linebox);
                        }
                        break;
                    case LineCoverage.FullLine:
                        {
                            //check if hitpoint is in the line area
                            linebox.SelectFull();
                            _selectedLines.Add(linebox);
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
                HitInfo startHitInfo = startChain.GetHitInfo(i);
                HitInfo endHitInfo = endChain.GetHitInfo(i);
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
            if (_selectedLines != null)
            {
                int j = _selectedLines.Count;

                if (j > 0)
                {
                    CssBox ownerCssBox = null;

                    float fx1 = 0, fy1 = 0; //left top
                    RectangleF selArea = RectangleF.Empty;

                    for (int i = 0; i < j; ++i)
                    {
                        CssLineBox line = _selectedLines[i];
                        if (line.OwnerBox != ownerCssBox)
                        {
                            ownerCssBox = line.OwnerBox;
                            ownerCssBox.GetGlobalLocationRelativeToRoot(out fx1, out fy1);
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
            readonly CssBlockRun _startBlockRun;
            readonly CssLineBox _startLineBox;
            public float _globalX;
            public float _globalY;


            //float __gy;
            //public float _globalY
            //{ //for debug
            //    get => __gy;
            //    set
            //    {
            //        if (value >= 150)
            //        {

            //        }
            //        __gy = value;
            //    }
            //}

            CssLineBox _currentVisitLineBox;
            float _targetX;
            float _targetY;

            public LineWalkVisitor(CssLineBox startLineBox)
            {
                PointF p = new PointF();
                startLineBox.OwnerBox.GetGlobalLocationRelativeToRoot(ref p);
                _startLineBox = startLineBox;


                //startLineBox.OwnerBox.GetGlobalLocation(out float endElemX, out float endElemY);
                //_globalX = endElemX;
                //_globalY = endElemY + startLineBox.CachedLineTop;

                _globalX = p.X;
                _globalY = p.Y + startLineBox.CachedLineTop;

            }
            public LineWalkVisitor(CssBlockRun startBlockRun)
            {

                PointF p = new PointF();
                startBlockRun.ContentBox.GetGlobalLocationRelativeToRoot(ref p);
                _globalX = p.X;
                _globalY = p.Y;
                _startBlockRun = startBlockRun;

                //startBlockRun.ContentBox.GetGlobalLocation(out float endElemX, out float endElemY);
                //_globalX = endElemX;
                //_globalY = endElemY;
                //_startBlockRun = startBlockRun;
            }
            public void SetWalkTargetPosition(float x, float y)
            {
                //if (y >= 150)
                //{
                //}
                _targetX = x;
                _targetY = y;
            }
            public void Walk(CssLineBox endLineBox, VisitLineDelegate del)
            {
                //2 cases :
                //1. start with BlockRun 
                //2. start with LineBox 
                InnerWalk(endLineBox,
                          del,
                          (_startBlockRun != null) ?
                                    GetLineWalkDownIter(this, _startBlockRun.ContentBox) :
                                    GetLineWalkDownAndUpIter(this, _startLineBox));
            }
            void InnerWalk(CssLineBox endLineBox, VisitLineDelegate del, IEnumerable<CssLineBox> lineIter)
            {
                //recursive 
                foreach (CssLineBox ln in lineIter)
                {
#if DEBUG
                    //System.Diagnostics.Debug.WriteLine("pre_sel:" + ln.dbugId);
#endif
                    _currentVisitLineBox = ln;
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
                return _targetY >= _globalY &&
                        _targetY < _globalY + _currentVisitLineBox.CacheLineHeight &&
                        _targetX >= _globalX &&
                        _targetX < _globalX + _currentVisitLineBox.CachedLineContentWidth;
            }


            ///// walk down and up
            ///// </summary>
            ///// <param name="startLine"></param>
            ///// <returns></returns>
            //static IEnumerable<CssLineBox> GetLineWalkDownAndUpIter(LineWalkVisitor visitor, CssLineBox startLine)
            //{
            //    float sx, sy;
            //    startLine.OwnerBox.GetGlobalLocation(out sx, out sy);
            //    CssLineBox curLine = startLine;
            //    //walk up and down the tree
            //    CssLineBox nextline = curLine.NextLine;
            //    while (nextline != null)
            //    {
            //        visitor._globalY = sy + startLine.CachedLineTop;
            //        yield return nextline;
            //        nextline = nextline.NextLine;
            //    }
            //    //--------------------
            //    //no next line 
            //    //then walk up  ***
            //    CssBox curBox = startLine.OwnerBox;

            //    RETRY://***
            //    CssBox level1Sibling = BoxHitUtils.GetNextSibling(curBox);
            //    while (level1Sibling != null)
            //    {
            //        level1Sibling.GetGlobalLocation(out sx, out sy);
            //        visitor._globalY = sy;
            //        //walk down
            //        foreach (CssLineBox line in GetLineWalkDownIter(visitor, level1Sibling))
            //        {
            //            yield return line;
            //        }

            //        level1Sibling = BoxHitUtils.GetNextSibling(level1Sibling);
            //    }
            //    //--------------------
            //    //other further sibling
            //    //then step to parent of lineOwner
            //    if (curBox.ParentBox != null)
            //    {
            //        //if has parent    
            //        //walk up***
            //        curBox = curBox.ParentBox;
            //        goto RETRY;
            //    }
            //}




            /// walk down and up
            /// </summary>
            /// <param name="startLine"></param>
            /// <returns></returns>
            static IEnumerable<CssLineBox> GetLineWalkDownAndUpIter(LineWalkVisitor visitor, CssLineBox startLine)
            {
                PointF p = new PointF();
                startLine.OwnerBox.GetGlobalLocationRelativeToRoot(ref p);
                CssLineBox curLine = startLine;
                //walk up and down the tree
                CssLineBox nextline = curLine.NextLine;
                while (nextline != null)
                {
                    visitor._globalY = p.Y + startLine.CachedLineTop;
                    yield return nextline;
                    nextline = nextline.NextLine;
                }
                //--------------------
                //no next line 
                //then walk up  ***
                CssBox curBox = startLine.OwnerBox;

                RETRY://***
                CssBox level1Sibling = BoxHitUtils.GetNextSibling(curBox);
                while (level1Sibling != null)
                {
                    p = new PointF();
                    level1Sibling.GetGlobalLocationRelativeToRoot(ref p);
                    visitor._globalY = p.Y;
                    //walk down
                    foreach (CssLineBox line in GetLineWalkDownIter(visitor, level1Sibling))
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
                float y = visitor._globalY;
                if (box.LineBoxCount > 0)
                {
                    foreach (CssLineBox linebox in box.GetLineBoxIter())
                    {
                        visitor._globalY = y + linebox.CachedLineTop;
                        yield return linebox;
                    }
                }
                else
                {
                    //element based
                    foreach (CssBox childbox in box.GetChildBoxIter())
                    {
                        visitor._globalY = y + childbox.LocalY;
                        //recursive
                        foreach (var linebox in GetLineWalkDownIter(visitor, childbox))
                        {
                            yield return linebox;
                        }
                    }
                }

                visitor._globalY = y;
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
            lineBox.SelectionSegment = SelectionSegment.FullLine;
        }

        public static void SelectPartialToEnd(this CssLineBox lineBox, int startAtPx, CssRun startRun, int startRunIndex)
        {
            //from startAt to end of line 

            lineBox.SelectionSegment = new SelectionSegment(startAtPx, (int)lineBox.CachedLineContentWidth - startAtPx)
            {
                StartHitRun = startRun,
                StartHitCharIndex = startRunIndex
            };
        }
        public static void SelectPartialFromStart(this CssLineBox lineBox, int endAtPx, CssRun endRun, int endRunIndex)
        {
            //from start of line to endAt             

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