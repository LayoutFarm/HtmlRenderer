// 2015,2014 ,BSD, WinterDev
//ArthurHub  , Jose Manuel Menendez Poo

using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
using LayoutFarm.Css;

namespace LayoutFarm.HtmlBoxes
{

    partial class CssBox
    {
        CssDisplay _cssDisplay = CssDisplay.Inline;
        CssBoxSizing _cssBoxSizing = CssBoxSizing.ContentBox;

        bool _isVisible;
        bool _borderLeftVisible;
        bool _borderTopVisible;
        bool _borderRightVisible;
        bool _borderBottomVisble;
        bool _renderBGAndBorder;

        void EvaluateSpec(BoxSpec spec)
        {  
            this._isVisible = this._cssDisplay != Css.CssDisplay.None &&
                              spec.Visibility == CssVisibility.Visible;

            this._cssBoxSizing = spec.BoxSizing;

            this._borderLeftVisible = spec.BorderLeftStyle >= CssBorderStyle.Visible;
            this._borderTopVisible = spec.BorderTopStyle >= CssBorderStyle.Visible;
            this._borderRightVisible = spec.BorderRightStyle >= CssBorderStyle.Visible;
            this._borderBottomVisble = spec.BorderBottomStyle >= CssBorderStyle.Visible;

            this._renderBGAndBorder = _cssDisplay != Css.CssDisplay.Inline ||
                   this.Position == CssPosition.Absolute || //out of flow
                   this.Position == CssPosition.Fixed; //out of flow

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