//Apache2, 2014-2017, WinterDev
using System;
using System.Collections.Generic;
using LayoutFarm.UI;
namespace LayoutFarm
{
    class TopWindowEventRoot : ITopWindowEventRoot
    {
        RootGraphic rootgfx;
        RenderElementEventPortal topWinBoxEventPortal;
        IEventPortal iTopBoxEventPortal;
        IEventListener currentKbFocusElem;
        IEventListener currentMouseActiveElement;
        IEventListener latestMouseDown;
        IEventListener draggingElement;
        DateTime lastTimeMouseUp;
        int dblClickSense = 150;//ms         
        UIHoverMonitorTask hoverMonitoringTask;
        MouseCursorStyle mouseCursorStyle;
        bool isMouseDown;
        bool isDragging;
        bool lastKeydownWithControl;
        bool lastKeydownWithAlt;
        bool lastKeydownWithShift;
        int prevLogicalMouseX;
        int prevLogicalMouseY;
        int localMouseDownX;
        int localMouseDownY;
        //-------
        //event stock
        Stack<UIMouseEventArgs> stockMouseEvents = new Stack<UIMouseEventArgs>();
        Stack<UIKeyEventArgs> stockKeyEvents = new Stack<UIKeyEventArgs>();
        Stack<UIFocusEventArgs> stockFocusEvents = new Stack<UIFocusEventArgs>();
        //-------


        public TopWindowEventRoot(RenderElement topRenderElement)
        {
            this.iTopBoxEventPortal = this.topWinBoxEventPortal = new RenderElementEventPortal(topRenderElement);
            this.rootgfx = topRenderElement.Root;
            this.hoverMonitoringTask = new UIHoverMonitorTask(OnMouseHover);
        }

        public IEventListener CurrentKeyboardFocusedElement
        {
            get
            {
                return this.currentKbFocusElem;
            }
            set
            {
                //1. lost keyboard focus
                if (this.currentKbFocusElem != null && this.currentKbFocusElem != value)
                {
                    currentKbFocusElem.ListenLostKeyboardFocus(null);
                }
                //2. keyboard focus
                currentKbFocusElem = value;
            }
        }
        void StartCaretBlink()
        {
            this.rootgfx.CaretStartBlink();
        }
        void StopCaretBlink()
        {
            this.rootgfx.CaretStopBlink();
        }

