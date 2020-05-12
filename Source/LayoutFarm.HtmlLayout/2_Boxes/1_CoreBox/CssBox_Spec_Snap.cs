//BSD, 2014-present, WinterDev 
//ArthurHub, Jose Manuel Menendez Poo

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
            _isVisible = _cssDisplay != Css.CssDisplay.None &&
                              spec.Visibility == CssVisibility.Visible;
            _cssBoxSizing = spec.BoxSizing;
            _borderLeftVisible = spec.BorderLeftStyle >= CssBorderStyle.Visible;
            _borderTopVisible = spec.BorderTopStyle >= CssBorderStyle.Visible;
            _borderRightVisible = spec.BorderRightStyle >= CssBorderStyle.Visible;
            _borderBottomVisble = spec.BorderBottomStyle >= CssBorderStyle.Visible;
            _renderBGAndBorder = _cssDisplay != Css.CssDisplay.Inline ||
                   this.Position == CssPosition.Absolute || //out of flow
                   this.Position == CssPosition.Fixed; //out of flow
        }
        internal bool BorderLeftVisible => _borderLeftVisible;
        internal bool BorderTopVisible => _borderTopVisible;
        internal bool BorderRightVisible => _borderRightVisible;
        internal bool BorderBottomVisible => _borderBottomVisble;
    }
}