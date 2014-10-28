//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using LayoutFarm.Drawing;


namespace LayoutFarm.UI
{
    public delegate void UIMouseEventHandler(object sender, UIMouseEventArgs e);
    public delegate void UIKeyEventHandler(object sender, UIKeyEventArgs e);
    public delegate void UIKeyPressEventHandler(object sender, UIKeyEventArgs e);

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
        IEventListener draggingElem;


        int lastestLogicalViewportMouseDownX;
        int lastestLogicalViewportMouseDownY;
        int currentLogicalX;
        int currentLogicalY;
        int lastestXDiff;
        int lastestYDiff;

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
            this.draggingElem = null;
            this.MouseCursorStyle = UI.MouseCursorStyle.Default;
            base.Clear();
        }
        public IEventListener DraggingElement
        {
            get { return this.draggingElem; }
            set { this.draggingElem = value; }
        }
        public void SetEventInfo(Point location, UIMouseButtons button, int lastestLogicalViewportMouseDownX,
           int lastestLogicalViewportMouseDownY,
           int currentLogicalX,
           int currentLogicalY,
           int lastestXDiff,
           int lastestYDiff)
        {

            Button = button;
            this.Location = location;

            this.currentLogicalX = currentLogicalX;
            this.currentLogicalY = currentLogicalY;
            this.lastestLogicalViewportMouseDownY = lastestLogicalViewportMouseDownY;
            this.lastestLogicalViewportMouseDownX = lastestLogicalViewportMouseDownX;
            this.lastestXDiff = lastestXDiff;
            this.lastestYDiff = lastestYDiff;
        }

        public bool IsDragging { get; set; }
        public bool JustEnter { get; set; }
        public int XDiffFromMouseDownPos
        {
            get
            {
                return this.currentLogicalX - this.lastestLogicalViewportMouseDownX;
            }
        }
        public int YDiffFromMouseDownPos
        {
            get
            {
                return this.currentLogicalY - this.lastestLogicalViewportMouseDownY;
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