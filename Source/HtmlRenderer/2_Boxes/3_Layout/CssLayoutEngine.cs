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
        /// <param name="imageWord">the image word to measure</param>
        public static void MeasureImageSize(CssImageRun imageWord, LayoutVisitor lay)
        {
            var width = imageWord.OwnerBox.Width;
            var height = imageWord.OwnerBox.Height;

            bool hasImageTagWidth = width.Number > 0 && width.UnitOrNames == CssUnitOrNames.Pixels;
            bool hasImageTagHeight = height.Number > 0 && height.UnitOrNames == CssUnitOrNames.Pixels;
            bool scaleImageHeight = false;

            if (hasImageTagWidth)
            {
                imageWord.Width = width.Number;
            }
            else if (width.Number > 0 && width.IsPercentage)
            {

                imageWord.Width = width.Number * lay.LatestContainingBlock.SizeWidth;
                scaleImageHeight = true;
            }
            else if (imageWord.Image != null)
            {
                imageWord.Width = imageWord.ImageRectangle == Rectangle.Empty ? imageWord.Image.Width : imageWord.ImageRectangle.Width;
            }
            else
            {
                imageWord.Width = hasImageTagHeight ? height.Number / 1.14f : 20;
            }

            var maxWidth = imageWord.OwnerBox.MaxWidth;// new CssLength(imageWord.OwnerBox.MaxWidth);
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


                if (maxWidthVal > -1 && imageWord.Width > maxWidthVal)
                {
                    imageWord.Width = maxWidthVal;
                    scaleImageHeight = !hasImageTagHeight;
                }
            }

            if (hasImageTagHeight)
            {
                imageWord.Height = height.Number;
            }
            else if (imageWord.Image != null)
            {
                imageWord.Height = imageWord.ImageRectangle == Rectangle.Empty ? imageWord.Image.Height : imageWord.ImageRectangle.Height;
            }
            else
            {
                imageWord.Height = imageWord.Width > 0 ? imageWord.Width * 1.14f : 22.8f;
            }

            if (imageWord.Image != null)
            {
                // If only the width was set in the html tag, ratio the height.
                if ((hasImageTagWidth && !hasImageTagHeight) || scaleImageHeight)
                {
                    // Divide the given tag width with the actual image width, to get the ratio.
                    float ratio = imageWord.Width / imageWord.Image.Width;
                    imageWord.Height = imageWord.Image.Height * ratio;
                }
                // If only the height was set in the html tag, ratio the width.
                else if (hasImageTagHeight && !hasImageTagWidth)
                {
                    // Divide the given tag height with the actual image height, to get the ratio.
                    float ratio = imageWord.Height / imageWord.Image.Height;
                    imageWord.Width = imageWord.Image.Width * ratio;
                }
            }
            //imageWord.Height += imageWord.OwnerBox.ActualBorderBottomWidth + imageWord.OwnerBox.ActualBorderTopWidth + imageWord.OwnerBox.ActualPaddingTop + imageWord.OwnerBox.ActualPaddingBottom;
        }
        public static void FlowInlinesContent(CssBox hostBlock, LayoutVisitor lay)
        {

            //*** hostBlock must confirm that it has all inline children             

            hostBlock.ResetLineBoxes();

            float limitLocalRight = hostBlock.SizeWidth - (hostBlock.ActualPaddingRight + hostBlock.ActualBorderRightWidth);

            float localY = hostBlock.ActualPaddingTop + hostBlock.ActualBorderTopWidth;
            float localX = hostBlock.ActualTextIndent + hostBlock.ActualPaddingLeft + hostBlock.ActualBorderLeftWidth;
            float enterLocalY = localY;

            float startLocalX = localX;
            //Reminds the maximum bottom reached
            float maxLocalRight = localX;
            float maxLocalBottom = localY;

            //First line box
            {
                CssLineBox line = new CssLineBox(hostBlock);
                hostBlock.AddLineBox(line);
                //****
                FlowBoxContentIntoHost(lay, hostBlock, hostBlock, limitLocalRight, 0, startLocalX,
                      ref line, ref localX, ref localY, ref maxLocalRight, ref maxLocalBottom);
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
            if (hostBlock.CssDirection == CssDirection.Rtl)
            {
                float cy = enterLocalY;
                foreach (CssLineBox linebox in hostBlock.GetLineBoxIter())
                {
                    ApplyAlignment(linebox, lay);
                    ApplyRightToLeft(hostBlock, linebox); //***
                    linebox.CloseLine(lay); //*** 

                    linebox.CachedLineTop = cy;
                    cy += linebox.CacheLineHeight; // + interline space?
                }
            }
            else
            {
                float cy = enterLocalY;
                foreach (CssLineBox linebox in hostBlock.GetLineBoxIter())
                {
                    ApplyAlignment(linebox, lay);
                    linebox.CloseLine(lay); //***
                    linebox.CachedLineTop = cy;
                    cy += linebox.CacheLineHeight;
                }
            }


            hostBlock.SetHeight(maxLocalBottom + hostBlock.ActualPaddingBottom + hostBlock.ActualBorderBottomWidth);
            // handle limiting block height when overflow is hidden             
            if (hostBlock.Overflow == CssOverflow.Hidden &&
                 !hostBlock.Height.IsEmptyOrAuto &&
                 hostBlock.SizeHeight > hostBlock.ExpectedHeight)
            {

                hostBlock.UseExpectedHeight();
            }
        }

        #region Private methods


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
          float interLineSpace,
          float firstRunStartX,
          ref CssLineBox hostLine,
          ref float current_line_x,
          ref float current_line_y,
          ref float maxRightForHostBox,
          ref float maxBottomForHostBox)
        {


            var oX = current_line_x;
            var oY = current_line_y;

            var localMaxRight = maxRightForHostBox;
            var localMaxBottom = maxBottomForHostBox;

            float splitBoxActualLineHeight = splitableBox.ActualLineHeight;
            bool splitableParentIsBlock = splitableBox.ParentBox.IsBlock;

            int childNumber = 0;


            if (splitableBox.MayHasSomeTextContent)
            {
                float leftMostSpace = 0, rightMostSpace = 0;
                FlowRunsIntoHost(lay, hostBox, splitableBox, splitableBox, limitLocalRight, interLineSpace, firstRunStartX,
                     ref hostLine, ref current_line_x, ref current_line_y,
                     ref maxRightForHostBox, ref maxBottomForHostBox,
                     childNumber, leftMostSpace, rightMostSpace, splitableParentIsBlock, splitBoxActualLineHeight);
            }
            else
            {
                float leftMostSpace = 0, rightMostSpace = 0;
                int totalChildCount = splitableBox.ChildCount;

                foreach (CssBox b in splitableBox.GetChildBoxIter())
                {
                    if (b.IsAbsolutePosition())
                    {
                        leftMostSpace = b.ActualMarginLeft + b.ActualBorderLeftWidth + b.ActualPaddingLeft;
                        rightMostSpace = b.ActualMarginRight + b.ActualBorderRightWidth + b.ActualPaddingRight;
                    }

                    if (b.NeedComputedValueEvaluation)
                    {
                        b.ReEvaluateComputedValues(hostBox);
                    }
                    b.MeasureRunsSize(lay);

                    current_line_x += leftMostSpace;

                    if (b.CssDisplay == CssDisplay.BlockInsideInlineAfterCorrection)
                    {

                        //-----------------------------------------
                        lay.PushContaingBlock(hostBox);
                        var currentLevelLatestSibling = lay.LatestSiblingBox;
                        lay.LatestSiblingBox = null;//reset

                        b.PerformLayout(lay);

                        lay.LatestSiblingBox = currentLevelLatestSibling;
                        lay.PopContainingBlock();
                        //-----------------------------------------


                        var newline = new CssLineBox(hostBox);
                        hostBox.AddLineBox(newline);
                        //reset x pos for new line
                        current_line_x = firstRunStartX;
                        //set y to new line      
                        newline.CachedLineTop = current_line_y = maxBottomForHostBox + interLineSpace;

                        CssBlockRun blockRun = new CssBlockRun(b);
                        newline.AddRun(blockRun);

                        blockRun.SetLocation(firstRunStartX, 0);
                        blockRun.SetSize(b.SizeWidth, b.SizeHeight);

                        maxBottomForHostBox += b.SizeHeight;
                        //-----------------------------------------
                        if (childNumber < totalChildCount)
                        {
                            //this is not last child
                            //create new line 
                            newline = new CssLineBox(hostBox);
                            hostBox.AddLineBox(newline);
                            newline.CachedLineTop = current_line_y = maxBottomForHostBox + interLineSpace;
                        }

                        hostLine = newline;
                        continue;
                    }

                    if (!b.HasRuns)
                    {
                        //go deeper  
                        FlowBoxContentIntoHost(lay, hostBox, b, limitLocalRight, interLineSpace, firstRunStartX,
                            ref hostLine, ref current_line_x, ref current_line_y,
                            ref maxRightForHostBox, ref maxBottomForHostBox);
                    }
                    else
                    {
                        FlowRunsIntoHost(lay, hostBox, splitableBox, b, limitLocalRight, interLineSpace, firstRunStartX,
                            ref hostLine, ref current_line_x, ref current_line_y,
                            ref maxRightForHostBox, ref maxBottomForHostBox,
                            childNumber, leftMostSpace, rightMostSpace, splitableParentIsBlock, splitBoxActualLineHeight);
                    }

                    current_line_x += rightMostSpace;
                    childNumber++;
                }
            }
            //------------
            if (oY + splitableBox.ExpectedHeight > maxBottomForHostBox)
            {
                maxBottomForHostBox = oY + splitableBox.ExpectedHeight;
            }
            //------------ 
            // handle width setting
            if (splitableBox.IsInline &&
                0 <= current_line_x - oX && current_line_x - oX < splitableBox.ExpectedWidth)
            {
                throw new NotSupportedException();
            }

            // hack to support specific absolute position elements 
            if (splitableBox.IsAbsolutePosition())
            {
                current_line_x = oX;

                AdjustAbsolutePosition(splitableBox, 0, 0);
            }
        }

        static void FlowRunsIntoHost(LayoutVisitor lay,
          CssBox hostBox,
          CssBox bParent,
          CssBox b,
          float limitLocalRight, float interLineSpace, float firstRunStartX,
          ref CssLineBox hostLine,
          ref float current_line_x, ref float current_line_y,
          ref float maxRightForHostBox,
          ref float maxBottomForHostBox,
          int childNumber,
          float leftMostSpace,
          float rightMostSpace,
          bool splitableParentIsBlock,
          float splitBoxActualLineHeight)
        {
            //flow runs into hostLine, create new line if need  
            List<CssRun> runs = CssBox.UnsafeGetRunList(b);
            bool wrapNoWrapBox = false;
            //-----------------------------------------------------
            if (b.WhiteSpace == CssWhiteSpace.NoWrap && current_line_x > firstRunStartX)
            {
                var tmpRight = current_line_x;
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

            for (int i = 0; i < j; ++i)
            {
                var run = runs[i];

                if (current_line_y + splitBoxActualLineHeight > maxBottomForHostBox)
                {
                    maxBottomForHostBox = current_line_y + splitBoxActualLineHeight;
                }

                //---------------------------------------------------
                //check if need to start new line ?

                if ((current_line_x + run.Width + rightMostSpace > limitLocalRight &&
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
                    current_line_x = firstRunStartX;
                    //set y to new line                            
                    hostLine.CachedLineTop = current_line_y = maxBottomForHostBox + interLineSpace;

                    // handle if line is wrapped for the first text element where parent has left margin/padding
                    if (childNumber == 0 && //b is first child of splitable box ('b' == splitableBox.GetFirstChild())
                        !run.IsLineBreak &&
                        (i == 0 || splitableParentIsBlock))//this run is first run of 'b' (run == b.FirstRun)
                    {
                        // var bParent = b.ParentBox;
                        current_line_x += bParent.ActualMarginLeft + bParent.ActualBorderLeftWidth + bParent.ActualPaddingLeft;
                    }

                    if (run.IsImage || i == 0)
                    {
                        current_line_x += leftMostSpace;
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

                run.SetLocation(current_line_x, 0);
                //move current_line_x to right of run
                current_line_x = run.Right;

                maxRightForHostBox = Math.Max(maxRightForHostBox, current_line_x);
                maxBottomForHostBox = Math.Max(maxBottomForHostBox, current_line_y + run.Bottom);

                if (b.IsAbsolutePosition())
                {
                    //var bParent = b.ParentBox;
                    run.Left += bParent.ActualMarginLeft;
                    run.Top += bParent.ActualMarginTop;
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
        private static void ApplyAlignment(CssLineBox lineBox, LayoutVisitor lay)
        {
            switch (lineBox.OwnerBox.CssTextAlign)
            {
                case CssTextAlign.Right:
                    ApplyRightAlignment(lay.Gfx, lineBox);
                    break;
                case CssTextAlign.Center:
                    ApplyCenterAlignment(lay.Gfx, lineBox);
                    break;
                case CssTextAlign.Justify:
                    ApplyJustifyAlignment(lay.Gfx, lineBox);
                    break;
                default:
                    //ApplyLeftAlignment(g, lineBox);
                    break;
            }
            //--------------------------------------------- 
            // Applies vertical alignment to the linebox             

            lineBox.ApplyBaseline(lineBox.CalculateTotalBoxBaseLine());

            //--------------------------------------------- 

        }

        /// <summary>
        /// Applies right to left direction to words
        /// </summary>
        /// <param name="blockBox"></param>
        /// <param name="lineBox"></param>
        static void ApplyRightToLeft(CssBox blockBox, CssLineBox lineBox)
        {

            if (blockBox.CssDirection == CssDirection.Rtl)
            {
                //ApplyRightToLeftOnLine:
                //apply RLT to all word in this lineBox  
                //move all linebox content 
                if (lineBox.WordCount > 0)
                {
                    //move only word ?
                    //what about bg strip ?
                    float left = lineBox.GetFirstRun().Left;
                    float right = lineBox.GetLastRun().Right;

                    foreach (CssRun run in lineBox.GetRunIter())
                    {
                        float diff = run.Left - left;
                        float wright = right - diff;
                        run.Left = wright - run.Width;
                    }
                }
            }
            else
            {
                //apply RTL to some child box
                //foreach (var box in lineBox.RelatedBoxes)
                //{
                //    // if (box.Direction == CssConstants.Rtl)
                //    if (box.CssDirection == CssDirection.Rtl)
                //    {
                //        ApplyRightToLeftOnSingleBox(lineBox, box);
                //    }
                //}
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



        static void ApplyJustifyAlignment(IGraphics g, CssLineBox lineBox)
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
        private static void ApplyCenterAlignment(IGraphics g, CssLineBox line)
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
        private static void ApplyRightAlignment(IGraphics g, CssLineBox line)
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

        #endregion
    }
}