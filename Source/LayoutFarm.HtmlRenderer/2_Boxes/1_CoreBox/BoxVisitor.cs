//BSD, 2014-present, WinterDev 

using System.Collections.Generic;
namespace LayoutFarm.HtmlBoxes
{
    public abstract class BoxVisitor
    {
        Stack<CssBox> _containgBlockStack = new Stack<CssBox>();
        CssBox _latestContainingBlock = null;
        float _globalXOffset;
        float _globalYOffset;
        internal void PushContaingBlock(CssBox box)
        {
            //enter new containing block
            OnPushContainingBlock(box);
            if (box != _latestContainingBlock)
            {
                this._globalXOffset += box.LocalX;
                this._globalYOffset += box.LocalY;
                OnPushDifferentContainingBlock(box);
            }
            this._containgBlockStack.Push(box);
            this._latestContainingBlock = box;
        }

        internal void PopContainingBlock()
        {
            switch (this._containgBlockStack.Count)
            {
                case 0:
                    {
                    }
                    break;
                case 1:
                    {
                        //last on
                        CssBox box = this._containgBlockStack.Pop();
                        OnPopContainingBlock();
                        if (this._latestContainingBlock != box)
                        {
                            this._globalXOffset -= box.LocalX;
                            this._globalYOffset -= box.LocalY;
                            OnPopDifferentContaingBlock(box);
                        }
                        this._latestContainingBlock = null;
                    }
                    break;
                default:
                    {
                        CssBox box = this._containgBlockStack.Pop();
                        OnPopContainingBlock();
                        if (this._latestContainingBlock != box)
                        {
                            this._globalXOffset -= box.LocalX;
                            this._globalYOffset -= box.LocalY;
                            OnPopDifferentContaingBlock(box);
                        }
                        this._latestContainingBlock = _containgBlockStack.Peek();
                    }
                    break;
            }
        }
        internal float ContainerBlockGlobalX => this._globalXOffset;

        internal float ContainerBlockGlobalY => this._globalYOffset;

        //-----------------------------------------
        protected virtual void OnPushContainingBlock(CssBox box)
        {
        }
        protected virtual void OnPopContainingBlock()
        {
        }
        protected virtual void OnPushDifferentContainingBlock(CssBox box)
        {
        }
        protected virtual void OnPopDifferentContaingBlock(CssBox box)
        {
        }
        internal CssBox LatestContainingBlock => this._latestContainingBlock;
    }
}
}