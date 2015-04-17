// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections.Generic;


namespace LayoutFarm.UI
{
    public interface ITopWindowEventPortal
    {
        IEventListener CurrentKeyboardFocusedElement { get; set; }
        MouseCursorStyle MouseCursorStyle { get; }
        

        void PortalMouseDown(int x, int y, int button);
        void PortalMouseUp(int x, int y, int button);
        void PortalMouseWheel(int delta);
        void PortalMouseMove(int x, int y, int button);
        void PortalGotFocus();
        void PortalLostFocus();
        void PortalKeyPress(char c);
        void PortalKeyDown(int keydata);
        void PortalKeyUp(int keydata);

        bool PortalProcessDialogKey(int keydata);

    }
}