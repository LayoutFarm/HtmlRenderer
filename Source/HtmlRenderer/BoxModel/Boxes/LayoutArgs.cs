//BSD 2014, WinterCore

using System;
using System.Collections.Generic;

namespace HtmlRenderer.Dom
{

    public class LayoutArgs
    {
        Stack<CssBox> containgBlockStack = new Stack<CssBox>();
        CssBox latestContaingBlock = null;
        public LayoutArgs(IGraphics gfx)
        {
            this.Gfx = gfx;
        }
        public IGraphics Gfx
        {
            get;
            private set;
        }
        public void PushContaingBlock(CssBox box)
        {   
            this.containgBlockStack.Push(box);
            this.latestContaingBlock = box;
        }
        public CssBox LatestContaingBlock
        {
            get { return this.latestContaingBlock; }
        }
        public void PopContainingBlock()
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
        


    }
}