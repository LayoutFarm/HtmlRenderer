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
using LayoutFarm.Drawing;
using System.Text;
using HtmlRenderer.Diagnostics;

namespace HtmlRenderer.Boxes
{
    delegate bool EachCssTextRunHandler(CssTextRun trun);


    /// <summary>
    /// Utility class for traversing DOM structure and execution stuff on it.
    /// </summary>
    public sealed class BoxUtils
    {
        public static CssBox AddNewAnonInline(CssBox parent)
        {
            var spec = CssBox.UnsafeGetBoxSpec(parent);
            var newBox = new CssBox(null, spec.GetAnonVersion());

            parent.AppendChild(newBox);
            CssBox.ChangeDisplayType(newBox, Css.CssDisplay.Inline);
            return newBox;
        }
        public static bool HitTest(CssBox box, float x, float y, CssBoxHitChain hitChain)
        {

            //recursive  
            if (box.IsPointInArea(x, y))
            {
                float boxHitLocalX = x - box.LocalX;
                float boxHitLocalY = y - box.LocalY;

                //int boxHitGlobalX = (int)(boxHitLocalX + hitChain.GlobalOffsetX);
                //int boxHitGlobalY = (int)(boxHitLocalY + hitChain.GlobalOffsetY);

                hitChain.AddHit(box, (int)boxHitLocalX, (int)boxHitLocalY);
                hitChain.PushContextBox(box);

                if (box.LineBoxCount > 0)
                {

                    bool foundSomeLine = false;
                    foreach (var lineBox in box.GetLineBoxIter())
                    {
                        //line box not overlap
                        if (lineBox.HitTest(boxHitLocalX, boxHitLocalY))
                        {
                            foundSomeLine = true;
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
                        else if (foundSomeLine)
                        {
                            return false;
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

        public static bool HitTestWithPreviousChainHint(CssBox box, float x, float y, CssBoxHitChain hitChain, CssBoxHitChain previousChain)
        {
            if (previousChain != null)
            {
                int j = previousChain.Count;
                for (int i = 0; i < j; ++i)
                {
                    HitInfo hitInfo = previousChain.GetHitInfo(i);
                    switch (hitInfo.hitObjectKind)
                    {
                        case HitObjectKind.CssBox:
                            {


                            } break;
                        case HitObjectKind.LineBox:
                            {
                            } break;
                        case HitObjectKind.Run:
                            {
                            } break;
                        default:
                            {
                                throw new NotSupportedException();
                            }
                    }


                }
            }
            return true;//
        }

        internal static CssBox GetNextSibling(CssBox a)
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
        /// <summary>
        /// Gets the containing block-box of this box. (The nearest parent box with display=block)
        /// </summary>
        internal static CssBox SearchUpForContainingBlockBox(CssBox startBox)
        {

            if (startBox.ParentBox == null)
            {
                return startBox; //This is the initial containing block.
            }

            var box = startBox.ParentBox;
            while (box.CssDisplay < Css.CssDisplay.__CONTAINER_BEGIN_HERE &&
                box.ParentBox != null)
            {
                box = box.ParentBox;
            }

            //Comment this following line to treat always superior box as block
            if (box == null)
                throw new Exception("There's no containing block on the chain");
            return box;
        }
    }
}