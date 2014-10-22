//BSD  2014 ,WinterDev

using System;
using System.Text;
using System.Collections.Generic;
using LayoutFarm;
using LayoutFarm.UI;

namespace HtmlRenderer.WebDom
{
    public delegate void HtmlEventHandler(UIEventArgs e);

    partial class DomElement : IEventListener
    {
        bool IEventListener.NeedPreviewBubbleUp { get { return false; } }
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
        bool IEventListener.AcceptKeyboardFocus { get { return false; } }
        //------------------------------------------------------
        public void AttachEvent(UIEventName eventName, HtmlEventHandler handler)
        {
            switch (eventName)
            {
                case UIEventName.MouseDown:
                    {
                        this.evhMouseDown += handler;
                    } break;
                case UIEventName.MouseUp:
                    {
                        this.evhMouseUp += handler;
                    } break;
            }
        }
        public void DetachEvent(UIEventName eventName, HtmlEventHandler handler)
        {
            switch (eventName)
            {
                case UIEventName.MouseDown:
                    {
                        this.evhMouseDown -= handler;
                    } break;
                case UIEventName.MouseUp:
                    {
                        this.evhMouseUp -= handler;
                    } break;
            }

        }
        //-------------------------------------------------------
        protected virtual void OnLostFocus(UIFocusEventArgs e)
        {
        }
        protected virtual void OnLostMouseFocus(UIFocusEventArgs e)
        {
        }
        protected virtual void OnGotFocus(UIFocusEventArgs e)
        {
        }

        protected virtual void OnDoubleClick(UIMouseEventArgs e)
        {


        }
        protected virtual void OnMouseDown(UIMouseEventArgs e)
        {
            if (this.evhMouseDown != null)
            {
                evhMouseDown(e);
            }
        }
        protected virtual void OnMouseWheel(UIMouseEventArgs e)
        {

        }
        protected virtual void OnDragStart(UIDragEventArgs e)
        {

        }
        protected virtual void OnDragEnter(UIDragEventArgs e)
        {

        }
        protected virtual void OnDragOver(UIDragEventArgs e)
        {

        }
        protected virtual void OnDragLeave(UIDragEventArgs e)
        {

        }
        protected virtual void OnDragStop(UIDragEventArgs e)
        {

        }
        protected virtual void OnDragging(UIDragEventArgs e)
        {
        }
        protected virtual void OnDragDrop(UIDragEventArgs e)
        {
        }
        protected virtual void OnCollapsed()
        {
        }
        protected virtual void OnExpanded()
        {

        }
        protected virtual void OnElementLanded()
        {

        }
        protected virtual void OnShown()
        {
        }
        protected virtual void OnHide()
        {
        }

        protected virtual void OnKeyDown(UIKeyEventArgs e)
        {
        }
        protected virtual void OnKeyUp(UIKeyEventArgs e)
        {
        }
        protected virtual void OnKeyPress(UIKeyPressEventArgs e)
        {
        }
        protected virtual bool OnProcessDialogKey(UIKeyEventArgs e)
        {
            return false;
        }

        protected virtual void OnMouseMove(UIMouseEventArgs e)
        {
        }
        protected virtual void OnMouseHover(UIMouseEventArgs e)
        {
        }
        protected virtual void OnMouseUp(UIMouseEventArgs e)
        {
            if (evhMouseUp != null)
            {
                evhMouseUp(e);
            }
        }
        protected virtual void OnMouseEnter(UIMouseEventArgs e)
        {

        }
        protected virtual void OnMouseLeave(UIMouseEventArgs e)
        {
        }
        protected virtual void OnDropInto()
        {

        }
        protected virtual void OnSizeChanged(UISizeChangedEventArgs e)
        {

        }
    }
}