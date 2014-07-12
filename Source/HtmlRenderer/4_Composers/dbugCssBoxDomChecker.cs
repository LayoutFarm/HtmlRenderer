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


using HtmlRenderer.Drawing;
using HtmlRenderer.Css;
using HtmlRenderer.RenderDom; 
namespace HtmlRenderer.Composers
{
#if DEBUG

    class dbugCssBoxDomChecker
    {


        static int dbugCorrectCount = 0;
        /// <summary>
        /// Go over all the text boxes (boxes that have some text that will be rendered) and
        /// remove all boxes that have only white-spaces but are not 'preformatted' so they do not effect
        /// the rendered html.
        /// </summary>
        /// <param name="box">the current box to correct its sub-tree</param>
        static void CorrectTextBoxes(CssBox box)
        {
            return;

            CssBoxCollection boxes = CssBox.UnsafeGetChildren(box);
            for (int i = boxes.Count - 1; i >= 0; i--)
            {
                var childBox = boxes[i];
                if (childBox.MayHasSomeTextContent)
                {
                    // is the box has text
                    // or is the box is pre-formatted
                    // or is the box is only one in the parent 
                    var element = CssBox.debugGetController(childBox) as BridgeHtmlElement;
                    bool keepBox = element != null;
                    if (!keepBox)
                    {
                        keepBox = !childBox.TextContentIsWhitespaceOrEmptyText ||
                         childBox.WhiteSpace == CssWhiteSpace.Pre ||
                         childBox.WhiteSpace == CssWhiteSpace.PreWrap ||
                         boxes.Count == 1;
                    }

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
                        //childBox.UpdateRunList();
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
        /// <summary>
        /// Go over all image boxes and if its display style is set to block, 
        /// put it inside another block but set the image to inline.
        /// </summary>
        /// <param name="box">the current box to correct its sub-tree</param>
        static void CorrectImgBoxes(CssBox box)
        {
            int childIndex = 0;
            foreach (var childBox in box.GetChildBoxIter())
            {

                if (childBox is CssBoxImage && childBox.CssDisplay == CssDisplay.Block)
                {
                    //create new anonymous box
                    var block = CssBox.CreateAnonBlock(childBox.ParentBox, childIndex);
                    //move this imgbox to new child 
                    childBox.SetNewParentBox(block);
                    CssBox.ChangeDisplayType(childBox, CssDisplay.Inline);
                }
                else
                {
                    // recursive
                    CorrectImgBoxes(childBox);
                }
                childIndex++;
            }
        }


        /// <summary>
        /// Correct the DOM tree recursively by replacing  "br" html boxes with anonymous blocks that respect br spec.<br/>
        /// If the "br" tag is after inline box then the anon block will have zero height only acting as newline,
        /// but if it is after block box then it will have min-height of the font size so it will create empty line.
        /// </summary>
        /// <param name="box">the current box to correct its sub-tree</param>
        /// <param name="followingBlock">used to know if the br is following a box so it should create an empty line or not so it only
        /// move to a new line</param>
        static void CorrectLineBreaksBlocks(CssBox box, ref bool followingBlock)
        {

            followingBlock = followingBlock || box.IsBlock;
            foreach (var childBox in box.GetChildBoxIter())
            {
                CorrectLineBreaksBlocks(childBox, ref followingBlock);
                followingBlock = (followingBlock || childBox.IsBlock);
            }

            CssBox brBox = null;//reset each loop
            int j = box.ChildCount;
            for (int i = 0; i < j; i++)
            {
                var curBox = box.GetChildBox(i);

                if (curBox.IsBrElement)
                {
                    brBox = curBox;
                    //check prev box
                    if (i > 0)// is not first child 
                    {
                        var prevBox = box.GetChildBox(i - 1);
                        if (prevBox.HasRuns)
                        {
                            followingBlock = false;
                        }
                        else if (prevBox.IsBlock)
                        {
                            followingBlock = true;
                        }
                    }


                    CssBox.ChangeDisplayType(brBox, CssDisplay.Block);
                    if (followingBlock)
                    {

                        // atodo: check the height to min-height when it is supported
                        //throw new NotSupportedException();
                        brBox.DirectSetHeight(CssConstConfig.DEFAULT_FONT_SIZE * 0.95f);
                        //brBox.Height = new CssLength(0.95f, CssUnitOrNames.Ems);
                    }
                }
            }
        }

        /// <summary>
        /// Check if the given box contains inline and block child boxes.
        /// </summary>
        /// <param name="box">the box to check</param>
        /// <returns>true - has variant child boxes, false - otherwise</returns>
        static bool ContainsMixedInlineAndBlockBoxes(CssBox box, out int mixFlags)
        {
            if (box.ChildCount == 0 && box.HasRuns)
            {
                mixFlags = HAS_IN_LINE;
                return false;
            }
            mixFlags = 0;
            var children = CssBox.UnsafeGetChildren(box);
            for (int i = children.Count - 1; i >= 0; --i)
            {
                if (children[i].IsInline)
                {
                    mixFlags |= HAS_IN_LINE;
                }
                else
                {
                    mixFlags |= HAS_BLOCK;
                }

                if (mixFlags == (HAS_BLOCK | HAS_IN_LINE))
                {
                    return true;
                }

            }
            return false;
        }


        const int HAS_BLOCK = 1 << (1 - 1);
        const int HAS_IN_LINE = 1 << (2 - 1);

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
        /// Check if the given box contains only inline child boxes in all subtree.
        /// </summary>
        /// <param name="box">the box to check</param>
        /// <returns>true - only inline child boxes, false - otherwise</returns>
        static bool ContainsInlinesOnlyDeep(CssBox box)
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
            else if (firstChild.CssDisplay == Css.CssDisplay.Inline)
            {
                CssBox.ChangeDisplayType(firstChild, Css.CssDisplay.Block);
            }

            if (box.CssDisplay == Css.CssDisplay.Inline)
            {
                CssBox.ChangeDisplayType(box, Css.CssDisplay.Block);
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


            var htmlE = CssBox.debugGetController(splitBox) as BridgeHtmlElement;
            var leftPart = BoxCreator.CreateBox(leftBlock, htmlE);

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

                    rightPart = BoxCreator.CreateBox(parentBox, htmlE);

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
                if (firstChild.IsBrElement && (had_new_leftbox || leftBlock.ChildCount > 1))
                {
                    CssBox.ChangeDisplayType(firstChild, CssDisplay.Inline);
                }
            }
        }

        static void dbugCompareSpecDiff(string prefix, CssBox box)
        {
            var element = CssBox.debugGetController(box) as BridgeHtmlElement;

            BoxSpec curSpec = CssBox.UnsafeGetBoxSpec(box);

            dbugPropCheckReport rep = new dbugPropCheckReport();
            if (element != null)
            {
                if (!BoxSpec.dbugCompare(
                    rep,
                    element.Spec,
                    curSpec))
                {
                    //foreach (string s in rep.GetList())
                    //{
                    //    Console.WriteLine(prefix + s);
                    //}

                }
            }
            else
            {
                //if (box.dbugAnonCreator != null)
                //{
                //    if (!BoxSpec.dbugCompare(
                //    rep,
                //    box.dbugAnonCreator.Spec.GetAnonVersion(),
                //    curSpec))
                //    {
                //        //foreach (string s in rep.GetList())
                //        //{
                //        //    Console.WriteLine(prefix + s);
                //        //}
                //    }
                //}
                //else
                //{

                //}
            }

        }

    }
    //==================================
    //old code backup from BoxModelBuilder

    //==================================
    //        /// <summary>
    //        /// Generate css tree by parsing the given html and applying the given css style data on it.
    //        /// </summary>
    //        /// <param name="html">the html to parse</param>
    //        /// <param name="htmlContainer">the html container to use for reference resolve</param>
    //        /// <param name="cssData">the css data to use</param>
    //        /// <returns>the root of the generated tree</returns>
    //        public static CssBox ParseAndBuildBoxTree(
    //            string html,
    //            HtmlContainer htmlContainer,
    //            CssActiveSheet cssData)
    //        {

    //            //1. parse
    //            HtmlDocument htmldoc = ParseDocument(new TextSnapshot(html.ToCharArray()));
    //            //2. active css template
    //            //----------------------------------------------------------------
    //            ActiveCssTemplate activeCssTemplate = new ActiveCssTemplate(cssData);
    //            //3. create bridge root
    //            BrigeRootElement bridgeRoot = CreateBridgeTree(htmlContainer, htmldoc, activeCssTemplate);
    //            //---------------------------------------------------------------- 


    //            //attach style to elements
    //            ApplyStyleSheetForBridgeElement(bridgeRoot, null, activeCssTemplate);

    //            //----------------------------------------------------------------
    //            //box generation
    //            //3. create cssbox from root
    //            CssBox rootBox = BoxCreator.CreateRootBlock();
    //            GenerateCssBoxes(bridgeRoot, rootBox);

    //#if DEBUG
    //            dbugTestParsePerformance(html);
    //#endif

    //            //2. decorate cssbox with styles
    //            if (rootBox != null)
    //            {

    //                CssBox.SetHtmlContainer(rootBox, htmlContainer);
    //                SetTextSelectionStyle(htmlContainer, cssData); 
    //                OnePassBoxCorrection(rootBox);

    //#if DEBUG
    //                //may not need ?, left this method
    //                //if want to check 
    //                //dbugCorrectTextBoxes(rootBox);
    //                //dbugCorrectImgBoxes(rootBox); 
    //                //bool followingBlock = true;
    //                //dbugCorrectLineBreaksBlocks(rootBox, ref followingBlock);


    //                //1. must test first
    //                //dbugCorrectInlineBoxesParent(rootBox);
    //                //2. then ...
    //                //dbugCorrectBlockInsideInline(rootBox);
    //#endif
    //            }
    //            return rootBox;
    //        }
#endif
}
