//BSD 2014, WinterDev
//ArthurHub
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
using HtmlRenderer.Css;
using HtmlRenderer.Drawing;
namespace HtmlRenderer.Boxes
{


    /// <summary>
    /// Helps on CSS Layout.
    /// </summary>
    static class CssLayoutEngine
    {

        const float CSS_OFFSET_THRESHOLD = 0.1f;

        /// <summary>
        /// Measure image box size by the width\height set on the box and the actual rendered image size.<br/>
        /// If no image exists for the box error icon will be set.
        /// </summary>
        /// <param name="imgRun">the image word to measure</param>
        public static void MeasureImageSize(CssImageRun imgRun, LayoutVisitor lay)
        {
            var width = imgRun.OwnerBox.Width;
            var height = imgRun.OwnerBox.Height;

            bool hasImageTagWidth = width.Number > 0 && width.UnitOrNames == CssUnitOrNames.Pixels;
            bool hasImageTagHeight = height.Number > 0 && height.UnitOrNames == CssUnitOrNames.Pixels;
            bool scaleImageHeight = false;

            if (hasImageTagWidth)
            {
                imgRun.Width = width.Number;
            }
            else if (width.Number > 0 && width.IsPercentage)
            {

                imgRun.Width = width.Number * lay.LatestContainingBlock.SizeWidth;
                scaleImageHeight = true;
            }
            else if (imgRun.HasUserImageContent)
            {
                imgRun.Width = imgRun.ImageRectangle == Rectangle.Empty ? imgRun.OriginalImageWidth : imgRun.ImageRectangle.Width;
            }
            else
            {
                imgRun.Width = hasImageTagHeight ? height.Number / 1.14f : 20;
            }

            var maxWidth = imgRun.OwnerBox.MaxWidth;// new CssLength(imageWord.OwnerBox.MaxWidth);
            if (maxWidth.Number > 0)
            {
                float maxWidthVal = -1;
                switch (maxWidth.UnitOrNames)
                {
                    case CssUnitOrNames.Percent:
                        {
                            maxWidthVal = maxWidth.Number * lay.LatestContainingBlock.SizeWidth;
                        } break;
                    case CssUnitOrNames.Pixels:
                        {
                            maxWidthVal = maxWidth.Number;
                        } break;
                }


                if (maxWidthVal > -1 && imgRun.Width > maxWidthVal)
                {
                    imgRun.Width = maxWidthVal;
                    scaleImageHeight = !hasImageTagHeight;
                }
            }

            if (hasImageTagHeight)
            {
                imgRun.Height = height.Number;
            }
            else if (imgRun.HasUserImageContent)
            {
                imgRun.Height = imgRun.ImageRectangle == Rectangle.Empty ? imgRun.OriginalImageHeight : imgRun.ImageRectangle.Height;
            }
            else
            {
                imgRun.Height = imgRun.Width > 0 ? imgRun.Width * 1.14f : 22.8f;
            }

            if (imgRun.HasUserImageContent)
            {
                // If only the width was set in the html tag, ratio the height.
                if ((hasImageTagWidth && !hasImageTagHeight) || scaleImageHeight)
                {
                    // Divide the given tag width with the actual image width, to get the ratio.
                    float ratio = imgRun.Width / imgRun.OriginalImageWidth;
                    imgRun.Height = imgRun.OriginalImageHeight * ratio;
                }
                // If only the height was set in the html tag, ratio the width.
                else if (hasImageTagHeight && !hasImageTagWidth)
                {
                    // Divide the given tag height with the actual image height, to get the ratio.
                    float ratio = imgRun.Height / imgRun.OriginalImageHeight;
                    imgRun.Width = imgRun.OriginalImageWidth * ratio;
                }
            }
            //imageWord.Height += imageWord.OwnerBox.ActualBorderBottomWidth + imageWord.OwnerBox.ActualBorderTopWidth + imageWord.OwnerBox.ActualPaddingTop + imageWord.OwnerBox.ActualPaddingBottom;
        }
        /// <summary>
        /// Check if the given box contains only inline child boxes.
        /// </summary>
        /// <param name="box">the box to check</param>
        /// <returns>true - only inline child boxes, false - otherwise</returns>
        static bool ContainsInlinesOnly(CssBox box)
        {
            var children = CssBox.UnsafeGetChildren(box);
            var linkedNode = children.GetFirstLinkedNode();
            while (linkedNode != null)
            {
                if (!linkedNode.Value.IsInline)
                {
                    return false;
                }
                linkedNode = linkedNode.Next;
            }
            return true;
        }
        public static void PerformContentLayout(CssBox box, LayoutVisitor lay)
        {

            //this box has its own  container property
            //this box may use...
            // 1) line formatting context  , or
            // 2) block formatting context 

            var myContainingBlock = lay.LatestContainingBlock;
            if (box.CssDisplay != Css.CssDisplay.TableCell)
            {
                //-------------------------------------------
                if (box.CssDisplay != Css.CssDisplay.Table)
                {
                    float availableWidth = myContainingBlock.ClientWidth;

                    if (!box.Width.IsEmptyOrAuto)
                    {
                        availableWidth = CssValueParser.ConvertToPx(box.Width, availableWidth, box);
                    }

                    box.SetWidth(availableWidth);
                    // must be separate because the margin can be calculated by percentage of the width
                    box.SetWidth(availableWidth - box.ActualMarginLeft - box.ActualMarginRight);
                }
                //-------------------------------------------

                float localLeft = myContainingBlock.ClientLeft + box.ActualMarginLeft;
                float localTop = 0;
                var prevSibling = lay.LatestSiblingBox;

                if (prevSibling == null)
                {
                    //this is first child of parent
                    if (box.ParentBox != null)
                    {
                        localTop = myContainingBlock.ClientTop;
                    }
                }
                else
                {
                    localTop = prevSibling.LocalBottom + prevSibling.ActualBorderBottomWidth;
                }

                localTop += box.MarginTopCollapse(prevSibling);

                box.SetLocation(localLeft, localTop);
                box.SetHeightToZero();
            }
            //--------------------------------------------------------------------------

            switch (box.CssDisplay)
            {
                case Css.CssDisplay.Table:
                case Css.CssDisplay.InlineTable:
                    {
                        //If we're talking about a table here..

                        lay.PushContaingBlock(box);
                        var currentLevelLatestSibling = lay.LatestSiblingBox;
                        lay.LatestSiblingBox = null;//reset

                        CssTableLayoutEngine.PerformLayout(box, lay);

                        lay.LatestSiblingBox = currentLevelLatestSibling;
                        lay.PopContainingBlock();

                    } break;
                default:
                    {
                        //formatting context for...
                        //1. line formatting context
                        //2. block formatting context     
                        if (ContainsInlinesOnly(box))
                        {
                            //This will automatically set the bottom of this block
                            PerformLayoutLinesContext(box, lay);
                        }
                        else if (box.ChildCount > 0)
                        {
                            PerformLayoutBlocksContext(box, lay);
                        }
                    } break;
            }
        }
        /// <summary>
        /// do layout line formatting context
        /// </summary>
        /// <param name="hostBlock"></param>
        /// <param name="lay"></param>
        static void PerformLayoutLinesContext(CssBox hostBlock, LayoutVisitor lay)
        {
            //this in line formatting context
            //*** hostBlock must confirm that it has all inline children             
            hostBlock.SetHeightToZero();
            hostBlock.ResetLineBoxes();

            //----------------------------------------------------------------------------------------
            float limitLocalRight = hostBlock.SizeWidth - (hostBlock.ActualPaddingRight + hostBlock.ActualBorderRightWidth);

            float localX = hostBlock.ActualTextIndent + hostBlock.ActualPaddingLeft + hostBlock.ActualBorderLeftWidth;
            float localY = hostBlock.ActualPaddingTop + hostBlock.ActualBorderTopWidth;

            float enterLocalY = localY;

            float startLocalX = localX;

            //Reminds the maximum bottom reached
            float maxLocalRight = localX;
            float maxLocalBottom = localY;
            int interlineSpace = 0;

            //First line box
            {
                CssLineBox line = new CssLineBox(hostBlock);
                hostBlock.AddLineBox(line);
                //****
                FlowBoxContentIntoHost(lay, hostBlock, hostBlock,
                      limitLocalRight, startLocalX,
                      ref line, ref localX);
            }

            //**** 
            // if width is not restricted we need to lower it to the actual width
            if (hostBlock.SizeWidth + lay.ContainerBlockGlobalX >= CssBoxConstConfig.BOX_MAX_RIGHT)
            {
                float newWidth = maxLocalRight + hostBlock.ActualPaddingRight + hostBlock.ActualBorderRightWidth;// CssBox.MAX_RIGHT - (args.ContainerBlockGlobalX + blockBox.LocalX);
                if (newWidth <= CSS_OFFSET_THRESHOLD)
                {
                    newWidth = CSS_OFFSET_THRESHOLD;
                }
                hostBlock.SetWidth(newWidth);
            }
            //---------------------
            float totalHeight = 0;
            if (hostBlock.CssDirection == CssDirection.Rtl)
            {
                float cy = enterLocalY;
                CssTextAlign textAlign = hostBlock.CssTextAlign;
                foreach (CssLineBox linebox in hostBlock.GetLineBoxIter())
                {

                    ApplyAlignment(linebox, textAlign, lay);
                    ApplyRightToLeft(linebox); //***

                    linebox.CloseLine(lay); //*** 

                    linebox.CachedLineTop = cy;
                    cy += linebox.CacheLineHeight + interlineSpace; // + interline space?
                }
                totalHeight = cy;
            }
            else
            {
                float cy = enterLocalY;
                CssTextAlign textAlign = hostBlock.CssTextAlign;
                foreach (CssLineBox linebox in hostBlock.GetLineBoxIter())
                {
                    ApplyAlignment(linebox, textAlign, lay);

                    linebox.CloseLine(lay); //***

                    linebox.CachedLineTop = cy;
                    cy += linebox.CacheLineHeight + interlineSpace;
                }
                totalHeight = cy;
            }

            //hostBlock.SetHeight(maxLocalBottom + hostBlock.ActualPaddingBottom + hostBlock.ActualBorderBottomWidth);
            hostBlock.SetHeight(totalHeight + hostBlock.ActualPaddingBottom + hostBlock.ActualBorderBottomWidth);
            // handle limiting block height when overflow is hidden             
            if (hostBlock.Overflow == CssOverflow.Hidden &&
               !hostBlock.Height.IsEmptyOrAuto &&
               hostBlock.SizeHeight > hostBlock.ExpectedHeight)
            {
                hostBlock.SetHeight(hostBlock.ExpectedHeight);
            }
        }

