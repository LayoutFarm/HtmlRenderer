// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections.Generic;
using PixelFarm.Drawing;

namespace LayoutFarm.UI
{
    partial class UIElement
    {
        void IEventListener.ListenKeyPress(UIKeyEventArgs e)
        {
            OnKeyPress(e);
        }
        void IEventListener.ListenKeyDown(UIKeyEventArgs e)
        {
            OnKeyDown(e);
        }
        void IEventListener.ListenKeyUp(UIKeyEventArgs e)
        {
            OnKeyUp(e);
        }
        bool IEventListener.ListenProcessDialogKey(UIKeyEventArgs e)
        {
            return OnProcessDialogKey(e);
        }
        void IEventListener.ListenMouseDown(UIMouseEventArgs e)
        {
            OnMouseDown(e);

        }
        void IEventListener.ListenMouseMove(UIMouseEventArgs e)
        {
            OnMouseMove(e);
        }
        void IEventListener.ListenMouseUp(UIMouseEventArgs e)
        {
            OnMouseUp(e);
        }
        void IEventListener.ListenLostMouseFocus(UIMouseEventArgs e)
        {
            OnLostSelectedFocus(e);
        }
        void IEventListener.ListenMouseClick(UIMouseEventArgs e)
        {
        }
        void IEventListener.ListenMouseDoubleClick(UIMouseEventArgs e)
        {

        }
        void IEventListener.ListenMouseWheel(UIMouseEventArgs e)
        {
            OnMouseWheel(e);
        }
        void IEventListener.ListenMouseLeave(UIMouseEventArgs e)
        {
            OnMouseLeave(e);
        }
        void IEventListener.ListenGotKeyboardFocus(UIFocusEventArgs e)
        {
            OnGotKeyboardFocus(e);
        }
        void IEventListener.ListenLostKeyboardFocus(UIFocusEventArgs e)
        {
            OnLostKeyboardFocus(e);
        }

        void IEventListener.HandleContentLayout()
        {
            OnContentLayout();
        }
        void IEventListener.HandleContentUpdate()
        {
            OnContentUpdate();
        }

        bool IEventListener.BypassAllMouseEvents
        {
            get
            {
                return this.TransparentAllMouseEvents;
            }
        }
        bool IEventListener.AutoStopMouseEventPropagation
        {
            get
            {
                return this.AutoStopMouseEventPropagation;
            }
        }
        void IEventListener.ListenInterComponentMsg(object sender, int msgcode, string msg)
        {
            this.OnInterComponentMsg(sender, msgcode, msg);
        }
    }
}