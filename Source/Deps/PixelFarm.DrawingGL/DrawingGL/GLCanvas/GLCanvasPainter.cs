//MIT, 2016-2017, WinterDev

using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
using PixelFarm.Agg;
using PixelFarm.Agg.Transform;
using PixelFarm.Agg.VertexSource;
using PixelFarm.Drawing.Fonts;


namespace PixelFarm.DrawingGL
{
    public sealed class GLCanvasPainter : CanvasPainter
    {
        CanvasGL2d _canvas;
        int _width;
        int _height;
        Color _fillColor;
        Color _strokeColor;
        RectInt _rectInt;

        RoundedRect roundRect;
        Arc arcTool;
        Ellipse ellipse = new Ellipse();
        Stroke _aggStroke = new Stroke(1);
        RequestFont _requestFont;
        ITextPrinter _textPriner;

        SmoothingMode _smoothingMode; //smoothing mode of this  painter
        public GLCanvasPainter(CanvasGL2d canvas, int w, int h)
        {
            _canvas = canvas;
            _width = w;
            _height = h;
            _rectInt = new RectInt(0, 0, w, h);
            arcTool = new Arc();
            CurrentFont = new RequestFont("tahoma", 14);
        }
        public override void SetOrigin(float ox, float oy)
        {
            _canvas.SetCanvasOrigin((int)ox, (int)oy);
        }
        public CanvasGL2d Canvas { get { return this._canvas; } }

        public override RequestFont CurrentFont
        {
            get
            {
                return _requestFont;
            }
            set
            {
                _requestFont = value;
                if (_textPriner != null)
                {
                    _textPriner.ChangeFont(value);
                }
            }
        }
        public override RectInt ClipBox
        {
            get
            {
                return _rectInt;
            }

            set
            {
                _rectInt = value;
            }
        }
        public override SmoothingMode SmoothingMode
        {
            get
            {
                return _smoothingMode;
            }
            set
            {
                switch (_smoothingMode = value)
                {
                    case SmoothingMode.HighQuality:
                    case SmoothingMode.AntiAlias:
                        _canvas.SmoothMode = CanvasSmoothMode.Smooth;
                        break;
                    default:
                        _canvas.SmoothMode = CanvasSmoothMode.No;
                        break;
                }

            }
        }
        public ITextPrinter TextPrinter
        {
            get { return _textPriner; }
            set
            {
                _textPriner = value;
                if (value != null && _requestFont != null)
                {
                    _textPriner.ChangeFont(this._requestFont);
                }
            }
        }
        public override Color FillColor
        {
            get
            {
                return _fillColor;
            }
            set
            {
                _fillColor = value;
                if (_textPriner != null)
                {
                    _textPriner.ChangeFontColor(value);
                }
            }
        }
        public override int Height
        {
            get
            {
                return this._height;
            }
        }

        public override Color StrokeColor
        {
            get
            {
                return _strokeColor;
            }
            set
            {
                _strokeColor = value;
                _canvas.StrokeColor = value;
            }
        }

        public override double StrokeWidth
        {
            get
            {
                return _canvas.StrokeWidth;
            }
            set
            {
                _canvas.StrokeWidth = (float)value;
            }
        }

        public override bool UseSubPixelRendering
        {
            get
            {
                return _canvas.SmoothMode == CanvasSmoothMode.Smooth;
            }

            set
            {
                _canvas.SmoothMode = value ? CanvasSmoothMode.Smooth : CanvasSmoothMode.No;
            }
        }

        public override int Width
        {
            get
            {
                return _width;
            }
        }

        public override void Clear(Color color)
        {
            _canvas.Clear(color);
        }
        public override void DoFilterBlurRecursive(RectInt area, int r)
        {
            //filter with glsl
        }
        public override void DoFilterBlurStack(RectInt area, int r)
        {
        }
        /// <summary>
        /// we do NOT store vxs
        /// </summary>
        /// <param name="vxs"></param>
        public override void Draw(VertexStore vxs)
        {
            _canvas.DrawGfxPath(this._strokeColor,
                InternalGraphicsPath.CreateGraphicsPath(new VertexStoreSnap(vxs)));
        }

