//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using LayoutFarm.Drawing;

namespace LayoutFarm.UI
{

    partial class UserInputEventAdapter : IUserEventPortal
    {
        int lastestLogicalMouseDownX;
        int lastestLogicalMouseDownY;
        int prevLogicalMouseX;
        int prevLogicalMouseY;

        //------------------------------------------------------------
        void IUserEventPortal.PortalMouseDown(UIMouseEventArgs e)
        {

            this.lastestLogicalMouseDownX = e.X;
            this.lastestLogicalMouseDownY = e.Y;
            this.prevLogicalMouseX = e.X;
            this.prevLogicalMouseY = e.Y;
            this.OnMouseDown(e);
        }
        void IUserEventPortal.PortalMouseUp(UIMouseEventArgs e)
        {

            this.prevLogicalMouseX = e.X;
            this.prevLogicalMouseY = e.Y;

            this.OnMouseUp(e);
        }
        void IUserEventPortal.PortalMouseMove(UIMouseEventArgs e)
        {

            //find diff    
            e.SetDiff(
                e.X - prevLogicalMouseX,
                e.Y - prevLogicalMouseY,
                e.X - this.lastestLogicalMouseDownX,
                e.Y - this.lastestLogicalMouseDownY);

            this.prevLogicalMouseX = e.X;
            this.prevLogicalMouseY = e.Y;
            this.OnMouseMove(e);
        }
        void IUserEventPortal.PortalMouseWheel(UIMouseEventArgs e)
        {
            this.OnMouseWheel(e);
        }

        //------------------------------------------------------------
        void IUserEventPortal.PortalKeyUp(UIKeyEventArgs e)
        {
            this.OnKeyUp(e);
        }
        void IUserEventPortal.PortalKeyDown(UIKeyEventArgs e)
        {
            this.OnKeyDown(e);
        }
        void IUserEventPortal.PortalKeyPress(UIKeyEventArgs e)
        {
            this.OnKeyPress(e);
        }
        bool IUserEventPortal.PortalProcessDialogKey(UIKeyEventArgs e)
        {
            return this.OnProcessDialogKey(e);
        }
        //------------------------------------------------------------
        void IUserEventPortal.PortalGotFocus(UIFocusEventArgs e)
        {
            this.OnGotFocus(e);
        }
        void IUserEventPortal.PortalLostFocus(UIFocusEventArgs e)
        {
            this.OnLostFocus(e);
        }

    }
}