// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text; 
using PixelFarm.Drawing;
using LayoutFarm.RenderBoxes;

namespace LayoutFarm.UI
{

    class CanvasEventsStock
    {
        Stack<UIMouseEventArgs> mouseEventsQ = new Stack<UIMouseEventArgs>();
        Stack<UIKeyEventArgs> keyEventsQ = new Stack<UIKeyEventArgs>();
        Stack<UIKeyEventArgs> keyPressEventsQ = new Stack<UIKeyEventArgs>(); 
        Stack<UIMouseEventArgs> dragEventQ = new Stack<UIMouseEventArgs>();

        public CanvasEventsStock()
        {
            mouseEventsQ.Push(new UIMouseEventArgs());
            keyEventsQ.Push(new UIKeyEventArgs());
            keyPressEventsQ.Push(new UIKeyEventArgs());
             
            canvasFocusEventsQ.Push(new UIFocusEventArgs());

             
            dragEventQ.Push(new UIMouseEventArgs());
        }
          
        public UIMouseEventArgs GetFreeMouseEventArgs()
        {
            if (mouseEventsQ.Count > 0)
            {
                var mouseE = mouseEventsQ.Pop(); 
                return mouseE;
            }
            else
            {
                var mouseE = new UIMouseEventArgs(); 
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

        public UIKeyEventArgs GetFreeKeyPressEventArgs()
        {
            if (keyPressEventsQ.Count > 0)
            {
                return keyPressEventsQ.Pop();
            }
            else
            {
                return new UIKeyEventArgs();
            }
        }
        public void ReleaseEventArgs(UIKeyEventArgs e)
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
        //public UIMouseEventArgs GetFreeDragEventArgs(Point p,
        //    UIMouseButtons button,
        //    int lastestLogicalViewportMouseDownX,
        //    int lastestLogicalViewportMouseDownY,
        //    int currentLogicalX,
        //    int currentLogicalY,
        //    int lastestXDiff,
        //    int lastestYDiff)
        //{
        //    UIMouseEventArgs e = null;
        //    if (dragEventQ.Count > 0)
        //    {
        //        e = dragEventQ.Pop();
        //    }
        //    else
        //    {
        //        e = new UIMouseEventArgs();
        //    }
        //    e.SetEventInfo(p, button,
        //        lastestLogicalViewportMouseDownX,
        //        lastestLogicalViewportMouseDownY,
        //        currentLogicalX,
        //        currentLogicalY,
        //        lastestXDiff,
        //        lastestYDiff);
        //    return e;
        //}

    }


}