        static void PerformLayoutBlocksContext(CssBox box, LayoutVisitor lay)
        {
            //block formatting context.... 
            lay.PushContaingBlock(box);
            var currentLevelLatestSibling = lay.LatestSiblingBox;
            lay.LatestSiblingBox = null;//reset 
            //------------------------------------------  
            var children = CssBox.UnsafeGetChildren(box);
            var cnode = children.GetFirstLinkedNode();
            while (cnode != null)
            {
                var childBox = cnode.Value;
                //----------------------------
                if (childBox.IsBrElement)
                {
                    //br always block
                    CssBox.ChangeDisplayType(childBox, Css.CssDisplay.Block);
                    childBox.DirectSetHeight(FontDefaultConfig.DEFAULT_FONT_SIZE * 0.95f);
                }
                //-----------------------------
                if (childBox.IsInline)
                {
                    //inline correction on-the-fly ! 
                    //1. collect consecutive inlinebox
                    //   and move to new anon box
                    CssBox anoForInline = CssBox.CreateAnonBlock(box, childBox);
                    anoForInline.ReEvaluateComputedValues(lay.Gfx, box);

                    var tmp = cnode.Next;
                    do
                    {
                        children.Remove(childBox);
                        anoForInline.AppendChild(childBox);

                        if (tmp != null)
                        {
                            childBox = tmp.Value;
                            if (childBox.IsInline)
                            {
                                tmp = tmp.Next;
                                if (tmp == null)
                                {

                                    children.Remove(childBox);
                                    anoForInline.AppendChild(childBox);
                                    break;
                                }
                            }
                            else
                            {
                                break;//break from do while
                            }
                        }
                        else
                        {
                            break;
                        }
                    } while (true);

                    childBox = anoForInline;
                    //------------------------   
                    //2. move this inline box 
                    //to new anonbox 
                    cnode = tmp;
                    //------------------------ 
                    childBox.PerformLayout(lay);

                    if (childBox.CanBeRefererenceSibling)
                    {
                        lay.LatestSiblingBox = childBox;
                    }
                }
                else
                {
                    //----------------------------
                    childBox.PerformLayout(lay);
                    if (childBox.CanBeRefererenceSibling)
                    {
                        lay.LatestSiblingBox = childBox;
                    }

                    cnode = cnode.Next;
                }
            }

            //------------------------------------------
            lay.LatestSiblingBox = currentLevelLatestSibling;
            lay.PopContainingBlock();
            //------------------------------------------------ 
            float width = box.CalculateActualWidth();
            if (lay.ContainerBlockGlobalX + width > CssBoxConstConfig.BOX_MAX_RIGHT)
            {
            }
            else
            {
                if (box.CssDisplay != Css.CssDisplay.TableCell)
                {
                    box.SetWidth(width);
                }
            }
            box.SetHeight(box.GetHeightAfterMarginBottomCollapse(lay.LatestContainingBlock));
        }




#if DEBUG
        static int dbugPassTotal = 0;
#endif
        /// <summary>
        /// Recursively flows the content of the box using the inline model
        /// </summary>
        /// <param name="g">Device Info</param>
        /// <param name="hostBox">Blockbox that contains the text flow</param>
        /// <param name="splitableBox">Current box to flow its content</param>
        /// <param name="limitLocalRight">Maximum reached right</param>
        /// <param name="interLineSpace">Space to use between rows of text</param>
        /// <param name="firstRunStartX">x starting coordinate for when breaking lines of text</param>
        /// <param name="hostLine">Current linebox being used</param>
        /// <param name="current_line_x">Current x coordinate that will be the left of the next word</param>
        /// <param name="current_line_y">Current y coordinate that will be the top of the next word</param>
        /// <param name="maxRightForHostBox">Maximum right reached so far</param>
        /// <param name="maxBottomForHostBox">Maximum bottom reached so far</param>
        static void FlowBoxContentIntoHost(
          LayoutVisitor lay,
          CssBox hostBox,
          CssBox splitableBox,
          float limitLocalRight,
          float firstRunStartX,
          ref CssLineBox hostLine,
          ref float current_line_x)
        {


            var oX = current_line_x;
             

            int childNumber = 0;

            if (splitableBox.MayHasSomeTextContent)
            {
                float leftMostSpace = 0, rightMostSpace = 0;
                FlowRunsIntoHost(lay, hostBox, splitableBox, splitableBox, childNumber, limitLocalRight, firstRunStartX,
                     ref hostLine, ref current_line_x,
                     leftMostSpace, rightMostSpace);
            }
            else
            {
                float leftMostSpace = 0, rightMostSpace = 0;
                int totalChildCount = splitableBox.ChildCount;

                bool splitableParentIsBlock = splitableBox.ParentBox.IsBlock;

                var fontPool = lay.Gfx;
                foreach (CssBox b in splitableBox.GetChildBoxIter())
                {
                    if (b.Position == CssPosition.Absolute)
                    {
                        leftMostSpace = b.ActualMarginLeft + b.ActualBorderLeftWidth + b.ActualPaddingLeft;
                        rightMostSpace = b.ActualMarginRight + b.ActualBorderRightWidth + b.ActualPaddingRight;
                    }

                    if (b.NeedComputedValueEvaluation)
                    {
                        b.ReEvaluateComputedValues(fontPool, hostBox);
                    }

                    b.MeasureRunsSize(lay); 
                    current_line_x += leftMostSpace;


                    //--------------------------------------------------------------------
                    //not used in this version ***
                    //if (b.CssDisplay == CssDisplay.BlockInsideInlineAfterCorrection)
                    //{ 
                    //    //-----------------------------------------
                    //    lay.PushContaingBlock(hostBox);
                    //    var currentLevelLatestSibling = lay.LatestSiblingBox;
                    //    lay.LatestSiblingBox = null;//reset

                    //    b.PerformLayout(lay);

                    //    lay.LatestSiblingBox = currentLevelLatestSibling;
                    //    lay.PopContainingBlock();
                    //    //----------------------------------------- 
                    //    var newline = new CssLineBox(hostBox);
                    //    hostBox.AddLineBox(newline);
                    //    //reset x pos for new line
                    //    current_line_x = firstRunStartX;
                    //    //set y to new line      
                    //    newline.CachedLineTop = current_line_y = maxBottomForHostBox + interLineSpace;

                    //    CssBlockRun blockRun = new CssBlockRun(b);
                    //    newline.AddRun(blockRun);

                    //    blockRun.SetLocation(firstRunStartX, 0);
                    //    blockRun.SetSize(b.SizeWidth, b.SizeHeight);

                    //    maxBottomForHostBox += b.SizeHeight;
                    //    //-----------------------------------------
                    //    if (childNumber < totalChildCount)
                    //    {
                    //        //this is not last child
                    //        //create new line 
                    //        newline = new CssLineBox(hostBox);
                    //        hostBox.AddLineBox(newline);
                    //        newline.CachedLineTop = current_line_y = maxBottomForHostBox + interLineSpace;
                    //    } 
                    //    hostLine = newline;
                    //    continue;
                    //}
                    //--------------------------------------------------------------------

                    if (!b.HasRuns)
                    {
                        //go deeper  
                        FlowBoxContentIntoHost(lay, hostBox, b, limitLocalRight, firstRunStartX,
                            ref hostLine, ref current_line_x);
                    }
                    else
                    {
                        FlowRunsIntoHost(lay, hostBox, splitableBox, b, childNumber, limitLocalRight, firstRunStartX,
                            ref hostLine, ref current_line_x,
                            leftMostSpace, rightMostSpace);
                    }

                    current_line_x += rightMostSpace;
                    childNumber++;
                }
            }
            //------------

            //------------ 
            // handle width setting
            if (splitableBox.IsInline &&
                0 <= current_line_x - oX && current_line_x - oX < splitableBox.ExpectedWidth)
            {
                throw new NotSupportedException();
            }

            // hack to support specific absolute position elements 
            if (splitableBox.Position == CssPosition.Absolute)
            {
                current_line_x = oX;
                AdjustAbsolutePosition(splitableBox, 0, 0);
            }
        }

