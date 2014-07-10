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

        bool _borderLeftVisible;
        bool _borderTopVisible;
        bool _borderRightVisible;
        bool _borderBottomVisble;

        void EvaluateSpecOnInit(BoxSpec spec)
        {
           
            this._isHiddenOverflow = this.Overflow == CssOverflow.Hidden;

            this._borderLeftVisible = spec.BorderLeftStyle >= CssBorderStyle.Visible;
            this._borderTopVisible = spec.BorderTopStyle >= CssBorderStyle.Visible;

            this._borderRightVisible = spec.BorderRightStyle >= CssBorderStyle.Visible;
            this._borderBottomVisble = spec.BorderBottomStyle >= CssBorderStyle.Visible;
        }
        internal bool BorderLeftVisible
        {
            get { return this._borderLeftVisible; }
        }
        internal bool BorderTopVisible
        {
            get { return this._borderTopVisible; }
        }
        internal bool BorderRightVisible
        {
            get { return this._borderRightVisible; }
        }
        internal bool BorderBottomVisible
        {
            get { return this._borderBottomVisble; }
        }
    }

}