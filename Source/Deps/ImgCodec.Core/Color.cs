
namespace ImageTools
{
    public struct Color
    {
        byte a, r, g, b;

        public Color(byte a, byte r, byte g, byte b)
        {
            this.a = a;
            this.r = r;
            this.g = g;
            this.b = b;
        }
        public byte R { get { return r; } set { r = value; } }
        public byte G { get { return g; } set { g = value; } }
        public byte B { get { return b; } set { b = value; } }
        public byte A { get { return a; } set { a = value; } }

        public static Color FromArgb(byte a, byte r, byte g, byte b)
        {
            return new Color(a, r, g, b);
        }
    }
} 