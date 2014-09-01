//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Drawing;



namespace LayoutFarm.Presentation
{
    partial class ArtUIElement
    {
        void IEventDispatcher.DispatchKeyPressEvent(ArtKeyPressEventArgs args)
        {
            OnKeyPress(args);
        }
        void IEventDispatcher.DispatchKeyEvent(UIKeyEventName keyEventName, ArtKeyEventArgs args)
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
        bool IEventDispatcher.DispatchProcessDialogKey(ArtKeyEventArgs e)
        {
            return OnProcessDialogKey(e);
        }
        void IEventDispatcher.DispatchMouseEvent(UIMouseEventName evName, ArtMouseEventArgs e)
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
        void IEventDispatcher.DispatchDragEvent(UIDragEventName evName, ArtDragEventArgs e)
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

    }
}