        MouseCursorStyle ITopWindowEventRoot.MouseCursorStyle
        {
            get { return this.mouseCursorStyle; }
        }
        void ITopWindowEventRoot.RootMouseDown(int x, int y, UIMouseButtons button)
        {
            this.prevLogicalMouseX = x;
            this.prevLogicalMouseY = y;
            this.isMouseDown = true;
            this.isDragging = false;
            UIMouseEventArgs e = GetFreeMouseEvent();
            SetUIMouseEventArgsInfo(e, x, y, button, 0);
            e.PreviousMouseDown = this.latestMouseDown;
            iTopBoxEventPortal.PortalMouseDown(e);
            this.currentMouseActiveElement = this.latestMouseDown = e.CurrentContextElement;
            this.localMouseDownX = e.X;
            this.localMouseDownY = e.Y;
            if (e.DraggingElement != null)
            {
                if (e.DraggingElement != e.CurrentContextElement)
                {
                    //change captured element
                    int globalX, globalY;
                    e.DraggingElement.GetGlobalLocation(out globalX, out globalY);
                    //find new capture pos
                    this.localMouseDownX = e.GlobalX - globalX;
                    this.localMouseDownY = e.GlobalY - globalY;
                }
                this.draggingElement = e.DraggingElement;
            }
            else
            {
                this.draggingElement = this.currentMouseActiveElement;
            }


            this.mouseCursorStyle = e.MouseCursorStyle;
            ReleaseMouseEvent(e);
        }
        void ITopWindowEventRoot.RootMouseUp(int x, int y, UIMouseButtons button)
        {
            int xdiff = x - prevLogicalMouseX;
            int ydiff = y - prevLogicalMouseY;
            this.prevLogicalMouseX = x;
            this.prevLogicalMouseY = y;
            UIMouseEventArgs e = GetFreeMouseEvent();
            SetUIMouseEventArgsInfo(e, x, y, button, 0);
            e.SetDiff(xdiff, ydiff);
            //----------------------------------
            e.IsDragging = isDragging;
            this.isMouseDown = this.isDragging = false;
            DateTime snapMouseUpTime = DateTime.Now;
            TimeSpan timediff = snapMouseUpTime - lastTimeMouseUp;

           
            if (this.isDragging)
            {
                if (draggingElement != null)
                {
                    //send this to dragging element first
                    int d_GlobalX, d_globalY;
                    draggingElement.GetGlobalLocation(out d_GlobalX, out d_globalY);
                    e.SetLocation(e.GlobalX - d_GlobalX, e.GlobalY - d_globalY);
                    e.CapturedMouseX = this.localMouseDownX;
                    e.CapturedMouseY = this.localMouseDownY;
                    var iportal = draggingElement as IEventPortal;
                    if (iportal != null)
                    {
                        iportal.PortalMouseUp(e);
                        if (!e.IsCanceled)
                        {
                            draggingElement.ListenMouseUp(e);
                        }
                    }
                    else
                    {
                        draggingElement.ListenMouseUp(e);
                    }
                }
            }
            else
            {
                e.IsAlsoDoubleClick = timediff.Milliseconds < dblClickSense;  
                iTopBoxEventPortal.PortalMouseUp(e);
            }
            this.lastTimeMouseUp = snapMouseUpTime;

            this.localMouseDownX = this.localMouseDownY = 0;
            this.mouseCursorStyle = e.MouseCursorStyle;
            ReleaseMouseEvent(e);
        }
        void ITopWindowEventRoot.RootMouseMove(int x, int y, UIMouseButtons button)
        {
            int xdiff = x - prevLogicalMouseX;
            int ydiff = y - prevLogicalMouseY;
            this.prevLogicalMouseX = x;
            this.prevLogicalMouseY = y;
            if (xdiff == 0 && ydiff == 0)
            {
                return;
            }

            //-------------------------------------------------------
            //when mousemove -> reset hover!            
            hoverMonitoringTask.Reset();
            hoverMonitoringTask.Enabled = true;
            UIMouseEventArgs e = GetFreeMouseEvent();
            SetUIMouseEventArgsInfo(e, x, y, button, 0);
            e.SetDiff(xdiff, ydiff);
            //-------------------------------------------------------
            e.IsDragging = this.isDragging = this.isMouseDown;
            if (this.isDragging)
            {
                if (draggingElement != null)
                {
                    //send this to dragging element first
                    int d_GlobalX, d_globalY;
                    draggingElement.GetGlobalLocation(out d_GlobalX, out d_globalY);
                    e.SetLocation(e.GlobalX - d_GlobalX, e.GlobalY - d_globalY);
                    e.CapturedMouseX = this.localMouseDownX;
                    e.CapturedMouseY = this.localMouseDownY;
                    var iportal = draggingElement as IEventPortal;
                    if (iportal != null)
                    {
                        iportal.PortalMouseMove(e);
                        if (!e.IsCanceled)
                        {
                            draggingElement.ListenMouseMove(e);
                        }
                    }
                    else
                    {
                        draggingElement.ListenMouseMove(e);
                    }
                }
            }
            else
            {
                iTopBoxEventPortal.PortalMouseMove(e);
                draggingElement = null;
            }
            //-------------------------------------------------------

            this.mouseCursorStyle = e.MouseCursorStyle;
            ReleaseMouseEvent(e);
        }
        void ITopWindowEventRoot.RootMouseWheel(int delta)
        {
            UIMouseEventArgs e = GetFreeMouseEvent();
            SetUIMouseEventArgsInfo(e, 0, 0, 0, delta);
            if (currentMouseActiveElement != null)
            {
                currentMouseActiveElement.ListenMouseWheel(e);
            }
            iTopBoxEventPortal.PortalMouseWheel(e);
            this.mouseCursorStyle = e.MouseCursorStyle;
            ReleaseMouseEvent(e);
        }
        void ITopWindowEventRoot.RootGotFocus()
        {
            UIFocusEventArgs e = GetFreeFocusEvent();
            iTopBoxEventPortal.PortalGotFocus(e);
            ReleaseFocusEvent(e);
        }
        void ITopWindowEventRoot.RootLostFocus()
        {
            UIFocusEventArgs e = GetFreeFocusEvent();
            iTopBoxEventPortal.PortalLostFocus(e);
            ReleaseFocusEvent(e);
        }
        void ITopWindowEventRoot.RootKeyPress(char c)
        {
            if (currentKbFocusElem == null)
            {
                return;
            }

            StopCaretBlink();
            UIKeyEventArgs e = GetFreeKeyEvent();
            e.SetKeyChar(c);
            e.ExactHitObject = e.SourceHitElement = currentKbFocusElem;
            currentKbFocusElem.ListenKeyPress(e);
            iTopBoxEventPortal.PortalKeyPress(e);
            ReleaseKeyEvent(e);
        }
        void ITopWindowEventRoot.RootKeyDown(int keydata)
        {
            if (currentKbFocusElem == null)
            {
                return;
            }

            UIKeyEventArgs e = GetFreeKeyEvent();
            SetKeyData(e, keydata);
            StopCaretBlink();
            e.ExactHitObject = e.SourceHitElement = currentKbFocusElem;
            currentKbFocusElem.ListenKeyDown(e);
            iTopBoxEventPortal.PortalKeyDown(e);
            ReleaseKeyEvent(e);
        }

