//BSD 2014, WinterDev 
using LayoutFarm.Drawing;
namespace LayoutFarm.Drawing
{
    static class FontDefaultConfig
    {        
        /// <summary>
        /// serif
        /// </summary>
        internal static string DEFAULT_FONT_NAME = LayoutFarm.Drawing.CurrentGraphicPlatform.GenericSerifFontName;
        /// <summary>
        /// Default font size in points. Change this value to modify the default font size.
        /// </summary>
        public const float DEFAULT_FONT_SIZE = 10f;
    }
}