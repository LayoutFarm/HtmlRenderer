//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using LayoutFarm.Drawing; 

namespace LayoutFarm.UI
{
    partial class UIElement
    {

        void IEventListener.ListenKeyPressEvent(UIKeyPressEventArgs args)
        {
            OnKeyPress(args);
        }
        void IEventListener.ListenKeyEvent(UIKeyEventName keyEventName, UIKeyEventArgs args)
        {
            switch (keyEventName)
            {
                case UIKeyEventName.KeyDown:
                    {
                        OnKeyDown(args);
                    } break;
                case UIKeyEventName.KeyUp:
                    {
                        OnKeyUp(args);
                    } break;
            }
        }
        bool IEventListener.ListenProcessDialogKey(UIKeyEventArgs e)
        {
            return OnProcessDialogKey(e);
        }
        void IEventListener.ListenMouseEvent(UIMouseEventName evName, UIMouseEventArgs e)
        {
            switch (evName)
            {
                case UIMouseEventName.Click:
                    {

                    } break;
                case UIMouseEventName.DoubleClick:
                    {
                        OnDoubleClick(e);
                    } break;
                case UIMouseEventName.MouseDown:
                    {
                        OnMouseDown(e);
                    } break;
                case UIMouseEventName.MouseMove:
                    {
                        OnMouseMove(e);
                    } break;
                case UIMouseEventName.MouseUp:
                    {
                        OnMouseUp(e);
                    } break;
                case UIMouseEventName.MouseWheel:
                    {
                        OnMouseWheel(e);
                    } break;
            }
        }
        void IEventListener.ListenDragEvent(UIDragEventName evName, UIDragEventArgs e)
        {
            switch (evName)
            {
                case UIDragEventName.Dragging:
                    {
                        OnDragging(e);
                    } break;
                case UIDragEventName.DragStart:
                    {
                        OnDragStart(e);
                    } break;
                case UIDragEventName.DragStop:
                    {
                        OnDragStop(e);
                    } break;
            }
        }

        void IEventListener.ListenFocusEvent(UIFocusEventName evName, UIFocusEventArgs e)
        {
            switch (evName)
            {
                case UIFocusEventName.Focus:
                    {
                         
                        OnGotFocus(e);
                    } break;
                case UIFocusEventName.LossingFocus:
                    {
                        OnLostFocus(e);
                    } break;
            }
        }
        
    }
}