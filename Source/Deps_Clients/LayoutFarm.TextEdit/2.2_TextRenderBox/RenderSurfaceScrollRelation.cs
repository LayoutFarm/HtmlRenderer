//Apache2, 2014-2017, WinterDev

using System;
namespace LayoutFarm.UI
{
    public class RenderSurfaceScrollRelation
    {
        public event EventHandler<ScrollSurfaceRequestEventArgs> VScrollRequest;
        public event EventHandler<ScrollSurfaceRequestEventArgs> HScrollRequest;
        public event EventHandler<UIScrollEventArgs> VScrollChanged;
        public event EventHandler<UIScrollEventArgs> HScrollChanged;
        object owner;
        public RenderSurfaceScrollRelation(object owner)
        {
            this.owner = owner;
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


        public int VerticalSmallChange
        {
            get;
            set;
        }
        public int VerticalLargeChange
        {
            get;
            set;
        }
        public int HorizontalLargeChange
        {
            get;
            set;
        }
        public int HorizontalSmallChange
        {
            get;
            set;
        }
    }
}