//BSD, 2014-2017, WinterDev
//ArthurHub  , Jose Manuel Menendez Poo


using PixelFarm.Drawing;

namespace LayoutFarm
{

  
    public interface IActualFontResolver
    {
        PixelFarm.Drawing.Fonts.ActualFont Resolve(RequestFont font);
    }

    public static class ActualFontResolver
    {
        public static IActualFontResolver Resolver { get; set; }

    }
    public static class TextServices
    {
        //implement ifonts  
        public static IFonts IFonts
        {
            get;
            set;
        }
    }

}