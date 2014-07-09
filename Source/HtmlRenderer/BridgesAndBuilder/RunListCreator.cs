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
                    CreateRunsPreserveWhitespace(buffer, runlist, toBox, spec.WordBreak != CssWordBreak.BreakAll);
                    break;
                case CssWhiteSpace.PreLine:
                    CreateRunsRespectNewLine(buffer, runlist, toBox, spec.WordBreak != CssWordBreak.BreakAll);
                    break;
                default:
                    CreateRunsDefault(buffer, runlist, toBox, spec.WordBreak != CssWordBreak.BreakAll, toBox.HtmlElement != null);
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
            List<CssRun> runlist,
            CssBox toBox,
            bool boxIsNotBreakAll)
        {

            bool hasSomeChar = false;
            int j = runlist.Count;

            for (int i = 0; i < j; ++i)
            {
                CssRun r = runlist[i];
                r.SetOwner(toBox);
                switch (r.Kind)
                {
                    //case CssRunKind.LineBreak: 
                    //case CssRunKind.SingleSpace:
                    //case CssRunKind.Space:
                    //    { 
                    //    } break;
                    case CssRunKind.Text:
                        {
                        
                            hasSomeChar = true;

                        } break;
                    default:
                        {

                        } break;
                }
            }
            toBox.SetContentRuns(runlist, hasSomeChar);
        }

        /// <summary>
        /// not preserve whitespace but respect newline
        /// </summary>
        /// <param name="textBuffer"></param>
        /// <param name="boxIsNotBreakAll"></param>
        /// <returns></returns>
        static void CreateRunsRespectNewLine(
            char[] buffer,
            List<CssRun> runlist,
            CssBox toBox,
            bool boxIsNotBreakAll)
        {
             
            bool hasSomeChar = false;
            int j = runlist.Count; 
            for (int i = 0; i < j; ++i)
            {
                CssRun r = runlist[i];
                r.SetOwner(toBox);
                switch (r.Kind)
                {
                    case CssRunKind.LineBreak:
                        {
                            //skip line break 
                            CssTextRun trun = (CssTextRun)r;
                            trun.MakeLength1();
                            //not run newline
                            continue;
                        }
                    case CssRunKind.SingleSpace:
                    case CssRunKind.Space:
                        {
                            if (i > 0)
                            {
                                CssTextRun trun = (CssTextRun)r;
                                trun.MakeLength1();
                            }
                            else
                            {
                                continue;
                            }
                        } break;
                    case CssRunKind.Text:
                        {
                            
                            hasSomeChar = true;
                        } break;
                    default:
                        {

                        } break;
                }
            }
            toBox.SetContentRuns(runlist, hasSomeChar);
        }
        static void CreateRunsDefault(
            char[] buffer,
            List<CssRun> runlist,
            CssBox toBox,
            bool boxIsNotBreakAll, bool keepPreWhiteSpace)
        {

            bool hasSomeChar = false;
            int j = runlist.Count;

            for (int i = 0; i < j; ++i)
            {
                CssRun r = runlist[i];
                r.SetOwner(toBox);
                switch (r.Kind)
                {   
                    case CssRunKind.LineBreak:
                        {
                            //skip line break 
                            CssTextRun trun = (CssTextRun)r;
                            trun.MakeLength1();
                            //not accept new line 
                            //not run newline
                            continue;
                        }
                    case CssRunKind.SingleSpace:
                    case CssRunKind.Space:
                        {
                            if (i > 0)
                            {
                                CssTextRun trun = (CssTextRun)r;
                                trun.MakeLength1();
                            }
                            else
                            {
                                continue;
                            }
                        } break;
                    case CssRunKind.Text:
                        {
                            
                            hasSomeChar = true;
                        } break;
                    default:
                        {

                        } break;
                }
            }
            toBox.SetContentRuns(runlist, hasSomeChar);
        }

    }
}