// 2015, MIT, WinterDev 
using System;
using System.Collections.Generic;
namespace LayoutFarm.HtmlBoxes
{
    partial class CssBox
    {
        bool hasFlexContext;
        internal bool HasFlexContext
        {
            get { return this.hasFlexContext; }
            set { this.hasFlexContext = value; }
        }
    }
}
