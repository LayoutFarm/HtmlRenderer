//BSD 2014, WinterCore

using System;
using System.Collections.Generic;

namespace HtmlRenderer.Dom
{

    public class LayoutArgs
    {
        Stack<CssBox> containgBlockStack = new Stack<CssBox>();
        CssBox latestContaingBlock = null;


        internal LayoutArgs(IGraphics gfx)
        {
            this.Gfx = gfx;
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

        //-----------------------------------------
        internal CssBox LatestSiblingBox
        {
            get;
            set;
        }
    }
}