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
        public Canvas CurrentCanvas
        {
            get { return this; }
        }

        /// <summary>
        /// Gets the bounding clipping region of this graphics.
        /// </summary>
        /// <returns>The bounding rectangle for the clipping region</returns>
        public override LayoutFarm.Drawing.RectangleF GetClip()
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
        public override void DrawText(char[] str, int startAt, int len, Rectangle logicalTextBox, int textAlignment)
        {

#if DEBUG
            dbugCounter.dbugDrawStringCount++;
#endif
            var color = this.CurrentTextColor;
            if (color.A == 255)
            {  
                unsafe
                {
                    fixed (char* startAddr = &str[0])
                    {
                        Win32Utils.TextOut2(_hdc, (int)Math.Round(logicalTextBox.X + canvasOriginX),
                            (int)Math.Round(logicalTextBox.Y + canvasOriginY), (startAddr + startAt), len);
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
                        Win32Utils.TextOut2(_hdc, (int)Math.Round(logicalTextBox.X + canvasOriginX),
                            (int)Math.Round(logicalTextBox.Y + canvasOriginY), (startAddr + startAt), len);
                    }
                }

                //DrawTransparentText(_hdc, str, font, new Point((int)Math.Round(point.X), (int)Math.Round(point.Y)), Size.Round(size), color);
            }
        }
         
    }
}