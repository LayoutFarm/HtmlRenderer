//BSD 2014, WinterDev

namespace HtmlRenderer.Dom
{

    //collection features

    partial class CssBox
    {
        internal void DirectSetBorderWidth(CssSide side, float w)
        {
            switch (side)
            {
                case CssSide.Left:
                    {
                        this._actualBorderLeftWidth = w;
                    } break;
                case CssSide.Top:
                    {
                        this._actualBorderTopWidth = w;
                    } break;
                case CssSide.Right:
                    {
                        this._actualBorderRightWidth = w;
                    } break;
                case CssSide.Bottom:
                    {
                        this._actualBorderBottomWidth = w;
                    } break; 
            }  
        }
        internal void DirectSetBorderStyle(float leftWpx, float topWpx, float rightWpx, float bottomWpx)
        {
            this._actualBorderLeftWidth = leftWpx;
            this._actualBorderTopWidth = topWpx;
            this._actualBorderRightWidth = rightWpx;
            this._actualBorderBottomWidth = bottomWpx;
        }
        internal void DirectSetHeight(float px)
        {
            this._sizeHeight = px;
        }
    }
}