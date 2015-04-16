// 2015,2014 ,Apache2, WinterDev
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
            this.Ctrl = control;
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
        public bool IsControlCharacter
        {
            get
            {
                return Char.IsControl(c);
            }
        }
        public UIKeys KeyCode
        {
            get
            {
                return (UIKeys)this.KeyData & UIKeys.KeyCode;
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
            x = y = 0;
            this.SourceHitElement = this.CurrentContextElement = null;
            this.Shift = this.Alt = this.Ctrl = this.CancelBubbling = false;

        }

        public object SourceHitElement
        {
            get;
            set;
        }
        public IEventListener CurrentContextElement
        {
            //TODO: review here, ensure set this value 
            get;
            set;
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
        public bool Ctrl
        {
            get;
            set;

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
        List<IEventListener> dragOverElements;

        public UIMouseEventArgs()
        {

        }
        public UIMouseButtons Button { get; private set; }
        public int Delta { get; private set; }
        public int Clicks { get; private set; }
        public int GlobalX { get; private set; }
        public int GlobalY { get; private set; }
        public int XDiff { get; private set; }
        public int YDiff { get; private set; }

        public void SetDiff(int xdiff, int ydiff)
        {
            this.XDiff = xdiff;
            this.YDiff = ydiff;
        }
        public void SetEventInfo(int x, int y, UIMouseButtons button, int clicks, int delta, bool isDragging)
        {
            this.GlobalX = x;
            this.GlobalY = y;
            this.SetLocation(x, y);
            Button = button;
            Clicks = clicks;
            Delta = delta;
            this.IsDragging = isDragging;
        }

        public bool IsFirstMouseEnter
        {
            get;
            set;
        }

        public override void Clear()
        {

            base.Clear();

            this.Button = 0;
            this.Clicks = 0;
            this.XDiff = 0;
            this.YDiff = 0;
            this.MouseCursorStyle = UI.MouseCursorStyle.Default;
            this.IsDragging = false;
            this.DraggingElement = null;
            this.IsFirstMouseEnter = false;

            if (this.dragOverElements != null)
            {
                dragOverElements.Clear();
            }

        }

        public MouseCursorStyle MouseCursorStyle
        {
            get;
            set;
        }

        public bool IsDragging
        {
            get;
            set;
        }

        //-------------------------------------------------------------------
        public IEventListener DraggingElement
        {
            get;
            set;
        }
        public void AddDragOverElement(IEventListener dragOverElement)
        {
            if (dragOverElements == null)
            {
                this.dragOverElements = new List<IEventListener>();
            }
            this.dragOverElements.Add(dragOverElement);
        }
        public IEventListener GetDragOverElement(int index)
        {
            return this.dragOverElements[index];
        }
        public int DragOverElementCount
        {
            get { return this.dragOverElements.Count; }
        }

        public IEventListener CurrentMouseActive
        {
            get;
            set;
        }
        public IEventListener PreviousMouseDown
        {
            get;
            set;
        }
        public bool IsAlsoDoubleClick { get; set; }
        
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



    public class UIDragOverEventArgs : UIEventArgs
    {

    }
}