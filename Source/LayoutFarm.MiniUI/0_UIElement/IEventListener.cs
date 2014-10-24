//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using LayoutFarm.Drawing;

namespace LayoutFarm.UI
{

    public interface IEventListener
    {
        //--------------------------------------------------------------------------
        void ListenKeyPressEvent(UIKeyEventArgs args);
        void ListenKeyEvent(UIKeyEventName keyEventName, UIKeyEventArgs e);
        bool ListenProcessDialogKey(UIKeyEventArgs args);
        void ListenMouseEvent(UIMouseEventName mouseEventName, UIMouseEventArgs e);
        void ListenDragEvent(UIDragEventName dragEventName, UIMouseEventArgs e);
        void ListenFocusEvent(UIFocusEventName focusEventName, UIFocusEventArgs e); 
        //--------------------------------------------------------------------------
        bool AcceptKeyboardFocus { get; } 
    } 
}