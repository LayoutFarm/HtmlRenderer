//BSD 2014, WinterDev

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


using HtmlRenderer.Utils;

namespace HtmlRenderer.Dom
{

    partial class BoxModelBuilder
    {
        //------------------------------------------
        static void OnePassBoxCorrection(CssBox root)
        {

        }
        /// <summary>
        /// Makes block boxes be among only block boxes and all inline boxes have block parent box.<br/>
        /// Inline boxes should live in a pool of Inline boxes only so they will define a single block.<br/>
        /// At the end of this process a block box will have only block siblings and inline box will have
        /// only inline siblings.
        /// </summary>
        /// <param name="box">the current box to correct its sub-tree</param>
        static void CorrectInlineBoxesParent(CssBox box)
        {
            return;//?
            //------------------------------------------------
            //recursive 
            int mixFlags;
            var allChildren = CssBox.UnsafeGetChildren(box);

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
        /// Correct DOM tree if there is block boxes that are inside inline blocks.<br/>
        /// Need to rearrange the tree so block box will be only the child of other block box.
        /// </summary>
        /// <param name="box">the current box to correct its sub-tree</param>
        static void CorrectBlockInsideInline(CssBox box)
        {

            return;
#if DEBUG
            dbugCorrectCount++;
#endif
            //recursive
            bool containsInlinesOnly = DomUtils.ContainsInlinesOnly(box);
            if (containsInlinesOnly && !ContainsInlinesOnlyDeep(box))
            {
                CorrectBlockInsideInlineImp(box);
                containsInlinesOnly = DomUtils.ContainsInlinesOnly(box);
            }
            //----------------------------------------------------------------------
            if (!containsInlinesOnly)
            {
                foreach (var childBox in box.GetChildBoxIter())
                {
                    //recursive
                    CorrectBlockInsideInline(childBox);
                }
            }
        }


        /// <summary>
        /// Rearrange the DOM of the box to have block box with boxes before the inner block box and after.
        /// </summary>
        /// <param name="box">the box that has the problem</param>
        static void CorrectBlockInsideInlineImp(CssBox box)
        {
            CssBox firstChild = null;

            if (box.ChildCount > 1 || box.GetFirstChild().ChildCount > 1)
            {
                firstChild = box.GetFirstChild();
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

                var splitBox = box.GetChildBox(1);
                ////remove splitbox 
                splitBox.SetNewParentBox(null); 
                CorrectBlockSplitBadBox(box, splitBox, leftAnonBox);
                //------------------------------------------- 

                if (box.ChildCount > 2)
                {
                    var rightAnonBox = CssBox.CreateAnonBlock(box, 2);
                    int childCount = box.ChildCount;
                    while (childCount > 3)
                    {
                        box.GetChildBox(3).SetNewParentBox(3, rightAnonBox);
                        childCount--;
                    }
                }
            }
            else if (firstChild.CssDisplay == CssDisplay.Inline)
            {
                CssBox.ChangeDisplayType(firstChild, CssDisplay.Block);
            }

            if (box.CssDisplay == CssDisplay.Inline)
            {
                CssBox.ChangeDisplayType(box, CssDisplay.Block);
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
            

          
            var leftPart = BoxCreator.CreateBox(leftBlock, (BridgeHtmlElement)splitBox.HtmlElement);

            bool had_new_leftbox = false;
            CssBox firstChild = null;
            bool firstChildHasOnlyInlineDeep = false;

            if (splitBox.HasRuns && splitBox.ChildCount == 0)
            {
                

            }
            else
            {

            }
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
                
                ////recursive
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

                    rightPart = BoxCreator.CreateBox(parentBox, (BridgeHtmlElement)splitBox.HtmlElement);

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
                    rightPart = parentBox.GetChildBox(2);
                }


                CssBoxCollection splitBoxBoxes = CssBox.UnsafeGetChildren(splitBox);
                while (splitBoxBoxes.Count > 0)
                {
                    //move all children to right box                      
                    splitBoxBoxes[0].SetNewParentBox(0, rightPart);
                }
            }
            else if (firstChild.ParentBox != null && parentBox.ChildCount > 1)
            {

                firstChild.ChangeSiblingOrder(1);

                if (firstChild.WellknownTagName == WellknownHtmlTagName.br
                    && (had_new_leftbox || leftBlock.ChildCount > 1))
                {

                    CssBox.ChangeDisplayType(firstChild, CssDisplay.Inline);
                }
            }
        }



    }
}