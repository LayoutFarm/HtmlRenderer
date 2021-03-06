﻿//MIT, 2014-present, WinterDev

using System.Collections.Generic;
using System.IO;

using PixelFarm.Drawing;
using PixelFarm.CpuBlit;
using PixelFarm.CpuBlit.VertexProcessing;
using PaintLab.Svg;
using LayoutFarm.UI;
using LayoutFarm.CustomWidgets;

using PixelFarm.VectorMath;

namespace LayoutFarm
{

    static class MyMathHelper
    {
        public static Vector2 FindPerpendicularCutPoint(Vector2 p0, double p0p1Angle, Vector2 p2)
        {
            //a line from p0 to p1
            //p2 is any point
            //return p3 -> cutpoint on p0,p1   
            return FindPerpendicularCutPoint(
                p0,
                p0 + new Vector2(System.Math.Cos(p0p1Angle), System.Math.Sin(p0p1Angle)),
                p2);
        }
        public static Vector2 FindPerpendicularCutPoint(Vector2 p0, Vector2 p1, Vector2 p2)
        {
            //a line from p0 to p1
            //p2 is any point
            //return p3 -> cutpoint on p0,p1  
            double xdiff = p1.X - p0.X;
            double ydiff = p1.Y - p0.Y;
            if (xdiff == 0)
            {
                return new Vector2(p1.X, p2.Y);
            }
            if (ydiff == 0)
            {
                return new Vector2(p2.X, p1.Y);
            }

            double m1 = ydiff / xdiff;
            double b1 = FindB(p0, p1);

            double m2 = -1 / m1;
            double b2 = p2.Y - (m2) * p2.X;
            //find cut point
            double cutx = (b2 - b1) / (m1 - m2);
            double cuty = (m2 * cutx) + b2;
            return new Vector2(cutx, cuty);
        }
        static double FindB(Vector2 p0, Vector2 p1)
        {

            double m1 = (p1.Y - p0.Y) / (p1.X - p0.X);
            //y = mx + b ...(1)
            //b = y- mx

            //substitute with known value to gett b 
            //double b0 = p0.Y - (slope_m) * p0.X;
            //double b1 = p1.Y - (slope_m) * p1.X;
            //return b0;

            return p0.Y - (m1) * p0.X;
        }

    }


    public class RotationUI : UISprite
    {

        double _rotateAngle = PixelFarm.CpuBlit.AggMath.deg2rad(0);
        VertexStore _outlineVxs;
        List<PointControllerBox> _controlBoxes = new List<PointControllerBox>();


        SvgPathSpec _pathSpec;
        VgVisualDoc _vgVisualDoc;

        PointControllerBox _centerPoint;
        PointControllerBox _radiusPoint;
        UISprite _refOwner;

        public event System.EventHandler AngleUpdated;

        public RotationUI()
        : base(100, 100)
        {
            _pathSpec = new SvgPathSpec() { FillColor = Color.Red };
            _vgVisualDoc = new VgVisualDoc();
            _vgVisualElem = new VgVisualElement(WellknownSvgElementName.Path, _pathSpec, _vgVisualDoc);
        }
        public double GetAngleInRad()
        {
            return System.Math.Atan2(_radiusPoint.Top - _centerPoint.Top, _radiusPoint.Left - _centerPoint.Left);

        }
        public void SetReferenceOwner(UISprite ui)
        {
            _refOwner = ui;
        }
        public void SetCenter(double x, double y)
        {
            _centerPoint.SetLocation((int)x, (int)y);
        }
        public void SetRadius(double x, double y)
        {
            _radiusPoint.SetLocation((int)x, (int)y);
        }
        public void AddControlPoints(
            PointControllerBox centerPoint,
            PointControllerBox radiusPoint)
        {
            _controlBoxes.Add(centerPoint);
            _controlBoxes.Add(radiusPoint);

            _centerPoint = centerPoint;
            _radiusPoint = radiusPoint;
            centerPoint.MouseDrag += (s, e) =>
            {

            };
            _radiusPoint.MouseDrag += (s, e) =>
            {
                radiusPoint.SetLocation(
                    radiusPoint.Left + e.XDiff,
                    radiusPoint.Top + e.YDiff);

                UpdateLineShapes();
                AngleUpdated?.Invoke(this, System.EventArgs.Empty);

                this.InvalidateGraphics();
            };
        }

        internal VertexStore OutlineVxs => _outlineVxs;

        public override void SetLocation(float left, float top)
        {
            float xdiff = left - this.Left;
            float ydiff = top - this.Top;
            base.SetLocation(left, top);
            _centerPoint.SetLocation((int)(_centerPoint.Left + xdiff), (int)(_centerPoint.Top + ydiff));
            _radiusPoint.SetLocation((int)(_radiusPoint.Left + xdiff), (int)(_radiusPoint.Top + ydiff));

        }
        protected override void OnUpdateVgVisualElement()
        {
            //TODO: review here, can we use shared vgvisual doc?
            //...
            //_simpleBox = new Box(100, 100);

            using (Tools.BorrowVxs(out var v1, out var v2))
            using (Tools.BorrowStroke(out var stroke))
            {
                stroke.Width = 2;

                v1.AddMoveTo(_centerPoint.Left, _centerPoint.Top);
                v1.AddLineTo(_radiusPoint.Left, _radiusPoint.Top);

                v1.AddNoMore();
                //
                _outlineVxs = v1.CreateTrim();//outline vxs***
                stroke.MakeVxs(v1, v2); //create stroke path around v1 
                _vgVisualElem.VxsPath = v2.CreateTrim();

            }
            //*** after set dest 
            CreateNewControlPoints(_controlBoxes, this.OutlineVxs);

            //SetCornerLocation(0, _controlBoxes[0].TargetX, _controlBoxes[0].TargetY);
            //SetCornerLocation(1, _controlBoxes[1].TargetX, _controlBoxes[1].TargetY);
            //SetCornerLocation(2, _controlBoxes[2].TargetX, _controlBoxes[2].TargetY);
            //SetCornerLocation(3, _controlBoxes[3].TargetX, _controlBoxes[3].TargetY);
            base.OnUpdateVgVisualElement();
        }


