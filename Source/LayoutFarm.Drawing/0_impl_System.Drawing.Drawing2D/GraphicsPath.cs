using System.Drawing;

namespace LayoutFarm.Drawing
{
    public class GraphicsPath : System.IDisposable
    {
        System.Drawing.Drawing2D.GraphicsPath p;
        public GraphicsPath()
        {
            p = new System.Drawing.Drawing2D.GraphicsPath();
        }
        public void AddArc(float x, float y, float width, float height, float startAngle, float sweepAngle)
        {
            p.AddArc(x, y, width, height, startAngle, sweepAngle);
        }
        public void AddArc(RectangleF rectF, float startAngle, float sweepAngle)
        {
            System.Drawing.RectangleF rectf = new System.Drawing.RectangleF(
                rectF.X, rectF.Y, rectF.Width, rectF.Height);

            p.AddArc(rectf, startAngle, sweepAngle);

        }
        public void AddLine(float x1, float y1, float x2, float y2)
        {
            p.AddLine(x1, y1, x2, y2);
        }
        public void AddLine(PointF p1, PointF p2)
        {
            p.AddLine(new System.Drawing.PointF(p1.X, p1.Y),
                      new System.Drawing.PointF(p2.X, p2.Y));
        }
        public void CloseFigure()
        {
            p.CloseFigure();
        }
        public void Dispose()
        {
            if (p != null)
            {
                p.Dispose();
                p = null;
            }
        }
        public void StartFigure()
        {
            p.StartFigure();
        }
        public void AddEllipse(float x, float y, float w, float h)
        {
            p.AddEllipse(x, y, w, h);
        }
        public void AddRectangle(RectangleF r)
        {
            p.AddRectangle(
                new System.Drawing.RectangleF(r.X, r.Y, r.Width, r.Height));
        }
        public object InnerPath { get { return this.p; } }
    }
}