        static void FlowRunsIntoHost(LayoutVisitor lay,
          CssBox hostBox,
          CssBox splitableBox,
          CssBox b,
          int childNumber, //child number of b
          float limitLocalRight,
          float firstRunStartX,
          ref CssLineBox hostLine,
          ref float cx,
          float leftMostSpace,
          float rightMostSpace)
        {
            //flow runs into hostLine, create new line if need  
            List<CssRun> runs = CssBox.UnsafeGetRunList(b);
            bool wrapNoWrapBox = false;

            bool splitableParentIsBlock = splitableBox.ParentBox.IsBlock;
            if (b.WhiteSpace == CssWhiteSpace.NoWrap && cx > firstRunStartX)
            {
                var tmpRight = cx;
                for (int i = runs.Count - 1; i >= 0; --i)
                {
                    tmpRight += runs[i].Width;
                }
                //----------------------------------------- 
                if (tmpRight > limitLocalRight)
                {
                    wrapNoWrapBox = true;
                }
            }

            //----------------------------------------------------- 
            int j = runs.Count;
            //bool splitableParentIsBlock = splitableBox.ParentBox.IsBlock;
            for (int i = 0; i < j; ++i)
            {
                var run = runs[i];
                //---------------------------------------------------
                //check if need to start new line ? 
                if ((cx + run.Width + rightMostSpace > limitLocalRight &&
                     b.WhiteSpace != CssWhiteSpace.NoWrap &&
                     b.WhiteSpace != CssWhiteSpace.Pre &&
                     (b.WhiteSpace != CssWhiteSpace.PreWrap || !run.IsSpaces))
                     || run.IsLineBreak || wrapNoWrapBox)
                {

                    wrapNoWrapBox = false; //once! 

                    //-------------------------------
                    //create new line ***
                    hostLine = new CssLineBox(hostBox);
                    hostBox.AddLineBox(hostLine);
                    //reset x pos for new line
                    cx = firstRunStartX;


                    // handle if line is wrapped for the first text element where parent has left margin/padding
                    if (childNumber == 0 && //b is first child of splitable box ('b' == splitableBox.GetFirstChild())
                        !run.IsLineBreak &&
                        (i == 0 || splitableParentIsBlock))//this run is first run of 'b' (run == b.FirstRun)
                    {
                        // var bParent = b.ParentBox;
                        cx += splitableBox.ActualMarginLeft + splitableBox.ActualBorderLeftWidth + splitableBox.ActualPaddingLeft;
                    }

                    if (run.IsImage || i == 0)
                    {
                        cx += leftMostSpace;
                    }
                }
                //---------------------------------------------------

                if (run.IsSpaces && hostLine.WordCount == 0)
                {
                    //not add 
                    continue;
                }
                else
                {
                    hostLine.AddRun(run); //***
                }

                run.SetLocation(cx, 0);
                //move current_line_x to right of run
                cx = run.Right;


                if (b.Position == CssPosition.Absolute)
                {
                    //var bParent = b.ParentBox;
                    run.Left += splitableBox.ActualMarginLeft;
                    run.Top += splitableBox.ActualMarginTop;
                }
            }
        }
        /// <summary>
        /// Adjust the position of absolute elements by letf and top margins.
        /// </summary>
        static void AdjustAbsolutePosition(CssBox box, float left, float top)
        {
            left += box.ActualMarginLeft;
            top += box.ActualMarginTop;
            if (box.HasRuns)
            {
                foreach (var word in box.GetRunIter())
                {
                    word.Left += left;
                    word.Top += top;
                }
            }
            else
            {
                foreach (var b in box.GetChildBoxIter())
                {
                    AdjustAbsolutePosition(b, left, top);
                }
            }
        }
        /// <summary>
        /// Applies vertical and horizontal alignment to words in lineboxes
        /// </summary>
        /// <param name="g"></param>
        /// <param name="lineBox"></param> 
        static void ApplyAlignment(CssLineBox lineBox, CssTextAlign textAlign, LayoutVisitor lay)
        {

            switch (textAlign)
            {
                case CssTextAlign.Right:
                    ApplyRightAlignment(lineBox);
                    break;
                case CssTextAlign.Center:
                    ApplyCenterAlignment(lineBox);
                    break;
                case CssTextAlign.Justify:
                    ApplyJustifyAlignment(lineBox);
                    break;
                default:
                    break;
            }
            //--------------------------------------------- 
            // Applies vertical alignment to the linebox 
            return;
            lineBox.ApplyBaseline(lineBox.CalculateTotalBoxBaseLine(lay));
            //---------------------------------------------  
        }

