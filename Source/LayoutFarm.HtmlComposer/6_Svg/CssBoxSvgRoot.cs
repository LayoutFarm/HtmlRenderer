//MS-PL, Apache2, 2014-present, WinterDev
using PixelFarm.Drawing;
using PaintLab.Svg;

namespace LayoutFarm.HtmlBoxes
{

    public sealed class CssBoxSvgRoot : CssBox
    {
        VgVisualElement _vgVisualElem;
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
        public bool DisableBmpCache
        {
            get;
            set;
        }
        public override void CustomRecomputedValue(CssBox containingBlock)
        {
            //TODO: review here again***
            //recreate entire dom?
            //why we need to re-create all 


            var vgDocBuilder = new VgVisualDocBuilder();
            vgDocBuilder.SetLoadImageHandler((ImageBinder reqImgBinder, VgVisualElement vgVisualE, object o) =>
            {

            });
            //
            vgDocBuilder.SetContainerSize(containingBlock.VisualWidth, containingBlock.VisualHeight);

            //create visual svg doc from current SvgDoc object

            _vgVisualElem = vgDocBuilder.CreateVgVisualDoc(SvgDoc, svgElem =>
            {
                _vgVisualElem.ClearBitmapSnapshot();
                _vgVisualElem.InvalidateBounds();
                this.InvalidateGraphics();
            }).VgRootElem;

            this.SetVisualSize(500, 500); //TODO: review here
        }
        protected override void PaintImp(PaintVisitor p)
        {
#if DEBUG
            p.dbugEnterNewContext(this, PaintVisitor.PaintVisitorContextName.Init);
#endif
            DrawBoard drawBoard = p.InnerDrawBoard;

            if (DisableBmpCache)
            {

                PixelFarm.CpuBlit.AggPainter painter = drawBoard.GetPainter() as PixelFarm.CpuBlit.AggPainter;
                if (painter == null)
                {
                    return;
                }
                //TODO: review here
                //temp fix
                if (s_openfontTextService == null)
                {
                    s_openfontTextService = new OpenFontTextService();
                }

                //painter.CurrentFont = new RequestFont("tahoma", 14);
                //var textPrinter = new PixelFarm.Drawing.Fonts.VxsTextPrinter(painter, s_openfontTextService);
                //painter.TextPrinter = textPrinter;
                //painter.Clear(Color.White);
                //
                double prevStrokeW = painter.StrokeWidth;
                //Color fillColor = painter.FillColor;
                //painter.StrokeWidth = 1;//default 
                //painter.FillColor = Color.Black; 

                using (VgPainterArgsPool.Borrow(painter, out var paintArgs))
                {
                    if (_vgVisualElem.CoordTx != null)
                    {

                    }
                    _vgVisualElem.Paint(paintArgs);
                }


                painter.StrokeWidth = prevStrokeW;//restore
                //painter.FillColor = fillColor;////restore

                return;
            }


            if (_vgVisualElem.HasBitmapSnapshot)
            {
                Image backimg = _vgVisualElem.BackingImage;
                drawBoard.DrawImage(backimg, new RectangleF(0, 0, backimg.Width, backimg.Height));
            }
            else
            {

                PixelFarm.CpuBlit.RectD bound = _vgVisualElem.GetRectBounds();
                //create
                PixelFarm.CpuBlit.MemBitmap backimg = new PixelFarm.CpuBlit.MemBitmap((int)bound.Width + 10, (int)bound.Height + 10);
#if DEBUG
                backimg._dbugNote = "cssBoxSvgRoot";
#endif
                PixelFarm.CpuBlit.AggPainter painter = PixelFarm.CpuBlit.AggPainter.Create(backimg);
                //TODO: review here
                //temp fix
                if (s_openfontTextService == null)
                {
                    s_openfontTextService = new OpenFontTextService();
                }


                //
                double prevStrokeW = painter.StrokeWidth;

                using (VgPainterArgsPool.Borrow(painter, out VgPaintArgs paintArgs))
                {
                    if (_vgVisualElem.CoordTx != null)
                    {

                    }
                    _vgVisualElem.Paint(paintArgs);
                }

                painter.StrokeWidth = prevStrokeW;//restore
                //painter.FillColor = fillColor;////restore
#if DEBUG
                //test 
                //PixelFarm.CpuBlit.Imaging.PngImageWriter.dbugSaveToPngFile(backimg, "d:\\WImageTest\\subimg1.png");
#endif
                _vgVisualElem.SetBitmapSnapshot(backimg, true);
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