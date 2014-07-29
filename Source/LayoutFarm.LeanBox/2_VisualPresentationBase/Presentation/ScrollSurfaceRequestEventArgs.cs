//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;



using LayoutFarm.Presentation;

namespace LayoutFarm.Presentation
{

    public enum ArtScrollEventType
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

    public enum ArtScrollOrientation
    {
        HorizontalScroll = 0,
        //
        VerticalScroll = 1,
    }
    public class ArtScrollEventArgs : EventArgs
    {
        ArtScrollEventType eventType;
        int oldValue;
        int newValue;
        ArtScrollOrientation orientation;
        public ArtScrollEventArgs(ArtScrollEventType eventType, int oldValue, int newValue, ArtScrollOrientation orientation)
        {
            this.eventType = eventType;
            this.oldValue = oldValue;
            this.newValue = newValue;
            this.orientation = orientation;
        }
        public ArtScrollEventArgs(ArtScrollEventType eventType, int oldValue, int newValue)
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
        public ArtScrollEventType Type
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