//BSD, 2014
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

//MIT, 2018-present, WinterDev
using PixelFarm.Drawing;
namespace LayoutFarm.HtmlBoxes
{
    partial class CssBoxHr
    {
        /// <summary>
        /// Paints the fragment
        /// </summary>
        /// <param name="g">the device to draw to</param>
        protected override void PaintImp(PaintVisitor p)
        {
#if DEBUG
            p.dbugEnterNewContext(this, PaintVisitor.PaintVisitorContextName.Init);
#endif
            var rect = new RectangleF(0, 0, this.VisualWidth, this.VisualHeight);
            if (rect.Height > 2 && RenderUtils.IsColorVisible(ActualBackgroundColor))
            {
                p.FillRectangle(ActualBackgroundColor, rect.Left, rect.Top, rect.Width, rect.Height);
            }

            if (rect.Height > 1)
            {
                p.PaintBorders(this, rect);
            }
            else
            {
                p.PaintBorder(this, CssSide.Top, this.BorderTopColor, rect);
            }
#if DEBUG
            p.dbugExitContext();
#endif
        }
    }

}