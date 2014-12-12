namespace LayoutFarm.Drawing
{
    public abstract class Image : System.IDisposable
    {
        public abstract void Dispose();
        public abstract int Width { get; }
        public abstract int Height { get; }
        public abstract object InnerImage { get; set; }
        public Size Size
        {
            get { return new Size(this.Width, this.Height); }
        }
    }
    public sealed class Bitmap : Image
    {
        int width;
        int height;
        object innerImage;
        public Bitmap(int w, int h)
        {
            this.width = w;
            this.height = h;
        }
        public Bitmap(int w, int h, object innerImage)
        {
            this.width = w;
            this.height = h;
            this.innerImage = innerImage;
        }
        public override int Width
        {
            get { return this.width; }
        }
        public override int Height
        {
            get { return this.height; }
        }
        public override object InnerImage
        {
            get { return this.innerImage; }
            set { this.innerImage = value; }
        }
        public override void Dispose()
        {

        }
    }

}