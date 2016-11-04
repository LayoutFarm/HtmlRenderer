//BSD, 2014-2016, WinterDev  


using LayoutFarm.UI;
namespace LayoutFarm.WebDom
{
    public delegate void HtmlEventHandler(UIEventArgs e);
    partial class DomElement : IEventListener
    {
        //------------------------------------------------------
        public void AttachEvent(UIEventName eventName, HtmlEventHandler handler)
        {
            switch (eventName)
            {
                case UIEventName.MouseDown:
                    {
                        this.evhMouseDown += handler;
                    }
                    break;
                case UIEventName.MouseUp:
                    {
                        this.evhMouseUp += handler;
                    }
                    break;
            }
        }

        public void DetachEvent(UIEventName eventName, HtmlEventHandler handler)
        {
            switch (eventName)
            {
                case UIEventName.MouseDown:
                    {
                        this.evhMouseDown -= handler;
                    }
                    break;
                case UIEventName.MouseUp:
                    {
                        this.evhMouseUp -= handler;
                    }
                    break;
            }
        }
        //-------------------------------------------------------
        public void AttachEventOnMouseLostFocus(HtmlEventHandler handler)
        {
            this.evhMouseLostFocus += handler;
        }

        //-------------------------------------------------------
        protected virtual void OnLostFocus(UIFocusEventArgs e)
        {
        }

        protected virtual void OnGotFocus(UIFocusEventArgs e)
        {
        }

        protected virtual void OnDoubleClick(UIMouseEventArgs e)
        {
        }
        protected virtual void OnLostMouseFocus(UIMouseEventArgs e)
        {
            if (this.evhMouseLostFocus != null)
            {
                this.evhMouseLostFocus(e);
            }
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
        protected virtual void OnKeyPress(UIKeyEventArgs e)
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


        protected virtual void OnContentLayout()
        {
        }
        protected virtual void OnContentUpdate()
        {
            this.OwnerDocument.DomUpdateVersion++;
        }
        protected virtual void OnElementChanged()
        {
        }
        protected virtual void OnInterComponentMsg(object sender, int msgcode, string msg)
        {
        }
    }
}