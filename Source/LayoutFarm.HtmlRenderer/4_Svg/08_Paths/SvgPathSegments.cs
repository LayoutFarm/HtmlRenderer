//Apache2, 2014-2017, WinterDev
//some code frmo vvv-svg project


using System;
using PixelFarm.Drawing;
namespace LayoutFarm.Svg.Pathing
{
    public enum SvgPathCommand : byte
    {
        MoveTo,
        LineTo,
        HorizontalLineTo,
        VerticalLineTo,
        CurveTo,
        SmoothCurveTo,
        QuadraticBezierCurve,
        TSmoothQuadraticBezierCurveTo,
        Arc,
        ZClosePath
    }

    public abstract class SvgPathSeg
    {
        public SvgPathSeg()
        {
        }
        public abstract SvgPathCommand Command { get; }
        public bool IsRelative
        {
            get;
            set;
        }
    }

    public class SvgPathSegMoveTo : SvgPathSeg
    {
        public SvgPathSegMoveTo(float x, float y)
        {
            this.X = x;
            this.Y = y;
        }
        public override SvgPathCommand Command
        {
            get { return SvgPathCommand.MoveTo; }
        }
        public float X { get; private set; }
        public float Y { get; private set; }

        public void GetAbsolutePoints(ref PointF last, out PointF p)
        {
            if (this.IsRelative)
            {
                p = new PointF(this.X + last.X, this.Y + last.Y);
            }
            else
            {
                p = new PointF(this.X, this.Y);
            }
        }

#if DEBUG
        public override string ToString()
        {
            char cmd = IsRelative ? 'm' : 'M';
            return cmd + X.ToString() + " " + Y;
        }
#endif

    }
    public class SvgPathSegLineTo : SvgPathSeg
    {
        public SvgPathSegLineTo(float x, float y)
        {
            this.X = x;
            this.Y = y;
        }
        public override SvgPathCommand Command
        {
            get { return SvgPathCommand.LineTo; }
        }
        public float X { get; private set; }
        public float Y { get; private set; }
        public void GetAbsolutePoints(ref PointF last, out PointF p)
        {
            if (this.IsRelative)
            {
                p = new PointF(this.X + last.X, this.Y + last.Y);
            }
            else
            {
                p = new PointF(this.X, this.Y);
            }
        }
#if DEBUG
        public override string ToString()
        {
            char cmd = IsRelative ? 'l' : 'L';
            return cmd + X.ToString() + " " + Y;
        }
#endif

    }
    public class SvgPathSegLineToHorizontal : SvgPathSeg
    {
        public SvgPathSegLineToHorizontal(float x)
        {
            this.X = x;
        }
        public override SvgPathCommand Command
        {
            get { return SvgPathCommand.HorizontalLineTo; }
        }
        public float X { get; private set; }
        public void GetAbsolutePoints(ref PointF last, out PointF p)
        {
            if (this.IsRelative)
            {
                p = new PointF(this.X + last.X, last.Y);
            }
            else
            {
                p = new PointF(this.X, last.Y);
            }
        }
#if DEBUG
        public override string ToString()
        {
            char cmd = IsRelative ? 'h' : 'H';
            return cmd + X.ToString();
        }
#endif

    }
    public class SvgPathSegLineToVertical : SvgPathSeg
    {
        public SvgPathSegLineToVertical(float y)
        {
            this.Y = y;
        }

        public override SvgPathCommand Command
        {
            get { return SvgPathCommand.VerticalLineTo; }
        }
        public float Y { get; private set; }
        public void GetAbsolutePoints(ref PointF last, out PointF p)
        {
            if (this.IsRelative)
            {
                p = new PointF(last.X, this.Y + last.Y);
            }
            else
            {
                p = new PointF(last.X, last.Y);
            }
        }
#if DEBUG
        public override string ToString()
        {
            char cmd = IsRelative ? 'v' : 'V';
            return cmd + Y.ToString();
        }
#endif
    }


    public class SvgPathSegCurveToCubic : SvgPathSeg
    {
        public override SvgPathCommand Command
        {
            get { return SvgPathCommand.CurveTo; }
        }
        public SvgPathSegCurveToCubic(float x1, float y1,
            float x2, float y2, float x, float y)
        {
            this.X = x;
            this.Y = y;
            this.X1 = x1;
            this.Y1 = y1;
            this.X2 = x2;
            this.Y2 = y2;
        }
        public float X { get; private set; }
        public float Y { get; private set; }
        public float X1 { get; private set; }
        public float Y1 { get; private set; }
        public float X2 { get; private set; }
        public float Y2 { get; private set; }


