// 2015,2014 ,Apache2, WinterDev

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using PixelFarm.DrawingGL;
namespace PixelFarm.Drawing.DrawingGL
{
    class LazyGdiBitmapBufferProvider : LazyBitmapBufferProvider
    {
        System.Drawing.Bitmap bitmap;
        System.Drawing.Imaging.BitmapData bmpdata;
        int bmpW;
        int bmpH;
        public LazyGdiBitmapBufferProvider(System.Drawing.Bitmap bitmap)
        {
            this.bitmap = bitmap;
            this.bmpW = bitmap.Width;
            this.bmpH = bitmap.Height;
        }
        public override bool IsInvert
        {
            get { return true; }
        }
        public override int Width
        {
            get { return this.bmpW; }
        }
        public override int Height
        {
            get { return this.bmpH; }
        }
        public override IntPtr GetRawBufferHead()
        {
            this.bmpdata = this.bitmap.LockBits(
                new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb); //read data as 32 bppArgb
            return bmpdata.Scan0;
        }
        public override void ReleaseBufferHead()
        {
            if (this.bmpdata != null)
            {
                this.bitmap.UnlockBits(this.bmpdata);
                this.bmpdata = null;
            }
        }
    }
    public static class GLBitmapTextureHelper
    {
        public static GLBitmap CreateBitmapTexture(int width, int height, System.Drawing.Bitmap bitmap)
        {
            return new GLBitmap(new LazyGdiBitmapBufferProvider(bitmap));
        }
        public static GLBitmap CreateBitmapTexture(PixelFarm.Agg.ActualImage image)
        {
            return new GLBitmap(new LazyAggBitmapBufferProvider(image));
        }
        public static GLBitmap CreateBitmapTexture(System.Drawing.Bitmap bitmap)
        {
            return new GLBitmap(new LazyGdiBitmapBufferProvider(bitmap));
        }
    }
}