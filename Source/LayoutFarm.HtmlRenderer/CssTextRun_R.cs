//BSD, 2014-present, WinterDev 
//ArthurHub, Jose Manuel Menendez Poo

// "Therefore those skilled at the unorthodox
// are infinite as heaven and earth,
// inexhaustible as the great rivers.
// When they come to an end,
// they begin again,
// like the days and months;
// they die and are reborn,
// like the four seasons."
// 
// - Sun Tsu,
// "The Art of War"


using System.Text;

namespace LayoutFarm.HtmlBoxes
{
    /// <summary>
    /// Represents a word inside an inline box
    /// </summary>
    partial class CssTextRun : CssRun
    {

        PixelFarm.Drawing.RenderVxFormattedString _cacheRenderVx;
        public static PixelFarm.Drawing.RenderVxFormattedString GetCachedFormatString(CssTextRun textrun) => textrun._cacheRenderVx;
        public static void SetCachedFormattedString(CssTextRun textrun, PixelFarm.Drawing.RenderVxFormattedString cache)
        {
            if (textrun._cacheRenderVx != null && textrun._cacheRenderVx != cache)
            {
                textrun._cacheRenderVx.Dispose();
            }
            textrun._cacheRenderVx = cache;
        }
    }
}