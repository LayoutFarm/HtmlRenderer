//BSD, 2015-present, WinterDev

using System;
using System.Collections.Generic;
namespace LayoutFarm.HtmlBoxes
{
    struct FloatFormattingContext
    {
        public CssBox rightFloatBox;
        public CssBox leftFloatBox;
        public float lineLeftOffset;
        public float lineRightOffset;
        public float offsetFloatTop;
        public bool floatingOutOfLine;
    }

    class FloatingContextStack
    {

#if DEBUG
        Dictionary<CssBox, bool> _dbugDic1 = new Dictionary<CssBox, bool>();
#endif
        List<FloatingContext> _totalContexts = new List<FloatingContext>();
        List<FloatingContext> _floatingContexts = new List<FloatingContext>();
        FloatingContext _latestFloatingContext;
        public FloatingContextStack()
        {
        }
        public void Reset()
        {
            _latestFloatingContext = null;
            _totalContexts.Clear();
            _floatingContexts.Clear();
#if DEBUG
            _dbugDic1.Clear();
#endif
        }
        public void PushContainingBlock(CssBox box)
        {
            if (_latestFloatingContext == null)
            {
                if (box.IsBody)
                {
                    _latestFloatingContext = new FloatingContext(box);
                    _totalContexts.Add(_latestFloatingContext);
                }
            }
            else
            {
                if (box.Float != Css.CssFloat.None)
                {
                    _latestFloatingContext = new FloatingContext(box);
                    _totalContexts.Add(_latestFloatingContext);
                }
            }
            if (_latestFloatingContext != null)
            {
                _floatingContexts.Add(_latestFloatingContext);
            }
        }
        public void PopContainingBlock()
        {
            switch (_floatingContexts.Count)
            {
                case 0:
                    _latestFloatingContext = null;
                    return;
                case 1:
                    _floatingContexts.Clear();
                    _latestFloatingContext = null;
                    break;
                default:
                    _floatingContexts.RemoveAt(_floatingContexts.Count - 1);
                    _latestFloatingContext = _floatingContexts[_floatingContexts.Count - 1];
                    break;
            }
        }
        internal CssBox LatestLeftFloatBox
        {
            get { return _latestFloatingContext.LatestLeftFloatBox; }
        }
        public CssBox LatestRightFloatBox
        {
            get
            {
                return _latestFloatingContext.LatestRightFloatBox;
            }
        }
        public bool HasFloatBoxInContext
        {
            get { return _latestFloatingContext != null && _latestFloatingContext.HasFloatBox; }
        }

        public void AddFloatBox(CssBox floatBox)
        {
#if DEBUG
            if (_dbugDic1.ContainsKey(floatBox))
            {
                return;
            }
            else
            {
                _dbugDic1.Add(floatBox, true);
            }
#endif
            _latestFloatingContext.AddFloatBox(floatBox);
        }
        public CssBox CurrentTopOwner
        {
            get
            {
                if (_latestFloatingContext != null)
                {
                    return _latestFloatingContext.Owner;
                }
                return null;
            }
        }

        public List<FloatingContext> GetTotalContexts()
        {
            return _totalContexts;
        }
    }

    class FloatingContext
    {
        CssBox _owner;
        List<CssBox> _floatBoxes;
        CssBox _latestLeftFloatBox;
        CssBox _latestRightFloatBox;
        public FloatingContext(CssBox owner)
        {
            _owner = owner;
        }
        public CssBox Owner
        {
            get { return this._owner; }
        }
        public void AddFloatBox(CssBox box)
        {
            if (_floatBoxes == null)
            {
                _floatBoxes = new List<CssBox>();
            }
            _floatBoxes.Add(box);
            switch (box.Float)
            {
                case Css.CssFloat.Left:
                    _latestLeftFloatBox = box;
                    break;
                case Css.CssFloat.Right:
                    _latestRightFloatBox = box;
                    break;
                default:
                    throw new NotSupportedException();
            }
        }
        public CssBox LatestLeftFloatBox
        {
            get { return _latestLeftFloatBox; }
        }
        public CssBox LatestRightFloatBox
        {
            get { return _latestRightFloatBox; }
        }
        public bool HasFloatBox
        {
            get { return _latestLeftFloatBox != null || _latestRightFloatBox != null; }
        }

        public bool OwnerIsFloat
        {
            get
            {
                return _owner.Float != Css.CssFloat.None;
            }
        }

        public int FloatBoxCount
        {
            get
            {
                if (_floatBoxes == null)
                {
                    return 0;
                }
                else
                {
                    return _floatBoxes.Count;
                }
            }
        }
        public CssBox GetBox(int index)
        {
            if (_floatBoxes != null)
            {
                return _floatBoxes[index];
            }
            return null;
        }
        public void Clear()
        {
            if (_floatBoxes != null)
            {
                _floatBoxes.Clear();
            }
        }
    }
}