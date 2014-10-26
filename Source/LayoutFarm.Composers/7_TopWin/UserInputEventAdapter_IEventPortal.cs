//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using LayoutFarm.Drawing;

namespace LayoutFarm.UI
{

    partial class UserInputEventAdapter : IUserEventPortal
    {
        //IUserEventPortalImplementation

        int lastestLogicalMouseDownX;
        int lastestLogicalMouseDownY;
        int prevLogicalMouseX;
        int prevLogicalMouseY;
        bool isMouseDown;
        bool isDragging;
        //------------------------------------------------------------
        void IUserEventPortal.PortalMouseDown(UIMouseEventArgs e)
        {
            this.isMouseDown = true;
            this.lastestLogicalMouseDownX = e.X;
            this.lastestLogicalMouseDownY = e.Y;
            this.prevLogicalMouseX = e.X;
            this.prevLogicalMouseY = e.Y;
            this.OnMouseDown(e);
        }
        void IUserEventPortal.PortalMouseUp(UIMouseEventArgs e)
        {
            e.IsDragging = this.isDragging;
            this.isDragging = this.isMouseDown = false;

            this.prevLogicalMouseX = e.X;
            this.prevLogicalMouseY = e.Y;
            this.OnMouseUp(e);
        }
        void IUserEventPortal.PortalMouseMove(UIMouseEventArgs e)
        {
            this.isDragging = e.IsDragging = this.isMouseDown;
            //find diff    
            e.SetDiff(
                (e.X) - prevLogicalMouseX,
                (e.Y) - prevLogicalMouseY);

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