        void UpdateLineShapes()
        {
            using (Tools.BorrowVxs(out var v1, out var v2))
            using (Tools.BorrowStroke(out var stroke))
            {
                stroke.Width = 2;
                //vxs is relative to  left and top of this UI

                v1.AddMoveTo(_centerPoint.Left - this.Left, _centerPoint.Top - this.Top);
                v1.AddLineTo(_radiusPoint.Left - this.Left, _radiusPoint.Top - this.Top);

                v1.AddNoMore();
                //
                _outlineVxs = v1.CreateTrim();//outline vxs***
                stroke.MakeVxs(v1, v2); //create stroke path around v1 
                _vgVisualElem.VxsPath = v2.CreateTrim();
            }
        }

        void SetupCornerBoxController(PointControllerBox box)
        {
            Color c = KnownColors.FromKnownColor(KnownColor.Orange);
            box.BackColor = new Color(100, c.R, c.G, c.B);
            //controllerBox1.dbugTag = 3;
            box.Visible = true;
            //for controller box    
            box.MouseDown += (s, e) =>
            {
                //UpdateRotationCenter(box.Index);
                //_affineFinalTx = UpdateAffineMatrix();

            };

            box.MouseDrag += (s, e) =>
            {

                //_eventSrcControlBox = box;
                double newX = box.TargetX + e.XDiff;// pos.X + e.XDiff;
                double newY = box.TargetY + e.YDiff; //pos.Y + e.YDiff;
                //_xdiff = e.XDiff;
                //_ydiff = e.YDiff;
                box.SetLocationRelativeToTarget(newX, newY);



                this.InvalidateGraphics();
                foreach (UIElement ui in _controlBoxes)
                {
                    ui.InvalidateGraphics();
                }

                UpdateLineShapes();
                this.InvalidateGraphics();
                foreach (UIElement ui in _controlBoxes)
                {
                    ui.InvalidateGraphics();
                }
                //need to update vg shape ***



                //switch (_currentTransformStyle)
                //{
                //    default: throw new System.NotSupportedException();

                //    case QuadTransformStyle.Bilinear:
                //        {
                //            box.SetLocationRelativeToTarget(newX, newY);
                //            //var targetBox = cornerBox.TargetBox;
                //            //if (targetBox != null)
                //            //{
                //            //    //move target box too
                //            //    targetBox.SetLocation(newX + 5, newY + 5);
                //            //}
                //            e.CancelBubbling = true;
                //        }
                //        break;
                //    case QuadTransformStyle.Affine_ScaleAndTranslate:
                //        {
                //            _affineFinalTx = UpdateAffineMatrix();
                //            box.SetLocationRelativeToTarget(newX, newY);
                //        }
                //        break;
                //    case QuadTransformStyle.Affine_Rotation:
                //        {

                //            switch (box.Index)
                //            {
                //                case 0:
                //                    {
                //                        _rotateAngle = System.Math.Atan2(_controlBoxes[1].TargetY - newY, _controlBoxes[1].TargetX - newX);
                //                    }
                //                    break;
                //                case 1:
                //                    {
                //                        //find new angle  
                //                        _rotateAngle = System.Math.Atan2(newY - _controlBoxes[0].TargetY, newX - _controlBoxes[0].TargetX);
                //                    }
                //                    break;
                //                case 2:
                //                    {
                //                        //find new angle   
                //                        _rotateAngle = System.Math.Atan2(newY - _controlBoxes[3].TargetY, newX - _controlBoxes[3].TargetX);
                //                    }
                //                    break;
                //                case 3:
                //                    {
                //                        _rotateAngle = System.Math.Atan2(_controlBoxes[2].TargetY - newY, _controlBoxes[2].TargetX - newX);

                //                    }
                //                    break;
                //            }

                //            //update affine matrix
                //            _affineFinalTx = UpdateAffineMatrix();
                //            box.SetLocationRelativeToTarget(newX, newY);
                //        }
                //        break;
                //}

                //--------------------------------- 
                //_simpleBox.InvalidateOuterGraphics();
                foreach (PointControllerBox ctrl in _controlBoxes)
                {
                    //update before move 
                    ctrl.InvalidateGraphics();
                }
                //then update the vxs shape 
                //_vxs.ReplaceVertex(box.Index, newX, newY);
                //this.UpdateRelatedControls();
                //_uiListener?.HandleElementUpdate();
            };
        }


        void CreateNewControlPoints(List<PointControllerBox> controls, VertexStore vxs)
        {
            float offsetX = 0;
            float offsetY = 0;
            int j = vxs.Count;
            for (int i = 0; i < j; ++i)
            {
                switch (vxs.GetVertex(i, out double x, out double y))
                {
                    case VertexCmd.NoMore:
                        return;//**
                    case VertexCmd.MoveTo:
                        {
                            var ctrlPoint = new PointControllerBox(8, 8);
                            ctrlPoint.Index = i;

                            x += offsetX;
                            y += offsetY;

                            ctrlPoint.SrcX = x;
                            ctrlPoint.SrcY = y;
                            ctrlPoint.SetLocationRelativeToTarget(x, y);

                            SetupCornerBoxController(ctrlPoint);
                            controls.Add(ctrlPoint);
                        }
                        break;
                    case VertexCmd.LineTo:
                        {
                            var ctrlPoint = new PointControllerBox(8, 8);
                            ctrlPoint.Index = i;
                            x += offsetX;
                            y += offsetY;

                            ctrlPoint.SrcX = x;
                            ctrlPoint.SrcY = y;

                            ctrlPoint.SetLocationRelativeToTarget(x, y);

                            SetupCornerBoxController(ctrlPoint);
                            controls.Add(ctrlPoint);
                        }
                        break;
                    case VertexCmd.Close:
                        break;
                }
            }
        }
    }

    public class QuadControllerUI : UISprite
    {


        public enum QuadTransformStyle
        {
            None,
            Bilinear,
            Perspective,
            Affine_Rotation,
            Affine_ScaleAndTranslate,
        }


        double _src_left, _src_top, _src_w, _src_h;
        bool _setDestRect;

        //-------------------
        double _src_x0, _src_y0,
            _src_x1, _src_y1,
            _src_x2, _src_y2,
            _src_x3, _src_y3;
        //-------------------
        //final coords
        double _x0, _y0,
               _x1, _y1,
               _x2, _y2,
               _x3, _y3;

