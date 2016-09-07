//BSD, 2015-2016, WinterDev

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
        List<FloatingContext> totalContext = new List<FloatingContext>();
        List<FloatingContext> floatingContexts = new List<FloatingContext>();
        FloatingContext latestFloatingContext;
        public FloatingContextStack()
        {
        }
        public void PushContainingBlock(CssBox box)
        {
            if (latestFloatingContext == null)
            {
                latestFloatingContext = new FloatingContext(box);
                totalContext.Add(latestFloatingContext);
            }
            else
            {
                if (box.Float != Css.CssFloat.None)
                {
                    latestFloatingContext = new FloatingContext(box);
                    totalContext.Add(latestFloatingContext);
                }
            }
            floatingContexts.Add(latestFloatingContext);
        }
        public void PopContainingBlock()
        {
            switch (floatingContexts.Count)
            {
                case 0:
                    latestFloatingContext = null;
                    return;
                case 1:
                    floatingContexts.Clear();
                    latestFloatingContext = null;
                    break;
                default:
                    floatingContexts.RemoveAt(floatingContexts.Count - 1);
                    latestFloatingContext = floatingContexts[floatingContexts.Count - 1];
                    break;
            }
        }
        internal CssBox LatestLeftFloatBox
        {
            get { return latestFloatingContext.LatestLeftFloatBox; }
        }
        public CssBox LatestRightFloatBox
        {
            get
            {
                return latestFloatingContext.LatestRightFloatBox;
            }
        }
        public bool HasFloatBoxInContext
        {
            get { return latestFloatingContext.HasFloatBox; }
        }

        public void AddFloatBox(CssBox floatBox)
        {
            latestFloatingContext.AddFloatBox(floatBox);
        }
        public CssBox CurrentTopOwner
        {
            get
            {
                if (latestFloatingContext != null)
                {
                    return latestFloatingContext.Owner;
                }
                return null;
            }
        }

        public List<FloatingContext> GetTotalContexts()
        {
            return totalContext;
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