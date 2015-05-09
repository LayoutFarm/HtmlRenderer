// 2015, BSD, WinterDev
using System;
using System.Collections.Generic;

namespace LayoutFarm.HtmlBoxes
{
    struct FloatFormattingContext
    {
        
        //Stack<CssBox> lineFormattingContextStack = new Stack<CssBox>();

        //Stack<float> lineOffsetLeftStack = new Stack<float>();
        //Stack<float> lineOffsetRightStack = new Stack<float>();
        //Stack<bool> floatingOutOfLineStack = new Stack<bool>();
        public CssBox rightFloatBox;
        public CssBox leftFloatBox;
        public float lineLeftOffset;
        public float lineRightOffset;
        public bool floatingOutOfLine;
    }

}