
namespace LayoutFarm.Drawing
{
    public abstract class GraphicPlatform
    {


        public abstract Bitmap CreateBitmap(int width, int height);
        public abstract Bitmap CreateBitmap(object bmp);
        public abstract Font CreateFont(object font);

        public abstract SolidBrush CreateSolidBrush(Color color);
        public abstract Pen CreatePen(Brush brush);
        public abstract Pen CreateSolidPen(Color color);

        public abstract TextureBrush CreateTextureBrush(Image img);
        public abstract TextureBrush CreateTextureBrush(Image img, Rectangle rect);

        public abstract LinearGradientBrush CreateLinearGradientBrush(PointF startPoint, PointF stopPoint, Color startColor, Color stopColor);
        public abstract LinearGradientBrush CreateLinearGradientBrush(RectangleF rect, Color startColor, Color stopColor, float angle);
      



        public static string GenericSerifFontName
        {
            get;
            set;
        }

        static bool isInit;
        static GraphicPlatform platform;

        public static void SetCurrentPlatform(GraphicPlatform platform)
        {
            if (isInit)
            {
                return;
            }
            isInit = true;
            GraphicPlatform.platform = platform;
        }

        public static SolidBrush CreateSolidBrushFromColor(Color c)
        {
            return platform.CreateSolidBrush(c);
        }
        public static Pen CreatePen(Color c)
        {
            return platform.CreateSolidPen(c);
        }
    }


}