//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using LayoutFarm.Drawing;

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
            if (e.IsDragging)
            {
                if (e.JustEnter)
                {
                    OnDragStart(e);
                }
                else
                {
                    OnDragging(e);
                }
            }
            else
            {
                OnMouseMove(e);
            }
        }
        void IEventListener.ListenMouseUp(UIMouseEventArgs e)
        {
            if (e.IsDragging)
            {
                OnDragStop(e);
            }
            else
            {
                OnMouseUp(e);
            }
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
            if (e.IsDragging)
            {
                OnDragLeave(e);
            }
            else
            {
                OnMouseLeave(e);
            }
        }
        void IEventListener.ListenGotFocus(UIFocusEventArgs e)
        {
            OnGotFocus(e);
        }
        void IEventListener.ListenLostFocus(UIFocusEventArgs e)
        {
            OnLostFocus(e);
        }

    }
}