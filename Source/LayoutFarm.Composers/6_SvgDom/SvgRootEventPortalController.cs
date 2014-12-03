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

    class SvgRootEventPortalController : IUserEventPortal
    {
        HtmlElement elementNode;
        public SvgRootEventPortalController(HtmlElement elementNode)
        {
            this.elementNode = elementNode;
        }
        public CssBoxSvgRoot SvgRoot
        {
            get;
            set;
        }
        int latestLogicalMouseDownX;
        int latestLogicalMouseDownY;
        int prevLogicalMouseX;
        int prevLogicalMouseY;

        //------------------------------------------------------------
        void IUserEventPortal.PortalMouseDown(UIMouseEventArgs e)
        {

            this.latestLogicalMouseDownX = e.X;
            this.latestLogicalMouseDownY = e.Y;
            this.prevLogicalMouseX = e.X;
            this.prevLogicalMouseY = e.Y;

            //find hit svg graphics....

            SvgHitChain hitChain = new SvgHitChain();
            
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


        //==================================================
        static void HitTestCore(SvgElement root, SvgHitChain chain, float x, float y)
        {
            //1.

            chain.AddHit(root, x, y);
            //2. find hit child
            var child = root.GetFirstNode();
            while (child != null)
            {
                var node = child.Value;
                if (node.HitTestCore(chain, x, y))
                {
                    break;
                }
                child = child.Next;
            }
        }

        static void ForEachOnlyEventPortalBubbleUp(UIEventArgs e, SvgHitChain hitPointChain, EventPortalAction eventPortalAction)
        {
            //only listener that need tunnel down 
            for (int i = hitPointChain.Count - 1; i >= 0; --i)
            {
                //propagate up 
                var hitInfo = hitPointChain.GetHitInfo(i);

                SvgElement svg = hitInfo.svg;
                if (svg != null)
                {
                    var controller = SvgElement.UnsafeGetController(hitInfo.svg) as IUserEventPortal;
                    if (controller != null)
                    {
                        e.Location = new Point((int)hitInfo.x, (int)hitInfo.y);
                        if (eventPortalAction(controller))
                        {
                            return;
                        }
                    }
                }
            }
        }

        static void ForEachEventListenerBubbleUp(UIEventArgs e, SvgHitChain hitChain, EventListenerAction listenerAction)
        {

            for (int i = hitChain.Count - 1; i >= 0; --i)
            {
                //propagate up 
                var hitInfo = hitChain.GetHitInfo(i);
                IEventListener controller = SvgElement.UnsafeGetController(hitInfo.svg) as IEventListener;
                //switch (hitInfo.hitObjectKind)
                //{
                //    default:
                //        {
                //            continue;
                //        }
                //    case HitObjectKind.Run:
                //        {
                //            CssRun run = (CssRun)hitInfo.hitObject;
                //            controller = CssBox.UnsafeGetController(run.OwnerBox) as IEventListener;

                //        } break;
                //    case HitObjectKind.CssBox:
                //        {
                //            CssBox box = (CssBox)hitInfo.hitObject;
                //            controller = CssBox.UnsafeGetController(box) as IEventListener;
                //        } break;
                //}

                //---------------------
                if (controller != null)
                {
                    //found controller

                    e.CurrentContextElement = controller;
                    e.Location = new Point((int)hitInfo.x, (int)hitInfo.y);

                    if (listenerAction())
                    {
                        return;
                    }
                }
            }
        }


    }
}