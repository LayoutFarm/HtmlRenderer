//BSD 2014,WinterCore 

using System;
using System.Collections.Generic;
using HtmlRenderer.Parse;
using HtmlRenderer.Utils;
using System.Text;

namespace HtmlRenderer.Dom
{

    class CssTextSplitter
    {
        public static readonly CssTextSplitter DefaultSplitter = new CssTextSplitter();
        public CssTextSplitter()
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

            List<CssRect> boxWords = box.Words;
            int startIndex = 0;
            int buffLength = textBuffer.Length;
            bool boxIsNotBreakAll = box.WordBreak != CssWordBreak.BreakAll;

            //not preserve whitespace
            WordParsingState parsingState = WordParsingState.Init;
            int appendLength = 0;
            bool lastBoxGenIsWhitespace = false;


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
                                //not switch                                 
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
                                lastBoxGenIsWhitespace = true;
                                //if (boxGenNumber > 0)
                                //{
                                //    //boxWords.Add(CssRectWord.CreateSingleWhitespace(box, (boxGenNumber == 0), false));
                                //    boxGenNumber++;
                                //}
                                //switch to character mode    
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
                                boxWords.Add(CssRectWord.CreateRefText(box, startIndex, appendLength, lastBoxGenIsWhitespace, true));
                                lastBoxGenIsWhitespace = false;
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
            if (appendLength > 0)
            {
                switch (parsingState)
                {
                    case WordParsingState.Whitespace:
                        {
                        } break;
                    case WordParsingState.CharacterCollecting:
                        {
                            boxWords.Add(CssRectWord.CreateRefText(box, startIndex, appendLength, lastBoxGenIsWhitespace, false));
                            lastBoxGenIsWhitespace = false;
                            appendLength = 0; //clear append length 
                        } break;
                }
            }
            //--------------------  
        }

        void ParseWordContentPreserveWhitespace(CssBox box)
        {

            char[] textBuffer = CssBox.UnsafeGetTextBuffer(box);
            List<CssRect> boxWords = box.Words;
            int startIndex = 0;
            int buffLength = textBuffer.Length;
            bool boxIsNotBreakAll = box.WordBreak != CssWordBreak.BreakAll;

            //whitespace and respect newline  
            WordParsingState parsingState = WordParsingState.Init;
            int appendLength = 0;

            bool lastBoxIsWhitespace = false;

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
                            boxWords.Add(CssRectWord.CreateRefText(box, startIndex, appendLength, lastBoxIsWhitespace, false));

                            lastBoxIsWhitespace = false;
                        }
                        else
                        {
                            boxWords.Add(CssRectWord.CreateWhitespace(box, appendLength, lastBoxIsWhitespace, false));

                            lastBoxIsWhitespace = true;
                        }
                        appendLength = 0; //clear append length 
                    }
                    //append with new line
                    startIndex = i;
                    boxWords.Add(CssRectWord.CreateLineBreak(box, false, false));

                    lastBoxIsWhitespace = false;
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
                                boxWords.Add(CssRectWord.CreateWhitespace(box, appendLength, false, false));
                                lastBoxIsWhitespace = true;

                                parsingState = WordParsingState.CharacterCollecting;
                                startIndex = i;//start collect
                                appendLength = 1;//start append length
                            }
                            else
                            {
                                appendLength++;
                            }
                        } break;
                    case WordParsingState.CharacterCollecting:
                        {
                            if (char.IsWhiteSpace(c0))
                            {
                                //flush collecting token
                                boxWords.Add(CssRectWord.CreateRefText(box, startIndex, appendLength, lastBoxIsWhitespace, true));
                                lastBoxIsWhitespace = false;


                                parsingState = WordParsingState.Whitespace;
                                startIndex = i;//start collect
                                appendLength = 1; //collect whitespace
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
                            boxWords.Add(CssRectWord.CreateWhitespace(box, appendLength, lastBoxIsWhitespace, false));

                            lastBoxIsWhitespace = true;
                        } break;
                    case WordParsingState.CharacterCollecting:
                        {
                            boxWords.Add(CssRectWord.CreateRefText(box, startIndex, appendLength, lastBoxIsWhitespace, false));

                            lastBoxIsWhitespace = false;
                        } break;
                }
            }
        }
        void ParseWordContentRespectNewLine(CssBox box)
        {
            //not preserve whitespace but respect newline
            char[] textBuffer = CssBox.UnsafeGetTextBuffer(box);
            List<CssRect> boxWords = box.Words;
            int startIndex = 0;
            int buffLength = textBuffer.Length;
            bool boxIsNotBreakAll = box.WordBreak != CssWordBreak.BreakAll;

            //whitespace and respect newline  
            WordParsingState parsingState = WordParsingState.Init;

            int appendLength = 0;
            bool lastBoxIsWhitespace = false;
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
                                } break;
                            case WordParsingState.CharacterCollecting:
                                {
                                    boxWords.Add(CssRectWord.CreateRefText(box, startIndex, appendLength, lastBoxIsWhitespace, false));
                                    lastBoxIsWhitespace = false;

                                } break;
                        }
                        appendLength = 0; //clear append length 
                    }
                    //append with new line
                    startIndex = i;
                    boxWords.Add(CssRectWord.CreateLineBreak(box, false, false));
                    lastBoxIsWhitespace = false;

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
                                //parsingState = WordParsingState.Whitespace;
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
                                lastBoxIsWhitespace = true;
                                parsingState = WordParsingState.CharacterCollecting;
                                //if (boxGenNumberInEachLine > 0)
                                //{
                                //    lastBoxIsWhitespace = true;
                                //    boxGenNumberInEachLine++;
                                //    parsingState = WordParsingState.CharacterCollecting;
                                //}

                                startIndex = i;
                                appendLength = 1;//start append length
                            }
                        } break;
                    case WordParsingState.CharacterCollecting:
                        {
                            if (char.IsWhiteSpace(c0))
                            {
                                //flush new token  
                                boxWords.Add(CssRectWord.CreateRefText(box, startIndex, appendLength, lastBoxIsWhitespace, true));
                                lastBoxIsWhitespace = false;
                                // boxGenNumberInEachLine++;
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
                        } break;
                    case WordParsingState.CharacterCollecting:
                        {
                            boxWords.Add(CssRectWord.CreateRefText(box, startIndex, appendLength, lastBoxIsWhitespace, false));
                            lastBoxIsWhitespace = false;
                            appendLength = 0; //clear append length 
                        } break;
                }
            }
        }
    }
}