        /// <summary>
        /// Applies right to left direction to words
        /// </summary>
        /// <param name="blockBox"></param>
        /// <param name="lineBox"></param>
        static void ApplyRightToLeft(CssLineBox lineBox)
        {
            if (lineBox.WordCount > 0)
            {

                float left = lineBox.GetFirstRun().Left;
                float right = lineBox.GetLastRun().Right;
                foreach (CssRun run in lineBox.GetRunIter())
                {
                    float diff = run.Left - left;
                    float w_right = right - diff;
                    run.Left = w_right - run.Width;
                }
            }
        }

        ///// <summary>
        ///// Applies RTL direction to specific box words on the line.
        ///// </summary>
        ///// <param name="lineBox"></param>
        ///// <param name="box"></param>
        //private static void ApplyRightToLeftOnSingleBox(CssLineBox lineBox, CssBox box)
        //{
        //    int leftWordIdx = -1;
        //    int rightWordIdx = -1;

        //    if (lineBox.WordCount > 0)
        //    {
        //        int i = 0;
        //        foreach (var run in lineBox.GetRunIter())
        //        {
        //            if (run.OwnerBox == box)
        //            {
        //                if (leftWordIdx < 0)
        //                {
        //                    leftWordIdx = i;
        //                }
        //                rightWordIdx = i;
        //            }
        //            i++;
        //        }