        public event System.EventHandler ElemUpdate;
        public event System.EventHandler UpdateTransformTarget;


        protected List<PointControllerBox> _controlBoxes;


        double _srcCenterX = 0;
        double _srcCenterY = 0;
        double _rotateCenterX;
        double _rotateCenterY;

        Bilinear _bilinearTx;
        ReusableAffineMatrix _affineFinalTx;


        QuadTransformStyle _currentTransformStyle = QuadTransformStyle.Affine_ScaleAndTranslate;
        PolygonControllerUI _polygonController;


        double _rotateAngle = PixelFarm.CpuBlit.AggMath.deg2rad(0);


        double _xdiff;
        double _ydiff;
        double _translate_X1, _translate_X2;
        double _translateY_1, _translate_Y2;

        double _scaleW = 1;
        double _scaleH = 1;
        PointControllerBox _eventSrcControlBox;
        public QuadControllerUI()
          : base(100, 100)
        {

        }
        public QuadTransformStyle TransformStyle
        {
            get => _currentTransformStyle;
            set
            {
                //change

                //if (_currentTransformStyle != value)
                //{
                //    switch (value)
                //    {
                //        default: throw new System.NotSupportedException();
                //        case QuadTransformStyle.Bilinear:
                //            {
                //                //how to deal with this

                //            }
                //            break;
                //        case QuadTransformStyle.Affine_ScaleAndTranslate:
                //            {

                //            }
                //            break;
                //        case QuadTransformStyle.Affine_Rotation:
                //            {
                //                switch (_currentTransformStyle)
                //                {
                //                    case QuadTransformStyle.Affine_Rotation:
                //                    case QuadTransformStyle.Affine_ScaleAndTranslate:
                //                        {
                //                            //do nothing
                //                        }
                //                        break;
                //                    case QuadTransformStyle.Bilinear:
                //                        {

                //                        }
                //                        break;
                //                    case QuadTransformStyle.Perspective:
                //                        {

                //                        }
                //                        break;
                //                }
                //            }
                //            break;
                //        case QuadTransformStyle.Perspective:
                //            {

                //            }
                //            break;
                //    }
                //}


                _currentTransformStyle = value;
            }
        }
        public ICoordTransformer GetCoordTransformer()
        {
            switch (_currentTransformStyle)
            {
                case QuadTransformStyle.Affine_ScaleAndTranslate:
                case QuadTransformStyle.Affine_Rotation:
                    return _affineFinalTx;
                case QuadTransformStyle.Bilinear:
                    return _bilinearTx;
                default: throw new System.NotSupportedException();
            }
        }

