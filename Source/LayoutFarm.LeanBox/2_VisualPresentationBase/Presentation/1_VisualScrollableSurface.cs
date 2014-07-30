//2014 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using System.Drawing;

using LayoutFarm.Presentation;

namespace LayoutFarm.Presentation
{

    public abstract class VisualScrollableSurface
    {
        public event EventHandler<ScrollSurfaceRequestEventArgs> VScrollRequest;
        public event EventHandler<ScrollSurfaceRequestEventArgs> HScrollRequest;
        public event EventHandler<ArtScrollEventArgs> VScrollChanged;
        public event EventHandler<ArtScrollEventArgs> HScrollChanged;
        ArtVisualContainerBase ownerVisualElement;
        public VisualScrollableSurface(ArtVisualContainerBase ownerVisualElement)
        {
            this.ownerVisualElement = ownerVisualElement;
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
        public void RaiseProperEvents(ArtScrollEventArgs hScrollEventArgs, ArtScrollEventArgs vScrollEventArgs)
        {
            if (this.VScrollChanged != null && vScrollEventArgs != null)
            {
                VScrollChanged.Invoke(ownerVisualElement, vScrollEventArgs);
            }
            if (this.HScrollChanged != null && hScrollEventArgs != null)
            {
                HScrollChanged.Invoke(ownerVisualElement, hScrollEventArgs);
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
        public abstract void DrawToThisPage(ArtCanvas destPage, InternalRect updateArea);
        //------------------------------------
    }


  


}