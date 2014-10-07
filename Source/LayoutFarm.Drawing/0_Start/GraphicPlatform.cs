
namespace LayoutFarm.Drawing
{
    public abstract class GraphicPlatform
    {


        public abstract Bitmap CreateBitmap(int width, int height);
        public abstract Bitmap CreateBitmap(object bmp);
        public abstract Font CreateFont(object font);
        public abstract TextFontInfo CreateTexFontInfo(object nativeFont);

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

        
        
        public abstract IGraphics SampleIGraphics { get; }
        public abstract IFonts SampleIFonts { get; } 
    }

    public static class CurrentGraphicPlatform
    {
        static bool isInit;
        static GraphicPlatform platform;

        public static GraphicPlatform P
        {
            get { return platform; }
        }
        public static void SetCurrentPlatform(GraphicPlatform platform)
        {
            if (isInit)
            {
                return;
            }
            isInit = true;
            CurrentGraphicPlatform.platform = platform;
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
        public static Font CreateFont(object f)
        {
            return platform.CreateFont(f);
        }
        public static TextFontInfo CreateTexFontInfo(object nativeFont)
        {
            return platform.CreateTexFontInfo(nativeFont);
        }
    }


}