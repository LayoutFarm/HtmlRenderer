//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Drawing;

namespace LayoutFarm.Presentation
{
    public enum UIKeyEventName
    {
        KeyDown,
        KeyUp,
        KeyPress,
        ProcessDialogKey
    }
    public enum UIMouseEventName
    {
        Click,
        DoubleClick,
        MouseDown,
        MouseMove,
        MouseUp,
        MouseEnter,
        MouseLeave,
        MouseHover,
        MouseWheel

    }
    public enum UIDragEventName
    {
        DragStart,
        DragStop,
        Dragging
    }

    public interface IEventListener
    {
        void ListenKeyPressEvent(UIKeyPressEventArgs args);
        void ListenKeyEvent(UIKeyEventName keyEventName, UIKeyEventArgs e);
        bool ListenProcessDialogKey(UIKeyEventArgs args);
        void ListenMouseEvent(UIMouseEventName mouseEventName, UIMouseEventArgs e);
        void ListenDragEvent(UIDragEventName dragEventName, UIDragEventArgs e);
    }


}