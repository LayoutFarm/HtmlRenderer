//BSD 2014, WinterDev

using System;
using System.Collections.Generic;

namespace HtmlRenderer.RenderDom
{
    public abstract class BoxVisitor
    {
        Stack<CssBox> containgBlockStack = new Stack<CssBox>();
        CssBox latestContaingBlock = null;

        float globalXOffset;
        float globalYOffset;
        internal void PushContaingBlock(CssBox box)
        {
            if (box != latestContaingBlock)
            {
                this.globalXOffset += box.LocalX;
                this.globalYOffset += box.LocalY;
                OnPushDifferentContaingBlock(box);
            }
            this.containgBlockStack.Push(box);
            this.latestContaingBlock = box;
        }
        protected virtual void OnPushDifferentContaingBlock(CssBox box)
        {
        }
        protected virtual void OnPopDifferentContaingBlock(CssBox box)
        {
        }
        internal CssBox LatestContainingBlock
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
                            OnPopDifferentContaingBlock(box);
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
                            OnPopDifferentContaingBlock(box);
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
    }

}