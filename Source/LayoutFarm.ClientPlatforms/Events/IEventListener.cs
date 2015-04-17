// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections.Generic;


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
        void ListenLostMouseFocus(UIMouseEventArgs e);

        void ListenDragRelease(UIMouseEventArgs e);
        //--------------------------------------------------------------------------

        void ListenMouseClick(UIMouseEventArgs e);
        void ListenMouseDoubleClick(UIMouseEventArgs e);
        //--------------------------------------------------------------------------
        void ListenGotKeyboardFocus(UIFocusEventArgs e);
        void ListenLostKeyboardFocus(UIFocusEventArgs e);
        //--------------------------------------------------------------------------

        void HandleContentLayout();
        void HandleContentUpdate();
        void HandleElementUpdate();

        //--------------------------------------------------------------------------
        bool BypassAllMouseEvents { get; }
        bool AutoStopMouseEventPropagation { get; }

        void ListenInterComponentMsg(object sender, int msgcode, string msg);
        //--------------------------------------------------------------------------
        void ListenOnDrop(UIMouseEventArgs e);
        void ListenDragOver(UIDragOverEventArgs e);

        //--------------------------------------------------------------------------


    }
}