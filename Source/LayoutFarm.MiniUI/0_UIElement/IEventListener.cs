//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using LayoutFarm.Drawing;

namespace LayoutFarm.UI
{

    public interface IEventListener
    {
        //--------------------------------------------------------------------------
        void ListenKeyPress(UIKeyEventArgs args);
        void ListenKeyDown(UIKeyEventArgs e);
        void ListenKeyUp(UIKeyEventArgs e);
        bool ListenProcessDialogKey(UIKeyEventArgs args);
        //--------------------------------------------------------------------------
        void ListenMouseDown(UIMouseEventArgs e);
        void ListenMouseMove(UIMouseEventArgs e);
        void ListenMouseUp(UIMouseEventArgs e);
        void ListenMouseLeave(UIMouseEventArgs e);
        void ListenMouseWheel(UIMouseEventArgs e);
        //--------------------------------------------------------------------------
        void ListenGotFocus(UIFocusEventArgs e);
        void ListenLostFocus(UIFocusEventArgs e);
        //--------------------------------------------------------------------------
        bool AcceptKeyboardFocus { get; }
    }
}