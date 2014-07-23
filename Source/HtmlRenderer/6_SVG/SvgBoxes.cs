//BSD 2014,WinterDev

using System.Drawing;
using HtmlRenderer.Drawing;
using HtmlRenderer.SvgDom;

namespace HtmlRenderer.Boxes.Svg
{

    public sealed class SvgRootBox : CssBox
    {
        
        public SvgRootBox(CssBox owner, object controller, Css.BoxSpec spec)
            : base(owner, controller, spec, Css.CssDisplay.Block)
        {
            //create svg node 
            
        }
        public SvgElement SvgElement
        {
            get;
            set;
        }
    }
}