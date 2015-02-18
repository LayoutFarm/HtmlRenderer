//BSD 2014-2015,WinterDev  
using System.Collections.Generic;
using LayoutFarm.Css;
using LayoutFarm.HtmlBoxes;

namespace LayoutFarm.Composers
{
    class ContentTextSplitter
    {
        //configure icu's locale here 
        string icuLocal = "th-TH";

        Stack<List<CssRun>> myRunPool = new Stack<List<CssRun>>(3);
        public ContentTextSplitter()
        {
        }
        enum WordParsingState
        {
            Init,
            Whitespace,
            CharacterCollecting
        }

        List<CssRun> GetStockRunList()
        {
            if (myRunPool.Count > 0)
            {
                return myRunPool.Pop();
            }
            else
            {
                return new List<CssRun>(10);
            }
        }
        void StoreBackNotUse(List<CssRun> tmpRuns)
        {
            tmpRuns.Clear();
            myRunPool.Push(tmpRuns);
        }

        void AddToRunList(char[] textBuffer, List<CssRun> runlist, int startIndex, int appendLength, ref bool needICUSplitter)
        {
            if (needICUSplitter)
            {
                //use icu splitter 
                //copy text buffer to icu *** 
                var parts = Icu.BreakIterator.GetSplitBoundIter(Icu.BreakIterator.UBreakIteratorType.WORD,
                    icuLocal, textBuffer, startIndex, appendLength);

                //iterate new split
                foreach (var bound in parts)
                {
                    runlist.Add(
                        CssTextRun.CreateTextRun(startIndex + bound.startIndex, bound.length));
                }

                needICUSplitter = false;//reset
            }
            else
            {
                runlist.Add(CssTextRun.CreateTextRun(startIndex, appendLength));
            }
        }
        public void ParseWordContent(char[] textBuffer, BoxSpec spec,
            out List<CssRun> runlistOutput, out bool hasSomeCharacter)
        {

            bool preserverLine = false;
            bool preserveWhiteSpace = false;
            switch (spec.WhiteSpace)
            {
                case CssWhiteSpace.Pre:
                case CssWhiteSpace.PreWrap:
                    //run and preserve whitespace  
                    preserveWhiteSpace = true;
                    preserverLine = true;
                    break;
                case CssWhiteSpace.PreLine:
                    preserverLine = true;
                    break;
            }

            //---------------------------------------
            //1. check if has some text 
            //--------------------------------------
            List<CssRun> runlist = GetStockRunList();
            hasSomeCharacter = false;
            //--------------------------------------
            //just parse and preserve all whitespace
            //--------------------------------------  
            int startIndex = 0;
            int buffLength = textBuffer.Length;
            //whitespace and respect newline  
            WordParsingState parsingState = WordParsingState.Init;
            int appendLength = 0;
            //--------------------------------------  
            int newRun = 0;
            bool needICUSplitter = false;

            for (int i = 0; i < buffLength; ++i)
            {
                char c0 = textBuffer[i];
                if (c0 == '\n')
                {
                    //flush exising 
                    if (appendLength > 0)
                    {
                        if (parsingState == WordParsingState.CharacterCollecting)
                        {
                            if (needICUSplitter)
                            {
                                AddToRunList(textBuffer, runlist, startIndex, appendLength, ref needICUSplitter);
                            }
                            else
                            {
                                runlist.Add(CssTextRun.CreateTextRun(startIndex, appendLength));
                            }

                            newRun++;
                            hasSomeCharacter = true;
                        }
                        else
                        {
                            if (newRun > 0 || preserveWhiteSpace)
                            {
                                runlist.Add(CssTextRun.CreateWhitespace(preserveWhiteSpace ? appendLength : 1));
                                newRun++;
                            }
                        }
                        appendLength = 0; //clear append length 
                    }
                    //append with new line                    
                    if (preserverLine)
                    {
                        runlist.Add(CssTextRun.CreateLineBreak());
                        newRun++;
                    }

                    startIndex = i;
                    parsingState = WordParsingState.Init;
                    continue;
                }
                else if (c0 == '\r')
                {
                    //skip 
                    continue;
                }
                //----------------------------------------
                //switch by state  
                switch (parsingState)
                {
                    case WordParsingState.Init:
                        {
                            if (char.IsWhiteSpace(c0))
                            {

                                parsingState = WordParsingState.Whitespace;
                                startIndex = i;
                                appendLength = 1;//start collect whitespace
                            }
                            else if (char.IsPunctuation(c0))
                            {
                                parsingState = WordParsingState.Init;
                                startIndex = i;
                                appendLength = 1;
                                //add single token
                                runlist.Add(CssTextRun.CreateTextRun(startIndex, appendLength));
                                newRun++;
                            }
                            else
                            {
                                //character  
                                if (c0 > '~') { needICUSplitter = true; }

                                parsingState = WordParsingState.CharacterCollecting;
                                startIndex = i;
                                appendLength = 1;//start collect whitespace 
                            }

                        } break;
                    case WordParsingState.Whitespace:
                        {
                            if (char.IsWhiteSpace(c0))
                            {
                                if (appendLength == 0)
                                {
                                    startIndex = i;
                                }
                                appendLength++;
                            }
                            else
                            {

                                if (newRun > 0 || preserveWhiteSpace)
                                {
                                    runlist.Add(CssTextRun.CreateWhitespace(preserveWhiteSpace ? appendLength : 1));
                                    newRun++;
                                }
                                //-----------------------------------------------------

                                if (char.IsPunctuation(c0))
                                {
                                    parsingState = WordParsingState.Init;
                                    startIndex = i;
                                    appendLength = 1;
                                    //add single token
                                    runlist.Add(CssTextRun.CreateTextRun(startIndex, appendLength));
                                    newRun++;
                                }
                                else
                                {
                                    if (c0 > '~') { needICUSplitter = true; }

                                    parsingState = WordParsingState.CharacterCollecting;
                                    startIndex = i;//start collect
                                    appendLength = 1;//start append length 
                                }
                            }
                        } break;
                    case WordParsingState.CharacterCollecting:
                        {
                            bool isWhiteSpace;
                            if ((isWhiteSpace = char.IsWhiteSpace(c0)) || char.IsPunctuation(c0))
                            {
                                //flush collecting token  
                                if (needICUSplitter)
                                {
                                    AddToRunList(textBuffer, runlist, startIndex, appendLength, ref needICUSplitter);
                                }
                                else
                                {
                                    runlist.Add(CssTextRun.CreateTextRun(startIndex, appendLength));
                                }

                                newRun++;
                                hasSomeCharacter = true;
                                startIndex = i;//start collect
                                appendLength = 1; //collect whitespace

                                if (isWhiteSpace)
                                {
                                    parsingState = WordParsingState.Whitespace;
                                }
                                else
                                {
                                    parsingState = WordParsingState.Init;
                                    runlist.Add(CssTextRun.CreateTextRun(startIndex, appendLength));
                                    newRun++;
                                }
                                //---------------------------------------- 
                            }
                            else
                            {
                                if (c0 > '~') { needICUSplitter = true; }

                                if (appendLength == 0)
                                {
                                    startIndex = i;
                                }
                                appendLength++;
                            }

                        } break;

                }
            }
            //--------------------
            if (appendLength > 0)
            {
                switch (parsingState)
                {
                    case WordParsingState.Whitespace:
                        {
                            if (newRun > 0 || preserveWhiteSpace)
                            {
                                runlist.Add(CssTextRun.CreateWhitespace(preserveWhiteSpace ? appendLength : 1));
                                newRun++;
                            }

                        } break;
                    case WordParsingState.CharacterCollecting:
                        {
                            if (needICUSplitter)
                            {
                                AddToRunList(textBuffer, runlist, startIndex, appendLength, ref needICUSplitter);
                            }
                            else
                            {
                                runlist.Add(CssTextRun.CreateTextRun(startIndex, appendLength));
                            }

                            hasSomeCharacter = true;
                            newRun++;
                        } break;
                }
            }

            if (newRun > 0)
            {
                runlistOutput = runlist;
                //send runlist 
                //not store to pool
            }
            else
            {
                StoreBackNotUse(runlist);
                runlistOutput = null;
            }
        }
    }
}