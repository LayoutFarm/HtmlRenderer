//Apache2 2014,WinterDev
//some code frmo vvv-svg project


using System;
using LayoutFarm.Drawing;

namespace Svg.Pathing
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
        public SvgPathSegMoveTo()
        {
        }
        public SvgPathSegMoveTo(float x, float y)
        {
            this.X = x;
            this.Y = y;
        }
        public override SvgPathCommand Command
        {
            get { return SvgPathCommand.MoveTo; }
        }
        public float X { get; set; }
        public float Y { get; set; }


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
        public SvgPathSegLineTo()
        {
        }
        public SvgPathSegLineTo(float x, float y)
        {
            this.X = x;
            this.Y = y;
        }
        public override SvgPathCommand Command
        {
            get { return SvgPathCommand.LineTo; }
        }
        public float X { get; set; }
        public float Y { get; set; }

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
        public SvgPathSegLineToHorizontal()
        {
        }
        public SvgPathSegLineToHorizontal(float x)
        {
            this.X = x;
        }
        public override SvgPathCommand Command
        {
            get { return SvgPathCommand.HorizontalLineTo; }
        }
        public float X { get; set; }

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
        public SvgPathSegLineToVertical()
        {
        }
        public SvgPathSegLineToVertical(float y)
        {
            this.Y = y;
        }

        public override SvgPathCommand Command
        {
            get { return SvgPathCommand.VerticalLineTo; }
        }
        public float Y { get; set; }

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
        public SvgPathSegCurveToCubic(float x, float y, float x1, float y1,
            float x2, float y2)
        {
            this.X = x;
            this.Y = y;
            this.X1 = x1;
            this.Y1 = y1;
            this.X2 = x2;
            this.Y2 = y2;
        }
        public float X { get; set; }
        public float Y { get; set; }
        public float X1 { get; set; }
        public float Y1 { get; set; }
        public float X2 { get; set; }
        public float Y2 { get; set; }

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
    public class SvgPathSegCurveToCubicSmooth : SvgPathSeg
    {
        public override SvgPathCommand Command
        {
            get { return SvgPathCommand.SmoothCurveTo; }
        }
        public SvgPathSegCurveToCubicSmooth(float x, float y, float x2, float y2)
        {
            this.X = x;
            this.Y = y;
            this.X2 = x2;
            this.Y2 = y2;
        }
        public float X { get; set; }
        public float Y { get; set; }
        public float X2 { get; set; }
        public float Y2 { get; set; }

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
        public float X { get; set; }
        public float Y { get; set; }
        public float X1 { get; set; }
        public float Y1 { get; set; }
        public PointF ControlPoint
        {
            get { return new PointF(this.X1, this.Y1); }
            set
            {
                this.X1 = value.X;
                this.Y1 = value.Y;
            }
        }
        public static void GetControlPoints(PointF start, PointF controlPoint, PointF endPoint, out PointF control1, out PointF control2)
        {
            float x1 = start.X + (controlPoint.X - start.X) * 2 / 3;
            float y1 = start.Y + (controlPoint.Y - start.Y) * 2 / 3;
            float x2 = controlPoint.X + (endPoint.X - controlPoint.X) / 3;
            float y2 = controlPoint.Y + (endPoint.Y - controlPoint.Y) / 3;

            control1 = new PointF(x1, y2);
            control2 = new PointF(x2, y2);
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
        public float X { get; set; }
        public float Y { get; set; }
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
        public override SvgPathCommand Command
        {
            get { return SvgPathCommand.Arc; }
        }
        public SvgPathSegArc(float r1, float r2, float xAxisRotation, int largeArcFlag, int sweepFlags, float x, float y)
        {
            this.X = x;
            this.Y = y;
            this.R1 = r1;
            this.R2 = r2;
            this.Angle = xAxisRotation;
            this.SweepFlag = sweepFlags;
            this.LargeArgFlag = largeArcFlag;
        }


        public float X { get; set; }
        public float Y { get; set; }
        public float R1 { get; set; }
        public float R2 { get; set; }
        public float Angle { get; set; }
        public int LargeArgFlag { get; set; }
        public int SweepFlag { get; set; }
#if DEBUG
        public override string ToString()
        {
            char cmd = IsRelative ? 'a' : 'A';

            return cmd + this.R1.ToString() + " " + this.R2 + " " +
                this.Angle + " " + this.LargeArgFlag + " " +
                +this.SweepFlag + " " + this.X + " " + this.Y;
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


}
