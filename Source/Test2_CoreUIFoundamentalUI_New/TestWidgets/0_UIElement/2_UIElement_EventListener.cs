//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using LayoutFarm.Drawing;

namespace LayoutFarm.UI
{
    partial class UIElement
    {

        bool isMouseDown;
        bool isDragging;

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
            this.isMouseDown = true;
            OnMouseDown(e);
        }


        void IEventListener.ListenMouseMove(UIMouseEventArgs e)
        {

            if (isMouseDown)
            {
                if (isDragging)
                {
                    OnDragging(e);
                }
                else
                {
                    //first time
                    this.isDragging = true;
                    OnDragBegin(e);
                    //store current dragging element
                    currentDraggingElement = this;
                }
            }
            else
            {
                this.isDragging = false;
                OnMouseMove(e);
            }
        }
        void IEventListener.ListenMouseUp(UIMouseEventArgs e)
        {
            if (isDragging)
            {
                //mouse up
                OnDragEnd(e);
            }
            else
            {
                if (currentDraggingElement != null)
                {
                    if (currentDraggingElement != this)
                    {
                        var otherElement = currentDraggingElement as IEventListener;
                        otherElement.ListenMouseUp(e);
                        currentDraggingElement = null;
                    }                    
                }
                OnMouseUp(e);
            }

            this.isDragging = this.isMouseDown = false;
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
            if (isDragging)
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