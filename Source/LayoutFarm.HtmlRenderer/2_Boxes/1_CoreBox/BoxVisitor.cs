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
                _globalXOffset += box.LocalX;
                _globalYOffset += box.LocalY;
                OnPushDifferentContainingBlock(box);
            }
            _containgBlockStack.Push(box);
            _latestContainingBlock = box;
        }

        internal void PopContainingBlock()
        {
            switch (_containgBlockStack.Count)
            {
                case 0:
                    {
                    }
                    break;
                case 1:
                    {
                        //last on
                        CssBox box = _containgBlockStack.Pop();
                        OnPopContainingBlock();
                        if (_latestContainingBlock != box)
                        {
                            _globalXOffset -= box.LocalX;
                            _globalYOffset -= box.LocalY;
                            OnPopDifferentContaingBlock(box);
                        }
                        _latestContainingBlock = null;
                    }
                    break;
                default:
                    {
                        CssBox box = _containgBlockStack.Pop();
                        OnPopContainingBlock();
                        if (_latestContainingBlock != box)
                        {
                            _globalXOffset -= box.LocalX;
                            _globalYOffset -= box.LocalY;
                            OnPopDifferentContaingBlock(box);
                        }
                        _latestContainingBlock = _containgBlockStack.Peek();
                    }
                    break;
            }
        }
        internal float ContainerBlockGlobalX => _globalXOffset;

        internal float ContainerBlockGlobalY => _globalYOffset;

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
        internal CssBox LatestContainingBlock => _latestContainingBlock;
    } 
}