//BSD 2014,WinterDev 

using System;
using System.Collections.Generic;
using HtmlRenderer.Parse;
using HtmlRenderer.Utils;
using System.Text;

namespace HtmlRenderer.Dom
{

    class ContentTextSplitter
    {
        public static readonly ContentTextSplitter DefaultSplitter = new ContentTextSplitter();
        public ContentTextSplitter()
        {

        }
        enum WordParsingState
        {
            Init,
            Whitespace,
            CharacterCollecting
        }


        public void ParseWordContent(CssBox box)
        {

            switch (box.WhiteSpace)
            {
                case CssWhiteSpace.Pre:
                case CssWhiteSpace.PreWrap:
                    ParseWordContentPreserveWhitespace(box);
                    return;
                case CssWhiteSpace.PreLine:
                    ParseWordContentRespectNewLine(box);
                    return;
            }
            //other ...  
            char[] textBuffer = CssBox.UnsafeGetTextBuffer(box);
#if DEBUG
            //string dbugStr = new string(textBuffer);

#endif

            bool keepPreWhitespace = box.HtmlElement == null;
            List<CssRun> boxRuns = CssBox.UnsafeGetRunListOrCreateIfNotExists(box);

            boxRuns.Clear(); //clear prev results

            int startIndex = 0;
            int buffLength = textBuffer.Length;
            bool boxIsNotBreakAll = box.WordBreak != CssWordBreak.BreakAll;

            //not preserve whitespace
            WordParsingState parsingState = WordParsingState.Init;
            int appendLength = 0;


            for (int i = 0; i < buffLength; ++i)
            {
                char c0 = textBuffer[i];
                //switch by state
                switch (parsingState)
                {
                    case WordParsingState.Init:
                        {
                            if (char.IsWhiteSpace(c0))
                            {
                                parsingState = WordParsingState.Whitespace;
                                appendLength = 1;
                            }
                            else
                            {
                                parsingState = WordParsingState.CharacterCollecting;
                                startIndex = i;
                                appendLength = 1;//start append length
                            }
                        } break;
                    case WordParsingState.Whitespace:
                        {
                            if (!char.IsWhiteSpace(c0))
                            {
                                //flush whitespace
                                //switch to character mode    
                                if (keepPreWhitespace || boxRuns.Count > 0)
                                {
                                    boxRuns.Add(CssTextRun.CreateSingleWhitespace(box));
                                }
                                parsingState = WordParsingState.CharacterCollecting;
                                startIndex = i;
                                appendLength = 1;//start append length
                            }
                        } break;
                    case WordParsingState.CharacterCollecting:
                        {
                            if (char.IsWhiteSpace(c0))
                            {
                                //flush new token   
                                boxRuns.Add(CssTextRun.CreateTextRun(box, startIndex, appendLength));

                                parsingState = WordParsingState.Whitespace;
                                appendLength = 1; //clear append length 
                            }
                            else
                            {
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
                            if (keepPreWhitespace || boxRuns.Count > 0)
                            {
                                boxRuns.Add(CssTextRun.CreateSingleWhitespace(box));
                            }

                        } break;
                    case WordParsingState.CharacterCollecting:
                        {
                            boxRuns.Add(CssTextRun.CreateTextRun(box, startIndex, appendLength));
                            appendLength = 0; //clear append length 
                        } break;
                }
            }
            //--------------------  
        }

        void ParseWordContentPreserveWhitespace(CssBox box)
        {

            char[] textBuffer = CssBox.UnsafeGetTextBuffer(box);
            //#if DEBUG
            //            string dbugStr = new string(textBuffer);
            //#endif
            List<CssRun> boxRuns = CssBox.UnsafeGetRunListOrCreateIfNotExists(box);
            boxRuns.Clear(); //clear prev results

            int startIndex = 0;
            int buffLength = textBuffer.Length;
            bool boxIsNotBreakAll = box.WordBreak != CssWordBreak.BreakAll;

            //whitespace and respect newline  
            WordParsingState parsingState = WordParsingState.Init;
            int appendLength = 0;



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
                            boxRuns.Add(CssTextRun.CreateTextRun(box, startIndex, appendLength));
                        }
                        else
                        {
                            boxRuns.Add(CssTextRun.CreateWhitespace(box, appendLength));
                        }
                        appendLength = 0; //clear append length 
                    }
                    //append with new line 
                    boxRuns.Add(CssTextRun.CreateLineBreak(box));
                    startIndex = i;
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
                                boxRuns.Add(CssTextRun.CreateWhitespace(box, appendLength));

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
                                boxRuns.Add(CssTextRun.CreateTextRun(box, startIndex, appendLength));

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
                            boxRuns.Add(CssTextRun.CreateWhitespace(box, appendLength));


                        } break;
                    case WordParsingState.CharacterCollecting:
                        {
                            boxRuns.Add(CssTextRun.CreateTextRun(box, startIndex, appendLength));


                        } break;
                }
            }
        }
        void ParseWordContentRespectNewLine(CssBox box)
        {
            //not preserve whitespace but respect newline
            char[] textBuffer = CssBox.UnsafeGetTextBuffer(box);
            List<CssRun> boxRuns = CssBox.UnsafeGetRunListOrCreateIfNotExists(box);
            boxRuns.Clear(); //clear prev results

            int startIndex = 0;
            int buffLength = textBuffer.Length;
            bool boxIsNotBreakAll = box.WordBreak != CssWordBreak.BreakAll;

            //whitespace and respect newline  
            WordParsingState parsingState = WordParsingState.Init;

            int appendLength = 0;

            for (int i = 0; i < buffLength; ++i)
            {
                char c0 = textBuffer[i];
                //----------------------------------------
                if (c0 == '\n')
                {
                    //flush exising 
                    if (appendLength > 0)
                    {
                        switch (parsingState)
                        {
                            case WordParsingState.Whitespace:
                                {
                                    if (boxRuns.Count > 0)
                                    {
                                        boxRuns.Add(CssTextRun.CreateSingleWhitespace(box));
                                    }
                                } break;
                            case WordParsingState.CharacterCollecting:
                                {
                                    boxRuns.Add(CssTextRun.CreateTextRun(box, startIndex, appendLength));

                                } break;
                        }
                        appendLength = 0; //clear append length 
                    }
                    //append with new line
                    startIndex = i;
                    boxRuns.Add(CssTextRun.CreateLineBreak(box));

                    continue;
                }
                else if (c0 == '\r')
                {
                    //skip 
                    continue;
                }
                //=====================================================================
                //switch by state -- like normal
                switch (parsingState)
                {
                    case WordParsingState.Init:
                        {
                            if (char.IsWhiteSpace(c0))
                            {
                                //not switch
                                parsingState = WordParsingState.Whitespace;
                            }
                            else
                            {
                                parsingState = WordParsingState.CharacterCollecting;
                                startIndex = i;
                                appendLength = 1;//start append length
                            }
                        } break;
                    case WordParsingState.Whitespace:
                        {
                            if (!char.IsWhiteSpace(c0))
                            {
                                //switch to character mode

                                if (boxRuns.Count > 0)
                                {
                                    boxRuns.Add(CssTextRun.CreateSingleWhitespace(box));
                                }

                                parsingState = WordParsingState.CharacterCollecting;


                                startIndex = i;
                                appendLength = 1;//start append length
                            }
                        } break;
                    case WordParsingState.CharacterCollecting:
                        {
                            if (char.IsWhiteSpace(c0))
                            {
                                //flush new token  
                                boxRuns.Add(CssTextRun.CreateTextRun(box, startIndex, appendLength));

                                parsingState = WordParsingState.Whitespace;
                                appendLength = 0; //clear append length 
                            }
                            else
                            {
                                appendLength++;
                            }
                        } break;
                }
            }
            //--------------------
            //last one
            if (appendLength > 0)
            {
                switch (parsingState)
                {
                    case WordParsingState.Whitespace:
                        {
                            if (boxRuns.Count > 0)
                            {
                                boxRuns.Add(CssTextRun.CreateSingleWhitespace(box));
                            }
                        } break;
                    case WordParsingState.CharacterCollecting:
                        {
                            boxRuns.Add(CssTextRun.CreateTextRun(box, startIndex, appendLength));

                            appendLength = 0; //clear append length 
                        } break;
                }
            }
        }
    }
}