//BSD 2014, WinterDev 
using System;

namespace HtmlRenderer.Dom
{
#if DEBUG
    partial class BoxModelBuilder
    {
        
        static void dbugCompareSpecDiff(string prefix, CssBox box)
        {
            BridgeHtmlElement element = box.HtmlElement as BridgeHtmlElement;
            BoxSpec curSpec = CssBox.UnsafeGetBoxSpec(box);

            dbugPropCheckReport rep = new dbugPropCheckReport();
            if (element != null)
            {
                if (!BoxSpec.dbugCompare(
                    rep,
                    element.Spec,
                    curSpec))
                {
                    foreach (string s in rep.GetList())
                    {
                        Console.WriteLine(prefix + s);
                    }

                }
            }
            else
            {
                if (box.dbugAnonCreator != null)
                {
                    if (!BoxSpec.dbugCompare(
                    rep,
                    box.dbugAnonCreator.Spec.GetAnonVersion(),
                    curSpec))
                    {
                        foreach (string s in rep.GetList())
                        {
                            Console.WriteLine(prefix + s);
                        }
                    }
                }
                else
                {

                }
            }

        }


        /// <summary>
        /// Go over all the text boxes (boxes that have some text that will be rendered) and
        /// remove all boxes that have only white-spaces but are not 'preformatted' so they do not effect
        /// the rendered html.
        /// </summary>
        /// <param name="box">the current box to correct its sub-tree</param>
        static void dbugCorrectTextBoxes(CssBox box)
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
                    bool keepBox = childBox.HtmlElement != null;
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
                        childBox.UpdateRunList();
                    }
                    else
                    {

                        boxes.RemoveAt(i);
                    }
                }
                else
                {
                    // recursive 
                    dbugCorrectTextBoxes(childBox);
                }
            }
        }
        /// <summary>
        /// Go over all image boxes and if its display style is set to block, 
        /// put it inside another block but set the image to inline.
        /// </summary>
        /// <param name="box">the current box to correct its sub-tree</param>
        static void dbugCorrectImgBoxes(CssBox box)
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
                    dbugCorrectImgBoxes(childBox);
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
        static void dbugCorrectLineBreaksBlocks(CssBox box, ref bool followingBlock)
        {

            followingBlock = followingBlock || box.IsBlock;
            foreach (var childBox in box.GetChildBoxIter())
            {
                dbugCorrectLineBreaksBlocks(childBox, ref followingBlock);
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
                        brBox.DirectSetHeight(ConstConfig.DEFAULT_FONT_SIZE * 0.95f);
                        //brBox.Height = new CssLength(0.95f, CssUnitOrNames.Ems);
                    }
                }
            }
        }
    
    }


#endif
}