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
    public class Bitmap : Image
    {   
        int width;
        int height;
        System.Drawing.Bitmap bmp;
        public Bitmap(int w, int h)
        {
            this.width = w;
            this.height = h;
            bmp = new System.Drawing.Bitmap(w, h);
        }
        public Bitmap(object bmp)
        {
            this.bmp = (System.Drawing.Bitmap)bmp;
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
}