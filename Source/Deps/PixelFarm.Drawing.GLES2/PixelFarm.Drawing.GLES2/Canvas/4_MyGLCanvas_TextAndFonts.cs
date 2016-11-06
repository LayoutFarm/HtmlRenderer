//BSD, 2014-2016, WinterDev
//ArthurHub  , Jose Manuel Menendez Poo

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
using PixelFarm.Drawing.Fonts;
using Win32;

namespace PixelFarm.Drawing.GLES2
{
    partial class MyGLCanvas
    {

        RequestFont currentTextFont = null;
        Color mycurrentTextColor = Color.Black;


        //public override float GetCharWidth(RequestFont f, char c)
        //{
        //    return GLES2PlatformFontMx.Default.ResolveForGdiFont(f).GetGlyph(c).horiz_adv_x >> 6;
        //    //NativeFont font = nativeFontStore.GetResolvedNativeFont(f);
        //    //return font.GetGlyph(c).horiz_adv_x >> 6;
        //}

   
        //============================================== 

        public override RequestFont CurrentFont
        {
            get
            {
                return currentTextFont;
            }
            set
            {
                this.currentTextFont = value;
            }
        }
        public override Color CurrentTextColor
        {
            get
            {
                return mycurrentTextColor;
            }
            set
            {
                this.currentTextColor = value;
            }
        }
        public override void DrawText(char[] buffer, int x, int y)
        {
            //var tmpColor = this.internalSolidBrush.Color;
            //internalSolidBrush.Color = this.currentTextColor;
            //gx.DrawString(new string(buffer),
            //    (System.Drawing.Font)this.currentTextFont.InnerFont,
            //    internalSolidBrush, new System.Drawing.PointF(x, y));
            //internalSolidBrush.Color = tmpColor;
        }
        public override void DrawText(char[] buffer, Rectangle logicalTextBox, int textAlignment)
        {
            //var tmpColor = this.internalSolidBrush.Color;
            //internalSolidBrush.Color = this.currentTextColor;
            //gx.DrawString(new string(buffer),
            //    (System.Drawing.Font)this.currentTextFont.InnerFont,
            //    internalSolidBrush,
            //    new System.Drawing.RectangleF(
            //        logicalTextBox.X,
            //        logicalTextBox.Y,
            //        logicalTextBox.Width,
            //        logicalTextBox.Height));
            //internalSolidBrush.Color = tmpColor;
        }
        public override void DrawText(char[] str, int startAt, int len, Rectangle logicalTextBox, int textAlignment)
        {
            ////var intersectRect = Rectangle.Intersect(logicalTextBox,
            ////    new Rectangle(currentClipRect.Left,
            ////        currentClipRect.Top,
            ////        currentClipRect.Width,
            ////        currentClipRect.Height));
            ////intersectRect.Offset(canvasOriginX, canvasOriginY);
            ////MyWin32.SetRectRgn(hRgn,
            //// intersectRect.Left,
            //// intersectRect.Top,
            //// intersectRect.Right,
            //// intersectRect.Bottom);
            ////MyWin32.SelectClipRgn(tempDc, hRgn);



            //var tmpColor = this.internalSolidBrush.Color;
            //internalSolidBrush.Color = this.currentTextColor;
            //gx.DrawString(new string(str, startAt, len),
            //    (System.Drawing.Font)this.currentTextFont.InnerFont,
            //    internalSolidBrush,
            //    logicalTextBox.X,
            //    logicalTextBox.Y);
            ////new System.Drawing.RectangleF(
            ////    logicalTextBox.X,
            ////    logicalTextBox.Y,
            ////    logicalTextBox.Width,
            ////    logicalTextBox.Height));
            //internalSolidBrush.Color = tmpColor;
            ////var str= new string(
            ////fixed (char* startAddr = &str[0])
            ////{
            ////    Win32.Win32Utils.TextOut2(tempDc,
            ////        (int)logicalTextBox.X + canvasOriginX,
            ////        (int)logicalTextBox.Y + canvasOriginY,
            ////        (startAddr + startAt), len);
            ////}


        }
    }
}