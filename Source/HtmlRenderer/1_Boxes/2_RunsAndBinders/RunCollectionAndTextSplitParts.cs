//BSD 2014,WinterDev
using System.Collections.Generic;

namespace HtmlRenderer.Dom
{
    enum TextSplitPartKind : byte
    {
        Text,
        Whitespace,
        SingleWhitespace,
        LineBreak,
    }
    struct TextSplitPart
    {


        public readonly int startIndex;
        public readonly int length;
        public readonly TextSplitPartKind kind;
        public TextSplitPart(int startIndex, int length, TextSplitPartKind kind)
        {
            this.startIndex = startIndex;
            this.length = length;
            this.kind = kind;


#if DEBUG
            this.dbugId = dbugTotalId++;

            if (length == 0)
            {

            }
#endif


        }

#if DEBUG
        public readonly int dbugId;
        static int dbugTotalId;
        public override string ToString()
        {
            return kind.ToString() + "(" + startIndex + "," + length + ")";
        }
#endif
    }

    //====================================================================================================
    class RunCollection
    {



        bool isWhiteSpace;

        bool isSingleRun;
        bool runListCreated;
        CssWhiteSpace whitespace;
        CssWordBreak wordBreak;

        CssBox ownerBox;

        char[] originalBuffer;
        List<CssRun> runList = new List<CssRun>();
        List<TextSplitPart> originalSplitParts;

        public RunCollection(char[] originalBuffer, List<TextSplitPart> originalSplitParts)
        {
            this.originalSplitParts = originalSplitParts;
            this.originalBuffer = originalBuffer;

            EvaluateWhitespace();
        }
        public RunCollection(CssRun singleRun)
        {
            singleRun.SetOwner(this);
            this.originalSplitParts = new List<TextSplitPart>(1);
            this.originalSplitParts.Add(new TextSplitPart(0, 1, TextSplitPartKind.Text));
            this.runList.Add(singleRun);
            isSingleRun = true;
        }

        internal char[] GetOriginalBuffer()
        {
            return this.originalBuffer;
        }
        internal CssBox OwnerCssBox
        {
            get { return this.ownerBox; }
            set { this.ownerBox = value; }
        }
        internal List<CssRun> GetInternalList()
        {
            return this.runList;
        }
        public int RunCount
        {
            get { return this.runList.Count; }
        }

        internal void UpdateRunList(CssWhiteSpace whitespace, CssWordBreak wordBreak, bool keepPreWhiteSpace)
        {
            if (!this.isSingleRun)
            {
                //re-create if nessesary

                if (!this.runListCreated ||
                    this.wordBreak != wordBreak ||
                    this.whitespace != whitespace)
                {
                    runList.Clear();

                    CreateRuns(this,
                       runList,
                       whitespace,
                       wordBreak != CssWordBreak.BreakAll,
                       keepPreWhiteSpace);

                    this.runListCreated = true;
                    this.wordBreak = wordBreak;
                    this.whitespace = whitespace;
                }

            }
        }
        void EvaluateWhitespace()
        {
            isWhiteSpace = false;

            if (this.isSingleRun)
            {
                return;
            }
            var tmpSplits = originalSplitParts;
            for (int i = tmpSplits.Count - 1; i >= 0; --i)
            {
                if (tmpSplits[i].kind == TextSplitPartKind.Text)
                {
                    //stop and return if found text
                    return;
                }
            }

            //exit here means all char is whitespace
            isWhiteSpace = true;
        }
        internal bool IsWhiteSpace
        {
            get { return this.isWhiteSpace; }
        }

        //==========================================================================================
        //whitespace split

        static void CreateRuns(
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
                    CreateRunsPreserveWhitespace(runList, collection, notBreakAll);
                    break;
                case CssWhiteSpace.PreLine:
                    CreateRunsRespectNewLine(runList, collection, notBreakAll);
                    break;
                default:
                    CreateRunsDefault(runList, collection, notBreakAll, keepPreWhiteSpace);
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
            RunCollection collection,
            bool boxIsNotBreakAll)
        {
            var originalSplitParts = collection.originalSplitParts;
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
            RunCollection collection,
            bool boxIsNotBreakAll)
        {
            var originalSplitParts = collection.originalSplitParts;
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
        static void CreateRunsDefault(List<CssRun> boxRuns, RunCollection collection,
            bool boxIsNotBreakAll, bool keepPreWhiteSpace)
        {
            var originalSplitParts = collection.originalSplitParts;
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





}