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

}