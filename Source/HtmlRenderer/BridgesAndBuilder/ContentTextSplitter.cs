//BSD 2014,WinterDev 


using System.Collections.Generic;


namespace HtmlRenderer.Dom
{



    class ContentTextSplitter
    {


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

        List<CssRun> GetNewRunList()
        {
            if (myRunPool.Count > 0)
            {
                return myRunPool.Pop();
            }
            else
            {
                return new List<CssRun>(5);
            }
        }
        void StoreBackNotUse(List<CssRun> tmpRuns)
        {
            tmpRuns.Clear();
            myRunPool.Push(tmpRuns);

        }
        public void ParseWordContent(char[] textBuffer, BoxSpec spec, out List<CssRun> runlistOutput, out bool hasSomeCharacter)
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

            //bool has_someword2 = false;

            //for (int i = textBuffer.Length - 1; i >= 0; --i)
            //{
            //    char c = textBuffer[i];
            //    if (!char.IsWhiteSpace(c))
            //    {
            //        has_someword2 = true;
            //        break;
            //    }
            //}

            //if (!has_someword2 && !preserveWhiteSpace && !preserverLine)
            //{
            //    runlistOutput = null;
            //    hasSomeCharacter = false;
            //    return;
            //}




            //---------------------------------------
            //1. check if has some text 
            //--------------------------------------
            var runlist = GetNewRunList();
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
                            runlist.Add(CssTextRun.CreateTextRun(startIndex, appendLength));
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
                                //other whitespace
                                parsingState = WordParsingState.Whitespace;
                                startIndex = i;
                                appendLength = 1;//start collect whitespace
                            }
                            else
                            {
                                //character 
                                parsingState = WordParsingState.CharacterCollecting;
                                startIndex = i;
                                appendLength = 1;//start collect whitespace 
                            }

                        } break;
                    case WordParsingState.Whitespace:
                        {
                            if (!char.IsWhiteSpace(c0))
                            {
                                //switch to character mode   
                                if (newRun > 0 || preserveWhiteSpace)
                                {
                                    runlist.Add(CssTextRun.CreateWhitespace(preserveWhiteSpace ? appendLength : 1));
                                    newRun++;
                                }
                                parsingState = WordParsingState.CharacterCollecting;
                                startIndex = i;//start collect
                                appendLength = 1;//start append length 
                            }
                            else
                            {
                                if (appendLength == 0)
                                {
                                    startIndex = i;
                                }
                                appendLength++;
                            }
                        } break;
                    case WordParsingState.CharacterCollecting:
                        {
                            if (char.IsWhiteSpace(c0))
                            {
                                //flush collecting token  
                                runlist.Add(CssTextRun.CreateTextRun(startIndex, appendLength));
                                newRun++;
                                hasSomeCharacter = true;
                                parsingState = WordParsingState.Whitespace;
                                startIndex = i;//start collect
                                appendLength = 1; //collect whitespace
                            }
                            else
                            {
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
                            runlist.Add(CssTextRun.CreateTextRun(startIndex, appendLength));
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