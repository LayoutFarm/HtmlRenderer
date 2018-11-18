//MIT, 2014-present, WinterDev

using System.IO;

using PixelFarm.Drawing;
using PixelFarm.CpuBlit.VertexProcessing;
using PaintLab.Svg;
using LayoutFarm.UI;

namespace LayoutFarm
{
    [DemoNote("9.2.2 ShapeControls")]
    class DemoShapeControl3 : App
    {

        QuadControllerUI _quadController = new QuadControllerUI();
        PolygonControllerUI _quadPolygonController = new PolygonControllerUI();


        VgRenderVx CreateTestRenderVx_FromSvg()
        {
            //string svgfile = "../Test8_HtmlRenderer.Demo/Samples/Svg/others/cat_simple.svg";
            //string svgfile = "../Test8_HtmlRenderer.Demo/Samples/Svg/others/cat_complex.svg";
            //string svgfile = "../Test8_HtmlRenderer.Demo/Samples/Svg/others/lion.svg";
            string svgfile = "../Test8_HtmlRenderer.Demo/Samples/Svg/others/tiger.svg";
            //string svgfile = "../Test8_HtmlRenderer.Demo/Samples/Svg/freepik/dog1.svg";
            //string svgfile = "1f30b.svg";
            //string svgfile = "../Data/Svg/twemoji/1f30b.svg";
            //string svgfile = "../Data/1f30b.svg";
            //string svgfile = "../Data/Svg/twemoji/1f370.svg";
            return VgRenderVxHelper.ReadSvgFile(svgfile);
        }
        VgRenderVx CreateEllipseVxs(PixelFarm.CpuBlit.RectD newBounds)
        {
            using (VxsTemp.Borrow(out var v1))
            using (VectorToolBox.Borrow(out Ellipse ellipse))
            {
                ellipse.Set((newBounds.Left + newBounds.Right) * 0.5,
                                             (newBounds.Bottom + newBounds.Top) * 0.5,
                                             (newBounds.Right - newBounds.Left) * 0.5,
                                             (newBounds.Top - newBounds.Bottom) * 0.5);


                var spec = new SvgPathSpec() { FillColor = Color.Red };
                VgVisualRootElement renderRoot = new VgVisualRootElement();
                VgVisualElement renderE = new VgVisualElement(WellknownSvgElementName.Path, spec, renderRoot);
                VgRenderVx svgRenderVx = new VgRenderVx(renderE);
                renderE._vxsPath = ellipse.MakeVxs(v1).CreateTrim();
                return svgRenderVx;
            }

        }
        VgRenderVx CreateTestRenderVx_BasicShape()
        {
            var spec = new SvgPathSpec() { FillColor = Color.Red };
            VgVisualRootElement renderRoot = new VgVisualRootElement();
            VgVisualElement renderE = new VgVisualElement(WellknownSvgElementName.Path, spec, renderRoot);
            VgRenderVx svgRenderVx = new VgRenderVx(renderE);

            using (VxsTemp.Borrow(out VertexStore vxs))
            {
                //red-triangle ***
                vxs.AddMoveTo(10, 10);
                vxs.AddLineTo(60, 10);
                vxs.AddLineTo(60, 30);
                vxs.AddLineTo(10, 30);
                vxs.AddCloseFigure();
                renderE._vxsPath = vxs.CreateTrim();
            }

            return svgRenderVx;
        }


        Typography.Contours.GlyphMeshStore _glyphMaskStore = new Typography.Contours.GlyphMeshStore();
        VgRenderVx CreateTestRenderVx_FromGlyph(char c, float sizeInPts)
        {
            //create vgrender vx from font-glyph
            string fontfile = "../Test8_HtmlRenderer.Demo/Samples/Fonts/SOV_Thanamas.ttf";

            Typography.OpenFont.Typeface typeface = null;
            using (System.IO.FileStream fs = new FileStream(fontfile, FileMode.Open))
            {
                Typography.OpenFont.OpenFontReader reader = new Typography.OpenFont.OpenFontReader();
                typeface = reader.Read(fs);
            }
            _glyphMaskStore.FlipGlyphUpward = true;
            _glyphMaskStore.SetFont(typeface, sizeInPts);
            //-----------------


            VertexStore vxs = _glyphMaskStore.GetGlyphMesh(typeface.LookupIndex(c));
            var spec = new SvgPathSpec() { FillColor = Color.Red };
            VgVisualRootElement renderRoot = new VgVisualRootElement();
            VgVisualElement renderE = new VgVisualElement(WellknownSvgElementName.Path, spec, renderRoot);
            VgRenderVx svgRenderVx = new VgRenderVx(renderE);


            //offset the original vxs to (0,0) bounds
            //PixelFarm.CpuBlit.RectD bounds = vxs.GetBoundingRect();
            //Affine translate = Affine.NewTranslation(-bounds.Left, -bounds.Bottom);
            //renderE._vxsPath = vxs.CreateTrim(translate);


            PixelFarm.CpuBlit.RectD bounds = vxs.GetBoundingRect();
            Affine translate = Affine.NewTranslation(-bounds.Left, -bounds.Bottom);
            renderE._vxsPath = vxs.CreateTrim(translate);
            return svgRenderVx;
        }

