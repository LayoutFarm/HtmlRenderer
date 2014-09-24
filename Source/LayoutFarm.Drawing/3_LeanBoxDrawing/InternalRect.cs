//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using LayoutFarm.Drawing; 
namespace LayoutFarm
{   
    public class InternalRect
    {
        public int _left;
        public int _top;
        public int _right;
        public int _bottom; 
        private InternalRect()
        {
        }
        private InternalRect(int left, int top, int right, int bottom)
        {
            this._left = left;
            this._top = top;
            this._bottom = bottom;
            this._right = right;
        }
        private InternalRect(int width, int height)
        {
            this._right = width;
            this._bottom = height;
        }

        public Rectangle ToRectangle()
        {
            return new Rectangle(this._left, this._top, this._right - this._left, this._bottom - this._top);

        }

        public void LoadValues(InternalRect anotherRect)
        {
            this._left = anotherRect._left;
            this._right = anotherRect._right;
            this._top = anotherRect._top;
            this._bottom = anotherRect._bottom;
        }
        public static InternalRect CreateFromLTRB(int left, int top, int right, int bottom)
        {
            if (rectPool.Count > 0)
            {
                InternalRect rectInPool = rectPool.Pop();
                rectInPool._left = left;
                rectInPool._top = top;
                rectInPool._right = right;
                rectInPool._bottom = bottom;
                return rectInPool;
            }
            else
            {
                return new InternalRect(left, top, right, bottom);
            }

        }
        public static InternalRect CreateFromWH(int width, int height)
        {
            if (rectPool.Count > 0)
            {
                InternalRect rectInPool = rectPool.Pop();
                rectInPool._left = 0;
                rectInPool._top = 0;
                rectInPool._right = width;
                rectInPool._bottom = height;
                return rectInPool;
            }
            else
            {
                return new InternalRect(width, height);
            }
        }
        public static InternalRect CreateFromRect(int left, int top, int width, int height)
        {
            if (rectPool.Count > 0)
            {
                InternalRect rectInPool = rectPool.Pop();
                rectInPool._left = left;
                rectInPool._top = top;
                rectInPool._right = left + width;
                rectInPool._bottom = top + height;
                return rectInPool;
            }
            else
            {
                return new InternalRect(left, top, left + width, top + height);
            }

        }

        static Stack<InternalRect> rectPool = new Stack<InternalRect>(20);
        public static InternalRect CreateFromRect(Rectangle r)
        {
            return CreateFromLTRB(r.Left, r.Top, r.Right, r.Bottom);
        }

        public static void FreeInternalRect(InternalRect rect)
        {
            rectPool.Push(rect);

        }
        public int Width
        {
            get
            {
                return _right - _left;
            }
        }
        public int Height
        {
            get
            {
                return _bottom - _top;
            }
        }
        public void MergeRect(InternalRect r2)
        {

            if (r2._left < _left)
            {
                _left = r2._left;
            }

            if (r2._right > _right)
            {
                _right = r2._right;
            }
            if (r2._top < _top)
            {
                _top = r2._top;
            }
            if (r2._bottom > _bottom)
            {
                _bottom = r2._bottom;
            }
        }

        public void MergeRect(Rectangle r2)
        {
            if (r2.Left < _left)
            {
                _left = r2.Left;
            }
            if (r2.Right > _right)
            {
                _right = r2.Right;
            }
            if (r2.Top < _top)
            {
                _top = r2.Top;
            }
            if (r2.Bottom > _bottom)
            {
                _bottom = r2.Bottom;
            }
        }
        public void Intersect(Rectangle r2)
        {
            if (r2.Left >= _left)
            {
                if (r2.Left < _right)
                {
                    _left = r2.Left;
                    if (r2.Right < _right)
                    {
                        _right = r2.Right;
                    }
                }
                else
                {
                    _left = 0;
                    _top = 0;
                    _right = 0;
                    _bottom = 0;
                    return;
                }
            }
            else
            {
                if (r2.Right <= _left)
                {
                    _left = 0;
                    _top = 0;
                    _right = 0;
                    _bottom = 0;
                    return;
                }
                else
                {
                    if (r2.Right < _right)
                    {
                        _right = r2.Right;
                    }

                }
            }
            if (r2.Top >= _top)
            {
                if (r2.Top < _bottom)
                {
                    _top = r2.Top;

                    if (r2.Bottom < _bottom)
                    {
                        _bottom = r2.Bottom;
                    }
                }
                else
                {
                    _left = 0;
                    _top = 0;
                    _right = 0;
                    _bottom = 0;
                    return;
                }
            }
            else
            {
                if (r2.Bottom <= _top)
                {
                    _left = 0;
                    _top = 0;
                    _right = 0;
                    _bottom = 0;
                    return;
                }
                else
                {
                    if (r2.Bottom < _bottom)
                    {
                        _bottom = r2.Bottom;
                    }
                }
            }
        }
        public void Offset(int dx, int dy)
        {
            _left += dx;
            _right += dx;
            _top += dy;
            _bottom += dy;
        }
        public void OffsetX(int dx)
        {
            _left += dx;
            _right += dx;
        }
        public void OffsetY(int dy)
        {

            _top += dy;
            _bottom += dy;
        }

        public bool IntersectsWith(int left, int top, int right, int bottom)
        {
            if (((_left <= left) && (_right > left)) || ((_left >= left) && (_left < right)))
            {
                if (((_top <= top) && (_bottom > top))
|| ((_top >= top) && (_top < bottom)))
                {
                    return true;
                }
            }
            return false;
        }




#if DEBUG
        public override string ToString()
        {
            return _left + "," + _top + "," + (_right - _left) + "," + (_bottom - _top);
        }
#endif

    }

}