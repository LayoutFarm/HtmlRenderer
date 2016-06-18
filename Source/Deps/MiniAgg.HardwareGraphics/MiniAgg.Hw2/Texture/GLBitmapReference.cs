//MIT 2014, WinterDev

namespace PixelFarm.DrawingGL
{
    public class GLBitmapReference
    {
        GLBitmap ownerBmp;
        public GLBitmapReference(GLBitmap ownerBmp, int x, int y, int w, int h)
        {
            this.Left = x;
            this.Top = y;
            this.Width = w;
            this.Height = h;
            this.ownerBmp = ownerBmp;
        }
        public int Left { get; set; }
        public int Top { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public GLBitmap OwnerBitmap
        {
            get { return this.ownerBmp; }
        }
        public PixelFarm.Drawing.RectangleF GetRectF()
        {
            return new Drawing.RectangleF(this.Left, this.Top, this.Width, this.Height);
        }
    }
}