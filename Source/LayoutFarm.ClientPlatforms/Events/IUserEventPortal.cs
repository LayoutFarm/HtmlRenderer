// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections.Generic;


namespace LayoutFarm.UI
{
    public interface ITopWindowEventPortal
    {
        void BindRenderElement(object topRenderElement);
        IEventListener CurrentKeyboardFocusedElement { get; set; }
    }


    public interface IUserEventPortal
    {
        //-------------------------------------------- 
        void PortalKeyPress(UIKeyEventArgs e);
        void PortalKeyDown(UIKeyEventArgs e);
        void PortalKeyUp(UIKeyEventArgs e);
        bool PortalProcessDialogKey(UIKeyEventArgs e);
        //----------------------------------------------

        void PortalMouseDown(UIMouseEventArgs e);
        void PortalMouseMove(UIMouseEventArgs e);
        void PortalMouseUp(UIMouseEventArgs e);
        void PortalMouseWheel(UIMouseEventArgs e);
        //---------------------------------------------- 
        void PortalGotFocus(UIFocusEventArgs e);
        void PortalLostFocus(UIFocusEventArgs e);
        //---------------------------------------------- 
    }

    public delegate bool EventPortalAction(IUserEventPortal evPortal);
    public delegate bool EventListenerAction();
}