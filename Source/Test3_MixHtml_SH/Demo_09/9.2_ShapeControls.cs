//MIT, 2014-present, WinterDev
using LayoutFarm.UI;
using PaintLab.Svg;
using PixelFarm.CpuBlit.VertexProcessing;
using PixelFarm.Drawing;

namespace LayoutFarm
{
    [DemoNote("9.2.2 ShapeControls")]
    class DemoShapeControl9_2 : App
    {
        RotationUI _rotationUI = new RotationUI();
        QuadControllerUI _quadController = new QuadControllerUI();
        PolygonControllerUI _quadPolygonController = new PolygonControllerUI();
        bool _hitTestOnSubPath = true;
        UISprite _uiSprite;
        AppHost _appHost;
        VgVisualElement _vgVisualElem;
        VgVisualDocHost _vgVisualDocHost; //**

        public DemoShapeControl9_2()
        {
            _vgVisualDocHost = new VgVisualDocHost();
            _vgVisualDocHost.SetImgRequestDelgate(LoadRawImg);
        }
        VgVisualElement CreateEllipseVxs(PixelFarm.CpuBlit.RectD newBounds)
        {
            using (VxsTemp.Borrow(out var v1))
            using (VectorToolBox.Borrow(out Ellipse ellipse))
            {
                ellipse.Set((newBounds.Left + newBounds.Right) * 0.5,
                                             (newBounds.Bottom + newBounds.Top) * 0.5,
                                             (newBounds.Right - newBounds.Left) * 0.5,
                                             (newBounds.Top - newBounds.Bottom) * 0.5);


                var spec = new SvgPathSpec() { FillColor = Color.Red };
                VgVisualDoc renderRoot = new VgVisualDoc();
                VgVisualElement renderE = new VgVisualElement(WellknownSvgElementName.Path, spec, renderRoot);
                renderE.VxsPath = ellipse.MakeVxs(v1).CreateTrim();
                return renderE;
            }

        }
        VgVisualElement CreateTestRenderVx_BasicShape()
        {
            var spec = new SvgPathSpec() { FillColor = Color.Red };
            VgVisualDoc renderRoot = new VgVisualDoc();
            VgVisualElement renderE = new VgVisualElement(WellknownSvgElementName.Path, spec, renderRoot);


            using (VxsTemp.Borrow(out VertexStore vxs))
            {
                //red-triangle ***
                vxs.AddMoveTo(10, 10);
                vxs.AddLineTo(60, 10);
                vxs.AddLineTo(60, 30);
                vxs.AddLineTo(10, 30);
                vxs.AddCloseFigure();
                renderE.VxsPath = vxs.CreateTrim();
            }

            return renderE;
        }

        static void LoadRawImg(ImageBinder binder, VgVisualElement vg, object o)
        {
            string imgsrc = binder.ImageSource;
            if (imgsrc != null)
            {

            }
        }
        VgVisualElement CreateTestRenderVx_FromImg(string filename)
        {
            ImageBinder imgBinder = _appHost.LoadImageAndBind(filename);

            var spec = new SvgImageSpec()
            {
                ImageSrc = filename,
                Width = new Css.CssLength(imgBinder.Width, Css.CssUnitOrNames.Pixels),
                Height = new Css.CssLength(imgBinder.Height, Css.CssUnitOrNames.Pixels),
            };

            VgVisualDoc renderRoot = new VgVisualDoc(_vgVisualDocHost);
            VgVisualElement vgimg = new VgVisualElement(WellknownSvgElementName.Image, spec, renderRoot);
            vgimg.ImageBinder = imgBinder;
            return vgimg;
        }
        VgVisualElement CreateTestRenderVx_FromImg2(string filename)
        {

            var spec = new SvgImageSpec()
            {
                ImageSrc = filename,
                Width = new Css.CssLength(50, Css.CssUnitOrNames.Pixels),
                Height = new Css.CssLength(50, Css.CssUnitOrNames.Pixels),
            };

            VgVisualDoc renderRoot = new VgVisualDoc(_vgVisualDocHost);

            VgVisualElement vgimg = new VgVisualElement(WellknownSvgElementName.Image, spec, renderRoot);
            vgimg.ImageBinder = _appHost.LoadImageAndBind(filename);
            return vgimg;
        }

