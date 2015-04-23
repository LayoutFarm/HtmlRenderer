//MS-PL, Apache2 
//2014,2015, WinterDev

using PixelFarm.Drawing;
using System.Collections.Generic;
using LayoutFarm.Svg;

namespace LayoutFarm.HtmlBoxes
{

    public sealed class CssBoxSvgRoot : CssBox
    {
        public CssBoxSvgRoot(Css.BoxSpec spec, IRootGraphics rootgfx, SvgElement svgElem)
            : base(spec, rootgfx, Css.CssDisplay.Block)
        {
            SetAsCustomCssBox(this);
            //create svg node 
            this.SvgSpec = svgElem;
            ChangeDisplayType(this, Css.CssDisplay.Block);
        }
        public override void CustomRecomputedValue(CssBox containingBlock, GraphicsPlatform gfxPlatform)
        {
            var svgElement = this.SvgSpec;
            //recompute value if need  
            var cnode = svgElement.GetFirstNode();

            ReEvaluateArgs reEvalArgs = new ReEvaluateArgs(gfxPlatform,
                containingBlock.SizeWidth,
                100,
                containingBlock.GetEmHeight());
            while (cnode != null)
            {
                cnode.Value.ReEvaluateComputeValue(ref reEvalArgs);
                cnode = cnode.Next;
            }

            this.SetSize(500, 500);
        }
        protected override void PaintImp(PaintVisitor p)
        {
            var g = p.InnerCanvas;
            var prevMode = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            //render this svg
            var cnode = this.SvgSpec.GetFirstNode();
            while (cnode != null)
            {
                cnode.Value.Paint(p);
                cnode = cnode.Next;
            }

            g.SmoothingMode = prevMode;
        }
        public SvgElement SvgSpec
        {
            get;
            set;
        }

        public override bool CustomContentHitTest(float x, float y, CssBoxHitChain hitChain)
        {
            return true;//stop here
        }


    }


}