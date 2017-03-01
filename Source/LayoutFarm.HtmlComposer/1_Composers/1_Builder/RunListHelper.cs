//BSD, 2014-2017, WinterDev 

using System.Collections.Generic;
using LayoutFarm.Css;
using LayoutFarm.Composers;
namespace LayoutFarm.HtmlBoxes
{
    static class RunListHelper
    {
        public static void AddRunList(CssBox toBox, BoxSpec spec, HtmlTextNode textnode)
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
            CssBox.UnsafeSetTextBuffer(toBox, buffer);
            if (runlist != null)
            {
                for (int i = runlist.Count - 1; i >= 0; --i)
                {
                    runlist[i].SetOwner(toBox);
                }
            }
            CssBox.UnsafeSetContentRuns(toBox, runlist, isAllWhitespace);
        }
    }
}