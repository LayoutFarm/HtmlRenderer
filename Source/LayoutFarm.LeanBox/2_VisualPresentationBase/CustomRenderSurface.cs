//2014 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using System.Drawing;

using LayoutFarm.Presentation;

namespace LayoutFarm.Presentation
{

    public abstract class CustomRenderSurface
    {
        public event EventHandler<ScrollSurfaceRequestEventArgs> VScrollRequest;
        public event EventHandler<ScrollSurfaceRequestEventArgs> HScrollRequest;
        public event EventHandler<UIScrollEventArgs> VScrollChanged;
        public event EventHandler<UIScrollEventArgs> HScrollChanged;
        object owner;
        public CustomRenderSurface(object owner)
        {
            this.owner = owner;
        }

        public abstract bool FullModeUpdate
        {
            get;
            set;
        }
        public bool HasHScrollChanged
        {
            get
            {
                return this.HScrollChanged != null;
            }
        }
        public bool HasVScrollChanged
        {
            get
            {
                return this.VScrollChanged != null;
            }
        }
        public void RaiseProperEvents(UIScrollEventArgs hScrollEventArgs, UIScrollEventArgs vScrollEventArgs)
        {
            if (this.VScrollChanged != null && vScrollEventArgs != null)
            {
                VScrollChanged.Invoke(owner, vScrollEventArgs);
            }
            if (this.HScrollChanged != null && hScrollEventArgs != null)
            {
                HScrollChanged.Invoke(owner, hScrollEventArgs);
            }
        }
        protected void RaiseScrollChangedEvents(bool needVeritcal, bool needHorizontal)
        {
            if (this.HasVScrollChanged)
            {
                VScrollRequest.Invoke(this, new ScrollSurfaceRequestEventArgs(needVeritcal));
            }
            if (this.HasHScrollChanged)
            {
                HScrollRequest.Invoke(this, new ScrollSurfaceRequestEventArgs(needHorizontal));
            }
        }
        public abstract int Width
        {
            get;
        }
        public abstract int Height
        {
            get;
        }
        public abstract void ConfirmSizeChanged();

        public abstract void QuadPagesCalculateCanvas();
        public abstract Size OwnerInnerContentSize
        {
            get;
        }
       
        public abstract int VerticalSmallChange
        {
            get;
        }
        public abstract int VerticalLargeChange
        {
            get;
        }
        public abstract int HorizontalLargeChange
        {
            get;
        }
        public abstract int HorizontalSmallChange
        {
            get;
        }


        public abstract void WindowRootNotifyInvalidArea(InternalRect clientRect);
        public abstract void DrawToThisPage(CanvasBase destPage, InternalRect updateArea);
        //------------------------------------
    }


  


}