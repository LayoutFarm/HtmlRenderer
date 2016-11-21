//MIT, 2016, WinterDev
// some code from icu-project
// © 2016 and later: Unicode, Inc. and others.
// License & terms of use: http://www.unicode.org/copyright.html#License

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LayoutFarm.TextBreak
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
            Stack<int> candidate = visitor.GetTempCandidateBreaks();

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
                WordGroup wordgroup = GetWordGroupForFirstChar(c);
                if (wordgroup == null)
                {
                    //continue next char
                    ++i;
                    visitor.AddWordBreakAt(i);
                }
                else
                {
                    //check if we can move next
                    if (visitor.IsEnd)
                    {
                        visitor.State = VisitorState.End;
                        return;
                    }
                    //---------------------
                    WordGroup c_wordgroup = wordgroup;
                    candidate.Clear();
                    int candidateLen = 1;
                    if (c_wordgroup.PrefixIsWord)
                    {
                        candidate.Push(candidateLen);
                    }

                    bool continueRead = true;

                    int savedIndex = visitor.CurrentIndex;
                    while (continueRead)
                    {
                        //not end
                        //then move next
                        candidateLen++;
                        visitor.SetCurrentIndex(i + 1);
                        WordGroup next = c_wordgroup.GetSubGroup(visitor);
                        //for debug
                        //string prefix = (next == null) ? "" : next.GetPrefix(CurrentCustomDic.TextBuffer);  
                        if (next == null)
                        {
                            continueRead = false;
                            //no deeper group
                            //then check if 
                            if (c_wordgroup.WordSpanListCount > 0)
                            {
                                int p1 = visitor.CurrentIndex;
                                //p2: suggest position
                                int p2 = c_wordgroup.FindInUnIndexMember(visitor);
                                if (p2 - p1 > 0)
                                {
                                    visitor.AddWordBreakAt(p2);
                                    visitor.SetCurrentIndex(p2);
                                }
                                else
                                {
                                    //on the same pos
                                    if (visitor.State == VisitorState.OutOfRangeChar)
                                    {
                                        return;
                                    }
                                    else
                                    {
                                        bool foundCandidate = false;
                                        while (candidate.Count > 0)
                                        {

                                            int candi1 = candidate.Pop();
                                            //try
                                            visitor.SetCurrentIndex(visitor.LatestBreakAt + candi1);
                                            //check if we can use this candidate
                                            char next_char = visitor.Char;
                                            if (CanBeStartChar(next_char))
                                            {
                                                //use this
                                                //use this candidate if possible
                                                visitor.AddWordBreakAt(visitor.CurrentIndex);
                                                foundCandidate = true;
                                                break;
                                            }
                                        }
                                        if (!foundCandidate)
                                        {
                                            //no next word, no candidate
                                            //skip this 
                                            visitor.AddWordBreakAt(visitor.LatestBreakAt + 1);
                                            visitor.SetCurrentIndex(visitor.LatestBreakAt); 
                                        }

                                    }
                                }

                            }
                            else
                            {

                                bool foundCandidate = false;
                                while (candidate.Count > 0)
                                {

                                    int candi1 = candidate.Pop();
                                    //try
                                    visitor.SetCurrentIndex(visitor.LatestBreakAt + candi1);
                                    //check if we can use this candidate
                                    char next_char = visitor.Char;
                                    if (!CanHandle(next_char))
                                    {
                                        //use this
                                        //use this candidate if possible
                                        visitor.AddWordBreakAt(visitor.CurrentIndex);
                                        foundCandidate = true;
                                        break;
                                    }
                                    if (CanBeStartChar(next_char))
                                    {
                                        //use this
                                        //use this candidate if possible
                                        visitor.AddWordBreakAt(visitor.CurrentIndex);
                                        foundCandidate = true;
                                        break;
                                    }
                                }
                                if (!foundCandidate)
                                {
                                    //no next word, no candidate
                                    //skip this 
                                    visitor.AddWordBreakAt(visitor.LatestBreakAt + 1);
                                    visitor.SetCurrentIndex(visitor.LatestBreakAt); 
                                }

                            }
                            i = visitor.CurrentIndex;
                        }
                        else
                        {
                            if (next.PrefixIsWord)
                            {
                                candidate.Push(candidateLen);
                            }
                            c_wordgroup = next;
                            i = visitor.CurrentIndex;
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