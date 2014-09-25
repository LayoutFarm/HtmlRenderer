

namespace LayoutFarm.Drawing.WinGdiPlatform
{
    public static class WinGdi
    {
        static WinGdiGraphicsPlateform platform;
        static bool isInit;
        public static void Start()
        {
            if (isInit)
            {
                return;
            }
            isInit = true;

            platform = new WinGdiGraphicsPlateform();
            CurrentGraphicPlatform.SetCurrentPlatform(platform);
            CurrentGraphicPlatform.GenericSerifFontName = System.Drawing.FontFamily.GenericSerif.Name;
        }
        public static void End()
        {

        }
        public static GraphicPlatform Platform
        {
            get { return platform; }
        }
    }


    class WinGdiGraphicsPlateform : GraphicPlatform
    {
        System.Drawing.Bitmap sampleBmp;
        IGraphics sampleIGraphics;
        public WinGdiGraphicsPlateform()
        {
        }

        ~WinGdiGraphicsPlateform()
        {
            if (sampleBmp != null)
            {
                sampleBmp.Dispose();
                sampleBmp = null;
            }
            if (sampleIGraphics != null)
            {
                sampleIGraphics.Dispose();
                sampleIGraphics = null;
            }
        }
        public override Bitmap CreateBitmap(int width, int height)
        {
            return new MyBitmap(width, height);
        }
        public override Bitmap CreateBitmap(object bmp)
        {
            return new MyBitmap(bmp as System.Drawing.Bitmap);
        }
        public override Font CreateFont(object font)
        {
            return new MyFont(font as System.Drawing.Font);
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
        public override TextFontInfo CreateTexFontInfo(object nativeFont)
        {
            return new TextFontInfo(
                new MyFont((System.Drawing.Font)nativeFont),
                new BasicGdi32FontHelper());
        }
        public override Canvas CreateCanvas(int horizontalPageNum, int verticalPageNum, int left, int top, int width, int height)
        {
            return new MyCanvas(horizontalPageNum, verticalPageNum,
                left, top, width, height);
        }
        public override IGraphics CreateIGraphics(int w, int h)
        {
            System.Drawing.Bitmap bb = new System.Drawing.Bitmap(2, 2);
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bb);
            return new WinGraphics(g, false);
        }
        public override IGraphics SampleIGraphics
        {
            get
            {
                if (sampleIGraphics == null)
                {
                    if (sampleBmp == null)
                    {
                        sampleBmp = new System.Drawing.Bitmap(2, 2);
                    }

                    System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(sampleBmp);
                    sampleIGraphics =new WinGraphics(g, false);
                }
                return this.sampleIGraphics;
            }
        }
        public override IGraphics CreateIGraphics(object nativeObj)
        {
            throw new System.NotImplementedException();
        }
    }
}