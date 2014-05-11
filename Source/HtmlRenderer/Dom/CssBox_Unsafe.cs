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
        private CssBox _parentBox; 
        internal static void UnsafeSetNodes(CssBox childNode, CssBox parent) 
        {
            childNode._parentBox = parent;    
        }
    }

}