        void ITopWindowEventRoot.RootKeyUp(int keydata)
        {
            if (currentKbFocusElem == null)
            {
                return;
            }

            StopCaretBlink();
            UIKeyEventArgs e = GetFreeKeyEvent();
            SetKeyData(e, keydata);
            //----------------------------------------------------

            e.ExactHitObject = e.SourceHitElement = currentKbFocusElem;
            currentKbFocusElem.ListenKeyUp(e);
            iTopBoxEventPortal.PortalKeyUp(e);
            //----------------------------------------------------
            ReleaseKeyEvent(e);
            StartCaretBlink();
        }
        bool ITopWindowEventRoot.RootProcessDialogKey(int keyData)
        {
            if (currentKbFocusElem == null)
            {
                return false;
            }


            StopCaretBlink();
            UI.UIKeys k = (UIKeys)keyData;
            UIKeyEventArgs e = GetFreeKeyEvent();
            e.KeyData = (int)keyData;
            e.SetEventInfo(
                (int)keyData,
                this.lastKeydownWithShift = ((k & UIKeys.Shift) == UIKeys.Shift),
                this.lastKeydownWithAlt = ((k & UIKeys.Alt) == UIKeys.Alt),
                this.lastKeydownWithControl = ((k & UIKeys.Control) == UIKeys.Control));
            bool result = false;
            e.ExactHitObject = e.SourceHitElement = currentKbFocusElem;
            result = currentKbFocusElem.ListenProcessDialogKey(e);
            ReleaseKeyEvent(e);
            return result;
        }


        void SetKeyData(UIKeyEventArgs keyEventArgs, int keydata)
        {
            keyEventArgs.SetEventInfo(keydata, lastKeydownWithShift, lastKeydownWithAlt, lastKeydownWithControl);
        }

        void SetUIMouseEventArgsInfo(UIMouseEventArgs mouseEventArg, int x, int y, UIMouseButtons button, int delta)
        {
            mouseEventArg.SetEventInfo(
                x, y,
               (UIMouseButtons)button,
                0,
                delta);
        }
        //--------------------------------------------------------------------
        void OnMouseHover(object sender, EventArgs e)
        {
            return;
            //HitTestCoreWithPrevChainHint(hitPointChain.LastestRootX, hitPointChain.LastestRootY);
            //RenderElement hitElement = this.hitPointChain.CurrentHitElement as RenderElement;
            //if (hitElement != null && hitElement.IsTestable)
            //{
            //    DisableGraphicOutputFlush = true;
            //    Point hitElementGlobalLocation = hitElement.GetGlobalLocation();

            //    UIMouseEventArgs e2 = new UIMouseEventArgs();
            //    e2.WinTop = this.topwin;
            //    e2.Location = hitPointChain.CurrentHitPoint;
            //    e2.SourceHitElement = hitElement;
            //    IEventListener ui = hitElement.GetController() as IEventListener;
            //    if (ui != null)
            //    {
            //        ui.ListenMouseEvent(UIMouseEventName.MouseHover, e2);
            //    }

            //    DisableGraphicOutputFlush = false;
            //    FlushAccumGraphicUpdate();
            //}
            //hitPointChain.SwapHitChain();
            //hoverMonitoringTask.SetEnable(false, this.topwin);
        }
        //------------------------------------------------
        UIFocusEventArgs GetFreeFocusEvent()
        {
            if (this.stockFocusEvents.Count == 0)
            {
                return new UIFocusEventArgs();
            }
            else
            {
                return this.stockFocusEvents.Pop();
            }
        }
        void ReleaseFocusEvent(UIFocusEventArgs e)
        {
            e.Clear();
            this.stockFocusEvents.Push(e);
        }
        UIKeyEventArgs GetFreeKeyEvent()
        {
            if (this.stockKeyEvents.Count == 0)
            {
                return new UIKeyEventArgs();
            }
            else
            {
                return this.stockKeyEvents.Pop();
            }
        }
        void ReleaseKeyEvent(UIKeyEventArgs e)
        {
            e.Clear();
            this.stockKeyEvents.Push(e);
        }
        UIMouseEventArgs GetFreeMouseEvent()
        {
            if (this.stockMouseEvents.Count == 0)
            {
                return new UIMouseEventArgs();
            }
            else
            {
                return this.stockMouseEvents.Pop();
            }
        }
        void ReleaseMouseEvent(UIMouseEventArgs e)
        {
            e.Clear();
            this.stockMouseEvents.Push(e);
        }
    }
}