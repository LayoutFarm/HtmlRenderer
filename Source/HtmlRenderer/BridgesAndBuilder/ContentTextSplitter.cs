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


        public const int P_TEXT = (int)TextSplitPartKind.Text << 13;
        public const int P_WHITESPACE = (int)TextSplitPartKind.Whitespace << 13;
        public const int P_SINGLE_WHITESPACE = (int)TextSplitPartKind.SingleWhitespace << 13;
        public const int P_LINEBREAK = (int)TextSplitPartKind.LineBreak << 13;
        public const int LEN_MASK = ~(7 << 13);

        public void ParseWordContent(char[] textBuffer,
            ushort[] splitBuffer,
            out int splitBufferLen,
            out bool hasSomeCharacter)
        {

            hasSomeCharacter = false;
            //--------------------------------------
            //just parse and preserve all whitespace
            //--------------------------------------  
            int startIndex = 0;
            int buffLength = textBuffer.Length;
            //whitespace and respect newline  
            WordParsingState parsingState = WordParsingState.Init;
            int appendLength = 0;

            //init 
            splitBufferLen = 0;
            int cIndex = 0;

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
                            splitBuffer[cIndex] = (ushort)(P_TEXT | (LEN_MASK & appendLength));
                            cIndex++;
                            hasSomeCharacter = true;
                        }
                        else
                        {
                            splitBuffer[cIndex] = (ushort)(P_WHITESPACE | (LEN_MASK & appendLength));
                            cIndex++;

                        }
                        appendLength = 0; //clear append length 
                    }
                    //append with new line   

                    splitBuffer[cIndex] = (ushort)(P_LINEBREAK | (LEN_MASK & 1));
                    cIndex++;

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

                                splitBuffer[cIndex] = (ushort)(P_WHITESPACE | (LEN_MASK & appendLength));
                                cIndex++;

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

                                splitBuffer[cIndex] = (ushort)(P_TEXT | (LEN_MASK & appendLength));
                                cIndex++;

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


                            splitBuffer[cIndex] = (ushort)(P_WHITESPACE | (LEN_MASK & appendLength));
                            cIndex++;

                        } break;
                    case WordParsingState.CharacterCollecting:
                        {

                            splitBuffer[cIndex] = (ushort)(P_TEXT | (LEN_MASK & appendLength));
                            cIndex++;

                            hasSomeCharacter = true;

                        } break;
                }
            }
            splitBufferLen = cIndex;
            ////--------------------
            //if (hasSomeCharacter)
            //{
            //    return new TextSplits(0, spList.ToArray());
            //}
            //else
            //{
            //    return new TextSplits(1, null);
            //}

        }

    }
}