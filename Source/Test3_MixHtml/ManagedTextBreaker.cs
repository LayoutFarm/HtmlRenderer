////Apache2, 2014-2017, WinterDev

//using System;
//using System.Collections.Generic;
//using System.Windows.Forms;
//using Typography.TextBreak;
//namespace LayoutFarm.Composers
//{
//    class MyManagedTextBreaker : ITextBreaker
//    {
//        CustomBreaker myTextBreaker;
//        public MyManagedTextBreaker()
//        {
//            //TODO: review config folder here            
//            myTextBreaker = CustomBreakerBuilder.NewCustomBreaker();
//        }
//        public void DoBreak(char[] inputBuffer, int startIndex, int len, List<int> breakAtList)
//        {
//            myTextBreaker.BreakWords(inputBuffer, startIndex);
//            myTextBreaker.LoadBreakAtList(breakAtList);

//        }
//    }

//}