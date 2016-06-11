// 2015,2014 ,BSD, WinterDev 

using System.Drawing;
namespace PixelFarm.Drawing.WinGdi
{
    class MyGraphicsPath : GraphicsPath
    {
        System.Drawing.Drawing2D.GraphicsPath p;
        public MyGraphicsPath()
        {
            p = new System.Drawing.Drawing2D.GraphicsPath();
        }
        public override void AddArc(float x, float y, float width, float height, float startAngle, float sweepAngle)
        {
            p.AddArc(x, y, width, height, startAngle, sweepAngle);
        }
        public override void AddArc(RectangleF rectF, float startAngle, float sweepAngle)
        {
            System.Drawing.RectangleF rectf = new System.Drawing.RectangleF(
                rectF.X, rectF.Y, rectF.Width, rectF.Height);
            p.AddArc(rectf, startAngle, sweepAngle);
        }
        public override void AddLine(float x1, float y1, float x2, float y2)
        {
            p.AddLine(x1, y1, x2, y2);
        }
        public override void AddLine(PointF p1, PointF p2)
        {
            p.AddLine(new System.Drawing.PointF(p1.X, p1.Y),
                      new System.Drawing.PointF(p2.X, p2.Y));
        }
        public override void CloseFigure()
        {
            p.CloseFigure();
        }
        public override void Dispose()
        {
            if (p != null)
            {
                p.Dispose();
                p = null;
            }
        }
        public override void StartFigure()
        {
            p.StartFigure();
        }
        public override void AddEllipse(float x, float y, float w, float h)
        {
            p.AddEllipse(x, y, w, h);
        }
        public override void AddRectangle(RectangleF r)
        {
            p.AddRectangle(
                new System.Drawing.RectangleF(r.X, r.Y, r.Width, r.Height));
        }
        public override object InnerPath { get { return this.p; } }
        public override void AddBezierCurve(PointF p1, PointF p2, PointF p3, PointF p4)
        {
            p.AddBezier(
                p1.X, p1.Y,
                p2.X, p2.Y,
                p3.X, p3.Y,
                p4.X, p4.Y);
        }
        public override object GetPathData()
        {
            return p.PathData;
        }
        public override object InnerPath2
        {
            get;
            set;
        }
    }
}