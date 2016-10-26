namespace PixelFarm.Drawing.GLES2
{
    class GLESBitmap : System.IDisposable
    {
        byte[] buffer;
        public GLESBitmap(int w, int h)
        {
            this.Width = w;
            this.Height = h;
        }
        public void Dispose()
        {

        }
        public int Width
        {
            get;
            set;
        }
        public int Height
        {
            get;
            set;
        }
        public byte[] Buffer
        {
            get { return buffer; }
            set { buffer = value; }
        }
        public bool IsBottomUp
        {
            get; set;
        }
    }
}