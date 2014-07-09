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
            AddRunList(toBox, spec, textnode.InternalGetRuns(), textnode.GetOriginalBuffer());
        }
        //---------------------------------------------------------------------------------------
        public static void AddRunList(CssBox toBox,
            BoxSpec spec,
            List<CssRun> runlist,
            char[] buffer)
        {
            toBox.SetTextBuffer(buffer);

            switch (spec.WhiteSpace)
            {
                case CssWhiteSpace.Pre:
                case CssWhiteSpace.PreWrap:
                    //run and preserve whitespace 
                    //CreateRunsPreserveWhitespace(buffer, originalSplitParts, toBox, spec.WordBreak != CssWordBreak.BreakAll);
                    for (int i = runlist.Count - 1; i >= 0; --i)
                    {
                        runlist[i].SetOwner(toBox);
                    }
                    toBox.SetContentRuns(runlist, toBox.HtmlElement != null);


                    break;
                case CssWhiteSpace.PreLine:
                    for (int i = runlist.Count - 1; i >= 0; --i)
                    {
                        runlist[i].SetOwner(toBox);
                    }
                    toBox.SetContentRuns(runlist, toBox.HtmlElement != null);

                    //CreateRunsRespectNewLine(buffer, originalSplitParts, toBox, spec.WordBreak != CssWordBreak.BreakAll);
                    break;
                default:

                    for (int i = runlist.Count - 1; i >= 0; --i)
                    {  
                        CssRun run = runlist[i];
                        run.SetOwner(toBox);
                        switch (run.Kind)
                        {
                            case CssRunKind.LineBreak:
                                {
                                    //skip linebreak
                                    CssTextRun trun = (CssTextRun)run;
                                    trun.MakeLength1();
                                    //Console.WriteLine(trun.Text);
                                } break;
                            case CssRunKind.SingleSpace:
                            case CssRunKind.Space:
                                {
                                    //make sure 1 
                                    CssTextRun trun = (CssTextRun)run;
                                    trun.MakeLength1();
                                    //Console.WriteLine(trun.Text);
                                } break;
                            case CssRunKind.Text:
                                {
                                    CssTextRun trun = (CssTextRun)run;
                                    //Console.WriteLine(trun.Text);
                                } break;
                        } 
                    }

                    toBox.SetContentRuns(runlist, toBox.HtmlElement != null);

                    //CreateRunsDefault(buffer, originalSplitParts, toBox, spec.WordBreak != CssWordBreak.BreakAll, toBox.HtmlElement != null);
                    break;
            }
        }
        //------------------------------------------






        ////=====================================================================
        ///// <summary>
        ///// whitespace and respect newline  
        ///// </summary>
        ///// <param name="textBuffer"></param>
        ///// <param name="boxIsNotBreakAll"></param>
        ///// <returns></returns>
        //static void CreateRunsPreserveWhitespace(
        //    char[] buffer,
        //    List<CssRun> runlist,
        //    CssBox toBox,
        //    bool boxIsNotBreakAll)
        //{

        //    //if (originalSplitParts.isWS)
        //    //{
        //    //    return;
        //    //}
        //    ////=================================================
        //    //toBox.SetTextBuffer(buffer);

        //    //ushort[] encodingSplits = originalSplitParts.encodedSplits;

        //    //bool hasSomeChar = false;
        //    //List<CssRun> boxRuns = new List<CssRun>();
        //    //int j = encodingSplits.Length;
        //    //int startIndex = 0;

        //    //for (int i = 0; i < j; ++i)
        //    //{
        //    //    ushort p = encodingSplits[i];
        //    //    int len = (p & ContentTextSplitter.LEN_MASK);

        //    //    CssRun r = null;
        //    //    switch ((TextSplitPartKind)(p >> 13))
        //    //    {
        //    //        case TextSplitPartKind.LineBreak:
        //    //            {
        //    //                r = CssTextRun.CreateLineBreak();
        //    //            } break;
        //    //        case TextSplitPartKind.SingleWhitespace:
        //    //            {
        //    //                r = CssTextRun.CreateSingleWhitespace();
        //    //            } break;
        //    //        case TextSplitPartKind.Whitespace:
        //    //            {
        //    //                r = CssTextRun.CreateWhitespace(len);
        //    //            } break;
        //    //        case TextSplitPartKind.Text:
        //    //            {
        //    //                r = CssTextRun.CreateTextRun(startIndex, len);
        //    //                hasSomeChar = true;
        //    //            } break;
        //    //        default:
        //    //            {
        //    //                throw new System.NotSupportedException();
        //    //            }
        //    //    }
        //    //    r.SetOwner(toBox);
        //    //    boxRuns.Add(r);
        //    //    startIndex += len;
        //    //}

        //    toBox.SetContentRuns(boxRuns, hasSomeChar);
        //}

        ///// <summary>
        ///// not preserve whitespace but respect newline
        ///// </summary>
        ///// <param name="textBuffer"></param>
        ///// <param name="boxIsNotBreakAll"></param>
        ///// <returns></returns>
        //static void CreateRunsRespectNewLine(
        //    char[] buffer,
        //    List<CssRun> runlist,
        //    CssBox toBox,
        //    bool boxIsNotBreakAll)
        //{

        //    //if (originalSplitParts.isWS)
        //    //{
        //    //    return;
        //    //}
        //    ////--------------------------------------------------
        //    //toBox.SetTextBuffer(buffer);
        //    //List<CssRun> boxRuns = new List<CssRun>();
        //    //bool hasSomeChar = false;
        //    //ushort[] encodedSplits = originalSplitParts.encodedSplits;
        //    //int j = encodedSplits.Length;

        //    //int startIndex = 0;
        //    //for (int i = 0; i < j; ++i)
        //    //{
        //    //    ushort p = encodedSplits[i];
        //    //    int len = (p & ContentTextSplitter.LEN_MASK);
        //    //    CssRun r = null;
        //    //    switch ((TextSplitPartKind)(p >> 13))
        //    //    {
        //    //        case TextSplitPartKind.LineBreak:
        //    //            {
        //    //                r = CssTextRun.CreateLineBreak();
        //    //            } break;
        //    //        case TextSplitPartKind.Whitespace:
        //    //        case TextSplitPartKind.SingleWhitespace:
        //    //            {
        //    //                if (i > 0)
        //    //                {
        //    //                    r = CssTextRun.CreateSingleWhitespace();
        //    //                }
        //    //                else
        //    //                {
        //    //                    startIndex += len;
        //    //                    continue;
        //    //                }
        //    //            } break;
        //    //        case TextSplitPartKind.Text:
        //    //            {
        //    //                r = CssTextRun.CreateTextRun(startIndex, len);
        //    //                hasSomeChar = true;
        //    //            } break;
        //    //    }
        //    //    startIndex += len;
        //    //    r.SetOwner(toBox);
        //    //    boxRuns.Add(r);
        //    //}
        //    toBox.SetContentRuns(boxRuns, hasSomeChar);
        //}
        //static void CreateRunsDefault(
        //    char[] buffer,
        //    List<CssRun> runlist,
        //    CssBox toBox,
        //    bool boxIsNotBreakAll, bool keepPreWhiteSpace)
        //{

        //    //if (originalSplitParts.isWS)
        //    //{
        //    //    return;
        //    //}


        //    //toBox.SetTextBuffer(buffer);
        //    //List<CssRun> boxRuns = new List<CssRun>();
        //    //bool hasSomeChar = false;
        //    //ushort[] encodedSplits = originalSplitParts.encodedSplits;
        //    //int j = encodedSplits.Length;

        //    //int startIndex = 0;
        //    //for (int i = 0; i < j; ++i)
        //    //{
        //    //    ushort p = encodedSplits[i];
        //    //    int len = (p & ContentTextSplitter.LEN_MASK);
        //    //    CssRun r = null;
        //    //    switch ((TextSplitPartKind)(p >> 13))
        //    //    {
        //    //        case TextSplitPartKind.LineBreak:
        //    //            {

        //    //                //skip line break
        //    //                startIndex += len;
        //    //                continue;
        //    //            }
        //    //        case TextSplitPartKind.Whitespace:
        //    //        case TextSplitPartKind.SingleWhitespace:
        //    //            {
        //    //                if (i > 0)
        //    //                {
        //    //                    r = CssTextRun.CreateSingleWhitespace();
        //    //                }
        //    //                else
        //    //                {
        //    //                    startIndex += len;
        //    //                    continue;
        //    //                }
        //    //            } break;
        //    //        case TextSplitPartKind.Text:
        //    //            {
        //    //                r = CssTextRun.CreateTextRun(startIndex, len);
        //    //                r.SetOwner(toBox);
        //    //                hasSomeChar = true;
        //    //            } break;
        //    //    }

        //    //    startIndex += len;
        //    //    boxRuns.Add(r);
        //    //}
        //    toBox.SetContentRuns(boxRuns, hasSomeChar);
        //}

    }
}