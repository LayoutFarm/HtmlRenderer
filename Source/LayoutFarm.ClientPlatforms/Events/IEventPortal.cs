// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections.Generic;


namespace LayoutFarm.UI
{


    public interface IEventPortal
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



    public delegate bool EventPortalAction(IEventPortal evPortal);
    public delegate bool EventListenerAction();
}