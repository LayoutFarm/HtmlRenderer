//Apache2, 2014-2017, WinterDev

namespace LayoutFarm.UI
{
    partial class UIElement
    {
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
        void IEventListener.ListenMouseMove(UIMouseEventArgs e)
        {
            OnMouseMove(e);
        }
        void IEventListener.ListenMouseUp(UIMouseEventArgs e)
        {
            OnMouseUp(e);
        }
        void IEventListener.ListenLostMouseFocus(UIMouseEventArgs e)
        {
            OnLostMouseFocus(e);
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
            OnGotKeyboardFocus(e);
        }
        void IEventListener.ListenLostKeyboardFocus(UIFocusEventArgs e)
        {
            OnLostKeyboardFocus(e);
        }



        void IEventListener.HandleContentLayout()
        {
            OnContentLayout();
        }
        void IEventListener.HandleContentUpdate()
        {
            OnContentUpdate();
        }
        void IEventListener.HandleElementUpdate()
        {
            OnElementChanged();
        }
        bool IEventListener.BypassAllMouseEvents
        {
            get
            {
                return this.TransparentAllMouseEvents;
            }
        }
        bool IEventListener.AutoStopMouseEventPropagation
        {
            get
            {
                return this.AutoStopMouseEventPropagation;
            }
        }
        void IEventListener.ListenInterComponentMsg(object sender, int msgcode, string msg)
        {
            this.OnInterComponentMsg(sender, msgcode, msg);
        }

        void IEventListener.ListenGuestTalk(UIGuestTalkEventArgs e)
        {
            this.OnGuestTalk(e);
        }
        void IEventListener.GetGlobalLocation(out int x, out int y)
        {
            var globalLoca = this.CurrentPrimaryRenderElement.GetGlobalLocation();
            x = globalLoca.X;
            y = globalLoca.Y;
        }
    }
}