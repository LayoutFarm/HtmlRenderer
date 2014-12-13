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


namespace LayoutFarm.Drawing.WinGdi
{
    partial class MyCanvas
    {
        int left;
        int top;
        int right;
        int bottom;
        int canvasOriginX = 0;
        int canvasOriginY = 0;
        Rect invalidateArea = Drawing.Rect.CreateFromLTRB(0, 0, 0, 0);


        //--------------------------------------------------------------------
        public override void SetCanvasOrigin(int x, int y)
        {
            ReleaseHdc();
            //-----------
            //move back to original ?
            this.gx.TranslateTransform(-this.canvasOriginX, -this.canvasOriginY);
            this.gx.TranslateTransform(x, y);

            this.canvasOriginX = x;
            this.canvasOriginY = y; 
        }
        public override int CanvasOriginX
        {
            get { return this.canvasOriginX; }
        }
        public override int CanvasOriginY
        {
            get { return this.canvasOriginY; }
        }


        /// <summary>
        /// Sets the clipping region of this <see cref="T:System.Drawing.Graphics"/> to the result of the specified operation combining the current clip region and the rectangle specified by a <see cref="T:System.Drawing.RectangleF"/> structure.
        /// </summary>
        /// <param name="rect"><see cref="T:System.Drawing.RectangleF"/> structure to combine. </param>
        /// <param name="combineMode">Member of the <see cref="T:System.Drawing.Drawing2D.CombineMode"/> enumeration that specifies the combining operation to use. </param>
        public override void SetClipRect(Rectangle rect, CombineMode combineMode = CombineMode.Replace)
        {
            ReleaseHdc();

            gx.SetClip(
                new System.Drawing.Rectangle(
                    rect.X, rect.Y,
                    rect.Width, rect.Height),
                    (System.Drawing.Drawing2D.CombineMode)combineMode);
        }
        public override bool IntersectsWith(Rect clientRect)
        {
            return clientRect.IntersectsWith(left, top, right, bottom);
        }

        public override bool PushClipArea(int width, int height, ref Rect updateArea)
        {
            clipRectStack.Push(currentClipRect);

            System.Drawing.Rectangle intersectResult =
                System.Drawing.Rectangle.Intersect(
                    currentClipRect,
                    System.Drawing.Rectangle.Intersect(
                    updateArea.ToRectangle().ToRect(),
                    new System.Drawing.Rectangle(0, 0, width, height)));

            currentClipRect = intersectResult;
            if (intersectResult.Width <= 0 || intersectResult.Height <= 0)
            {
                //not intersec?
                return false;
            }
            else
            {

                gx.SetClip(intersectResult);
                return true;
            }
        }
        public override void PopClipArea()
        {
            if (clipRectStack.Count > 0)
            {
                currentClipRect = clipRectStack.Pop();

                gx.SetClip(currentClipRect);
            }
        }



        public override Rectangle CurrentClipRect
        {
            get
            {
                return currentClipRect.ToRect();
            }
        }

        public override Brush CurrentBrush
        {
            get
            {
                return this.currentBrush;
            }
            set
            {
                this.currentBrush = value;
            }
        }

        public override int Top
        {
            get
            {
                return top;
            }
        }
        public override int Left
        {
            get
            {
                return left;
            }
        }

        public override int Width
        {
            get
            {
                return right - left;
            }
        }
        public override int Height
        {
            get
            {
                return bottom - top;
            }
        }
        public override int Bottom
        {
            get
            {
                return bottom;
            }
        }
        public override int Right
        {
            get
            {
                return right;
            }
        }
        public override Rectangle Rect
        {
            get
            {
                return Rectangle.FromLTRB(left, top, right, bottom);
            }
        }
        public override Rect InvalidateArea
        {
            get
            {
                return invalidateArea;
            }
        }
        public override void Invalidate(Rect rect)
        {
            invalidateArea.MergeRect(rect);
            this.IsContentReady = false;
        }
    }

}