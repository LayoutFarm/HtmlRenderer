//2014,2015 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;


namespace LayoutFarm.UI
{


    public enum UIEventName
    {
        Unknown,
        Click,
        DblClick,
        MouseDown,
        MouseMove,
        MouseUp,
        MouseHover,

        DragBegin,
        Dragging,
        DragEnd,
         

        KeyDown,
        KeyUp,
        KeyPress,

    }
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
    public enum UIFocusEventName
    {
        Focus,
        LossingFocus
    }
}