        public override void DrawBezierCurve(float startX, float startY, float endX, float endY, float controlX1, float controlY1, float controlX2, float controlY2)
        {
            var v1 = GetFreeVxs();
            BezierCurve.CreateBezierVxs4(v1,
                new PixelFarm.VectorMath.Vector2(startX, startY),
                new PixelFarm.VectorMath.Vector2(endX, endY),
                new PixelFarm.VectorMath.Vector2(controlX1, controlY1),
                new PixelFarm.VectorMath.Vector2(controlY2, controlY2));
            _aggStroke.Width = this.StrokeWidth;

            var v2 = GetFreeVxs();
            _canvas.DrawGfxPath(_canvas.StrokeColor, InternalGraphicsPath.CreateGraphicsPath(new VertexStoreSnap(_aggStroke.MakeVxs(v1, v2))));
            ReleaseVxs(ref v2);
            ReleaseVxs(ref v1);
        }

        public override void DrawImage(ActualImage actualImage, params AffinePlan[] affinePlans)
        {
            //create gl bmp
            GLBitmap glBmp = new GLBitmap(actualImage.Width, actualImage.Height, ActualImage.GetBuffer(actualImage), false);
            _canvas.DrawImage(glBmp, 0, 0);
            glBmp.Dispose();
        }
        public override void DrawImage(ActualImage actualImage, double x, double y)
        {
            GLBitmap glBmp = new GLBitmap(actualImage.Width, actualImage.Height, ActualImage.GetBuffer(actualImage), false);
            _canvas.DrawImage(glBmp, (float)x, (float)y);
            glBmp.Dispose();
        }
        public override void DrawRoundRect(double left, double bottom, double right, double top, double radius)
        {
            if (roundRect == null)
            {
                roundRect = new RoundedRect(left, bottom, right, top, radius);
                roundRect.NormalizeRadius();
            }
            else
            {
                roundRect.SetRect(left, bottom, right, top);
                roundRect.SetRadius(radius);
                roundRect.NormalizeRadius();
            }
            var v1 = GetFreeVxs();
            this.Draw(roundRect.MakeVxs(v1));
            ReleaseVxs(ref v1);
        }
        public override float OriginX
        {
            get
            {
                return _canvas.CanvasOriginX;
            }
        }
        public override float OriginY
        {
            get
            {
                return _canvas.CanvasOriginY;
            }
        }
        public override void DrawString(string text, double x, double y)
        {
            if (_textPriner == null) { return; }
            //--------------------------------
            _textPriner.DrawString(text, x, y);

        }



        public override void Fill(VertexStore vxs)
        {
            _canvas.FillGfxPath(
                _fillColor,
                InternalGraphicsPath.CreateGraphicsPath(new VertexStoreSnap(vxs))
                );
        }

        public override void Fill(VertexStoreSnap snap)
        {
            _canvas.FillGfxPath(
                _fillColor,
               InternalGraphicsPath.CreateGraphicsPath(snap));
        }
        public override void Draw(VertexStoreSnap snap)
        {
            _canvas.DrawGfxPath(
             this._strokeColor,
             InternalGraphicsPath.CreateGraphicsPath(snap)
             );
        }

        public override void FillCircle(double x, double y, double radius)
        {
            FillEllipse(x - radius, y - radius, x + radius, y + radius);
        }

        public override void FillCircle(double x, double y, double radius, Color color)
        {
            Color prevColor = _fillColor;
            _fillColor = color;
            FillEllipse(x - radius, y - radius, x + radius, y + radius);
            _fillColor = prevColor; //reset
        }
        public void FillRoundRect(Color color, float x, float y, float w, float h, float rx, float ry)
        {
            roundRect.SetRect(x, y, x + w, y + h);
            roundRect.SetRadius(rx, ry);
            //create round rect vxs

            var vxs = roundRect.MakeVxs(GetFreeVxs());
            _canvas.FillGfxPath(_fillColor, InternalGraphicsPath.CreateGraphicsPath(new VertexStoreSnap(vxs)));
            ReleaseVxs(ref vxs);
        }
        public void DrawRoundRect(float x, float y, float w, float h, float rx, float ry)
        {
            roundRect.SetRect(x, y, x + w, y + h);
            roundRect.SetRadius(rx, ry);
            _aggStroke.Width = this.StrokeWidth;

            var v1 = GetFreeVxs();
            var v2 = GetFreeVxs();
            _aggStroke.MakeVxs(roundRect.MakeVxs(v1), v2);
            _canvas.DrawGfxPath(_strokeColor, InternalGraphicsPath.CreateGraphicsPath(new VertexStoreSnap(v2)));
            ReleaseVxs(ref v2);
            ReleaseVxs(ref v1);
        }

