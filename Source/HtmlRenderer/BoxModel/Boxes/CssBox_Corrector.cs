//BSD 2014, WinterCore

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
        public static CssBox GetPreviousSibling(CssBox b)
        {
            if (b.ParentBox != null)
            {

                CssBox sib = b.PrevSibling;
                if (sib != null)
                {
                    do
                    {
                        if (sib.CssDisplay != CssDisplay.None && !sib.IsAbsolutePosition)
                        {
                            return sib;
                        }
                        sib = sib.PrevSibling;
                    } while (sib != null);
                }
                return null;
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

            foreach (var childBox in allChildren.GetChildBoxIter())
            {
                //recursive to child first
                CorrectLineBreaksBlocks(childBox, ref followingBlock);
                followingBlock = childBox.RunCount == 0 && (followingBlock || childBox.IsBlock);
            }


            CssBox brBox = null;//reset each loop
            int j = allChildren.Count;
            for (int i = 0; i < j; i++)
            {
                var curBox = allChildren[i];
                if (curBox.IsBrElement)
                {
                    brBox = curBox;
                    //check prev box
                    if (i > 0)
                    {
                        var prevBox = allChildren[i - 1];
                        if (prevBox.HasRuns)
                        {
                            followingBlock = false;
                        }
                        else if (prevBox.IsBlock)
                        {
                            followingBlock = true;
                        }
                    }
                    brBox.CssDisplay = CssDisplay.Block;
                    if (followingBlock)
                    {   // atodo: check the height to min-height when it is supported
                        brBox.Height = new CssLength(0.95f, false, CssUnit.Ems);
                    }
                }
            }
        }

        /// <summary>
        /// Check if the given box contains inline and block child boxes.
        /// </summary>
        /// <param name="box">the box to check</param>
        /// <returns>true - has variant child boxes, false - otherwise</returns>
        internal static bool ContainsMixedInlineAndBlockBoxes(CssBox box, out int mixFlags)
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
                if ((mixFlags |= children[i].IsInline ? HAS_IN_LINE : HAS_BLOCK) == (HAS_BLOCK | HAS_IN_LINE))
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
            CssBox firstChild = null;

            if (box.ChildCount > 1 || (firstChild = box.GetFirstChild()).ChildCount > 1)
            {

                CssBox leftAnonBox = CssBox.CreateAnonBlock(box);
                //1. newLeftBlock is Created and add is latest child of the 'box'
                //-------------------------------------------

                while (ContainsInlinesOnlyDeep(firstChild))
                {
                    //if first box has only inline(deep) then
                    //move first child to newLeftBlock ***                     
                    firstChild.SetNewParentBox(leftAnonBox);
                    //next
                    firstChild = box.GetFirstChild();
                }
                //------------------------------------------- 
                //insert left block as leftmost (firstbox) in the line 

                leftAnonBox.ChangeSiblingOrder(0);

                var splitBox = box.Boxes[1];

                splitBox.SetNewParentBox(null);

                CorrectBlockSplitBadBox(box, splitBox, leftAnonBox);

                //------------------------------------------- 

                if (box.ChildCount > 2)
                {
                    var rightAnonBox = CssBox.CreateAnonBlock(box, 2);
                    int childCount = box.ChildCount;
                    while (childCount > 3)
                    {
                        box.Boxes[3].SetNewParentBox(3, rightAnonBox);
                        childCount--;
                    }
                }
            }
            else if (firstChild.CssDisplay == CssDisplay.Inline)
            {
                firstChild.CssDisplay = CssDisplay.Block;
            }

            if (box.CssDisplay == CssDisplay.Inline)
            {
                box.CssDisplay = CssDisplay.Block;
            }
        }

        /// <summary>
        /// Split bad box that has inline and block boxes into two parts, the left - before the block box
        /// and right - after the block box.
        /// </summary>
        /// <param name="parentBox">the parent box that has the problem</param>
        /// <param name="splitBox">the box to split into different boxes</param>
        /// <param name="leftBlock">the left block box that is created for the split</param>
        static void CorrectBlockSplitBadBox(CssBox parentBox, CssBox splitBox, CssBox leftBlock)
        {
            //recursive

            var leftPart = CssBox.CreateBox(leftBlock, splitBox.HtmlTag);
            leftPart.InheritStyle(splitBox, true);

            bool had_new_leftbox = false;
            CssBox firstChild = null;
            bool firstChildHasOnlyInlineDeep = false;

            while ((firstChild = splitBox.GetFirstChild()).IsInline &&
                (firstChildHasOnlyInlineDeep = ContainsInlinesOnlyDeep(firstChild)))
            {
                had_new_leftbox = true;
                //move element that has only inline(deep) to leftbox
                firstChild.SetNewParentBox(leftPart);
                firstChildHasOnlyInlineDeep = false;//reset
            }
            if (!firstChildHasOnlyInlineDeep)
            {
                //recursive
                CorrectBlockSplitBadBox(parentBox, firstChild, leftBlock);
                //remove self
                firstChild.SetNewParentBox(null);
            }
            else
            {
                firstChild.SetNewParentBox(parentBox);
            }

            if (splitBox.ChildCount > 0)
            {
                CssBox rightPart;
                if (firstChild.ParentBox != null || parentBox.ChildCount < 3)
                {
                    rightPart = CssBox.CreateBox(parentBox, splitBox.HtmlTag);
                    rightPart.InheritStyle(splitBox, true);

                    if (parentBox.ChildCount > 2)
                    {
                        rightPart.ChangeSiblingOrder(1);
                    }
                    if (firstChild.ParentBox != null)
                    {
                        firstChild.ChangeSiblingOrder(1);
                    }
                }
                else
                {
                    rightPart = parentBox.Boxes[2];
                }


                CssBoxCollection splitBoxBoxes = splitBox.Boxes;
                while (splitBoxBoxes.Count > 0)
                {
                    //move all children to right box                      
                    splitBoxBoxes[0].SetNewParentBox(0, rightPart);
                }
            }
            else if (firstChild.ParentBox != null && parentBox.ChildCount > 1)
            {

                firstChild.ChangeSiblingOrder(1);

                if (firstChild.WellknownTagName == WellknownHtmlTagName.BR
                    && (had_new_leftbox || leftBlock.ChildCount > 1))
                {
                    firstChild.CssDisplay = CssDisplay.Inline;

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

            if (ContainsMixedInlineAndBlockBoxes(box, out mixFlags))
            {
                //if box contains both inline and block 
                //then make all children to be block box

                for (int i = 0; i < allChildren.Count; i++)
                {
                    var curBox = allChildren[i];
                    if (curBox.IsInline)
                    {
                        //1. creat new box anonymous block (no html tag) then
                        //  add it before this box 

                        var newbox = CssBox.CreateAnonBlock(box, i);
                        //2. skip newly add box 
                        i++;

                        //3. move next  inline child box to new anonymous box                                              
                        CssBox tomoveBox = null;
                        while (i < allChildren.Count && ((tomoveBox = allChildren[i]).IsInline))
                        {
                            //** allChildren number will be changed after move****    
                            tomoveBox.SetNewParentBox(i, newbox);
                        }
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

        }

        /// <summary>
        /// Check if the given box contains only inline child boxes in all subtree.
        /// </summary>
        /// <param name="box">the box to check</param>
        /// <returns>true - only inline child boxes, false - otherwise</returns>
        internal static bool ContainsInlinesOnlyDeep(CssBox box)
        {
            //recursive
            foreach (var childBox in box.GetChildBoxIter())
            {
                if (!childBox.IsInline || !ContainsInlinesOnlyDeep(childBox))
                {

                    return false;
                }
            }
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
            CssBoxCollection boxes = box.Boxes;

            for (int i = boxes.Count - 1; i >= 0; i--)
            {
                var childBox = boxes[i];
                if (childBox.MayHasSomeTextContent)
                {
                    // is the box has text
                    // or is the box is pre-formatted
                    // or is the box is only one in the parent


                    bool keepBox = !childBox.TextContentIsWhitespaceOrEmptyText ||
                        childBox.WhiteSpace == CssWhiteSpace.Pre ||
                        childBox.WhiteSpace == CssWhiteSpace.PreWrap ||
                        boxes.Count == 1;

                    if (!keepBox && box.ChildCount > 0)
                    {
                        if (i == 0)
                        {
                            //first
                            // is first/last box where is in inline box and it's next/previous box is inline
                            keepBox = box.IsInline && boxes[1].IsInline;
                        }
                        else if (i == box.ChildCount - 1)
                        {
                            //last
                            // is first/last box where is in inline box and it's next/previous box is inline
                            keepBox = box.IsInline && boxes[i - 1].IsInline;
                        }
                        else
                        {
                            //between
                            // is it a whitespace between two inline boxes
                            keepBox = boxes[i - 1].IsInline && boxes[i + 1].IsInline;
                        }
                    }
                    if (keepBox)
                    {
                        // valid text box, parse it to words  
                        childBox.ParseWordContent();
                    }
                    else
                    {
                        boxes.RemoveAt(i);
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