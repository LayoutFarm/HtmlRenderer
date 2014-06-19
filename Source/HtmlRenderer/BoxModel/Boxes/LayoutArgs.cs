//BSD 2014, WinterCore

using System;
using System.Collections.Generic;

namespace HtmlRenderer.Dom
{

    public class LayoutArgs
    {
        Stack<CssBox> containgBlockStack = new Stack<CssBox>();
        CssBox latestContaingBlock = null;

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
            this.containgBlockStack.Push(box);
            this.latestContaingBlock = box;
        }
        internal CssBox LatestContaingBlock
        {
            get { return this.latestContaingBlock; }
        }
        internal void PopContainingBlock()
        {
            this.containgBlockStack.Pop();
            if (this.containgBlockStack.Count > 0)
            {
                this.latestContaingBlock = this.containgBlockStack.Peek();
            }
            else
            {
                this.latestContaingBlock = null;
            }
        }

        internal void UpdateGlobalSize(CssBox box, float newCandidateWidth)
        {
            //need revisit
            //htmlContainer.UpdateSizeIfWiderOrHeigher(newWidthCandidate,
            //    (box.GlobalY + box.SizeHeight) - this.HtmlContainer.Root.GlobalY);

            float newCandidateHeight = box.SizeHeight;//
            htmlContainer.UpdateSizeIfWiderOrHeigher(newCandidateWidth, box.SizeHeight);
        }
        internal float GetLocalRightLimit(CssBox box)
        {   //need revisit
            return CssBox.MAX_RIGHT;
        }
        //-----------------------------------------
        internal CssBox LatestSiblingBox
        {
            get;
            set;
        }
    }
}