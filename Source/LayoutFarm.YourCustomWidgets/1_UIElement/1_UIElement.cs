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
#if DEBUG
        public bool dbugBreakMe;
#endif
        public UIElement()
        {

        }

        
        public abstract RenderElement GetPrimaryRenderElement(RootGraphic rootgfx);


        protected void RegisterNativeEvent(int eventFlags)
        {
            this.oneBitNativeEventFlags |= eventFlags;
        }


        public bool TransparentAllMouseEvents
        {
            get;
            set;
        }
        public bool AutoStopMouseEventPropagation
        {
            get;
            set;
        }

        public abstract RenderElement CurrentPrimaryRenderElement
        {
            get;
        }
        internal static RenderElement GetCurrentPrimaryRenderElement(UIElement box)
        {
            return box.CurrentPrimaryRenderElement;
        }
        protected abstract bool HasReadyRenderElement
        {
            get;
        }
        public abstract void InvalidateGraphics();
        public UIElement ParentUI
        {
            get { return this.parentElement; }
            set { this.parentElement = value; }
        }


        public virtual bool NeedContentLayout
        {
            get { return false; }
        }
        //-------------------------------------------------------
        protected virtual void OnShown()
        {
        }
        protected virtual void OnHide()
        {
        }
        protected virtual void OnLostKeyboardFocus(UIFocusEventArgs e)
        {
        }
        protected virtual void OnLostMouseFocus(UIMouseEventArgs e)
        {
        }
        protected virtual void OnGotKeyboardFocus(UIFocusEventArgs e)
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
        //------------------------------------------------------------
        public void InvalidateLayout()
        {
            //add to layout queue
            if (this.HasReadyRenderElement)
            {
                this.CurrentPrimaryRenderElement.Root.AddToLayoutQueue(this.CurrentPrimaryRenderElement);
            }
        }
        protected virtual void OnContentLayout()
        {
        }
        protected virtual void OnContentUpdate()
        {
        }
        protected virtual void OnInterComponentMsg(object sender, int msgcode, string msg)
        {

        } 

        protected virtual void OnElementChanged()
        {
        }
        public abstract void Walk(UIVisitor visitor);

        protected virtual void OnGuestTalk(UIGuestTalkEventArgs e)
        {
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