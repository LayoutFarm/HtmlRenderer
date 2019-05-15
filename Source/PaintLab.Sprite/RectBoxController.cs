//-----------------------------------------------------------------
//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
using LayoutFarm.UI;

namespace LayoutFarm.CustomWidgets
{
    public class UIControllerBox : LayoutFarm.CustomWidgets.AbstractBox
    {
        public UIControllerBox(int w, int h)
            : base(w, h)
        {
        }
        public LayoutFarm.UI.AbstractRectUI TargetBox
        {
            get;
            set;
        }
        public override void Walk(UIVisitor visitor)
        {
            visitor.BeginElement( "ctrlbox");
            this.Describe(visitor);
            visitor.EndElement();
        }

        public int Index { get; set; }
        public MoveDirection MoveDirection { get; set; }

        //public double SrcX { get; set; }
        //public double SrcY { get; set; }
        //public double TargetX { get; set; }
        //public double TargetY { get; set; }

#if DEBUG
        public override string ToString() => Left + "," + Top;
#endif

    }
    public enum MoveDirection
    {
        Both,
        XAxis,
        YAxis,
    }


    public class RectBoxController : UIElement
    {

        //TODO: review how to configure the controller box ...

        //-------------
        //corners
        UIControllerBox _boxLeftTop = new UIControllerBox(20, 20) { BackColor = Color.Yellow };
        UIControllerBox _boxLeftBottom = new UIControllerBox(20, 20) { BackColor = Color.Yellow };
        UIControllerBox _boxRightTop = new UIControllerBox(20, 20) { BackColor = Color.Yellow };
        UIControllerBox _boxRightBottom = new UIControllerBox(20, 20) { BackColor = Color.Yellow };
        //-------------

        //mid 
        UIControllerBox _midLeftSide = new UIControllerBox(20, 20) { BackColor = Color.Yellow };
        UIControllerBox _midRightSide = new UIControllerBox(20, 20) { BackColor = Color.Yellow };
        UIControllerBox _midTop = new UIControllerBox(20, 20) { BackColor = Color.Yellow };
        UIControllerBox _midBottom = new UIControllerBox(20, 20) { BackColor = Color.Yellow };

        //-------------

        UIControllerBox _centralBox = new UIControllerBox(40, 40);
        List<UIControllerBox> _controls = new List<UIControllerBox>();

        Box _groundBox;
        bool _hasPrimRenderE;
        public RectBoxController()
        {
            _groundBox = new Box(10, 10);
            _groundBox.BackColor = Color.Transparent;//*** 
            _groundBox.NeedClipArea = false;
        }
        //-------------
        public override void InvalidateGraphics()
        {
            if (this.HasReadyRenderElement)
            {
                this.CurrentPrimaryRenderElement.InvalidateGraphics();
            }
        }
        //
        public override object Tag { get; set; }
        public UIControllerBox CentralBox => _centralBox;
        public List<UIControllerBox> ControlBoxes => _controls;
        //
        protected override bool HasReadyRenderElement => _hasPrimRenderE;
        //
        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {
            _hasPrimRenderE = true;
            return _groundBox.GetPrimaryRenderElement(rootgfx);
        }
        public override void Walk(UIVisitor visitor)
        {
        }
        public override RenderElement CurrentPrimaryRenderElement => _groundBox.CurrentPrimaryRenderElement;
        //-------------

        public void SetPosition(int x, int y)
        {
            //TODO: review here again***
            //temp fix for invalidate area of overlap children
            _groundBox.InvalidateOuterGraphics();
            foreach (var ctrl in _controls)
            {
                ctrl.InvalidateOuterGraphics();
            }
            _groundBox.SetLocation(x, y);
        }

