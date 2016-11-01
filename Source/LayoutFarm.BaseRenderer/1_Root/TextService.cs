//BSD, 2014-2016, WinterDev
//ArthurHub  , Jose Manuel Menendez Poo


using PixelFarm.Drawing;

namespace LayoutFarm
{

    public interface IFonts
    {
        float MeasureWhitespace(RequestFont f);
        Size MeasureString(char[] str, int startAt, int len, RequestFont font);
        Size MeasureString(char[] str, int startAt, int len, RequestFont font, float maxWidth, out int charFit, out int charFitWidth);
        void Dispose();
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