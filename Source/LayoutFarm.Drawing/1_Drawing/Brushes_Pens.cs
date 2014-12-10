
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
 

     
    public abstract class LinearGradientBrush : Brush
    {
    }
}