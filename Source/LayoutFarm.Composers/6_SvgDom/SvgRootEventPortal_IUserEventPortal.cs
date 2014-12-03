//MS-PL, Apache2 
//2014, WinterDev

using System;
using System.Collections.Generic;
using LayoutFarm.Drawing;
using HtmlRenderer.Boxes;
using HtmlRenderer.Css;
using HtmlRenderer.Composers;
using HtmlRenderer.Composers.BridgeHtml;
using HtmlRenderer.WebDom;
using LayoutFarm.UI;
using LayoutFarm.SvgDom;

namespace HtmlRenderer.Composers.BridgeHtml
{

    partial class SvgRootEventPortal : IUserEventPortal
    {
        

        //------------------------------------------------------------
        void IUserEventPortal.PortalMouseDown(UIMouseEventArgs e)
        {

            this.latestLogicalMouseDownX = e.X;
            this.latestLogicalMouseDownY = e.Y;
            this.prevLogicalMouseX = e.X;
            this.prevLogicalMouseY = e.Y;

            //find hit svg graphics....
            SvgHitChain hitChain = GetFreeHitChain();

            HitTestCore(this.SvgRoot.SvgSpec, hitChain, e.X, e.Y);

            //propagate ...
            ForEachOnlyEventPortalBubbleUp(e, hitChain, (portal) =>
            {
                portal.PortalMouseDown(e);
                return true;
            });

            if (!e.CancelBubbling)
            {
                ForEachEventListenerBubbleUp(e, hitChain, () =>
                {
                    e.CurrentContextElement.ListenMouseDown(e);
                    return true;
                });
            }
            e.CancelBubbling = true;
        }
        void IUserEventPortal.PortalMouseUp(UIMouseEventArgs e)
        {

            this.prevLogicalMouseX = e.X;
            this.prevLogicalMouseY = e.Y;

            //this.OnMouseUp(e);
        }
        void IUserEventPortal.PortalMouseMove(UIMouseEventArgs e)
        {

            //find diff    

            e.SetDiff(
                e.X - prevLogicalMouseX,
                e.Y - prevLogicalMouseY,
                e.X - this.latestLogicalMouseDownX,
                e.Y - this.latestLogicalMouseDownY);

            this.prevLogicalMouseX = e.X;
            this.prevLogicalMouseY = e.Y;
            //this.OnMouseMove(e);
        }
        void IUserEventPortal.PortalMouseWheel(UIMouseEventArgs e)
        {
            //this.OnMouseWheel(e);
        }

        //------------------------------------------------------------
        void IUserEventPortal.PortalKeyUp(UIKeyEventArgs e)
        {
            //this.OnKeyUp(e);
        }
        void IUserEventPortal.PortalKeyDown(UIKeyEventArgs e)
        {
            //this.OnKeyDown(e);
        }
        void IUserEventPortal.PortalKeyPress(UIKeyEventArgs e)
        {
            // this.OnKeyPress(e);
        }
        bool IUserEventPortal.PortalProcessDialogKey(UIKeyEventArgs e)
        {
            return false;
            //return this.OnProcessDialogKey(e);
        }
        //------------------------------------------------------------
        void IUserEventPortal.PortalGotFocus(UIFocusEventArgs e)
        {
            //this.OnGotFocus(e);
        }
        void IUserEventPortal.PortalLostFocus(UIFocusEventArgs e)
        {
            //this.OnLostFocus(e);
        }


     
    }
}