//MIT, 2014-present, WinterDev

using System.Collections.Generic;
using System.IO;

using PixelFarm.Drawing;
using PixelFarm.CpuBlit.VertexProcessing;
using PaintLab.Svg;
using LayoutFarm.UI;

namespace LayoutFarm
{
    [DemoNote("9.2 ShapeControls")]
    class DemoShapeControl : App
    {
        LayoutFarm.CustomWidgets.PolygonController polygonController = new CustomWidgets.PolygonController();
        LayoutFarm.CustomWidgets.RectBoxController rectBoxController = new CustomWidgets.RectBoxController();



        protected override void OnStart(AppHost host)
        {

            var spec = new SvgPathSpec() { FillColor = Color.Red };
            SvgRenderRootElement renderRoot = new SvgRenderRootElement();
            SvgRenderElement renderE = new SvgRenderElement(WellknownSvgElementName.Path, spec, renderRoot);
            VgRenderVx svgRenderVx = new VgRenderVx(renderE);
            using (VxsTemp.Borrow(out VertexStore vxs))
            {
                //red-triangle ***
                vxs.AddMoveTo(100, 20);
                vxs.AddLineTo(150, 50);
                vxs.AddLineTo(110, 80);
                vxs.AddCloseFigure();
                renderE._vxsPath = vxs.CreateTrim();
            }

            svgRenderVx.DisableBackingImage = true;
            var uiSprite = new UISprite(10, 10); //init size = (10,10), location=(0,0) 
            uiSprite.DisableBmpCache = true;
            uiSprite.LoadVg(svgRenderVx);//
            host.AddChild(uiSprite);

            var spriteEvListener = new GeneralEventListener();
            uiSprite.AttachExternalEventListener(spriteEvListener);



            //box1 = new LayoutFarm.CustomWidgets.SimpleBox(50, 50);
            //box1.BackColor = Color.Red;
            //box1.SetLocation(10, 10);
            ////box1.dbugTag = 1;
            //SetupActiveBoxProperties(box1);
            //viewport.AddContent(box1);
            //-------- 
            rectBoxController.Init();
            //polygonController.Visible = false;
            host.AddChild(polygonController);
            //-------------------------------------------
            host.AddChild(rectBoxController);




            //foreach (var ui in rectBoxController.GetControllerIter())
            //{
            //    viewport.AddContent(ui);
            //}

            spriteEvListener.MouseDown += e1 =>
            {
                //mousedown on ui sprite
                polygonController.SetPosition((int)uiSprite.Left, (int)uiSprite.Top);
                polygonController.SetTargetListener(uiSprite);
                polygonController.UpdateControlPoints(renderE._vxsPath);

            };
            spriteEvListener.MouseMove += e1 =>
            {
                if (e1.IsDragging)
                {
                    //drag event on uisprite

                    int left = (int)uiSprite.Left;
                    int top = (int)uiSprite.Top;

                    int new_left = left + e1.DiffCapturedX;
                    int new_top = top + e1.DiffCapturedY;
                    uiSprite.SetLocation(new_left, new_top);
                    //-----
                    //also update controller position
                    polygonController.SetPosition(new_left, new_top);
                }
            };

        }
        void SetupActiveBoxProperties(LayoutFarm.CustomWidgets.Box box)
        {
            //1. mouse down         
            box.MouseDown += (s, e) =>
            {
                box.BackColor = KnownColors.FromKnownColor(KnownColor.DeepSkyBlue);
                e.MouseCursorStyle = MouseCursorStyle.Pointer;
                //--------------------------------------------
                e.SetMouseCapture(rectBoxController.ControllerBoxMain);
                rectBoxController.UpdateControllerBoxes(box);

            };
            //2. mouse up
            box.MouseUp += (s, e) =>
            {
                e.MouseCursorStyle = MouseCursorStyle.Default;
                box.BackColor = Color.LightGray;
                //controllerBox1.Visible = false;
                //controllerBox1.TargetBox = null;
            };
        }
    }


    sealed class Box2 : LayoutFarm.CustomWidgets.AbstractBox
    {
        public Box2(int w, int h)
            : base(w, h)
        {

        }
        public LayoutFarm.CustomWidgets.AbstractBox FriendBox { get; private set; }
        public UISprite FriendSprite { get; private set; }

