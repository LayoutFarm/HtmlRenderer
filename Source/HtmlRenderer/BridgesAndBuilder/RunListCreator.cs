//BSD 2014, WinterDev 
using System;
using System.Drawing;
using System.Collections.Generic;
using HtmlRenderer.Handlers;
using HtmlRenderer.WebDom;

namespace HtmlRenderer.Dom
{
    static class RunListCreator
    {

        public static void AddRunList(CssBox toBox, BoxSpec spec, BridgeHtmlTextNode textnode)
        {
            AddRunList(toBox, spec, textnode.GetSplitBuffer(), textnode.SplitPartCount, textnode.GetOriginalBuffer());
        }
        public static void AddRunList(CssBox toBox,
            BoxSpec spec,
            ushort[] originalSplitParts,
            int splitPartCount,
            char[] buffer)
        {

            switch (spec.WhiteSpace)
            {
                case CssWhiteSpace.Pre:
                case CssWhiteSpace.PreWrap:
                    CreateRunsPreserveWhitespace(buffer, originalSplitParts, splitPartCount, toBox, spec.WordBreak != CssWordBreak.BreakAll);
                    break;
                case CssWhiteSpace.PreLine:
                    CreateRunsRespectNewLine(buffer, originalSplitParts, splitPartCount, toBox, spec.WordBreak != CssWordBreak.BreakAll);
                    break;
                default:
                    CreateRunsDefault(buffer, originalSplitParts, splitPartCount, toBox, spec.WordBreak != CssWordBreak.BreakAll, toBox.HtmlElement != null);
                    break;
            }

        }


        //=====================================================================
        /// <summary>
        /// whitespace and respect newline  
        /// </summary>
        /// <param name="textBuffer"></param>
        /// <param name="boxIsNotBreakAll"></param>
        /// <returns></returns>
        static void CreateRunsPreserveWhitespace(
            char[] buffer,
            ushort[] encodingSplits,
            int splitPartCount,
            CssBox toBox,
            bool boxIsNotBreakAll)
        {


            //=================================================
            toBox.SetTextBuffer(buffer);

            bool hasSomeChar = false;
            List<CssRun> boxRuns = new List<CssRun>();

            int startIndex = 0;

            for (int i = 0; i < splitPartCount; ++i)
            {
                ushort p = encodingSplits[i];
                int len = (p & ContentTextSplitter.LEN_MASK);

                CssRun r = null;
                switch ((TextSplitPartKind)(p >> 13))
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
                            r = CssTextRun.CreateWhitespace(len);
                        } break;
                    case TextSplitPartKind.Text:
                        {
                            r = CssTextRun.CreateTextRun(startIndex, len);
                            hasSomeChar = true;
                        } break;
                    default:
                        {
                            throw new System.NotSupportedException();
                        }
                }
                r.SetOwner(toBox);
                boxRuns.Add(r);
                startIndex += len;
            }

            toBox.SetContentRuns(boxRuns, hasSomeChar);
        }

        /// <summary>
        /// not preserve whitespace but respect newline
        /// </summary>
        /// <param name="textBuffer"></param>
        /// <param name="boxIsNotBreakAll"></param>
        /// <returns></returns>
        static void CreateRunsRespectNewLine(
            char[] buffer,
            ushort[] encodedSplits,
            int splitPartCount,
            CssBox toBox,
            bool boxIsNotBreakAll)
        {


            //--------------------------------------------------
            toBox.SetTextBuffer(buffer);
            List<CssRun> boxRuns = new List<CssRun>();
            bool hasSomeChar = false;


            int startIndex = 0;
            for (int i = 0; i < splitPartCount; ++i)
            {
                ushort p = encodedSplits[i];
                int len = (p & ContentTextSplitter.LEN_MASK);
                CssRun r = null;
                switch ((TextSplitPartKind)(p >> 13))
                {
                    case TextSplitPartKind.LineBreak:
                        {
                            r = CssTextRun.CreateLineBreak();
                        } break;
                    case TextSplitPartKind.Whitespace:
                    case TextSplitPartKind.SingleWhitespace:
                        {
                            if (i > 0)
                            {
                                r = CssTextRun.CreateSingleWhitespace();
                            }
                            else
                            {
                                startIndex += len;
                                continue;
                            }
                        } break;
                    case TextSplitPartKind.Text:
                        {
                            r = CssTextRun.CreateTextRun(startIndex, len);
                            hasSomeChar = true;
                        } break;
                }
                startIndex += len;
                r.SetOwner(toBox);
                boxRuns.Add(r);
            }
            toBox.SetContentRuns(boxRuns, hasSomeChar);
        }
        static void CreateRunsDefault(
            char[] buffer,
            ushort[] encodedSplits,
            int splitPartCount,
            CssBox toBox,
            bool boxIsNotBreakAll, bool keepPreWhiteSpace)
        {



            toBox.SetTextBuffer(buffer);
            List<CssRun> boxRuns = new List<CssRun>();
            bool hasSomeChar = false;
            int startIndex = 0;
            for (int i = 0; i < splitPartCount; ++i)
            {
                ushort p = encodedSplits[i];
                int len = (p & ContentTextSplitter.LEN_MASK);
                CssRun r = null;
                switch ((TextSplitPartKind)(p >> 13))
                {
                    case TextSplitPartKind.LineBreak:
                        {

                            //skip line break
                            startIndex += len;
                            continue;
                        }
                    case TextSplitPartKind.Whitespace:
                    case TextSplitPartKind.SingleWhitespace:
                        {
                            if (i > 0)
                            {
                                r = CssTextRun.CreateSingleWhitespace();
                            }
                            else
                            {
                                startIndex += len;
                                continue;
                            }
                        } break;
                    case TextSplitPartKind.Text:
                        {
                            r = CssTextRun.CreateTextRun(startIndex, len);
                            r.SetOwner(toBox);
                            hasSomeChar = true;
                        } break;
                }

                startIndex += len;
                boxRuns.Add(r);
            }
            toBox.SetContentRuns(boxRuns, hasSomeChar);
        }

    }
}