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
        /// <summary>
        /// Gets the previous sibling of this box.
        /// </summary>
        /// <returns>Box before this one on the tree. Null if its the first</returns>
        public static CssBox GetPreviousContainingBlockSibling(CssBox b)
        {
            var conBlock = b;
            int index = conBlock.ParentBox.Boxes.IndexOf(conBlock);
            //while (conBlock.ParentBox != null &&
            //   index < 1 &&
            //   conBlock.Display != CssConstants.Block
            //   && conBlock.Display != CssConstants.Table
            //   && conBlock.Display != CssConstants.TableCell
            //   && conBlock.Display != CssConstants.ListItem)

            while (conBlock.ParentBox != null &&
                index < 1 &&
                conBlock.CssDisplay != CssDisplay.Block &&
                conBlock.CssDisplay != CssDisplay.Table &&
                conBlock.CssDisplay != CssDisplay.TableCell &&
                conBlock.CssDisplay != CssDisplay.ListItem)
            {
                conBlock = conBlock.ParentBox;
                index = conBlock.ParentBox != null ? conBlock.ParentBox.Boxes.IndexOf(conBlock) : -1;
            }

            conBlock = conBlock.ParentBox;
            if (conBlock != null && index > 0)
            {
                int diff = 1;
                CssBox sib = conBlock.Boxes[index - diff];

                //while ((sib.Display == CssConstants.None || 
                //    sib.Position == CssConstants.Absolute) && index - diff - 1 >= 0)
                while ((sib.CssDisplay == CssDisplay.None ||
                   sib.IsAbsolutePosition) && index - diff - 1 >= 0)
                {
                    sib = conBlock.Boxes[index - ++diff];
                }

                return sib.CssDisplay == CssDisplay.None ? null : sib;
            }
            return null;
        }
        /// <summary>
        /// Gets the previous sibling of this box.
        /// </summary>
        /// <returns>Box before this one on the tree. Null if its the first</returns>
        public static CssBox GetPreviousSibling(CssBox b)
        {
            if (b.ParentBox != null)
            {
                int index = b.ParentBox.Boxes.IndexOf(b);
                if (index > 0)
                {
                    int diff = 1;
                    CssBox sib = b.ParentBox.Boxes[index - diff];

                    // while ((sib.Display == CssConstants.None || sib.Position == CssConstants.Absolute) && index - diff - 1 >= 0)
                    while ((sib.CssDisplay == CssDisplay.None || sib.IsAbsolutePosition) && index - diff - 1 >= 0)
                    {
                        sib = b.ParentBox.Boxes[index - ++diff];
                    }
                    return sib.CssDisplay == CssDisplay.None ? null : sib;
                    //return sib.Display == CssConstants.None ? null : sib;
                }
            }
            return null;
        }
        /// <summary>
        /// Correct the DOM tree recursively by replacing  "br" html boxes with anonymous blocks that respect br spec.<br/>
        /// If the "br" tag is after inline box then the anon block will have zero height only acting as newline,
        /// but if it is after block box then it will have min-height of the font size so it will create empty line.
        /// </summary>
        /// <param name="box">the current box to correct its sub-tree</param>
        /// <param name="followingBlock">used to know if the br is following a box so it should create an empty line or not so it only
        /// move to a new line</param>
        public static void CorrectLineBreaksBlocks(CssBox box, ref bool followingBlock)
        {
            //recursive

            followingBlock = followingBlock || box.IsBlock;
            var allChildren = box.Boxes;

            foreach (var childBox in allChildren)
            {
                //recursive to child first
                CorrectLineBreaksBlocks(childBox, ref followingBlock);
                followingBlock = childBox.Words.Count == 0 && (followingBlock || childBox.IsBlock);
            }
             
            //-------------------------------------
            int latestCheckIndex = -1;
            CssBox brBox = null;
            do
            {
                brBox = null;//reset each loop
                int j = allChildren.Count;
                for (int i = latestCheckIndex + 1; i < j; i++)
                {
                    var curBox = allChildren[i];
                    if (curBox.IsBrElement)
                    {
                        brBox = curBox;
                        latestCheckIndex = i;
                        //check prev box
                        if (i > 0)
                        {
                            var prevBox = allChildren[i - 1];
                            if (prevBox.Words.Count > 0)
                            {
                                followingBlock = false;
                            }
                            else if (prevBox.IsBlock)
                            {
                                followingBlock = true;
                            }
                        }
                        break;
                    }
                }

                if (brBox != null)
                {
                    //create new box then add to 'box'  before brbox  
                    var anonBlock = CssBox.CreateBlock(box, new HtmlTag("br"), brBox);
                    if (followingBlock)
                    {
                        //anonBlock.Height = ".95em"; // atodo: check the height to min-height when it is supported
                        anonBlock.Height = new CssLength(0.95f, false, CssUnit.Ems);
                    }

                    //remove this br box from parent ***
                    brBox.ParentBox = null;
                }

            } while (brBox != null);
        }

        /// <summary>
        /// Check if the given box contains inline and block child boxes.
        /// </summary>
        /// <param name="box">the box to check</param>
        /// <returns>true - has variant child boxes, false - otherwise</returns>
        internal static bool ContainsVariantBoxes(CssBox box, out int mixFlags)
        {
            //bool hasBlock = false;
            //bool hasInline = false;
            //for (int i = 0; i < box.Boxes.Count && (!hasBlock || !hasInline); i++)
            //{
            //    var isBlock = !box.Boxes[i].IsInline;
            //    hasBlock = hasBlock || isBlock;
            //    hasInline = hasInline || !isBlock;
            //}
            mixFlags = 0;

            var children = box.Boxes;
            for (int i = children.Count - 1; i >= 0; --i)
            {
                if ((mixFlags |= children[i].IsInline ? HAS_IN_LINE : HAS_BLOCK)
                    == (HAS_BLOCK | HAS_IN_LINE))
                {
                    return true;
                }
            }

            return false;
            //return checkFlags == (HAS_BLOCK | HAS_IN_LINE);
        }
        const int HAS_BLOCK = 1 << (1 - 1);
        const int HAS_IN_LINE = 1 << (2 - 1);
        /// <summary>
        /// Rearrange the DOM of the box to have block box with boxes before the inner block box and after.
        /// </summary>
        /// <param name="box">the box that has the problem</param>
        internal static void CorrectBlockInsideInlineImp(CssBox box)
        {
            if (box.ChildCount > 1 || box.GetFirstChild().ChildCount > 1)
            {
                var leftBlock = CssBox.CreateBlock(box);
                while (ContainsInlinesOnlyDeep(box.GetFirstChild()))
                {
                    //if first box has only inline(deep) then
                    //move first child to leftBlock ***
                    box.GetFirstChild().ParentBox = leftBlock;
                }
                //insert left block as leftmost (firstbox) in the line 
                leftBlock.SetBeforeBox(box.GetFirstChild());

                var splitBox = box.Boxes[1];
                splitBox.ParentBox = null;

                CorrectBlockSplitBadBox(box, splitBox, leftBlock);

                if (box.ChildCount > 2)
                {
                    var rightBox = CssBox.CreateBox(box, null, box.Boxes[2]);
                    while (box.ChildCount > 3)
                        box.Boxes[3].ParentBox = rightBox;
                }
            }
            //else if (box.Boxes[0].Display == CssConstants.Inline)
            else if (box.Boxes[0].CssDisplay == CssDisplay.Inline)
            {
                box.Boxes[0].CssDisplay = CssDisplay.Block;
                //box.Boxes[0].Display = CssConstants.Block;
            }
            //if (box.Display == CssConstants.Inline)
            if (box.CssDisplay == CssDisplay.Inline)
            {
                box.CssDisplay = CssDisplay.Block;
                //box.Display = CssConstants.Block;
            }
        }

        /// <summary>
        /// Split bad box that has inline and block boxes into two parts, the left - before the block box
        /// and right - after the block box.
        /// </summary>
        /// <param name="parentBox">the parent box that has the problem</param>
        /// <param name="badBox">the box to split into different boxes</param>
        /// <param name="leftBlock">the left block box that is created for the split</param>
        private static void CorrectBlockSplitBadBox(CssBox parentBox, CssBox badBox, CssBox leftBlock)
        {//recursive

            var leftBox = CssBox.CreateBox(leftBlock, badBox.HtmlTag);
            leftBox.InheritStyle(badBox, true);

            bool had_new_leftbox = false;
            while (badBox.GetFirstChild().IsInline && ContainsInlinesOnlyDeep(badBox.GetFirstChild()))
            {
                had_new_leftbox = true;
                //move element that has only inline(deep) to leftbox
                badBox.GetFirstChild().ParentBox = leftBox;
            }

            var splitBox = badBox.GetFirstChild();

            if (!ContainsInlinesOnlyDeep(splitBox))
            {
                //recursive
                CorrectBlockSplitBadBox(parentBox, splitBox, leftBlock);
                splitBox.ParentBox = null;
            }
            else
            {
                splitBox.ParentBox = parentBox;
            }

            if (badBox.ChildCount > 0)
            {
                CssBox rightBox;
                if (splitBox.ParentBox != null || parentBox.ChildCount < 3)
                {
                    rightBox = CssBox.CreateBox(parentBox, badBox.HtmlTag);
                    rightBox.InheritStyle(badBox, true);

                    if (parentBox.ChildCount > 2)
                        rightBox.SetBeforeBox(parentBox.Boxes[1]);

                    if (splitBox.ParentBox != null)
                        splitBox.SetBeforeBox(rightBox);
                }
                else
                {
                    rightBox = parentBox.Boxes[2];
                }

                while (badBox.ChildCount > 0)
                {   //move all children to right box 
                    badBox.Boxes[0].ParentBox = rightBox;
                }
            }
            else if (splitBox.ParentBox != null && parentBox.ChildCount > 1)
            {
                splitBox.SetBeforeBox(parentBox.Boxes[1]);
                //if (splitBox.HtmlTag != null && splitBox.HtmlTag.Name == "br" && (hadLeft || leftBlock.Boxes.Count > 1))
                if (splitBox.WellknownTagName == WellknownHtmlTagName.BR
                    && (had_new_leftbox || leftBlock.ChildCount > 1))
                {
                    splitBox.CssDisplay = CssDisplay.Inline;
                    //splitBox.Display = CssConstants.Inline;
                }
            }
        }

        /// <summary>
        /// Makes block boxes be among only block boxes and all inline boxes have block parent box.<br/>
        /// Inline boxes should live in a pool of Inline boxes only so they will define a single block.<br/>
        /// At the end of this process a block box will have only block siblings and inline box will have
        /// only inline siblings.
        /// </summary>
        /// <param name="box">the current box to correct its sub-tree</param>
        internal static void CorrectInlineBoxesParent(CssBox box)
        {
            //------------------------------------------------
            //recursive 
            int mixFlags;
            var allChildren = box.Boxes;
            if (ContainsVariantBoxes(box, out mixFlags))
            {
                //if box contains both inline and block 
                //then correct it                
                for (int i = 0; i < allChildren.Count; i++)
                {
                    var curBox = allChildren[i];
                    if (curBox.IsInline)
                    {
                        //1. creat new box anonymous block (no html tag) then
                        //  add it before this box 
                        var newbox = CssBox.CreateBlock(box, null, curBox);
                        //2. skip newly add box 
                        i++;
                        //3. move next child that is inline element to new box                     
                        CssBox tomoveBox = null;
                        while (i < allChildren.Count && ((tomoveBox = allChildren[i]).IsInline))
                        {
                            tomoveBox.ParentBox = newbox;
                        }
                        //so new box contains inline that move from current line

                    }
                }
                //after correction , now all children in this box are block element 
            }
            //------------------------------------------------
            if (mixFlags != HAS_IN_LINE)
            {
                foreach (var childBox in allChildren)
                {
                    //recursive
                    CorrectInlineBoxesParent(childBox);
                }
            }
            //if (!DomUtils.ContainsInlinesOnly(box))
            //{ 
            //    foreach (var childBox in box.Boxes)
            //    {
            //        CorrectInlineBoxesParent(childBox);
            //    }
            //}
        }
        /// <summary>
        /// Gets the next sibling of this box.
        /// </summary>
        /// <returns>Box before this one on the tree. Null if its the first</returns>
        public static CssBox GetNextSibling(CssBox b)
        {
            CssBox sib = null;
            if (b.ParentBox != null)
            {
                var index = b.ParentBox.Boxes.IndexOf(b) + 1;
                while (index <= b.ParentBox.ChildCount - 1)
                {
                    var pSib = b.ParentBox.Boxes[index];
                    //if (pSib.Display != CssConstants.None && pSib.Position != CssConstants.Absolute)
                    if (pSib.CssDisplay != CssDisplay.None && !pSib.IsAbsolutePosition)
                    {
                        sib = pSib;
                        break;
                    }
                    index++;
                }
            }
            return sib;
        }
        /// <summary>
        /// Check if the given box contains only inline child boxes in all subtree.
        /// </summary>
        /// <param name="box">the box to check</param>
        /// <returns>true - only inline child boxes, false - otherwise</returns>
        internal static bool ContainsInlinesOnlyDeep(CssBox box)
        {
            //if (box.PassTestInlineOnlyDeep && box.InlineOnlyDeepResult == true)
            //{
            //    return true;
            //}

            //recursive
            foreach (var childBox in box.GetChildBoxIter())
            {
                //if box is inline then check its sub tree
                if (!childBox.IsInline || !ContainsInlinesOnlyDeep(childBox))
                {
                    // box.SetInlineOnlyDeepResult(false);
                    return false;
                }
            }

            //box.SetInlineOnlyDeepResult(true);
            return true;
        }


        /// <summary>
        /// Go over all the text boxes (boxes that have some text that will be rendered) and
        /// remove all boxes that have only white-spaces but are not 'preformatted' so they do not effect
        /// the rendered html.
        /// </summary>
        /// <param name="box">the current box to correct its sub-tree</param>
        internal static void CorrectTextBoxes(CssBox box)
        {

            for (int i = box.ChildCount - 1; i >= 0; i--)
            {
                var childBox = box.Boxes[i];
                if (childBox.Text != null)
                {
                    // is the box has text
                    var keepBox = !childBox.Text.IsEmptyOrWhitespace();

                    // is the box is pre-formatted
                    //keepBox = keepBox || childBox.WhiteSpace == CssConstants.Pre || childBox.WhiteSpace == CssConstants.PreWrap;
                    keepBox = keepBox || childBox.WhiteSpace == CssWhiteSpace.Pre || childBox.WhiteSpace == CssWhiteSpace.PreWrap;

                    // is the box is only one in the parent
                    keepBox = keepBox || box.ChildCount == 1;

                    // is it a whitespace between two inline boxes
                    keepBox = keepBox || (i > 0 && i < box.ChildCount - 1 && box.Boxes[i - 1].IsInline && box.Boxes[i + 1].IsInline);

                    // is first/last box where is in inline box and it's next/previous box is inline
                    keepBox = keepBox || (i == 0 && box.ChildCount > 1 && box.Boxes[1].IsInline && box.IsInline) ||
                        (i == box.ChildCount - 1 && box.ChildCount > 1 && box.Boxes[i - 1].IsInline && box.IsInline);

                    if (keepBox)
                    {
                        // valid text box, parse it to words
                        childBox.ParseToWords();
                    }
                    else
                    {
                        // remove text box that has no 
                        childBox.ParentBox.Boxes.RemoveAt(i);
                    }
                }
                else
                {
                    // recursive
                    CorrectTextBoxes(childBox);
                }
            }
        }

    }
}