        public void SetPolygonController(PolygonControllerUI polygonController)
        {
            _polygonController = polygonController;
            polygonController.SetTargetListener(this);

            //
            this.GetInnerCoords(
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

            this.ElemUpdate += UpdateTransformedShape;
            UpdateTransformedShape(this, System.EventArgs.Empty);
        }


        void UpdateRotationCenter(int left, int mid, int right)
        {
            return;
            Vector2 mid1 = new Vector2((_controlBoxes[left].TargetX + _controlBoxes[mid].TargetX) / 2,
                                        (_controlBoxes[left].TargetY + _controlBoxes[mid].TargetY) / 2);

            Vector2 mid2 = mid1 + new Vector2(System.Math.Cos(_rotateAngle), System.Math.Sin(_rotateAngle));

            Vector2 mid3 = new Vector2((_controlBoxes[right].TargetX + _controlBoxes[mid].TargetX) / 2,
                                       (_controlBoxes[right].TargetY + _controlBoxes[mid].TargetY) / 2);

            Vector2 rcenter = MyMathHelper.FindPerpendicularCutPoint(mid1, mid2, mid3);

            _rotateCenterX = rcenter.X;
            _rotateCenterY = rcenter.Y;
        }

        void UpdateRotationCenter(int cornerIndex)
        {
            switch (cornerIndex)
            {
                case 0:
                    UpdateRotationCenter(1, 2, 3);
                    break;
                case 1:
                    UpdateRotationCenter(0, 3, 2);
                    break;
                case 2:
                    UpdateRotationCenter(3, 0, 1);
                    break;
                case 3:
                    UpdateRotationCenter(2, 1, 0);
                    break;
            }
        }
        public void ClearCurrentEventSourceBox()
        {
            _eventSrcControlBox = null;
        }
        public void UpdateRotationAngle(double newAngle)
        {

            _rotateAngle = newAngle;
            UpdateAffineMatrix();
            //--------------------------------- 
            //_simpleBox.InvalidateOuterGraphics(); 
            foreach (PointControllerBox ctrl in _controlBoxes)
            {
                //update before move 
                ctrl.InvalidateGraphics();
            }
            //then update the vxs shape 
            //_vxs.ReplaceVertex(box.Index, newX, newY);
            this.UpdateRelatedControls();
        }

        void SetupCornerBoxController(PointControllerBox box)
        {
            Color c = KnownColors.FromKnownColor(KnownColor.Orange);
            box.BackColor = new Color(100, c.R, c.G, c.B);
            //controllerBox1.dbugTag = 3;
            box.Visible = true;
            //for controller box    
            box.MouseDown += (s, e) =>
            {
                UpdateRotationCenter(box.Index);
                UpdateAffineMatrix();

            };

            box.MouseDrag += (s, e) =>
            {

                _eventSrcControlBox = box;
                double newX = box.TargetX + e.XDiff;// pos.X + e.XDiff;
                double newY = box.TargetY + e.YDiff; //pos.Y + e.YDiff;
                _xdiff = e.XDiff;
                _ydiff = e.YDiff;
                switch (_currentTransformStyle)
                {
                    default: throw new System.NotSupportedException();

                    case QuadTransformStyle.Bilinear:
                        {
                            box.SetLocationRelativeToTarget(newX, newY);
                            //var targetBox = cornerBox.TargetBox;
                            //if (targetBox != null)
                            //{
                            //    //move target box too
                            //    targetBox.SetLocation(newX + 5, newY + 5);
                            //}
                            e.CancelBubbling = true;
                        }
                        break;
                    case QuadTransformStyle.Affine_ScaleAndTranslate:
                        {
                            UpdateAffineMatrix();
                            box.SetLocationRelativeToTarget(newX, newY);
                        }
                        break;
                    case QuadTransformStyle.Affine_Rotation:
                        {

                            switch (box.Index)
                            {
                                case 0:
                                    {
                                        _rotateAngle = System.Math.Atan2(_controlBoxes[1].TargetY - newY, _controlBoxes[1].TargetX - newX);
                                    }
                                    break;
                                case 1:
                                    {
                                        //find new angle  
                                        _rotateAngle = System.Math.Atan2(newY - _controlBoxes[0].TargetY, newX - _controlBoxes[0].TargetX);
                                    }
                                    break;
                                case 2:
                                    {
                                        //find new angle   
                                        _rotateAngle = System.Math.Atan2(newY - _controlBoxes[3].TargetY, newX - _controlBoxes[3].TargetX);
                                    }
                                    break;
                                case 3:
                                    {
                                        _rotateAngle = System.Math.Atan2(_controlBoxes[2].TargetY - newY, _controlBoxes[2].TargetX - newX);

                                    }
                                    break;
                            }

                            //update affine matrix
                            UpdateAffineMatrix();
                            box.SetLocationRelativeToTarget(newX, newY);
                        }
                        break;
                }

                //--------------------------------- 
                //_simpleBox.InvalidateOuterGraphics(); 
                foreach (PointControllerBox ctrl in _controlBoxes)
                {
                    //update before move 
                    ctrl.InvalidateGraphics();
                }
                //then update the vxs shape 
                //_vxs.ReplaceVertex(box.Index, newX, newY);
                this.UpdateRelatedControls();
                //_uiListener?.HandleElementUpdate();
            };
        }

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

            this.GetInnerCoords(
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

            UpdateTransformTarget?.Invoke(this, ErrorEventArgs.Empty);

        }

        void CreateNewControlPoints(List<PointControllerBox> controls, VertexStore vxs)
        {
            float offsetX = 0;
            float offsetY = 0;
            int j = vxs.Count;
            for (int i = 0; i < j; ++i)
            {
                switch (vxs.GetVertex(i, out double x, out double y))
                {
                    case VertexCmd.NoMore:
                        return;
                    case VertexCmd.MoveTo:
                        {
                            var ctrlPoint = new PointControllerBox(8, 8);
                            ctrlPoint.Index = i;

                            x += offsetX;
                            y += offsetY;

                            ctrlPoint.SrcX = x;
                            ctrlPoint.SrcY = y;
                            ctrlPoint.SetLocationRelativeToTarget(x, y);

                            SetupCornerBoxController(ctrlPoint);
                            controls.Add(ctrlPoint);
                        }
                        break;
                    case VertexCmd.LineTo:
                        {
                            var ctrlPoint = new PointControllerBox(8, 8);
                            ctrlPoint.Index = i;
                            x += offsetX;
                            y += offsetY;

                            ctrlPoint.SrcX = x;
                            ctrlPoint.SrcY = y;

                            ctrlPoint.SetLocationRelativeToTarget(x, y);

                            SetupCornerBoxController(ctrlPoint);
                            controls.Add(ctrlPoint);
                        }
                        break;
                    case VertexCmd.Close:
                        break;
                }
            }
        }

        static void AssignXY(PointControllerBox box, out double x, out double y)
        {
            x = box.TargetX;
            y = box.TargetY;
        }
        protected override void OnMouseDown(UIMouseDownEventArgs e)
        {
            base.OnMouseDown(e);
            this.Focus(); //if we not focus => we can't get keyboard input**
        }


        //protected override void OnKeyDown(UIKeyEventArgs e)
        //{
        //    //TODO: remove this
        //    switch (e.KeyCode)
        //    {
        //        case UIKeys.NumPad0:
        //            _currentTransformStyle = QuadTransformStyle.Bilinear;
        //            break;
        //    }
        //    base.OnKeyDown(e);
        //}


        void SetCornerLocation(int index, double x, double y)
        {

            switch (index)
            {
                default: throw new System.NotSupportedException();
                case 0:
                    _controlBoxes[0].SetLocationRelativeToTarget(x, y);
                    break;
                case 1:
                    _controlBoxes[1].SetLocationRelativeToTarget(x, y);
                    break;
                case 2:
                    _controlBoxes[2].SetLocationRelativeToTarget(x, y);
                    break;
                case 3:
                    _controlBoxes[3].SetLocationRelativeToTarget(x, y);
                    break;
            }
        }

        void UpdateAffineMatrix()
        {
            //preserve***
            //return Affine.NewMatix2(
            //        AffinePlan.Translate(-_srcCenterX, -_srcCenterY),//move to src_center
            //        AffinePlan.Scale(_scaleW, _scaleH),  //scale
            //        AffinePlan.Translate(_translateX, _translateY), //translate to constrain position
            //        AffinePlan.Translate(-(_rotateCenterX - _srcCenterX), -(_rotateCenterY - _srcCenterY)), //move to rotate center
            //        AffinePlan.Rotate(_rotateAngle), //rotate
            //        AffinePlan.Translate((_rotateCenterX - _srcCenterX), (_rotateCenterY - _srcCenterY)), //move back to constrain position
            //        AffinePlan.Translate(_srcCenterX, _srcCenterY)
            //       );   

            AffineMat mat = AffineMat.Iden();
            mat.Translate(-_srcCenterX, -_srcCenterY);
            mat.Scale(_scaleW, _scaleH);
            mat.Translate(_translate_X1 + _translate_X2 - (_rotateCenterX - _srcCenterX), _translateY_1 + _translate_Y2 + -(_rotateCenterY - _srcCenterY));
            mat.Rotate(_rotateAngle);
            mat.Translate((_rotateCenterX - _srcCenterX) + _srcCenterX, (_rotateCenterY - _srcCenterY) + _srcCenterY);

            _affineFinalTx.SetElems(mat);
        }
        public void UpdateRelatedControls()
        {

            //***SUB_METHOD***
            void UpdateAffineSide_Right(int left_index)
            {
                //find cut point from current active point to related edge 
                PointControllerBox box = _controlBoxes[left_index];
                //
                Vector2 edgeCutAt = MyMathHelper.FindPerpendicularCutPoint(
                                     new Vector2(box.TargetX, box.TargetY),
                                    _rotateAngle,
                                     new Vector2(_eventSrcControlBox.TargetX, _eventSrcControlBox.TargetY));

                double dx = edgeCutAt.X - box.TargetX;
                double dy = edgeCutAt.Y - box.TargetY;
                double newDistance_W = System.Math.Sqrt(dx * dx + dy * dy);
                //----------------
                _scaleW = newDistance_W / _src_w;
                _translate_X1 = _translate_X2 + ((newDistance_W - _src_w) / 2);
            }
            //***SUB_METHOD***
            void UpdateAffineSide_Top(int left_index)
            {
                //find cut point from current active point to related edge 
                PointControllerBox box = _controlBoxes[left_index];
                //
                Vector2 edgeCutAt = MyMathHelper.FindPerpendicularCutPoint(
                     new Vector2(box.TargetX, box.TargetY),
                     System.Math.PI / 2 + _rotateAngle,
                     new Vector2(_eventSrcControlBox.TargetX, _eventSrcControlBox.TargetY));

                double dx = edgeCutAt.X - box.TargetX;
                double dy = edgeCutAt.Y - box.TargetY;
                double newDistance_H = System.Math.Sqrt(dx * dx + dy * dy);

                _scaleH = newDistance_H / _src_h;
                _translate_Y2 = _translateY_1 - ((newDistance_H - _src_h) / 2); //*** notice: _translate_Y2 and minus sign
            }
            //***SUB_METHOD***
            void UpdateAffineSide_Bottom(int top_index)
            {

                //find cut point from current active point to related edge 

                PointControllerBox box = _controlBoxes[top_index];
                //
                Vector2 edgeCutAt = MyMathHelper.FindPerpendicularCutPoint(
                                          new Vector2(box.TargetX, box.TargetY),
                                          System.Math.PI / 2 + _rotateAngle,
                                          new Vector2(_eventSrcControlBox.TargetX, _eventSrcControlBox.TargetY));

                double dx = edgeCutAt.X - box.TargetX;
                double dy = edgeCutAt.Y - box.TargetY;
                double newDistance_H = System.Math.Sqrt(dx * dx + dy * dy);
                _scaleH = newDistance_H / _src_h;
                _translateY_1 = (newDistance_H - _src_h) / 2 + _translate_Y2;//*** 
            }
            //***SUB_METHOD***
            void UpdateAffineSide_Left(int right_index)
            {
                //find cut point from current active point to related edge 
                PointControllerBox box = _controlBoxes[right_index];
                //
                Vector2 edgeCutAt = MyMathHelper.FindPerpendicularCutPoint(
                                               new Vector2(box.TargetX, box.TargetY),
                                               _rotateAngle,
                                               new Vector2(_eventSrcControlBox.TargetX, _eventSrcControlBox.TargetY));
                double dx = edgeCutAt.X - box.TargetX;
                double dy = edgeCutAt.Y - box.TargetY;
                double newDistance_W = System.Math.Sqrt(dx * dx + dy * dy);
                _scaleW = newDistance_W / _src_w;
                _translate_X2 = -(newDistance_W - _src_w) / 2 + _translate_X1; //*** notice: _translate_X2 and minus sign
            }
            //===============


            List<PointControllerBox> controlBoxes = _controlBoxes;
            switch (_currentTransformStyle)
            {
                case QuadTransformStyle.Bilinear:
                    {
                        SetDestQuad(
                           controlBoxes[0].Left, controlBoxes[0].Top,
                           controlBoxes[1].Left, controlBoxes[1].Top,
                           controlBoxes[2].Left, controlBoxes[2].Top,
                           controlBoxes[3].Left, controlBoxes[3].Top
                         );
                        _polygonController.UpdateControlPoints(_outlineVxs);

                        ElemUpdate?.Invoke(this, System.EventArgs.Empty);
                        UpdateTransformedShape(this, System.EventArgs.Empty);
                    }
                    break;
                case QuadTransformStyle.Affine_ScaleAndTranslate:
                    {


                        if (_rotateAngle != 0)
                        {
                            int rotateCenter_index = 0;
                            int cornerIndex = _eventSrcControlBox.Index;
                            //preserve pararell
                            switch (cornerIndex)
                            {

                                default: throw new System.NotSupportedException();
                                case 2:
                                    //right-top  
                                    UpdateAffineSide_Right(0);
                                    UpdateAffineSide_Top(0);
                                    break;
                                case 1:
                                    //right-bottom  
                                    UpdateAffineSide_Right(0);
                                    UpdateAffineSide_Bottom(3);
                                    break;
                                //--------------------------------------------------------------------------------------------------------------
                                case 0:
                                    //left, bottom
                                    UpdateAffineSide_Left(1);
                                    UpdateAffineSide_Bottom(3);
                                    break;
                                case 3:
                                    //left-top 
                                    UpdateAffineSide_Left(1);
                                    UpdateAffineSide_Top(0);
                                    break;
                            }


                            UpdateAffineMatrix();
                            double x0 = _src_x0, y0 = _src_y0,
                                   x1 = _src_x1, y1 = _src_y1,
                                   x2 = _src_x2, y2 = _src_y2,
                                   x3 = _src_x3, y3 = _src_y3;

                            _affineFinalTx.Transform(ref x0, ref y0);
                            _affineFinalTx.Transform(ref x1, ref y1);
                            _affineFinalTx.Transform(ref x2, ref y2);
                            _affineFinalTx.Transform(ref x3, ref y3);

                            _x0 = x0; _y0 = y0;
                            _x1 = x1; _y1 = y1;
                            _x2 = x2; _y2 = y2;
                            _x3 = x3; _y3 = y3;

                            controlBoxes[0].SetLocationRelativeToTarget(x0, y0);
                            controlBoxes[1].SetLocationRelativeToTarget(x1, y1);
                            controlBoxes[2].SetLocationRelativeToTarget(x2, y2);
                            controlBoxes[3].SetLocationRelativeToTarget(x3, y3);

                            UpdateRotationCenter(rotateCenter_index);

                        }
                        else
                        {
                            double a_px = _eventSrcControlBox.TargetX; //active point X
                            double a_py = _eventSrcControlBox.TargetY; //active point Y
                            //rotate angle=0
                            int cornerIndex = _eventSrcControlBox.Index;
                            switch (cornerIndex)
                            {
                                //bottom-up index...
                                default: throw new System.NotSupportedException();
                                case 0:
                                    {

                                        //left-bottom
                                        //left  
                                        _scaleW = (controlBoxes[1].TargetX - a_px) / _src_w;
                                        _scaleH = (a_py - controlBoxes[3].TargetY) / _src_h;
                                        _translate_X1 = ((a_px + controlBoxes[1].TargetX) - _src_w) / 2;
                                        _translateY_1 = ((a_py + controlBoxes[3].TargetY) - _src_h) / 2;

                                        controlBoxes[3].SetLocationRelativeToTarget(a_px, controlBoxes[3].TargetY);
                                        controlBoxes[1].SetLocationRelativeToTarget(controlBoxes[1].TargetX, a_py);
                                    }
                                    break;
                                case 1:
                                    {
                                        //right-bottom

                                        double px = controlBoxes[0].TargetX;
                                        double py = controlBoxes[2].TargetY;

                                        _scaleW = (a_px - px) / _src_w; //new scale 
                                        _scaleH = (a_py - py) / _src_h;
                                        _translate_X1 = ((a_px + px) - _src_w) / 2;
                                        _translateY_1 = ((a_py + py) - _src_h) / 2;


                                        SetCornerLocation(2, a_px, py);
                                        SetCornerLocation(0, px, a_py);
                                    }
                                    break;
                                case 2:
                                    {
                                        //right-top
                                        double px = controlBoxes[3].TargetX;
                                        double py = controlBoxes[1].TargetY;

                                        _scaleW = (a_px - px) / _src_w; //new scale                                         
                                        _scaleH = (py - a_py) / _src_h;

                                        _translate_X1 = ((a_px + px) - _src_w) / 2;
                                        _translateY_1 = ((a_py + py) - _src_h) / 2;

                                        controlBoxes[1].SetLocationRelativeToTarget(a_px, py);
                                        controlBoxes[3].SetLocationRelativeToTarget(px, a_py);
                                    }
                                    break;
                                case 3:
                                    {
                                        //left-top
                                        _scaleW = (controlBoxes[2].TargetX - a_px) / _src_w;
                                        _scaleH = (controlBoxes[0].TargetY - a_py) / _src_h;
                                        _translate_X1 = ((a_px + controlBoxes[2].TargetX) - _src_w) / 2;
                                        _translateY_1 = ((a_py + controlBoxes[0].TargetY) - _src_h) / 2;

                                        controlBoxes[0].SetLocationRelativeToTarget(a_px, controlBoxes[0].TargetY);
                                        controlBoxes[2].SetLocationRelativeToTarget(controlBoxes[2].TargetX, a_py);
                                    }
                                    break;
                            }
                        }

                        SetDestQuad(
                            controlBoxes[0].TargetX, controlBoxes[0].TargetY,
                            controlBoxes[1].TargetX, controlBoxes[1].TargetY,
                            controlBoxes[2].TargetX, controlBoxes[2].TargetY,
                            controlBoxes[3].TargetX, controlBoxes[3].TargetY);
#if DEBUG
                        controlBoxes[0].BackColor = Color.Red;
                        controlBoxes[1].BackColor = Color.Green;
                        controlBoxes[2].BackColor = Color.Yellow;
                        controlBoxes[3].BackColor = Color.Magenta;

                        dbugCheckPerpendicular(3, 0, 1);
                        dbugCheckPerpendicular(0, 1, 2);
                        dbugCheckPerpendicular(1, 2, 3);
                        dbugCheckPerpendicular(2, 3, 0);
#endif

                        UpdateTransformedShape(this, System.EventArgs.Empty);
                    }
                    break;
                case QuadTransformStyle.Affine_Rotation:
                    {

                        int j = controlBoxes.Count;
                        for (int i = 0; i < j; ++i)
                        {
                            //TODO: add actual position to control box
                            PointControllerBox ctrlBox = controlBoxes[i];
                            if (ctrlBox == _eventSrcControlBox) { continue; }

                            double x1 = ctrlBox.SrcX;
                            double y1 = ctrlBox.SrcY;

                            _affineFinalTx.Transform(ref x1, ref y1);
                            //_actualX = x1;
                            //_actualY = y1;
                            //TODO: box.InvalidateOuterGraphics();//...

                            //ctrlBox.TargetX = x1;
                            //ctrlBox.TargetY = y1;

                            ctrlBox.SetLocationRelativeToTarget(x1, y1); //essential to use double, prevent rounding err *** 
                        }


                        //ElemUpdate?.Invoke(this, System.EventArgs.Empty);

                        SetDestQuad(
                            controlBoxes[0].TargetX, controlBoxes[0].TargetY,
                            controlBoxes[1].TargetX, controlBoxes[1].TargetY,
                            controlBoxes[2].TargetX, controlBoxes[2].TargetY,
                            controlBoxes[3].TargetX, controlBoxes[3].TargetY);


#if DEBUG
                        //check vector is perpendicular or not
                        dbugCheckPerpendicular(3, 0, 1);
                        dbugCheckPerpendicular(0, 1, 2);
                        dbugCheckPerpendicular(1, 2, 3);
                        dbugCheckPerpendicular(2, 3, 0);
#endif

                        UpdateTransformedShape(this, System.EventArgs.Empty);
                    }
                    break;
            }
        }

#if DEBUG
        bool dbugCheckPerpendicular(int left, int middle, int right)
        {
            double result = PixelFarm.VectorMath.Vector2.Dot(
                              new PixelFarm.VectorMath.Vector2(_controlBoxes[left].TargetX - _controlBoxes[middle].TargetX, _controlBoxes[left].TargetY - _controlBoxes[middle].TargetY),
                              new PixelFarm.VectorMath.Vector2(_controlBoxes[right].TargetX - _controlBoxes[middle].TargetX, _controlBoxes[right].TargetY - _controlBoxes[middle].TargetY));
            return result == 0;
        }
#endif

        protected override void OnElementChanged()
        {
            //base.OnElementChanged();
            //update dest quad
            UpdateRelatedControls();
        }


        public event System.EventHandler<UIMouseMoveEventArgs> Drag;
        public event System.EventHandler<UIMouseUpEventArgs> MouseUp;

        public event System.EventHandler<UIMouseEventArgs> MouseDblClick;

        protected override void OnDoubleClick(UIMouseEventArgs e)
        {
            base.OnDoubleClick(e);
            MouseDblClick?.Invoke(this, e);
        }
        protected override void OnMouseUp(UIMouseUpEventArgs e)
        {
            base.OnMouseUp(e);
            MouseUp?.Invoke(this, e);
        }
        protected override void OnMouseMove(UIMouseMoveEventArgs e)
        {
            if (e.IsDragging)
            {
                //move quad control
                this.SetLocation(
                    this.Left + e.XDiff,
                    this.Top + e.YDiff);
                _polygonController.InvalidateGraphics();
                _polygonController.SetLocation(
                    _polygonController.Left + e.XDiff,
                    _polygonController.Top + e.YDiff);
                _polygonController.InvalidateGraphics();

                UpdateTransformedShape(this, System.EventArgs.Empty);
                //
                //
                Drag?.Invoke(this, e);
            }
            base.OnMouseMove(e);
        }
        public void SetSrcRect(double left, double top, double w, double h)
        {
            _src_left = left;
            _src_top = top;
            _src_w = w;
            _src_h = h;
            //
            _srcCenterX = (_src_left + _src_w) / 2;
            _srcCenterY = (_src_top + _src_h) / 2;

            _src_x0 = left; _src_y0 = top + h; //left-bottom
            _src_x1 = left + w; _src_y1 = top + h; //right-bottom
            _src_x2 = left + w; _src_y2 = top; //right-top
            _src_x3 = left; _src_y3 = top; //left-top

            _rotateCenterX = _srcCenterX;
            _rotateCenterY = _srcCenterY;


            if (!_setDestRect)
            {
                LoadVg(CreateQuadVgFromSrcRect());
            }
        }


        VgVisualElement _vgVisElem;//*** 
        VertexStore _outlineVxs;
        internal VertexStore OutlineVxs => _outlineVxs;

        VgVisualElement CreateQuadVgFromSrcRect()
        {
            var spec = new SvgPathSpec() { FillColor = Color.Aqua };
            VgVisualDoc vgVisualDoc = new VgVisualDoc();
            VgVisualElement vgVisElem = new VgVisualElement(WellknownSvgElementName.Path, spec, vgVisualDoc);

            using (Tools.BorrowRect(out var rect))
            using (Tools.BorrowVxs(out var v1))
            {

                rect.SetRect(_src_left, _src_top + _src_h, _src_left + _src_w, _src_top);
                rect.MakeVxs(v1);

                //
                _outlineVxs = vgVisElem.VxsPath = v1.CreateTrim();
                _vgVisElem = vgVisElem;

            }
            return _vgVisElem;
        }

        VgVisualElement CreateQuadVgFromDestQuad()
        {
            //create 4 side of quad (green line)
            var spec = new SvgPathSpec() { FillColor = Color.Green };
            VgVisualDoc vgVisualDoc = new VgVisualDoc();
            VgVisualElement vgVisElem = new VgVisualElement(WellknownSvgElementName.Path, spec, vgVisualDoc);

            using (Tools.BorrowVxs(out var v1, out var v2))
            using (Tools.BorrowStroke(out var stroke))
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
                vgVisElem.VxsPath = v2.CreateTrim();
#if DEBUG
                vgVisElem.VxsPath.dbugNote = 1;
#endif

                _vgVisElem = vgVisElem;

            }
            return _vgVisElem;
        }
        public void SetDestQuad(
            double x0, double y0,
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
        //public override RenderElement GetPrimaryRenderElement()
        //{
        //    return base.GetPrimaryRenderElement();
        //}
        public void BuildControlBoxes()
        {
            //*** after set dest
            _controlBoxes = new List<PointControllerBox>();
            CreateNewControlPoints(_controlBoxes, this.OutlineVxs);
            //---------
            _polygonController.ClearControlPoints();
            _polygonController.LoadControlPoints(_controlBoxes);// _quadController.OutlineVxs);

            SetCornerLocation(0, _controlBoxes[0].TargetX, _controlBoxes[0].TargetY);
            SetCornerLocation(1, _controlBoxes[1].TargetX, _controlBoxes[1].TargetY);
            SetCornerLocation(2, _controlBoxes[2].TargetX, _controlBoxes[2].TargetY);
            SetCornerLocation(3, _controlBoxes[3].TargetX, _controlBoxes[3].TargetY);
        }

        public void GetInnerCoords(
                out double src_left, out double src_top,
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

    public class PointControllerBox : LayoutFarm.CustomWidgets.AbstractControlBox
    {
#if DEBUG
        static int s_total;
        public readonly int dbugControllerId = s_total++;
#endif
        public PointControllerBox(int w, int h)
            : base(w, h)
        {
        }


        public int Index { get; set; }
        public MoveDirection MoveDirection { get; set; }

        public double SrcX { get; set; }
        public double SrcY { get; set; }
        public double TargetX { get; set; }
        public double TargetY { get; set; }

        public void SetLocationRelativeToTarget(double targetBoxX, double targetBoxY)
        {
            this.TargetX = targetBoxX;
            this.TargetY = targetBoxY;
            this.SetLocation((int)System.Math.Round(targetBoxX), (int)System.Math.Round(targetBoxY));
        }
        public override void InvalidateGraphics()
        {
            CurrentPrimaryRenderElement?.InvalidateGraphics();
        }
        
#if DEBUG
        public override string ToString() => this.dbugControllerId + " ," + Left + "," + Top;
#endif

    }


    public class PolygonControllerUI : UIElement
    {
        float _offsetX;
        float _offsetY;
        IUIEventListener _uiListener;
        PixelFarm.Drawing.VertexStore _vxs;

        Box _simpleBox;//primary render elemenet
        bool _hasPrimRenderE;

        List<PointControllerBox> _controls = new List<PointControllerBox>();
        public PolygonControllerUI()
        {

            _simpleBox = new Box(10, 10);
            _simpleBox.TransparentForMouseEvents = true;

            //_simpleBox.BackColor = Color.Transparent;//*** 
#if DEBUG
            _simpleBox.BackColor = Color.Blue;//***  
#endif
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
        protected override bool HasReadyRenderElement => _hasPrimRenderE;
        public override RenderElement CurrentPrimaryRenderElement => _simpleBox.CurrentPrimaryRenderElement;
        //
        public override RenderElement GetPrimaryRenderElement()
        {
            _hasPrimRenderE = true;
            RenderElement renderE = _simpleBox.GetPrimaryRenderElement();
            renderE.TransparentForMouseEvents = this.TransparentForMouseEvents;
            return renderE;
        }

        public void Add(UIElement ui) => _simpleBox.Add(ui);

        public void SetLocation(int x, int y)
        {
            //TODO: review here again***
            //temp fix for invalidate area of overlap children
            _simpleBox.InvalidateGraphics();
            foreach (var ctrl in _controls)
            {
                ctrl.InvalidateGraphics();
            }
            _simpleBox.SetLocation(x, y);
        }

        public int Left => _simpleBox.Left;
        public int Top => _simpleBox.Top;

        public void SetTargetListener(IUIEventListener uiListener)
        {
            _uiListener = uiListener;
        }
        ////--------------------
        //public void BringToTopMost()
        //{
        //    AbstractBox parentBox = this.ParentUI as AbstractBox;
        //    if (parentBox != null)
        //    {
        //        RemoveSelf();
        //        parentBox.Add(this);
        //    }
        //    else
        //    {
        //        //may be at top level
        //        var parentBox2 = this.CurrentPrimaryRenderElement.ParentRenderElement as LayoutFarm.RenderElement;
        //        if (parentBox2 != null)
        //        {
        //            parentBox2.RemoveChild(this.CurrentPrimaryRenderElement);
        //        }
        //        parentBox2.AddChild(CurrentPrimaryRenderElement);
        //        InvalidateOuterGraphics();
        //    }
        //}


        public override bool Visible
        {
            get => base.Visible;
            set
            {
                base.Visible = value;
                int j = _controls.Count;
                for (int i = 0; i < j; ++i)
                {
                    _controls[i].InvalidateGraphics();
                }
            }
        }

        public void UpdateControlPoints(PixelFarm.Drawing.VertexStore vxs)
        {
            UpdateControlPoints(vxs, _offsetX, _offsetY, null);
        }
        public void ClearControlPoints()
        {
            int m = _controls.Count;
            for (int n = 0; n < m; ++n)
            {
                _controls[n].RemoveSelf();
            }
            _controls.Clear(); //***
            _simpleBox.ClearChildren();
        }
        public void LoadControlPoints(List<PointControllerBox> controlPoints)
        {
            _controls = controlPoints;
            int m = controlPoints.Count;
            for (int i = 0; i < m; ++i)
            {
                _simpleBox.Add(controlPoints[i]);
            }
        }

        bool _clearAllPointsWhenUpdate = false;
        public void UpdateControlPoints(PixelFarm.Drawing.VertexStore vxs, float offsetX, float offsetY, ICoordTransformer iCoordTx)
        {
            //1. we remove existing point from root

            _vxs = vxs;
            _offsetX = offsetX;
            _offsetY = offsetY;

            int m = _controls.Count;

            if (m > 0)
            {
                if (!_clearAllPointsWhenUpdate)
                {
                    int j2 = vxs.Count;

                    for (int i = 0; i < j2; ++i)
                    {

                        var cmd = vxs.GetVertex(i, out double x, out double y);
                        if (iCoordTx != null)
                        {
                            iCoordTx.Transform(ref x, ref y);
                        }

                        switch (cmd)
                        {
                            case VertexCmd.NoMore:
                                return;
                            case VertexCmd.MoveTo:
                                {
                                    _controls[i].SetLocation((int)(x + offsetX), (int)(y + offsetY));
                                }
                                break;
                            case VertexCmd.LineTo:
                                {
                                    _controls[i].SetLocation((int)(x + offsetX), (int)(y + offsetY));
                                }
                                break;
                            case VertexCmd.Close:
                                break;
                        }
                    }
                    //****
                    return;
                }

            }
            //-----------------------------
            for (int n = 0; n < m; ++n)
            {
                _controls[n].RemoveSelf();
            }
            _controls.Clear(); //***
            _simpleBox.ClearChildren();

            //2. create new control points...

            int j = vxs.Count;
            for (int i = 0; i < j; ++i)
            {
                var cmd = vxs.GetVertex(i, out double x, out double y);
                if (iCoordTx != null)
                {
                    iCoordTx.Transform(ref x, ref y);
                }

                switch (cmd)
                {
                    case VertexCmd.NoMore:
                        return;
                    case VertexCmd.MoveTo:
                        {

                            var ctrlPoint = new PointControllerBox(8, 8);
                            ctrlPoint.Index = i;
                            ctrlPoint.SetLocation((int)(x + offsetX), (int)(y + offsetY));
                            SetupCornerBoxController(ctrlPoint);
                            _controls.Add(ctrlPoint);
                            //...
                            _simpleBox.Add(ctrlPoint);
                        }
                        break;
                    case VertexCmd.LineTo:
                        {
                            var ctrlPoint = new PointControllerBox(8, 8);
                            ctrlPoint.Index = i;
                            ctrlPoint.SetLocation((int)(x + offsetX), (int)(y + offsetY));
                            SetupCornerBoxController(ctrlPoint);
                            _controls.Add(ctrlPoint);
                            //...
                            _simpleBox.Add(ctrlPoint);
                        }
                        break;
                    case VertexCmd.Close:
                        break;
                }
            }

        }
        void SetupCornerBoxController(PointControllerBox box)
        {
            Color c = KnownColors.FromKnownColor(KnownColor.Orange);
            box.BackColor = new Color(100, c.R, c.G, c.B);

            //controllerBox1.dbugTag = 3;
            box.Visible = true;
            //for controller box  
            box.MouseDrag += (s, e) =>
            {
                var pos = box.Position;
                int newX = pos.X + e.XDiff;
                int newY = pos.Y + e.YDiff;
                box.SetLocation(newX, newY);
                //var targetBox = cornerBox.TargetBox;
                //if (targetBox != null)
                //{
                //    //move target box too
                //    targetBox.SetLocation(newX + 5, newY + 5);
                //}
                e.CancelBubbling = true;
                //---------------------------------
                _simpleBox.InvalidateGraphics();
                foreach (var ctrl in _controls)
                {
                    ctrl.InvalidateGraphics();
                }
                //then update the vxs shape 
                //_vxs.ReplaceVertex(box.Index, newX, newY);
                _uiListener?.HandleElementUpdate();
            };

        }

    }


}