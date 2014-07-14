//BSD 2014, WinterDev 

using System.Drawing;
namespace HtmlRenderer.Drawing
{

    public interface IFontPool
    {
        FontInfo GetFontInfo(Font f);
        FontInfo GetFontInfo(string fontname, float fontSize, FontStyle fontStyle);
        
    }

}