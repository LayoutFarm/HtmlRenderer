//BSD 2014, WinterDev 
using System;
using System.Drawing;
using System.Collections.Generic;

using HtmlRenderer.WebDom;
using HtmlRenderer.Css;
using HtmlRenderer.RenderDom;
namespace HtmlRenderer.Composers
{
    static class RunListHelper
    {

        public static void AddRunList(CssBox toBox, BoxSpec spec, BridgeHtmlTextNode textnode)
        {
            AddRunList(toBox, spec, textnode.InternalGetRuns(), textnode.GetOriginalBuffer(), textnode.IsWhiteSpace);
        }
        //---------------------------------------------------------------------------------------
        public static void AddRunList(CssBox toBox,
            BoxSpec spec,
            List<CssRun> runlist,
            char[] buffer,
            bool isAllWhitespace)
        {   
            toBox.SetTextBuffer(buffer);
            if (runlist != null)
            {
                for (int i = runlist.Count - 1; i >= 0; --i)
                {
                    runlist[i].SetOwner(toBox);
                }
            }
            toBox.SetContentRuns(runlist, isAllWhitespace); 
        } 
    }
}