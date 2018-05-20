//BSD, 2014-2017, WinterDev

//
// System.Drawing.KnownColors
//
// Authors:
// Gonzalo Paniagua Javier (gonzalo@ximian.com)
// Peter Dennis Bartok (pbartok@novell.com)
// Sebastien Pouliot <sebastien@ximian.com>
//
// Copyright (C) 2007 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//


namespace LayoutFarm.WebDom
{
    public struct CssColor
    {
        byte r, g, b, a;
        public CssColor(byte a, byte r, byte g, byte b)
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
        public static CssColor FromArgb(int a, CssColor c)
        {
            return new CssColor((byte)a, c.R, c.G, c.B);
        }
        public static CssColor FromArgb(int a, int r, int g, int b)
        {
            return new CssColor((byte)a, (byte)r, (byte)g, (byte)b);
        }
        public static CssColor FromArgb(int r, int g, int b)
        {
            return new CssColor(255, (byte)r, (byte)g, (byte)b);
        }


        public static readonly CssColor Empty = new CssColor(0, 0, 0, 0);
        public static readonly CssColor Transparent = new CssColor(0, 255, 255, 255);
        public static readonly CssColor White = new CssColor(255, 255, 255, 255);
        public static readonly CssColor Black = new CssColor(255, 0, 0, 0);
        public static readonly CssColor Blue = new CssColor(255, 0, 0, 255);
        public static readonly CssColor Red = new CssColor(255, 255, 0, 0);
        public static readonly CssColor Yellow = new CssColor(255, 255, 255, 0);
        public static readonly CssColor LightGray = new CssColor(0xFF, 0xD3, 0xD3, 0xD3);
        public static readonly CssColor Gray = new CssColor(0xFF, 0x80, 0x80, 0x80);
        public static readonly CssColor Green = new CssColor(0xFF, 0x00, 0x80, 0x00);
        public static readonly CssColor OrangeRed = new CssColor(0xFF, 0xFF, 0x45, 0x00);//0xFF FF 45 00
        public static readonly CssColor DeepPink = new CssColor(0xFF, 0xFF, 0x14, 0x93);
        public static readonly CssColor Magenta = new CssColor(0xFF, 0xFF, 0, 0xFF);
        //internal static Color ColorFromDrawingColor(System.Drawing.Color c)
        //{
        //    return new Color(c.A, c.R, c.G, c.B);
        //}
        public static CssColor FromName(string name)
        {
            var color = KnownColors.FromKnownColor(name);
            return new CssColor(color.A, color.R, color.G, color.B);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public static bool operator ==(CssColor c1, CssColor c2)
        {
            return (uint)((c1.a << 24) | (c1.r << 16) | (c1.g << 8) | (c1.b)) ==
                   (uint)((c2.a << 24) | (c2.r << 16) | (c2.g << 8) | (c2.b));
        }
        public static bool operator !=(CssColor c1, CssColor c2)
        {
            return (uint)((c1.a << 24) | (c1.r << 16) | (c1.g << 8) | (c1.b)) !=
                  (uint)((c2.a << 24) | (c2.r << 16) | (c2.g << 8) | (c2.b));
        }
        public uint ToARGB()
        {
            return (uint)((this.a << 24) | (this.r << 16) | (this.g << 8) | this.b);
        }
        public uint ToABGR()
        {
            return (uint)((this.a << 24) | (this.b << 16) | (this.g << 8) | this.r);
        }
    }
}