using System.Drawing;

namespace LayoutFarm.Drawing
{
    public class LinearGradientBrush : Brush
    {
        Point startPoint;
        Point stopPoint;
        Color startColor;
        Color stopColor;
        float gradientAngle;

       
        System.Drawing.Drawing2D.LinearGradientBrush linearGrBrush;

        public LinearGradientBrush(Point startPoint, Point stopPoint, Color startColor, Color stopColor)
        {
           
            this.startPoint = startPoint;
            this.stopPoint = stopPoint;
            this.startColor = startColor;
            this.stopColor = stopColor;
            linearGrBrush =new System.Drawing.Drawing2D.LinearGradientBrush(
                      new System.Drawing.Point(startPoint.X, startPoint.Y),
                      new System.Drawing.Point(stopPoint.X, stopPoint.Y),
                      startColor.ToDrawingColor(),
                      stopColor.ToDrawingColor());

        }
        public LinearGradientBrush(Rectangle  rect, Color startColor, Color stopColor, float angle)
        {
             
            this.startPoint = new Point(rect.X, rect.Y);
            this.stopPoint = new Point(rect.Right, rect.Bottom);
            this.startColor = startColor;
            this.stopColor = stopColor;
            this.gradientAngle = angle;

            linearGrBrush =new System.Drawing.Drawing2D.LinearGradientBrush(
                  System.Drawing.Rectangle.FromLTRB(
                   this.startPoint.X, this.startPoint.Y,
                   this.stopPoint.X, this.stopPoint.Y),
                   this.startColor.ToDrawingColor(),
                   this.stopColor.ToDrawingColor(),
                   this.gradientAngle);
        }
        public LinearGradientBrush(RectangleF rect, Color startColor, Color stopColor, float angle)
        {
            
            this.startPoint = new Point((int)rect.X, (int)rect.Y);
            this.stopPoint = new Point((int)rect.Right, (int)rect.Bottom);
            this.startColor = startColor;
            this.stopColor = stopColor;
            this.gradientAngle = angle;

            linearGrBrush = new System.Drawing.Drawing2D.LinearGradientBrush(
                  System.Drawing.Rectangle.FromLTRB(
                   this.startPoint.X, this.startPoint.Y,
                   this.stopPoint.X, this.stopPoint.Y),
                   this.startColor.ToDrawingColor(),
                   this.stopColor.ToDrawingColor(),
                   this.gradientAngle);
        }

        public override object InnerBrush
        {
            get { return this.linearGrBrush; }
        }
        public override void Dispose()
        {
            if (linearGrBrush != null)
            {
                linearGrBrush.Dispose();
                linearGrBrush = null;
            }
        }
    }
}