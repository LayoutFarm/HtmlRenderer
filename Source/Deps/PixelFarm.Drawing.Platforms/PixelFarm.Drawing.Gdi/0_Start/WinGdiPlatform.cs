//2014,2015 BSD, WinterDev


namespace PixelFarm.Drawing.WinGdi
{
    class WinGdiPlatform : GraphicsPlatform
    {
        static FontStore fontStore = new FontStore();
        System.Drawing.Bitmap sampleBmp;
        IFonts sampleIFonts;
        public WinGdiPlatform()
        {
        }

        ~WinGdiPlatform()
        {
            if (sampleBmp != null)
            {
                sampleBmp.Dispose();
                sampleBmp = null;
            }
            if (sampleIFonts != null)
            {
                sampleIFonts.Dispose();
                sampleIFonts = null;
            }
        }

        public override GraphicsPath CreateGraphicsPath()
        {
            return new MyGraphicsPath();
        }
        public override FontInfo GetFont(string fontfaceName, float emsize, FontStyle fontStyle)
        {
            //System.Drawing.Font nativeFont = new System.Drawing.Font(fontfaceName, emsize,fonts);
            return fontStore.GetCachedFont(fontfaceName, emsize, (System.Drawing.FontStyle)fontStyle);
        }

        public override Canvas CreateCanvas(int left, int top, int width, int height)
        {
            return new MyScreenCanvas(this, 0, 0, left, top, width, height);
        }
        public override Canvas CreateCanvas(object platformCanvas, int left, int top, int width, int height)
        {
            return new MyGdiPlusCanvas(this,
                platformCanvas as System.Drawing.Graphics,
                left,
                top,
                width,
                height);
        }
        public override IFonts SampleIFonts
        {
            get
            {
                if (sampleIFonts == null)
                {
                    if (sampleBmp == null)
                    {
                        sampleBmp = new System.Drawing.Bitmap(2, 2);
                    }

                    System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(sampleBmp);
                    sampleIFonts = new MyScreenCanvas(this, 0, 0, 0, 0, 2, 2);
                }
                return this.sampleIFonts;
            }
        }
        public override Bitmap CreatePlatformBitmap(int w, int h, byte[] rawBuffer, bool isBottomUp)
        {
            //create platform bitmap
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(w, h,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            CopyFromAggActualImageToGdiPlusBitmap(rawBuffer, bmp);
            if (isBottomUp)
            {
                bmp.RotateFlip(System.Drawing.RotateFlipType.RotateNoneFlipY);
            }
            return new Bitmap(w, h, bmp);
        }
        static void CopyFromAggActualImageToGdiPlusBitmap(byte[] rawBuffer, System.Drawing.Bitmap bitmap)
        {
            //platform specific
            var bmpdata = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
                 System.Drawing.Imaging.ImageLockMode.ReadOnly,
                 System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            System.Runtime.InteropServices.Marshal.Copy(rawBuffer, 0,
                bmpdata.Scan0, rawBuffer.Length);
            bitmap.UnlockBits(bmpdata);
        }
    }
}