        public void GetAbsolutePoints(ref PointF last, out PointF c1, out PointF c2, out PointF p)
        {
            if (this.IsRelative)
            {
                p = new PointF(this.X + last.X, this.Y + last.Y);
                c1 = new PointF(this.X1 + last.X, this.Y1 + last.Y);
                c2 = new PointF(this.X2 + last.X, this.Y2 + last.Y);
            }
            else
            {
                p = new PointF(this.X, this.Y);
                c1 = new PointF(this.X1, this.Y1);
                c2 = new PointF(this.X2, this.Y2);
            }
        }
#if DEBUG
        public override string ToString()
        {
            char cmd = IsRelative ? 'c' : 'C';
            return cmd + this.X.ToString() + " " + this.Y + " " +
                this.X1 + " " + this.Y1 + " "
                + this.X2 + " " + this.Y2;
        }
#endif
    }

    public static class SvgCurveHelper
    {
        public static void Curve3GetControlPoints(PointF start, PointF controlPoint, PointF endPoint, out PointF control1, out PointF control2)
        {
            float x1 = start.X + (controlPoint.X - start.X) * 2 / 3;
            float y1 = start.Y + (controlPoint.Y - start.Y) * 2 / 3;
            float x2 = controlPoint.X + (endPoint.X - controlPoint.X) / 3;
            float y2 = controlPoint.Y + (endPoint.Y - controlPoint.Y) / 3;
            control1 = new PointF(x1, y1);
            control2 = new PointF(x2, y2);
        }

