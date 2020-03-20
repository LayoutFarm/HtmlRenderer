//BSD, 2014-present, WinterDev 


using LayoutFarm.UI;
namespace LayoutFarm.WebDom
{
    partial class DomElement : IUIEventListener
    {
        public bool AcceptKeyboardFocus { get; set; }
        bool IUIEventListener.DisableAutoMouseCapture => false;
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
        void IEventListener.ListenLostMouseFocus(UIMouseEventArgs e)
        {
            OnLostMouseFocus(e);
        }
        void IEventListener.ListenMouseMove(UIMouseEventArgs e)
        {
            OnMouseMove(e);
        }
        void IEventListener.ListenMouseUp(UIMouseEventArgs e)
        {
            //1. mouse up
            OnMouseUp(e);
        }
        void IEventListener.ListenMouseClick(UIMouseEventArgs e)
        {
        }
        void IEventListener.ListenMouseDoubleClick(UIMouseEventArgs e)
        {
            OnDoubleClick(e);
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
            OnGotFocus(e);
        }
        void IEventListener.ListenLostKeyboardFocus(UIFocusEventArgs e)
        {
            OnLostFocus(e);
        }
        bool IUIEventListener.AutoStopMouseEventPropagation
        {
            get { return false; }
        }
         

        //--------------
        void IUIEventListener.HandleContentLayout()
        {
            OnContentLayout();
        }
        void IUIEventListener.HandleContentUpdate()
        {
            OnContentUpdate();
        }
        void IUIEventListener.HandleElementUpdate()
        {
            OnElementChanged();
        }
        bool IUIEventListener.BypassAllMouseEvents
        {
            get { return false; }
        }


        void IEventListener.ListenGuestMsg(UIGuestMsgEventArgs e)
        {
        }
        void IUIEventListener.GetGlobalLocation(out int x, out int y)
        {
            this.GetGlobalLocation(out x, out y);
        }
        void IUIEventListener.GetViewport(out int left, out int top)
        {
            this.GetViewport(out left, out top);
        }
        public abstract void GetGlobalLocation(out int x, out int y);
        public abstract void GetGlobalLocationRelativeToRoot(out int x, out int y);
        public abstract void GetViewport(out int x, out int y);
    }
}