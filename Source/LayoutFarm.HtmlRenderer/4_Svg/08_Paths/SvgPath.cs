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

            if (this.IsPathValid) { return; }
            ClearCachePath();

            if (segments == null)
            {
                this.myCachedPath = null;
            }
            else
            {    
                List<SvgPathSeg> segs = this.segments;
                int segcount = segs.Count;  


                GraphicsPath gpath = this.myCachedPath = CurrentGraphicPlatform.CreateGraphicPath();
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
                        case SvgPathCommand.HorizontalLineTo:
                            {
                                var hlintTo = (SvgPathSegLineToHorizontal)seg;

                                if (hlintTo.IsRelative)
                                {
                                    gpath.AddLine(new PointF(lastX, lastY),
                                       new PointF(lastX += hlintTo.X, lastY));
                                }
                                else
                                {
                                    gpath.AddLine(new PointF(lastX, lastY),
                                       new PointF(lastX = hlintTo.X, lastY));
                                }

                            } break;
                        case SvgPathCommand.VerticalLineTo:
                            {
                                var vlineTo = (SvgPathSegLineToVertical)seg;
                                if (vlineTo.IsRelative)
                                {
                                    gpath.AddLine(new PointF(lastX, lastY),
                                       new PointF(lastX, lastY += vlineTo.Y));
                                }
                                else
                                {
                                    gpath.AddLine(new PointF(lastX, lastY),
                                       new PointF(lastX, lastY = vlineTo.Y));
                                }

                            } break;
                        case SvgPathCommand.Arc:
                            {
                                var arcTo = (SvgPathSegArc)seg;
                                PointF start = new PointF(lastX, lastY);
                                PointF end = new PointF(arcTo.X, arcTo.Y);
                                if (start.IsEq(end))
                                {
                                    return;
                                }
                                if (arcTo.R1 == 0 && arcTo.R2 == 0)
                                {
                                    gpath.AddLine(start, end);
                                    return;
                                }
                                float angle = arcTo.Angle;
                                float rx = arcTo.R1; //rx
                                float ry = arcTo.R2; //ry
                                SvgArcSize arcSize = arcTo.LargeArgFlag;
                                SvgArcSweep arcSweep = arcTo.SweepFlag;

                                if (arcTo.IsRelative)
                                {
                                    //make absolute path
                                    lastX = end.X += lastX;
                                    lastY = end.Y += lastY;

                                }


                                double sinPhi = Math.Sin(angle * SvgPathSegArc.RAD_PER_DEG);
                                double cosPhi = Math.Cos(angle * SvgPathSegArc.RAD_PER_DEG);

                                double x1dash = cosPhi * (start.X - end.X) / 2.0 + sinPhi * (start.Y - end.Y) / 2.0;
                                double y1dash = -sinPhi * (start.X - end.X) / 2.0 + cosPhi * (start.Y - end.Y) / 2.0;

                                double root;
                                double numerator = (rx * rx * ry * ry) - (rx * rx * y1dash * y1dash) - (ry * ry * x1dash * x1dash);

                                if (numerator < 0.0)
                                {
                                    float s = (float)Math.Sqrt(1.0 - numerator / (rx * rx * ry * ry));
                                    rx *= s;
                                    ry *= s;
                                    root = 0.0;
                                }
                                else
                                {
                                    root = ((arcSize == SvgArcSize.Large && arcSweep == SvgArcSweep.Positive)
                                        || (arcSize == SvgArcSize.Small && arcSweep == SvgArcSweep.Negative) ? -1.0 : 1.0) * Math.Sqrt(numerator / (rx * rx * y1dash * y1dash + ry * ry * x1dash * x1dash));
                                }

                                double cxdash = root * rx * y1dash / ry;
                                double cydash = -root * ry * x1dash / rx;

                                double cx = cosPhi * cxdash - sinPhi * cydash + (start.X + end.X) / 2.0;
                                double cy = sinPhi * cxdash + cosPhi * cydash + (start.Y + end.Y) / 2.0;

                                double theta1 = SvgPathSegArc.CalculateVectorAngle(1.0, 0.0, (x1dash - cxdash) / rx, (y1dash - cydash) / ry);
                                double dtheta = SvgPathSegArc.CalculateVectorAngle((x1dash - cxdash) / rx, (y1dash - cydash) / ry, (-x1dash - cxdash) / rx, (-y1dash - cydash) / ry);

                                if (arcSweep == SvgArcSweep.Negative && dtheta > 0)
                                {
                                    dtheta -= 2.0 * Math.PI;
                                }
                                else if (arcSweep == SvgArcSweep.Positive && dtheta < 0)
                                {
                                    dtheta += 2.0 * Math.PI;
                                }

                                int nsegments = (int)Math.Ceiling((double)Math.Abs(dtheta / (Math.PI / 2.0)));

                                double delta = dtheta / nsegments;
                                double t = 8.0 / 3.0 * Math.Sin(delta / 4.0) * Math.Sin(delta / 4.0) / Math.Sin(delta / 2.0);

                                double startX = start.X;
                                double startY = start.Y;

                                for (int n = 0; n < nsegments; ++n)
                                {
                                    double cosTheta1 = Math.Cos(theta1);
                                    double sinTheta1 = Math.Sin(theta1);
                                    double theta2 = theta1 + delta;
                                    double cosTheta2 = Math.Cos(theta2);
                                    double sinTheta2 = Math.Sin(theta2);

                                    double endpointX = cosPhi * rx * cosTheta2 - sinPhi * ry * sinTheta2 + cx;
                                    double endpointY = sinPhi * rx * cosTheta2 + cosPhi * ry * sinTheta2 + cy;

                                    double dx1 = t * (-cosPhi * rx * sinTheta1 - sinPhi * ry * cosTheta1);
                                    double dy1 = t * (-sinPhi * rx * sinTheta1 + cosPhi * ry * cosTheta1);

                                    double dxe = t * (cosPhi * rx * sinTheta2 + sinPhi * ry * cosTheta2);
                                    double dye = t * (sinPhi * rx * sinTheta2 - cosPhi * ry * cosTheta2);


                                    gpath.AddBezierCurve(
                                        new PointF((float)startX, (float)startY),
                                        new PointF((float)(startX + dx1), (float)(startY + dy1)),
                                        new PointF((float)(endpointX + dxe), (float)(endpointY + dye)),
                                        new PointF((float)endpointX, (float)endpointY));

                                    theta1 = theta2;
                                    startX = (float)endpointX;
                                    startY = (float)endpointY;
                                }
                            } break;
                        case SvgPathCommand.CurveTo:
                            {
                                var cubicCurve = (SvgPathSegCurveToCubic)seg;
                                if (cubicCurve.IsRelative)
                                {
                                    //relative
                                    PointF p1 = new PointF(lastX, lastY);
                                    PointF p2 = new PointF(lastX + cubicCurve.X1, lastY + cubicCurve.Y1);
                                    PointF p3 = new PointF(lastX + cubicCurve.X2, lastY + cubicCurve.Y2);
                                    PointF p4 = new PointF(lastX += cubicCurve.X, lastY += cubicCurve.Y);
                                    gpath.AddBezierCurve(p1, p2, p3, p4);
                                }
                                else
                                {
                                    PointF p1 = new PointF(lastX, lastY);
                                    PointF p2 = new PointF(cubicCurve.X1, cubicCurve.Y1);
                                    PointF p3 = new PointF(cubicCurve.X2, cubicCurve.Y2);
                                    PointF p4 = new PointF(lastX = cubicCurve.X, lastY = cubicCurve.Y);
                                    gpath.AddBezierCurve(p1, p2, p3, p4);
                                }
                            } break;
                        case SvgPathCommand.SmoothCurveTo:
                            {
                                var scubicCurve = (SvgPathSegCurveToCubicSmooth)seg;
                                //connect with prev segment
                                if (i > 0)
                                {
                                    SvgPathSegCurveToCubic prevCurve = segments[i - 1] as SvgPathSegCurveToCubic;
                                    if (prevCurve != null)
                                    {
                                        //use 1st control point from prev segment
                                        PointF p1 = new PointF(lastX, lastY);
                                        PointF p2 = new PointF(prevCurve.X2, prevCurve.Y2);
                                        if (prevCurve.IsRelative)
                                        {
                                            float diffX = lastX - prevCurve.X;
                                            float diffY = lastY - prevCurve.Y;
                                            p2 = new PointF(prevCurve.X2 - diffX, prevCurve.Y2 - diffY);
                                        }

                                        //make a mirror point***
                                        p2 = SvgPathSegCurveToCubic.MakeMirrorPoint(p1, p2);

                                        if (scubicCurve.IsRelative)
                                        {
                                            PointF p3 = new PointF(scubicCurve.X2 + lastX, scubicCurve.Y2 + lastY);
                                            PointF p4 = new PointF(lastX = scubicCurve.X + lastX, lastY = scubicCurve.Y + lastY);
                                            gpath.AddBezierCurve(p1, p2, p3, p4);
                                        }
                                        else
                                        {
                                            PointF p3 = new PointF(scubicCurve.X2, scubicCurve.Y2);
                                            PointF p4 = new PointF(lastX = scubicCurve.X, lastY = scubicCurve.Y);
                                            gpath.AddBezierCurve(p1, p2, p3, p4);
                                        }

                                    }
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

            ValidatePath();
        }
        public override void Paint(Painter p)
        {
            IGraphics g = p.Gfx;
            if (fillColor != Color.Transparent)
            {
                using (SolidBrush sb = g.Platform.CreateSolidBrush(this.fillColor))
                {
                    g.FillPath(sb, this.myCachedPath);
                }
            }
            if (this.strokeColor != Color.Transparent)
            {
                using (SolidBrush sb = g.Platform.CreateSolidBrush(this.strokeColor))
                using (Pen pen = g.Platform.CreatePen(sb))
                {
                    pen.Width = this.ActualStrokeWidth;
                    g.DrawPath(pen, this.myCachedPath);
                }
            }

        }

    }
}