        public static PointF CreateMirrorPoint(PointF mirrorPoint, PointF fixedPoint)
        {
            return new PointF(
                fixedPoint.X - (mirrorPoint.X - fixedPoint.X),
                fixedPoint.Y - (mirrorPoint.Y - fixedPoint.Y));
        }
        internal static void MakeBezierCurveFromArc(ref PointF start,
            ref PointF end,
            float rx,
            float ry,
            float angle,
            SvgArcSize arcSize,
            SvgArcSweep arcSweep,
            out PointF[] bezier4Points)
        {
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
            bezier4Points = new PointF[nsegments * 4];
            int nn = 0;
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
                bezier4Points[nn] = new PointF((float)startX, (float)startY);
                bezier4Points[nn + 1] = new PointF((float)(startX + dx1), (float)(startY + dy1));
                bezier4Points[nn + 2] = new PointF((float)(endpointX + dxe), (float)(endpointY + dye));
                bezier4Points[nn + 3] = new PointF((float)endpointX, (float)endpointY);
                nn += 4;
                theta1 = theta2;
                startX = (float)endpointX;
                startY = (float)endpointY;
            }
        }
    }
    public class SvgPathSegCurveToCubicSmooth : SvgPathSeg
    {
        public override SvgPathCommand Command
        {
            get { return SvgPathCommand.SmoothCurveTo; }
        }
        public SvgPathSegCurveToCubicSmooth(float x2, float y2, float x, float y)
        {
            this.X = x;
            this.Y = y;
            this.X2 = x2;
            this.Y2 = y2;
        }
        public float X { get; private set; }
        public float Y { get; private set; }
        public float X2 { get; private set; }
        public float Y2 { get; private set; }

        public void GetAbsolutePoints(ref PointF last, out PointF c2, out PointF p)
        {
            if (this.IsRelative)
            {
                p = new PointF(this.X + last.X, this.Y + last.Y);
                c2 = new PointF(this.X2 + last.X, this.Y2 + last.Y);
            }
            else
            {
                p = new PointF(this.X, this.Y);
                c2 = new PointF(this.X2, this.Y2);
            }
        }
#if DEBUG
        public override string ToString()
        {
            char cmd = IsRelative ? 's' : 'S';
            return cmd + this.X.ToString() + " " + this.Y + " " +
                this.X2 + " " + this.Y2;
        }
#endif
    }

    public class SvgPathSegCurveToQuadratic : SvgPathSeg
    {
        public override SvgPathCommand Command
        {
            get { return SvgPathCommand.QuadraticBezierCurve; }
        }
        public SvgPathSegCurveToQuadratic(float x1, float y1, float x, float y)
        {
            this.X1 = x1;
            this.Y1 = y1;
            this.X = x;
            this.Y = y;
        }
        public float X { get; private set; }
        public float Y { get; private set; }
        public float X1 { get; private set; }
        public float Y1 { get; private set; }
        public PointF ControlPoint
        {
            get { return new PointF(this.X1, this.Y1); }
        }
        public void GetAbsolutePoints(ref PointF last, out PointF c1, out PointF p)
        {
            if (this.IsRelative)
            {
                p = new PointF(this.X + last.X, this.Y + last.Y);
                c1 = new PointF(this.X1 + last.X, this.Y1 + last.Y);
            }
            else
            {
                p = new PointF(this.X, this.Y);
                c1 = new PointF(this.X1, this.Y1);
            }
        }



#if DEBUG
        public override string ToString()
        {
            char cmd = IsRelative ? 'q' : 'Q';
            return cmd + this.X.ToString() + " " + this.Y + " " +
                this.X1 + " " + this.Y1;
        }
#endif
    }
    public class SvgPathSegCurveToQuadraticSmooth : SvgPathSeg
    {
        public override SvgPathCommand Command
        {
            get { return SvgPathCommand.TSmoothQuadraticBezierCurveTo; }
        }
        public SvgPathSegCurveToQuadraticSmooth(float x, float y)
        {
            this.X = x;
            this.Y = y;
        }
        public float X { get; private set; }
        public float Y { get; private set; }


        public void GetAbsolutePoints(ref PointF last, out PointF p)
        {
            if (this.IsRelative)
            {
                p = new PointF(this.X + last.X, this.Y + last.Y);
            }
            else
            {
                p = new PointF(this.X, this.Y);
            }
        }

#if DEBUG
        public override string ToString()
        {
            char cmd = IsRelative ? 't' : 'T';
            return cmd + this.X.ToString() + " " + this.Y;
        }
#endif
    }

    public class SvgPathSegArc : SvgPathSeg
    {
        public const double RAD_PER_DEG = Math.PI / 180.0;
        public const double DOUBLE_PI = Math.PI * 2;
        public override SvgPathCommand Command
        {
            get { return SvgPathCommand.Arc; }
        }
        public SvgPathSegArc(float r1, float r2, float xAxisRotation,
            int largeArcFlag,
            int sweepFlags, float x, float y)
        {
            this.X = x;
            this.Y = y;
            this.R1 = r1;
            this.R2 = r2;
            this.Angle = xAxisRotation;
            this.SweepFlag = (SvgArcSweep)sweepFlags;
            this.LargeArgFlag = (SvgArcSize)largeArcFlag;
        }

        public float X { get; private set; }
        public float Y { get; private set; }
        public float R1 { get; private set; }
        public float R2 { get; private set; }
        public float Angle { get; private set; }
        public SvgArcSize LargeArgFlag { get; private set; }
        public SvgArcSweep SweepFlag { get; private set; }


        public void GetAbsolutePoints(ref PointF last, out PointF p)
        {
            if (this.IsRelative)
            {
                p = new PointF(this.X + last.X, this.Y + last.Y);
            }
            else
            {
                p = new PointF(this.X, this.Y);
            }
        }
        public static double CalculateVectorAngle(double ux, double uy, double vx, double vy)
        {
            double ta = Math.Atan2(uy, ux);
            double tb = Math.Atan2(vy, vx);
            if (tb >= ta)
            {
                return tb - ta;
            }

            return DOUBLE_PI - (ta - tb);
        }
#if DEBUG
        public override string ToString()
        {
            char cmd = IsRelative ? 'a' : 'A';
            return cmd + this.R1.ToString() + " " + this.R2 + " " +
                this.Angle + " " + this.LargeArgFlag + " " +
                this.SweepFlag.ToString() + " " + this.X + " " + this.Y;
        }
#endif
    }

    public class SvgPathSegClosePath : SvgPathSeg
    {
        public SvgPathSegClosePath()
        {
        }
        public override SvgPathCommand Command
        {
            get { return SvgPathCommand.ZClosePath; }
        }

#if DEBUG
        public override string ToString()
        {
            return "Z";
        }
#endif
    }

    [Flags]
    public enum SvgArcSweep
    {
        Negative = 0,
        Positive = 1
    }

    [Flags]
    public enum SvgArcSize
    {
        Small = 0,
        Large = 1
    }
}
