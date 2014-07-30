//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;

using System.Drawing;

namespace LayoutFarm.Presentation
{

    public class CanvasEventsStock
    {
        Stack<ArtMouseEventArgs> mouseEventsQ = new Stack<ArtMouseEventArgs>();
        Stack<ArtKeyEventArgs> keyEventsQ = new Stack<ArtKeyEventArgs>();
        Stack<ArtKeyPressEventArgs> keyPressEventsQ = new Stack<ArtKeyPressEventArgs>();
        Stack<ArtInvalidatedEventArgs> canvasInvalidatedEventsQ = new Stack<ArtInvalidatedEventArgs>();

        Stack<ArtCaretEventArgs> caretEventQ = new Stack<ArtCaretEventArgs>();
        Stack<ArtCursorEventArgs> cursorEventQ = new Stack<ArtCursorEventArgs>();
        Stack<ArtPopupEventArgs> popEventQ = new Stack<ArtPopupEventArgs>();

        public CanvasEventsStock()
        {
            mouseEventsQ.Push(new ArtMouseEventArgs());
            keyEventsQ.Push(new ArtKeyEventArgs());
            keyPressEventsQ.Push(new ArtKeyPressEventArgs());
            canvasInvalidatedEventsQ.Push(new ArtInvalidatedEventArgs());
            canvasFocusEventsQ.Push(new ArtFocusEventArgs());

            caretEventQ.Push(new ArtCaretEventArgs());
            cursorEventQ.Push(new ArtCursorEventArgs());
            popEventQ.Push(new ArtPopupEventArgs());
        }
        public ArtPopupEventArgs GetFreeCanvasPopupEventArgs()
        {
            if (popEventQ.Count > 0)
            {
                return popEventQ.Pop();
            }
            else
            {
                return new ArtPopupEventArgs();
            }
        }
        public void ReleaseEventArgs(ArtPopupEventArgs e)
        {
            e.Clear();
            popEventQ.Push(e);
        }
        public ArtCursorEventArgs GetFreeCursorEventArgs()
        {
            if (cursorEventQ.Count > 0)
            {
                return cursorEventQ.Pop();
            }
            else
            {
                return new ArtCursorEventArgs();
            }
        }
        public void ReleaseEventArgs(ArtCursorEventArgs e)
        {
            e.Clear();
            cursorEventQ.Push(e);
        }
        public ArtCaretEventArgs GetFreeCaretEventArgs()
        {
            if (caretEventQ.Count > 0)
            {
                return caretEventQ.Pop();
            }
            else
            {
                return new ArtCaretEventArgs();
            }
        }
        public void ReleaseEventArgs(ArtCaretEventArgs e)
        {
            e.Clear();
            caretEventQ.Push(e);
        }

        public ArtInvalidatedEventArgs GetFreeCanvasInvalidatedEventArgs()
        {
            if (canvasInvalidatedEventsQ.Count > 0)
            {
                return canvasInvalidatedEventsQ.Pop();
            }
            else
            {
                return new ArtInvalidatedEventArgs();
            }

        }
        public void ReleaseEventArgs(ArtInvalidatedEventArgs e)
        {
            e.Clear();
            canvasInvalidatedEventsQ.Push(e);
        }
        public ArtMouseEventArgs GetFreeMouseEventArgs()
        {
            if (mouseEventsQ.Count > 0)
            {
                return mouseEventsQ.Pop();
            }
            else
            {
                return new ArtMouseEventArgs();
            }
        }
        public void ReleaseEventArgs(ArtMouseEventArgs e)
        {
            e.Clear();
            mouseEventsQ.Push(e);
        }
        public ArtKeyEventArgs GetFreeKeyEventArgs()
        {
            if (keyEventsQ.Count > 0)
            {
                return keyEventsQ.Pop();
            }
            else
            {
                return new ArtKeyEventArgs();
            }

        }
        public void ReleaseEventArgs(ArtKeyEventArgs e)
        {
            e.Clear();
            keyEventsQ.Push(e);
        }
        public ArtKeyPressEventArgs GetFreeKeyPressEventArgs()
        {
            if (keyPressEventsQ.Count > 0)
            {
                return keyPressEventsQ.Pop();
            }
            else
            {
                return new ArtKeyPressEventArgs();
            }
        }
        public void ReleaseEventArgs(ArtKeyPressEventArgs e)
        {
            e.Clear();
            keyPressEventsQ.Push(e);
        }
        Stack<ArtFocusEventArgs> canvasFocusEventsQ = new Stack<ArtFocusEventArgs>();

        public ArtFocusEventArgs GetFreeFocusEventArgs(ArtVisualElement tobeFocusElement, ArtVisualElement tobeLostFocusElement)
        {
            if (canvasFocusEventsQ.Count > 0)
            {
                ArtFocusEventArgs e = canvasFocusEventsQ.Pop();
                e.ToBeFocusElement = tobeFocusElement;
                e.ToBeLostFocusElement = tobeLostFocusElement;
                return e;
            }
            else
            {
                ArtFocusEventArgs e = new ArtFocusEventArgs();
                e.ToBeFocusElement = tobeFocusElement;
                e.ToBeLostFocusElement = tobeLostFocusElement;
                return e;

            }
        }
        public void ReleaseEventArgs(ArtFocusEventArgs e)
        {
            e.Clear();
            canvasFocusEventsQ.Push(e);

        }


    }


}
