// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections.Generic;
using PixelFarm.Drawing;

namespace LayoutFarm.UI
{
    public abstract partial class UIElement : IEventListener
    {
        int oneBitNativeEventFlags;
        UIElement parentElement;
        public UIElement()
        {
        }

        protected void RegisterNativeEvent(int eventFlags)
        {
            this.oneBitNativeEventFlags |= eventFlags;
        }
        public virtual bool AcceptKeyboardFocus
        {
            get { return false; }
        }
        public abstract RenderElement GetPrimaryRenderElement(RootGraphic rootgfx);
        protected abstract RenderElement CurrentPrimaryRenderElement
        {
            get;
        }
        internal static RenderElement GetCurrentPrimaryRenderElement(UIElement box)
        {
            return box.CurrentPrimaryRenderElement;
        }
        public abstract void InvalidateGraphics();
        public UIElement ParentUI
        {
            get { return this.parentElement; }
            set { this.parentElement = value; }
        }

        //-------------------------------------------------------
        protected virtual void OnShown()
        {
        }
        protected virtual void OnHide()
        {
        }
        protected virtual void OnLostFocus(UIFocusEventArgs e)
        {
        }
        protected virtual void OnLostSelectedFocus(UIMouseEventArgs e)
        {
        }
        protected virtual void OnGotFocus(UIFocusEventArgs e)
        {
        }
        protected virtual void OnDoubleClick(UIMouseEventArgs e)
        {
        }
        //-------------------------------------------------------
        protected virtual void OnMouseDown(UIMouseEventArgs e)
        {

        }
        protected virtual void OnMouseMove(UIMouseEventArgs e)
        {
        }
        protected virtual void OnMouseUp(UIMouseEventArgs e)
        {
        }
        protected virtual void OnMouseEnter(UIMouseEventArgs e)
        {

        }
        protected virtual void OnMouseLeave(UIMouseEventArgs e)
        {
        }
        protected virtual void OnMouseWheel(UIMouseEventArgs e)
        {
        }
        protected virtual void OnMouseHover(UIMouseEventArgs e)
        {
        }

        //------------------------------------------------------------
        protected virtual void OnKeyDown(UIKeyEventArgs e)
        {
        }
        protected virtual void OnKeyUp(UIKeyEventArgs e)
        {
        }
        protected virtual void OnKeyPress(UIKeyEventArgs e)
        {
        }
        protected virtual bool OnProcessDialogKey(UIKeyEventArgs e)
        {
            return false;
        }
      
#if DEBUG
        object dbugTagObject;
        public object dbugTag
        {
            get
            {
                return this.dbugTagObject;
            }
            set
            {
                this.dbugTagObject = value;
            }
        }
#endif
    }
}