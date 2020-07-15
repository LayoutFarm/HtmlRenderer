//MIT, 2020-present, WinterDev

using PixelFarm.Drawing;
 

namespace LayoutFarm.Css
{
    public interface IHtmlTextService
    {
        float MeasureWhitespace(RequestFont f);
        float MeasureBlankLineHeight(RequestFont f);

        Size MeasureString(in TextBufferSpan textBufferSpan, RequestFont font);
        Size MeasureString(in TextBufferSpan textBufferSpan, Typography.Text.ResolvedFont font);
        void MeasureString(in TextBufferSpan textBufferSpan, RequestFont font, int maxWidth, out int charFit, out int charFitWidth);

        Typography.Text.ResolvedFont ResolveFont(RequestFont reqFont);
    }

    public interface IHtmlRequestFont
    {

    }
}