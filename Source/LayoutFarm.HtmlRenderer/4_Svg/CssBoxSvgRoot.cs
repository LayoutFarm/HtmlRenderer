﻿//MS-PL, Apache2, 2014-present, WinterDev
using PixelFarm.Drawing;
using LayoutFarm.Svg;
using PaintLab.Svg;
namespace LayoutFarm.HtmlBoxes
{

    public sealed class CssBoxSvgRoot : CssBox
    {
        PixelFarm.CpuBlit.VgRenderVx _renderVx;

        public CssBoxSvgRoot(Css.BoxSpec spec, IRootGraphics rootgfx, SvgDocument svgdoc)
            : base(spec, rootgfx, Css.CssDisplay.Block)
        {
            SetAsCustomCssBox(this);
            //create svg node 
            this.SvgDoc = svgdoc;
            //convert svgElem to agg-based 
            ChangeDisplayType(this, Css.CssDisplay.Block);


            var renderVxDocBuilder = new PixelFarm.CpuBlit.SvgRenderVxDocBuilder();
            _renderVx = renderVxDocBuilder.CreateRenderVx(svgdoc);

        }
        public override void CustomRecomputedValue(CssBox containingBlock)
        {


           

            //var svgElement = this.SvgSpec;
            ////recompute value if need  
            //var cnode = svgElement.GetFirstNode();
            //ReEvaluateArgs reEvalArgs = new ReEvaluateArgs(
            //    containingBlock.VisualWidth,
            //    100,//temp 
            //    containingBlock.GetEmHeight());
            //while (cnode != null)
            //{
            //    cnode.Value.ReEvaluateComputeValue(ref reEvalArgs);
            //    cnode = cnode.Next;
            //}

            this.SetVisualSize(500, 500); //TODO: review here
        }
        protected override void PaintImp(PaintVisitor p)
        {
#if DEBUG
            p.dbugEnterNewContext(this, PaintVisitor.PaintVisitorContextName.Init);
#endif

            DrawBoard drawBoard = p.InnerCanvas;
            drawBoard.DrawRenderVx(_renderVx, 0, 0);
            //var g = p.InnerCanvas;
            //var prevMode = g.SmoothingMode;
            //g.SmoothingMode = SmoothingMode.AntiAlias;
            ////render this svg
            //var cnode = this.SvgSpec.GetFirstNode();
            //while (cnode != null)
            //{
            //    cnode.Value.Paint(p);
            //    cnode = cnode.Next;
            //}

            //g.SmoothingMode = prevMode;
#if DEBUG
            p.dbugExitContext();
#endif
        }
        public SvgDocument SvgDoc
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