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
        internal void UpdateRootSize(float localBoxWidth, float localBoxHeight)
        {
            this.htmlContainer.UpdateSizeIfWiderOrHeigher(this.ContainerBlockGlobalX + localBoxWidth, this.ContainerBlockGlobalY + localBoxHeight);
        }
    }
}