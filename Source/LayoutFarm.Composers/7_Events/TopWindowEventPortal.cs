using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
using LayoutFarm.UI;

namespace LayoutFarm
{

    class TopWindowEventPortal : ITopWindowEventPortal
    {
        RootGraphic rootGraphic;

        CanvasEventsStock eventStock = new CanvasEventsStock();

        IEventListener currentKbFocusElem;
        UserEventPortal userEventPortal;
        IUserEventPortal iuserEventPortal;
        IEventListener currentMouseActiveElement;
        IEventListener latestMouseDown;
        IEventListener currentMouseDown;
        IEventListener draggingElement;//current dragging element
        int localMouseDownX;
        int localMouseDownY;

        DateTime lastTimeMouseUp;
        const int DOUBLE_CLICK_SENSE = 150;//ms         


        UIHoverMonitorTask hoverMonitoringTask;
        MouseCursorStyle mouseCursorStyle;

        bool isMouseDown;
        bool isDragging;

        bool lastKeydownWithControl;
        bool lastKeydownWithAlt;
        bool lastKeydownWithShift;
        int prevLogicalMouseX;
        int prevLogicalMouseY;
        public TopWindowEventPortal()
        {
            this.userEventPortal = new UserEventPortal();
            this.iuserEventPortal = userEventPortal;
            this.hoverMonitoringTask = new UIHoverMonitorTask(OnMouseHover);
        }
        public void BindRenderElement(RenderElement topRenderElement)
        {
            this.userEventPortal.BindTopRenderElement(topRenderElement);
            this.rootGraphic = topRenderElement.Root;
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

        void KeyPress(UIKeyEventArgs e)
        {
            if (currentKbFocusElem != null)
            {
                e.SourceHitElement = currentKbFocusElem;
                currentKbFocusElem.ListenKeyPress(e);
            }
            iuserEventPortal.PortalKeyPress(e);
        }
        void KeyDown(UIKeyEventArgs e)
        {
            if (currentKbFocusElem != null)
            {
                e.SourceHitElement = currentKbFocusElem;
                currentKbFocusElem.ListenKeyDown(e);
            }

            iuserEventPortal.PortalKeyDown(e);
        }

        void KeyUp(UIKeyEventArgs e)
        {
            if (currentKbFocusElem != null)
            {
                e.SourceHitElement = currentKbFocusElem;
                currentKbFocusElem.ListenKeyUp(e);
            }
            iuserEventPortal.PortalKeyUp(e);
        }

        bool ProcessDialogKey(UIKeyEventArgs e)
        {
            bool result = false;
            if (currentKbFocusElem != null)
            {
                e.SourceHitElement = currentKbFocusElem;
                result = currentKbFocusElem.ListenProcessDialogKey(e);
            }
            return result;
        }

        void MouseDown(UIMouseEventArgs e)
        {
            this.isMouseDown = true;
            this.isDragging = false;
            //---------------------
            e.PreviousMouseDown = this.latestMouseDown;

            iuserEventPortal.PortalMouseDown(e);
            this.currentMouseActiveElement = this.currentMouseDown = this.latestMouseDown = e.CurrentContextElement;
            this.localMouseDownX = e.X;
            this.localMouseDownY = e.Y;
            this.draggingElement = this.currentMouseActiveElement;

        }
        void MouseMove(UIMouseEventArgs e)
        {
            e.IsDragging = this.isDragging = this.isMouseDown;
            if (this.isDragging)
            {
                if (draggingElement != null)
                {
                    //send this to dragging element first
                    int d_GlobalX, d_globalY;
                    draggingElement.GetGlobalLocation(out d_GlobalX, out d_globalY);
                    e.SetLocation(e.GlobalX - d_GlobalX, e.GlobalY - d_globalY);
                    draggingElement.ListenMouseMove(e);
                    return;
                }

                e.DraggingElement = this.draggingElement;
                iuserEventPortal.PortalMouseMove(e);
                draggingElement = e.DraggingElement;
            }
            else
            {
                iuserEventPortal.PortalMouseMove(e);
                draggingElement = null;
            }

        }
        void MouseUp(UIMouseEventArgs e)
        {

            e.IsDragging = isDragging;

            this.isMouseDown = this.isDragging = false;

            DateTime snapMouseUpTime = DateTime.Now;
            TimeSpan timediff = snapMouseUpTime - lastTimeMouseUp;


            if (draggingElement != null)
            {
                //notify release drag?
                draggingElement.ListenDragRelease(e);
            }

            this.lastTimeMouseUp = snapMouseUpTime;
            e.IsAlsoDoubleClick = timediff.Milliseconds < DOUBLE_CLICK_SENSE;
            iuserEventPortal.PortalMouseUp(e);
            this.currentMouseDown = null;


        }

        void MouseWheel(UIMouseEventArgs e)
        {
            //only on mouse active element
            if (currentMouseActiveElement != null)
            {
                currentMouseActiveElement.ListenMouseWheel(e);
            }
            iuserEventPortal.PortalMouseWheel(e);
        }

        void GotFocus(UIFocusEventArgs e)
        {
            iuserEventPortal.PortalGotFocus(e);
        }

        void LostFocus(UIFocusEventArgs e)
        {
            iuserEventPortal.PortalLostFocus(e);
        }
        void StartCaretBlink()
        {
            this.rootGraphic.CaretStartBlink();
        }
        void StopCaretBlink()
        {
            this.rootGraphic.CaretStopBlink();
        }


        MouseCursorStyle ITopWindowEventPortal.MouseCursorStyle
        {
            get { return this.mouseCursorStyle; }
        }
        void ITopWindowEventPortal.PortalMouseDown(int x, int y, int button)
        {
            this.prevLogicalMouseX = x;
            this.prevLogicalMouseY = y;

            UIMouseEventArgs mouseEventArg = eventStock.GetFreeMouseEventArgs();
            SetUIMouseEventArgsInfo(mouseEventArg, x, y, 0, button);

            this.MouseDown(mouseEventArg);
            this.mouseCursorStyle = mouseEventArg.MouseCursorStyle;
            eventStock.ReleaseEventArgs(mouseEventArg);
        }
        void ITopWindowEventPortal.PortalMouseUp(int x, int y, int button)
        {
            int xdiff = x - prevLogicalMouseX;
            int ydiff = y - prevLogicalMouseY;
            this.prevLogicalMouseX = x;
            this.prevLogicalMouseY = y;


            UIMouseEventArgs mouseEventArg = eventStock.GetFreeMouseEventArgs();
            SetUIMouseEventArgsInfo(mouseEventArg, x, y, 0, button);
            mouseEventArg.SetDiff(xdiff, ydiff);
            this.MouseUp(mouseEventArg);
            this.mouseCursorStyle = mouseEventArg.MouseCursorStyle;
            eventStock.ReleaseEventArgs(mouseEventArg);
        }
        void ITopWindowEventPortal.PortalMouseMove(int x, int y, int button)
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

            UIMouseEventArgs mouseEventArg = eventStock.GetFreeMouseEventArgs();
            SetUIMouseEventArgsInfo(mouseEventArg, x, y, 0, button);
            mouseEventArg.SetDiff(xdiff, ydiff);
            this.MouseMove(mouseEventArg);
            this.mouseCursorStyle = mouseEventArg.MouseCursorStyle;
            eventStock.ReleaseEventArgs(mouseEventArg);

        }
        void ITopWindowEventPortal.PortalMouseWheel(int delta)
        {
            UIMouseEventArgs mouseEventArg = eventStock.GetFreeMouseEventArgs();
            SetUIMouseEventArgsInfo(mouseEventArg, 0, 0, 0, delta);
            this.MouseWheel(mouseEventArg);
            this.mouseCursorStyle = mouseEventArg.MouseCursorStyle;
            eventStock.ReleaseEventArgs(mouseEventArg);
        }
        void ITopWindowEventPortal.PortalGotFocus()
        {
            UIFocusEventArgs focusEventArg = eventStock.GetFreeFocusEventArgs(null, null);
            this.GotFocus(focusEventArg);
            eventStock.ReleaseEventArgs(focusEventArg);
        }
        void ITopWindowEventPortal.PortalLostFocus()
        {
            UIFocusEventArgs focusEventArg = eventStock.GetFreeFocusEventArgs(null, null);
            this.LostFocus(focusEventArg);
            eventStock.ReleaseEventArgs(focusEventArg);
        }
        void ITopWindowEventPortal.PortalKeyPress(char c)
        {
            StopCaretBlink();
            UIKeyEventArgs keyPressEventArgs = eventStock.GetFreeKeyPressEventArgs();
            keyPressEventArgs.SetKeyChar(c);
            this.KeyPress(keyPressEventArgs);
            eventStock.ReleaseEventArgs(keyPressEventArgs);
        }
        void ITopWindowEventPortal.PortalKeyDown(int keydata)
        {

            UIKeyEventArgs keyEventArgs = eventStock.GetFreeKeyEventArgs();
            SetKeyData(keyEventArgs, keydata);
            StopCaretBlink();
            this.KeyDown(keyEventArgs);
            eventStock.ReleaseEventArgs(keyEventArgs);
        }

