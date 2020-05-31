//MS-PL, Apache2, 2014-present, WinterDev
using PixelFarm.Drawing;
using PixelFarm.CpuBlit;
using PaintLab.Svg;

namespace LayoutFarm.HtmlBoxes
{

    public sealed class CssBoxSvgRoot : CssBox
    {
        //css-box for a svg document

        VgVisualElement _vgVisualElem;
        VgVisualDocHost _vgVisualDocHost;

        
        public CssBoxSvgRoot(Css.BoxSpec spec, VgDocument svgdoc)
            : base(spec, Css.CssDisplay.Block)
        {
            
            //----------
            _vgVisualDocHost = new PaintLab.Svg.VgVisualDocHost();
            _vgVisualDocHost.SetImgRequestDelgate((ImageBinder reqImgBinder, PaintLab.Svg.VgVisualElement vgVisualE, object requestFrom) =>
            {
                //TODO: implementation here
            });
            _vgVisualDocHost.SetInvalidateDelegate(vgVisualElem =>
            {
                vgVisualElem.ClearBitmapSnapshot();
                vgVisualElem.InvalidateBounds();
                this.InvalidateGraphics();
            });
            //----------

            SetAsCustomCssBox(this);
            //create svg node 
            this.SvgDoc = svgdoc;
            //convert svgElem to agg-based 
            ChangeDisplayType(this, Css.CssDisplay.Block);
        }
        public bool DisableBmpCache { get; set; }
        public override void CustomRecomputedValue(CssBox containingBlock)
        {
            //TODO: review here again***
            //recreate entire dom?
            //why we need to re-create all  
            var vgDocBuilder = new VgVisualDocBuilder();
            vgDocBuilder.SetContainerSize(containingBlock.VisualWidth, containingBlock.VisualHeight);
            _vgVisualElem = vgDocBuilder.CreateVgVisualDoc(SvgDoc, _vgVisualDocHost).VgRootElem;
            this.SetVisualSize(500, 500); //TODO: review here
        }
        protected override void PaintImp(PaintVisitor p)
        {
#if DEBUG
            p.dbugEnterNewContext(this, PaintVisitor.PaintVisitorContextName.Init);
#endif

            Color bgColorHint = p.CurrentSolidBackgroundColorHint;//save

            p.CurrentSolidBackgroundColorHint = Color.Transparent;

            DrawBoard drawBoard = p.InnerDrawBoard;

            if (DisableBmpCache)
            {
                if (!(drawBoard.GetPainter() is PixelFarm.CpuBlit.AggPainter painter))
                {
                    return;
                }
                //TODO: review here
                //temp fix
                 
                //painter.CurrentFont = new RequestFont("tahoma", 14);
                //var textPrinter = new PixelFarm.Drawing.Fonts.VxsTextPrinter(painter, s_openfontTextService);
                //painter.TextPrinter = textPrinter;
                //painter.Clear(Color.White);
                //
                double prevStrokeW = painter.StrokeWidth;
                //Color fillColor = painter.FillColor;
                //painter.StrokeWidth = 1;//default 
                //painter.FillColor = Color.Black; 

                using (Tools.More.BorrowVgPaintArgs(painter, out var paintArgs))
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

                PixelFarm.CpuBlit.VertexProcessing.Q1RectD bound = _vgVisualElem.GetRectBounds();
                //create
                PixelFarm.CpuBlit.MemBitmap backimg = new PixelFarm.CpuBlit.MemBitmap((int)bound.Width + 10, (int)bound.Height + 10);
#if DEBUG
                backimg._dbugNote = "cssBoxSvgRoot";
#endif
                PixelFarm.CpuBlit.AggPainter painter = PixelFarm.CpuBlit.AggPainter.Create(backimg);
                //TODO: review here
                //temp fix
                

                //
                double prevStrokeW = painter.StrokeWidth;

                using (Tools.More.BorrowVgPaintArgs(painter, out VgPaintArgs paintArgs))
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


            p.CurrentSolidBackgroundColorHint = bgColorHint;
        }
        public VgDocument SvgDoc { get; set; }

        public override bool CustomContentHitTest(float x, float y, CssBoxHitChain hitChain)
        {
            return true;//stop here
        }
    }




}