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
using HtmlRenderer.Dom;
using HtmlRenderer.Entities;
namespace HtmlRenderer.Utils
{
    internal delegate bool EachCssTextRunHandler(CssTextRun trun);


    /// <summary>
    /// Utility class for traversing DOM structure and execution stuff on it.
    /// </summary>
    public sealed class DomUtils
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
            CssBox level1Sibling = DomUtils.GetNextSibling(curBox);
            while (level1Sibling != null)
            {
                foreach (var visit in Utils.DomUtils.GetDeepBoxOrLineIter(level1Sibling))
                {
                    yield return visit;
                }

                level1Sibling = DomUtils.GetNextSibling(level1Sibling);
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
            if (a.ParentBox == null)
            {
                return null;
            }
            CssBox parent = a.ParentBox;
            int childCount = parent.ChildCount;
            for (int i = 0; i < childCount; ++i)
            {
                if (parent.GetChildBox(i) == a)
                {
                    //found 
                    if (i < childCount - 1)
                    {
                        return parent.GetChildBox(i + 1);
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            return null;
        }
        public static CssBox GetPrevSibling(CssBox a)
        {
            CssBox parent = a.ParentBox;
            int childCount = parent.ChildCount;
            for (int i = 0; i < childCount; ++i)
            {
                if (parent.GetChildBox(i) == a)
                {
                    //found 
                    if (i == 0)
                    {
                        return null;
                    }
                    else
                    {
                        return parent.GetChildBox(i - 1);
                    }
                }
            }
            return null;
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


        /// <summary>
        /// Check if the given box contains only inline child boxes.
        /// </summary>
        /// <param name="box">the box to check</param>
        /// <returns>true - only inline child boxes, false - otherwise</returns>
        public static bool ContainsInlinesOnly(CssBox box)
        {

            foreach (CssBox b in box.GetChildBoxIter())
            {
                if (!b.IsInline)
                {
                    return false;
                }
            }
            return true;
        }


        /// <summary>
        /// Get css link box under the given sub-tree at the given x,y location.<br/>
        /// the location must be in correct scroll offset.
        /// </summary>
        /// <param name="box">the box to start search from</param>
        /// <param name="location">the location to find the box by</param>
        /// <returns>css link box if exists or null</returns>
        public static CssBox GetLinkBox(CssBox box, Point location)
        {

            //------------------------------------
            //2014-04-27: temp remove 
            //to be replace by another technique
            //------------------------------------

            //if (box != null)
            //{
            //    //if (box.IsClickable && box.Visibility == CssConstants.Visible)
            //    if (box.IsClickable && box.CssVisibility == CssVisibility.Visible)
            //    {
            //        foreach (var line in box.Rectangles)
            //        {
            //            if (line.Value.Contains(location))
            //            {
            //                return box;
            //            }
            //        }
            //    }

            //    if (box.ClientRectangle.IsEmpty || box.ClientRectangle.Contains(location))
            //    {
            //        foreach (var childBox in box.GetChildBoxIter())
            //        {
            //            var foundBox = GetLinkBox(childBox, location);
            //            if (foundBox != null)
            //                return foundBox;
            //        }
            //    }
            //}

            return null;
        }


        //wait for another technique
        ///// <summary>
        ///// Get css box under the given sub-tree with the given id.<br/>
        ///// </summary>
        ///// <param name="box">the box to start search from</param>
        ///// <param name="id">the id to find the box by</param>
        ///// <returns>css box if exists or null</returns>
        //public static CssBox GetBoxById(CssBox box, string id)
        //{ wait for another technique
        //    if (box != null && !string.IsNullOrEmpty(id))
        //    {
        //        if (box.HtmlElement != null && id.Equals(box.HtmlElement.TryGetAttribute("id"), StringComparison.OrdinalIgnoreCase))
        //        {
        //            return box;
        //        }

        //        foreach (var childBox in box.GetChildBoxIter())
        //        {
        //            var foundBox = GetBoxById(childBox, id);
        //            if (foundBox != null)
        //                return foundBox;
        //        }
        //    }

        //    return null;
        //}

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

                            var foundRun = DomUtils.GetCssRunOnLocation(lineBox, (int)boxHitLocalX, (int)lineBoxLocalY);

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
                switch (box.WellknownTagName)
                {
                    case WellknownHtmlTagName.tr:
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
        //public static CssRun GetWordOnLocation(CssBox box, Point loc)
        //{
        //    var lineBox = DomUtils.GetCssLineBox(box, loc);

        //    if (lineBox != null)
        //    {
        //        // get the word under the mouse
        //        return DomUtils.GetCssRunOnLocation(lineBox, loc);
        //        //// if no word found under the mouse use the last or the first word in the line
        //        //if (word == null && lineBox.WordCount > 0)
        //        //{
        //        //    if (loc.Y > lineBox.ReCalculateLineBottom())
        //        //    {
        //        //        // under the line
        //        //        //word = lineBox.Words[lineBox.Words.Count - 1];
        //        //        word = lineBox.GetLastRun();
        //        //    }
        //        //    else if (loc.X < lineBox.GetFirstRun().Left)
        //        //    {
        //        //        // before the line
        //        //        word = lineBox.GetFirstRun();
        //        //    }
        //        //    else if (loc.X > lineBox.GetLastRun().Right)
        //        //    {
        //        //        // at the end of the line
        //        //        word = lineBox.GetLastRun();
        //        //    }
        //        //}

        //        //return word;
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}
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
        ///// <summary>
        ///// Get css line box under the given sub-tree at the given y location or the nearest line from the top.<br/>
        ///// the location must be in correct scroll offset.
        ///// </summary>
        ///// <param name="box">the box to start search from</param>
        ///// <param name="location">the location to find the box at</param>
        ///// <returns>css word box if exists or null</returns>
        //internal static CssLineBox GetCssLineBox(CssBox box, Point location)
        //{
        //    CssLineBox line = null;
        //    if (box != null)
        //    {
        //        if (box.LineBoxCount > 0)
        //        {
        //            if (box.WellknownTagName == WellknownHtmlTagName.NotAssign ||
        //                box.WellknownTagName != WellknownHtmlTagName.td ||
        //                box.IsPointInArea(location.X, location.Y))
        //            {
        //                foreach (var lineBox in box.GetLineBoxIter())
        //                {
        //                    foreach (var rect_top in lineBox.GetAreaStripTopPosIter())
        //                    {
        //                        if (rect_top <= location.Y)
        //                        {
        //                            line = lineBox;
        //                        }
        //                        else
        //                        {
        //                            return line;
        //                        }
        //                    }
        //                }
        //            }
        //        }

        //        foreach (var childBox in box.GetChildBoxIter())
        //        {
        //            line = GetCssLineBox(childBox, location) ?? line;
        //        }
        //    }

        //    return line;
        //}

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
        /// <summary>
        /// Get css word box under the given sub-tree at the given x,y location.<br/>
        /// the location must be in correct scroll offset.
        /// </summary>
        /// <param name="box">the box to start search from</param>
        /// <param name="location">the location to find the box at</param>
        /// <returns>css word box if exists or null</returns>
        public static CssRun GetCssBoxWord(CssBox box, Point location)
        {
            if (box == null || box.Visibility != CssVisibility.Visible)
            {
                return null;
            }
            //--------------------------------------------------------------- 
            if (box.LineBoxCount > 0)
            {
                int x = location.X;
                int y = location.Y;
                if (x >= box.LocalX && x <= box.LocalRight)
                {
                    foreach (CssLineBox lineBox in box.GetLineBoxIter())
                    {
                        if (lineBox.HitTest(x, y))
                        {
                            //hit this line
                            //then find css run at location
                            var wordBox = GetCssRunOnLocation(lineBox, location);
                            if (wordBox != null)
                            {
                                return wordBox;
                            }

                        }
                    }
                }
            }
            else
            {
                //--------------------------------------------------------------- 
                if (box.IsPointInClientArea(location.X, location.Y))
                {
                    foreach (var childBox in box.GetChildBoxIter())
                    {
                        var foundWord = GetCssBoxWord(childBox, location);
                        if (foundWord != null)
                        {
                            return foundWord;
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Get css word box under the given sub-tree at the given x,y location.<br/>
        /// the location must be in correct scroll offset.
        /// </summary>
        /// <param name="lineBox">the line box to search in</param>
        /// <param name="location">the location to find the box at</param>
        /// <returns>css word box if exists or null</returns>
        internal static CssRun GetCssRunOnLocation(CssLineBox lineBox, Point location)
        {
            foreach (CssRun word in lineBox.GetRunIter())
            {
                // add word spacing to word width so sentance won't have hols in it when moving the mouse
                var rect = word.Rectangle;
                //rect.Width += word.OwnerBox.ActualWordSpacing;
                if (rect.Contains(location))
                {
                    return word;
                }
            }
            return null;
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
        //public static bool IsOnTheSameLine(CssRun start, CssRun end)
        //{
        //    return DomUtils.GetCssLineBoxByWord(start) == DomUtils.GetCssLineBoxByWord(end);
        //}
        //public static float GetLineBottom(CssRun cssRect)
        //{
        //    return GetCssLineBoxByWord(cssRect).ReCalculateLineBottom();
        //}
        ///// <summary>
        ///// Find the css line box that the given word is in.
        ///// </summary>
        ///// <param name="word">the word to search for it's line box</param>
        ///// <returns>line box that the word is in</returns>
        //internal static CssLineBox GetCssLineBoxByWord(CssRun word)
        //{
        //    var box = word.OwnerBox;
        //    while (box.LineBoxCount == 0)
        //    {
        //        box = box.ParentBox;
        //    }
        //    foreach (var lineBox in box.GetLineBoxIter())
        //    {
        //        foreach (var lineWord in lineBox.GetRunIter())
        //        {
        //            if (lineWord == word)
        //            {
        //                return lineBox;
        //            }
        //        }
        //    }
        //    //return box.LineBoxes[0];
        //    return box.GetFirstLineBox();
        //}

        /// <summary>
        /// Get selected plain text of the given html sub-tree.
        /// </summary>
        /// <param name="root">the DOM box to get selected text from its sub-tree</param>
        /// <returns>the selected plain text string</returns>
        public static string GetSelectedPlainText(CssBox root)
        {
            return "";

            //var sb = new StringBuilder();
            //var lastWordIndex = GetSelectedPlainText(sb, root);
            //return sb.ToString(0, lastWordIndex).Trim();
        }

        /// <summary>
        /// Generate html from the given dom tree.<br/>
        /// Generate all the tyle inside the html.
        /// </summary>
        /// <param name="root">the box of the html generate html from</param>
        /// <param name="styleGen">Optional: controls the way styles are generated when html is generated</param>
        /// <param name="onlySelected">Optional: true - generate only selected html subset, false - generate all (default - false)</param>
        /// <returns>generated html</returns>
        public static string GenerateHtml(CssBox root, HtmlGenerationStyle styleGen = HtmlGenerationStyle.Inline, bool onlySelected = false)
        {
            var sb = new StringBuilder();

            if (root != null)
            {
                WriteHtml(sb, root, 0, styleGen, onlySelected ? CollectSelectedHtmlTags(root) : null);
            }
            return sb.ToString();
        }

        ///// <summary>
        ///// Generate textual tree representation of the css boxes tree starting from the given root.<br/>
        ///// Used for debugging html parsing.
        ///// </summary>
        ///// <param name="root">the root to generate tree from</param>
        ///// <returns>generated tree</returns>
        //public static string GenerateBoxTree(CssBox root)
        //{
        //    var sb = new StringBuilder();
        //    GenerateBoxTree(root, sb, 0);
        //    return sb.ToString();
        //}


        #region Private methods

        ///// <summary>
        ///// Get selected plain text of the given html sub-tree.<br/>
        ///// Append all the selected words.
        ///// </summary>
        ///// <param name="sb">the builder to append the selected text to</param>
        ///// <param name="box">the DOM box to get selected text from its sub-tree</param>
        ///// <returns>the index of the last word appended</returns>
        //private static int GetSelectedPlainText(StringBuilder sb, CssBox box)
        //{


        //    int lastWordIndex = 0;
        //    foreach (var boxWord in box.Runs)
        //    {
        //        // append the text of selected word (handle partial selected words)
        //        if (boxWord.Selected)
        //        {
        //            sb.Append(GetSelectedWord(boxWord, true));
        //            lastWordIndex = sb.Length;
        //        }
        //    }

        //    // empty span box
        //    if (box.ChildCount < 1 && box.MayHasSomeTextContent && box.TextContentIsAllWhitespace)
        //    {
        //        sb.Append(' ');
        //    }

        //    // deep traversal
        //    if (box.CssVisibility != CssVisibility.Hidden && //box.Visibility != CssConstants.Hidden &&
        //        box.CssDisplay != CssDisplay.None)// box.Display != CssConstants.None)
        //    {
        //        foreach (var childBox in box.GetChildBoxIter())
        //        {
        //            var innerLastWordIdx = GetSelectedPlainText(sb, childBox);
        //            lastWordIndex = Math.Max(lastWordIndex, innerLastWordIdx);
        //        }
        //    }

        //    if (sb.Length > 0)
        //    {
        //        // convert hr to line of dashes
        //        //if (box.HtmlTag != null && box.HtmlTag.Name == "hr")
        //        if (box.WellknownTagName == WellknownHtmlTagName.HR)
        //        {
        //            if (sb.Length > 1 && sb[sb.Length - 1] != '\n')
        //                sb.AppendLine();
        //            sb.AppendLine(new string('-', 80));
        //        }

        //        // new line for css block
        //        //if (box.Display == CssConstants.Block || box.Display == CssConstants.ListItem || box.Display == CssConstants.TableRow)
        //        if (box.CssDisplay == CssDisplay.Block ||
        //            box.CssDisplay == CssDisplay.ListItem ||
        //            box.CssDisplay == CssDisplay.TableRow)
        //        {
        //            if (!(box.IsBrElement && sb.Length > 1 && sb[sb.Length - 1] == '\n'))
        //                sb.AppendLine();
        //        }

        //        // space between table cells
        //        //if (box.Display == CssConstants.TableCell)
        //        if (box.CssDisplay == CssDisplay.TableCell)
        //        {
        //            sb.Append(' ');
        //        }

        //        // paragraphs has additional newline for nice formatting
        //        //if (box.HtmlTag != null && box.HtmlTag.Name == "p")
        //        if (box.WellknownTagName == WellknownHtmlTagName.P)
        //        {
        //            int newlines = 0;
        //            for (int i = sb.Length - 1; i >= 0 && char.IsWhiteSpace(sb[i]); i--)
        //                newlines += sb[i] == '\n' ? 1 : 0;
        //            if (newlines < 2)
        //                sb.AppendLine();
        //        }
        //    }

        //    return lastWordIndex;
        //}

        /// <summary>
        /// Collect the html tags that have at least one word down the heirarchy that is selected recursevly.<br/>
        /// </summary>
        /// <param name="root">the box to check its sub-tree</param>
        /// <returns>the collection to add the selected tags to</returns>
        private static Dictionary<IHtmlElement, bool> CollectSelectedHtmlTags(CssBox root)
        {
            var selectedTags = new Dictionary<IHtmlElement, bool>();
            var maybeTags = new Dictionary<IHtmlElement, bool>();
            CollectSelectedHtmlTags(root, selectedTags, maybeTags);
            return selectedTags;
        }

        /// <summary>
        /// Collect the html tags that have at least one word down the heirarchy that is selected recursevly.<br/>
        /// </summary>
        /// <param name="box">the box to check its sub-tree</param>
        /// <param name="selectedTags">the collection to add the selected tags to</param>
        /// <param name="maybeTags">used to handle tags that are between selected words but don't have selected word inside</param>
        /// <returns>is the current box is in selected sub-tree</returns>
        private static bool CollectSelectedHtmlTags(CssBox box, Dictionary<IHtmlElement, bool> selectedTags, Dictionary<IHtmlElement, bool> maybeTags)
        {
            bool isInSelection = false;
            //foreach (var word in box.Runs)
            //{   
            //    if (word.Selected)
            //    {
            //        if (box.HtmlTag != null)
            //        {
            //            selectedTags.Add(box.HtmlTag, true);
            //        }
            //        foreach (var maybeTag in maybeTags)
            //            selectedTags[maybeTag.Key] = maybeTag.Value;
            //        maybeTags.Clear();
            //        isInSelection = true;
            //    }
            //}

            //foreach (var childBox in box.GetChildBoxIter())
            //{
            //    var childInSelection = CollectSelectedHtmlTags(childBox, selectedTags, maybeTags);
            //    if (childInSelection)
            //    {
            //        if (box.HtmlTag != null)
            //        {
            //            selectedTags[box.HtmlTag] = true;
            //        }
            //        isInSelection = true;
            //    }
            //}

            //if (box.HtmlTag != null && selectedTags.Count > 0)
            //{
            //    maybeTags[box.HtmlTag] = true;
            //}

            return isInSelection;
        }

        /// <summary>
        /// Write the given html dom subtree into the given string builder.
        /// </summary>
        /// <param name="sb">the string builder to write html into</param>
        /// <param name="box">the html sub-tree to write</param>
        /// <param name="indent">the indent to use for nice formating</param>
        /// <param name="styleGen">Controls the way styles are generated when html is generated</param>
        /// <param name="selectedTags">Control if to generate only selected tags, if given only tags found in collection will be generated</param>
        private static void WriteHtml(StringBuilder sb, CssBox box, int indent, HtmlGenerationStyle styleGen, Dictionary<IHtmlElement, bool> selectedTags)
        {

            //2014 - 05 - 24;
            //tempoary remove
            //wait for another method , 


            return;
            //if (box.HtmlTag != null && selectedTags != null && !selectedTags.ContainsKey(box.HtmlTag))
            //    return;

            //if (box.HtmlTag != null)
            //{
            //    string hrefAttrValue = null;
            //    if (box.WellknownTagName != WellknownHtmlTagName.LINK ||
            //        ((hrefAttrValue = box.HtmlTag.TryGetAttribute("href")) == null) ||
            //        (!hrefAttrValue.StartsWith("property") && !hrefAttrValue.StartsWith("method"))
            //        )
            //    {
            //        WriteHtmlTag(sb, box, indent, styleGen);
            //        indent = indent + (box.HtmlTag.IsSingle ? 0 : 1);
            //    }

            //    //if (box.WellknownTagName != WellknownHtmlTagName.LINK ||
            //    //    !box.HtmlTag.Attributes.ContainsKey("href") ||
            //    //    (!box.HtmlTag.Attributes["href"].StartsWith("property") && !box.HtmlTag.Attributes["href"].StartsWith("method")))
            //    //{
            //    //    WriteHtmlTag(sb, box, indent, styleGen);
            //    //    indent = indent + (box.HtmlTag.IsSingle ? 0 : 1);
            //    //}


            //    if (styleGen == HtmlGenerationStyle.InHeader &&
            //        box.WellknownTagName == WellknownHtmlTagName.HTML && box.HtmlContainer.CssData != null)
            //    {
            //        sb.Append(new string(' ', indent * 4));
            //        sb.AppendLine("<head>");
            //        WriteStylesheet(sb, box.HtmlContainer.CssData, indent + 1);
            //        sb.Append(new string(' ', indent * 4));
            //        sb.AppendLine("</head>");
            //    }
            //}

            //if (box.Words.Count > 0)
            //{
            //    sb.Append(new string(' ', indent * 4));
            //    foreach (var word in box.Words)
            //    {
            //        if (selectedTags == null || word.Selected)
            //        {
            //            var wordText = GetSelectedWord(word, selectedTags != null);
            //            sb.Append(HtmlUtils.EncodeHtml(wordText));
            //        }
            //    }
            //    sb.AppendLine();
            //}

            //foreach (var childBox in box.GetChildBoxIter())
            //{
            //    WriteHtml(sb, childBox, indent, styleGen, selectedTags);
            //}

            //if (box.HtmlTag != null && !box.HtmlTag.IsSingle)
            //{
            //    sb.Append(new string(' ', Math.Max((indent - 1) * 4, 0)));
            //    sb.AppendFormat("</{0}>", box.HtmlTag.Name);
            //    sb.AppendLine();
            //}
        }

        /// <summary>
        /// Write the given html tag with all its attributes and styles.
        /// </summary>
        /// <param name="sb">the string builder to write html into</param>
        /// <param name="box">the css box with the html tag to write</param>
        /// <param name="indent">the indent to use for nice formating</param>
        /// <param name="styleGen">Controls the way styles are generated when html is generated</param>
        private static void WriteHtmlTag(StringBuilder sb, CssBox box, int indent, HtmlGenerationStyle styleGen)
        {
            //2014-05-24 temporary remove,
            //
            //sb.Append(new string(' ', indent * 4));
            //sb.AppendFormat("<{0}", box.HtmlTag.Name);

            //// collect all element style properties incliding from stylesheet
            //var tagStyles = new Dictionary<string, string>();
            //var tagCssBlock = box.HtmlContainer.CssData.GetCssRuleSetIter(box.HtmlTag.Name);
            //if (tagCssBlock != null)
            //{
            //    // TODO: handle selectors
            //    foreach (var cssBlock in tagCssBlock)
            //        foreach (var prop in cssBlock.Properties)
            //            tagStyles[prop.Key] = prop.Value.Value;
            //}

            //if (box.HtmlTag.HasAttributes())
            //{
            //    sb.Append(" ");

            //    bool isImageBox = box.HtmlTag.WellknownTagName == WellknownHtmlTagName.IMG;

            //    foreach (var att in box.HtmlTag.GetAttributeIter())
            //    {

            //        // handle image tags by inserting the image using base64 data
            //        //if (box.HtmlTag.Name == "img" && att.Key == "src" && (att.Value.StartsWith("property") || att.Value.StartsWith("method")))
            //        if (isImageBox && att.Name == "src" && (att.Value.StartsWith("property") || att.Value.StartsWith("method")))
            //        {
            //            var img = ((CssBoxImage)box).Image;
            //            if (img != null)
            //            {
            //                using (var buffer = new MemoryStream())
            //                {
            //                    img.Save(buffer, ImageFormat.Png);
            //                    var base64 = Convert.ToBase64String(buffer.ToArray());
            //                    sb.AppendFormat("{0}=\"data:image/png;base64, {1}\" ", att.Name, base64);
            //                }
            //            }
            //        }
            //        else if (styleGen == HtmlGenerationStyle.Inline && att.Name == HtmlConstants.Style)
            //        {
            //            throw new NotSupportedException();
            //            //// if inline style add the styles to the collection
            //            //var block = CssParser.ParseCssBlock(box.HtmlTag.Name, box.HtmlTag.TryGetAttribute("style"));
            //            //foreach (var prop in block.Properties)
            //            //    tagStyles[prop.Key] = prop.Value.Value;
            //        }
            //        else if (styleGen == HtmlGenerationStyle.Inline && att.Name == HtmlConstants.Class)
            //        {
            //            // if inline style convert the style class to actual properties and add to collection
            //            var cssBlocks = box.HtmlContainer.CssData.GetCssRuleSetIter("." + att.Value);
            //            if (cssBlocks != null)
            //            {
            //                // TODO: handle selectors
            //                foreach (var cssBlock in cssBlocks)
            //                    foreach (var prop in cssBlock.Properties)
            //                        tagStyles[prop.Key] = prop.Value.Value;
            //            }
            //        }
            //        else
            //        {
            //            sb.AppendFormat("{0}=\"{1}\" ", att.Name, att.Value);
            //        }
            //    }

            //    sb.Remove(sb.Length - 1, 1);
            //}

            //// if inline style insert the style tag with all collected style properties
            //if (styleGen == HtmlGenerationStyle.Inline && tagStyles.Count > 0)
            //{
            //    sb.Append(" style=\"");
            //    foreach (var style in tagStyles)
            //    {
            //        sb.AppendFormat("{0}: {1}; ", style.Key, style.Value);
            //    }
            //    sb.Remove(sb.Length - 1, 1);
            //    sb.Append("\"");
            //}

            //sb.AppendFormat("{0}>", box.HtmlTag.IsSingle ? "/" : "");
            //sb.AppendLine();
        }

        /// <summary>
        /// Write stylesheet data inline into the html.
        /// </summary>
        /// <param name="sb">the string builder to write stylesheet into</param>
        /// <param name="cssData">the css data to write to the head</param>
        /// <param name="indent">the indent to use for nice formating</param>
        private static void WriteStylesheet(StringBuilder sb, CssActiveSheet cssData, int indent)
        {
            //wait for another technique
            throw new NotSupportedException("wait for another technique");

            //sb.Append(new string(' ', indent * 4));
            //sb.AppendLine("<style type=\"text/css\">");
            //foreach (CssCodeBlock block in cssData.DefaultMediaBlock.GetCodeBlockIter())
            //{

            //    sb.Append(new string(' ', (indent + 1) * 4));
            //    sb.Append(block.CssClassName);
            //    sb.Append(" { ");
            //    foreach (var cssProperty in block.Properties.Values)
            //    {
            //        sb.AppendFormat("{0}: {1};", cssProperty.Name, cssProperty.Value);
            //        //sb.AppendFormat("{0}: {1};",, property.Value);
            //        //foreach (var property in cssBlock.Properties)
            //        //{
            //        //    // TODO: handle selectors
            //        //    sb.AppendFormat("{0}: {1};", property.Key, property.Value);
            //        //}
            //    }
            //    sb.Append(" }");
            //    sb.AppendLine();
            //}
            //sb.Append(new string(' ', indent * 4));
            //sb.AppendLine("</style>");
        }

        /// <summary>
        /// Get the selected word with respect to partial selected words.
        /// </summary>
        /// <param name="rect">the word to append</param>
        /// <param name="selectedText">is to get selected text or all the text in the word</param>
        private static string GetSelectedWord(CssRun rect, bool selectedText)
        {
            return "";

            //if (selectedText && rect.SelectedStartIndex > -1 && rect.SelectedEndIndexOffset > -1)
            //{
            //    return rect.Text.Substring(rect.SelectedStartIndex, rect.SelectedEndIndexOffset - rect.SelectedStartIndex);
            //}
            //else if (selectedText && rect.SelectedStartIndex > -1)
            //{
            //    return rect.Text.Substring(rect.SelectedStartIndex) + (rect.HasSpaceAfter ? " " : "");
            //}
            //else if (selectedText && rect.SelectedEndIndexOffset > -1)
            //{
            //    return rect.Text.Substring(0, rect.SelectedEndIndexOffset);
            //}
            //else
            //{
            //    var whitespaceBefore = rect.OwnerBox.Words[0] == rect ? IsBoxHasWhitespace(rect.OwnerBox) : rect.HasSpaceBefore;
            //    return (whitespaceBefore ? " " : "") + rect.Text + (rect.HasSpaceAfter ? " " : "");
            //}
        }

        ///// <summary>
        ///// Generate textual tree representation of the css boxes tree starting from the given root.<br/>
        ///// Used for debugging html parsing.
        ///// </summary>
        ///// <param name="box">the box to generate for</param>
        ///// <param name="builder">the string builder to generate to</param>
        ///// <param name="indent">the current indent level to set indent of generated text</param>
        //private static void GenerateBoxTree(CssBox box, StringBuilder builder, int indent)
        //{
        //    builder.AppendFormat("{0}<{1}", new string(' ', 2 * indent), box.CssDisplay.ToCssStringValue());
        //    if (box.HtmlElement != null)
        //        builder.AppendFormat(" elm=\"{0}\"", box.HtmlElement != null ? box.HtmlElement.Name : string.Empty);
        //    if (box.RunCount > 0)
        //        builder.AppendFormat(" words=\"{0}\"", box.RunCount);
        //    builder.AppendFormat("{0}>\r\n", box.ChildCount > 0 ? "" : "/");
        //    if (box.ChildCount > 0)
        //    {
        //        foreach (var childBox in box.GetChildBoxIter())
        //        {
        //            GenerateBoxTree(childBox, builder, indent + 1);
        //        }
        //        builder.AppendFormat("{0}</{1}>\r\n", new string(' ', 2 * indent), box.CssDisplay.ToCssStringValue());
        //    }
        //}

        #endregion
    }
}