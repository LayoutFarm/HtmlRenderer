//Apache2, 2014-2017, WinterDev

using PixelFarm.Drawing;
namespace LayoutFarm.CustomWidgets
{
    public class CustomTextRun : RenderElement
    {
        char[] textBuffer;
        Color textColor;
#if DEBUG
        public bool dbugBreak;
#endif
        public CustomTextRun(RootGraphic rootgfx, int width, int height)
            : base(rootgfx, width, height)
        {
        }
        public override void ResetRootGraphics(RootGraphic rootgfx)
        {
            DirectSetRootGraphics(this, rootgfx);
        }
        public string Text
        {
            get { return new string(this.textBuffer); }
            set
            {
                if (value == null)
                {
                    this.textBuffer = null;
                }
                else
                {
                    this.textBuffer = value.ToCharArray();
                }
            }
        }
        public Color TextColor
        {
            get { return this.textColor; }
            set { this.textColor = value; }
        }
        public override void CustomDrawToThisCanvas(Canvas canvas, Rectangle updateArea)
        {
            if (this.textBuffer != null)
            {
                var prevColor = canvas.CurrentTextColor;
                canvas.CurrentTextColor = textColor;
                canvas.DrawText(this.textBuffer, this.X, this.Y);
                canvas.CurrentTextColor = prevColor;
            }
        }
    }
}