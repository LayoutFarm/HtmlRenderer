
namespace LayoutFarm.Drawing
{
    public abstract class GraphicsPlatform
    {   
        public abstract FontInfo CreateNativeFontWrapper(object nativeFont);      
        public abstract GraphicsPath CreateGraphicsPath(); 
        public abstract Canvas CreateCanvas( 
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