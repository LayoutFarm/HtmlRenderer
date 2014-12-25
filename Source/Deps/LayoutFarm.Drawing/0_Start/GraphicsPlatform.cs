
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

        public static string GenericSerifFontName
        {
            get;
            set;
        } 
      
    }
 

}