        bool _hitTestOnSubPath = false;
        VgRenderVx _svgRenderVx;

        UISprite _uiSprite;


        protected override void OnStart(AppHost host)
        {


            _svgRenderVx = CreateTestRenderVx_FromSvg();
            //_svgRenderVx = CreateTestRenderVx_BasicShape();
            //_svgRenderVx = CreateTestRenderVx_FromGlyph('a', 256); //create from glyph
            //PixelFarm.CpuBlit.RectD org_rectD = _svgRenderVx.GetBounds(); 
            //_svgRenderVx = CreateEllipseVxs(org_rectD);

            PixelFarm.CpuBlit.RectD org_rectD = _svgRenderVx.GetBounds();

            org_rectD.Offset(-org_rectD.Left, -org_rectD.Bottom);
            _quadController.SetSrcRect(org_rectD.Left, org_rectD.Bottom, org_rectD.Right, org_rectD.Top);
            _quadController.SetDestQuad(
              org_rectD.Left, org_rectD.Top,
              org_rectD.Right, org_rectD.Top,
              org_rectD.Right, org_rectD.Bottom,
              org_rectD.Left, org_rectD.Bottom);



            //---------
            //create control point
            _quadController.SetPolygonController(_quadPolygonController);
            _quadController.BuildControlBoxes();
            _quadController.UpdateTransformTarget += (s1, e1) =>
            {

                _uiSprite.SetTransformation(_quadController.GetCoordTransformer()); //set transformation 
                if (_quadController.Left != 0 || _quadController.Top != 0)
                {
                    _uiSprite.SetLocation(_quadController.Left, _quadController.Top);
                }
            };


            //_rectBoundsWidgetBox = new Box2(50, 50); //visual rect box
            //Color c = KnownColors.FromKnownColor(KnownColor.DeepSkyBlue);
            //_rectBoundsWidgetBox.BackColor = Color.FromArgb(100, c);
            //_rectBoundsWidgetBox.SetLocation(10, 10);
            /////box1.dbugTag = 1;
            //SetupActiveBoxProperties(_rectBoundsWidgetBox);
            //host.AddChild(_rectBoundsWidgetBox);



            _quadController.Visible = _quadPolygonController.Visible = false;


            //_rectBoxController.Init();

            //VgRenderVx svgRenderVx = CreateTestRenderVx(); //create from svg
            //test...
            //1. scale svg to fix the 'src rect'  
            //2. then transform to the 'dest rect' 

            PixelFarm.CpuBlit.RectD svg_bounds = _svgRenderVx.GetBounds();

            //double w_scale = src_w / svg_bounds.Width;
            //double h_scale = src_h / svg_bounds.Height;

            //double actualXOffset = -svg_bounds.Left;
            //double actualYOffset = -svg_bounds.Bottom;

            //Affine scaleMat = Affine.NewMatix(
            //    AffinePlan.Translate(
            //        actualXOffset - svg_bounds.Width / 2, //move to its middle point
            //        actualYOffset - svg_bounds.Height / 2),//move to its middle point
            //    AffinePlan.Scale(w_scale, h_scale),
            //    AffinePlan.Translate(
            //        -(actualXOffset - svg_bounds.Width / 2) * w_scale,//move back
            //        -(actualYOffset - svg_bounds.Height / 2) * h_scale)); //move back

            //ICoordTransformer tx = ((ICoordTransformer)_bilinearTx).MultiplyWith(scaleMat);
            ICoordTransformer tx = _quadController.GetCoordTransformer();
            //svgRenderVx._coordTx = tx;

            //svgRenderVx._coordTx = ((ICoordTransformer)_bilinearTx).MultiplyWith(scaleMat);

            //host.AddChild(_quadController);
            //host.AddChild(_quadPolygonController);
            //VgRenderVx svgRenderVx = CreateTestRenderVx(); 
            //test transform svgRenderVx 
            _svgRenderVx.DisableBackingImage = true;
            _uiSprite = new UISprite(10, 10); //init size = (10,10), location=(0,0)       
            _uiSprite.DisableBmpCache = true;
            _uiSprite.LoadVg(_svgRenderVx);// 
            _uiSprite.SetTransformation(tx); //set transformation




            host.AddChild(_uiSprite);


            //-----------------------------------------
            host.AddChild(_quadController);
            host.AddChild(_quadPolygonController);

            //-----------------------------------------


            var spriteEvListener = new GeneralEventListener();

            _uiSprite.AttachExternalEventListener(spriteEvListener);
            spriteEvListener.MouseMove += e1 =>
            {
                if (e1.IsDragging)
                {
                    _uiSprite.InvalidateOuterGraphics();
                    _uiSprite.SetLocation(
                        _uiSprite.Left + e1.XDiff,
                        _uiSprite.Top + e1.YDiff
                        );
                    _quadController.InvalidateOuterGraphics();
                    _quadController.SetLocation(
                        _quadController.Left + e1.XDiff,
                        _quadController.Top + e1.YDiff);
                    _quadController.InvalidateOuterGraphics();
                    _quadPolygonController.InvalidateOuterGraphics();
                    _quadPolygonController.SetPosition(
                        _quadPolygonController.Left + e1.XDiff,
                        _quadPolygonController.Top + e1.YDiff
                        );
                    _quadPolygonController.InvalidateOuterGraphics();
                }
            };
            spriteEvListener.MouseDown += e1 =>
            {
                //mousedown on ui sprite 
                //find exact part ...  

                _quadController.BringToTopMost();
                _quadController.Visible = true;
                _quadPolygonController.Visible = true;
                _quadController.Focus();

                // _polygonController.BringToTopMost();

                if (_hitTestOnSubPath)
                {
                    VgHitInfo hitInfo = _uiSprite.FindRenderElementAtPos(e1.X, e1.Y, true);
                    if (hitInfo.svg != null &&
                        hitInfo.svg._vxsPath != null)
                    {

                        PixelFarm.CpuBlit.RectD bounds = hitInfo.copyOfVxs.GetBoundingRect();
                        _quadPolygonController.ClearControlPoints();
                        _quadPolygonController.UpdateControlPoints(hitInfo.copyOfVxs,
                            _uiSprite.ActualXOffset, _uiSprite.ActualYOffset);

                        ////move redbox and its controller
                        //_rectBoundsWidgetBox.SetLocationAndSize(
                        //    (int)(bounds.Left + _uiSprite.ActualXOffset), (int)(bounds.Top - bounds.Height + _uiSprite.ActualYOffset),
                        //    (int)bounds.Width, (int)bounds.Height);
                        ////_rectBoxController.UpdateControllerBoxes(_rectBoundsWidgetBox);

                        //_rectBoundsWidgetBox.Visible = true;
                        ////_rectBoxController.Visible = true;
                        //show bounds
                    }
                    else
                    {
                        //_rectBoundsWidgetBox.Visible = false;
                        // _rectBoxController.Visible = false;
                    }
                }
                else
                {
                    //hit on sprite 




                    if (e1.Ctrl)
                    {
                        //test*** 
                        //
                        _uiSprite.GetElementBounds(out float left, out float top, out float right, out float bottom);
                        //
                        using (VectorToolBox.Borrow(out SimpleRect s))
                        using (VxsTemp.Borrow(out var v1))
                        {
                            s.SetRect(left - _uiSprite.ActualXOffset,
                                bottom - _uiSprite.ActualYOffset,
                                right - _uiSprite.ActualXOffset,
                                top - _uiSprite.ActualYOffset);
                            s.MakeVxs(v1);
                            //_polygonController.UpdateControlPoints(v1.CreateTrim());
                        }
                    }
                    else
                    {

                        //_rectBoundsWidgetBox.SetTarget(_uiSprite);
                        //_rectBoundsWidgetBox.SetLocationAndSize(    //blue
                        //      (int)_uiSprite.Left, (int)_uiSprite.Top,
                        //      (int)_uiSprite.Width, (int)_uiSprite.Height);
                        ////_rectBoxController.SetTarget(_uiSprite);

                        ////_rectBoxController.UpdateControllerBoxes(_rectBoundsWidgetBox);  //control 4 corners
                        //_rectBoundsWidgetBox.Visible = true;
                        ////_rectBoxController.Visible = true;

                        //UpdateTransformedShape2();
                    }
                }
            };

            //-

            //_rectBoxController.UpdatedShape += (s3, e3) => UpdateTransformedShape2();

        }



    }





}