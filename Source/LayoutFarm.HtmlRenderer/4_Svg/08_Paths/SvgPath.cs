using System;
using System.Collections.Generic;
using System.Text;

using LayoutFarm.Drawing;

using Svg.Pathing;
using Svg.Transforms;
using HtmlRenderer;

namespace LayoutFarm.SvgDom
{

    public class SvgPath : SvgVisualElement
    {
        Color strokeColor = Color.Transparent;
        Color fillColor = Color.Black;

        GraphicsPath _path;

        SvgPathSpec spec;
        List<Svg.Pathing.SvgPathSeg> segments;
        public SvgPath(SvgPathSpec spec, object controller)
            : base(controller)
        {
            this.spec = spec;
        }
        public List<Svg.Pathing.SvgPathSeg> Segments
        {
            get { return this.segments; }
            set { this.segments = value; }
        }


        public override void ReEvaluateComputeValue(float containerW, float containerH, float emHeight)
        {
            var myspec = this.spec;
            this.fillColor = myspec.ActualColor;
            this.strokeColor = myspec.StrokeColor;
            this.ActualStrokeWidth = ConvertToPx(myspec.StrokeWidth, containerW, emHeight);

            if (segments == null)
            {
                this._path = null;
            }
            else
            {
                GraphicsPath gpath = this._path = CurrentGraphicPlatform.CreateGraphicPath();

                List<SvgPathSeg> segs = this.segments;
                int segcount = segs.Count;

                float lastMoveX = 0;
                float lastMoveY = 0;

                float lastX = 0;
                float lastY = 0;
                for (int i = 0; i < segcount; ++i)
                {
                    SvgPathSeg seg = segs[i];

                    switch (seg.Command)
                    {
                        case SvgPathCommand.MoveTo:
                            {
                                var moveTo = (SvgPathSegMoveTo)seg;
                                if (moveTo.IsRelative)
                                {
                                    lastX = lastMoveX = lastX + moveTo.X;
                                    lastY = lastMoveY = lastY + moveTo.Y;
                                }
                                else
                                {
                                    lastX = lastMoveX = moveTo.X;
                                    lastY = lastMoveY = moveTo.Y;
                                }
                                gpath.StartFigure();

                            } break;
                        case SvgPathCommand.LineTo:
                            {
                                var lineTo = (SvgPathSegLineTo)seg;

                                if (lineTo.IsRelative)
                                {
                                    gpath.AddLine(new PointF(lastX, lastY),
                                       new PointF(lastX += lineTo.X, lastY += lineTo.Y));
                                }
                                else
                                {
                                    gpath.AddLine(new PointF(lastX, lastY),
                                       new PointF(lastX = lineTo.X, lastY = lineTo.Y));
                                }
                            } break;
                        case SvgPathCommand.QuadraticBezierCurve:
                            {
                                var quadCurve = (SvgPathSegCurveToQuadratic)seg;
                                if (quadCurve.IsRelative)
                                {
                                    //relative
                                    PointF p1 = new PointF(lastX, lastY);
                                    PointF c = new PointF(lastX + quadCurve.X1, lastY + quadCurve.Y1);
                                    PointF p4 = new PointF(lastX += quadCurve.X, lastY += quadCurve.Y);
                                   

                                    PointF p2, p3;

                                    SvgPathSegCurveToQuadratic.GetControlPoints(p1, c, p4, out p2, out p3);
                                    gpath.AddBezierCurve(p1, p2, p3, p4);
                                }
                                else
                                {
                                    PointF p1 = new PointF(lastX, lastY);
                                    PointF c = new PointF(quadCurve.X1, quadCurve.Y1);
                                    PointF p4 = new PointF(lastX = quadCurve.X, lastY = quadCurve.Y);
                                 

                                    PointF p2, p3;

                                    SvgPathSegCurveToQuadratic.GetControlPoints(p1, c, p4, out p2, out p3);
                                    gpath.AddBezierCurve(p1, p2, p3, p4);
                                }

                            } break;
                        case SvgPathCommand.ZClosePath:
                            {
                                gpath.CloseFigure();

                            } break;
                        default:
                            throw new NotSupportedException();
                    }
                }
            }
        }
        public override void Paint(Painter p)
        {
            IGraphics g = p.Gfx;
            if (fillColor != Color.Transparent)
            {
                using (SolidBrush sb = g.Platform.CreateSolidBrush(this.fillColor))
                {
                    g.FillPath(sb, this._path);
                }
            }
            if (this.strokeColor != Color.Transparent)
            {
                using (SolidBrush sb = g.Platform.CreateSolidBrush(this.strokeColor))
                using (Pen pen = g.Platform.CreatePen(sb))
                {
                    pen.Width = this.ActualStrokeWidth;
                    g.DrawPath(pen, this._path);
                }
            }
            
        }

    }
}