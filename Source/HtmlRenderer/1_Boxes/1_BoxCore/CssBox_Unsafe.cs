//BSD, 2014, WinterDev 
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
        internal static BoxSpec UnsafeGetBoxSpec(CssBox box)
        {
            //this method is for BoxCreator and debug only!
            //box.Spec is private
            return box._myspec;
        }

#if DEBUG
        internal BridgeHtmlElement dbugAnonCreator
        {
            get;
            set;
        }
#endif
    }

}