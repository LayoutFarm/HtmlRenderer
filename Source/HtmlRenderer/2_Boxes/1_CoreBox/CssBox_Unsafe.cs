//BSD, 2014, WinterDev 
using System;
using System.Collections.Generic;
using HtmlRenderer.Css;
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
        internal static void UnsafeSetNodes(CssBox childNode, CssBox parent, LinkedListNode<CssBox> linkNode)
        {
            childNode._parentBox = parent;
            childNode._linkedNode = linkNode;
        }

        internal static List<CssRun> UnsafeGetRunList(CssBox box)
        {
            return box._aa_contentRuns;
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
        public static void UnsafeSetParent(CssBox box, CssBox parent)
        {
            box._parentBox = parent;
        }
        public static object UnsafeGetController(CssBox box)
        {
            return box._controller;
        }
        public static void UnsafeSetTextBuffer(CssBox box, char[] textBuffer)
        {
            //TODO: change to unsafe static 
            box._buffer = textBuffer;
        }
        public static void UnsafeSetContentRuns(CssBox box, List<CssRun> runs, bool isAllWhitespace)
        {
            box._aa_contentRuns = runs;
            if (isAllWhitespace)
            {
                box._boxCompactFlags |= BoxFlags.TEXT_IS_ALL_WHITESPACE;
            }
            else
            {
                box._boxCompactFlags &= ~BoxFlags.TEXT_IS_ALL_WHITESPACE;

            }
        }

#if DEBUG
        public override string ToString()
        {
            if (this._controller != null)
            {
                if (this.HasRuns)
                {
                    return this._controller.ToString() + " " + this.CssDisplay + " r=" + this.RunCount;
                }
                else
                {
                    return this._controller.ToString() + " " + this.CssDisplay + " c=" + this.ChildCount;
                }
            }
            else
            {
                if (this.HasRuns)
                {
                    return "!a " + " " + this.CssDisplay + " r=" + this.RunCount;
                }
                else
                {
                    return "!a " + " " + this.CssDisplay + " c=" + this.ChildCount;
                }
            }
            return base.ToString();
        }
#endif
    }

}