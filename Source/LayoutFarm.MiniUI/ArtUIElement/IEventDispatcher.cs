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

    public interface IEventDispatcher
    {
        void DispatchKeyPressEvent(UIKeyPressEventArgs args);
        void DispatchKeyEvent(UIKeyEventName keyEventName, UIKeyEventArgs e);
        bool DispatchProcessDialogKey(UIKeyEventArgs args);
        void DispatchMouseEvent(UIMouseEventName mouseEventName, UIMouseEventArgs e);
        void DispatchDragEvent(UIDragEventName dragEventName, UIDragEventArgs e);
    }


}