//BSD 2014,WinterDev 


using System.Collections.Generic;


namespace HtmlRenderer.Dom
{



    class ContentTextSplitter
    {
        List<ushort> _spList = new List<ushort>(50);
        public ContentTextSplitter()
        { 
        }
        enum WordParsingState
        {
            Init,
            Whitespace,
            CharacterCollecting
        } 

        public const int P_TEXT = (int)TextSplitPartKind.Text << 13;
        public const int P_WHITESPACE = (int)TextSplitPartKind.Whitespace << 13;
        public const int P_SINGLE_WHITESPACE = (int)TextSplitPartKind.SingleWhitespace << 13;
        public const int P_LINEBREAK = (int)TextSplitPartKind.LineBreak << 13;
        public const int LEN_MASK = ~(7 << 13);

        public TextSplits ParseWordContent(char[] textBuffer, out bool hasSomeCharacter)
        {

            hasSomeCharacter = false; 
            //--------------------------------------
            //just parse and preserve all whitespace
            //--------------------------------------  
            int startIndex = 0;
            int buffLength = textBuffer.Length; 
            //whitespace and respect newline  
            WordParsingState parsingState = WordParsingState.Init;
            
            var spList = _spList;
            int appendLength = 0;

            spList.Clear();           


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
                            spList.Add((ushort)(P_TEXT | (LEN_MASK & appendLength)));
                            hasSomeCharacter = true;
                        }
                        else
                        {
                            spList.Add((ushort)(P_WHITESPACE | (LEN_MASK & appendLength)));
                        }
                        appendLength = 0; //clear append length 
                    }
                    //append with new line  
                    spList.Add((ushort)(P_LINEBREAK | (LEN_MASK & 1)));
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
                                spList.Add((ushort)(P_WHITESPACE | (LEN_MASK & appendLength)));
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
                                spList.Add((ushort)(P_TEXT | (LEN_MASK & appendLength)));
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
                            spList.Add((ushort)(P_WHITESPACE | (LEN_MASK & appendLength)));

                        } break;
                    case WordParsingState.CharacterCollecting:
                        {
                            spList.Add((ushort)(P_TEXT | (LEN_MASK & appendLength)));
                            hasSomeCharacter = true;

                        } break;
                }
            }
            //--------------------
            if (hasSomeCharacter)            
            {
                return new TextSplits(0, spList.ToArray());
            }
            else
            {   
                return new TextSplits(1, null);
            }
            
        }

    }
}