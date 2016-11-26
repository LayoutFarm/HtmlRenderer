//MIT, 2014-2016, WinterDev
using SkiaSharp;
namespace PixelFarm.Drawing.Skia
{
    partial class MySkiaCanvas
    {
        RequestFont currentTextFont = null;
        Color mycurrentTextColor = Color.Black;

        public override void DrawText(char[] buffer, int x, int y)
        {
            SKRect clipRect = currentClipRect;
            clipRect.Offset(canvasOriginX, canvasOriginY);
            skCanvas.ClipRect(clipRect);
            skCanvas.DrawText(new string(buffer), x, y, textStroke);
        }
        public override void DrawText(char[] buffer, Rectangle logicalTextBox, int textAlignment)
        {
            SKRect clipRect = currentClipRect;
            clipRect.Offset(canvasOriginX, canvasOriginY);
            skCanvas.ClipRect(clipRect);
            //TODO: review here
            skCanvas.DrawText(new string(buffer), logicalTextBox.X, logicalTextBox.Y, textStroke);
        }
        public override void DrawText(char[] str, int startAt, int len, Rectangle logicalTextBox, int textAlignment)
        {
            //this is the most common used function for text drawing
            //return;
#if DEBUG
            dbugDrawStringCount++;
#endif
            var color = this.CurrentTextColor;
            if (color.A == 255)
            {
                //1. find clip rect
                var clipRect = Rectangle.Intersect(logicalTextBox,
                    new Rectangle((int)currentClipRect.Left,
                        (int)currentClipRect.Top,
                        (int)currentClipRect.Width,
                        (int)currentClipRect.Height));
                //2. offset to canvas origin 
                clipRect.Offset(canvasOriginX, canvasOriginY);
                //3. set rect rgn

                clipRect.Offset(canvasOriginX, canvasOriginY);
                //gx.SetClip(clipRect);
                skCanvas.DrawText(new string(str, startAt, len), logicalTextBox.X, logicalTextBox.Y, textStroke);
                //gx.ClearClip();
#if DEBUG
                //NativeTextWin32.dbugDrawTextOrigin(tempDc,
                //        logicalTextBox.X + canvasOriginX,
                //        logicalTextBox.Y + canvasOriginY);
#endif

            }
            else
            {

                //1. find clip rect
                var clipRect = Rectangle.Intersect(logicalTextBox,
                    new Rectangle((int)currentClipRect.Left,
                        (int)currentClipRect.Top,
                        (int)currentClipRect.Width,
                        (int)currentClipRect.Height));
                //2. offset to canvas origin 
                clipRect.Offset(canvasOriginX, canvasOriginY);
                //3. set rect rgn

                clipRect.Offset(canvasOriginX, canvasOriginY);
                //gx.SetClip(clipRect);
                //gx.DrawString(str, logicalTextBox);
                skCanvas.DrawText(new string(str, startAt, len), logicalTextBox.X, logicalTextBox.Y, textStroke);
                //gx.ClearClip();

#if DEBUG
                //NativeTextWin32.dbugDrawTextOrigin(tempDc,
                //        logicalTextBox.X + canvasOriginX,
                //        logicalTextBox.Y + canvasOriginY);
#endif
            }
        }
        //====================================================
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
                mycurrentTextColor = value;
            }
        }
    }
}