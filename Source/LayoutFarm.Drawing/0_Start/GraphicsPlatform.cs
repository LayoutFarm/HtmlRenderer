
namespace LayoutFarm.Drawing
{
    public abstract class GraphicsPlatform
    {

        public abstract Bitmap CreateBitmap(int width, int height);

        public abstract Bitmap CreateNativeBitmapWrapper(object bmp);
        public abstract FontInfo CreateNativeFontWrapper(object nativeFont);

        public abstract SolidBrush CreateSolidBrush(Color color);
        public abstract Pen CreatePen(Brush brush);
        public abstract Pen CreateSolidPen(Color color);


        public abstract TextureBrush CreateTextureBrush(Image img);
        public abstract TextureBrush CreateTextureBrush(Image img, Rectangle rect);

        public abstract LinearGradientBrush CreateLinearGradientBrush(PointF startPoint, PointF stopPoint, Color startColor, Color stopColor);
        public abstract LinearGradientBrush CreateLinearGradientBrush(RectangleF rect, Color startColor, Color stopColor, float angle);
        public abstract Matrix CreateMatrix();
        public abstract Matrix CreateMatrix(float m11, float m12, float m21, float m22, float dx, float dy);

        public abstract GraphicsPath CreateGraphicPath();
        public abstract Region CreateRegion();

        public abstract Canvas CreateCanvas(int horizontalPageNum,
            int verticalPageNum,
            int left,
            int top,
            int width,
            int height);

        
        public abstract IFonts SampleIFonts { get; }
    }

    public static class CurrentGraphicsPlatform
    {
        static bool isInit;
        static GraphicsPlatform platform;
        public static GraphicsPlatform P
        {
            get { return platform; }
        }
        public static void SetCurrentPlatform(GraphicsPlatform platform)
        {
            if (isInit)
            {
                return;
            }
            isInit = true;
            CurrentGraphicsPlatform.platform = platform;
        }

        public static SolidBrush CreateSolidBrush(Color c)
        {
            return platform.CreateSolidBrush(c);
        }
        public static Pen CreatePen(Color c)
        {
            return platform.CreateSolidPen(c);
        }
        public static Matrix CreateMatrix()
        {
            return platform.CreateMatrix();
        }
        public static GraphicsPath CreateGraphicPath()
        {
            return platform.CreateGraphicPath();
        }
        public static string GenericSerifFontName
        {
            get;
            set;
        }

        public static FontInfo CreateNativeFontWrapper(object nativeFont)
        {
            return platform.CreateNativeFontWrapper(nativeFont);
        }
    }


}