        public void SetTarget(LayoutFarm.CustomWidgets.AbstractBox target)
        {
            Clear();
            FriendBox = target;

        }
        public void SetTarget(UISprite target)
        {
            Clear();
            FriendSprite = target;
        }
        void Clear()
        {
            FriendSprite = null;
            FriendBox = null;
        }
        public override void SetLocation(int left, int top)
        {
            int xdiff = left - this.Left;
            int ydiff = top - this.Top;
            if (FriendBox != null)
            {
                FriendBox.SetLocation(FriendBox.Left + xdiff, FriendBox.Top + ydiff);
            }
            else if (FriendSprite != null)
            {
                FriendSprite.SetLocation(FriendSprite.Left + xdiff, FriendSprite.Top + ydiff);
            }
            base.SetLocation(left, top);
        }
        public override void Walk(UIVisitor visitor)
        {
            visitor.BeginElement(this, "box2");
            this.Describe(visitor);
            //descrube child 
            visitor.EndElement();
        }

        public override void NotifyContentUpdate(UIElement childContent)
        {
            //set propersize

            //if (childContent is ImageBox)
            //{
            //    ImageBox imgBox = (ImageBox)childContent;
            //    this.SetSize(imgBox.Width, imgBox.Height); 
            //}

            this.InvalidateLayout();
            //this.ParentUI?.NotifyContentUpdate(this);
            this.ParentUI?.InvalidateLayout();
        }
    }
    [DemoNote("9.2.1 ShapeControls")]
    class DemoShapeControl2 : App
    {
        LayoutFarm.CustomWidgets.PolygonController _polygonController = new CustomWidgets.PolygonController();
        LayoutFarm.CustomWidgets.RectBoxController _rectBoxController = new CustomWidgets.RectBoxController();

        //
        QuadController _quadController = new QuadController();
        LayoutFarm.CustomWidgets.PolygonController _quadPolygonController = new CustomWidgets.PolygonController();
        Bilinear _bilinearTx;

        //

        Box2 _rectBoundsWidgetBox;


