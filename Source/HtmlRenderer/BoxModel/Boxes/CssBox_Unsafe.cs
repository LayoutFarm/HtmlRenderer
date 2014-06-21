//BSD, 2014, WinterCore 
using System;
using System.Collections.Generic;

namespace HtmlRenderer.Dom
{


    partial class CssBox
    {

        /// <summary>
        /// the parent css box of this css box in the hierarchy
        /// </summary>
        CssBox _parentBox;


        internal static void UnsafeSetNodes(CssBox childNode, CssBox parent, CssBox prevSibling)
        {
            childNode._parentBox = parent;

        }
        internal static void UnsafeGetHostLine(CssBox box, out CssLineBox firstHostLine, out CssLineBox lastHostLine)
        {
            if (box._boxRuns == null)
            {
                firstHostLine = lastHostLine = null;
            }
            else
            {
                int j = box._boxRuns.Count;
                firstHostLine = box._boxRuns[0].HostLine;
                lastHostLine = box._boxRuns[j - 1].HostLine;
            }
        }
        internal static List<CssRun> UnsafeGetRunListOrCreateIfNotExists(CssBox box)
        {
            if (box._boxRuns == null)
            {
                return box._boxRuns = new List<CssRun>();
            }
            else
            {
                return box._boxRuns;
            }
        }
        internal static CssBoxCollection UnsafeGetChildren(CssBox box)
        {
            return box.Boxes;
        }
    }

}