        //    }

        //    if (leftWordIdx > -1 && rightWordIdx > leftWordIdx)
        //    {
        //        //line word of the same owner box 
        //        //alway stay sequentialy in the same linebox

        //        float left = lineBox.GetRun(leftWordIdx).Left; //lineWords[leftWordIdx].Left;
        //        float right = lineBox.GetRun(rightWordIdx).Right; //lineWords[rightWordIdx].Right;

        //        for (int i = leftWordIdx; i <= rightWordIdx; i++)
        //        {
        //            var moveWord = lineBox.GetRun(i);

        //            float diff = moveWord.Left - left;
        //            float new_right = right - diff;
        //            moveWord.Left = new_right - moveWord.Width;
        //        }
        //    }
        //}



        static void ApplyJustifyAlignment(CssLineBox lineBox)
        {


            if (lineBox.IsLastLine) return;

            float indent = lineBox.IsFirstLine ? lineBox.OwnerBox.ActualTextIndent : 0f;

            float runWidthSum = 0f;
            int runCount = 0;

            float availableWidth = lineBox.OwnerBox.ClientWidth - indent;

            // Gather text sum
            foreach (CssRun w in lineBox.GetRunIter())
            {
                runWidthSum += w.Width;
                runCount++;
            }

            if (runCount == 0) return; //Avoid Zero division

            float spaceOfEachRun = (availableWidth - runWidthSum) / runCount; //Spacing that will be used

            float cX = lineBox.OwnerBox.ClientLeft + indent;
            CssRun lastRun = lineBox.GetLastRun();
            foreach (CssRun run in lineBox.GetRunIter())
            {
                run.Left = cX;
                cX = run.Right + spaceOfEachRun;
                if (run == lastRun)
                {
                    run.Left = lineBox.OwnerBox.ClientRight - run.Width;
                }
            }
        }

