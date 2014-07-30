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
        void DispatchKeyPressEvent(ArtKeyPressEventArgs args);
        void DispatchKeyEvent(UIKeyEventName keyEventName, ArtKeyEventArgs e);
        bool DispatchProcessDialogKey(ArtKeyEventArgs args);
        void DispatchMouseEvent(UIMouseEventName mouseEventName, ArtMouseEventArgs e);
        void DispatchDragEvent(UIDragEventName dragEventName, ArtDragEventArgs e);
    }


}