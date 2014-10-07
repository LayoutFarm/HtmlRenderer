//BSD 2014, WinterDev

namespace LayoutFarm.Drawing
{

    public class FontInfo
    {

        public FontInfo(Font f, int lineHeight, float ascentPx, float descentPx, float baseline)
        {
            this.Font = f;
            this.LineHeight = lineHeight;
            this.DescentPx = descentPx;
            this.AscentPx = ascentPx;
            this.BaseLine = baseline;
        }
        public Font Font { get; private set; }
        public float AscentPx { get; private set; }
        public float DescentPx { get; private set; }
        public float BaseLine { get; private set; }
        public int LineHeight { get; private set; }

    }
}