        public override void Focus()
        {
            _centralBox.AcceptKeyboardFocus = true;
            _centralBox.Focus();
        }
        public void UpdateControllerBoxes(LayoutFarm.UI.AbstractRectUI targetBox)
        {

            //move controller here 
            _centralBox.SetLocationAndSize(
                            targetBox.Left - 5, targetBox.Top - 5,
                            targetBox.Width + 10, targetBox.Height + 10);
            _centralBox.Visible = true;
            _centralBox.TargetBox = targetBox;
            //------------
            //corners 
            //------------
            {
                //left-top
                UIControllerBox ctrlBox = _boxLeftTop;
                ctrlBox.SetLocationAndSize(targetBox.Left - 5, targetBox.Top - 5, 5, 5);
                ctrlBox.TargetBox = targetBox;
                ctrlBox.Visible = true;
            }
            {
                //right-top
                UIControllerBox ctrlBox = _boxRightTop;
                ctrlBox.SetLocationAndSize(targetBox.Left + targetBox.Width, targetBox.Top - 5, 5, 5);
                ctrlBox.TargetBox = targetBox;
                ctrlBox.Visible = true;
            }
            {
                //left-bottom
                UIControllerBox ctrlBox = _boxLeftBottom;
                ctrlBox.SetLocationAndSize(targetBox.Left - 5, targetBox.Top + targetBox.Height, 5, 5);
                ctrlBox.TargetBox = targetBox;
                ctrlBox.Visible = true;
            }
            {
                //right-bottom
                UIControllerBox ctrlBox = _boxRightBottom;
                ctrlBox.SetLocationAndSize(targetBox.Left + targetBox.Width, targetBox.Top + targetBox.Height, 5, 5);
                ctrlBox.TargetBox = targetBox;
                ctrlBox.Visible = true;
            }


            //------------
            //mid-edges
            //------------
            {
                //left 
                UIControllerBox ctrlBox = _midLeftSide;
                ctrlBox.SetLocationAndSize(targetBox.Left - 5, (targetBox.Top - 5) + (targetBox.Height / 2), 5, 5);
                ctrlBox.TargetBox = targetBox;
                ctrlBox.Visible = true;
            }
            {
                //right 
                UIControllerBox ctrlBox = _midRightSide;
                ctrlBox.SetLocationAndSize(targetBox.Left + targetBox.Width, targetBox.Top - 5 + (targetBox.Height / 2), 5, 5);
                ctrlBox.TargetBox = targetBox;
                ctrlBox.Visible = true;
            }
            {
                //top
                UIControllerBox ctrlBox = _midTop;
                ctrlBox.SetLocationAndSize(targetBox.Left - 5 + (targetBox.Width / 2), targetBox.Top - 5, 5, 5);
                ctrlBox.TargetBox = targetBox;
                ctrlBox.Visible = true;
            }
            {
                // bottom
                UIControllerBox ctrlBox = _midBottom;
                ctrlBox.SetLocationAndSize(targetBox.Left + (targetBox.Width / 2), targetBox.Bottom, 5, 5);
                ctrlBox.TargetBox = targetBox;
                ctrlBox.Visible = true;
            }
            OnUpdateShape();
        }
        protected virtual void OnUpdateShape()
        {

        }
        void InitCornerControlBoxes()
        {

            _boxLeftTop = new UIControllerBox(20, 20) { BackColor = Color.Yellow, Visible = false };
            SetupCorner_Controller(_boxLeftTop);
            _boxLeftTop.MouseDrag += (s1, e1) =>
            {
                //move other boxes ...
                AbstractRectUI target1 = _boxLeftTop.TargetBox;
                //update target
                if (target1 != null)
                {
                    target1.SetLocationAndSize(
                                        _boxLeftTop.Right,
                                       _boxLeftTop.Bottom,
                                          _boxRightTop.Left - _boxLeftTop.Right,
                                          _boxRightBottom.Top - _boxLeftTop.Bottom);
                    //update other controller
                    UpdateControllerBoxes(target1);
                }

            };
            _groundBox.AddChild(_boxLeftTop);
            //------------
            _boxLeftBottom = new UIControllerBox(20, 20) { BackColor = Color.Yellow, Visible = false };
            SetupCorner_Controller(_boxLeftBottom);
            _boxLeftBottom.MouseDrag += (s1, e1) =>
            {
                AbstractRectUI target1 = _boxLeftBottom.TargetBox;
                //update target
                if (target1 != null)
                {
                    target1.SetLocationAndSize(_boxLeftBottom.Right,
                                    _boxLeftTop.Bottom,
                                    _boxRightTop.Left - _boxLeftBottom.Right,
                                    _boxLeftBottom.Top - _boxLeftTop.Bottom);
                    //update other controller
                    UpdateControllerBoxes(target1);
                }

            };
            _groundBox.AddChild(_boxLeftBottom);
            //------------ 

            _boxRightTop = new UIControllerBox(20, 20) { BackColor = Color.Yellow, Visible = false };
            SetupCorner_Controller(_boxRightTop);
            _boxRightTop.MouseDrag += (s1, e1) =>
            {
                AbstractRectUI target1 = _boxRightTop.TargetBox;
                //update target
                if (target1 != null)
                {
                    target1.SetLocationAndSize(
                                          _boxLeftTop.Right,
                                          _boxRightTop.Bottom,
                                          _boxRightTop.Left - _boxLeftTop.Right,
                                          _boxRightBottom.Top - _boxRightTop.Bottom);
                    //update other controllers
                    UpdateControllerBoxes(target1);
                }
            };
            _groundBox.AddChild(_boxRightTop);

            //------------ 
            _boxRightBottom = new UIControllerBox(20, 20) { BackColor = Color.Yellow, Visible = false };
            SetupCorner_Controller(_boxRightBottom);
            _boxRightBottom.MouseDrag += (s1, e1) =>
            {

                OnUpdateShape();
                //rotate all other control points
                //AbstractRectUI target1 = _boxRightBottom.TargetBox;



                ////update target
                //if (target1 != null)
                //{
                //    target1.SetLocationAndSize(_boxLeftTop.Right,
                //                      _boxLeftTop.Bottom,
                //                      _boxRightBottom.Left - _boxLeftTop.Right,
                //                      _boxRightBottom.Top - _boxLeftTop.Bottom);
                //    //update other controllers
                //    UpdateControllerBoxes(target1);
                //}

            };
            _groundBox.AddChild(_boxRightBottom);
            //------------ 

        }
        void SetupEdge_Controller(UIControllerBox box)
        {
            Color c = KnownColors.FromKnownColor(KnownColor.Blue);
            box.BackColor = c;// new Color(200, c.R, c.G, c.B);
            box.SetLocation(200, 200);
            box.Visible = true;
            switch (box.MoveDirection)
            {
                case MoveDirection.XAxis:
                    {
                        box.MouseDrag += (s, e) =>
                        {
                            Point pos = box.Position;
                            box.SetLocation(pos.X + e.XDiff, pos.Y);
                            //var targetBox = cornerBox.TargetBox;
                            //if (targetBox != null)
                            //{
                            //    //move target box too
                            //    targetBox.SetLocation(newX + 5, newY + 5);
                            //}

                            e.CancelBubbling = true;
                        };
                    }
                    break;
                case MoveDirection.YAxis:
                    {
                        box.MouseDrag += (s, e) =>
                        {
                            Point pos = box.Position;
                            box.SetLocation(pos.X, pos.Y + e.YDiff);
                            //var targetBox = cornerBox.TargetBox;
                            //if (targetBox != null)
                            //{
                            //    //move target box too
                            //    targetBox.SetLocation(newX + 5, newY + 5);
                            //}

                            e.CancelBubbling = true;
                        };
                    }
                    break;
            }

            _controls.Add(box);
        }
        void InitEdgeControlBoxes()
        {
            _midLeftSide = new UIControllerBox(20, 20) { MoveDirection = MoveDirection.XAxis, BackColor = Color.Blue, Visible = false };
            SetupEdge_Controller(_midLeftSide);
            _midLeftSide.MouseDrag += (s1, e1) =>
            {
                //move other boxes ...
                AbstractRectUI target1 = _boxLeftTop.TargetBox;
                //update target
                if (target1 != null)
                {
                    //change left side x and width
                    int xdiff = _midLeftSide.Right - target1.Left;
                    target1.SetLocationAndSize(
                                          target1.Left + xdiff,
                                          target1.Top,
                                          target1.Width - xdiff,
                                          target1.Height);
                    //update other controller
                    UpdateControllerBoxes(target1);
                }

            };
            _groundBox.AddChild(_midLeftSide);
            //------------
            _midRightSide = new UIControllerBox(20, 20) { MoveDirection = MoveDirection.XAxis, BackColor = Color.Blue, Visible = false };
            SetupEdge_Controller(_midRightSide);
            _midRightSide.MouseDrag += (s1, e1) =>
            {
                AbstractRectUI target1 = _boxLeftTop.TargetBox;
                //change left side x and width
                int xdiff = _midRightSide.Left - target1.Right;
                target1.SetLocationAndSize(
                                        target1.Left,
                                        target1.Top,
                                        target1.Width + xdiff,
                                        target1.Height);

                //update other controller
                UpdateControllerBoxes(target1);

            };
            _groundBox.AddChild(_midRightSide);
            //------------ 

            _midTop = new UIControllerBox(20, 20) { MoveDirection = MoveDirection.YAxis, BackColor = Color.Blue, Visible = false };
            SetupEdge_Controller(_midTop);
            _midTop.MouseDrag += (s1, e1) =>
            {
                AbstractRectUI target1 = _boxLeftTop.TargetBox;
                int ydiff = target1.Top - _midTop.Bottom;
                target1.SetLocationAndSize(
                                        target1.Left,
                                        target1.Top - ydiff,
                                        target1.Width,
                                        target1.Height + ydiff);
                //update other controller
                UpdateControllerBoxes(target1);
            };
            _groundBox.AddChild(_midTop);

            //------------ 
            _midBottom = new UIControllerBox(20, 20) { MoveDirection = MoveDirection.YAxis, BackColor = Color.Blue, Visible = false };
            SetupEdge_Controller(_midBottom);
            _midBottom.MouseDrag += (s1, e1) =>
            {
                AbstractRectUI target1 = _boxLeftTop.TargetBox;
                int ydiff = _midBottom.Top - target1.Bottom;

                target1.SetLocationAndSize(
                                        target1.Left,
                                        target1.Top,
                                        target1.Width,
                                        target1.Height + ydiff);
                //update other controller
                UpdateControllerBoxes(target1);
            };
            _groundBox.AddChild(_midBottom);
            //------------ 
        }
        public void Init()
        {
            //------------
            _centralBox = new UIControllerBox(40, 40);
            {
                Color c = KnownColors.FromKnownColor(KnownColor.Yellow);
                _centralBox.BackColor = new Color(100, c.R, c.G, c.B);
                _centralBox.SetLocation(200, 200);
                //controllerBox1.dbugTag = 3;
                _centralBox.Visible = false;
                SetupControllerBoxProperties(_centralBox);
                //viewport.AddChild(controllerBox1);
                _controls.Add(_centralBox);
            }
            _groundBox.AddChild(_centralBox);
            //------------
            InitCornerControlBoxes();
            InitEdgeControlBoxes();
            //------------
        }

