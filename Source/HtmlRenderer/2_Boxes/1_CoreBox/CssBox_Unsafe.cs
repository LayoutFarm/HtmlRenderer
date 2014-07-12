//BSD, 2014, WinterDev 
using System;
using System.Collections.Generic;

namespace HtmlRenderer.Boxes
{


    partial class CssBox
    {

        /// <summary>
        /// the parent css box of this css box in the hierarchy
        /// </summary>
        CssBox _parentBox;
        LinkedListNode<CssBox> _linkedNode;
        //-------------------------------------------------------
        internal static LinkedListNode<CssBox> UnsafeGetLinkedNode(CssBox box)
        {
            return box._linkedNode;
        }
        internal static void UnsafeSetNodes2(CssBox childNode, CssBox parent, LinkedListNode<CssBox> linkNode)
        {
            childNode._parentBox = parent;
            childNode._linkedNode = linkNode;
        }
        internal static void UnsafeSetNodes(CssBox childNode, CssBox parent, CssBox prevSibling)
        {
            childNode._parentBox = parent;
        }
        internal static List<CssRun> UnsafeGetRunList(CssBox box)
        {
            return box._aa_contentRuns;
        }
        internal static CssBoxCollection UnsafeGetChildren(CssBox box)
        {
            return box.Boxes;
        }
        internal static Css.BoxSpec UnsafeGetBoxSpec(CssBox box)
        {
            //this method is for BoxCreator and debug only!
            //box.Spec is private
            return box._myspec;
        }
        internal static void UnsafeSetParent(CssBox box, CssBox parent)
        {
            box._parentBox = parent;
            //box._htmlContainer = htmlContainer;
        }

#if DEBUG
        //internal BridgeHtmlElement dbugAnonCreator
        //{
        //    get;
        //    set;
        //}

        internal static object debugGetController(CssBox box)
        {
            return box._controller;
        }

#endif
    }

}