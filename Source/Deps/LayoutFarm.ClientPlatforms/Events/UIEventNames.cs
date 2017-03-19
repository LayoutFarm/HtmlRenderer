//Apache2, 2014-2017, WinterDev

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