        VgRenderVx CreateTestRenderVx()
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
            return ReadSvgFile(svgfile);
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
                SvgRenderRootElement renderRoot = new SvgRenderRootElement();
                SvgRenderElement renderE = new SvgRenderElement(WellknownSvgElementName.Path, spec, renderRoot);
                VgRenderVx svgRenderVx = new VgRenderVx(renderE);
                renderE._vxsPath = ellipse.MakeVxs(v1).CreateTrim();
                return svgRenderVx;
            }

        }
        VgRenderVx CreateTestRenderVx2()
        {
            var spec = new SvgPathSpec() { FillColor = Color.Red };
            SvgRenderRootElement renderRoot = new SvgRenderRootElement();
            SvgRenderElement renderE = new SvgRenderElement(WellknownSvgElementName.Path, spec, renderRoot);
            VgRenderVx svgRenderVx = new VgRenderVx(renderE);

            using (VxsTemp.Borrow(out VertexStore vxs))
            {
                //red-triangle ***
                vxs.AddMoveTo(100, 20);
                vxs.AddLineTo(150, 50);
                vxs.AddLineTo(110, 80);
                vxs.AddCloseFigure();
                renderE._vxsPath = vxs.CreateTrim();
            }

            return svgRenderVx;
        }


        Typography.Contours.GlyphMeshStore _glyphMaskStore = new Typography.Contours.GlyphMeshStore();
        VgRenderVx CreateTestRenderVx3(char c, float sizeInPts)
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
            SvgRenderRootElement renderRoot = new SvgRenderRootElement();
            SvgRenderElement renderE = new SvgRenderElement(WellknownSvgElementName.Path, spec, renderRoot);
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

        bool _hitTestOnSubPath = true;
        VgRenderVx _svgRenderVx;

        void UpdateTransformedShape(object sender, System.EventArgs e)
        {
            //_quadController.GetInnerCoords(
            //  //src
            //  out double src_left, out double src_top,
            //  out double src_w, out double src_h,
            //  //dest
            //  out double dst_x0, out double dst_y0,
            //  out double dst_x1, out double dst_y1,
            //  out double dst_x2, out double dst_y2,
            //  out double dst_x3, out double dst_y3);

            _quadController.GetInnerCoords(
                //src
                out double src_left, out double src_bottom,
                out double src_right, out double src_top,
                //dest
                out double dst_x0, out double dst_y0, //left,top
                out double dst_x1, out double dst_y1, //right,top
                out double dst_x2, out double dst_y2, //right,bottom
                out double dst_x3, out double dst_y3); //left,bottom

            //_bilinearTx = Bilinear.RectToQuad(src_left, src_bottom, src_right, src_top,
            //    new double[] {
            //        dst_x0, dst_y0,
            //        dst_x1, dst_y1,
            //        dst_x2, dst_y2,
            //        dst_x3, dst_y3
            //    });
            _bilinearTx = Bilinear.RectToQuad(src_left, src_bottom, src_right, src_top,
             new double[] {
                    dst_x3, dst_y3,
                    dst_x2, dst_y2,
                    dst_x1, dst_y1,
                    dst_x0, dst_y0
             });

            //PixelFarm.CpuBlit.RectD svg_bounds = _svgRenderVx.GetBounds();

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
            ICoordTransformer tx = _bilinearTx;
            _uiSprite.SetTransformation(tx); //set transformation
        }

        UISprite _uiSprite;

        protected override void OnStart(AppHost host)
        {
            //_svgRenderVx = CreateTestRenderVx();
            _svgRenderVx = CreateTestRenderVx3('a', 128); //create from glyph
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

            _quadPolygonController.UpdateControlPoints(_quadController.OutlineVxs);
            _quadController.SetPolygonController(_quadPolygonController);
            _quadPolygonController.SetTargetListener(_quadController);
            //
            _quadController.GetInnerCoords(
               //src
               out double src_left, out double src_top,
               out double src_w, out double src_h,
               //dest
               out double dst_x0, out double dst_y0,
               out double dst_x1, out double dst_y1,
               out double dst_x2, out double dst_y2,
               out double dst_x3, out double dst_y3);

            _bilinearTx = Bilinear.RectToQuad(src_left, src_top, src_left + src_w, src_top + src_h,
                new double[] {
                    dst_x0, dst_y0,
                    dst_x1, dst_y1,
                    dst_x2, dst_y2,
                    dst_x3, dst_y3
                });
            if (!_bilinearTx.IsValid)
            {

            }
            // 

            _rectBoundsWidgetBox = new Box2(50, 50); //visual rect box
            Color c = KnownColors.FromKnownColor(KnownColor.DeepSkyBlue);
            _rectBoundsWidgetBox.BackColor = Color.FromArgb(100, c);
            _rectBoundsWidgetBox.SetLocation(10, 10);
            //box1.dbugTag = 1;
            SetupActiveBoxProperties(_rectBoundsWidgetBox);
            host.AddChild(_rectBoundsWidgetBox);

            _rectBoxController.Init();

            //VgRenderVx svgRenderVx = CreateTestRenderVx(); //create from svg
            //test...
            //1. scale svg to fix the 'src rect'  
            //2. then transform to the 'dest rect' 

            PixelFarm.CpuBlit.RectD svg_bounds = _svgRenderVx.GetBounds();

            double w_scale = src_w / svg_bounds.Width;
            double h_scale = src_h / svg_bounds.Height;

            double actualXOffset = -svg_bounds.Left;
            double actualYOffset = -svg_bounds.Bottom;

            Affine scaleMat = Affine.NewMatix(
                AffinePlan.Translate(
                    actualXOffset - svg_bounds.Width / 2, //move to its middle point
                    actualYOffset - svg_bounds.Height / 2),//move to its middle point
                AffinePlan.Scale(w_scale, h_scale),
                AffinePlan.Translate(
                    -(actualXOffset - svg_bounds.Width / 2) * w_scale,//move back
                    -(actualYOffset - svg_bounds.Height / 2) * h_scale)); //move back

            //ICoordTransformer tx = ((ICoordTransformer)_bilinearTx).MultiplyWith(scaleMat);
            ICoordTransformer tx = _bilinearTx;
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


            _quadController.ElemUpdate += UpdateTransformedShape;
            UpdateTransformedShape(this, System.EventArgs.Empty);

            //***
            //tx.Transform(ref actualXOffset, ref actualYOffset); //we need to translate actual offset too!
            //_uiSprite.SetActualLeftTop(actualXOffset, actualYOffset);

            //double newXOffset = (-svg_bounds.Left + svg_bounds.Width) * w_scale;// - svg_bounds.Width / 2) * w_scale + (svg_bounds.Width * w_scale / 2);
            //double newYOffset = (-svg_bounds.Top + svg_bounds.Height) * h_scale;

            //_uiSprite.SetActualLeftTop(newXOffset, newYOffset);

            //_uiSprite.SetBounds(
            //   (float)(svg_bounds.Left * w_scale), (float)(svg_bounds.Top * h_scale),
            //   (float)(svg_bounds.Width * w_scale), (float)(svg_bounds.Height * h_scale));


            host.AddChild(_polygonController);
            host.AddChild(_rectBoxController);


            host.AddChild(_uiSprite);

            var spriteEvListener = new GeneralEventListener();
            _uiSprite.AttachExternalEventListener(spriteEvListener);

            spriteEvListener.MouseDown += e1 =>
            {
                //mousedown on ui sprite 
                //find exact part ...  

                //_quadController.BringToTopMost();
                _polygonController.BringToTopMost();

                if (_hitTestOnSubPath)
                {
                    SvgHitInfo hitInfo = _uiSprite.FindRenderElementAtPos(e1.X, e1.Y, true);
                    if (hitInfo.svg != null &&
                        hitInfo.svg._vxsPath != null)
                    {

                        PixelFarm.CpuBlit.RectD bounds = hitInfo.copyOfVxs.GetBoundingRect();

                        _polygonController.UpdateControlPoints(hitInfo.copyOfVxs, _uiSprite.ActualXOffset, _uiSprite.ActualYOffset);

                        //move redbox and its controller
                        _rectBoundsWidgetBox.SetLocationAndSize(
                            (int)(bounds.Left + _uiSprite.ActualXOffset), (int)(bounds.Top - bounds.Height + _uiSprite.ActualYOffset),
                            (int)bounds.Width, (int)bounds.Height);
                        _rectBoxController.UpdateControllerBoxes(_rectBoundsWidgetBox);

                        _rectBoundsWidgetBox.Visible = true;
                        _rectBoxController.Visible = true;



                        //show bounds
                    }
                    else
                    {
                        _rectBoundsWidgetBox.Visible = false;
                        _rectBoxController.Visible = false;
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
                            _polygonController.UpdateControlPoints(v1.CreateTrim());
                        }
                    }
                    else
                    {
                        _rectBoundsWidgetBox.SetTarget(_uiSprite);

                        _rectBoundsWidgetBox.SetLocationAndSize(    //blue
                              (int)_uiSprite.Left, (int)_uiSprite.Top,
                              (int)_uiSprite.Width, (int)_uiSprite.Height);

                        _rectBoxController.UpdateControllerBoxes(_rectBoundsWidgetBox);  //control 4 corners
                        _rectBoundsWidgetBox.Visible = true;
                        _rectBoxController.Visible = true;
                    }
                    //polygonController.SetPosition((int)uiSprite.Left, (int)uiSprite.Top);
                    //polygonController.SetTargetUISprite(uiSprite);
                    //polygonController.UpdateControlPoints(svgRenderVx._renderE._vxsPath);
                }
            };



        }



        void SetupActiveBoxProperties(LayoutFarm.CustomWidgets.AbstractBox box)
        {
            //1. mouse down         
            box.MouseDown += (s, e) =>
            {
                Color c = KnownColors.FromKnownColor(KnownColor.DeepSkyBlue);
                box.BackColor = Color.FromArgb(100, c);
                e.MouseCursorStyle = MouseCursorStyle.Pointer;
                //--------------------------------------------
                e.SetMouseCapture(_rectBoxController.ControllerBoxMain);
                _rectBoxController.UpdateControllerBoxes(box);

            };
            //2. mouse up
            box.MouseUp += (s, e) =>
            {
                e.MouseCursorStyle = MouseCursorStyle.Default;
                //box.BackColor = Color.LightGray;
                //controllerBox1.Visible = false;
                //controllerBox1.TargetBox = null;
            };
        }

        VgRenderVx ReadSvgFile(string filename)
        {

            string svgContent = System.IO.File.ReadAllText(filename);
            SvgDocBuilder docBuidler = new SvgDocBuilder();
            SvgParser parser = new SvgParser(docBuidler);
            WebLexer.TextSnapshot textSnapshot = new WebLexer.TextSnapshot(svgContent);
            parser.ParseDocument(textSnapshot);
            //TODO: review this step again
            SvgRenderVxDocBuilder builder = new SvgRenderVxDocBuilder();
            return builder.CreateRenderVx(docBuidler.ResultDocument, svgElem =>
            {

            });
        }
    }


    public class QuadController : UISprite
    {
        double _src_left, _src_top, _src_w, _src_h;
        bool _setDestRect;

        double _x0, _y0,
               _x1, _y1,
               _x2, _y2,
               _x3, _y3;
        public event System.EventHandler ElemUpdate;

        public QuadController()
            : base(100, 100)
        {

        }

        LayoutFarm.CustomWidgets.PolygonController _polygonController;
        public void SetPolygonController(LayoutFarm.CustomWidgets.PolygonController polygonController)
        {
            _polygonController = polygonController;
        }
        protected override void OnElementChanged()
        {
            //base.OnElementChanged();
            //update dest quad
            List<CustomWidgets.UIControllerBox> controlBoxes = _polygonController.ControlBoxes;

            SetDestQuad(
                controlBoxes[0].Left, controlBoxes[0].Top,
                controlBoxes[1].Left, controlBoxes[1].Top,
                controlBoxes[2].Left, controlBoxes[2].Top,
                controlBoxes[3].Left, controlBoxes[3].Top
                );
            _polygonController.UpdateControlPoints(_outlineVxs);

            ElemUpdate?.Invoke(this, System.EventArgs.Empty);
            //
        }

        public void SetSrcRect(double left, double top, double w, double h)
        {
            _src_left = left;
            _src_top = top;
            _src_w = w;
            _src_h = h;
            //

            if (!_setDestRect)
            {
                LoadVg(CreateQuadVgFromSrcRect());
            }
        }
        //
        VgRenderVx _svgRenderVx;
        SvgRenderElement _renderE;
        VertexStore _outlineVxs;
        internal VertexStore OutlineVxs => _outlineVxs;

        VgRenderVx CreateQuadVgFromSrcRect()
        {
            var spec = new SvgPathSpec() { FillColor = Color.Aqua };
            SvgRenderRootElement renderRoot = new SvgRenderRootElement();
            SvgRenderElement renderE = new SvgRenderElement(WellknownSvgElementName.Path, spec, renderRoot);
            VgRenderVx svgRenderVx = new VgRenderVx(renderE);

            using (VectorToolBox.Borrow(out SimpleRect rect))
            using (VxsTemp.Borrow(out VertexStore v1))
            {

                rect.SetRect(_src_left, _src_top + _src_h, _src_left + _src_w, _src_top);
                rect.MakeVxs(v1);

                //
                _outlineVxs = renderE._vxsPath = v1.CreateTrim();
                _renderE = renderE;
                _svgRenderVx = svgRenderVx;

            }
            return svgRenderVx;
        }

        VgRenderVx CreateQuadVgFromDestQuad()
        {
            var spec = new SvgPathSpec() { FillColor = Color.Green };
            SvgRenderRootElement renderRoot = new SvgRenderRootElement();
            SvgRenderElement renderE = new SvgRenderElement(WellknownSvgElementName.Path, spec, renderRoot);
            VgRenderVx svgRenderVx = new VgRenderVx(renderE);


            using (VxsTemp.Borrow(out var v1, out var v2))
            using (VectorToolBox.Borrow(out Stroke stroke))
            {
                stroke.Width = 2;
                v1.AddMoveTo(_x0, _y0);
                v1.AddLineTo(_x1, _y1);
                v1.AddLineTo(_x2, _y2);
                v1.AddLineTo(_x3, _y3);
                v1.AddCloseFigure();
                v1.AddNoMore();

                _outlineVxs = v1.CreateTrim();//outline vxs***
                stroke.MakeVxs(v1, v2); //create stroke path around v1

                //
                renderE._vxsPath = v2.CreateTrim();
                _renderE = renderE;
                _svgRenderVx = svgRenderVx;
            }
            return svgRenderVx;
        }
        public void SetDestQuad(double x0, double y0,
            double x1, double y1,
            double x2, double y2,
            double x3, double y3)
        {
            _setDestRect = true;
            _x0 = x0;
            _y0 = y0;
            //
            _x1 = x1;
            _y1 = y1;
            //
            _x2 = x2;
            _y2 = y2;
            //
            _x3 = x3;
            _y3 = y3;

            LoadVg(CreateQuadVgFromDestQuad());
        }

        public void GetInnerCoords(out double src_left, out double src_top,
                out double src_w, out double src_h,
                out double x0, out double y0,
                out double x1, out double y1,
                out double x2, out double y2,
                out double x3, out double y3)
        {
            src_left = _src_left;
            src_top = _src_top;
            src_w = _src_w;
            src_h = _src_h;
            //
            //
            x0 = _x0; y0 = _y0;
            x1 = _x1; y1 = _y1;
            x2 = _x2; y2 = _y2;
            x3 = _x3; y3 = _y3;
        }

    }

}