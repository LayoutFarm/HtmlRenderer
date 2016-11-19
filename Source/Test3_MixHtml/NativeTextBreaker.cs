//Apache2, 2014-2016, WinterDev

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using LayoutFarm.TextBreaker;
using PixelFarm.Drawing.Text;

namespace LayoutFarm.Composers
{
    class MyNativeTextBreaker : ITextBreaker
    {
        NativeTextBreaker myTextBreaker;
        public MyNativeTextBreaker()
        {
            myTextBreaker = new NativeTextBreaker(PixelFarm.Drawing.Text.TextBreakKind.Word, "en-US");
        }
        public void DoBreak(char[] inputBuffer, int startIndex, int len, List<int> breakAtList)
        {
            myTextBreaker.DoBreak(inputBuffer, startIndex, len, splitBound =>
            {
                breakAtList.Add(splitBound.startIndex + splitBound.length);
            });
        }
    }
}