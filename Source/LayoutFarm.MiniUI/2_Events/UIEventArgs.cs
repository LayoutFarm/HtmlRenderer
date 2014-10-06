//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using LayoutFarm.Drawing;


namespace LayoutFarm
{
    public delegate void UIMouseEventHandler(object sender, UIMouseEventArgs e);
    public delegate void UIKeyEventHandler(object sender, UIKeyEventArgs e);
    public delegate void UIKeyPressEventHandler(object sender, UIKeyPressEventArgs e);

    public enum UIMouseEventType
    {
        MouseMove, MouseDown, MouseUp,
        MouseEnter, MouseLeave, MouseWheel,
        DragStart, Drag, DragStop,

    }

    public abstract class UIEventArgs : EventArgs
    {
        int x;
        int y;

        UIEventName evName;
        public UIEventArgs()
        {

        }
        public UIEventName EventName
        {
            get { return this.evName; }
            set { this.evName = value; }
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
        public object CurentContextElement
        {
            get;
            set;
        }

        public bool IsShiftKeyDown
        {
            get;
            set;
        }
        public bool IsAltKeyDown
        {
            get;
            set;
        }
        public bool IsCtrlKeyDown
        {
            get;
            set;
        }
        public Point Location
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


      
        public void OffsetCanvasOrigin(Point p)
        {
            x += p.X;
            y += p.Y;
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



    public class UIMouseEventArgs : UIEventArgs
    {
        public UIMouseButtons Button;
        public int Delta;
        public int Clicks;
        public int XDiff;
        public int YDiff;
        public UIMouseEventType EventType;
        public TopWindowRenderBox WinTop;

        public UIMouseEventArgs()
        {
        }
        public void SetDiff(int xdiff, int ydiff)
        {
            this.XDiff = xdiff;
            this.YDiff = ydiff;
        }
        public void SetEventInfo(Point location, UIMouseButtons button, int clicks, int delta)
        {
            Location = location;

            Button = button;
            Clicks = clicks;
            Delta = delta;
        }
        public override void Clear()
        {
            Button = 0;
            Clicks = 0;
            XDiff = 0;
            YDiff = 0;
            base.Clear();
        }
    }
    public class UIKeyEventArgs : UIEventArgs
    {
        int keyData;
        bool shift, alt, control;
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

        public bool Shift
        {
            get
            {
                return this.shift;
            }
        }
        public bool Alt
        {
            get
            {
                return this.alt;
            }
        }
        public bool Control
        {
            get
            {
                return this.control;
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
            this.shift = shift;
            this.alt = alt;
            this.control = control;
        }

    }
    public class UIKeyPressEventArgs : UIEventArgs
    {

        char c;
        public UIKeyPressEventArgs()
        {

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

    public enum AffectedElementSideFlags
    {
        None = 0,
        Left = 1,
        Top = 1 << 1,
        Right = 1 << 2,
        Bottom = 1 << 3
    }




    public class UISizeChangedEventArgs : UIEventArgs
    {
        AffectedElementSideFlags changeFromSideFlags;
        static Stack<UISizeChangedEventArgs> pool = new Stack<UISizeChangedEventArgs>();
        private UISizeChangedEventArgs(RenderElement sourceElement, int widthDiff, int heightDiff, AffectedElementSideFlags changeFromSideFlags)
        {
            this.SourceHitElement = sourceElement;
            this.Location = new Point(widthDiff, heightDiff);
            this.changeFromSideFlags = changeFromSideFlags;
        }
        public AffectedElementSideFlags ChangeFromSideFlags
        {
            get
            {
                return changeFromSideFlags;
            }
        }
        public static UISizeChangedEventArgs GetFreeOne(RenderElement sourceElement, int widthDiff, int heightDiff, AffectedElementSideFlags changeFromSideFlags)
        {
            if (pool.Count > 0)
            {
                UISizeChangedEventArgs e = pool.Pop();

                e.Location = new Point(widthDiff, heightDiff);
                e.SourceHitElement = sourceElement;
                e.changeFromSideFlags = changeFromSideFlags;
                return e;
            }
            else
            {
                return new UISizeChangedEventArgs(sourceElement, widthDiff, heightDiff, changeFromSideFlags);
            }
        }
        public override void Clear()
        {
            base.Clear();
        }
        public static void ReleaseOne(UISizeChangedEventArgs e)
        {
            e.Clear();
            pool.Push(e);
        }
    }

    public class UIInvalidateEventArgs : UIEventArgs
    {
        public Rectangle InvalidArea;
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

}