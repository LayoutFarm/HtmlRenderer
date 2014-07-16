//BSD 2014 ,WinterDev

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
using System.Drawing;
using System.Text;
using HtmlRenderer.Drawing;
using HtmlRenderer.WebDom;
using HtmlRenderer.Diagnostics;

namespace HtmlRenderer.Boxes
{
    delegate bool EachCssTextRunHandler(CssTextRun trun);


    /// <summary>
    /// Utility class for traversing DOM structure and execution stuff on it.
    /// </summary>
    public sealed class BoxUtils
    {


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
                foreach (var visit in BoxUtils.GetDeepBoxOrLineIter(level1Sibling))
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
        public static CssBox GetNextSibling(CssBox a)
        {
            return a.GetNextNode();
        }
        internal static CssLineBox GetNearestLine(CssBox a, Point point, out bool found)
        {
            if (a.LineBoxCount > 0)
            {
                CssLineBox latestLine = null;

                int y = point.Y;
                found = false;
                foreach (CssLineBox linebox in a.GetLineBoxIter())
                {
                    if (linebox.CachedLineBottom < y)
                    {
                        latestLine = linebox;
                    }
                    else
                    {
                        if (latestLine != null)
                        {
                            found = true;
                        }
                        break;
                    }
                }

                return latestLine;
            }
            else
            {
                bool foundExact = false;
                CssLineBox lastLine = null;
                foreach (CssBox cbox in a.GetChildBoxIter())
                {

                    CssLineBox candidateLine = GetNearestLine(cbox, point, out foundExact);
                    if (candidateLine != null)
                    {
                        if (foundExact)
                        {
                            found = true;
                            return lastLine;
                        }
                        //not exact
                        lastLine = candidateLine;
                    }
                    else
                    {
                        if (lastLine != null)
                        {
                            found = false;
                            return lastLine;
                        }
                    }
                }
                found = foundExact;


            }
            return null;
        }

        



        public static bool HitTest(CssBox box, float x, float y, BoxHitChain hitChain)
        {
            //recursive  
            if (box.IsPointInArea(x, y))
            {

                float boxHitLocalX = x - box.LocalX;
                float boxHitLocalY = y - box.LocalY;

                int boxHitGlobalX = (int)(boxHitLocalX + hitChain.GlobalOffsetX);
                int boxHitGlobalY = (int)(boxHitLocalY + hitChain.GlobalOffsetY);

                hitChain.AddHit(box, (int)boxHitLocalX, (int)boxHitLocalY);

                hitChain.PushContextBox(box);

                if (box.LineBoxCount > 0)
                {

                    foreach (var lineBox in box.GetLineBoxIter())
                    {

                        if (lineBox.HitTest(boxHitLocalX, boxHitLocalY))
                        {

                            float lineBoxLocalY = boxHitLocalY - lineBox.CachedLineTop;

                            //2.
                            hitChain.AddHit(lineBox, (int)boxHitLocalX, (int)lineBoxLocalY);

                            var foundRun = BoxUtils.GetCssRunOnLocation(lineBox, (int)boxHitLocalX, (int)lineBoxLocalY);

                            if (foundRun != null)
                            {
                                //3.
                                hitChain.AddHit(foundRun, (int)(boxHitLocalX - foundRun.Left), (int)lineBoxLocalY);
                            }
                            //found line box
                            hitChain.PopContextBox(box);
                            return true;
                        }
                    }
                }
                else
                {
                    //iterate in child 
                    foreach (var childBox in box.GetChildBoxIter())
                    {
                        if (HitTest(childBox, boxHitLocalX, boxHitLocalY, hitChain))
                        {
                            //recursive
                            hitChain.PopContextBox(box);
                            return true;
                        }
                    }
                }
                hitChain.PopContextBox(box);
                return true;
            }
            else
            {
                //switch (box.WellknownTagName)
                switch (box.CssDisplay)
                {

                    case Css.CssDisplay.TableRow:
                        {

                            foreach (var childBox in box.GetChildBoxIter())
                            {
                                if (HitTest(childBox, x, y, hitChain))
                                {
                                    return true;
                                }
                            }
                        } break;
                    default:
                        {
                        } break;
                }

            }
            return false;
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
        internal static IEnumerable<CssLineBox> GetDeepDownLineBoxIter(CssBox box)
        {
            if (box.LineBoxCount > 0)
            {
                foreach (CssLineBox linebox in box.GetLineBoxIter())
                {
                    yield return linebox;
                }
            }
            else
            {
                if (box.ChildCount > 0)
                {
                    foreach (CssBox child in box.GetChildBoxIter())
                    {
                        foreach (CssLineBox linebox in GetDeepDownLineBoxIter(child))
                        {
                            yield return linebox;
                        }
                    }
                }
            }
        }
        internal static bool ForEachTextRunDeep(CssBox box, EachCssTextRunHandler handler)
        {

            if (box.LineBoxCount > 0)
            {
                foreach (CssLineBox line in box.GetLineBoxIter())
                {
                    //each line contains run
                    foreach (CssRun run in line.GetRunIter())
                    {
                        CssTextRun trun = run as CssTextRun;
                        if (trun != null)
                        {
                            if (handler(trun))
                            {
                                //found and exit
                                return true;
                            }
                        }
                    }
                }
            }
            else if (box.ChildCount > 0)
            {
                foreach (CssBox child in box.GetChildBoxIter())
                {
                    if (ForEachTextRunDeep(child, handler))
                    {
                        //found and exit
                        return true;
                    }
                }

            }
            else if (box.RunCount > 0)
            {
                foreach (CssRun run in box.GetRunIter())
                {
                    CssTextRun trun = run as CssTextRun;
                    if (trun != null)
                    {
                        if (handler(trun))
                        {
                            //found and exit
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        internal static CssRun GetCssRunOnLocation(CssLineBox lineBox, int x, int y)
        {
            foreach (CssRun word in lineBox.GetRunIter())
            {
                // add word spacing to word width so sentance won't have hols in it when moving the mouse
                var rect = word.Rectangle;
                //rect.Width += word.OwnerBox.ActualWordSpacing;
                if (rect.Contains(x, y))
                {
                    return word;
                }
            }
            return null;
        }


    }
}