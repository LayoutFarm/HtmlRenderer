//BSD 2014, WinterDev
using System.Drawing;
namespace LayoutFarm.Drawing
{
    public struct Color
    {
        byte r, g, b, a;
        public Color(byte a, byte r, byte g, byte b)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }
        public byte R
        {
            get { return this.r; }
        }
        public byte G
        {
            get { return this.g; }
        }
        public byte B
        {
            get { return this.b; }
        }
        public byte A
        {
            get { return this.a; }
        }
        internal System.Drawing.Color ToDrawingColor()
        {
            return System.Drawing.Color.FromArgb(this.a, this.r, this.g, this.b);
        }
        public static Color FromArgb(int a, int r, int g, int b)
        {
            return new Color((byte)a, (byte)r, (byte)g, (byte)b);
        }
        public static Color FromArgb(int r, int g, int b)
        {
            return new Color(255, (byte)r, (byte)g, (byte)b);
        }
        public static readonly Color Empty = new Color(0, 0, 0, 0);
        public static readonly Color Transparent = new Color(0, 0, 0, 0);
        public static readonly Color White = new Color(255, 255, 255, 255);
        public static readonly Color Black = new Color(255, 0, 0, 0);
        public static readonly Color Blue = new Color(255, 0, 0, 255);
        public static readonly Color Red = new Color(255, 255, 0, 0);
        public static readonly Color Yellow = ColorFromDrawingColor(System.Drawing.Color.Yellow);
        public static readonly Color LightGray = ColorFromDrawingColor(System.Drawing.Color.LightGray);


        internal static Color ColorFromDrawingColor(System.Drawing.Color c)
        {
            return new Color(c.A, c.R, c.G, c.B);
        }
        public static Color FromName(string name)
        {
            var color = System.Drawing.Color.FromName(name);
            return new Color(color.A, color.R, color.G, color.B);

        }
        public override bool Equals(object obj)
        {
            Color c2 = (Color)obj;
            return this.r == c2.r &&
                 this.g == c2.g &&
                 this.b == c2.b &&
                 this.a == c2.a;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public static bool operator ==(Color c1, Color c2)
        {
            return c1.r == c2.r &&
                c1.g == c2.g &&
                c1.b == c2.b &&
                c1.a == c2.a;
        }
        public static bool operator !=(Color c1, Color c2)
        {

            return c1.r != c2.r ||
                c1.g != c2.g ||
                c1.b != c2.b ||
                c1.a != c2.a;
        }

        public bool IsTransparent()
        {
            return this == Color.Transparent;
        }
    }
}