        public AbstractBox ControllerBoxMain
        {
            get { return _centralBox; }
        }
        void SetupControllerBoxProperties(UIControllerBox controllerBox)
        {
            //for controller box  
            controllerBox.MouseDrag += (s, e) =>
            {
                Point pos = controllerBox.Position;
                int newX = pos.X + e.XDiff;
                int newY = pos.Y + e.YDiff;

                //also move controller box?
                int j = _controls.Count;
                for (int i = 0; i < j; ++i)
                {
                    UIControllerBox corner = _controls[i];
                    Point p2 = corner.Position;
                    int newX2 = p2.X + e.XDiff;
                    int newY2 = p2.Y + e.YDiff;
                    corner.SetLocation(newX2, newY2);
                }
                var targetBox = controllerBox.TargetBox;
                if (targetBox != null)
                {
                    //move target box too
                    targetBox.SetLocation(newX + 5, newY + 5);
                }

                e.CancelBubbling = true;
            };
        }



        double _mouseDownX;
        double _mouseDownY;
        double _actualX;
        double _actualY;
        bool firstTime = true;
        double _firstTimeRad;

        //***
        public double _rotateAngleDiff;
        //
        void SetupCorner_Controller(UIControllerBox box)
        {
            Color c = KnownColors.FromKnownColor(KnownColor.Orange);
            box.BackColor = c;// new Color(200, c.R, c.G, c.B);
            box.SetLocation(200, 200);
            box.Visible = true;

            box.MouseDrag += (s, e) =>
            {
                Point pos = box.Position;
                double x1 = pos.X;
                double y1 = pos.Y;

                if (firstTime)
                {
                    _mouseDownX = x1;
                    _mouseDownY = y1;
                    firstTime = false;
                    //find rad of firsttime
                    _firstTimeRad = Math.Atan2(y1, x1);
                    _rotateAngleDiff = 0;
                }
                else
                {
                    double newX = _actualX + e.XDiff;
                    double newY = _actualY + e.YDiff;

                    //find new angle
                    double thisTimeRad = Math.Atan2(newY, newX);
                    _rotateAngleDiff = thisTimeRad - _firstTimeRad;

                    x1 = _mouseDownX; //prevent rounding error
                    y1 = _mouseDownY; //prevent rounding error
                }
                //if (firstTime)
                //{
                //    _snapX1 = x1;
                //    _snapY1 = y1;
                //    firstTime = false;
                //}
                //else
                //{
                //    x1 = _snapX1;
                //    y1 = _snapY1;
                //}
                //double current_distance = Math.Sqrt(x1 * x1 + y1 * y1);

                //
                //double newX = pos.X + e.XDiff;
                //double newY = pos.Y + e.YDiff;

                //float diff = 1;

                //if (e.XDiff > 0)
                //{
                //    diff = -1;
                //}

                //box.SetLocation((int)newX, (int)newY); 
                PixelFarm.CpuBlit.VertexProcessing.Affine aff = PixelFarm.CpuBlit.VertexProcessing.Affine.NewMatix(
                    //PixelFarm.CpuBlit.VertexProcessing.AffinePlan.Translate(-x1, -y1),
                    PixelFarm.CpuBlit.VertexProcessing.AffinePlan.Rotate(_rotateAngleDiff));

                //PixelFarm.CpuBlit.VertexProcessing.AffinePlan.Translate(x1, y1));

                aff.Transform(ref x1, ref y1);
                _actualX = x1;
                _actualY = y1;

                box.SetLocation((int)Math.Round(x1), (int)Math.Round(y1)); //essential to use double, prevent rounding err ***



                //var targetBox = box.TargetBox;

                ////test rotation around some axis
                ////find box center
                //if (targetBox != null)
                //{
                //    //find box center
                //    float centerX = (float)((targetBox.Width + targetBox.Left) / 2f);
                //    float centerY = (float)((targetBox.Height + targetBox.Top) / 2f);
                //    //
                //    Double angle = Math.Atan2(newY - centerY, newX - centerX);
                //    //rotate
                //    PixelFarm.CpuBlit.VertexProcessing.Affine aff = PixelFarm.CpuBlit.VertexProcessing.Affine.NewMatix(
                //       PixelFarm.CpuBlit.VertexProcessing.AffinePlan.Translate(-targetBox.Width / 2, -targetBox.Height / 2),
                //       PixelFarm.CpuBlit.VertexProcessing.AffinePlan.Rotate(angle),
                //       PixelFarm.CpuBlit.VertexProcessing.AffinePlan.Translate(targetBox.Width / 2, targetBox.Height / 2)
                //    );
                //    //transform 
                //    aff.Transform(ref x1, ref y1);
                //    box.SetLocation((int)x1, (int)y1);

                //}

                //if (targetBox != null)
                //{
                //    //move target box too
                //    targetBox.SetLocation(newX + 5, newY + 5);
                //}

                e.CancelBubbling = true;
            };
            _controls.Add(box);
        }
    }
}