        public override void DrawEllipse(double left, double bottom, double right, double top)
        {
            double x = (left + right) / 2;
            double y = (bottom + top) / 2;
            double rx = Math.Abs(right - x);
            double ry = Math.Abs(top - y);
            ellipse.Reset(x, y, rx, ry);
            VertexStore vxs = ellipse.MakeVxs(GetFreeVxs());
            _canvas.DrawGfxPath(_strokeColor, InternalGraphicsPath.CreateGraphicsPath(new VertexStoreSnap(vxs)));
            ReleaseVxs(ref vxs);
        }
        public override void FillEllipse(double left, double bottom, double right, double top)
        {
            double x = (left + right) / 2;
            double y = (bottom + top) / 2;
            double rx = Math.Abs(right - x);
            double ry = Math.Abs(top - y);
            ellipse.Reset(x, y, rx, ry);
            var v1 = GetFreeVxs();
            ellipse.MakeVxs(v1);
            //other mode
            int n = v1.Count;
            //make triangular fan*** 

            float[] coords = new float[(n * 2) + 4];
            int i = 0;
            int nn = 0;
            int npoints = 0;
            double vx, vy;
            //center
            coords[nn++] = (float)x;
            coords[nn++] = (float)y;
            npoints++;
            var cmd = v1.GetVertex(i, out vx, out vy);
            while (i < n)
            {
                switch (cmd)
                {
                    case VertexCmd.MoveTo:
                        {
                            coords[nn++] = (float)vx;
                            coords[nn++] = (float)vy;
                            npoints++;
                        }
                        break;
                    case VertexCmd.LineTo:
                        {
                            coords[nn++] = (float)vx;
                            coords[nn++] = (float)vy;
                            npoints++;
                        }
                        break;
                    case VertexCmd.NoMore:
                        {
                        }
                        break;
                    default:
                        {
                        }
                        break;
                }
                i++;
                cmd = v1.GetVertex(i, out vx, out vy);
            }


            //close circle
            coords[nn++] = coords[2];
            coords[nn++] = coords[3];
            npoints++;
            //----------------------------------------------
            _canvas.FillTriangleFan(_fillColor, coords, npoints);
            ReleaseVxs(ref v1);
        }
        public override void FillRectangle(double left, double bottom, double right, double top)
        {
            FillRect((float)left, (float)bottom, (float)(right - left), (float)(top - bottom));
        }
        public override void FillRectangle(double left, double bottom, double right, double top, Color fillColor)
        {
            Color prev_color = FillColor; //store prev value
            FillColor = fillColor;
            FillRect((float)left, (float)bottom, (float)(right - left), (float)(top - bottom));
            FillColor = prev_color;
        }
        public override void FillRectLBWH(double left, double bottom, double width, double height)
        {
            FillRect((float)left, (float)bottom, (float)width, (float)height);
        }

        public override void FillRenderVx(Brush brush, RenderVx renderVx)
        {
            _canvas.FillRenderVx(brush, renderVx);
        }
        public override void FillRenderVx(RenderVx renderVx)
        {
            _canvas.FillRenderVx(_fillColor, renderVx);
        }
        public override void DrawRenderVx(RenderVx renderVx)
        {
            _canvas.DrawRenderVx(_strokeColor, renderVx);
        }



