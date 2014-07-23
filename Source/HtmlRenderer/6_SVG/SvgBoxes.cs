//BSD 2014,WinterDev

using System.Drawing;
using HtmlRenderer.Drawing;
using HtmlRenderer.SvgDom;

namespace HtmlRenderer.Boxes.Svg
{

    public sealed class SvgRootBox : CssBox
    {

        public SvgRootBox(CssBox owner, object controller, Css.BoxSpec spec, SvgElement svgSpec)
            : base(owner, controller, spec, Css.CssDisplay.Block)
        {
            //create svg node 
            this.SvgSpec = svgSpec;
            ChangeDisplayType(this, Css.CssDisplay.Block);
            SetAsSvgRoot(this);
        }
        protected override void PaintImp(IGraphics g, PaintVisitor p)
        {

            //render this svg
            var cnode = this.SvgSpec.GetFirstNode();
            while (cnode != null)
            {
                 
                cnode.Value.Paint(g);
                cnode = cnode.Next;
            }
        }
        public SvgElement SvgSpec
        {
            get;
            set;
        }

        /// <summary>
        /// for svg
        /// </summary>
        /// <param name="containingBlock"></param>
        internal void ReComputeSvgAspectValue(CssBox containingBlock)
        {

            var svgElement = this.SvgSpec;
            //recompute value if need 
            var cnode = svgElement.GetFirstNode();
            float containerW = containingBlock.SizeWidth;
            float emH = containingBlock.GetEmHeight();
            while (cnode != null)
            {
                cnode.Value.ReEvaluateComputeValue(containerW, 100, emH);
                cnode = cnode.Next;
            }

            this.SetSize(500, 500);
        }


    }

}