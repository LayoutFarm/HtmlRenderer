//2014 BSD, WinterDev
//ArthurHub

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

using System;
using System.Collections.Generic;
using System.Text;
using LayoutFarm.Drawing;



namespace LayoutFarm
{
    partial class MyCanvas
    {


        static readonly int[] _charFit = new int[1];
        static readonly int[] _charFitWidth = new int[1000];
        /// <summary>
        /// Used for GDI+ measure string.
        /// </summary>
        static readonly System.Drawing.CharacterRange[] _characterRanges = new System.Drawing.CharacterRange[1];
        /// <summary>
        /// The string format to use for measuring strings for GDI+ text rendering
        /// </summary>
        static readonly System.Drawing.StringFormat _stringFormat;

        const int CANVAS_UNUSED = 1 << (1 - 1);
        const int CANVAS_DIMEN_CHANGED = 1 << (2 - 1);




        static IntPtr defaultHFont;
        static System.Drawing.Font defaultFont;
        static Font defaultFontInfo;
        static MyCanvas()
        {
            _stringFormat = new System.Drawing.StringFormat(System.Drawing.StringFormat.GenericDefault);
            _stringFormat.FormatFlags = System.Drawing.StringFormatFlags.NoClip | System.Drawing.StringFormatFlags.MeasureTrailingSpaces;
            //---------------------------
            MyCanvas.SetupDefaultFont(new System.Drawing.Font("Tahoma", 10));

        }
        static void SetupDefaultFont(System.Drawing.Font f)
        {
            defaultFont = f;
            defaultHFont = f.ToHfont();
            defaultFontInfo = FontsUtils.GetCachedFont(f).ResolvedFont;

        }
        static bool IsEqColor(Color c1, System.Drawing.Color c2)
        {
            return c1.R == c2.R &&
                   c1.G == c2.G &&
                   c1.B == c2.B &&
                   c1.A == c2.A;
        }
        static System.Drawing.Point[] ConvPointArray(Point[] points)
        {
            int j = points.Length;
            System.Drawing.Point[] outputPoints = new System.Drawing.Point[j];
            for (int i = 0; i < j; ++i)
            {
                outputPoints[i] = points[i].ToPoint();
            }
            return outputPoints;
        }
        static System.Drawing.PointF[] ConvPointFArray(PointF[] points)
        {
            int j = points.Length;
            System.Drawing.PointF[] outputPoints = new System.Drawing.PointF[j];
            for (int i = 0; i < j; ++i)
            {
                outputPoints[i] = points[i].ToPointF();
            }
            return outputPoints;
        }
        static System.Drawing.Color ConvColor(Color c)
        {
            return System.Drawing.Color.FromArgb(c.A, c.R, c.G, c.B);
        }
        static System.Drawing.Brush ConvBrush(Brush b)
        {
            return b.InnerBrush as System.Drawing.Brush;
        }
        static System.Drawing.Pen ConvPen(Pen p)
        {
            return p.InnerPen as System.Drawing.Pen;
        }
        static System.Drawing.Bitmap ConvBitmap(Bitmap bmp)
        {
            return bmp.InnerImage as System.Drawing.Bitmap;
        }
        static System.Drawing.Image ConvBitmap(Image img)
        {
            return img.InnerImage as System.Drawing.Image;
        }
        static System.Drawing.Drawing2D.GraphicsPath ConvPath(GraphicsPath p)
        {
            return p.InnerPath as System.Drawing.Drawing2D.GraphicsPath;
        }
        static System.Drawing.Font ConvFont(Font f)
        {
            return f.InnerFont as System.Drawing.Font;
        }
        static System.Drawing.Region ConvRgn(Region rgn)
        {
            return rgn.InnerRegion as System.Drawing.Region;
        }
        static System.Drawing.Drawing2D.GraphicsPath ConvFont(GraphicsPath p)
        {
            return p.InnerPath as System.Drawing.Drawing2D.GraphicsPath;
        }
    }

}