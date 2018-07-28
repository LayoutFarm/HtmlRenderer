////MS-PL, 
////Apache2, 2014-present, WinterDev



//using LayoutFarm.Css;
//using LayoutFarm.HtmlBoxes;
//using PixelFarm.Drawing;
//namespace LayoutFarm.Svg
//{
//    public class SvgImage : SvgVisualElement
//    {
//        SvgImageSpec imageSpec;
//        GraphicsPath _path;
//        LayoutFarm.HtmlBoxes.CssImageRun _imgRun;
//        public SvgImage(SvgImageSpec spec, object controller)
//            : base(controller)
//        {
//            this.imageSpec = spec;
//            this._imgRun = new LayoutFarm.HtmlBoxes.CssImageRun();
//        }
//        //----------------------------
//        public float ActualX
//        {
//            get;
//            set;
//        }
//        public float ActualY
//        {
//            get;
//            set;
//        }
//        public float ActualWidth
//        {
//            get;
//            set;
//        }
//        public float ActualHeight
//        {
//            get;
//            set;
//        }
//        //----------------------------
//        public ImageBinder ImageBinder
//        {
//            get { return this._imgRun.ImageBinder; }
//            set { this._imgRun.ImageBinder = value; }
//        }


//        public override void ReEvaluateComputeValue(ref ReEvaluateArgs args)
//        {
//            var myspec = this.imageSpec;
//            this.fillColor = myspec.FillColor;
//            this.strokeColor = myspec.StrokeColor;
//            this.ActualX = ConvertToPx(myspec.X, ref args);
//            this.ActualY = ConvertToPx(myspec.Y, ref args);
//            this.ActualWidth = ConvertToPx(myspec.Width, ref args);
//            this.ActualHeight = ConvertToPx(myspec.Height, ref args);
//            this.ActualStrokeWidth = ConvertToPx(myspec.StrokeWidth, ref args);
//            this._path = CreateRectGraphicPath(
//                    this.ActualX,
//                    this.ActualY,
//                    this.ActualWidth,
//                    this.ActualHeight);
//            if (this._imgRun.ImageBinder == null)
//            {
//                this._imgRun.ImageBinder = new SvgImageBinder(myspec.ImageSrc);
//            }
//            ValidatePath();
//        }
//        public override void Paint(PaintVisitor p)
//        {
//            //DrawBoard g = p.InnerCanvas;
//            if (fillColor.A > 0)
//            {
//                p.FillPath(_path, this.fillColor);
//            }
//            //---------------------------------------------------------  
//            if (this.ImageBinder != null)
//            {
//                //---------------------------------------------------------  
//                //Because we need external image resource , so ...
//                //use render technique like CssBoxImage ****
//                //---------------------------------------------------------  

//                RectangleF r = new RectangleF(this.ActualX, this.ActualY, this.ActualWidth, this.ActualHeight);
//                bool tryLoadOnce = false;
//                EVAL_STATE:
//                switch (this.ImageBinder.State)
//                {
//                    case BinderState.Unload:
//                        {
//                            //async request image
//                            if (!tryLoadOnce)
//                            {
//                                p.RequestImageAsync(_imgRun.ImageBinder, this._imgRun, this);
//                                //retry again
//                                tryLoadOnce = true;
//                                goto EVAL_STATE;
//                            }
//                        }
//                        break;
//                    case BinderState.Loading:
//                        {
//                            //RenderUtils.DrawImageLoadingIcon(g, r);
//                        }
//                        break;
//                    case BinderState.Loaded:
//                        {
//                            Image img;
//                            if ((img = _imgRun.ImageBinder.Image) != null)
//                            {
//                                if (_imgRun.ImageRectangle == Rectangle.Empty)
//                                {
//                                    p.DrawImage(img, r);

//                                }
//                                else
//                                {
//                                    //
//                                    p.DrawImage(img, _imgRun.ImageRectangle);
//                                }
//                            }
//                            else
//                            {
//                                RenderUtils.DrawImageLoadingIcon(p, r);
//                                if (r.Width > 19 && r.Height > 19)
//                                {
//                                    p.DrawImage(img, _imgRun.ImageRectangle);
//                                    p.DrawRectangle(Color.LightGray, r.X, r.Y, r.Width, r.Height);
//                                }
//                            }
//                        }
//                        break;
//                    case BinderState.Blank:
//                        {
//                        }
//                        break;
//                    case BinderState.Error:
//                        {
//                            RenderUtils.DrawImageErrorIcon(p, r);
//                        }
//                        break;
//                }
//            }
//            //--------------------------------------------------------- 
//            if (this.strokeColor.A > 0
//                && this.ActualStrokeWidth > 0)
//            {
//                p.DrawPath(_path, strokeColor, ActualStrokeWidth);
//            }
//        }
//        static GraphicsPath CreateRectGraphicPath(float x, float y, float w, float h)
//        {
//            var _path = new GraphicsPath();
//            _path.StartFigure();
//            _path.AddRectangle(new RectangleF(x, y, w, h));
//            _path.CloseFigure();
//            return _path;
//        }
//    }

//    public class SvgImageSpec : SvgVisualSpec
//    {
//        public CssLength X
//        {
//            get;
//            set;
//        }
//        public CssLength Y
//        {
//            get;
//            set;
//        }
//        public CssLength Width
//        {
//            get;
//            set;
//        }
//        public CssLength Height
//        {
//            get;
//            set;
//        }

//        public string ImageSrc
//        {
//            get;
//            set;
//        }
//    }
//}