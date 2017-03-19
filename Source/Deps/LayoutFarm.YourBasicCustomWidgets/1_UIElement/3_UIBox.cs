//Apache2, 2014-2017, WinterDev

using System;
using PixelFarm.Drawing;
namespace LayoutFarm.UI
{
    public abstract class UIBox : UIElement, IScrollable, IBoxElement
    {
        int _left;
        int _top;
        int _width;
        int _height;
        bool _hide;
        bool specificWidth;
        bool specificHeight;
        public event EventHandler LayoutFinished;
#if DEBUG
        static int dbugTotalId;
        public readonly int dbugId = dbugTotalId++;
#endif
        public UIBox(int width, int height)
        {
            this._width = width;
            this._height = height;
            //default for box
            this.AutoStopMouseEventPropagation = true;
        }
        public virtual void Focus()
        {
            //make this keyboard focus able
            if (this.HasReadyRenderElement)
            {
                //focus
                this.CurrentPrimaryRenderElement.Root.SetCurrentKeyboardFocus(this.CurrentPrimaryRenderElement);
            }
        }
        public virtual void Blur()
        {
            if (this.HasReadyRenderElement)
            {
                //focus
                this.CurrentPrimaryRenderElement.Root.SetCurrentKeyboardFocus(null);
            }
        }
        public bool HasSpecificWidth
        {
            get { return this.specificWidth; }
            set
            {
                this.specificWidth = value;
                if (this.CurrentPrimaryRenderElement != null)
                {
                    CurrentPrimaryRenderElement.HasSpecificWidth = value;
                }
            }
        }
        public bool HasSpecificHeight
        {
            get { return this.specificHeight; }
            set
            {
                this.specificHeight = value;
                if (this.CurrentPrimaryRenderElement != null)
                {
                    CurrentPrimaryRenderElement.HasSpecificHeight = value;
                }
            }
        }
        protected void RaiseLayoutFinished()
        {
            if (this.LayoutFinished != null)
            {
                this.LayoutFinished(this, EventArgs.Empty);
            }
        }
        public virtual void SetLocation(int left, int top)
        {
            this._left = left;
            this._top = top;
            if (this.HasReadyRenderElement)
            {
                this.CurrentPrimaryRenderElement.SetLocation(left, top);
            }
        }
        public Point GetGlobalLocation()
        {
            if (this.CurrentPrimaryRenderElement != null)
            {
                return this.CurrentPrimaryRenderElement.GetGlobalLocation();
            }
            return new Point(this.Left, this.Top);
        }
        public virtual void SetSize(int width, int height)
        {
            this._width = width;
            this._height = height;
            if (this.HasReadyRenderElement)
            {
                this.CurrentPrimaryRenderElement.SetSize(_width, _height);
            }
        }
        public void SetBounds(int left, int top, int width, int height)
        {
            SetLocation(left, top);
            SetSize(width, height);
        }
        public int Left
        {
            get
            {
                if (this.HasReadyRenderElement)
                {
                    return this.CurrentPrimaryRenderElement.X;
                }
                else
                {
                    return this._left;
                }
            }
        }
        public int Top
        {
            get
            {
                if (this.HasReadyRenderElement)
                {
                    return this.CurrentPrimaryRenderElement.Y;
                }
                else
                {
                    return this._top;
                }
            }
        }
        public Point Position
        {
            get
            {
                if (this.HasReadyRenderElement)
                {
                    return new Point(CurrentPrimaryRenderElement.X, CurrentPrimaryRenderElement.Y);
                }
                else
                {
                    return new Point(this._left, this._top);
                }
            }
        }
        public int Width
        {
            get
            {
                if (this.HasReadyRenderElement)
                {
                    return this.CurrentPrimaryRenderElement.Width;
                }
                else
                {
                    return this._width;
                }
            }
        }
        public int Height
        {
            get
            {
                if (this.HasReadyRenderElement)
                {
                    return this.CurrentPrimaryRenderElement.Height;
                }
                else
                {
                    return this._height;
                }
            }
        }

        public override void InvalidateGraphics()
        {
            if (this.HasReadyRenderElement)
            {
                this.CurrentPrimaryRenderElement.InvalidateGraphics();
            }
        }
        public void InvalidateOuterGraphics()
        {
            if (this.CurrentPrimaryRenderElement != null)
            {
                this.CurrentPrimaryRenderElement.InvalidateGraphicBounds();
            }
        }
        public virtual int ViewportX
        {
            get { return 0; }
        }
        public virtual int ViewportY
        {
            get { return 0; }
        }
        public virtual int ViewportWidth
        {
            get { return this.Width; }
        }
        public virtual int ViewportHeight
        {
            get { return this.Height; }
        }
        public virtual void SetViewport(int x, int y)
        {
        }

        public virtual bool Visible
        {
            get { return !this._hide; }
            set
            {
                this._hide = !value;
                if (this.HasReadyRenderElement)
                {
                    this.CurrentPrimaryRenderElement.SetVisible(value);
                }
            }
        }

        public virtual void PerformContentLayout()
        {
        }
        public virtual int DesiredHeight
        {
            get { return this.Height; }
        }
        public virtual int DesiredWidth
        {
            get { return this.Width; }
        }

        //----------------------------------- 
        public object Tag { get; set; }
        //----------------------------------- 


        protected virtual void Describe(UIVisitor visitor)
        {
            visitor.Attribute("left", this.Left);
            visitor.Attribute("top", this.Top);
            visitor.Attribute("width", this.Width);
            visitor.Attribute("height", this.Height);
        }



        public Rectangle Bounds
        {
            get { return new Rectangle(this.Left, this.Top, this.Width, this.Height); }
        }
        void IBoxElement.ChangeElementSize(int w, int h)
        {
            this.SetSize(w, h);
        }
        int IBoxElement.MinHeight
        {
            get
            {
                //TODO: use mimimum current font height ***
                return this.Height;
            }
        }
    }
}