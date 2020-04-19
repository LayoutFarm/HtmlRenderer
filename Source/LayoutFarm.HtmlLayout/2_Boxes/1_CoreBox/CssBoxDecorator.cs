//BSD, 2014-present, WinterDev 

using PixelFarm.Drawing;
namespace LayoutFarm.HtmlBoxes
{
    partial class CssBoxDecorator
    {
#if DEBUG
        static int totaldbugId = 0;
        public readonly int dbugId = totaldbugId++;
#endif
        public CssBoxDecorator()
        {
        }
        public Color Color { get; set; }
        public int HBoxShadowOffset { get; set; }
        public int VBoxShadowOffset { get; set; }
        public int BlurRadius { get; set; }
        public int SpreadDistance { get; set; }

    }
}
