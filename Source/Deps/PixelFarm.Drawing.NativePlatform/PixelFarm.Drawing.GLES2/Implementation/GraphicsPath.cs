using System;
using System.Collections.Generic;

namespace PixelFarm.Drawing.GLES2
{

    enum PathCommandKind
    {
        Arc,
        Bezier,
        Ellipse,
        Line,
        Rect,
        StartFigure,
        CloseFigure
    }
    abstract class PathCommand
    {

        public abstract PathCommandKind Kind { get; }
    }
    class ArcCommand : PathCommand
    {
        public RectangleF rectF;
        public float startAngle;
        public float sweepAngle;
        public override PathCommandKind Kind
        {
            get
            {
                return PathCommandKind.Arc;
            }
        }
    }
    class BezierCurveCommand : PathCommand
    {
        public PointF p1, p2, p3, p4;
        public override PathCommandKind Kind
        {
            get
            {
                return PathCommandKind.Bezier;
            }
        }
    }
    class EllipseCommand : PathCommand
    {
        public float x, y, w, h;
        public override PathCommandKind Kind
        {
            get
            {
                return PathCommandKind.Ellipse;
            }
        }
    }
    class LineCommand : PathCommand
    {
        public float x1, y1, x2, y2;
        public override PathCommandKind Kind
        {
            get
            {
                return PathCommandKind.Line;
            }
        }
    }
    class RectCommand : PathCommand
    {
        public float x, y, w, h;
        public override PathCommandKind Kind
        {
            get
            {
                return PathCommandKind.Rect;
            }
        }
    }
    class StartFigureCommand : PathCommand
    {
        public override PathCommandKind Kind
        {
            get
            {
                return PathCommandKind.StartFigure;
            }
        }
    }
    class CloseFigureCommand : PathCommand
    {
        public override PathCommandKind Kind
        {
            get
            {
                return PathCommandKind.CloseFigure;
            }
        }
    }

    class GLES2GraphicsPath : GraphicsPath
    {
        List<PathCommand> _cmds = new List<PathCommand>();
        public override object InnerPath
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        public override void AddArc(RectangleF rectF, float startAngle, float sweepAngle)
        {
            var cmd = new ArcCommand();
            cmd.rectF = rectF;
            cmd.startAngle = startAngle;
            cmd.sweepAngle = sweepAngle;
            _cmds.Add(cmd);
        }
        public override void AddArc(float x, float y, float width, float height, float startAngle, float sweepAngle)
        {
            var cmd = new ArcCommand();
            cmd.rectF = new RectangleF(x, y, width, height);
            cmd.startAngle = startAngle;
            cmd.sweepAngle = sweepAngle;
            _cmds.Add(cmd);
        }
        public override void AddBezierCurve(PointF p1, PointF p2, PointF p3, PointF p4)
        {
            var cmd = new BezierCurveCommand();
            cmd.p1 = p1;
            cmd.p2 = p2;
            cmd.p3 = p3;
            cmd.p4 = p4;
            _cmds.Add(cmd);
        }
        public override void AddEllipse(float x, float y, float w, float h)
        {
            var cmd = new EllipseCommand();
            cmd.x = x;
            cmd.y = y;
            cmd.w = w;
            cmd.h = h;
            _cmds.Add(cmd);
        }
        public override void AddLine(PointF p1, PointF p2)
        {
            var cmd = new LineCommand();
            cmd.x1 = p1.X;
            cmd.y1 = p1.Y;
            cmd.x2 = p2.X;
            cmd.y2 = p2.Y;
            _cmds.Add(cmd);
        }
        public override void AddLine(float x1, float y1, float x2, float y2)
        {
            var cmd = new LineCommand();
            cmd.x1 = x1;
            cmd.y1 = y1;
            cmd.x2 = x2;
            cmd.y2 = y2;
            _cmds.Add(cmd);
        }

        public override void AddRectangle(RectangleF r)
        {
            var cmd = new RectCommand();
            cmd.x = r.X;
            cmd.y = r.Y;
            cmd.w = r.Width;
            cmd.h = r.Height;
            _cmds.Add(cmd);
        }

        public override void CloseFigure()
        {
            _cmds.Add(new CloseFigureCommand());
        } 
        public override void Dispose()
        {

        } 
        public override void StartFigure()
        {
            _cmds.Add(new StartFigureCommand());
        }
    }
}