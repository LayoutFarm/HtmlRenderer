// 2015, BSD, WinterDev
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
    class FloatingContext
    {
        CssBox _parent;
        List<CssBox> _floatBoxes;
        CssBox _latestLeftFloatBox;
        CssBox _latestRightFloatBox;
        public FloatingContext(CssBox parent)
        {
            _parent = parent;
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
                return _parent.Float != Css.CssFloat.None;
            }
        }

    }

}