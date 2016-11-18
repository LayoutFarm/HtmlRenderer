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


    public class ThaiDictionaryBreakingEngine : DictionaryBreakingEngine
    {
        CustomDic _customDic;
        public void SetDictionaryData(CustomDic customDic)
        {
            this._customDic = customDic;

        }
        protected override CustomDic CurrentCustomDic
        {
            get { return _customDic; }
        }

        public override void BreakWord(WordVisitor visitor, char[] charBuff, int startAt, int len)
        {
            base.BreakWord(visitor, charBuff, startAt, len);
        }
        public override bool CanBeStartChar(char c)
        {
            return canbeStartChars[c - this.FirstUnicodeChar];

        }
        protected override WordGroup GetWordGroupForFirstChar(char c)
        {
            return _customDic.GetWordGroupForFirstChar(c);
        }

        public override char FirstUnicodeChar
        {
            //0E00-0E7F 

            get { return s_firstChar; }
        }
        public override char LastUnicodeChar
        {
            get
            {
                return s_lastChar;
            }
        }
        //------------------------------------
        //eg thai sara

        static bool[] canbeStartChars;
        const char s_firstChar = (char)0x0E00;
        const char s_lastChar = (char)0xE7F;
        static ThaiDictionaryBreakingEngine()
        {
            char[] cannotStartWithChars = "ะาิีึืุู่้๊๋็ฺ์ํั".ToCharArray();
            canbeStartChars = new bool[s_lastChar - s_firstChar + 1];
            for (int i = canbeStartChars.Length - 1; i >= 0; --i)
            {
                canbeStartChars[i] = true;
            }
            //------------------------------------------------
            for (int i = cannotStartWithChars.Length - 1; i >= 0; --i)
            {
                int shiftedIndex = cannotStartWithChars[i] - s_firstChar;
                //some char can't be start char
                canbeStartChars[shiftedIndex] = false;
            }

        }
    }


}