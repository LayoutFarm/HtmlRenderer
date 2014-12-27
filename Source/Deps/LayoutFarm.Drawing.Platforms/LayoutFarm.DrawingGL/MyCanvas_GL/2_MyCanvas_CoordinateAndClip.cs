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


namespace LayoutFarm.Drawing.DrawingGL
{
    partial class MyCanvasGL
    {
        int left;
        int top;
        int right;
        int bottom;
        //int canvasOriginX = 0;
        //int canvasOriginY = 0;
        Rect invalidateArea = Drawing.Rect.CreateFromLTRB(0, 0, 0, 0);
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
            if (x != 0 || y != 0)
            {
                //this.gx.TranslateTransform(-this.canvasOriginX, -this.canvasOriginY);
                //this.gx.TranslateTransform(x, y); 
                canvasGL2d.SetCanvasOrigin(x, y);
            }
            else
            {
                canvasGL2d.SetCanvasOrigin(x, y);
            }
            //canvasGL2d.SetCanvasOrigin(x, y);
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
            canvasGL2d.SetClipRect(
                 rect.X,
                 rect.Y,
                 rect.Width,
                 rect.Height);
            //--------------------------
        }
        //-------------------------------------------
        public override bool IntersectsWith(Rect clientRect)
        {
            return clientRect.IntersectsWith(left, top, right, bottom);
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