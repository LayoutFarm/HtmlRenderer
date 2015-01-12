//2014,2015 BSD, WinterDev
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


namespace PixelFarm.Drawing.DrawingGL
{
    static class Conv
    {
        public static Rectangle ToRect(System.Drawing.Rectangle r)
        {
            return new Rectangle(r.Left, r.Right, r.Top, r.Bottom);
        }
    }
    partial class MyCanvasGL
    {
        int left;
        int top;
        int right;
        int bottom;
        //int canvasOriginX = 0;
        //int canvasOriginY = 0;
        Rectangle invalidateArea;
        CanvasOrientation orientation;
        public override CanvasOrientation Orientation
        {
            get
            {
                return this.orientation;
            }
            set
            {
                this.orientation = value;
                if (canvasGL2d != null)
                {
                    canvasGL2d.Orientation = value;
                }
            }
        }
        public override void SetCanvasOrigin(int x, int y)
        {
            //    ReleaseHdc();
            //    //-----------
            //    //move back to original ?
            //    //this.gx.TranslateTransform(-this.canvasOriginX, -this.canvasOriginY);
            //    //this.gx.TranslateTransform(x, y);

            //this.canvasOriginX = x;
            //this.canvasOriginY = y;
            canvasGL2d.SetCanvasOrigin(x, y);
        }
        public override int CanvasOriginX
        {
            get
            {

                return canvasGL2d.CanvasOriginX;
            }
        }
        public override int CanvasOriginY
        {
            get
            {
                return canvasGL2d.CanvasOriginY;
            }
        }

        public override void SetClipRect(Rectangle rect, CombineMode combineMode = CombineMode.Replace)
        {
            canvasGL2d.EnableClipRect();
            //--------------------------
            canvasGL2d.SetClipRectRel(
                 rect.X,
                 rect.Y,
                 rect.Width,
                 rect.Height);
            //--------------------------
        }

        public override bool IntersectsWith(Rectangle clientRect)
        {
            return clientRect.IntersectsWith(left, top, right, bottom);
        } 
        //---------------------------------------------------
        public override bool PushClipAreaRect(int width, int height,ref Rectangle updateArea)
        {
            this.clipRectStack.Push(currentClipRect);

            System.Drawing.Rectangle intersectResult =
                System.Drawing.Rectangle.Intersect(
                System.Drawing.Rectangle.FromLTRB(updateArea.Left, updateArea.Top, updateArea.Right, updateArea.Bottom),
                new System.Drawing.Rectangle(0, 0, width, height));

            currentClipRect = intersectResult;
            if (intersectResult.Width <= 0 || intersectResult.Height <= 0)
            {
                //not intersect?
                return false;
            }
            else
            {
                updateArea = Conv.ToRect(intersectResult);
                canvasGL2d.EnableClipRect();
                canvasGL2d.SetClipRectRel(currentClipRect.X, currentClipRect.Y, currentClipRect.Width, currentClipRect.Height);
                return true;
            }
        }

        public override void PopClipAreaRect()
        {
            if (clipRectStack.Count > 0)
            {
                currentClipRect = clipRectStack.Pop();
            }


            canvasGL2d.EnableClipRect();
            canvasGL2d.SetClipRectRel(currentClipRect.X, currentClipRect.Y, currentClipRect.Width, currentClipRect.Height);

        }
        public override Rectangle CurrentClipRect
        {
            get
            {
                return currentClipRect.ToRect();
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
        public override Rectangle InvalidateArea
        {
            get
            {
                return invalidateArea;
            }
        }
       
        public override void Invalidate(Rectangle rect)
        {
            invalidateArea = Rectangle.Union(rect, invalidateArea);
            this.IsContentReady = false;
        }
        public override void ResetInvalidateArea()
        {
            this.invalidateArea = Rectangle.Empty;
        }
    }

}