        void ITopWindowEventPortal.PortalKeyUp(int keydata)
        {

            StopCaretBlink();

            UIKeyEventArgs keyEventArgs = eventStock.GetFreeKeyEventArgs();
            SetKeyData(keyEventArgs, keydata);
            this.KeyUp(keyEventArgs);
            eventStock.ReleaseEventArgs(keyEventArgs);

            StartCaretBlink();
        }
        bool ITopWindowEventPortal.PortalProcessDialogKey(int keyData)
        {
            StopCaretBlink();
            UI.UIKeys k = (UIKeys)keyData;

            UIKeyEventArgs keyEventArg = eventStock.GetFreeKeyEventArgs();
            keyEventArg.KeyData = (int)keyData;
            keyEventArg.SetEventInfo(
                (int)keyData,
                this.lastKeydownWithShift = ((k & UIKeys.Shift) == UIKeys.Shift),
                this.lastKeydownWithAlt = ((k & UIKeys.Alt) == UIKeys.Alt),
                this.lastKeydownWithControl = ((k & UIKeys.Control) == UIKeys.Control));

            bool result = ProcessDialogKey(keyEventArg);
            eventStock.ReleaseEventArgs(keyEventArg);
            return result;
        }


        void SetKeyData(UIKeyEventArgs keyEventArgs, int keydata)
        {
            keyEventArgs.SetEventInfo(keydata, lastKeydownWithShift, lastKeydownWithAlt, lastKeydownWithControl);
        }

        void SetUIMouseEventArgsInfo(UIMouseEventArgs mouseEventArg, int x, int y, int button, int delta)
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
    }
}