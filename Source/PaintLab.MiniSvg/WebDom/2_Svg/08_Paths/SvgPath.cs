//Apache2, 2014-2017, WinterDev
using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
using LayoutFarm.HtmlBoxes;
using LayoutFarm.Svg.Pathing;
namespace LayoutFarm.Svg
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


        public override void ReEvaluateComputeValue(ref ReEvaluateArgs args)
        {
            var myspec = this.spec;
            this.fillColor = myspec.ActualColor;
            this.strokeColor = myspec.StrokeColor;
            this.ActualStrokeWidth = ConvertToPx(myspec.StrokeWidth, ref args);
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
                GraphicsPath gpath = this.myCachedPath = new GraphicsPath();
                float lastMoveX = 0;
                float lastMoveY = 0;
                PointF lastPoint = new PointF();
                PointF p2 = new PointF();//curve control point
                PointF p3 = new PointF();//curve control point
                PointF intm_c3_c = new PointF();
                for (int i = 0; i < segcount; ++i)
                {
                    SvgPathSeg seg = segs[i];
                    switch (seg.Command)
                    {
                        case SvgPathCommand.ZClosePath:
                            {
                                gpath.CloseFigure();
                            }
                            break;
                        case SvgPathCommand.MoveTo:
                            {
                                var moveTo = (SvgPathSegMoveTo)seg;
                                PointF moveToPoint;
                                moveTo.GetAbsolutePoints(ref lastPoint, out moveToPoint);
                                lastPoint = moveToPoint;
                                gpath.StartFigure();
                                lastMoveX = lastPoint.X;
                                lastMoveY = lastPoint.Y;
                            }
                            break;
                        case SvgPathCommand.LineTo:
                            {
                                var lineTo = (SvgPathSegLineTo)seg;
                                PointF lineToPoint;
                                lineTo.GetAbsolutePoints(ref lastPoint, out lineToPoint);
                                gpath.AddLine(lastPoint, lineToPoint);
                                lastPoint = lineToPoint;
                            }
                            break;
                        case SvgPathCommand.HorizontalLineTo:
                            {
                                var hlintTo = (SvgPathSegLineToHorizontal)seg;
                                PointF lineToPoint;
                                hlintTo.GetAbsolutePoints(ref lastPoint, out lineToPoint);
                                gpath.AddLine(lastPoint, lineToPoint);
                                lastPoint = lineToPoint;
                            }
                            break;
                        case SvgPathCommand.VerticalLineTo:
                            {
                                var vlineTo = (SvgPathSegLineToVertical)seg;
                                PointF lineToPoint;
                                vlineTo.GetAbsolutePoints(ref lastPoint, out lineToPoint);
                                gpath.AddLine(lastPoint, lineToPoint);
                                lastPoint = lineToPoint;
                            }
                            break;
                        //---------------------------------------------------------------------------
                        //curve modes...... 
                        case SvgPathCommand.CurveTo:
                            {
                                //cubic curve to  (2 control points)
                                var cubicCurve = (SvgPathSegCurveToCubic)seg;
                                PointF p;
                                cubicCurve.GetAbsolutePoints(ref lastPoint, out p2, out p3, out p);
                                gpath.AddBezierCurve(lastPoint, p2, p3, p);
                                lastPoint = p;
                            }
                            break;
                        case SvgPathCommand.QuadraticBezierCurve:
                            {
                                //quadratic curve (1 control point)
                                //auto calculate for c1,c2 
                                var quadCurve = (SvgPathSegCurveToQuadratic)seg;
                                PointF p;
                                quadCurve.GetAbsolutePoints(ref lastPoint, out intm_c3_c, out p);
                                SvgCurveHelper.Curve3GetControlPoints(lastPoint, intm_c3_c, p, out p2, out p3);
                                gpath.AddBezierCurve(lastPoint, p2, p3, p);
                                lastPoint = p;
                            }
                            break;
                        //------------------------------------------------------------------------------------
                        case SvgPathCommand.SmoothCurveTo:
                            {
                                //smooth cubic curve to
                                var smthC4 = (SvgPathSegCurveToCubicSmooth)seg;
                                PointF c2, p;
                                smthC4.GetAbsolutePoints(ref lastPoint, out c2, out p);
                                //connect with prev segment
                                if (i > 0)
                                {
                                    //------------------
                                    //calculate p1 from  prev segment 
                                    //------------------
                                    var prevSeg = segments[i - 1];
                                    //check if prev is curve 
                                    switch (prevSeg.Command)
                                    {
                                        case SvgPathCommand.Arc:
                                        case SvgPathCommand.CurveTo:
                                        case SvgPathCommand.SmoothCurveTo:
                                        case SvgPathCommand.QuadraticBezierCurve:
                                        case SvgPathCommand.TSmoothQuadraticBezierCurveTo:

                                            //make mirror point

                                            p2 = SvgCurveHelper.CreateMirrorPoint(p3, lastPoint);
                                            p3 = c2;
                                            gpath.AddBezierCurve(lastPoint, p2, p3, p);
                                            break;
                                        default:

                                            continue;
                                    }
                                }

                                lastPoint = p;
                            }
                            break;
                        case SvgPathCommand.TSmoothQuadraticBezierCurveTo:
                            {
                                //curve 3
                                var smtC3 = (SvgPathSegCurveToQuadraticSmooth)seg;
                                PointF p;
                                smtC3.GetAbsolutePoints(ref lastPoint, out p);
                                if (i > 0)
                                {
                                    //------------------
                                    //calculate p1 from  prev segment 
                                    //------------------
                                    var prevSeg = segments[i - 1];
                                    //check if prev is curve 
                                    switch (prevSeg.Command)
                                    {
                                        case SvgPathCommand.Arc:
                                        case SvgPathCommand.CurveTo:
                                        case SvgPathCommand.SmoothCurveTo:
                                            {
                                                PointF c = SvgCurveHelper.CreateMirrorPoint(p3, lastPoint);
                                                SvgCurveHelper.Curve3GetControlPoints(lastPoint, c, p, out p2, out p3);
                                                gpath.AddBezierCurve(lastPoint, p2, p3, p);
                                                lastPoint = p;
                                            }
                                            break;
                                        case SvgPathCommand.TSmoothQuadraticBezierCurveTo:
                                            {
                                                //make mirror point
                                                PointF c = SvgCurveHelper.CreateMirrorPoint(intm_c3_c, lastPoint);
                                                SvgCurveHelper.Curve3GetControlPoints(lastPoint, c, p, out p2, out p3);
                                                gpath.AddBezierCurve(lastPoint, p2, p3, p);
                                                lastPoint = p;
                                                intm_c3_c = c;
                                            }
                                            break;
                                        case SvgPathCommand.QuadraticBezierCurve:
                                            {
                                                PointF c = SvgCurveHelper.CreateMirrorPoint(intm_c3_c, lastPoint);
                                                SvgCurveHelper.Curve3GetControlPoints(lastPoint, c, p, out p2, out p3);
                                                gpath.AddBezierCurve(lastPoint, p2, p3, p);
                                                lastPoint = p;
                                                intm_c3_c = c;
                                            }
                                            break;
                                        default:

                                            continue;
                                    }
                                }
                                lastPoint = p;
                            }
                            break;
                        case SvgPathCommand.Arc:
                            {
                                var arcTo = (SvgPathSegArc)seg;
                                PointF p;
                                arcTo.GetAbsolutePoints(ref lastPoint, out p);
                                if (lastPoint.IsEq(p))
                                {
                                    return;
                                }
                                if (arcTo.R1 == 0 && arcTo.R2 == 0)
                                {
                                    gpath.AddLine(lastPoint, p);
                                    lastPoint = p;
                                    return;
                                }
                                PointF[] bz4Points;
                                SvgCurveHelper.MakeBezierCurveFromArc(
                                    ref lastPoint,
                                    ref p,
                                    arcTo.R1,
                                    arcTo.R2,
                                    arcTo.Angle,
                                    arcTo.LargeArgFlag,
                                    arcTo.SweepFlag,
                                    out bz4Points);
                                int j = bz4Points.Length;
                                int nn = 0;
                                while (nn < j)
                                {
                                    gpath.AddBezierCurve(
                                        bz4Points[nn],
                                        bz4Points[nn + 1],
                                        bz4Points[nn + 2],
                                        bz4Points[nn + 3]);
                                    nn += 4;//step 4 points
                                }
                                //set control points
                                p3 = bz4Points[nn - 2];
                                p2 = bz4Points[nn - 3];
                                lastPoint = p;
                                //--------------------------------------------- 

                            }
                            break;
                        default:
                            throw new NotSupportedException();
                    }
                }
            }

            ValidatePath();
        }
        public override void Paint(PaintVisitor p)
        {
            if (fillColor.A > 0)
            {
                p.FillPath(this.myCachedPath, this.fillColor);
            }
            if (this.strokeColor.A > 0)
            {
                p.DrawPath(this.myCachedPath, this.strokeColor, this.ActualStrokeWidth);
            }
        }
    }
}