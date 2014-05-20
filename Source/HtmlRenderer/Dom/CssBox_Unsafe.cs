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
        /// <summary>
        /// store linked to prev sibling
        /// </summary>
        CssBox _prevSibling;

        internal static void UnsafeSetNodes(CssBox childNode, CssBox parent, CssBox prevSibling)
        {
            childNode._parentBox = parent;
            childNode._prevSibling = prevSibling;
        }
        internal CssBox PrevSibling
        {
            get
            {
                return this._prevSibling;
            }
        }
    }

}