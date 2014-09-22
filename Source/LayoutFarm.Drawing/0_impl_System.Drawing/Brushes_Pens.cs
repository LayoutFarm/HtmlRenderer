using System.Drawing;
namespace LayoutFarm.Drawing
{
    public abstract class Brush : System.IDisposable
    {
        public abstract void Dispose();
        public abstract object InnerBrush { get; }
    }
    public static class Brushes
    {   
        public static SolidBrush White = new SolidBrush(Color.White);
        public static SolidBrush Black = new SolidBrush(Color.Black);
        public static SolidBrush Transparent = new SolidBrush(Color.Transparent);
        public static SolidBrush Red = new SolidBrush(Color.Red);
        public static SolidBrush LightGray = new SolidBrush(Color.LightGray);
    }
    public class SolidBrush : Brush
    {
        Color color;
        System.Drawing.SolidBrush brush;
        public SolidBrush(Color color)
        {
            this.color = color;
            brush = new System.Drawing.SolidBrush(color.ToDrawingColor());
        }
        public override void Dispose()
        {
            if (this.brush != null)
            {
                this.brush.Dispose();
                this.brush = null;
            }
        }
        public Color Color
        {
            get { return this.color; }
            set
            {
                this.color = value;
                this.brush.Color = value.ToDrawingColor();
            }
        }
        public override object InnerBrush
        {
            get { return this.brush; }
        }
    }
    public class TextureBrush : Brush
    {
        System.Drawing.TextureBrush brush;
        public TextureBrush(Image img)
        {
            brush = new System.Drawing.TextureBrush((System.Drawing.Image)img.InnerImage);
        }
        public TextureBrush(Image img, Rectangle src)
        {
            brush = new System.Drawing.TextureBrush((System.Drawing.Image)img.InnerImage,
               new System.Drawing.Rectangle(src.X, src.Y, src.Width, src.Height));
        }
        public void TranslateTransform(float dx, float dy)
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
    public class Pen : System.IDisposable
    {
        System.Drawing.Pen pen;
        public Pen(Brush brush)
        {
            pen = new System.Drawing.Pen((System.Drawing.Brush)brush.InnerBrush);

        }
        public Pen(Color color)
        {
            pen = new System.Drawing.Pen(new System.Drawing.SolidBrush(color.ToDrawingColor()));
        }
        public void Dispose()
        {
            if (this.pen != null)
            {
                this.pen.Dispose();
                this.pen = null;
            }
        }
        public float[] DashPattern
        {
            get { return this.pen.DashPattern; }
            set { this.pen.DashPattern = value; }
        }

        public float Width
        {
            get { return this.pen.Width; }
            set { this.pen.Width = value; }
        }
        public DashStyle DashStyle
        {
            get { return (DashStyle)this.pen.DashStyle; }
            set { this.pen.DashStyle = (System.Drawing.Drawing2D.DashStyle)value; }
        }
        public object InnerPen
        {
            get { return this.pen; }
        }
    }

    public static class Pens
    {
        public static readonly Pen Blue = new Pen(Color.ColorFromDrawingColor(System.Drawing.Color.Blue));
        public static readonly Pen LightGray = new Pen(Color.ColorFromDrawingColor(System.Drawing.Color.LightGray));
        public static readonly Pen Gray = new Pen(Color.ColorFromDrawingColor(System.Drawing.Color.Gray));
        public static readonly Pen DeepPink = new Pen(Color.ColorFromDrawingColor(System.Drawing.Color.DeepPink));
        public static readonly Pen OrangeRed = new Pen(Color.ColorFromDrawingColor(System.Drawing.Color.OrangeRed));
        public static readonly Pen Green = new Pen(Color.ColorFromDrawingColor(System.Drawing.Color.Green));


    }
}