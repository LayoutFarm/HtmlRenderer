//2014,2015 Apache2, WinterDev
using System;
using System.Collections.Generic;
 

namespace LayoutFarm.UI
{

    public delegate void UIMouseEventHandler(object sender, UIMouseEventArgs e);
    public delegate void UIKeyEventHandler(object sender, UIKeyEventArgs e);
    public delegate void UIKeyPressEventHandler(object sender, UIKeyEventArgs e);

    public class UIKeyEventArgs : UIEventArgs
    {
        int keyData;
        char c;
        public UIKeyEventArgs()
        {
        }
        public int KeyData
        {
            get
            {
                return this.keyData;
            }
            set
            {
                this.keyData = value;
            }
        }


        public bool HasKeyData
        {
            get
            {
                return true;
            }
        }

        public void SetEventInfo(int keydata, bool shift, bool alt, bool control)
        {
            this.keyData = keydata;
            this.Shift = shift;
            this.Alt = alt;
            this.Control = control;
        }
        public void SetKeyChar(char c)
        {
            this.c = c;
        }
        public char KeyChar
        {
            get
            {
                return c;
            }
        }
        public bool IsControlKey
        {
            get
            {
                return Char.IsControl(c);
            }
        }
    }
    public abstract class UIEventArgs : EventArgs
    {
        int x;
        int y;


        public UIEventArgs()
        {

        }

        public virtual void Clear()
        {
            x = 0;
            y = 0;
            CancelBubbling = false;
        }

        public object SourceHitElement
        {
            get;
            set;
        }
        public IEventListener CurrentContextElement
        {
            get;
            set;
        }

        public bool IsShiftKeyDown
        {
            get { return this.Shift; }
        }
        public bool IsAltKeyDown
        {
            get { return this.Alt; }
        }
        public bool IsCtrlKeyDown
        {
            get { return this.Control; }

        }

        public bool Shift
        {
            get;
            set;
        }
        public bool Alt
        {
            get;
            set;
        }
        public bool Control
        {
            get;
            set;
        }
        
        internal Point Location
        {
            get
            {
                return new Point(x, y);
            }
            set
            {
                x = value.X;
                y = value.Y;
            }
        }
        public void SetLocation(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public int X
        {
            get
            {
                return x;
            }
        }
        public int Y
        {
            get
            {
                return y;
            }
        }

        public void OffsetCanvasOrigin(int dx, int dy)
        {
            x += dx;
            y += dy;
        }
        //-----------------------------------------------
        public bool IsCanceled
        {
            get;
            private set;
        }
        public void StopPropagation()
        {
            this.IsCanceled = true;
        }
        public bool CancelBubbling
        {
            get { return this.IsCanceled; }
            set { this.IsCanceled = value; }
        }
    }


    public enum UIMouseButtons
    {
        Left,
        Right,
        Middle,
        None
    }
    public enum FocusEventType
    {
        PreviewLostFocus,
        PreviewFocus,
        Focus,
        LostFocus
    }
    public class UIFocusEventArgs : UIEventArgs
    {
        object tobeFocusElement;
        object tobeLostFocusElement;
        FocusEventType focusEventType = FocusEventType.PreviewLostFocus;
        public UIFocusEventArgs()
        {
        }

        public FocusEventType FocusEventType
        {
            get
            {
                return focusEventType;
            }
            set
            {
                focusEventType = value;
            }
        }
        public object ToBeFocusElement
        {
            get
            {
                return tobeFocusElement;
            }
            set
            {
                tobeFocusElement = value;
            }
        }
        public object ToBeLostFocusElement
        {
            get
            {
                return tobeLostFocusElement;
            }
            set
            {
                tobeLostFocusElement = value;
            }
        }
        public override void Clear()
        {
            tobeFocusElement = null;
            tobeLostFocusElement = null;
            focusEventType = FocusEventType.PreviewFocus;
            base.Clear();
        }

    }


    public class UIMouseEventArgs : UIEventArgs
    {
        public UIMouseButtons Button;
        public int Delta;
        public int Clicks; 
        public int XDiff;
        public int YDiff; 
        public bool IsMouseDown; 
        int xdiffFromMouseDown;
        int ydiffFromMouseDown; 

