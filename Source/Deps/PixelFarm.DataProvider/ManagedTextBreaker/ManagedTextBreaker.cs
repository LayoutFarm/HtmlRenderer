//MIT,  2016, WinterDev

using System;

namespace PixelFarm.Drawing.Text
{


    public class ManagedTextBreaker : TextBreaker
    {
        string locale;
        byte[] localebuff;
        public ManagedTextBreaker(TextBreakKind breakKind, string locale)
        {
            this.BreakKind = breakKind;
            this.locale = locale;
            localebuff = System.Text.Encoding.ASCII.GetBytes(locale);
        }
        enum State
        {
            Character,
            Number,
            Punc,
            Whitespace,
        }
        public override void DoBreak(char[] input, int start, int len, OnBreak onbreak)
        {
            //just break when found space

            int end_before = start + len;

            //----------------------------------------
            //this is simple example***
            //use state,similar to other lang lexer/parser
            //TODO: improve efficientcy and performance
            //----------------------------------------
            for (int i = start; i < end_before; ++i)
            {
                char c = input[i];

                if (char.IsLetter(c) || c == '_')
                {

                }
                else if (char.IsWhiteSpace(c))
                {

                }
                else if (char.IsDigit(c))
                {
                }
                else if (char.IsPunctuation(c))
                {
                }
            }



            //
            //1. 
            //UBreakIteratorType type = UBreakIteratorType.WORD;
            //switch (BreakKind)
            //{
            //    default:
            //    case TextBreakKind.Word:
            //        type = UBreakIteratorType.WORD;
            //        break;
            //    case TextBreakKind.Sentence:
            //        type = UBreakIteratorType.SENTENCE;
            //        break;
            //}
            ////------------------------ 
            //int errCode = 0;
            ////break all string  
            //unsafe
            //{
            //    fixed (char* h = &input[start])
            //    {
            //        IntPtr nativeIter = NativeTextBreakerLib.MtFt_UbrkOpen(type, localebuff, h, len, out errCode);
            //        int cur = NativeTextBreakerLib.MtFt_UbrkFirst(nativeIter);
            //        while (cur != DONE)
            //        {
            //            int next = NativeTextBreakerLib.MtFt_UbrkNext(nativeIter);
            //            int status = NativeTextBreakerLib.MtFt_UbrkGetRuleStatus(nativeIter);
            //            if (next != DONE && AddToken(type, status))
            //            {
            //                onbreak(new SplitBound(cur, next - cur));
            //            }
            //            cur = next;
            //        }
            //        NativeTextBreakerLib.MtFt_UbrkClose(nativeIter);
            //    }
            //}
        }


        const int DONE = -1;
        static bool AddToken(UBreakIteratorType type, int status)
        {
            switch (type)
            {
                case UBreakIteratorType.CHARACTER:
                    return true;
                case UBreakIteratorType.LINE:
                case UBreakIteratorType.SENTENCE:
                    return true;
                case UBreakIteratorType.WORD:
                    return status < (int)UWordBreak.NONE || status >= (int)UWordBreak.NONE_LIMIT;
            }
            return false;
        }

        ////this is text breaker impl with ICU lib
        //static InMemoryIcuDataHolder dataHolder;

        //static string s_icuDataFile;
        //static bool s_isDataLoaded;
        //static object s_dataLoadLock = new object();
        public static void SetICUDataFile(string icudatafile)
        {
            //lock (s_dataLoadLock)
            //{
            //    if (s_isDataLoaded)
            //    {
            //        return;
            //    }
            //}
            //if (s_isDataLoaded)
            //{
            //    return;
            //}
            //s_isDataLoaded = true;
            //s_icuDataFile = icudatafile;
            ////----------
            //int major, minor, revision;
            //NativeTextBreakerLib.MyFtLibGetFullVersion(out major, out minor, out revision);
            //if (dataHolder == null)
            //{
            //    //dataHolder = new InMemoryIcuDataHolder(@"d:\WImageTest\icudt57l\icudt57l.dat");
            //    dataHolder = new InMemoryIcuDataHolder(icudatafile);
            //    dataHolder.Use();
            //}
        }

    }

}