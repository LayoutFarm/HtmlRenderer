//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;


namespace LayoutFarm.Presentation
{
    public delegate void ArtMouseEventHandler(object sender, ArtMouseEventArgs e);
    public delegate void ArtKeyEventHandler(object sender, ArtKeyEventArgs e);
    public delegate void ArtKeyPressEventHandler(object sender, ArtKeyPressEventArgs e);

    public enum ArtVisualMouseEventType
    {
        MouseMove, MouseDown, MouseUp,
        MouseEnter, MouseLeave, MouseWheel,
        DragStart, Drag, DragStop,

    }

    public abstract class ArtEventArgs : EventArgs
    {
        int x;
        int y;
        ArtVisualElement sourceVisualElement;
        public bool CancelBubbling = false;


        int canvasXOrigin;
        int canvasYOrigin;

        public ArtEventArgs()
        {

        }

        public virtual void Clear()
        {

            sourceVisualElement = null;
            x = 0;
            y = 0;
            CancelBubbling = false;

            canvasXOrigin = 0;
            canvasYOrigin = 0;
            this.vroot = null;
            this.winRoot = null;
        }
        public ArtVisualElement SourceVisualElement
        {
            get
            {
                return sourceVisualElement;
            }
            set
            {
                sourceVisualElement = value;
            }
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
        protected void SetLocation(int x, int y)
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

        int beforeTranslateOriginX = 0;
        int beforeTranslateOriginY = 0;
        public void TranslateCanvasOrigin(Point newOrigin)
        {
            beforeTranslateOriginX = canvasXOrigin; beforeTranslateOriginY = canvasYOrigin;
            OffsetCanvasOrigin(newOrigin.X - canvasXOrigin, newOrigin.Y - canvasYOrigin);
        }
        public void TranslateCanvasOrigin(int newXOrigin, int newYOrigin)
        {
            beforeTranslateOriginX = canvasXOrigin; beforeTranslateOriginY = canvasYOrigin;
            OffsetCanvasOrigin(newXOrigin - canvasXOrigin, newYOrigin - canvasYOrigin);
        }
        public void TranslateCanvasOriginBack()
        {
            OffsetCanvasOrigin(beforeTranslateOriginX - canvasXOrigin, beforeTranslateOriginY - canvasYOrigin);
        }
        public void OffsetCanvasOrigin(int dx, int dy)
        {
            if (dx != 0 || dy != 0)
            {
                x -= dx; y -= dy; canvasXOrigin += dx;
                canvasYOrigin += dy;
            }

        }
        public VisualRoot VisualRoot
        {
            get
            {
                return vroot;
            }

        }
        public ArtVisualRootWindow WinRoot
        {
            get
            {
                return winRoot;
            }
        }
        public void SetWinRoot(ArtVisualRootWindow winRoot)
        {
            this.winRoot = winRoot;
            this.vroot = winRoot.VisualRoot;
        }

        VisualRoot vroot;
        ArtVisualRootWindow winRoot;

        public VisualElementArgs GetVisualInvalidateCanvasArgs()
        {
            return winRoot.GetVInv();
        }
        public void FreeVisualInvalidateCanvasArgs(VisualElementArgs vinv)
        {
            winRoot.FreeVInv(vinv);
        }
    }


    public enum ArtMouseButtons
    {
        Left,
        Right,
        Middle,
        None
    }
    public class ArtMouseEventArgs : ArtEventArgs
    {
        public ArtMouseButtons Button;
        public int Delta;
        public int Clicks;
        public int XDiff;
        public int YDiff;

        public ArtVisualMouseEventType EventType;
        public ArtMouseEventArgs()
        {
        }
        public void SetDiff(int xdiff, int ydiff)
        {
            this.XDiff = xdiff;
            this.YDiff = ydiff;
        }

        public void SetEventInfo(Point location, ArtMouseButtons button, int clicks, int delta)
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
    public class ArtKeyEventArgs : ArtEventArgs
    {
        int keyData;
        bool shift, alt, control;


        public ArtKeyEventArgs()
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
    public class ArtKeyPressEventArgs : ArtEventArgs
    {

        char c;
        public ArtKeyPressEventArgs()
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