        void UpdateRotationPoints(RotationUI _rotationUI)
        {

        }
        protected override void OnStart(AppHost host)
        {
            _appHost = host;//** 

            //string svgfile = "../Test8_HtmlRenderer.Demo/Samples/Svg/others/cat_simple.svg";
            //string svgfile = "../Test8_HtmlRenderer.Demo/Samples/Svg/others/cat_complex.svg";
            //string svgfile = "../Test8_HtmlRenderer.Demo/Samples/Svg/others/lion.svg";
            string svgfile = "../Test8_HtmlRenderer.Demo/Samples/Svg/others/tiger.svg";
            //return VgVisualElemHelper.ReadSvgFile(svgfile);
            _rotationUI.AngleUpdated += _rotationUI_AngleUpdated;
            //string svgfile = "../Test8_HtmlRenderer.Demo/Samples/Svg/freepik/dog1.svg";
            //string svgfile = "1f30b.svg";
            //string svgfile = "../Data/Svg/twemoji/1f30b.svg";
            //string svgfile = "../Data/1f30b.svg";
            //string svgfile = "../Data/Svg/twemoji/1f370.svg";  

            //_svgRenderVx = CreateTestRenderVx_FromSvg();
            //_svgRenderVx = CreateTestRenderVx_BasicShape();
            //_vgVisualElem = CreateTestRenderVx_FromImg("d:\\WImageTest\\alpha1.png"); 

            //string fontfile = "../Test8_HtmlRenderer.Demo/Samples/Fonts/SOV_Thanamas.ttf";
            //_vgVisualElem = VgVisualElemHelper.CreateVgVisualElementFromGlyph(fontfile, 256, 'a'); //create from glyph

            _vgVisualElem = CreateTestRenderVx_FromImg("d:\\WImageTest\\fenec.png");
            //_vgVisualElem = VgVisualElemHelper.ReadSvgFile(svgfile);


            //PixelFarm.CpuBlit.RectD org_rectD = _svgRenderVx.GetBounds(); 
            //_svgRenderVx = CreateEllipseVxs(org_rectD);

            PixelFarm.CpuBlit.RectD org_rectD = _vgVisualElem.GetRectBounds();
            org_rectD.Offset(-org_rectD.Left, -org_rectD.Bottom);
            //
            _quadController.SetSrcRect(org_rectD.Left, org_rectD.Bottom, org_rectD.Right, org_rectD.Top);
            _quadController.SetDestQuad(
                  org_rectD.Left, org_rectD.Top,
                  org_rectD.Right, org_rectD.Top,
                  org_rectD.Right, org_rectD.Bottom,
                  org_rectD.Left, org_rectD.Bottom);
            //create control point
            _quadController.SetPolygonController(_quadPolygonController);
            _quadController.BuildControlBoxes();
            _quadController.UpdateTransformTarget += (s1, e1) =>
            {
                //after quadController is updated then 
                //we use the coordTransformer to transform target uiSprite
                _uiSprite.SetTransformation(_quadController.GetCoordTransformer());
                _uiSprite.InvalidateOuterGraphics();
                if (_quadController.Left != 0 || _quadController.Top != 0)
                {
                    float xxdiff = _quadController.Left - _uiSprite.Left;
                    float yydiff = _quadController.Top - _uiSprite.Top;

                    _uiSprite.SetLocation(_quadController.Left, _quadController.Top);
                    _uiSprite.InvalidateOuterGraphics();


                    //_rotationUI.InvalidateGraphics();
                    //_rotationUI.SetLocation(
                    //    _rotationUI.Left + xxdiff,
                    //    _rotationUI.Top + yydiff);
                    //_rotationUI.InvalidateGraphics();

                    //_rotationControllerPointUI.InvalidateGraphics();
                    //_rotationControllerPointUI.SetPosition(
                    //   (int)(_rotationControllerPointUI.Left + xxdiff),
                    //   (int)(_rotationControllerPointUI.Top + yydiff));
                    //_rotationControllerPointUI.InvalidateGraphics();

                }
            };



            //_rectBoundsWidgetBox = new Box2(50, 50); //visual rect box
            //Color c = KnownColors.FromKnownColor(KnownColor.DeepSkyBlue);
            //_rectBoundsWidgetBox.BackColor = Color.FromArgb(100, c);
            //_rectBoundsWidgetBox.SetLocation(10, 10);
            /////box1.dbugTag = 1;
            //SetupActiveBoxProperties(_rectBoundsWidgetBox);
            //host.AddChild(_rectBoundsWidgetBox); 
            //_quadController.Visible = _quadPolygonController.Visible = false;
            //_rectBoxController.Init();

            PixelFarm.CpuBlit.RectD svg_bounds = _vgVisualElem.GetRectBounds(); //bounds of graphic shape
            //ICoordTransformer tx = ((ICoordTransformer)_bilinearTx).MultiplyWith(scaleMat);
            ICoordTransformer tx = _quadController.GetCoordTransformer();
            //svgRenderVx._coordTx = tx; 
            //svgRenderVx._coordTx = ((ICoordTransformer)_bilinearTx).MultiplyWith(scaleMat); 
            //host.AddChild(_quadController);
            //host.AddChild(_quadPolygonController);
            //VgRenderVx svgRenderVx = CreateTestRenderVx(); 

            //test transform svgRenderVx 

            _vgVisualElem.DisableBackingImage = true;


            //-----------------------------------------             
            _uiSprite = new UISprite(10, 10); //init size = (10,10), location=(0,0)       
            _uiSprite.DisableBmpCache = true;
            _uiSprite.LoadVg(_vgVisualElem);// 
            _uiSprite.SetTransformation(tx); //set transformation
            host.AddChild(_uiSprite);
            //-----------------------------------------
            //host.AddChild(_quadController);
            host.AddChild(_quadPolygonController);
            {

                PointControllerBox center = new PointControllerBox(10, 10);
                PointControllerBox radius = new PointControllerBox(10, 10);
                host.AddChild(center);
                host.AddChild(radius);
                _rotationUI.AddControlPoints(center, radius);
            }

            _rotationUI.SetReferenceOwner(_quadController);
            _rotationUI.SetCenter(svg_bounds.XCenter, svg_bounds.YCenter);
            _rotationUI.SetRadius(svg_bounds.XCenter + 200, svg_bounds.YCenter);
            host.AddChild(_rotationUI);



            _quadController.Drag += ev =>
            {
                _rotationUI.SetLocation(
                    _rotationUI.Left + ev.XDiff,
                    _rotationUI.Top + ev.YDiff);
            };


            var spriteEvListener = new GeneralEventListener();
            _uiSprite.AttachExternalEventListener(spriteEvListener);
            spriteEvListener.MouseMove += e1 =>
            {

                if (e1.IsDragging)
                {
                    //when drag on sprie 


                    _uiSprite.InvalidateOuterGraphics();
                    _uiSprite.SetLocation(
                        _uiSprite.Left + e1.XDiff,
                        _uiSprite.Top + e1.YDiff
                        );
                    //we also move quadController and _quadPolygonController
                    //
                    _quadController.InvalidateOuterGraphics();
                    _quadController.SetLocation(
                        _quadController.Left + e1.XDiff,
                        _quadController.Top + e1.YDiff);
                    _quadController.InvalidateOuterGraphics();
                    //
                    _quadPolygonController.InvalidateOuterGraphics();
                    _quadPolygonController.SetLocation(
                        _quadPolygonController.Left + e1.XDiff,
                        _quadPolygonController.Top + e1.YDiff
                        );
                    _quadPolygonController.InvalidateOuterGraphics();
                    // 
                    _rotationUI.InvalidateGraphics();
                    _rotationUI.SetLocation(
                        _rotationUI.Left + e1.XDiff,
                        _rotationUI.Top + e1.YDiff);
                    _rotationUI.InvalidateGraphics();

                    //_rotationControllerPointUI.InvalidateGraphics();
                    //_rotationControllerPointUI.SetPosition(
                    //   _rotationControllerPointUI.Left + e1.XDiff,
                    //   _rotationControllerPointUI.Top + e1.YDiff);
                    //_rotationControllerPointUI.InvalidateGraphics();
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
                    //find which part ...

                    double e1_x = e1.X;
                    double e1_y = e1.Y;
                    ICoordTransformer tx1 = _quadController.GetCoordTransformer();
                    if (tx1 != null)
                    {
                        //if the sprite is transformed before render
                        //we must invert x,y back to 
                        ICoordTransformer tx1_inv = tx1.CreateInvert();
                        tx1_inv.Transform(ref e1_x, ref e1_y);
                    }

                    VgHitInfo hitInfo = _uiSprite.FindRenderElementAtPos((float)e1_x, (float)e1_y, true);

                    if (hitInfo.hitElem != null &&
                        hitInfo.hitElem.VxsPath != null)
                    {

                        PixelFarm.CpuBlit.RectD bounds = hitInfo.copyOfVxs.GetBoundingRect();

                        _quadPolygonController.ClearControlPoints();//clear old control points
                        _quadPolygonController.UpdateControlPoints( //create new control points
                            hitInfo.copyOfVxs,
                            _uiSprite.ActualXOffset, _uiSprite.ActualYOffset, tx1);

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
        }
        private void _rotationUI_AngleUpdated(object sender, System.EventArgs e)
        {
            //when angle update 
            //we transform quad at specific angle
            QuadControllerUI.QuadTransformStyle prevStyle = _quadController.TransformStyle;
            _quadController.TransformStyle = QuadControllerUI.QuadTransformStyle.Affine_Rotation;
            double angle = _rotationUI.GetAngleInRad();
            _quadController.ClearCurrentEventSourceBox();
            _quadController.UpdateRotationAngle(angle);
            _quadController.TransformStyle = prevStyle;

        }
    }


}