        public UIMouseEventArgs()
        {

        } 
        public void SetDiff(int xdiff, int ydiff, int xdiffFromMouseDown, int ydiffFromMouseDown)
        {
            this.XDiff = xdiff;
            this.YDiff = ydiff;
            this.xdiffFromMouseDown = xdiffFromMouseDown;
            this.ydiffFromMouseDown = ydiffFromMouseDown;
        }
        //public void SetEventInfo(Point location, UIMouseButtons button, int clicks, int delta)
        //{
        //    Location = location;
        //    Button = button;
        //    Clicks = clicks;
        //    Delta = delta;
        //}
        public void SetEventInfo(int x, int y, UIMouseButtons button, int clicks, int delta)
        {
            Location = new Point(x, y);
            Button = button;
            Clicks = clicks;
            Delta = delta;
        }
        public override void Clear()
        {
            this.Button = 0;
            this.Clicks = 0;
            this.XDiff = 0;
            this.YDiff = 0; 
            this.MouseCursorStyle = UI.MouseCursorStyle.Default;
            base.Clear();
        }

         

        public int XDiffFromMouseDownPos
        {
            get
            {
                return this.xdiffFromMouseDown;
            }
        }
        public int YDiffFromMouseDownPos
        {
            get
            {
                return this.ydiffFromMouseDown;
            }
        }

        public MouseCursorStyle MouseCursorStyle
        {
            get;
            set;
        }
    }
    public enum MouseCursorStyle
    {
        Default,
        Arrow, //arrow (default)
        Hidden,//hidden cursor
        Pointer, //hand cursor
        IBeam,
        Move,
        EastWest,
        NorthSouth,
        CustomStyle,
    }






    public enum AffectedElementSideFlags
    {
        None = 0,
        Left = 1,
        Top = 1 << 1,
        Right = 1 << 2,
        Bottom = 1 << 3
    }




    //public class UISizeChangedEventArgs : UIEventArgs
    //{
    //    AffectedElementSideFlags changeFromSideFlags;
    //    static Stack<UISizeChangedEventArgs> pool = new Stack<UISizeChangedEventArgs>();
    //    private UISizeChangedEventArgs(RenderElement sourceElement, int widthDiff, int heightDiff, AffectedElementSideFlags changeFromSideFlags)
    //    {
    //        this.SourceHitElement = sourceElement;
    //        this.Location = new Point(widthDiff, heightDiff);
    //        this.changeFromSideFlags = changeFromSideFlags;
    //    }
    //    public AffectedElementSideFlags ChangeFromSideFlags
    //    {
    //        get
    //        {
    //            return changeFromSideFlags;
    //        }
    //    }
    //    public static UISizeChangedEventArgs GetFreeOne(RenderElement sourceElement, int widthDiff, int heightDiff, AffectedElementSideFlags changeFromSideFlags)
    //    {
    //        if (pool.Count > 0)
    //        {
    //            UISizeChangedEventArgs e = pool.Pop();

    //            e.Location = new Point(widthDiff, heightDiff);
    //            e.SourceHitElement = sourceElement;
    //            e.changeFromSideFlags = changeFromSideFlags;
    //            return e;
    //        }
    //        else
    //        {
    //            return new UISizeChangedEventArgs(sourceElement, widthDiff, heightDiff, changeFromSideFlags);
    //        }
    //    }
    //    public override void Clear()
    //    {
    //        base.Clear();
    //    }
    //    public static void ReleaseOne(UISizeChangedEventArgs e)
    //    {
    //        e.Clear();
    //        pool.Push(e);
    //    }
    //}

    public class UIInvalidateEventArgs : UIEventArgs
    {
        //public Rectangle InvalidArea;
        public UIInvalidateEventArgs()
        {
        }
    }

    public class UICaretEventArgs : UIEventArgs
    {
        public bool Visible = false;
        public override void Clear()
        {
            Visible = false;
            base.Clear();
        }
    }
    public class UICursorEventArgs : UIEventArgs
    {

    }
    public class UIPopupEventArgs : UIEventArgs
    {
        public bool pleaseShow = false;
        public object popupWindow;
        public override void Clear()
        {
            pleaseShow = false;
            popupWindow = null;
            base.Clear();
        }
    }

}