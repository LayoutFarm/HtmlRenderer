// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;

namespace LayoutFarm.UI
{

    partial class UserInputEventAdapter : IUserEventPortal
    {
        int latestLogicalMouseDownX;
        int latestLogicalMouseDownY;
        int prevLogicalMouseX;
        int prevLogicalMouseY;
        IEventListener draggingElement;

        //------------------------------------------------------------
        void IUserEventPortal.PortalMouseDown(UIMouseEventArgs e)
        {

            this.latestLogicalMouseDownX = e.X;
            this.latestLogicalMouseDownY = e.Y;
            this.prevLogicalMouseX = e.X;
            this.prevLogicalMouseY = e.Y;

            //auto pause? 
            this.OnMouseDown(e);


        }
        void IUserEventPortal.PortalMouseUp(UIMouseEventArgs e)
        {

            this.prevLogicalMouseX = e.X;
            this.prevLogicalMouseY = e.Y;

            if (draggingElement != null)
            {
                //notify release drag?
                draggingElement.ListenDragRelease(e);
            }

            this.OnMouseUp(e);

        }
        void IUserEventPortal.PortalMouseMove(UIMouseEventArgs e)
        {

            //find diff    
            e.SetDiff(
                e.X - prevLogicalMouseX,
                e.Y - prevLogicalMouseY);

            if (e.XDiff == 0 && e.YDiff == 0)
            {
                return;
            }

            this.prevLogicalMouseX = e.X;
            this.prevLogicalMouseY = e.Y;

            this.OnMouseMove(e);

            //registered dragging element
            draggingElement = e.DraggingElement;

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