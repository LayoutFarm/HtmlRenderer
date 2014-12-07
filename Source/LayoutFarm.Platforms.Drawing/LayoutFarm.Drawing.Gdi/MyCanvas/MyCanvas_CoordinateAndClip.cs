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
        public override  void SetCanvasOrigin(float x, float y)
        {
            ReleaseHdc();
            //-----------
            //move back to original ?
            this.gx.TranslateTransform(-this.canvasOriginX, -this.canvasOriginY);
            this.gx.TranslateTransform(x, y);

            this.canvasOriginX = x;
            this.canvasOriginY = y;
        }
        public override float CanvasOriginX
        {
            get { return this.canvasOriginX; }
        }
        public override float CanvasOriginY
        {
            get { return this.canvasOriginY; }
        }
        int CanvasOrgX { get { return (int)this.canvasOriginX; } }
        int CanvasOrgY { get { return (int)this.canvasOriginY; } }

        public override void OffsetCanvasOrigin(int dx, int dy)
        {

            SetCanvasOrigin(this.canvasOriginX + dx, this.canvasOriginY + dy);
            currentClipRect.Offset(-dx, -dy);
        }
        public override void OffsetCanvasOriginX(int dx)
        {

            SetCanvasOrigin(this.canvasOriginX + dx, this.canvasOriginY);
            currentClipRect.Offset(-dx, 0);
        }
        public override void OffsetCanvasOriginY(int dy)
        {

            SetCanvasOrigin(this.canvasOriginX, this.canvasOriginY + dy);
            currentClipRect.Offset(0, -dy);
        }

        //--------------------------------------------------------------------

        /// <summary>
        /// Sets the clipping region of this <see cref="T:System.Drawing.Graphics"/> to the result of the specified operation combining the current clip region and the rectangle specified by a <see cref="T:System.Drawing.RectangleF"/> structure.
        /// </summary>
        /// <param name="rect"><see cref="T:System.Drawing.RectangleF"/> structure to combine. </param>
        /// <param name="combineMode">Member of the <see cref="T:System.Drawing.Drawing2D.CombineMode"/> enumeration that specifies the combining operation to use. </param>
        public override void SetClip(RectangleF rect, CombineMode combineMode = CombineMode.Replace)
        {
            ReleaseHdc();
            gx.SetClip(rect.ToRectF(), (System.Drawing.Drawing2D.CombineMode)combineMode);
        }
        public override bool IntersectsWith(Rect clientRect)
        {
            return clientRect.IntersectsWith(left, top, right, bottom);
        }
        public override bool PushClipAreaForNativeScrollableElement(Rect updateArea)
        {

            clipRectStack.Push(currentClipRect);

            System.Drawing.Rectangle intersectResult = System.Drawing.Rectangle.Intersect(
                currentClipRect,
                updateArea.ToRectangle().ToRect());

            if (intersectResult.Width <= 0 || intersectResult.Height <= 0)
            {
                currentClipRect = intersectResult;
                return false;
            }

            gx.SetClip(intersectResult);
            currentClipRect = intersectResult;
            return true;
        }


        public override bool PushClipArea(int width, int height, Rect updateArea)
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
                return false;
            }
            else
            {
                gx.SetClip(intersectResult); return true;
            }
        }

        public override void DisableClipArea()
        {
            gx.ResetClip();
        }
        public override void EnableClipArea()
        {
            gx.SetClip(currentClipRect);
        }

        public override Rectangle CurrentClipRect
        {
            get
            {
                return currentClipRect.ToRect();
            }
        }



        public override bool PushClipArea(int x, int y, int width, int height)
        {
            clipRectStack.Push(currentClipRect);
            System.Drawing.Rectangle intersectRect = System.Drawing.Rectangle.Intersect(
                currentClipRect,
                new System.Drawing.Rectangle(x, y, width, height));


            if (intersectRect.Width == 0 || intersectRect.Height == 0)
            {
                currentClipRect = intersectRect;
                return false;
            }
            else
            {
                gx.SetClip(intersectRect);
                currentClipRect = intersectRect;
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

    }

}