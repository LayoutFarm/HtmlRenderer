//MS-PL, Apache2, 2014-present, WinterDev
using PixelFarm.Drawing;
using LayoutFarm.Svg;
using PaintLab.Svg;

namespace LayoutFarm.HtmlBoxes
{

    public sealed class CssBoxSvgRoot : CssBox
    {
        VgRenderVx _renderVx;

        public CssBoxSvgRoot(Css.BoxSpec spec, IRootGraphics rootgfx, SvgDocument svgdoc)
            : base(spec, rootgfx, Css.CssDisplay.Block)
        {
            SetAsCustomCssBox(this);
            //create svg node 
            this.SvgDoc = svgdoc;
            //convert svgElem to agg-based 
            ChangeDisplayType(this, Css.CssDisplay.Block);
            var renderVxDocBuilder = new SvgRenderVxDocBuilder();
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
            if (_renderVx.HasBitmapSnapshot)
            {
                Image backimg = _renderVx.BackingImage;
                drawBoard.DrawImage(backimg, new RectangleF(0, 0, backimg.Width, backimg.Height));
            }
            else
            {

                PixelFarm.CpuBlit.RectD bound = _renderVx.GetBounds();

                //create 
                PixelFarm.CpuBlit.ActualBitmap backimg = new PixelFarm.CpuBlit.ActualBitmap((int)bound.Width + 200, (int)bound.Height + 200);
                PixelFarm.CpuBlit.AggPainter painter = PixelFarm.CpuBlit.AggPainter.Create(backimg);

                painter.StrokeWidth = 1;//default

                SvgPainter svgPainter = new SvgPainter();
                svgPainter.P = painter;
                ((SvgRenderElement)_renderVx._renderE).Paint(svgPainter);
#if DEBUG
                //test 
                //PixelFarm.CpuBlit.Imaging.PngImageWriter.dbugSaveToPngFile(backimg, "d:\\WImageTest\\subimg1.png");
#endif
                _renderVx.SetBitmapSnapshot(backimg);
                drawBoard.DrawImage(backimg, new RectangleF(0, 0, backimg.Width, backimg.Height));
                //drawBoard.DrawRenderVx(_renderVx, 0, 0); 
            }
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