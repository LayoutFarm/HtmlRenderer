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

using DrawingBridge;

namespace LayoutFarm
{
    partial class MyCanvas
    {

        
        //------------------
        Canvas IGraphics.CurrentCanvas
        {
            get { return this; }
        }
        GraphicPlatform IGraphics.Platform
        {
            get { return LayoutFarm.Drawing.CurrentGraphicPlatform.P; }
        }

        /// <summary>
        /// Gets the bounding clipping region of this graphics.
        /// </summary>
        /// <returns>The bounding rectangle for the clipping region</returns>
        LayoutFarm.Drawing.RectangleF IGraphics.GetClip()
        {
            if (_hdc == IntPtr.Zero)
            {
                var clip1 = gx.ClipBounds;
                return new LayoutFarm.Drawing.RectangleF(
                    clip1.X, clip1.Y,
                    clip1.Width, clip1.Height);
            }
            else
            {
                System.Drawing.Rectangle lprc;
                Win32Utils.GetClipBox(_hdc, out lprc);


                return new LayoutFarm.Drawing.RectangleF(
                    lprc.X, lprc.Y,
                    lprc.Width, lprc.Height);
            }
        }

        void IGraphics.DrawString(char[] str, int startAt, int len, Font font, Color color, PointF point, SizeF size)
        {

#if DEBUG
            dbugCounter.dbugDrawStringCount++;
#endif
            if (_useGdiPlusTextRendering)
            {
                //ReleaseHdc();
                //_g.DrawString(
                //    new string(str, startAt, len),
                //    font,
                //    RenderUtils.GetSolidBrush(color),
                //    (int)Math.Round(point.X + canvasOriginX - FontsUtils.GetFontLeftPadding(font) * .8f),
                //    (int)Math.Round(point.Y + canvasOriginY));

            }
            else
            {
                if (color.A == 255)
                {
                    SetFont(font);
                    SetTextColor(color);
                    unsafe
                    {
                        fixed (char* startAddr = &str[0])
                        {
                            Win32Utils.TextOut2(_hdc, (int)Math.Round(point.X + canvasOriginX),
                                (int)Math.Round(point.Y + canvasOriginY), (startAddr + startAt), len);
                        }
                    }
                }
                else
                {
                    //translucent / transparent text
                    InitHdc();
                    unsafe
                    {
                        fixed (char* startAddr = &str[0])
                        {
                            Win32Utils.TextOut2(_hdc, (int)Math.Round(point.X + canvasOriginX),
                                (int)Math.Round(point.Y + canvasOriginY), (startAddr + startAt), len);
                        }
                    }

                    //DrawTransparentText(_hdc, str, font, new Point((int)Math.Round(point.X), (int)Math.Round(point.Y)), Size.Round(size), color);
                }
            }
        }
    }
}