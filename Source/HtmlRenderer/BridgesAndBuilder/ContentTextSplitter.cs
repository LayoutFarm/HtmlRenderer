//BSD 2014,WinterDev 


using System.Collections.Generic;


namespace HtmlRenderer.Dom
{


    static class RunMaker
    {
        //==========================================================================================
        //whitespace split

        public static void CreateRuns(
            List<TextSplitPart> originalSplitParts,
            RunCollection collection,
            List<CssRun> runList,
            CssWhiteSpace whitespace,
            bool notBreakAll,
            bool keepPreWhiteSpace)
        {
            switch (whitespace)
            {
                case CssWhiteSpace.Pre:
                case CssWhiteSpace.PreWrap:
                    CreateRunsPreserveWhitespace(runList, originalSplitParts, collection, notBreakAll);
                    break;
                case CssWhiteSpace.PreLine:
                    CreateRunsRespectNewLine(runList, originalSplitParts, collection, notBreakAll);
                    break;
                default:
                    CreateRunsDefault(runList, originalSplitParts, collection, notBreakAll, keepPreWhiteSpace);
                    break;
            }
        }

        /// <summary>
        /// whitespace and respect newline  
        /// </summary>
        /// <param name="textBuffer"></param>
        /// <param name="boxIsNotBreakAll"></param>
        /// <returns></returns>
        static void CreateRunsPreserveWhitespace(
            List<CssRun> boxRuns,
            List<TextSplitPart> originalSplitParts,
            RunCollection collection,
            bool boxIsNotBreakAll)
        {

            int j = originalSplitParts.Count;

            for (int i = 0; i < j; ++i)
            {
                TextSplitPart p = originalSplitParts[i];
                CssRun r = null;
                switch (p.kind)
                {
                    case TextSplitPartKind.LineBreak:
                        {
                            r = CssTextRun.CreateLineBreak();

                        } break;
                    case TextSplitPartKind.SingleWhitespace:
                        {
                            r = CssTextRun.CreateSingleWhitespace();
                        } break;
                    case TextSplitPartKind.Whitespace:
                        {
                            r = CssTextRun.CreateWhitespace(p.length);
                        } break;
                    case TextSplitPartKind.Text:
                        {
                            r = CssTextRun.CreateTextRun(p.startIndex, p.length);

                        } break;
                    default:
                        {
                            throw new System.NotSupportedException();
                        }
                }
                r.SetOwner(collection);
                boxRuns.Add(r);
            }
        }

        /// <summary>
        /// not preserve whitespace but respect newline
        /// </summary>
        /// <param name="textBuffer"></param>
        /// <param name="boxIsNotBreakAll"></param>
        /// <returns></returns>
        static void CreateRunsRespectNewLine(List<CssRun> boxRuns,
            List<TextSplitPart> originalSplitParts,
            RunCollection collection,
            bool boxIsNotBreakAll)
        {

            int j = originalSplitParts.Count;

            for (int i = 0; i < j; ++i)
            {
                TextSplitPart p = originalSplitParts[i];
                CssRun r = null;
                switch (p.kind)
                {
                    case TextSplitPartKind.LineBreak:
                        {
                            r = CssTextRun.CreateLineBreak();

                        } break;
                    case TextSplitPartKind.Whitespace:
                    case TextSplitPartKind.SingleWhitespace:
                        {
                            //a whitespace of any size is collapse to single whitespace
                            if (i > 0)
                            {
                                r = CssTextRun.CreateSingleWhitespace();
                            }
                            else
                            {
                                continue;
                            }
                        } break;
                    case TextSplitPartKind.Text:
                        {
                            r = CssTextRun.CreateTextRun(p.startIndex, p.length);

                        } break;
                    default:
                        {
                            throw new System.NotSupportedException();
                        }
                }

                r.SetOwner(collection);
                boxRuns.Add(r);
            }


        }
        static void CreateRunsDefault(List<CssRun> boxRuns,
            List<TextSplitPart> originalSplitParts,
            RunCollection collection,
            bool boxIsNotBreakAll, bool keepPreWhiteSpace)
        {

            int j = originalSplitParts.Count;

            for (int i = 0; i < j; ++i)
            {
                TextSplitPart p = originalSplitParts[i];
                CssRun r = null;
                switch (p.kind)
                {
                    case TextSplitPartKind.LineBreak:
                        {
                            //not include line break
                            continue;
                        }
                    case TextSplitPartKind.Whitespace:
                    case TextSplitPartKind.SingleWhitespace:
                        {
                            //a whitespace of any size is collapse to single whitespace
                            if (i > 0 || keepPreWhiteSpace)
                            {
                                r = CssTextRun.CreateSingleWhitespace();
                            }
                            else
                            {
                                continue;
                            }
                        } break;
                    case TextSplitPartKind.Text:
                        {
                            r = CssTextRun.CreateTextRun(p.startIndex, p.length);

                        } break;
                    default:
                        {
                            throw new System.NotSupportedException();
                        }
                }

                r.SetOwner(collection);
                boxRuns.Add(r);
            }

        }

    }
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



        public void ParseWordContent2(
            RunCollection collection,
            char[] textBuffer,
            CssWhiteSpace whitespace,
            CssWordBreak wordBreak)
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


            List<CssRun> cssRuns = new List<CssRun>(); 
            
            RunMaker.CreateRuns(spList, collection, cssRuns,
                whitespace,
                wordBreak != CssWordBreak.BreakAll,
                false); 
            
        }


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