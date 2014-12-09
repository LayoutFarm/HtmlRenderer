using System;
namespace LayoutFarm.Drawing
{

    class WinGdiPlatform : GraphicsPlatform
    {

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
        public override Bitmap CreateBitmap(int width, int height)
        {
            return new MyBitmap(width, height);
        }
        public override Bitmap CreateNativeBitmapWrapper(object bmp)
        {
            return new MyBitmap(bmp as System.Drawing.Bitmap);
        }

        public override SolidBrush CreateSolidBrush(Color color)
        {
            return new MySolidBrush(color);
        }
        public override Pen CreatePen(Brush b)
        {
            return new MyPen(b);
        }

        public override Pen CreateSolidPen(Color color)
        {
            SolidBrush sb = new MySolidBrush(color);
            return new MyPen(sb);
        }
        public override TextureBrush CreateTextureBrush(Image img)
        {

            return new MyTextureBrush(img);
        }
        public override TextureBrush CreateTextureBrush(Image img, Rectangle rect)
        {
            return new MyTextureBrush(img, rect);
        }
        public override LinearGradientBrush CreateLinearGradientBrush(PointF startPoint, PointF stopPoint, Color startColor, Color stopColor)
        {
            return new MyLinearGradientBrush(startPoint, stopPoint, startColor, stopColor);
        }
        public override LinearGradientBrush CreateLinearGradientBrush(RectangleF rect, Color startColor, Color stopColor, float angle)
        {
            return new MyLinearGradientBrush(rect, startColor, stopColor, angle);
        }
        public override Matrix CreateMatrix()
        {
            return new MyMatrix();
        }
        public override Matrix CreateMatrix(float m11, float m12, float m21, float m22, float dx, float dy)
        {
            return new MyMatrix(m11, m12, m21, m22, dx, dy);
        }
        public override GraphicsPath CreateGraphicPath()
        {
            return new MyGraphicsPath();
        }
        public override Region CreateRegion()
        {
            return new MyRegion();
        }
        public override FontInfo CreateNativeFontWrapper(object nativeFont)
        {
            return LayoutFarm.FontsUtils.GetCachedFont((System.Drawing.Font)nativeFont);

        }
        public override Canvas CreateCanvas(int left, int top, int width, int height)
        {
            return new MyCanvas(this, 0, 0, left, top, width, height);
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
                    sampleIFonts = new MyCanvas(this, 0, 0, 0, 0, 2, 2);
                }
                return this.sampleIFonts;
            }
        }
    }
}