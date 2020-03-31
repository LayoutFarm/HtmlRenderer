//BSD, 2014-present, WinterDev 


using LayoutFarm.UI;
namespace LayoutFarm.WebDom
{
    public delegate void HtmlEventHandler(UIEventArgs e);
    partial class DomElement : IUIEventListener
    {
        //------------------------------------------------------
        public virtual void AttachEvent(UIEventName eventName, HtmlEventHandler handler) { }
        public virtual void DetachEvent(UIEventName eventName, HtmlEventHandler handler) { }
        public virtual void RaiseEvent(UIEventName eventName, UIEventArgs e) { }
        //-------------------------------------------------------
        protected virtual void OnLostFocus(UIFocusEventArgs e) { }
        protected virtual void OnGotFocus(UIFocusEventArgs e) { }
        protected virtual void OnDoubleClick(UIMouseEventArgs e) { }
        protected virtual void OnLostMouseFocus(UIMouseLostFocusEventArgs e) { }
        protected virtual void OnMouseDown(UIMouseEventArgs e)
        {
            //if (_evhMouseDown != null)
            //{
            //    _evhMouseDown(e);
            //}
            if (!e.CancelBubbling)
            {
                if (this.ParentNode is DomElement parentAsDomElem)
                {
                    //recursive to its parent
                    parentAsDomElem.OnMouseDown(e);
                    //when stop the cancel bubbline
                    //e.CancelBubbling = true;
                }
            }
        }
        protected virtual void OnMouseUp(UIMouseEventArgs e)
        {
            //if (_evhMouseUp != null)
            //{
            //    _evhMouseUp(e);
            //}
            if (!e.CancelBubbling)
            {
                if (this.ParentNode is DomElement parentAsDomElem)
                {
                    //recursive to its parent
                    parentAsDomElem.OnMouseUp(e);
                    //when stop the cancel bubbline
                    //e.CancelBubbling = true;
                }
            }
        }
        protected virtual void OnMousePress(UIMousePressEventArgs e)
        {

        }
        protected virtual void OnMouseWheel(UIMouseEventArgs e) { }
        protected virtual void OnCollapsed() { }
        protected virtual void OnExpanded() { }
        protected virtual void OnElementLanded() { }
        protected virtual void OnShown() { }
        protected virtual void OnHide() { }

        protected virtual void OnKeyDown(UIKeyEventArgs e) { }
        protected virtual void OnKeyUp(UIKeyEventArgs e) { }
        protected virtual void OnKeyPress(UIKeyEventArgs e) { }
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

        protected virtual void OnMouseEnter(UIMouseEventArgs e)
        {
        }
        protected virtual void OnMouseLeave(UIMouseLeaveEventArgs e)
        {
        }


        protected virtual void OnContentLayout()
        {
        }
        protected virtual void OnContentUpdate()
        {
            this.OwnerDocument.IncDomVersion();
        }
        protected virtual void OnElementChanged()
        {
        }

    }
}