        void FillRect(float x, float y, float w, float h)
        {
            float[] coords = CreateRectTessCoordsTriStrip(x, y, w, h);
            _canvas.FillTriangleStrip(_fillColor, coords, 4);
        }
        static float[] CreateRectTessCoordsTriStrip(float x, float y, float w, float h)
        {
            //float x0 = x;
            //float y0 = y + h;
            //float x1 = x;
            //float y1 = y;
            //float x2 = x + w;
            //float y2 = y + h;
            //float x3 = x + w;
            //float y3 = y;
            return new float[]{
               x,y + h,
               x,y,
               x + w, y + h,
               x + w, y,
            };
        }
        public override void FillRoundRectangle(double left, double bottom, double right, double top, double radius)
        {
            if (roundRect == null)
            {
                roundRect = new Agg.VertexSource.RoundedRect(left, bottom, right, top, radius);
                roundRect.NormalizeRadius();
            }
            else
            {
                roundRect.SetRect(left, bottom, right, top);
                roundRect.SetRadius(radius);
                roundRect.NormalizeRadius();
            }
            var v1 = GetFreeVxs();
            this.Fill(roundRect.MakeVxs(v1));
            ReleaseVxs(ref v1);
        }



        public override void Line(double x1, double y1, double x2, double y2)
        {
            _canvas.StrokeColor = _strokeColor;
            _canvas.DrawLine((float)x1, (float)y1, (float)x2, (float)y2);
        }
        public override void Line(double x1, double y1, double x2, double y2, Color color)
        {
            _canvas.StrokeColor = color;
            _canvas.DrawLine((float)x1, (float)y1, (float)x2, (float)y2);
        }
        public override void PaintSeries(VertexStore vxs, Color[] colors, int[] pathIndexs, int numPath)
        {
            //TODO: review here.
            //
            for (int i = 0; i < numPath; ++i)
            {
                _canvas.FillGfxPath(colors[i], InternalGraphicsPath.CreateGraphicsPath(new VertexStoreSnap(vxs, pathIndexs[i])));
            }
        }
        public override void Rectangle(double left, double bottom, double right, double top)
        {
            //draw rectangle
            _canvas.DrawRect((float)left, (float)bottom, (float)(right - left), (float)(top - bottom));
        }
        public override void Rectangle(double left, double bottom, double right, double top, Color color)
        {   //draw rectangle
            var prev = _canvas.StrokeColor;
            _canvas.StrokeColor = color;
            _canvas.DrawRect((float)left, (float)bottom, (float)(right - left), (float)(top - bottom));
            _canvas.StrokeColor = prev;
        }
        public override void SetClipBox(int x1, int y1, int x2, int y2)
        {
        }
        public void DrawCircle(float x, float y, double radius)
        {
            DrawEllipse(x - radius, y - radius, x + radius, y + radius);
        }
        //-----------------------------------------------------------------------------------------------------------------
        public override RenderVx CreateRenderVx(VertexStoreSnap snap)
        {
            //store internal gfx path inside render vx
            return new GLRenderVx(InternalGraphicsPath.CreateGraphicsPath(snap));
        }
        public RenderVx CreatePolygonRenderVx(float[] xycoords)
        {
            //store internal gfx path inside render vx
            return new GLRenderVx(InternalGraphicsPath.CreatePolygonGraphicsPath(xycoords));
        }

        struct CenterFormArc
        {
            public double cx;
            public double cy;
            public double radStartAngle;
            public double radSweepDiff;
            public bool scaleUp;
        }

