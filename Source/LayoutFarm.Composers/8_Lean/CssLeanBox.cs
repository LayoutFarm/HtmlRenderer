//MS-PL, Apache2 
//2014, WinterDev 
using LayoutFarm.Drawing;
using System.Collections.Generic;


namespace HtmlRenderer.Boxes.Lean
{

    public sealed class CssLeanBox : CssBox
    {
        LayoutFarm.RenderElement renderElement;
        public CssLeanBox(object controller, Css.BoxSpec spec, LayoutFarm.RenderElement renderElement)
            : base(controller, spec, Css.CssDisplay.Block)
        {
            this.renderElement = renderElement;  
            ChangeDisplayType(this, Css.CssDisplay.Block); 
        }
        public LayoutFarm.RenderElement RenderElement
        {
            get { return this.renderElement; }            
        }
        public override void CustomRecomputedValue(CssBox containingBlock)
        {
            //var svgElement = this.SvgSpec;
            ////recompute value if need 
            //var cnode = svgElement.GetFirstNode();
            //float containerW = containingBlock.SizeWidth;
            //float emH = containingBlock.GetEmHeight();
            //while (cnode != null)
            //{
            //    cnode.Value.ReEvaluateComputeValue(containerW, 100, emH);
            //    cnode = cnode.Next;
            //} 
            //this.SetSize(500, 500);
        }
        protected override void PaintImp(IGraphics g, Painter p)
        {
            
            ////render this svg
            //var cnode = this.SvgSpec.GetFirstNode();
            //while (cnode != null)
            //{

            //    cnode.Value.Paint(g);
            //    cnode = cnode.Next;
            //}
        }
        //public SvgElement SvgSpec
        //{
        //    get;
        //    set;
        //}
        //public void HitTestCore(SvgHitChain chain, float x, float y)
        //{
        //    ////1.
        //    //SvgElement root = this.SvgSpec;
        //    //chain.AddHit(root, x, y);
        //    ////2. find hit child
        //    //var child = root.GetFirstNode();
        //    //while (child != null)
        //    //{
        //    //    var node = child.Value;
        //    //    if (node.HitTestCore(chain, x, y))
        //    //    {
        //    //        break;
        //    //    }
        //    //    child = child.Next;
        //    //}
        //}
    }


}