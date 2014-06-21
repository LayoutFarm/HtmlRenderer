//BSD 2014, WinterCore

using System;
using System.Collections.Generic;

namespace HtmlRenderer.Dom
{

    public class LayoutArgs
    {
        Stack<CssBox> containgBlockStack = new Stack<CssBox>();
        CssBox latestContaingBlock = null;

        float globalXOffset;
        float globalYOffset;
        float totalMarginLeftAndRight;


        HtmlContainer htmlContainer;
        internal LayoutArgs(IGraphics gfx, HtmlContainer htmlContainer)
        {
            this.Gfx = gfx;
            this.htmlContainer = htmlContainer;
        }

        internal IGraphics Gfx
        {
            get;
            private set;
        }
        internal void PushContaingBlock(CssBox box)
        {
            if (box != latestContaingBlock)
            {
                this.globalXOffset += box.LocalX;
                this.globalYOffset += box.LocalY;
                this.totalMarginLeftAndRight += (box.ActualMarginLeft + box.ActualMarginRight);
            }

            this.containgBlockStack.Push(box);
            this.latestContaingBlock = box;

        }
        internal CssBox LatestContaingBlock
        {
            get { return this.latestContaingBlock; }
        }
        internal void PopContainingBlock()
        {
            switch (this.containgBlockStack.Count)
            {
                case 0:
                    {
                    } break;
                case 1:
                    {
                        var box = this.containgBlockStack.Pop();
                        if (this.latestContaingBlock != box)
                        {
                            this.globalXOffset -= box.LocalX;
                            this.globalYOffset -= box.LocalY;
                            this.totalMarginLeftAndRight -= (box.ActualMarginLeft + box.ActualMarginRight);
                        }
                        this.latestContaingBlock = null;
                    } break;
                default:
                    {
                        var box = this.containgBlockStack.Pop();
                        if (this.latestContaingBlock != box)
                        {
                            this.globalXOffset -= box.LocalX;
                            this.globalYOffset -= box.LocalY;
                            this.totalMarginLeftAndRight -= (box.ActualMarginLeft + box.ActualMarginRight);
                        }
                        this.latestContaingBlock = this.containgBlockStack.Peek();

                    } break;
            }

        }
        internal float ContainerBlockGlobalX
        {
            get { return this.globalXOffset; }
        }
        internal float ContainerBlockGlobalY
        {
            get { return this.globalYOffset; }
        }
        //-----------------------------------------
        internal CssBox LatestSiblingBox
        {
            get;
            set;
        }
        internal void UpdateRootSize(CssBox box)
        {
            float candidateRootWidth = Math.Max(box.CalculateMinimumWidth() + CalculateWidthMarginTotalUp(box),
                         (box.SizeWidth + this.ContainerBlockGlobalX) < CssBoxConst.MAX_RIGHT ? box.SizeWidth : 0);

            this.htmlContainer.UpdateSizeIfWiderOrHeigher(
                this.ContainerBlockGlobalX + candidateRootWidth,
                this.ContainerBlockGlobalY + box.SizeHeight);
        }
        /// <summary>
        /// Get the total margin value (left and right) from the given box to the given end box.<br/>
        /// </summary>
        /// <param name="box">the box to start calculation from.</param>
        /// <returns>the total margin</returns>
        float CalculateWidthMarginTotalUp(CssBox box)
        {

            if ((box.SizeWidth + this.ContainerBlockGlobalX) > CssBoxConst.MAX_RIGHT ||
                (box.ParentBox != null && (box.ParentBox.SizeWidth + this.ContainerBlockGlobalX) > CssBoxConst.MAX_RIGHT))
            {
                return (box.ActualMarginLeft + box.ActualMarginRight) + totalMarginLeftAndRight;
            }
            return 0;
        }

    }


}