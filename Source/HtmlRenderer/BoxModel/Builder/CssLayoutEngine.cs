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
using HtmlRenderer.Entities;
using HtmlRenderer.Utils;

namespace HtmlRenderer.Dom
{
    /// <summary>
    /// Helps on CSS Layout.
    /// </summary>
    internal static class CssLayoutEngine
    {


        /// <summary>
        /// Measure image box size by the width\height set on the box and the actual rendered image size.<br/>
        /// If no image exists for the box error icon will be set.
        /// </summary>
        /// <param name="imageWord">the image word to measure</param>
        public static void MeasureImageSize(CssImageRun imageWord)
        {
            ArgChecker.AssertArgNotNull(imageWord, "imageWord");
            ArgChecker.AssertArgNotNull(imageWord.OwnerBox, "imageWord.OwnerBox");

            var width = imageWord.OwnerBox.Width; // new CssLength(imageWord.OwnerBox.Width);
            var height = imageWord.OwnerBox.Height;// new CssLength(imageWord.OwnerBox.Height);

            bool hasImageTagWidth = width.Number > 0 && width.Unit == CssUnit.Pixels;
            bool hasImageTagHeight = height.Number > 0 && height.Unit == CssUnit.Pixels;
            bool scaleImageHeight = false;

            if (hasImageTagWidth)
            {
                imageWord.Width = width.Number;
            }
            else if (width.Number > 0 && width.IsPercentage)
            {
                imageWord.Width = width.Number * imageWord.OwnerBox.ContainingBlock.Size.Width;
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
                switch (maxWidth.Unit)
                {
                    case CssUnit.Percent:
                        {
                            maxWidthVal = maxWidth.Number * imageWord.OwnerBox.ContainingBlock.SizeWidth;
                        } break;
                    case CssUnit.Pixels:
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

            imageWord.Height += imageWord.OwnerBox.ActualBorderBottomWidth + imageWord.OwnerBox.ActualBorderTopWidth + imageWord.OwnerBox.ActualPaddingTop + imageWord.OwnerBox.ActualPaddingBottom;
        }

        /// <summary>
        /// Creates line boxes for the specified blockbox
        /// </summary>
        /// <param name="g"></param>
        /// <param name="blockBox"></param>
        public static void CreateLineBoxes(IGraphics g, CssBox blockBox)
        {
            ArgChecker.AssertArgNotNull(g, "g");
            ArgChecker.AssertArgNotNull(blockBox, "blockBox");


            blockBox.ResetLineBoxes();//blockBox.LineBoxes.Clear();

            float limitRight = blockBox.ActualRight - blockBox.ActualPaddingRight - blockBox.ActualBorderRightWidth;

            //Get the start x and y of the blockBox
            float startx = blockBox.LocationX + blockBox.ActualPaddingLeft - 0 + blockBox.ActualBorderLeftWidth;
            float starty = blockBox.LocationY + blockBox.ActualPaddingTop - 0 + blockBox.ActualBorderTopWidth;
            float curx = startx + blockBox.ActualTextIndent;
            float cury = starty;

            //Reminds the maximum bottom reached
            float maxRight = startx;
            float maxBottom = starty;
            //float lineSpacing = 0f;

            //First line box
            CssLineBox line = new CssLineBox(blockBox);

            //Flow words and boxes
            FlowBox(g, blockBox, blockBox, limitRight, 0, startx, ref line, ref curx, ref cury, ref maxRight, ref maxBottom);

            // if width is not restricted we need to lower it to the actual width
            if (blockBox.ActualRight >= CssBox.MAX_RIGHT)
            {
                blockBox.ActualRight = maxRight + blockBox.ActualPaddingRight + blockBox.ActualBorderRightWidth;
            } 

            foreach (CssLineBox linebox in blockBox.GetLineBoxIter())
            {

                //Gets the rectangles for each line-box
                ApplyAlignment(g, linebox);

                ApplyRightToLeft(blockBox, linebox);
                BubbleStripsUpdate(blockBox, linebox);
                //***
                linebox.CloseLine();
            }

            blockBox.ActualBottom = maxBottom + blockBox.ActualPaddingBottom + blockBox.ActualBorderBottomWidth;

            // handle limiting block height when overflow is hidden
            //if (blockBox.Height != null && blockBox.Height != CssConstants.Auto && 
            if (!blockBox.Height.IsEmpty && !blockBox.Height.IsAuto &&
                blockBox.Overflow == CssOverflow.Hidden &&
                blockBox.ActualBottom - blockBox.LocationY > blockBox.ActualHeight)
            {
                blockBox.ActualBottom = blockBox.LocationY + blockBox.ActualHeight;
            }
        }

        /// <summary>
        /// Applies special vertical alignment for table-cells
        /// </summary>
        /// <param name="g"></param>
        /// <param name="cell"></param>
        public static void ApplyCellVerticalAlignment(IGraphics g, CssBox cell)
        {
            ArgChecker.AssertArgNotNull(g, "g");
            ArgChecker.AssertArgNotNull(cell, "cell"); 
             
            float dist = 0f;
            switch (cell.VerticalAlign)
            {
                case CssVerticalAlign.Bottom:
                    dist = cell.ClientBottom - cell.CalculateMaximumBottom(cell, 0f);
                    break;
                case CssVerticalAlign.Middle:
                    dist = (cell.ClientBottom - cell.CalculateMaximumBottom(cell, 0f)) / 2;
                    break;
                default:
                    return; 
            } 

            foreach (CssBox b in cell.GetChildBoxIter())
            {
                b.OffsetTop(dist);
            }

        }


        #region Private methods

        /// <summary>
        /// Recursively flows the content of the box using the inline model
        /// </summary>
        /// <param name="g">Device Info</param>
        /// <param name="hostBox">Blockbox that contains the text flow</param>
        /// <param name="splitableBox">Current box to flow its content</param>
        /// <param name="limitRight">Maximum reached right</param>
        /// <param name="linespacing">Space to use between rows of text</param>
        /// <param name="startx">x starting coordinate for when breaking lines of text</param>
        /// <param name="hostLine">Current linebox being used</param>
        /// <param name="cx">Current x coordinate that will be the left of the next word</param>
        /// <param name="cy">Current y coordinate that will be the top of the next word</param>
        /// <param name="maxRight">Maximum right reached so far</param>
        /// <param name="maxbottom">Maximum bottom reached so far</param>
        static void FlowBox(IGraphics g,
          CssBox hostBox, CssBox splitableBox,
          float limitRight, float linespacing, float startx,
          ref CssLineBox hostLine, ref float cx, ref float cy, ref float maxRight, ref float maxbottom)
        {
            var startX = cx;
            var startY = cy;
            var localCurx = cx;
            var localMaxRight = maxRight;
            var localmaxbottom = maxbottom;

            splitableBox.FirstHostingLineBox = hostLine;

            foreach (CssBox b in splitableBox.GetChildBoxIter())
            {

                float leftspacing = !b.IsAbsolutePosition ? b.ActualMarginLeft + b.ActualBorderLeftWidth + b.ActualPaddingLeft : 0;
                float rightspacing = !b.IsAbsolutePosition ? b.ActualMarginRight + b.ActualBorderRightWidth + b.ActualPaddingRight : 0;

                b.ResetSummaryBound();
                b.MeasureRunsSize(g);

                cx += leftspacing;

                if (b.HasRuns)
                {

                    bool wrapNoWrapBox = false;
                    List<CssRun> runs = CssBox.UnsafeGetRunListOrCreateIfNotExists(b);
                    if (b.WhiteSpace == CssWhiteSpace.NoWrap && cx > startx)
                    {
                        var boxRight = cx;
                        foreach (CssRun word in runs)
                        {
                            boxRight += word.Width;
                        }
                        if (boxRight > limitRight)
                        {
                            wrapNoWrapBox = true;
                        }
                    }
                    //-----------------------------------------------------
                    foreach (var run in runs)
                    {
                        if (maxbottom - cy < splitableBox.ActualLineHeight)
                        {
                            maxbottom += splitableBox.ActualLineHeight - (maxbottom - cy);
                        }

                        if ((b.WhiteSpace != CssWhiteSpace.NoWrap &&
                             b.WhiteSpace != CssWhiteSpace.Pre &&
                            cx + run.Width + rightspacing > limitRight &&
                            (b.WhiteSpace != CssWhiteSpace.PreWrap || !run.IsSpaces))
                            || run.IsLineBreak || wrapNoWrapBox)
                        {
                            wrapNoWrapBox = false;
                            cx = startx;

                            // handle if line is wrapped for the first text element where parent has left margin/padding
                            if (b == splitableBox.GetFirstChild() && !run.IsLineBreak && (run == b.FirstRun ||
                                (splitableBox.ParentBox != null && splitableBox.ParentBox.IsBlock)))
                            {
                                cx += splitableBox.ActualMarginLeft + splitableBox.ActualBorderLeftWidth + splitableBox.ActualPaddingLeft;
                            }

                            cy = maxbottom + linespacing;

                            hostLine = new CssLineBox(hostBox);

                            if (run.IsImage || run.Equals(b.FirstRun))
                            {
                                cx += leftspacing;
                            }
                        }

                        if (hostLine.WordCount == 0)
                        {
                            //first run of hostLine
                            if (run.IsSpaces)
                            {
                                //not add 
                                continue;
                            }
                            else
                            {
                                hostLine.AddRun(run);
                            }
                        }
                        else
                        {
                            hostLine.AddRun(run);
                        }
                        run.Left = cx;
                        run.Top = cy;

                        cx = run.Left + run.Width;

                        maxRight = Math.Max(maxRight, run.Right);
                        maxbottom = Math.Max(maxbottom, run.Bottom);


                        if (b.IsAbsolutePosition)
                        {
                            run.Left += splitableBox.ActualMarginLeft;
                            run.Top += splitableBox.ActualMarginTop;
                        }
                    }
                }
                else
                {
                    FlowBox(g, hostBox, b, limitRight, linespacing, startx, ref hostLine, ref cx, ref cy, ref maxRight, ref maxbottom);
                }
                cx += rightspacing;
            }

            // handle height setting
            if (maxbottom - startY < splitableBox.ActualHeight)
            {
                maxbottom += splitableBox.ActualHeight - (maxbottom - startY);
            }

            // handle width setting
            if (splitableBox.IsInline && 0 <= cx - startX && cx - startX < splitableBox.ActualWidth)
            {
                throw new NotSupportedException();
                //// hack for actual width handling
                //cx += splitableBox.ActualWidth - (cx - startX);
                ////add new one
                //hostLine.AddStripInfo(splitableBox, startX, startY, splitableBox.ActualWidth, splitableBox.ActualHeight);
            }
            // handle box that is only a whitespace
            if (splitableBox.MayHasSomeTextContent &&
                splitableBox.TextContentIsAllWhitespace &&
                !splitableBox.IsImage &&
                splitableBox.IsInline &&
                splitableBox.ChildCount == 0 &&
                splitableBox.RunCount == 0)
            {
                cx += splitableBox.ActualWordSpacing;
            }

            // hack to support specific absolute position elements 
            if (splitableBox.IsAbsolutePosition)
            {
                cx = localCurx;
                maxRight = localMaxRight;
                maxbottom = localmaxbottom;

                AdjustAbsolutePosition(splitableBox, 0, 0);
            }

            splitableBox.LastHostingLineBox = hostLine;
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
        /// Recursively creates the rectangles of the blockBox, by bubbling from deep to outside of the boxes 
        /// in the rectangle structure
        /// </summary>         
        static void BubbleStripsUpdate(CssBox box, CssLineBox hostLine)
        {
            if (box.HasRuns)
            {
                float x = Single.MaxValue, y = Single.MaxValue, r = Single.MinValue, b = Single.MinValue;
                bool foundSomeRun = false;

                //select only run in this of this box in this line 
                bool boxIsFirstChild = box == box.ParentBox.GetFirstChild();
                CssRun firstBoxRun = box.FirstRun;
                bool hostLineIsNotFirstLine = hostLine != hostLine.OwnerBox.GetFirstLineBox();

                foreach (CssRun run in hostLine.GetRunIter(box))
                {
                    //summary all runs of box in hostLine
                    //one line may have word from more than one box      
                    foundSomeRun = true;
                    // handle if line is wrapped for the first text element where parent has left margin\padding
                    var left = run.Left;
                    if (boxIsFirstChild && //1. box is first child 
                        run == firstBoxRun && //2. this run is first run of box
                        run == hostLine.GetFirstRun() && //3. this run is first run of line
                        hostLineIsNotFirstLine &&
                        !run.IsLineBreak)
                    {
                        left -= box.ParentBox.ActualMarginLeft + box.ParentBox.ActualBorderLeftWidth + box.ParentBox.ActualPaddingLeft;
                    }
                    x = Math.Min(x, left);
                    r = Math.Max(r, run.Right);
                    y = Math.Min(y, run.Top);
                    b = Math.Max(b, run.Bottom);
                }
                //------------------

                if (foundSomeRun)
                {
                    hostLine.BubbleStripUpdate(box, x, y, r, b);
                }
            }
            else
            {
                //recursive ****
                foreach (CssBox b in box.GetChildBoxIter())
                {
                    BubbleStripsUpdate(b, hostLine);
                }
            }
        }

        /// <summary>
        /// Applies vertical and horizontal alignment to words in lineboxes
        /// </summary>
        /// <param name="g"></param>
        /// <param name="lineBox"></param>
        private static void ApplyAlignment(IGraphics g, CssLineBox lineBox)
        {
            switch (lineBox.OwnerBox.CssTextAlign)
            {
                case CssTextAlign.Right:
                    ApplyRightAlignment(g, lineBox);
                    break;
                case CssTextAlign.Center:
                    ApplyCenterAlignment(g, lineBox);
                    break;
                case CssTextAlign.Justify:
                    ApplyJustifyAlignment(g, lineBox);
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
        private static void ApplyRightToLeft(CssBox blockBox, CssLineBox lineBox)
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

        /// <summary>
        /// Applies RTL direction to specific box words on the line.
        /// </summary>
        /// <param name="lineBox"></param>
        /// <param name="box"></param>
        private static void ApplyRightToLeftOnSingleBox(CssLineBox lineBox, CssBox box)
        {
            int leftWordIdx = -1;
            int rightWordIdx = -1;

            if (lineBox.WordCount > 0)
            {
                int i = 0;
                foreach (var run in lineBox.GetRunIter())
                {
                    if (run.OwnerBox == box)
                    {
                        if (leftWordIdx < 0)
                        {
                            leftWordIdx = i;
                        }
                        rightWordIdx = i;
                    }
                    i++;
                }

            }

            if (leftWordIdx > -1 && rightWordIdx > leftWordIdx)
            {
                //line word of the same owner box 
                //alway stay sequentialy in the same linebox

                float left = lineBox.GetRun(leftWordIdx).Left; //lineWords[leftWordIdx].Left;
                float right = lineBox.GetRun(rightWordIdx).Right; //lineWords[rightWordIdx].Right;

                for (int i = leftWordIdx; i <= rightWordIdx; i++)
                {
                    var moveWord = lineBox.GetRun(i);

                    float diff = moveWord.Left - left;
                    float new_right = right - diff;
                    moveWord.Left = new_right - moveWord.Width;
                }
            }
        }


        /// <summary>
        /// Applies centered alignment to the text on the linebox
        /// </summary>
        /// <param name="g"></param>
        /// <param name="lineBox"></param>
        private static void ApplyJustifyAlignment(IGraphics g, CssLineBox lineBox)
        {

            if (lineBox.Equals(lineBox.OwnerBox.GetLastLineBox())) return;


            float indent = lineBox.Equals(lineBox.OwnerBox.GetFirstLineBox()) ? lineBox.OwnerBox.ActualTextIndent : 0f;
            float textSum = 0f;
            float words = 0f;
            float availWidth = lineBox.OwnerBox.ClientRectangle.Width - indent;

            // Gather text sum
            foreach (CssRun w in lineBox.GetRunIter())
            {
                textSum += w.Width;
                words += 1f;
            }

            if (words <= 0f) return; //Avoid Zero division
            float spacing = (availWidth - textSum) / words; //Spacing that will be used
            float curx = lineBox.OwnerBox.ClientLeft + indent;

            CssRun lastRun = lineBox.GetLastRun();
            foreach (CssRun run in lineBox.GetRunIter())
            {
                run.Left = curx;
                curx = run.Right + spacing;
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
            float right = line.OwnerBox.ActualRight - line.OwnerBox.ActualPaddingRight - line.OwnerBox.ActualBorderRightWidth;
            float diff = right - lastRun.Right - lastRun.OwnerBox.ActualBorderRightWidth - lastRun.OwnerBox.ActualPaddingRight;
            diff /= 2;

            if (diff > 0)
            {
                foreach (CssRun word in line.GetRunIter())
                {
                    word.Left += diff;
                }

                ////foreach (CssBox b in line.Rectangles.Keys)
                //foreach (CssBox b in line.GetBoxIter())
                //{    //update data back from this line to box
                //    RectangleF r = b.Rectangles[line];
                //    b.Rectangles[line] = new RectangleF(r.X + diff, r.Y, r.Width, r.Height);
                //}
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

            CssRun lastWord = line.GetLastRun();
            float right = line.OwnerBox.ActualRight - line.OwnerBox.ActualPaddingRight - line.OwnerBox.ActualBorderRightWidth;
            float diff = right - lastWord.Right - lastWord.OwnerBox.ActualBorderRightWidth - lastWord.OwnerBox.ActualPaddingRight;

            if (diff > 0)
            {
                foreach (CssRun word in line.GetRunIter())
                {
                    word.Left += diff;
                }


                ////foreach (CssBox b in line.Rectangles.Keys)
                //foreach (CssBox b in line.GetBoxIter())
                //{
                //    //update data back from this line to box
                //    RectangleF r = b.Rectangles[line];
                //    b.Rectangles[line] = new RectangleF(r.X + diff, r.Y, r.Width, r.Height);
                //}
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