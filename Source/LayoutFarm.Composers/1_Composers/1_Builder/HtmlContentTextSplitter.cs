//BSD, 2014-2017, WinterDev

using System.Collections.Generic;
using LayoutFarm.Css;
using LayoutFarm.HtmlBoxes;
using PixelFarm.Drawing.Text;

namespace LayoutFarm.Composers
{

    public interface ITextBreaker
    {
        void DoBreak(char[] inputBuffer, int startIndex, int len, List<int> breakAtList);
    }

    /// <summary>
    /// parse html text content, 
    /// not include some consecutive text 
    /// that need more special parser
    /// </summary>
    class HtmlContentTextSplitter
    {
        List<int> breakAtList = new List<int>();
        public HtmlContentTextSplitter()
        {

        }
        enum WordParsingState
        {
            Init,
            Whitespace,
            CharacterCollecting
        }
        public ITextBreaker TextBreaker { get; set; }


        void CreateTextRuns(char[] textBuffer, List<CssRun> runlist, int startIndex, int appendLength)
        {

            //this may produce more than 1 css text run
            if (TextBreaker != null)
            {
                //use text break to parse more
                breakAtList.Clear();
                TextBreaker.DoBreak(textBuffer, startIndex, appendLength, breakAtList);
                int j = breakAtList.Count;
                int pos = startIndex;
                for (int i = 0; i < j; ++i)
                {
                    int sepAt = breakAtList[i];
                    runlist.Add(
                        CssTextRun.CreateTextRun(pos, sepAt - pos));
                    pos = sepAt;
                }
                breakAtList.Clear();
                //TextBreaker.DoBreak(textBuffer, startIndex, appendLength, bounds =>
                //{
                //    //iterate new split 
                //    runlist.Add(
                //        CssTextRun.CreateTextRun(startIndex + bounds.startIndex, bounds.length));
                //});
            }
            else
            {
                runlist.Add(CssTextRun.CreateTextRun(startIndex, appendLength));
            }

        }

        internal void ParseWordContent(
            char[] textBuffer,
            BoxSpec spec,
            bool isBlock,
            List<CssRun> runlistOutput,
            out bool hasSomeCharacter)
        {
            bool preserverLine = false;
            bool preserveWhiteSpace = false;
            var isblock = spec.CssDisplay == CssDisplay.Block;
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

            //some text
            bool needFinerTextParser = false;

            for (int i = 0; i < buffLength; ++i)
            {
                char c0 = textBuffer[i];
                if (c0 == '\r')
                {
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
                                if (c0 == '\n')
                                {
                                    if (preserverLine)
                                    {
                                        runlistOutput.Add(CssTextRun.CreateLineBreak());
                                        appendLength = 0; //clear append length 
                                        continue;
                                    }
                                }
                                parsingState = WordParsingState.Whitespace;
                                //start collect whitespace
                                startIndex = i;
                                appendLength = 1;//start collect whitespace
                            }
                            else if (char.IsPunctuation(c0))
                            {
                                parsingState = WordParsingState.Init;
                                startIndex = i;
                                appendLength = 1;
                                //add single token
                                runlistOutput.Add(CssTextRun.CreateTextRun(startIndex, appendLength));
                            }
                            else
                            {
                                //character  
                                if (c0 > '~') { needFinerTextParser = true; }

                                parsingState = WordParsingState.CharacterCollecting;
                                startIndex = i;
                                appendLength = 1;//start collect whitespace 
                            }
                        }
                        break;
                    case WordParsingState.Whitespace:
                        {
                            if (char.IsWhiteSpace(c0))
                            {
                                if (c0 == '\n')
                                {
                                    if (preserverLine)
                                    {
                                        runlistOutput.Add(CssTextRun.CreateLineBreak());
                                        appendLength = 0; //clear append length 
                                        continue;
                                    }
                                }
                                if (appendLength == 0)
                                {
                                    startIndex = i;
                                }

                                appendLength++;
                            }
                            else
                            {
                                runlistOutput.Add(CssTextRun.CreateWhitespace(preserveWhiteSpace ? appendLength : 1));
                                if (char.IsPunctuation(c0))
                                {
                                    parsingState = WordParsingState.Init;
                                    startIndex = i;
                                    appendLength = 1;
                                    //add single token
                                    runlistOutput.Add(CssTextRun.CreateTextRun(startIndex, appendLength));
                                }
                                else
                                {
                                    if (c0 > '~') { needFinerTextParser = true; }

                                    parsingState = WordParsingState.CharacterCollecting;
                                    startIndex = i;//start collect
                                    appendLength = 1;//start append length 
                                }
                            }
                        }
                        break;
                    case WordParsingState.CharacterCollecting:
                        {
                            bool isWhiteSpace;
                            if ((isWhiteSpace = char.IsWhiteSpace(c0)) || char.IsPunctuation(c0))
                            {
                                //flush collecting token  
                                if (needFinerTextParser && TextBreaker != null)
                                {

                                    CreateTextRuns(textBuffer, runlistOutput, startIndex, appendLength);
                                    needFinerTextParser = false;//reset                                     
                                }
                                else
                                {
                                    runlistOutput.Add(CssTextRun.CreateTextRun(startIndex, appendLength));
                                }

                                hasSomeCharacter = true;
                                startIndex = i;//start collect
                                appendLength = 1; //collect whitespace
                                if (isWhiteSpace)
                                {
                                    if (c0 == '\n')
                                    {
                                        if (preserverLine)
                                        {
                                            runlistOutput.Add(CssTextRun.CreateLineBreak());
                                            appendLength = 0; //clear append length 
                                            continue;
                                        }
                                    }
                                    parsingState = WordParsingState.Whitespace;
                                }
                                else
                                {
                                    parsingState = WordParsingState.Init;
                                    runlistOutput.Add(CssTextRun.CreateTextRun(startIndex, appendLength));
                                }
                                //---------------------------------------- 
                            }
                            else
                            {
                                if (c0 > '~') { needFinerTextParser = true; }

                                if (appendLength == 0)
                                {
                                    startIndex = i;
                                }
                                appendLength++;
                            }
                        }
                        break;
                }
            }
            //--------------------

            if (appendLength > 0)
            {
                switch (parsingState)
                {
                    case WordParsingState.Whitespace:
                        {
                            //last with whitespace
                            if (preserveWhiteSpace)
                            {
                                runlistOutput.Add(CssTextRun.CreateWhitespace(appendLength));
                            }
                            else
                            {
                                if (!isblock || (runlistOutput.Count > 0))
                                {
                                    runlistOutput.Add(CssTextRun.CreateWhitespace(1));
                                }
                            }
                        }
                        break;
                    case WordParsingState.CharacterCollecting:
                        {
                            if (needFinerTextParser && TextBreaker != null)
                            {
                                CreateTextRuns(textBuffer, runlistOutput, startIndex, appendLength);
                                needFinerTextParser = false;//reset
                            }
                            else
                            {
                                runlistOutput.Add(CssTextRun.CreateTextRun(startIndex, appendLength));
                            }

                            hasSomeCharacter = true;
                        }
                        break;
                }
            }
        }
    }


    public struct TextSplitBound
    {
        public readonly int startIndex;
        public readonly int length;
        public TextSplitBound(int startIndex, int length)
        {
            this.startIndex = startIndex;
            this.length = length;
        }
    }



}