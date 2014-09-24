
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
        public static SolidBrush White = CurrentGraphicPlatform.CreateSolidBrush(Color.White);
        public static SolidBrush Black = CurrentGraphicPlatform.CreateSolidBrush(Color.Black);
        public static SolidBrush Transparent = CurrentGraphicPlatform.CreateSolidBrush(Color.Transparent);
        public static SolidBrush Red = CurrentGraphicPlatform.CreateSolidBrush(Color.Red);
        public static SolidBrush LightGray = CurrentGraphicPlatform.CreateSolidBrush(Color.LightGray);
    }

    public static class Pens
    {
        public static readonly Pen Blue = CurrentGraphicPlatform.CreatePen(Color.Blue);
        public static readonly Pen LightGray = CurrentGraphicPlatform.CreatePen(Color.LightGray);
        public static readonly Pen Gray = CurrentGraphicPlatform.CreatePen(Color.Gray);
        public static readonly Pen DeepPink = CurrentGraphicPlatform.CreatePen(Color.DeepPink);
        public static readonly Pen OrangeRed = CurrentGraphicPlatform.CreatePen(Color.OrangeRed);
        public static readonly Pen Green = CurrentGraphicPlatform.CreatePen(Color.Green);
    }
    public abstract class LinearGradientBrush : Brush
    {
    }
}