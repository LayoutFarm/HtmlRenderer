//Apache2 2014,WinterDev

using System;
using LayoutFarm.Drawing;

namespace Svg.Pathing
{
    public enum PathCommand : byte
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
        public abstract PathCommand Command { get; }
        public bool IsRelative
        {
            get;
            set;
        }
    }
        
    public class MoveTo : SvgPathSeg
    {
        public MoveTo()
        {

        }
        public override PathCommand Command
        {
            get { return PathCommand.MoveTo; }
        }
        public float X { get; set; }
        public float Y { get; set; }
    }
    public class LineTo : SvgPathSeg
    {
        public LineTo()
        {
        }
        public override PathCommand Command
        {
            get { return PathCommand.LineTo; }
        }
        public float X { get; set; }
        public float Y { get; set; }
    }
    public class LineToHorizontal : SvgPathSeg
    {
        public LineToHorizontal()
        {
        }
        public override PathCommand Command
        {
            get { return PathCommand.HorizontalLineTo; }
        }
        public float X { get; set; }

    }
    public class LineToVertical : SvgPathSeg
    {
        public LineToVertical()
        {
        }
        public override PathCommand Command
        {
            get { return PathCommand.VerticalLineTo; }
        }
        public float Y { get; set; }
    }


    public class CurveToCubic : SvgPathSeg
    {
        public override PathCommand Command
        {
            get { return PathCommand.CurveTo; }
        }
        public float X { get; set; }
        public float Y { get; set; }
        public float X1 { get; set; }
        public float Y1 { get; set; }
        public float X2 { get; set; }
        public float Y2 { get; set; }
    }
    public class CurveToCubicSmooth : SvgPathSeg
    {
        public override PathCommand Command
        {
            get { return PathCommand.SmoothCurveTo; }
        }
        public float X { get; set; }
        public float Y { get; set; }
        public float X2 { get; set; }
        public float Y2 { get; set; }
    }

    public class CurveToQuadratic : SvgPathSeg
    {
        public override PathCommand Command
        {
            get { return PathCommand.QuadraticBezierCurve; }
        }
        public float X { get; set; }
        public float Y { get; set; }
        public float X1 { get; set; }
        public float Y1 { get; set; }
    }
    public class CurveToQuadraticSmooth : SvgPathSeg
    {
        public override PathCommand Command
        {
            get { return PathCommand.TSmoothQuadraticBezierCurveTo; }
        }
        public float X { get; set; }
        public float Y { get; set; }
    }

    public class Arc : SvgPathSeg
    {
        public override PathCommand Command
        {
            get { return PathCommand.Arc; }
        }
        public float X { get; set; }
        public float Y { get; set; }
        public float R1 { get; set; }
        public float R2 { get; set; }
        public float Angle { get; set; }
        public bool LargeArgFlag { get; set; }
        public bool SweepFlag { get; set; }
    }

    public class ClosePath : SvgPathSeg
    {
        public ClosePath()
        {
        }
        public override PathCommand Command
        {
            get { return PathCommand.ZClosePath; }
        }
    }


}
