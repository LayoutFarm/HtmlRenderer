//Apache2, 2014-2017, WinterDev

using PixelFarm.Drawing;
namespace LayoutFarm
{
    partial class RenderElement
    {
        //----------------------
        //rectangle boundary area 
        int b_top;
        int b_left;
        int b_width;
        int b_height;
        int uiLayoutFlags;
        //-----------------------------------------------

        public Rectangle RectBounds
        {
            get
            {
                return new Rectangle(b_left, b_top, b_width, b_height);
            }
        }
        public Size Size
        {
            get
            {
                return new Size(b_width, b_height);
            }
        }
        public int X
        {
            get
            {
                return b_left;
            }
        }
        public int Y
        {
            get
            {
                return b_top;
            }
        }
        public int Right
        {
            get
            {
                return b_left + b_width;
            }
        }
        public int Bottom
        {
            get
            {
                return b_top + b_height;
            }
        }
        public Point Location
        {
            get
            {
                return new Point(b_left, b_top);
            }
        }
        public int Width
        {
            get
            {
                return b_width;
            }
        }
        public int Height
        {
            get
            {
                return b_height;
            }
        }


        //-----------------------------------------------
        public virtual int ViewportY
        {
            get
            {
                return 0;
            }
        }
        public virtual int ViewportX
        {
            get
            {
                return 0;
            }
        }
        public int ViewportBottom
        {
            get
            {
                return this.Bottom + this.ViewportY;
            }
        }
        public int ViewportRight
        {
            get
            {
                return this.Right + this.ViewportX;
            }
        }

        public virtual void SetViewport(int viewportX, int viewportY)
        {
            //do nothing
        }
        public virtual Size InnerContentSize
        {
            get { return this.Size; }
        }
        //-----------------------------------------------
        public Point GetGlobalLocation()
        {
            return GetGlobalLocationStatic(this);
        }
        static Point GetGlobalLocationStatic(RenderElement re)
        {
            RenderElement parentVisualElement = re.ParentRenderElement;
            if (parentVisualElement != null)
            {
                Point parentGlobalLocation = GetGlobalLocationStatic(parentVisualElement);
                re.parentLink.AdjustLocation(ref parentGlobalLocation);
                if (parentVisualElement.MayHasViewport)
                {
                    return new Point(
                        re.b_left + parentGlobalLocation.X - parentVisualElement.ViewportX,
                        re.b_top + parentGlobalLocation.Y - parentVisualElement.ViewportY);
                }
                else
                {
                    return new Point(re.b_left + parentGlobalLocation.X, re.b_top + parentGlobalLocation.Y);
                }
            }
            else
            {
                return re.Location;
            }
        }
        //----------------------------------------------- 
        public bool HasSpecificWidth
        {
            get
            {
                return ((uiLayoutFlags & RenderElementConst.LY_HAS_SPC_WIDTH) == RenderElementConst.LY_HAS_SPC_WIDTH);
            }
            set
            {
                uiLayoutFlags = value ?
                   uiLayoutFlags | RenderElementConst.LY_HAS_SPC_WIDTH :
                   uiLayoutFlags & ~RenderElementConst.LY_HAS_SPC_WIDTH;
            }
        }
        public bool HasSpecificHeight
        {
            get
            {
                return ((uiLayoutFlags & RenderElementConst.LY_HAS_SPC_HEIGHT) == RenderElementConst.LY_HAS_SPC_HEIGHT);
            }
            set
            {
                uiLayoutFlags = value ?
                    uiLayoutFlags | RenderElementConst.LY_HAS_SPC_HEIGHT :
                    uiLayoutFlags & ~RenderElementConst.LY_HAS_SPC_HEIGHT;
            }
        }
        public bool HasSpecificSize
        {
            get
            {
                return ((uiLayoutFlags & RenderElementConst.LY_HAS_SPC_SIZE) != 0);
            }
            set
            {
                uiLayoutFlags = value ?
                    uiLayoutFlags | RenderElementConst.LY_HAS_SPC_SIZE :
                    uiLayoutFlags & ~RenderElementConst.LY_HAS_SPC_SIZE;
            }
        }


        public bool Contains(Point testPoint)
        {
            return ((propFlags & RenderElementConst.HIDDEN) != 0) ?
                        false :
                        ContainPoint(testPoint.X, testPoint.Y);
        }
        public bool ContainPoint(int x, int y)
        {
            return ((x >= b_left && x < Right) && (y >= b_top && y < Bottom));
        }
        public bool ContainRect(Rectangle r)
        {
            return r.Left >= b_left &&
                    r.Top >= b_top &&
                    r.Right <= b_left + b_width &&
                    r.Bottom <= b_top + b_height;
        }
        public bool ContainRect(int x, int y, int width, int height)
        {
            return x >= b_left &&
                    y >= b_top &&
                    x + width <= b_left + b_width &&
                    y + height <= b_top + b_height;
        }
        /// <summary>
        /// no rect change
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public bool IntersectsWith(ref Rectangle r)
        {
            int left = this.b_left;
            if (((left <= r.Left) && (this.Right > r.Left)) ||
                ((left >= r.Left) && (left < r.Right)))
            {
                int top = this.b_top;
                return (((top <= r.Top) && (this.Bottom > r.Top)) ||
                          ((top >= r.Top) && (top < r.Bottom)));
            }
            return false;
        }
        /// <summary>
        /// no rect change
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public bool IntersectOnHorizontalWith(ref Rectangle r)
        {
            int left = this.b_left;
            return (((left <= r.Left) && (this.Right > r.Left)) ||
                     ((left >= r.Left) && (left < r.Right)));
        }
    }
}