//Apache2, 2014-2017, WinterDev

using System;
namespace LayoutFarm.UI
{
    public enum UIScrollEventType
    {
        SmallDecrement = 0,
        //
        SmallIncrement = 1,
        //
        LargeDecrement = 2,
        //
        LargeIncrement = 3,
        //
        ThumbPosition = 4,
        //
        ThumbTrack = 5,
        //
        First = 6,
        //
        Last = 7,
        //
        EndScroll = 8,
    }

    public enum UIScrollOrientation
    {
        HorizontalScroll = 0,
        //
        VerticalScroll = 1,
    }

    public class UIScrollEventArgs : EventArgs
    {
        UIScrollEventType eventType;
        int oldValue;
        int newValue;
        UIScrollOrientation orientation;
        public UIScrollEventArgs(UIScrollEventType eventType, int oldValue, int newValue, UIScrollOrientation orientation)
        {
            this.eventType = eventType;
            this.oldValue = oldValue;
            this.newValue = newValue;
            this.orientation = orientation;
        }
        public UIScrollEventArgs(UIScrollEventType eventType, int oldValue, int newValue)
        {
            this.eventType = eventType;
            this.oldValue = oldValue;
            this.newValue = newValue;
        }
        public int NewValue
        {
            get
            {
                return this.newValue;
            }
        }
        public int OldValue
        {
            get
            {
                return this.oldValue;
            }
        }
        public UIScrollEventType Type
        {
            get
            {
                return this.eventType;
            }
        }
    }

    public class ScrollSurfaceRequestEventArgs : EventArgs
    {
        bool need_it = false;
        public ScrollSurfaceRequestEventArgs(bool need)
        {
            need_it = need;
        }
        public bool Need
        {
            get
            {
                return need_it;
            }
        }
    }
}