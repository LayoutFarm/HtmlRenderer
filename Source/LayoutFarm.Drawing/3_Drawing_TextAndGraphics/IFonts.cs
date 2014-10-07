//BSD 2014, WinterDev   
using System; 
namespace LayoutFarm.Drawing
{
    public interface IFonts
    {   
        FontInfo GetFontInfo(Font f);
        FontInfo GetFontInfo(string fontname, float fsize, FontStyle st);
        float MeasureWhitespace(Font f);
        Size MeasureString2(char[] str, int startAt, int len, Font font);


        /// <summary>
        /// Measure the width and height of string <paramref name="str"/> when drawn on device context HDC
        /// using the given font <paramref name="font"/>.
        /// </summary>
        /// <param name="str">the string to measure</param>
        /// <param name="font">the font to measure string with</param>
        /// <returns>the size of the string</returns>
        Size MeasureString(string str, Font font);

        /// <summary>
        /// Measure the width and height of string <paramref name="str"/> when drawn on device context HDC
        /// using the given font <paramref name="font"/>.<br/>
        /// Restrict the width of the string and get the number of characters able to fit in the restriction and
        /// the width those characters take.
        /// </summary>
        /// <param name="str">the string to measure</param>
        /// <param name="font">the font to measure string with</param>
        /// <param name="maxWidth">the max width to render the string in</param>
        /// <param name="charFit">the number of characters that will fit under <see cref="maxWidth"/> restriction</param>
        /// <param name="charFitWidth"></param>
        /// <returns>the size of the string</returns>
        Size MeasureString(string str, Font font, float maxWidth, out int charFit, out int charFitWidth);
        Size MeasureString2(char[] str, int startAt, int len, Font font, float maxWidth, out int charFit, out int charFitWidth);


    }

}