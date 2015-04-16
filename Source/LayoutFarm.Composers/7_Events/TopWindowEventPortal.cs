using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
using LayoutFarm.UI;

namespace LayoutFarm
{
    class TopWindowEventPortal : ITopWindowEventPortal
    {
        IEventListener currentKbFocusElem;
        UserEventPortal userEventPortal;
        IUserEventPortal iuserEventPortal;
        IEventListener currentMouseActiveElement;
        IEventListener latestMouseDown;
        DateTime lastTimeMouseUp;
        const int DOUBLE_CLICK_SENSE = 150;//ms  

        public TopWindowEventPortal()
        {
            this.userEventPortal = new UserEventPortal();
            this.iuserEventPortal = userEventPortal;
        }
        public void BindRenderElement(RenderElement topRenderElement)
        {
            this.userEventPortal.BindTopRenderElement(topRenderElement);
        } 
        public IEventListener CurrentKeyboardFocusedElement
        {
            get
            {
                return this.currentKbFocusElem;
            }
            set
            {
                //1. lost keyboard focus
                if (this.currentKbFocusElem != null && this.currentKbFocusElem != value)
                {
                    currentKbFocusElem.ListenLostKeyboardFocus(null);
                }
                //2. keyboard focus
                currentKbFocusElem = value;
            }
        }

        void ITopWindowEventPortal.PortalKeyPress(UIKeyEventArgs e)
        {
            if (currentKbFocusElem != null)
            {
                e.SourceHitElement = currentKbFocusElem;
                currentKbFocusElem.ListenKeyPress(e);
            }
            iuserEventPortal.PortalKeyPress(e);
        }
        void ITopWindowEventPortal.PortalKeyDown(UIKeyEventArgs e)
        {
            if (currentKbFocusElem != null)
            {
                e.SourceHitElement = currentKbFocusElem;
                currentKbFocusElem.ListenKeyDown(e);
            }

            iuserEventPortal.PortalKeyDown(e);
        }

        void ITopWindowEventPortal.PortalKeyUp(UIKeyEventArgs e)
        {
            if (currentKbFocusElem != null)
            {
                e.SourceHitElement = currentKbFocusElem;
                currentKbFocusElem.ListenKeyUp(e);
            }
            iuserEventPortal.PortalKeyUp(e);
        }

        bool ITopWindowEventPortal.PortalProcessDialogKey(UIKeyEventArgs e)
        {
            bool result = false;
            if (currentKbFocusElem != null)
            {
                e.SourceHitElement = currentKbFocusElem;
                result = currentKbFocusElem.ListenProcessDialogKey(e);
            }
            return result;
        }

        void ITopWindowEventPortal.PortalMouseDown(UIMouseEventArgs e)
        {
            e.PreviousMouseDown = this.latestMouseDown;
            iuserEventPortal.PortalMouseDown(e);
            this.currentMouseActiveElement = this.latestMouseDown = e.CurrentContextElement;
        }
        void ITopWindowEventPortal.PortalMouseMove(UIMouseEventArgs e)
        {
            iuserEventPortal.PortalMouseMove(e);
        } 
        void ITopWindowEventPortal.PortalMouseUp(UIMouseEventArgs e)
        {
            DateTime snapMouseUpTime = DateTime.Now;
            TimeSpan timediff = snapMouseUpTime - lastTimeMouseUp;
            
            this.lastTimeMouseUp = snapMouseUpTime;
            e.IsAlsoDoubleClick = timediff.Milliseconds < DOUBLE_CLICK_SENSE;
            iuserEventPortal.PortalMouseUp(e); 
        }

        void ITopWindowEventPortal.PortalMouseWheel(UIMouseEventArgs e)
        {
            //only on mouse active element
            if (currentMouseActiveElement != null)
            {
                currentMouseActiveElement.ListenMouseWheel(e);
            }
            iuserEventPortal.PortalMouseWheel(e);
        }

        void ITopWindowEventPortal.PortalGotFocus(UIFocusEventArgs e)
        {
            iuserEventPortal.PortalGotFocus(e);
        }

        void ITopWindowEventPortal.PortalLostFocus(UIFocusEventArgs e)
        {
            iuserEventPortal.PortalLostFocus(e);
        }
    }
}