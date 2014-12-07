using System.Drawing;

namespace LayoutFarm.Drawing
{
    class MyBitmap : Bitmap
    {
        int width;
        int height;
        System.Drawing.Bitmap bmp;
        public MyBitmap(int w, int h)
        {
            this.width = w;
            this.height = h;
            bmp = new System.Drawing.Bitmap(w, h);
        }
        public MyBitmap(System.Drawing.Bitmap bmp)
        {
            this.bmp = bmp;
            this.width = bmp.Width;
            this.height = bmp.Height;

        }
        public override object InnerImage
        {
            get { return this.bmp; }
        }
        public override int Width
        {
            get { return this.width; }
        }
        public override int Height
        {
            get { return this.height; }
        }
        public override void Dispose()
        {
            if (this.bmp != null)
            {
                this.bmp.Dispose();
                this.bmp = null;
            }
        }
    }

    class MyFont : Font
    {
        System.Drawing.Font myFont;
        System.IntPtr hFont;
     
        public MyFont(System.Drawing.Font f)
        {
            this.myFont = f;
            this.hFont = f.ToHfont();
        }
        
        public System.IntPtr ToHFont()
        {
            return this.hFont;
        }
        public override string Name
        {
            get { return this.myFont.Name; }
        }
        public override int Height
        {
            get { return this.myFont.Height; }
        }
        public override System.IntPtr ToHfont()
        {
            return this.hFont;
        }
        public override float Size
        {
            get { return this.myFont.Size; }
        }
        public override FontStyle Style
        {
            get
            {
                return (FontStyle)this.myFont.Style;
            }
        }
        public override void Dispose()
        {
            if (myFont != null)
            {
                myFont.Dispose();
                myFont = null;
            }
        }

        public override object InnerFont
        {
            get { return this.myFont; }
        }
    }
    class MyPen : Pen
    {
        System.Drawing.Pen pen;
        public MyPen(Brush brush)
        {
            pen = new System.Drawing.Pen((System.Drawing.Brush)brush.InnerBrush);
        }
        public MyPen(Color color)
        {
            pen = new System.Drawing.Pen(new System.Drawing.SolidBrush(
                System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B)));
        }
        public override void Dispose()
        {
            if (this.pen != null)
            {
                this.pen.Dispose();
                this.pen = null;
            }
        }
        public override float[] DashPattern
        {
            get { return this.pen.DashPattern; }
            set { this.pen.DashPattern = value; }
        }

        public override float Width
        {
            get { return this.pen.Width; }
            set { this.pen.Width = value; }
        }
        public override DashStyle DashStyle
        {
            get { return (DashStyle)this.pen.DashStyle; }
            set { this.pen.DashStyle = (System.Drawing.Drawing2D.DashStyle)value; }
        }
        public override object InnerPen
        {
            get { return this.pen; }
        }
    }
    class MySolidBrush : SolidBrush
    {
        Color color;
        System.Drawing.SolidBrush brush;
        public MySolidBrush(Color color)
        {
            this.color = color;
            brush = new System.Drawing.SolidBrush(
                System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B));
        }
        public override void Dispose()
        {
            if (this.brush != null)
            {
                this.brush.Dispose();
                this.brush = null;
            }
        }
        public override Color Color
        {
            get { return this.color; }
            set
            {
                this.color = value;
                this.brush.Color = System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
            }
        }
        public override object InnerBrush
        {
            get { return this.brush; }
        }
    }

    class MyTextureBrush : TextureBrush
    {
        System.Drawing.TextureBrush brush;
        public MyTextureBrush(Image img)
        {
            brush = new System.Drawing.TextureBrush((System.Drawing.Image)img.InnerImage);
        }
        public MyTextureBrush(Image img, Rectangle src)
        {
            brush = new System.Drawing.TextureBrush((System.Drawing.Image)img.InnerImage,
               new System.Drawing.Rectangle(src.X, src.Y, src.Width, src.Height));
        }
        public override void TranslateTransform(float dx, float dy)
        {
            brush.TranslateTransform(dx, dy);
        }
        public override void Dispose()
        {
            if (brush != null)
            {
                brush.Dispose();
                brush = null;
            }
        }
        public override object InnerBrush
        {
            get { return this.brush; }
        }

    }

    class MyLinearGradientBrush : LinearGradientBrush
    {
        PointF startPoint;
        PointF stopPoint;
        Color startColor;
        Color stopColor;
        float gradientAngle;


        System.Drawing.Drawing2D.LinearGradientBrush linearGrBrush;

        public MyLinearGradientBrush(PointF startPoint, PointF stopPoint, Color startColor, Color stopColor)
        {

            this.startPoint = startPoint;
            this.stopPoint = stopPoint;
            this.startColor = startColor;
            this.stopColor = stopColor;
            linearGrBrush = new System.Drawing.Drawing2D.LinearGradientBrush(
                      new System.Drawing.PointF(startPoint.X, startPoint.Y),
                      new System.Drawing.PointF(stopPoint.X, stopPoint.Y),
                      ToDrawingColor(startColor),
                      ToDrawingColor(stopColor));

        }

        public MyLinearGradientBrush(Rectangle rect, Color startColor, Color stopColor, float angle)
        {

            this.startPoint = new Point(rect.X, rect.Y);
            this.stopPoint = new Point(rect.Right, rect.Bottom);
            this.startColor = startColor;
            this.stopColor = stopColor;
            this.gradientAngle = angle;

            linearGrBrush = new System.Drawing.Drawing2D.LinearGradientBrush(
                   System.Drawing.RectangleF.FromLTRB(
                   this.startPoint.X, this.startPoint.Y,
                   this.stopPoint.X, this.stopPoint.Y),
                   ToDrawingColor(startColor),
                   ToDrawingColor(stopColor),
                   this.gradientAngle);
        }
        public MyLinearGradientBrush(RectangleF rect, Color startColor, Color stopColor, float angle)
        {

            this.startPoint = new Point((int)rect.X, (int)rect.Y);
            this.stopPoint = new Point((int)rect.Right, (int)rect.Bottom);
            this.startColor = startColor;
            this.stopColor = stopColor;
            this.gradientAngle = angle;

            linearGrBrush = new System.Drawing.Drawing2D.LinearGradientBrush(
                  System.Drawing.RectangleF.FromLTRB(
                   this.startPoint.X, this.startPoint.Y,
                   this.stopPoint.X, this.stopPoint.Y),
                   ToDrawingColor(startColor),
                   ToDrawingColor(stopColor),
                   this.gradientAngle);
        }


        static System.Drawing.Color ToDrawingColor(Color c)
        {
            return System.Drawing.Color.FromArgb(c.A, c.R, c.G, c.B);
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