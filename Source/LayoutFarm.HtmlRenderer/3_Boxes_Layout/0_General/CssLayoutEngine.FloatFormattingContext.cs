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
        List<CssBox> _children;
        public FloatingContext(CssBox parent)
        {
            _parent = parent;
        }
        public void AddChildBox(CssBox box)
        {
            if (_children == null)
            {
                _children = new List<CssBox>();
            }

        }
    }

}