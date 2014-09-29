//MS-PL, Apache2 
//2014, WinterDev 
using LayoutFarm.Drawing;
using System.Collections.Generic;


namespace HtmlRenderer.Boxes.LeanBox
{

    public sealed class CssLeanBox : CssBox
    {
        LayoutFarm.RenderElement renderElement;
        public CssLeanBox(object controller, 
            Css.BoxSpec spec,
            LayoutFarm.RenderElement renderElement)
            : base(controller, spec, Css.CssDisplay.Block)
        {
            this.renderElement = renderElement;  
            ChangeDisplayType(this, Css.CssDisplay.Block);
            SetAsCustomCssBox(this);
            this.SetSize(100, 20);
            
        }
        public LayoutFarm.RenderElement RenderElement
        {
            get { return this.renderElement; }            
        }
        public override void CustomRecomputedValue(CssBox containingBlock)
        {
            this.SetSize(100, 20);
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
            //for debug!
            g.FillRectangle(LayoutFarm.Drawing.Brushes.Red,
                0, 0, 100, 20);
        }
            
    }


}