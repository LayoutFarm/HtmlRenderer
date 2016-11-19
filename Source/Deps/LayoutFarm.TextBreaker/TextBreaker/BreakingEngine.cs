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
    public abstract class BreakingEngine
    {
        public abstract void BreakWord(WordVisitor visitor, char[] charBuff, int startAt, int len);
        public abstract bool CanBeStartChar(char c);
        public abstract bool CanHandle(char c);
    }
    public abstract class DictionaryBreakingEngine : BreakingEngine
    {
        public abstract char FirstUnicodeChar { get; }
        public abstract char LastUnicodeChar { get; }
        public override bool CanHandle(char c)
        {
            //in this range or not
            return c >= this.FirstUnicodeChar && c <= this.LastUnicodeChar;
        }
        protected abstract CustomDic CurrentCustomDic { get; }
        protected abstract WordGroup GetWordGroupForFirstChar(char c);
        public override void BreakWord(WordVisitor visitor, char[] charBuff, int startAt, int len)
        {
            visitor.State = VisitorState.Parsing;
            visitor.CurrentCustomDic = this.CurrentCustomDic;
            char c_first = this.FirstUnicodeChar;
            char c_last = this.LastUnicodeChar;
            int endAt = startAt + len;
            for (int i = startAt; i < endAt; )
            {
                //find proper start words;
                char c = charBuff[i];
                //----------------------
                //check if c is in our responsiblity
                if (c < c_first || c > c_last)
                {
                    //out of our range
                    //should return ?
                    visitor.State = VisitorState.OutOfRangeChar;
                    return;
                }
                //----------------------
                WordGroup state = GetWordGroupForFirstChar(c); // _customDic.GetWordGroupForFirstChar(c);
                if (state == null)
                {
                    //continue next char
                    ++i;
                    visitor.AddWordBreakAt(i);
                }
                else
                {
                    //use this to find a proper word
                    int prevIndex = i;
                    visitor.SetCurrentIndex(i + 1);
                    state.FindBreak(visitor);
                    i = visitor.LatestBreakAt;
                    if (prevIndex == i)
                    {
                        if (visitor.CurrentIndex >= len - 1)
                        {
                            //the last one 
                            visitor.State = VisitorState.End;
                            break;
                        }
                        else
                        {
                            //current char is not found in dic
                            //check if it can stand alone or not

                            if (!visitor.CanbeStartChar(visitor.Char))
                            {
                                //if not
                                //save it in this group
                                i++;
                                i++;
                                visitor.AddWordBreakAt(i);
                                visitor.SetCurrentIndex(visitor.LatestBreakAt);
                            }
                            else
                            {
                                i++;
                                visitor.AddWordBreakAt(i);
                                visitor.SetCurrentIndex(visitor.LatestBreakAt);
                            }

                        }
                    }
                }
            }
            //------
            if (visitor.CurrentIndex >= len - 1)
            {
                //the last one 
                visitor.State = VisitorState.End;

            }
        }
    }



}