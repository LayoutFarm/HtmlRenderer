using System.Drawing;
namespace LayoutFarm.Drawing
{
    public abstract class Brush : System.IDisposable
    {
        public abstract void Dispose();
        public abstract object InnerBrush { get; }
    }  
    public abstract class SolidBrush : Brush
    {   
        public abstract Color Color { get; set; }
         
    }
    public abstract class TextureBrush : Brush
    {   
        public abstract void TranslateTransform(float dx, float dy); 
    }
    public abstract class Pen : System.IDisposable
    {

        public abstract void Dispose();
        public abstract float[] DashPattern { get; set; }

        public abstract float Width { get; set; }
        public abstract DashStyle DashStyle { get; set; }
        public abstract object InnerPen { get; }
    }



    public static class Brushes
    {
        public static SolidBrush White = GraphicPlatform.CreateSolidBrushFromColor(Color.White);
        public static SolidBrush Black =GraphicPlatform.CreateSolidBrushFromColor(Color.Black);
        public static SolidBrush Transparent = GraphicPlatform.CreateSolidBrushFromColor(Color.Transparent);
        public static SolidBrush Red = GraphicPlatform.CreateSolidBrushFromColor(Color.Red);
        public static SolidBrush LightGray = GraphicPlatform.CreateSolidBrushFromColor(Color.LightGray);
    }

    public static class Pens
    {
        public static readonly Pen Blue = GraphicPlatform.CreatePen(Color.Blue);
        public static readonly Pen LightGray = GraphicPlatform.CreatePen(Color.LightGray);
        public static readonly Pen Gray = GraphicPlatform.CreatePen(Color.Gray);
        public static readonly Pen DeepPink = GraphicPlatform.CreatePen(Color.DeepPink);
        public static readonly Pen OrangeRed = GraphicPlatform.CreatePen(Color.OrangeRed);
        public static readonly Pen Green = GraphicPlatform.CreatePen(Color.Green);
    }
    public abstract class LinearGradientBrush : Brush
    {
    }
}