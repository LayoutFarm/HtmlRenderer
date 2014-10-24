//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using LayoutFarm.Drawing;

namespace LayoutFarm.UI
{
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
        void PortalClick(UIMouseEventArgs e);
        void PortalDoubleClick(UIMouseEventArgs e);
        //---------------------------------------------- 
        void PortalGotFocus(UIFocusEventArgs e);
        void PortalLostFocus(UIFocusEventArgs e);
        //---------------------------------------------- 
    }

}