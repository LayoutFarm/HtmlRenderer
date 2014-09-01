//MS-PL, Apache2 
//2014, WinterDev

using System.Drawing;
using System.Collections.Generic;
using HtmlRenderer.Drawing;
using HtmlRenderer.SvgDom;

namespace HtmlRenderer.Boxes
{

    public sealed class CssBoxSvgRoot : CssBox
    {
        public CssBoxSvgRoot(object controller, Css.BoxSpec spec, SvgElement svgSpec)
            : base(controller, spec, Css.CssDisplay.Block)
        {
            //create svg node 
            this.SvgSpec = svgSpec;
            ChangeDisplayType(this, Css.CssDisplay.Block);
            SetAsCustomCssBox(this);
        }
        public override void CustomRecomputedValue(CssBox containingBlock)
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
        protected override void PaintImp(IGraphics g, Painter p)
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



        public void HitTestCore(SvgHitChain chain, float x, float y)
        {
            //1.
            SvgElement root = this.SvgSpec;
            chain.AddHit(root, x, y);
            //2. find hit child
            var child = root.GetFirstNode();
            while (child != null)
            {
                var node = child.Value;
                if (node.HitTestCore(chain, x, y))
                {
                    break;
                }
                child = child.Next;
            }
        }
    }


}