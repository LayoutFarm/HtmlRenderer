using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
using LayoutFarm.UI;

namespace LayoutFarm
{

    class TopWindowEventRoot : ITopWindowEventRoot
    {
        RootGraphic rootgfx;
        RenderElementEventPortal topWinBoxEventPortal;
        IEventPortal iTopBoxEventPortal;

        CanvasEventsStock eventStock = new CanvasEventsStock();

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
        void ITopWindowEventRoot.RootMouseDown(int x, int y, int button)
        {
            this.prevLogicalMouseX = x;
            this.prevLogicalMouseY = y;
            this.isMouseDown = true;
            this.isDragging = false;

            UIMouseEventArgs e = eventStock.GetFreeMouseEventArgs();
            SetUIMouseEventArgsInfo(e, x, y, 0, button);

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
            eventStock.ReleaseEventArgs(e);
        }
        void ITopWindowEventRoot.RootMouseUp(int x, int y, int button)
        {
            int xdiff = x - prevLogicalMouseX;
            int ydiff = y - prevLogicalMouseY;
            this.prevLogicalMouseX = x;
            this.prevLogicalMouseY = y;


            UIMouseEventArgs e = eventStock.GetFreeMouseEventArgs();
            SetUIMouseEventArgsInfo(e, x, y, 0, button);
            e.SetDiff(xdiff, ydiff);
            //----------------------------------
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
            e.IsAlsoDoubleClick = timediff.Milliseconds < dblClickSense;
            iTopBoxEventPortal.PortalMouseUp(e);

            this.localMouseDownX = this.localMouseDownY = 0;
            this.mouseCursorStyle = e.MouseCursorStyle;
            eventStock.ReleaseEventArgs(e);
        }
        void ITopWindowEventRoot.RootMouseMove(int x, int y, int button)
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

            UIMouseEventArgs e = eventStock.GetFreeMouseEventArgs();
            SetUIMouseEventArgsInfo(e, x, y, 0, button);
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

                    draggingElement.ListenMouseMove(e);
                    return;
                }

                iTopBoxEventPortal.PortalMouseMove(e);
                draggingElement = e.DraggingElement;
            }
            else
            {
                iTopBoxEventPortal.PortalMouseMove(e);
                draggingElement = null;
            }
            //-------------------------------------------------------

            this.mouseCursorStyle = e.MouseCursorStyle;
            eventStock.ReleaseEventArgs(e);

        }
        void ITopWindowEventRoot.RootMouseWheel(int delta)
        {
            UIMouseEventArgs e = eventStock.GetFreeMouseEventArgs();
            SetUIMouseEventArgsInfo(e, 0, 0, 0, delta);

            if (currentMouseActiveElement != null)
            {
                currentMouseActiveElement.ListenMouseWheel(e);
            }
            iTopBoxEventPortal.PortalMouseWheel(e);

            this.mouseCursorStyle = e.MouseCursorStyle;
            eventStock.ReleaseEventArgs(e);
        }
        void ITopWindowEventRoot.RootGotFocus()
        {
            UIFocusEventArgs e = eventStock.GetFreeFocusEventArgs(null, null);
            iTopBoxEventPortal.PortalGotFocus(e);
            eventStock.ReleaseEventArgs(e);
        }
        void ITopWindowEventRoot.RootLostFocus()
        {
            UIFocusEventArgs e = eventStock.GetFreeFocusEventArgs(null, null);
            iTopBoxEventPortal.PortalLostFocus(e);
            eventStock.ReleaseEventArgs(e);
        }
        void ITopWindowEventRoot.RootKeyPress(char c)
        {
            if (currentKbFocusElem == null)
            {
                return;
            }

            StopCaretBlink();

            UIKeyEventArgs e = eventStock.GetFreeKeyPressEventArgs();
            e.SetKeyChar(c);

            e.SourceHitElement = currentKbFocusElem;
            currentKbFocusElem.ListenKeyPress(e);

            iTopBoxEventPortal.PortalKeyPress(e);


            eventStock.ReleaseEventArgs(e);
        }
        void ITopWindowEventRoot.RootKeyDown(int keydata)
        {
            if (currentKbFocusElem == null)
            {
                return;
            }

            UIKeyEventArgs e = eventStock.GetFreeKeyEventArgs();
            SetKeyData(e, keydata);
            StopCaretBlink();


            e.SourceHitElement = currentKbFocusElem;
            currentKbFocusElem.ListenKeyDown(e);

            iTopBoxEventPortal.PortalKeyDown(e);

            eventStock.ReleaseEventArgs(e);
        }

        void ITopWindowEventRoot.RootKeyUp(int keydata)
        {
            if (currentKbFocusElem == null)
            {
                return;
            }

            StopCaretBlink();

            UIKeyEventArgs e = eventStock.GetFreeKeyEventArgs();
            SetKeyData(e, keydata);
            //----------------------------------------------------

            e.SourceHitElement = currentKbFocusElem;
            currentKbFocusElem.ListenKeyUp(e);

            iTopBoxEventPortal.PortalKeyUp(e);
            //----------------------------------------------------
            eventStock.ReleaseEventArgs(e);
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

            UIKeyEventArgs e = eventStock.GetFreeKeyEventArgs();
            e.KeyData = (int)keyData;
            e.SetEventInfo(
                (int)keyData,
                this.lastKeydownWithShift = ((k & UIKeys.Shift) == UIKeys.Shift),
                this.lastKeydownWithAlt = ((k & UIKeys.Alt) == UIKeys.Alt),
                this.lastKeydownWithControl = ((k & UIKeys.Control) == UIKeys.Control));

            bool result = false;

            e.SourceHitElement = currentKbFocusElem;
            result = currentKbFocusElem.ListenProcessDialogKey(e);


            eventStock.ReleaseEventArgs(e);
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