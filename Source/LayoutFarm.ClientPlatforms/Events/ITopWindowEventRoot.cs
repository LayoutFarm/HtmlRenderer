// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections.Generic;


namespace LayoutFarm.UI
{
    public interface ITopWindowEventRoot
    {
        IEventListener CurrentKeyboardFocusedElement { get; set; }
        MouseCursorStyle MouseCursorStyle { get; }
        

        void RootMouseDown(int x, int y, int button);
        void RootMouseUp(int x, int y, int button);
        void RootMouseWheel(int delta);
        void RootMouseMove(int x, int y, int button);
        void RootGotFocus();
        void RootLostFocus();
        void RootKeyPress(char c);
        void RootKeyDown(int keydata);
        void RootKeyUp(int keydata);
        bool RootProcessDialogKey(int keydata);

    }
}