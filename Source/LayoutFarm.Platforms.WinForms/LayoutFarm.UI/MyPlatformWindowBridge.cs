//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using LayoutFarm.Drawing;

namespace LayoutFarm.UI
{

    partial class MyPlatformWindowBridge
    {
        CanvasEventsStock eventStock = new CanvasEventsStock();
        CanvasViewport canvasViewport;
        IUserEventPortal userEventPortal;
        TopWindowRenderBox topwin;
        Control windowControl;

        bool isDragging;
        bool isMouseDown;
        MouseCursorStyle currentCursorStyle = MouseCursorStyle.Default;

        public event EventHandler<ScrollSurfaceRequestEventArgs> VScrollRequest;
        public event EventHandler<ScrollSurfaceRequestEventArgs> HScrollRequest;
        public event EventHandler<UIScrollEventArgs> VScrollChanged;
        public event EventHandler<UIScrollEventArgs> HScrollChanged;

        RootGraphic rootGraphic;




        public MyPlatformWindowBridge(TopWindowRenderBox topwin, IUserEventPortal winEventBridge)
        {




            this.userEventPortal = winEventBridge;
            this.topwin = topwin;
            this.rootGraphic = topwin.Root;

            topwin.CanvasForcePaint += (s, e) =>
            {
                PaintToOutputWindow();
            };

        }
        public void BindWindowControl(Control windowControl)
        {
            this.topwin.MakeCurrent();
            this.windowControl = windowControl;
            this.canvasViewport = new CanvasViewport(topwin, this.Size.ToSize(), 4);

#if DEBUG
            this.canvasViewport.dbugOutputWindow = this;
#endif
            this.EvaluateScrollbar();
        }
        void PaintToOutputWindow()
        {
            IntPtr hdc = DrawingBridge.Win32Utils.GetDC(this.windowControl.Handle);
            canvasViewport.PaintMe(hdc);
            DrawingBridge.Win32Utils.ReleaseDC(this.windowControl.Handle, hdc);
        }

        void PaintToOutputWindowIfNeed()
        {

            if (!this.canvasViewport.IsQuadPageValid)
            {
                IntPtr hdc = DrawingBridge.Win32Utils.GetDC(this.windowControl.Handle);
                canvasViewport.PaintMe(hdc);
                DrawingBridge.Win32Utils.ReleaseDC(this.windowControl.Handle, hdc);
            }

        }

        public void UpdateCanvasViewportSize(int w, int h)
        {
            this.canvasViewport.UpdateCanvasViewportSize(w, h);
        }


        System.Drawing.Size Size
        {
            get { return this.windowControl.Size; }
        }


        static UIMouseButtons GetUIMouseButton(MouseButtons mouseButton)
        {
            switch (mouseButton)
            {
                case MouseButtons.Right:
                    return UIMouseButtons.Right;
                case MouseButtons.Middle:
                    return UIMouseButtons.Middle;
                case MouseButtons.None:
                    return UIMouseButtons.None;
                default:
                    return UIMouseButtons.Left;
            }
        }
        void SetUIMouseEventArgsInfo(UIMouseEventArgs mouseEventArg, MouseEventArgs e)
        {
            mouseEventArg.SetEventInfo(
                e.Location.ToPoint(),
                GetUIMouseButton(e.Button),
                e.Clicks,
                e.Delta);
            mouseEventArg.OffsetCanvasOrigin(this.canvasViewport.LogicalViewportLocation);
        }

        public void Close()
        {
            canvasViewport.Close();
        }

        //---------------------------------------------------------------------
        public void EvaluateScrollbar()
        {
            ScrollSurfaceRequestEventArgs hScrollSupportEventArgs;
            ScrollSurfaceRequestEventArgs vScrollSupportEventArgs;
            canvasViewport.EvaluateScrollBar(out hScrollSupportEventArgs, out vScrollSupportEventArgs);
            if (hScrollSupportEventArgs != null)
            {
                viewport_HScrollRequest(this, hScrollSupportEventArgs);
            }
            if (vScrollSupportEventArgs != null)
            {
                viewport_VScrollRequest(this, vScrollSupportEventArgs);
            }
        }
        public void ScrollBy(int dx, int dy)
        {

            UIScrollEventArgs hScrollEventArgs;
            UIScrollEventArgs vScrollEventArgs;
            canvasViewport.ScrollByNotRaiseEvent(dx, dy, out hScrollEventArgs, out vScrollEventArgs);
            if (vScrollEventArgs != null)
            {
                viewport_VScrollChanged(this, vScrollEventArgs);
            }
            if (hScrollEventArgs != null)
            {
                viewport_HScrollChanged(this, hScrollEventArgs);
            }


            PaintToOutputWindow();
        }
        public void ScrollTo(int x, int y)
        {
            Point viewporyLocation = canvasViewport.LogicalViewportLocation;

            if (viewporyLocation.Y == y && viewporyLocation.X == x)
            {
                return;
            }
            UIScrollEventArgs hScrollEventArgs;
            UIScrollEventArgs vScrollEventArgs;
            canvasViewport.ScrollToNotRaiseScrollChangedEvent(x, y, out hScrollEventArgs, out vScrollEventArgs);

            if (vScrollEventArgs != null)
            {
                viewport_VScrollChanged(this, vScrollEventArgs);

            }
            if (hScrollEventArgs != null)
            {
                viewport_HScrollChanged(this, vScrollEventArgs);
            }

            PaintToOutputWindow();
        }

