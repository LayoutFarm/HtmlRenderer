//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
using Typography.TextBreak;

namespace LayoutFarm.Composers
{
    public class MyManagedTextBreaker : ITextBreaker
    {
        CustomBreaker _textBreaker;
        List<int> _breakAtList;

        public MyManagedTextBreaker()
        {
            //TODO: review config folder here            
            _textBreaker = CustomBreakerBuilder.NewCustomBreaker();
            _textBreaker.SetNewBreakHandler(vis => _breakAtList.Add(vis.LatestBreakAt));
        }

        public void DoBreak(char[] inputBuffer, int startIndex, int len, List<int> breakAtList)
        {
            _breakAtList = breakAtList;
            //
            _textBreaker.BreakWords(inputBuffer, startIndex, len);
        }
        public void DoBreak(char[] inputBuffer, int startIndex, int len, List<WordBreakInfo> breakAtList)
        {

        }
    }
}