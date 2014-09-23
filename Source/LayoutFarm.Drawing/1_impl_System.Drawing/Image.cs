using System.Drawing;

namespace LayoutFarm.Drawing
{
    public abstract class Image : System.IDisposable
    {   
        public abstract void Dispose();  
        public abstract int Width { get; }
        public abstract int Height { get; }
        public abstract object InnerImage { get; }
        public Size Size
        {
            get { return new Size(this.Width, this.Height); }
        } 
    }
    public abstract class Bitmap : Image
    {    
    }
}