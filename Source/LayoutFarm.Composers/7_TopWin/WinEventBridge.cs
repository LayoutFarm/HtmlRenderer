//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using LayoutFarm.Drawing;

namespace LayoutFarm.UI
{

    public class WinEventBridge : IUserEventPortal
    {

        public TopWindowRenderBox topwin;
        UserInputEventBridge userInputEventBridge;
        bool isMouseDown = false;
        bool isDraging = false;
        int prevLogicalMouseX = 0;
        int prevLogicalMouseY = 0;
        int lastestLogicalMouseDownX = 0;
        int lastestLogicalMouseDownY = 0;

        public WinEventBridge(TopWindowRenderBox topwin)
        {
            this.topwin = topwin;
            this.userInputEventBridge = new MyUserInputEventBridge();
            this.userInputEventBridge.Bind(topwin);
        }
        
        //------------------------------------------------------------
        void IUserEventPortal.PortalMouseDown(UIMouseEventArgs e)
        {
            this.lastestLogicalMouseDownX = e.X;
            this.lastestLogicalMouseDownY = e.Y;
            this.prevLogicalMouseX = e.X;
            this.prevLogicalMouseY = e.Y;
            isMouseDown = true;
            isDraging = false;
            userInputEventBridge.OnMouseDown(e);
        }
        void IUserEventPortal.PortalMouseUp(UIMouseEventArgs e)
        {
            this.prevLogicalMouseX = e.X;
            this.prevLogicalMouseY = e.Y;

            isMouseDown = false;
            userInputEventBridge.OnMouseUp(e);
        }
        void IUserEventPortal.PortalMouseMove(UIMouseEventArgs e)
        {    //find diff    
            e.SetDiff(
                (e.X) - prevLogicalMouseX,
                (e.Y) - prevLogicalMouseY);

            userInputEventBridge.OnMouseMove(e);

            this.prevLogicalMouseX = e.X;
            this.prevLogicalMouseY = e.Y;
        }
        void IUserEventPortal.PortalMouseWheel(UIMouseEventArgs e)
        {
            this.userInputEventBridge.OnMouseWheel(e);
        }
        void IUserEventPortal.PortalClick(UIMouseEventArgs e)
        {

        }
        void IUserEventPortal.PortalDoubleClick(UIMouseEventArgs e)
        {

        }
        //------------------------------------------------------------
        void IUserEventPortal.PortalKeyUp(UIKeyEventArgs e)
        {
            userInputEventBridge.OnKeyUp(e);
        }
        void IUserEventPortal.PortalKeyDown(UIKeyEventArgs e)
        {
            userInputEventBridge.OnKeyDown(e);
        }
        void IUserEventPortal.PortalKeyPress(UIKeyEventArgs e)
        {
            userInputEventBridge.OnKeyPress(e);
        }
        bool IUserEventPortal.PortalProcessDialogKey(UIKeyEventArgs e)
        {
            return this.userInputEventBridge.OnProcessDialogKey(e);
        }
        //------------------------------------------------------------
        void IUserEventPortal.PortalGotFocus(UIFocusEventArgs e)
        {
            userInputEventBridge.OnGotFocus(e);
        }
        void IUserEventPortal.PortalLostFocus(UIFocusEventArgs e)
        {
            userInputEventBridge.OnLostFocus(e);
        }

        public void OnDragStart(UIMouseEventArgs e)
        {
            this.userInputEventBridge.OnDragStart(e);
        }
        public void OnDraging(UIMouseEventArgs e)
        {
            this.userInputEventBridge.OnDrag(e);
        }
        public void OnDragStop(UIMouseEventArgs e)
        {
            this.userInputEventBridge.OnDragStop(e);
        }
    }
}