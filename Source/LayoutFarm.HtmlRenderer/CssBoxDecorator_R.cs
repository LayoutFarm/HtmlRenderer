//BSD, 2014-present, WinterDev 


namespace LayoutFarm.HtmlBoxes
{
    partial class CssBoxDecorator
    {

        public void Paint(CssBox box, PaintVisitor p)
        {
            p.FillRectangle(this.Color,
                box.LocalX + this.HBoxShadowOffset,
                box.LocalY + this.VBoxShadowOffset,
                box.VisualWidth,
                box.VisualHeight);
        }
    }
}
