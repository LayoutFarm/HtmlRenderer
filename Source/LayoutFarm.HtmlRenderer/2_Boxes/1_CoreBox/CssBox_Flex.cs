//MIT, 2015-2017, WinterDev

namespace LayoutFarm.HtmlBoxes
{
    partial class CssBox
    {
        bool hasFlexContext;
        internal bool HasFlexContext
        {
            get { return this.hasFlexContext; }
            set { this.hasFlexContext = value; }
        }
    }
}