    public class ArtSizeChangedEventArgs : ArtEventArgs
    {
        AffectedElementSideFlags changeFromSideFlags;
        static Stack<ArtSizeChangedEventArgs> pool = new Stack<ArtSizeChangedEventArgs>();
        private ArtSizeChangedEventArgs(ArtVisualElement sourceElement, int widthDiff, int heightDiff, AffectedElementSideFlags changeFromSideFlags)
        {
            this.SourceVisualElement = sourceElement;
            this.SetLocation(widthDiff, heightDiff);
            this.changeFromSideFlags = changeFromSideFlags;
        }
        public AffectedElementSideFlags ChangeFromSideFlags
        {
            get
            {
                return changeFromSideFlags;
            }
        }
        public static ArtSizeChangedEventArgs GetFreeOne(ArtVisualElement sourceElement, int widthDiff, int heightDiff, AffectedElementSideFlags changeFromSideFlags)
        {
            if (pool.Count > 0)
            {
                ArtSizeChangedEventArgs e = pool.Pop();
                e.SetLocation(widthDiff, heightDiff);
                e.SourceVisualElement = sourceElement;
                e.changeFromSideFlags = changeFromSideFlags;
                return e;
            }
            else
            {
                return new ArtSizeChangedEventArgs(sourceElement, widthDiff, heightDiff, changeFromSideFlags);
            }
        }
        public override void Clear()
        {
            base.Clear();
        }
        public static void ReleaseOne(ArtSizeChangedEventArgs e)
        {
            e.Clear();
            pool.Push(e);
        }
    }

    public class ArtInvalidatedEventArgs : ArtEventArgs
    {

        public InternalRect InvalidArea;
        public ArtInvalidatedEventArgs()
        {

        }
    }

    public class ArtCaretEventArgs : ArtEventArgs
    {
        public bool Visible = false;
        public override void Clear()
        {
            Visible = false;
            base.Clear();
        }
    }
    public class ArtCursorEventArgs : ArtEventArgs
    {

    }
    public class ArtPopupEventArgs : ArtEventArgs
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

    public class ArtRectChangeEventArgs : ArtEventArgs
    {
        public int affectedSize = 0;
        public int xdiff = 0;
        public int ydiff = 0;
        public ArtRectChangeEventArgs(int xdiff, int ydiff)
        {
            this.xdiff = xdiff;
            this.ydiff = ydiff;
        }
    }


    public class ArtFocusEventArgs : ArtEventArgs
    {
        ArtVisualElement tobeFocusElement;
        ArtVisualElement tobeLostFocusElement;
        FocusEventType focusEventType = FocusEventType.PreviewLostFocus;
        public ArtFocusEventArgs()
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
        public ArtVisualElement ToBeFocusElement
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
        public ArtVisualElement ToBeLostFocusElement
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
    public class ArtFocusEventArgs2 : ArtEventArgs
    {
        object tobeFocusElement;
        object tobeLostFocusElement;
        FocusEventType focusEventType = FocusEventType.PreviewLostFocus;
        public ArtFocusEventArgs2()
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


    public class ArtRectHitEventArgs : ArtEventArgs
    {
        Rectangle hitRect; object hitter; ArtMouseButtons button;

        public ArtRectHitEventArgs(
             ArtMouseButtons button,
             object hitter,
             Rectangle hitRect)
        {
            this.hitRect = hitRect;
            this.hitter = hitter;
            this.button = button;
        }
        public ArtMouseButtons Button
        {
            get
            {
                return button;
            }
        }
        public object Hitter
        {
            get
            {
                return hitter;
            }
        }
        public Rectangle HitRect
        {
            get
            {
                return hitRect;
            }
        }
        public Size SuggestedOffset(Rectangle testRect)
        {
            int xOffset;
            int yOffset;
            if (hitRect.Left >= testRect.Left + testRect.Width / 2)
            {
                xOffset = -hitRect.Width;
            }
            else
            {
                xOffset = hitRect.Width;
            }

            if (hitRect.Top >= testRect.Top + testRect.Height / 2)
            {
                yOffset = -hitRect.Height;
            }
            else
            {
                yOffset = hitRect.Height;
            }
            return new Size(xOffset, yOffset);


        }
    }

}