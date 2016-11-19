//MIT, 2016, WinterDev
// some code from icu-project
// © 2016 and later: Unicode, Inc. and others.
// License & terms of use: http://www.unicode.org/copyright.html#License

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LayoutFarm.TextBreaker
{
    public static class CustomBreakerBuilder
    {
        static ThaiDictionaryBreakingEngine thaiDicBreakingEngine;
        static LaoDictionaryBreakingEngine laoDicBreakingEngine;
        static bool isInit;

        static void InitAllDics()
        {
            if (thaiDicBreakingEngine == null)
            {
                var customDic = new CustomDic();
                thaiDicBreakingEngine = new ThaiDictionaryBreakingEngine();
                thaiDicBreakingEngine.SetDictionaryData(customDic);//add customdic to the breaker
                customDic.SetCharRange(thaiDicBreakingEngine.FirstUnicodeChar, thaiDicBreakingEngine.LastUnicodeChar);
                customDic.LoadFromTextfile(DataDir + "/thaidict.txt");

            }
            if (laoDicBreakingEngine == null)
            {
                var customDic = new CustomDic();
                laoDicBreakingEngine = new LaoDictionaryBreakingEngine();
                laoDicBreakingEngine.SetDictionaryData(customDic);//add customdic to the breaker
                customDic.SetCharRange(laoDicBreakingEngine.FirstUnicodeChar, laoDicBreakingEngine.LastUnicodeChar);
                customDic.LoadFromTextfile(DataDir + "/laodict.txt");
            }
        }
        public static string DataDir
        {
            get;
            set;
        }
        public static CustomBreaker NewCustomBreaker()
        {
            if (!isInit)
            {
                InitAllDics();
                isInit = true;
            }
            var breaker = new CustomBreaker();
            breaker.AddBreakingEngine(thaiDicBreakingEngine);
            breaker.AddBreakingEngine(laoDicBreakingEngine);
            return breaker;
        }
    }
}