//BSD 2014,WinterDev 

 
using System.Collections.Generic;
 

namespace HtmlRenderer.Dom
{



    class ContentTextSplitter
    {
         
        public ContentTextSplitter()
        {

        }
        enum WordParsingState
        {
            Init,
            Whitespace,
            CharacterCollecting
        }

        //---------------------
        static TextSplitPart CreateTextPart(int startIndex, int appendLength)
        {
            return new TextSplitPart(startIndex, appendLength, TextSplitPartKind.Text);
        }
        static TextSplitPart CreateLineBreak()
        {
            return new TextSplitPart(0, 1, TextSplitPartKind.LineBreak);
        }
        static TextSplitPart CreateWhitespace(int appendLength)
        {
            if (appendLength == 1)
            {
                return new TextSplitPart(0, appendLength, TextSplitPartKind.SingleWhitespace);
            }
            else
            {
                return new TextSplitPart(0, appendLength, TextSplitPartKind.Whitespace);
            }
        }
        //---------------------


        public List<TextSplitPart> ParseWordContent(char[] textBuffer)
        {

            //--------------------------------------
            //just parse and preserve all whitespace
            //--------------------------------------

            List<TextSplitPart> spList = new List<TextSplitPart>();


            int startIndex = 0;
            int buffLength = textBuffer.Length;

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
                            spList.Add(CreateTextPart(startIndex, appendLength));
                        }
                        else
                        {
                            spList.Add(CreateWhitespace(appendLength));
                        }
                        appendLength = 0; //clear append length 
                    }
                    //append with new line 
                    spList.Add(CreateLineBreak());
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
                                spList.Add(CreateWhitespace(appendLength));
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
                                spList.Add(CreateTextPart(startIndex, appendLength));
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
                            spList.Add(CreateWhitespace(appendLength));

                        } break;
                    case WordParsingState.CharacterCollecting:
                        {
                            spList.Add(CreateTextPart(startIndex, appendLength));

                        } break;
                }
            }
            return spList;
        } 

       
    
    }
}