        VertexStorePool _vxsPool = new VertexStorePool();
        VertexStore GetFreeVxs()
        {
            return _vxsPool.GetFreeVxs();
        }
        void ReleaseVxs(ref VertexStore vxs)
        {
            _vxsPool.Release(ref vxs);
        }
        //---------------------------------------------------------------------
        public void DrawArc(float fromX, float fromY, float endX, float endY,
         float xaxisRotationAngleDec, float rx, float ry,
         SvgArcSize arcSize, SvgArcSweep arcSweep)
        {
            //------------------
            //SVG Elliptical arc ...
            //from Apache Batik
            //-----------------

            CenterFormArc centerFormArc = new CenterFormArc();
            ComputeArc2(fromX, fromY, rx, ry,
                 DegToRad(xaxisRotationAngleDec),
                 arcSize == SvgArcSize.Large,
                 arcSweep == SvgArcSweep.Negative,
                 endX, endY, ref centerFormArc);
            arcTool.Init(centerFormArc.cx, centerFormArc.cy, rx, ry,
                centerFormArc.radStartAngle,
                (centerFormArc.radStartAngle + centerFormArc.radSweepDiff));

            VertexStore v1 = GetFreeVxs();
            bool stopLoop = false;
            foreach (VertexData vertexData in arcTool.GetVertexIter())
            {
                switch (vertexData.command)
                {
                    case VertexCmd.NoMore:
                        stopLoop = true;
                        break;
                    default:
                        v1.AddVertex(vertexData.x, vertexData.y, vertexData.command);
                        //yield return vertexData;
                        break;
                }
                //------------------------------
                if (stopLoop) { break; }
            }

            double scaleRatio = 1;
            if (centerFormArc.scaleUp)
            {
                int vxs_count = v1.Count;
                double px0, py0, px_last, py_last;
                v1.GetVertex(0, out px0, out py0);
                v1.GetVertex(vxs_count - 1, out px_last, out py_last);
                double distance1 = Math.Sqrt((px_last - px0) * (px_last - px0) + (py_last - py0) * (py_last - py0));
                double distance2 = Math.Sqrt((endX - fromX) * (endX - fromX) + (endY - fromY) * (endY - fromY));
                if (distance1 < distance2)
                {
                    scaleRatio = distance2 / distance1;
                }
                else
                {
                }
            }

            if (xaxisRotationAngleDec != 0)
            {
                //also  rotate 
                if (centerFormArc.scaleUp)
                {
                    var mat = PixelFarm.Agg.Transform.Affine.NewMatix(
                            new PixelFarm.Agg.Transform.AffinePlan(PixelFarm.Agg.Transform.AffineMatrixCommand.Translate, -centerFormArc.cx, -centerFormArc.cy),
                            new PixelFarm.Agg.Transform.AffinePlan(PixelFarm.Agg.Transform.AffineMatrixCommand.Scale, scaleRatio, scaleRatio),
                            new PixelFarm.Agg.Transform.AffinePlan(PixelFarm.Agg.Transform.AffineMatrixCommand.Rotate, DegToRad(xaxisRotationAngleDec)),
                            new PixelFarm.Agg.Transform.AffinePlan(PixelFarm.Agg.Transform.AffineMatrixCommand.Translate, centerFormArc.cx, centerFormArc.cy));
                    var v2 = GetFreeVxs();
                    mat.TransformToVxs(v1, v2);
                    ReleaseVxs(ref v1);
                    v1 = v2;
                }
                else
                {
                    //not scalue
                    var mat = PixelFarm.Agg.Transform.Affine.NewMatix(
                            new PixelFarm.Agg.Transform.AffinePlan(PixelFarm.Agg.Transform.AffineMatrixCommand.Translate, -centerFormArc.cx, -centerFormArc.cy),
                            new PixelFarm.Agg.Transform.AffinePlan(PixelFarm.Agg.Transform.AffineMatrixCommand.Rotate, DegToRad(xaxisRotationAngleDec)),
                            new PixelFarm.Agg.Transform.AffinePlan(PixelFarm.Agg.Transform.AffineMatrixCommand.Translate, centerFormArc.cx, centerFormArc.cy));
                    var v2 = GetFreeVxs();
                    mat.TransformToVxs(v1, v2);
                    ReleaseVxs(ref v1);
                    v1 = v2;
                }
            }
            else
            {
                //no rotate
                if (centerFormArc.scaleUp)
                {
                    var mat = PixelFarm.Agg.Transform.Affine.NewMatix(
                            new PixelFarm.Agg.Transform.AffinePlan(PixelFarm.Agg.Transform.AffineMatrixCommand.Translate, -centerFormArc.cx, -centerFormArc.cy),
                            new PixelFarm.Agg.Transform.AffinePlan(PixelFarm.Agg.Transform.AffineMatrixCommand.Scale, scaleRatio, scaleRatio),
                            new PixelFarm.Agg.Transform.AffinePlan(PixelFarm.Agg.Transform.AffineMatrixCommand.Translate, centerFormArc.cx, centerFormArc.cy));
                    var v2 = GetFreeVxs();
                    mat.TransformToVxs(v1, v2);
                    ReleaseVxs(ref v1);
                    v1 = v2;
                }
            }

            _aggStroke.Width = this.StrokeWidth;


            var v3 = _aggStroke.MakeVxs(v1, GetFreeVxs());
            _canvas.DrawGfxPath(_canvas.StrokeColor, InternalGraphicsPath.CreateGraphicsPath(new VertexStoreSnap(v3)));

            ReleaseVxs(ref v3);
            ReleaseVxs(ref v1);

        }
        static double DegToRad(double degree)
        {
            return degree * (Math.PI / 180d);
        }
        static double RadToDeg(double degree)
        {
            return degree * (180d / Math.PI);
        }