        void viewport_HScrollChanged(object sender, UIScrollEventArgs e)
        {
            if (HScrollChanged != null)
            {
                HScrollChanged.Invoke(sender, e);
            }
        }
        void viewport_HScrollRequest(object sender, ScrollSurfaceRequestEventArgs e)
        {
            if (HScrollRequest != null)
            {
                HScrollRequest.Invoke(sender, e);
            }
        }
        void viewport_VScrollChanged(object sender, UIScrollEventArgs e)
        {
            if (VScrollChanged != null)
            {
                VScrollChanged.Invoke(sender, e);
            }
        }
        void viewport_VScrollRequest(object sender, ScrollSurfaceRequestEventArgs e)
        {
            if (VScrollRequest != null)
            {
                VScrollRequest.Invoke(sender, e);
            }
        }
        public void HandleMouseEnterToViewport()
        {

            //System.Windows.Forms.Cursor.Hide();
        }
        public void HandleMouseLeaveFromViewport()
        {
            System.Windows.Forms.Cursor.Show();
        }
        public void HandleGotFocus(EventArgs e)
        {
            UIFocusEventArgs focusEventArg = eventStock.GetFreeFocusEventArgs(null, null);
            canvasViewport.FullMode = false;
            focusEventArg.OffsetCanvasOrigin(canvasViewport.LogicalViewportLocation);
            this.userEventPortal.PortalGotFocus(focusEventArg);
            PaintToOutputWindowIfNeed();

            eventStock.ReleaseEventArgs(focusEventArg);
        }
        public void HandleLostFocus(EventArgs e)
        {
            UIFocusEventArgs focusEventArg = eventStock.GetFreeFocusEventArgs(null, null);
            canvasViewport.FullMode = false;
            focusEventArg.OffsetCanvasOrigin(canvasViewport.LogicalViewportLocation);

            this.userEventPortal.PortalLostFocus(focusEventArg);
            eventStock.ReleaseEventArgs(focusEventArg);
        }
        UIMouseEventArgs GetReadyMouseEventArgs(MouseEventArgs e)
        {
            UIMouseEventArgs mouseEventArg = eventStock.GetFreeMouseEventArgs(this.topwin);
            SetUIMouseEventArgsInfo(mouseEventArg, e);

            this.isDragging = mouseEventArg.IsMouseDown = this.isMouseDown;
            return mouseEventArg;
        }
        void ReleaseMouseEvent(UIMouseEventArgs e)
        {
            eventStock.ReleaseEventArgs(e);
        }
        //------------------------------------------------------------------------
        public void HandleMouseDown(MouseEventArgs e)
        {
            this.isMouseDown = true;
            this.isDragging = false;

            this.topwin.MakeCurrent();
            canvasViewport.FullMode = false;

            UIMouseEventArgs mouseEventArg = GetReadyMouseEventArgs(e);

            this.userEventPortal.PortalMouseDown(mouseEventArg);
            if (currentCursorStyle != mouseEventArg.MouseCursorStyle)
            {
                //change cursor if need
                ChangeCursorStyle(mouseEventArg);
            }
            ReleaseMouseEvent(mouseEventArg);
            //----------- 
            PaintToOutputWindowIfNeed();
            //---------------
#if DEBUG
            RootGraphic visualroot = this.topwin.dbugVRoot;
            if (visualroot.dbug_RecordHitChain)
            {
                dbug_rootDocHitChainMsgs.Clear();
                visualroot.dbug_DumpCurrentHitChain(dbug_rootDocHitChainMsgs);
                dbug_InvokeHitChainMsg();
            }
#endif

        }
        public void HandleMouseMove(MouseEventArgs e)
        {

            Point viewLocation = canvasViewport.LogicalViewportLocation;
            UIMouseEventArgs mouseEventArg = GetReadyMouseEventArgs(e);
            if (this.isMouseDown ||
                this.isDragging)
            {
                
            }


            this.userEventPortal.PortalMouseMove(mouseEventArg);

            if (currentCursorStyle != mouseEventArg.MouseCursorStyle)
            {
                //change cursor if need
                ChangeCursorStyle(mouseEventArg);
            }
            ReleaseMouseEvent(mouseEventArg);
            PaintToOutputWindowIfNeed();
        }
        public void HandleMouseUp(MouseEventArgs e)
        {

            UIMouseEventArgs mouseEventArg = GetReadyMouseEventArgs(e);
            this.isDragging = this.isMouseDown = false;//reset
            canvasViewport.FullMode = false;

            this.userEventPortal.PortalMouseUp(mouseEventArg);
            if (this.currentCursorStyle != mouseEventArg.MouseCursorStyle)
            {
                //change cursor if need
                ChangeCursorStyle(mouseEventArg);
            }

            ReleaseMouseEvent(mouseEventArg);
            PaintToOutputWindowIfNeed();
        }
        public void HandleMouseWheel(MouseEventArgs e)
        {

            UIMouseEventArgs mouseEventArg = GetReadyMouseEventArgs(e);
            canvasViewport.FullMode = true;
            this.userEventPortal.PortalMouseWheel(mouseEventArg);

            ReleaseMouseEvent(mouseEventArg);
            PaintToOutputWindowIfNeed();
        }
        void ChangeCursorStyle(UIMouseEventArgs mouseEventArg)
        {
            switch (mouseEventArg.MouseCursorStyle)
            {
                case MouseCursorStyle.Pointer:
                    {
                        windowControl.Cursor = Cursors.Hand;
                    } break;
                case MouseCursorStyle.IBeam:
                    {
                        windowControl.Cursor = Cursors.IBeam;
                    } break;
                default:
                    {
                        windowControl.Cursor = Cursors.Default;
                    } break;
            }
            this.currentCursorStyle = mouseEventArg.MouseCursorStyle;
        }

