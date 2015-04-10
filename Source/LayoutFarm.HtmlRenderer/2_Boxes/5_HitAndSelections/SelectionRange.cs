//BSD 2014 ,WinterDev 
using System;
using System.Collections.Generic;
using PixelFarm.Drawing;

namespace LayoutFarm.HtmlBoxes
{

    public class SelectionRange
    {
        //start line

        CssLineBox startHitHostLine;
        List<CssLineBox> selectedLines;
        bool isValid = true;
        CssBoxHitChain tmpStartChain = null;

        int startLineBeginSelectionAtPixel;

        public SelectionRange(CssBoxHitChain startChain,
            CssBoxHitChain endChain,
            IFonts ifonts)
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

            this.tmpStartChain = startChain;
            this.SetupEndHitPoint(endChain, ifonts);
        }



        public bool IsValid
        {
            get { return this.isValid; }
        }

        public void ClearSelectionStatus()
        {

            if (this.selectedLines != null)
            {
                for (int i = selectedLines.Count - 1; i >= 0; --i)
                {
                    this.selectedLines[i].LineSelectionSegment = null;
                }
                this.selectedLines.Clear();
            }
            else
            {
                if (this.startHitHostLine != null)
                {
                    this.startHitHostLine.LineSelectionSegment = null;
                }

            }
        }