        static void ComputeArc2(double x0, double y0,
                             double rx, double ry,
                             double xAngleRad,
                             bool largeArcFlag,
                             bool sweepFlag,
                             double x, double y, ref CenterFormArc result)
        {
            //from  SVG1.1 spec
            //----------------------------------
            //step1: Compute (x1dash,y1dash)
            //----------------------------------

            double dx2 = (x0 - x) / 2.0;
            double dy2 = (y0 - y) / 2.0;
            double cosAngle = Math.Cos(xAngleRad);
            double sinAngle = Math.Sin(xAngleRad);
            double x1 = (cosAngle * dx2 + sinAngle * dy2);
            double y1 = (-sinAngle * dx2 + cosAngle * dy2);
            // Ensure radii are large enough
            rx = Math.Abs(rx);
            ry = Math.Abs(ry);
            double prx = rx * rx;
            double pry = ry * ry;
            double px1 = x1 * x1;
            double py1 = y1 * y1;
            // check that radii are large enough


            double radiiCheck = px1 / prx + py1 / pry;
            if (radiiCheck > 1)
            {
                rx = Math.Sqrt(radiiCheck) * rx;
                ry = Math.Sqrt(radiiCheck) * ry;
                prx = rx * rx;
                pry = ry * ry;
                result.scaleUp = true;
            }

            //----------------------------------
            //step2: Compute (cx1,cy1)
            //----------------------------------
            double sign = (largeArcFlag == sweepFlag) ? -1 : 1;
            double sq = ((prx * pry) - (prx * py1) - (pry * px1)) / ((prx * py1) + (pry * px1));
            sq = (sq < 0) ? 0 : sq;
            double coef = (sign * Math.Sqrt(sq));
            double cx1 = coef * ((rx * y1) / ry);
            double cy1 = coef * -((ry * x1) / rx);
            //----------------------------------
            //step3:  Compute (cx, cy) from (cx1, cy1)
            //----------------------------------
            double sx2 = (x0 + x) / 2.0;
            double sy2 = (y0 + y) / 2.0;
            double cx = sx2 + (cosAngle * cx1 - sinAngle * cy1);
            double cy = sy2 + (sinAngle * cx1 + cosAngle * cy1);
            //----------------------------------
            //step4: Compute theta and anfkediff
            double ux = (x1 - cx1) / rx;
            double uy = (y1 - cy1) / ry;
            double vx = (-x1 - cx1) / rx;
            double vy = (-y1 - cy1) / ry;
            double p, n;
            // Compute the angle start
            n = Math.Sqrt((ux * ux) + (uy * uy));
            p = ux; // (1 * ux) + (0 * uy)
            sign = (uy < 0) ? -1d : 1d;
            double angleStart = (sign * Math.Acos(p / n));  // Math.toDegrees(sign * Math.Acos(p / n));
            // Compute the angle extent
            n = Math.Sqrt((ux * ux + uy * uy) * (vx * vx + vy * vy));
            p = ux * vx + uy * vy;
            sign = (ux * vy - uy * vx < 0) ? -1d : 1d;
            double angleExtent = (sign * Math.Acos(p / n));// Math.toDegrees(sign * Math.Acos(p / n));
            //if (!sweepFlag && angleExtent > 0)
            //{
            //    angleExtent -= 360f;
            //}
            //else if (sweepFlag && angleExtent < 0)
            //{
            //    angleExtent += 360f;
            //}

            result.cx = cx;
            result.cy = cy;
            result.radStartAngle = angleStart;
            result.radSweepDiff = angleExtent;
        }
        static Arc ComputeArc(double x0, double y0,
                              double rx, double ry,
                              double angle,
                              bool largeArcFlag,
                              bool sweepFlag,
                               double x, double y)
        {
            /** 
         * This constructs an unrotated Arc2D from the SVG specification of an 
         * Elliptical arc.  To get the final arc you need to apply a rotation
         * transform such as:
         * 
         * AffineTransform.getRotateInstance
         *     (angle, arc.getX()+arc.getWidth()/2, arc.getY()+arc.getHeight()/2);
         */
            //
            // Elliptical arc implementation based on the SVG specification notes
            //

            // Compute the half distance between the current and the final point
            double dx2 = (x0 - x) / 2.0;
            double dy2 = (y0 - y) / 2.0;
            // Convert angle from degrees to radians
            angle = ((angle % 360.0) * Math.PI / 180f);
            double cosAngle = Math.Cos(angle);
            double sinAngle = Math.Sin(angle);
            //
            // Step 1 : Compute (x1, y1)
            //
            double x1 = (cosAngle * dx2 + sinAngle * dy2);
            double y1 = (-sinAngle * dx2 + cosAngle * dy2);
            // Ensure radii are large enough
            rx = Math.Abs(rx);
            ry = Math.Abs(ry);
            double Prx = rx * rx;
            double Pry = ry * ry;
            double Px1 = x1 * x1;
            double Py1 = y1 * y1;
            // check that radii are large enough
            double radiiCheck = Px1 / Prx + Py1 / Pry;
            if (radiiCheck > 1)
            {
                rx = Math.Sqrt(radiiCheck) * rx;
                ry = Math.Sqrt(radiiCheck) * ry;
                Prx = rx * rx;
                Pry = ry * ry;
            }

            //
            // Step 2 : Compute (cx1, cy1)
            //
            double sign = (largeArcFlag == sweepFlag) ? -1 : 1;
            double sq = ((Prx * Pry) - (Prx * Py1) - (Pry * Px1)) / ((Prx * Py1) + (Pry * Px1));
            sq = (sq < 0) ? 0 : sq;
            double coef = (sign * Math.Sqrt(sq));
            double cx1 = coef * ((rx * y1) / ry);
            double cy1 = coef * -((ry * x1) / rx);
            //
            // Step 3 : Compute (cx, cy) from (cx1, cy1)
            //
            double sx2 = (x0 + x) / 2.0;
            double sy2 = (y0 + y) / 2.0;
            double cx = sx2 + (cosAngle * cx1 - sinAngle * cy1);
            double cy = sy2 + (sinAngle * cx1 + cosAngle * cy1);
            //
            // Step 4 : Compute the angleStart (angle1) and the angleExtent (dangle)
            //
            double ux = (x1 - cx1) / rx;
            double uy = (y1 - cy1) / ry;
            double vx = (-x1 - cx1) / rx;
            double vy = (-y1 - cy1) / ry;
            double p, n;
            // Compute the angle start
            n = Math.Sqrt((ux * ux) + (uy * uy));
            p = ux; // (1 * ux) + (0 * uy)
            sign = (uy < 0) ? -1d : 1d;
            double angleStart = (sign * Math.Acos(p / n));  // Math.toDegrees(sign * Math.Acos(p / n));
            // Compute the angle extent
            n = Math.Sqrt((ux * ux + uy * uy) * (vx * vx + vy * vy));
            p = ux * vx + uy * vy;
            sign = (ux * vy - uy * vx < 0) ? -1d : 1d;
            double angleExtent = (sign * Math.Acos(p / n));// Math.toDegrees(sign * Math.Acos(p / n));
            if (!sweepFlag && angleExtent > 0)
            {
                angleExtent -= 360f;
            }
            else if (sweepFlag && angleExtent < 0)
            {
                angleExtent += 360f;
            }
            //angleExtent %= 360f;
            //angleStart %= 360f;

            //
            // We can now build the resulting Arc2D in double precision
            //
            //Arc2D.Double arc = new Arc2D.Double();
            //arc.x = cx - rx;
            //arc.y = cy - ry;
            //arc.width = rx * 2.0;
            //arc.height = ry * 2.0;
            //arc.start = -angleStart;
            //arc.extent = -angleExtent;
            Arc arc = new Arc();
            arc.Init(x, y, rx, ry, -(angleStart), -(angleExtent));
            return arc;
        }
    }
}