        public void PaintMe()
        {
            if (canvasViewport != null)
            {
                //temp ? for debug
                canvasViewport.FullMode = true;
                PaintToOutputWindow();
            }
        }

        public void PaintMe(PaintEventArgs e)
        {
            PaintMe();
        }


        public void HandleKeyDown(KeyEventArgs e)
        {


            this.topwin.MakeCurrent();
            UIKeyEventArgs keyEventArgs = eventStock.GetFreeKeyEventArgs();

            SetKeyData(keyEventArgs, e);

            StopCaretBlink();
            canvasViewport.FullMode = false;
            keyEventArgs.OffsetCanvasOrigin(canvasViewport.LogicalViewportLocation);
#if DEBUG
            topwin.dbugVisualRoot.dbug_PushLayoutTraceMessage("======");
            topwin.dbugVisualRoot.dbug_PushLayoutTraceMessage("KEYDOWN " + (LayoutFarm.UI.UIKeys)e.KeyData);
            topwin.dbugVisualRoot.dbug_PushLayoutTraceMessage("======");
#endif


            this.userEventPortal.PortalKeyDown(keyEventArgs);
            eventStock.ReleaseEventArgs(keyEventArgs);

            PaintToOutputWindowIfNeed();

        }
        void StartCaretBlink()
        {
            this.rootGraphic.CaretStartBlink();
        }
        void StopCaretBlink()
        {
            this.rootGraphic.CaretStopBlink();
        }

        public void HandleKeyUp(KeyEventArgs e)
        {

            this.topwin.MakeCurrent();
            UIKeyEventArgs keyEventArgs = eventStock.GetFreeKeyEventArgs();
            SetKeyData(keyEventArgs, e);


            StopCaretBlink();
            canvasViewport.FullMode = false;
            keyEventArgs.OffsetCanvasOrigin(canvasViewport.LogicalViewportLocation);


            this.userEventPortal.PortalKeyUp(keyEventArgs);
            eventStock.ReleaseEventArgs(keyEventArgs);


            StartCaretBlink();

        }
        static void SetKeyData(UIKeyEventArgs keyEventArgs, KeyEventArgs e)
        {
            keyEventArgs.SetEventInfo((int)e.KeyCode, e.Shift, e.Alt, e.Control);
        }
        public void HandleKeyPress(KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar))
            {
                return;
            }


            UIKeyEventArgs keyPressEventArgs = eventStock.GetFreeKeyPressEventArgs();
            keyPressEventArgs.SetKeyChar(e.KeyChar);


            StopCaretBlink();
#if DEBUG
            topwin.dbugVisualRoot.dbug_PushLayoutTraceMessage("======");
            topwin.dbugVisualRoot.dbug_PushLayoutTraceMessage("KEYPRESS " + e.KeyChar);
            topwin.dbugVisualRoot.dbug_PushLayoutTraceMessage("======");
#endif

            canvasViewport.FullMode = false;
            keyPressEventArgs.OffsetCanvasOrigin(canvasViewport.LogicalViewportLocation);

            this.userEventPortal.PortalKeyPress(keyPressEventArgs);

            eventStock.ReleaseEventArgs(keyPressEventArgs);

            PaintToOutputWindowIfNeed();
        }

        public bool ProcessDialogKey(Keys keyData)
        {

            UIKeyEventArgs keyEventArg = eventStock.GetFreeKeyEventArgs();
            keyEventArg.KeyData = (int)keyData;

            StopCaretBlink();
            canvasViewport.FullMode = false;
            keyEventArg.OffsetCanvasOrigin(canvasViewport.LogicalViewportLocation);

            bool result = this.userEventPortal.PortalProcessDialogKey(keyEventArg);

            eventStock.ReleaseEventArgs(keyEventArg);
            if (result)
            {
                PaintToOutputWindowIfNeed();

            }
            return result;
        }


    }



}