        void SetupStartHitPoint(CssBoxHitChain startChain, IFonts ifonts)
        {

            //find global location of start point
            HitInfo startHit = startChain.GetLastHit();
            //-----------------------------
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


                        //modify hitpoint
                        this.startHitHostLine = (CssLineBox)startChain.GetHitInfo(startChain.Count - 2).hitObject;
                        this.startLineBeginSelectionAtPixel = (int)(run.Left + sel_offset);

                    } break;
                case HitObjectKind.LineBox:
                    {

                        this.startHitHostLine = (CssLineBox)startHit.hitObject;
                        this.startLineBeginSelectionAtPixel = startHit.localX;
                        //make global            
                    } break;
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
                    } break;
                default:
                    {
                        throw new NotSupportedException();
                    }
            }
        }


        void SetupEndHitPoint(CssBoxHitChain endChain, IFonts ifonts)
        {

            //find global location of end point 
            HitInfo endHit = endChain.GetLastHit();
            switch (endHit.hitObjectKind)
            {
                default:
                    {
                        throw new NotSupportedException();
                    }
                case HitObjectKind.Run:
                    {

                        CssRun endRun = (CssRun)endHit.hitObject;
                        if (endRun.Text.Contains("555"))
                        {
                        }
                        //---------------------------------
                        int run_sel_index;
                        int run_sel_offset;
                        endRun.FindSelectionPoint(ifonts,
                             endHit.localX,
                             out run_sel_index,
                             out run_sel_offset);

                        //1. find endline 
                        CssLineBox endline = endRun.HostLine;

                        //find selection direction 
                        if (startHitHostLine == endline)
                        {
                            //on the sameline
                            this.selectedLines = new List<CssLineBox>();
                            this.selectedLines.Add(endline);
                            int xposOnEndLine = (int)(endRun.Left + run_sel_offset);
                            endline.LineSelectionSegment = new SelectionSegment(startLineBeginSelectionAtPixel, xposOnEndLine - startLineBeginSelectionAtPixel);

                        }
                        else
                        {

                            //select on different line 
                            CommonGroundInfo commonGroundInfo = FindCommonGround(this.tmpStartChain, endChain);
                            this.selectedLines = new List<CssLineBox>();
                            if (commonGroundInfo.isDeepDown)
                            {

                                //TODO:  review this
                                HitInfo hitOnEndLine = endChain.GetHitInfo(commonGroundInfo.breakAtLevel);
                                //mean it found block run
                                var hitBlockRun = hitOnEndLine.hitObject as CssBlockRun;
                                if (hitBlockRun != null)
                                {

                                    CssLineBox startLineBox = this.startHitHostLine;
                                    //multiple select
                                    var multiSelectionSegment = new MultiSegmentPerLine();
                                    startLineBox.LineSelectionSegment = multiSelectionSegment;

                                    //1. first part
                                    var firstSegment = new SelectionSegment(startLineBeginSelectionAtPixel, (int)hitBlockRun.Left - startLineBeginSelectionAtPixel);
                                    multiSelectionSegment.AddSubSegment(firstSegment);
                                    selectedLines.Add(startLineBox);


                                    float endElemX = 0, endElemY = 0;
                                    hitBlockRun.ContentBox.GetElementGlobalLocation(out endElemX, out endElemY);
                                    float lineX = endElemX;
                                    float lineY = endElemY + endline.CachedLineTop;

                                    var lineWalkVisitor = new LineWalkVisitor();
                                    lineWalkVisitor.globalX = lineX;
                                    lineWalkVisitor.globalY = lineY;

                                    //----------------------------------------------------------------
                                    foreach (var linebox in GetLineWalkIter(lineWalkVisitor, hitBlockRun.ContentBox))
                                    {
                                        if (linebox == endline)
                                        {
                                            //found end line  
                                            linebox.SelectPartialEnd((int)(endRun.Left + run_sel_offset));
                                            selectedLines.Add(linebox);
                                            break;
                                        }
                                        else
                                        {
                                            if (endChain.RootGlobalX >= lineWalkVisitor.globalX &&
                                                endChain.RootGlobalY >= lineWalkVisitor.globalY &&
                                                endChain.RootGlobalX < lineWalkVisitor.globalX + linebox.CachedLineContentWidth &&
                                                endChain.RootGlobalY < lineWalkVisitor.globalY + linebox.CacheLineHeight)
                                            {

                                                //explore all run in this line 

                                                int j = linebox.RunCount;
                                                bool isOK = false;
                                                for (int i = 0; i < j && !isOK; ++i)
                                                {
                                                    var run2 = linebox.GetRun(i);
                                                    var run3 = run2 as CssBlockRun;

                                                    if (run3 != null)
                                                    {
                                                        //recursive here

                                                        foreach (var line2 in GetLineWalkIter(lineWalkVisitor, run3.ContentBox))
                                                        {
                                                            if (line2 == endline)
                                                            {
                                                                //found here!
                                                                //add line outter lnie 
                                                                if (i > 0)
                                                                {
                                                                    linebox.SelectPartialEnd((int)linebox.GetRun(i - 1).Right);
                                                                    selectedLines.Add(linebox);
                                                                }
                                                                line2.SelectPartialEnd(run_sel_offset);
                                                                selectedLines.Add(line2);
                                                                //linebox.SelectPartialEnd((int)run2.Left + sel_offset);
                                                                isOK = true;
                                                                break;
                                                            }
                                                            else
                                                            {
                                                                line2.SelectFull();
                                                                selectedLines.Add(line2);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {   //check if hitpoint is in the line area
                                                linebox.SelectFull();
                                                selectedLines.Add(linebox);
                                            }

                                        }
                                    }
                                    //----------------------------------------------------------------
                                }
                                else
                                {
                                    //Console.WriteLine("d2." + dbugCount01++);
                                }
                            }
                            else
                            {
                                int xposOnEndLine = (int)(endRun.Left + run_sel_offset);
                                //1. select all in start line      
                                CssLineBox startLineBox = this.startHitHostLine;

                               
                                bool isOK = false;
                                foreach (var visit in WalkDownAndUp(startLineBox))
                                {
                                    if (isOK)
                                    {
                                        break;
                                    }
                                    //only linebox?0
                                    switch (visit.Kind)
                                    {
                                        case VisitMarkerKind.Line:
                                            {
                                                var linebox = visit.visitObject as CssLineBox;
                                                if (linebox == startLineBox)
                                                {
                                                    linebox.SelectPartialStart(startLineBeginSelectionAtPixel);
                                                    selectedLines.Add(linebox);
                                                }
                                                else if (linebox == endline)
                                                {
                                                    //TODO: review this, temp fix here 
                                                    linebox.SelectPartialEnd(xposOnEndLine);
                                                    selectedLines.Add(linebox); 
                                                    isOK = true;

                                                }
                                                else
                                                {
                                                    //inbetween
                                                    linebox.SelectFull();
                                                    selectedLines.Add(linebox);
                                                }
                                            } break;
                                       
                                    }

                                }
                            }
                        }


                    } break;
                case HitObjectKind.LineBox:
                    {
                        //1. find endline

                        CssLineBox endline = (CssLineBox)endHit.hitObject;
                        //find selection direction 
                        if (this.startHitHostLine == endline)
                        {
                            //sameline
                            endline.LineSelectionSegment = new SelectionSegment(startLineBeginSelectionAtPixel, endHit.localX - startLineBeginSelectionAtPixel);
                        }
                        else
                        {
                            //different line
                            CommonGroundInfo commonGroundInfo = FindCommonGround(this.tmpStartChain, endChain);
                            this.selectedLines = new List<CssLineBox>();
                            if (commonGroundInfo.isDeepDown)
                            {

                                //TODO:  review this
                                HitInfo hitOnEndLine = endChain.GetHitInfo(commonGroundInfo.breakAtLevel);
                                //mean it found block run
                                var hitBlockRun = hitOnEndLine.hitObject as CssBlockRun;
                                if (hitBlockRun != null)
                                {
                                    //Console.WriteLine("d1." + dbugCount01++);
                                    CssLineBox startLineBox = this.startHitHostLine;
                                    //multiple select
                                    var multiSelectionSegment = new MultiSegmentPerLine();
                                    startLineBox.LineSelectionSegment = multiSelectionSegment;

                                    //1. first part
                                    var firstSegment = new SelectionSegment(startLineBeginSelectionAtPixel, (int)hitBlockRun.Left - startLineBeginSelectionAtPixel);
                                    multiSelectionSegment.AddSubSegment(firstSegment);
                                    selectedLines.Add(startLineBox);

                                    float endElemX = 0, endElemY = 0;
                                    hitBlockRun.ContentBox.GetElementGlobalLocation(out endElemX, out endElemY);
                                    float lineX = endElemX;
                                    float lineY = endElemY + endline.CachedLineTop;

                                    var lineWalkVisitor = new LineWalkVisitor();
                                    lineWalkVisitor.globalX = lineX;
                                    lineWalkVisitor.globalY = lineY;

                                    foreach (var linebox in GetLineWalkIter(lineWalkVisitor, hitBlockRun.ContentBox))
                                    {


                                        if (linebox == endline)
                                        {
                                            //found end line  
                                            linebox.SelectPartialEnd(endHit.localX - startLineBeginSelectionAtPixel);
                                            selectedLines.Add(linebox);
                                            break;
                                        }
                                        else
                                        {
                                            if (endChain.RootGlobalX >= lineWalkVisitor.globalX &&
                                                endChain.RootGlobalY >= lineWalkVisitor.globalY &&
                                                endChain.RootGlobalX < lineWalkVisitor.globalX + linebox.CachedLineContentWidth &&
                                                endChain.RootGlobalY < lineWalkVisitor.globalY + linebox.CacheLineHeight)
                                            {

                                                //explore all run in this line 

                                                int j = linebox.RunCount;
                                                bool isOK = false;
                                                for (int i = 0; i < j && !isOK; ++i)
                                                {
                                                    var run2 = linebox.GetRun(i);
                                                    var run3 = run2 as CssBlockRun;

                                                    if (run3 != null)
                                                    {
                                                        //recursive here

                                                        foreach (var line2 in GetLineWalkIter(lineWalkVisitor, run3.ContentBox))
                                                        {
                                                            if (line2 == endline)
                                                            {
                                                                //found here!
                                                                //add line outter lnie 
                                                                if (i > 0)
                                                                {
                                                                    linebox.SelectPartialEnd((int)linebox.GetRun(i - 1).Right);
                                                                    selectedLines.Add(linebox);
                                                                }
                                                                line2.SelectPartialEnd(endHit.localX - startLineBeginSelectionAtPixel);
                                                                selectedLines.Add(line2);
                                                                //linebox.SelectPartialEnd((int)run2.Left + sel_offset);
                                                                isOK = true;
                                                                break;
                                                            }
                                                            else
                                                            {
                                                                line2.SelectFull();
                                                                selectedLines.Add(line2);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {   //check if hitpoint is in the line area
                                                linebox.SelectFull();
                                                selectedLines.Add(linebox);
                                            }

                                        }
                                    }
                                }
                                else
                                {

                                }
                            }
                            else
                            {
                                //between line
                                //select on different line 
                                //------- 
                                //1. select all in start line      
                                CssLineBox startLineBox = this.startHitHostLine;
                                 
                                bool isOK = false;
                                foreach (var visit in WalkDownAndUp(startLineBox))
                                {
                                    if (isOK)
                                    {
                                        break;
                                    }
                                    //only linebox?
                                    switch (visit.Kind)
                                    {
                                             
                                        case VisitMarkerKind.Line:
                                            {
                                                var line = visit.visitObject as CssLineBox;
                                                if (line == startLineBox)
                                                {
                                                    line.SelectPartialStart(this.startLineBeginSelectionAtPixel);
                                                    selectedLines.Add(line);
                                                }
                                                else if (line == endline)
                                                {
                                                    endline.SelectPartialEnd(endHit.localX);
                                                    selectedLines.Add(line);
                                                    isOK = true;
                                                }
                                                else
                                                {
                                                    //between
                                                    line.SelectFull();
                                                    selectedLines.Add(line);
                                                }
                                            } break;
                                    }
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
                        //find nearest line 
                        var nearestEndline = FindNearestLine(hitBox, endChain.RootGlobalY, 5);

                        //convert to global position
                        float globalHitY = endChain.RootGlobalY;
                        //check if should use first line of this box                         
                        //or last line box this box

                        foreach (var line in WalkDownAndUp(this.startHitHostLine, hitBox))
                        {
                            if (line == startHitHostLine)
                            {
                                line.SelectPartialStart(this.startLineBeginSelectionAtPixel);
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

        struct CommonGroundInfo
        {
            public int breakAtLevel;
            public bool isDeepDown;
        }

        //-------------------------------------------------------------------------------------------
        static CommonGroundInfo FindCommonGround(CssBoxHitChain startChain, CssBoxHitChain endChain)
        {

            //find common ground of startChain and endChain
            int startChainCount = startChain.Count;
            int endChainCount = endChain.Count;
            int lim = Math.Min(startChainCount, endChainCount);
            //from root to leave
            int breakAtLevel = 0;
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
            CommonGroundInfo commonGroundInfo = new CommonGroundInfo();
            commonGroundInfo.breakAtLevel = breakAtLevel;
            commonGroundInfo.isDeepDown = endChainCount > startChainCount && (breakAtLevel == startChainCount - 1);
            return commonGroundInfo;
            //----------------------------
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

        static IEnumerable<CssLineBox> WalkDownAndUp(CssLineBox startLine, CssBox endBox)
        {

            bool foundEndBox = false;
            foreach (var visit in WalkDownAndUp(startLine))
            {
                if (foundEndBox)
                {
                    break;
                }

                switch (visit.Kind)
                {
                    case VisitMarkerKind.Line:
                        {
                            yield return visit.visitObject as CssLineBox;
                        } break;
                    case VisitMarkerKind.Box:
                        {
                            var box = visit.visitObject as CssBox;
                            if (box == endBox)
                            {
                                foundEndBox = true;
                                foreach (var visit2 in GetWalkDownIter(endBox))
                                {
                                    if (visit2.Kind == VisitMarkerKind.Line)
                                    {
                                        yield return visit2.visitObject as CssLineBox;
                                    }
                                }
                            }
                        } break;
                }

            }
        }
        static IEnumerable<VisitMarker> GetWalkDownIter(CssBox box)
        {
            //recursive
            if (box.LineBoxCount > 0)
            {
                //line model 
                var linebox = box.GetFirstLineBox();
                while (linebox != null)
                {
                    yield return new VisitMarker(linebox);
                    linebox = linebox.NextLine;
                }
            }
            else
            {
                if (box.ChildCount > 0)
                {
                    foreach (CssBox child in box.GetChildBoxIter())
                    {
                        yield return new VisitMarker(child);

                        foreach (var child2 in GetWalkDownIter(child))
                        {
                            yield return child2;
                        }
                    }
                }
            }
        }
         
        /// <summary>
        /// walk down and up
        /// </summary>
        /// <param name="startLine"></param>
        /// <returns></returns>
        static IEnumerable<VisitMarker> WalkDownAndUp(CssLineBox startLine)
        {
            //start at line
            //1. start line***            
            yield return new VisitMarker(startLine);
            CssLineBox curLine = startLine;
            //walk up and down the tree
            CssLineBox nextline = curLine.NextLine;
            while (nextline != null)
            {
                yield return new VisitMarker(nextline);
                nextline = nextline.NextLine;
            }
            //--------------------
            //no next line 
            //then step up  
            CssBox curBox = startLine.OwnerBox;
        RETRY:
           
            CssBox level1Sibling = BoxHitUtils.GetNextSibling(curBox);
            while (level1Sibling != null)
            {
                foreach (var visit in GetWalkDownIter(level1Sibling))
                {
                    yield return visit;
                }

                level1Sibling = BoxHitUtils.GetNextSibling(level1Sibling);
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
        //--------------------------------------------------------------------------------------------------
        class LineWalkVisitor
        {
            public float globalX;
            public float globalY;

        }
        static IEnumerable<CssLineBox> GetLineWalkIter(LineWalkVisitor visitor, CssBox box)
        {
            //walk only line
            //recursive
            float y = visitor.globalY;

            foreach (var visit in GetWalkDownIter(box))
            {
                switch (visit.Kind)
                {
                    case VisitMarkerKind.Line:
                        {
                            var linebox = visit.visitObject as CssLineBox;
                            visitor.globalY = y + linebox.CachedLineTop;
                            yield return linebox;

                        } break;
                    case VisitMarkerKind.Box:
                        {
                            //recursive
                            var childBox = visit.visitObject as CssBox;
                            visitor.globalY = y + childBox.LocalY;
                            foreach (var line in GetLineWalkIter(visitor, visit.visitObject as CssBox))
                            {
                                yield return line;
                            }
                        } break; 
                }
            }
            visitor.globalY = y;
        }
    }

    static class CssLineBoxExtension
    {
        public static void SelectFull(this CssLineBox lineBox)
        {
            //full line selection 
            lineBox.LineSelectionSegment = new SelectionSegment(0, (int)lineBox.CachedLineContentWidth);
        }
        public static void SelectPartialStart(this CssLineBox lineBox, int startAt)
        {
            //from startAt to end of line

            lineBox.LineSelectionSegment = new SelectionSegment(startAt, (int)lineBox.CachedLineContentWidth - startAt);
        }
        public static void SelectPartialEnd(this CssLineBox lineBox, int endAt)
        {
            //from start of line to endAt

            lineBox.LineSelectionSegment = new SelectionSegment(0, endAt);
        }
    }

    enum VisitMarkerKind : byte
    {
        Unknown,
        Line,
        Box 
    }


    struct VisitMarker
    {
        internal readonly object visitObject;
        internal readonly VisitMarkerKind Kind;

        public VisitMarker(CssLineBox lineBox)
        {
            this.visitObject = lineBox;
            this.Kind = VisitMarkerKind.Line;
        }
        public VisitMarker(CssBox box)
        {
            this.visitObject = box;
            this.Kind = VisitMarkerKind.Box;
        }
        //public VisitMarker(CssBlockRun blockRun)
        //{
        //    this.visitObject = blockRun;
        //    this.Kind = VisitMarkerKind.NotifyInlineBlockBox;
        //}
    }
}