//MS-PL, Apache2, 2014-present, WinterDev
using PixelFarm.Drawing;
using LayoutFarm.Svg;
using PaintLab.Svg;

namespace LayoutFarm.HtmlBoxes
{

    public sealed class CssBoxSvgRoot : CssBox
    {
        VgRenderVx _renderVx;
        static LayoutFarm.OpenFontTextService s_openfontTextService;


        public CssBoxSvgRoot(Css.BoxSpec spec, IRootGraphics rootgfx, SvgDocument svgdoc)
            : base(spec, rootgfx, Css.CssDisplay.Block)
        {
            SetAsCustomCssBox(this);
            //create svg node 
            this.SvgDoc = svgdoc;
            //convert svgElem to agg-based 
            ChangeDisplayType(this, Css.CssDisplay.Block);
        }
        public override void CustomRecomputedValue(CssBox containingBlock)
        {

            var renderVxDocBuilder = new SvgRenderVxDocBuilder();
            renderVxDocBuilder.SetContainerSize(containingBlock.VisualWidth, containingBlock.VisualHeight);
            //
            _renderVx = renderVxDocBuilder.CreateRenderVx(SvgDoc, svgElem =>
            {
                _renderVx.SetBitmapSnapshot(null);
                _renderVx.InvalidateBounds();
                this.InvalidateGraphics();
            });



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
                PixelFarm.CpuBlit.ActualBitmap backimg = new PixelFarm.CpuBlit.ActualBitmap((int)bound.Width + 10, (int)bound.Height + 10);
                PixelFarm.CpuBlit.AggPainter painter = PixelFarm.CpuBlit.AggPainter.Create(backimg);
                //TODO: review here
                //temp fix
                if (s_openfontTextService == null)
                {
                    s_openfontTextService = new OpenFontTextService();
                }

                painter.CurrentFont = new RequestFont("tahoma", 14);
                var textPrinter = new PixelFarm.Drawing.Fonts.VxsTextPrinter(painter, s_openfontTextService);
                painter.TextPrinter = textPrinter;
                painter.Clear(Color.White);
                //
                double prevStrokeW = painter.StrokeWidth;
                //Color fillColor = painter.FillColor;
                //painter.StrokeWidth = 1;//default 
                //painter.FillColor = Color.Black;

                VgPainterArgsPool.GetFreePainterArgs(painter, out VgPaintArgs paintArgs);
                _renderVx._renderE.Paint(paintArgs);
                VgPainterArgsPool.ReleasePainterArgs(ref paintArgs);
                painter.StrokeWidth = prevStrokeW;//restore
                //painter.FillColor = fillColor;////restore
#if DEBUG
                //test 
                //PixelFarm.CpuBlit.Imaging.PngImageWriter.dbugSaveToPngFile(backimg, "d:\\WImageTest\\subimg1.png");
#endif
                _renderVx.SetBitmapSnapshot(backimg);
                drawBoard.DrawImage(backimg, new RectangleF(0, 0, backimg.Width, backimg.Height));

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