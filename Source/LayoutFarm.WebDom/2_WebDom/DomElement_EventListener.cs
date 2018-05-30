//BSD, 2014-2018, WinterDev


using LayoutFarm.UI;
namespace LayoutFarm.WebDom
{
    partial class DomElement : IUIEventListener
    {
        void IUIEventListener.ListenKeyPress(UIKeyEventArgs e)
        {
            OnKeyPress(e);
        }
        void IUIEventListener.ListenKeyDown(UIKeyEventArgs e)
        {
            OnKeyDown(e);
        }
        void IUIEventListener.ListenKeyUp(UIKeyEventArgs e)
        {
            OnKeyUp(e);
        }
        bool IUIEventListener.ListenProcessDialogKey(UIKeyEventArgs e)
        {
            return OnProcessDialogKey(e);
        }
        void IUIEventListener.ListenMouseDown(UIMouseEventArgs e)
        {
            OnMouseDown(e);
        }
        void IUIEventListener.ListenLostMouseFocus(UIMouseEventArgs e)
        {
            OnLostMouseFocus(e);
        }
        void IUIEventListener.ListenMouseMove(UIMouseEventArgs e)
        {
            OnMouseMove(e);
        }
        void IUIEventListener.ListenMouseUp(UIMouseEventArgs e)
        {
            //1. mouse up
            OnMouseUp(e);
        }
        void IUIEventListener.ListenMouseClick(UIMouseEventArgs e)
        {
        }
        void IUIEventListener.ListenMouseDoubleClick(UIMouseEventArgs e)
        {
            OnDoubleClick(e);
        }
        void IUIEventListener.ListenMouseWheel(UIMouseEventArgs e)
        {
            OnMouseWheel(e);
        }
        void IUIEventListener.ListenMouseLeave(UIMouseEventArgs e)
        {
            OnMouseLeave(e);
        }
        void IUIEventListener.ListenGotKeyboardFocus(UIFocusEventArgs e)
        {
            OnGotFocus(e);
        }
        void IUIEventListener.ListenLostKeyboardFocus(UIFocusEventArgs e)
        {
            OnLostFocus(e);
        }

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
        bool IUIEventListener.AutoStopMouseEventPropagation
        {
            get { return false; }
        }
        void IUIEventListener.ListenInterComponentMsg(object sender, int msgcode, string msg)
        {
            this.OnInterComponentMsg(sender, msgcode, msg);
        }

        void IUIEventListener.ListenGuestTalk(UIGuestTalkEventArgs e)
        {
        }
        void IUIEventListener.GetGlobalLocation(out int x, out int y)
        {
            this.GetGlobalLocation(out x, out y);
        }

        public abstract void GetGlobalLocation(out int x, out int y);
    }
}