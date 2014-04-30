using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace HtmlRenderer
{
    internal class Test
    {
        private const int BaseIterations = 3;
        private const int RenderIterations = 20000;
        private const string String = "Test-string m(g=j{}3)";
        private const TextRenderingHint TextRendering = TextRenderingHint.AntiAlias;
        private const SmoothingMode Smoothing = SmoothingMode.HighQuality;
        private static readonly Font _font1 = new Font("Arial", 10);
//        private static readonly Font _font1 = new Font("Segoe UI", 10);
//        private static readonly Font _font1 = new Font("Tahoma", 10);
        private static readonly Font _font2 = new Font(SystemFonts.DefaultFont.FontFamily, 12);
        private static readonly Font _font3 = new Font("Arial", 12);
        private static readonly Font[] _fonts = new[] {_font1, _font2, _font3};

        public static void Run()
        {
            RunVisualTest();

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

//            RunPerformanceTest();
        }

        private static void RunVisualTest()
        {
            var image = new Bitmap(560, 400, PixelFormat.Format32bppArgb);

            int xOffset = image.Width/2 + 5;
            int yOffset = 5;

            using(var g = Graphics.FromImage(image))
            {
                g.FillRectangle(Brushes.White, 0, 0, image.Width/2, image.Height);

                IntPtr dib;
                var memoryHdc = CreateMemoryHdc(IntPtr.Zero, image.Width, 40, out dib);
                try
                {
                    // create memory buffer graphics to use for HTML rendering
                    using(var memoryGraphics = Graphics.FromHdc(memoryHdc))
                    {
                        memoryGraphics.FillRectangle(Brushes.White, 0, 0, image.Width/2, image.Height);

                        // execute GDI text rendering
                        var hdc = memoryGraphics.GetHdc();
                        SelectObject(hdc, _font1.ToHfont());
                        SetTextColor(hdc, ( Color.Red.B & 0xFF ) << 16 | ( Color.Red.G & 0xFF ) << 8 | Color.Red.R);
                        const string tStr = String + " (Native)";
                        TextOut(hdc, 5, yOffset, tStr, tStr.Length);
                        TextOut(hdc, xOffset, yOffset, tStr, tStr.Length);
                        memoryGraphics.ReleaseHdc(hdc);
                    }

                    // copy from memory buffer to image
                    using(var imageGraphics = Graphics.FromImage(image))
                    {
                        var imgHdc = imageGraphics.GetHdc();
                        BitBlt(imgHdc, 0, 0, image.Width, 40, memoryHdc, 0, 0, 0x00CC0020);
                        imageGraphics.ReleaseHdc(imgHdc);
                    }
                }
                finally
                {
                    // release memory buffer
                    DeleteObject(dib);
                    DeleteDC(memoryHdc);
                }

                yOffset += 50;

//                TextRenderer.DrawText(g, String + " (TextRenderer)", _font1, new Point(5, yOffset), Color.Red, Color.White);
//                TextRenderer.DrawText(g, String + " (TextRenderer)", _font1, new Point(xOffset, yOffset), Color.Red);
//                yOffset += 50;

                g.TextRenderingHint = TextRendering;
                g.DrawString(String + " (DrawString)", _font1, Brushes.Red, new Point(5, yOffset));
                g.DrawString(String + " (DrawString)", _font1, Brushes.Red, new Point(xOffset, yOffset));
                g.TextRenderingHint = TextRenderingHint.SystemDefault;
                yOffset += 50;

                g.SmoothingMode = Smoothing;
                float emSize = g.DpiY * _font1.Size / 72f;
                using (var path = new GraphicsPath())
                {
                    path.AddString(String + " (GraphicsPath)", _font1.FontFamily, (int)_font1.Style, emSize, new Point(5, yOffset), StringFormat.GenericDefault);
                    g.FillPath(Brushes.Red, path);
                }

                using (var path = new GraphicsPath())
                {
                    path.AddString(String + " (GraphicsPath)", _font1.FontFamily, (int)_font1.Style, emSize, new Point(xOffset, yOffset), StringFormat.GenericDefault);
                    g.FillPath(Brushes.Red, path);
                }
                g.SmoothingMode = SmoothingMode.Default;
            }

            image.Save("TestVisal.png", ImageFormat.Png);
        }

        private static void RunPerformanceTest()
        {
            GC.Collect();
            long tr = RunPerformanceIterations(1);
            GC.Collect();
            long ds = RunPerformanceIterations(2);
            GC.Collect();
            long gp1 = RunPerformanceIterations(3);
            GC.Collect();
            long gp2 = RunPerformanceIterations(4);
            GC.Collect();
            long n = RunPerformanceIterations(5);

            var msg = string.Format("Native: {0}\r\nDrawString: {1}\t({2:N1} times slower)\r\nTextRenderer: {3}\t({4:N1}-{5:N1} times slower)\r\n" +
                                    "GraphicsPath Normal: {6}\t({7:N1}-{8:N1} times slower)\r\nGraphicsPath HighQuality: {9}\t({10:N1}-{11:N1} times slower)",
                                    n, ds, ds / (double)n, tr, tr / (double)n, tr / (double)ds, gp1, gp1 / (double)n, gp1 / (double)ds, gp2, gp2 / (double)n, gp2 / (double)ds);
            MessageBox.Show(msg);
        }

        public static long RunPerformanceIterations(int type)
        {
            var sw = Stopwatch.StartNew();

            for (int i = 0; i < BaseIterations; i++)
            {
                var font = _fonts[i%_fonts.Length];

                var image = new Bitmap(500, 500, PixelFormat.Format32bppArgb);
                using(var g = Graphics.FromImage(image))
                {
                    switch( type )
                    {
                        case 1:
                            RunTextRenderer(g, font);
                            break;
                        case 2:
                            RunDrawString(g, font);
                            break;
                        case 3:
                            RunGraphicsPathNormal(g, font);
                            break;
                        case 4:
                            RunGraphicsPathHighQuiality(g, font);
                            break;
                        case 5:
                            RunNative(image, font);
                            break;
                    }
                }
            }

            return sw.ElapsedMilliseconds;
        }

        private static void RunTextRenderer(Graphics g, Font font)
        {
            for(int i = 0; i < RenderIterations; i++)
            {
                TextRenderer.DrawText(g, String, font, new Point(5, 5), Color.Red);
            }
        }

        private static void RunDrawString(Graphics g, Font font)
        {
            g.TextRenderingHint = TextRendering;
            for (int i = 0; i < RenderIterations; i++)
            {
                g.DrawString(String, font, Brushes.Red, new Point(5, 5));
            }
            g.TextRenderingHint = TextRenderingHint.SystemDefault;
        }

        private static void RunGraphicsPathNormal(Graphics g, Font font)
        {
            var fontFamily = font.FontFamily;
            float emSize = g.DpiY * _font1.Size / 72f;
            for (int i = 0; i < RenderIterations; i++)
            {
                using (var path = new GraphicsPath())
                {
                    path.AddString(String, fontFamily, (int)font.Style, emSize, new Point(5, 5), StringFormat.GenericDefault);
                    g.FillPath(Brushes.Red, path);
                }
            }
        }
        private static void RunGraphicsPathHighQuiality(Graphics g, Font font)
        {
            var fontFamily = font.FontFamily;
            float emSize = g.DpiY * _font1.Size / 72f;
            g.SmoothingMode = SmoothingMode.HighQuality;
            for (int i = 0; i < RenderIterations; i++)
            {
                using (var path = new GraphicsPath())
                {
                    path.AddString(String, fontFamily, (int)font.Style, emSize, new Point(5, 5), StringFormat.GenericDefault);
                    g.FillPath(Brushes.Red, path);
                }
            }
            g.SmoothingMode = SmoothingMode.Default;
        }

        private static void RunNative(Image image, Font font)
        {
            // create memory buffer from desktop handle that supports alpha channel
            IntPtr dib;
            var memoryHdc = CreateMemoryHdc(IntPtr.Zero, image.Width, image.Height, out dib);
            try
            {
                // create memory buffer graphics to use for HTML rendering
                using (var memoryGraphics = Graphics.FromHdc(memoryHdc))
                {
                    // must not be transparent background 
                    memoryGraphics.Clear(Color.White);

                    // execute GDI text rendering
                    var hdc = memoryGraphics.GetHdc();
                    SelectObject(hdc, font.ToHfont());
                    SetTextColor(hdc, (Color.Red.B & 0xFF) << 16 | (Color.Red.G & 0xFF) << 8 | Color.Red.R);
                    for(int i = 0; i < RenderIterations; i++)
                    {
                        TextOut(hdc, 5, 5, String, String.Length);
                    }
                    memoryGraphics.ReleaseHdc(hdc);
                }

                // copy from memory buffer to image
                using (var imageGraphics = Graphics.FromImage(image))
                {
                    var imgHdc = imageGraphics.GetHdc();
                    BitBlt(imgHdc, 0, 0, image.Width, image.Height, memoryHdc, 0, 0, 0x00CC0020);
                    imageGraphics.ReleaseHdc(imgHdc);
                }
            }
            finally
            {
                // release memory buffer
                DeleteObject(dib);
                DeleteDC(memoryHdc);
            }
        }


        private static IntPtr CreateMemoryHdc(IntPtr hdc, int width, int height, out IntPtr dib)
        {
            // Create a memory DC so we can work off-screen
            IntPtr memoryHdc = CreateCompatibleDC(hdc);
            SetBkMode(memoryHdc, 1);

            // Create a device-independent bitmap and select it into our DC
            var info = new BitMapInfo();
            info.biSize = Marshal.SizeOf(info);
            info.biWidth = width;
            info.biHeight = -height;
            info.biPlanes = 1;
            info.biBitCount = 32;
            info.biCompression = 0; // BI_RGB
            IntPtr ppvBits;
            dib = CreateDIBSection(hdc, ref info, 0, out ppvBits, IntPtr.Zero, 0);
            SelectObject(memoryHdc, dib);

            return memoryHdc;
        }

        [DllImport("gdi32.dll")]
        public static extern int SetBkMode(IntPtr hdc, int mode);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        private static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport("gdi32.dll")]
        private static extern int SetTextColor(IntPtr hdc, int color);
 
        [DllImport("gdi32.dll", EntryPoint = "TextOutW")]
        private static extern bool TextOut(IntPtr hdc, int x, int y, [MarshalAs(UnmanagedType.LPWStr)] string str, int len);
 
        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateDIBSection(IntPtr hdc, [In] ref BitMapInfo pbmi, uint iUsage, out IntPtr ppvBits, IntPtr hSection, uint dwOffset);

        [DllImport("gdi32.dll")]
        public static extern int SelectObject(IntPtr hdc, IntPtr hgdiObj);

        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BitBlt(IntPtr hdc, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, long dwRop);

        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern bool DeleteDC(IntPtr hdc);

        [StructLayout(LayoutKind.Sequential)]
        internal struct BitMapInfo
        {
            public int biSize;
            public int biWidth;
            public int biHeight;
            public short biPlanes;
            public short biBitCount;
            public int biCompression;
            public int biSizeImage;
            public int biXPelsPerMeter;
            public int biYPelsPerMeter;
            public int biClrUsed;
            public int biClrImportant;
            public byte bmiColors_rgbBlue;
            public byte bmiColors_rgbGreen;
            public byte bmiColors_rgbRed;
            public byte bmiColors_rgbReserved;
        }
    }
}