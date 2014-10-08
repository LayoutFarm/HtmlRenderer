//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;

using LayoutFarm.Drawing;
 

namespace LayoutFarm.UI
{

    public class CanvasEventsStock
    {
        Stack<UIMouseEventArgs> mouseEventsQ = new Stack<UIMouseEventArgs>();
        Stack<UIKeyEventArgs> keyEventsQ = new Stack<UIKeyEventArgs>();
        Stack<UIKeyPressEventArgs> keyPressEventsQ = new Stack<UIKeyPressEventArgs>();
        Stack<UIInvalidateEventArgs> canvasInvalidatedEventsQ = new Stack<UIInvalidateEventArgs>();

        Stack<UICaretEventArgs> caretEventQ = new Stack<UICaretEventArgs>();
        Stack<UICursorEventArgs> cursorEventQ = new Stack<UICursorEventArgs>();
        Stack<UIPopupEventArgs> popEventQ = new Stack<UIPopupEventArgs>();
        Stack<UIDragEventArgs> dragEventQ = new Stack<UIDragEventArgs>();

        public CanvasEventsStock()
        {
            mouseEventsQ.Push(new UIMouseEventArgs());
            keyEventsQ.Push(new UIKeyEventArgs());
            keyPressEventsQ.Push(new UIKeyPressEventArgs());
            canvasInvalidatedEventsQ.Push(new UIInvalidateEventArgs());
            canvasFocusEventsQ.Push(new UIFocusEventArgs());

            caretEventQ.Push(new UICaretEventArgs());
            cursorEventQ.Push(new UICursorEventArgs());
            popEventQ.Push(new UIPopupEventArgs());

            dragEventQ.Push(new UIDragEventArgs());
        }
        public UIPopupEventArgs GetFreeCanvasPopupEventArgs()
        {
            if (popEventQ.Count > 0)
            {
                return popEventQ.Pop();
            }
            else
            {
                return new UIPopupEventArgs();
            }
        }
        public void ReleaseEventArgs(UIPopupEventArgs e)
        {
            e.Clear();
            popEventQ.Push(e);
        }
        public UICursorEventArgs GetFreeCursorEventArgs()
        {
            if (cursorEventQ.Count > 0)
            {
                return cursorEventQ.Pop();
            }
            else
            {
                return new UICursorEventArgs();
            }
        }
        public void ReleaseEventArgs(UICursorEventArgs e)
        {
            e.Clear();
            cursorEventQ.Push(e);
        }
        public UICaretEventArgs GetFreeCaretEventArgs()
        {
            if (caretEventQ.Count > 0)
            {
                return caretEventQ.Pop();
            }
            else
            {
                return new UICaretEventArgs();
            }
        }
        public void ReleaseEventArgs(UICaretEventArgs e)
        {
            e.Clear();
            caretEventQ.Push(e);
        }

        public UIInvalidateEventArgs GetFreeCanvasInvalidatedEventArgs()
        {
            if (canvasInvalidatedEventsQ.Count > 0)
            {
                return canvasInvalidatedEventsQ.Pop();
            }
            else
            {
                return new UIInvalidateEventArgs();
            }

        }
        public void ReleaseEventArgs(UIInvalidateEventArgs e)
        {
            e.Clear();
            canvasInvalidatedEventsQ.Push(e);
        }
        public UIMouseEventArgs GetFreeMouseEventArgs(TopWindowRenderBox wintop)
        {
            if (mouseEventsQ.Count > 0)
            {
                var mouseE = mouseEventsQ.Pop();
                mouseE.WinTop = wintop;
                return mouseE;
            }
            else
            {
                var mouseE = new UIMouseEventArgs();
                mouseE.WinTop = wintop;
                return mouseE;
            }
        }
        public void ReleaseEventArgs(UIMouseEventArgs e)
        {
            e.Clear();
            mouseEventsQ.Push(e);
        }
        public UIKeyEventArgs GetFreeKeyEventArgs()
        {
            if (keyEventsQ.Count > 0)
            {
                return keyEventsQ.Pop();
            }
            else
            {
                return new UIKeyEventArgs();
            }

        }
        public void ReleaseEventArgs(UIKeyEventArgs e)
        {
            e.Clear();
            keyEventsQ.Push(e);
        }
        public UIKeyPressEventArgs GetFreeKeyPressEventArgs()
        {
            if (keyPressEventsQ.Count > 0)
            {
                return keyPressEventsQ.Pop();
            }
            else
            {
                return new UIKeyPressEventArgs();
            }
        }
        public void ReleaseEventArgs(UIKeyPressEventArgs e)
        {
            e.Clear();
            keyPressEventsQ.Push(e);
        }
        Stack<UIFocusEventArgs> canvasFocusEventsQ = new Stack<UIFocusEventArgs>();

        public UIFocusEventArgs GetFreeFocusEventArgs(RenderElement tobeFocusElement, RenderElement tobeLostFocusElement)
        {
            if (canvasFocusEventsQ.Count > 0)
            {
                UIFocusEventArgs e = canvasFocusEventsQ.Pop();
                e.ToBeFocusElement = tobeFocusElement;
                e.ToBeLostFocusElement = tobeLostFocusElement;
                return e;
            }
            else
            {
                UIFocusEventArgs e = new UIFocusEventArgs();
                e.ToBeFocusElement = tobeFocusElement;
                e.ToBeLostFocusElement = tobeLostFocusElement;
                return e;

            }
        }
        public void ReleaseEventArgs(UIFocusEventArgs e)
        {
            e.Clear();
            canvasFocusEventsQ.Push(e);
        }
        public UIDragEventArgs GetFreeDragEventArgs(Point p,
            UIMouseButtons button,
            int lastestLogicalViewportMouseDownX,
            int lastestLogicalViewportMouseDownY,
            int currentLogicalX,
            int currentLogicalY,
            int lastestXDiff,
            int lastestYDiff)
        {
            UIDragEventArgs e = null;
            if (dragEventQ.Count > 0)
            {
                e = dragEventQ.Pop();
            }
            else
            {
                e = new UIDragEventArgs();
            }
            e.SetEventInfo(p, button,
                lastestLogicalViewportMouseDownX,
                lastestLogicalViewportMouseDownY,
                currentLogicalX,
                currentLogicalY,
                lastestXDiff,
                lastestYDiff);
            return e;
        }
        public void ReleaseEventArgs(UIDragEventArgs e)
        {
            e.Clear();
            dragEventQ.Push(e);
        }
    }


}
