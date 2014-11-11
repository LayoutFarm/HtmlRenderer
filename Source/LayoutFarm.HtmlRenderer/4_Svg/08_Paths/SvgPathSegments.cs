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
        public float X { get; private set;  }
        public float Y { get; private set; }

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

        public static PointF MakeMirrorPoint(PointF p1, PointF p2)
        {
            //make new mirror point both x and y
            float xdiff = p2.X - p1.X;
            float ydiff = p2.Y - p1.Y;
            return new PointF(p1.X - xdiff, p1.Y - ydiff);
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
        public float X { get; private set; }
        public float Y { get; private set; }
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
