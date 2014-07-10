//BSD 2014, WinterDev
//ArthurHub 

using System;
using System.Collections.Generic;
using System.Drawing;

namespace HtmlRenderer.Dom
{

    partial class CssBox
    {
        CssDisplay _cssDisplay = CssDisplay.Inline;
        bool _isVisible;
        bool _isHiddenOverflow;
        void EvaluateSpec(BoxSpec spec)
        {
            this._isVisible = this._cssDisplay != Dom.CssDisplay.None && spec.Visibility == CssVisibility.Visible;
            this._isHiddenOverflow = this.Overflow == CssOverflow.Hidden;
        }

    }

}