        /// <summary>
        /// Applies centered alignment to the text on the linebox
        /// </summary>
        /// <param name="g"></param>
        /// <param name="line"></param>
        private static void ApplyCenterAlignment(CssLineBox line)
        {

            if (line.WordCount == 0) return;
            CssRun lastRun = line.GetLastRun();
            float diff = (line.OwnerBox.ClientWidth - lastRun.Right) / 2;
            if (diff > CSS_OFFSET_THRESHOLD)
            {
                foreach (CssRun word in line.GetRunIter())
                {
                    word.Left += diff;
                }
                line.CachedLineContentWidth += diff;
            }
        }

        /// <summary>
        /// Applies right alignment to the text on the linebox
        /// </summary>
        /// <param name="g"></param>
        /// <param name="line"></param>
        private static void ApplyRightAlignment(CssLineBox line)
        {
            if (line.WordCount == 0)
            {
                return;
            }
            CssRun lastRun = line.GetLastRun();
            float diff = line.OwnerBox.ClientWidth - line.GetLastRun().Right;
            if (diff > CSS_OFFSET_THRESHOLD)
            {
                foreach (CssRun word in line.GetRunIter())
                {
                    word.Left += diff;
                }
            }
        }

        /// <summary>
        /// Simplest alignment, just arrange words.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="line"></param>
        private static void ApplyLeftAlignment(IGraphics g, CssLineBox line)
        {
            //No alignment needed.

            //foreach (LineBoxRectangle r in line.Rectangles)
            //{
            //    float curx = r.Left + (r.Index == 0 ? r.OwnerBox.ActualPaddingLeft + r.OwnerBox.ActualBorderLeftWidth / 2 : 0);

            //    if (r.SpaceBefore) curx += r.OwnerBox.ActualWordSpacing;

            //    foreach (BoxWord word in r.Words)
            //    {
            //        word.Left = curx;
            //        word.Top = r.Top;// +r.OwnerBox.ActualPaddingTop + r.OwnerBox.ActualBorderTopWidth / 2;

            //        curx = word.Right + r.OwnerBox.ActualWordSpacing;
            //    }
            //}
        }


    }
}