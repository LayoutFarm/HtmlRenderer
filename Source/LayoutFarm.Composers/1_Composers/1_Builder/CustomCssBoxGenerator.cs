// 2015,2014 ,BSD, WinterDev 
using System.Collections.Generic;
using System.Globalization;
using PixelFarm.Drawing;
using LayoutFarm.WebDom;
using LayoutFarm.Css;
using LayoutFarm.HtmlBoxes;
using LayoutFarm.UI;
using LayoutFarm.RenderBoxes;

namespace LayoutFarm.Composers
{

    public abstract class CustomCssBoxGenerator
    {
        protected abstract HtmlHost MyHost { get; }
        public abstract CssBox CreateCssBox(LayoutFarm.WebDom.DomElement tag, CssBox parentBox, BoxSpec spec, LayoutFarm.RootGraphic rootgfx);

        public static CssBox CreateWrapper(object owner, RenderElement renderElement, BoxSpec spec, bool isInline)
        {
            var portalEvent = owner as IUserEventPortal;
            if (portalEvent == null)
            {
                //auto create iuser event portal
                portalEvent = new RenderElementUserEventPortal(renderElement);
            }

            if (isInline)
            {
                return new LayoutFarm.HtmlBoxes.InternalWrappers.WrapperInlineCssBox(portalEvent, spec, renderElement.Root, renderElement);
            }
            else
            {
                return new LayoutFarm.HtmlBoxes.InternalWrappers.WrapperBlockCssBox(portalEvent, spec, renderElement);
            }
        }
        

        class RenderElementUserEventPortal : IUserEventPortal
        {
            RenderElement renderE;
            public RenderElementUserEventPortal(RenderElement renderE)
            {

                this.renderE = renderE;
            }
            //---------------------------------------------------
            //user event portal impl
            void IUserEventPortal.PortalKeyPress(UIKeyEventArgs e)
            {
            }
            void IUserEventPortal.PortalKeyDown(UIKeyEventArgs e)
            {

            }
            void IUserEventPortal.PortalKeyUp(UIKeyEventArgs e)
            {

            }
            bool IUserEventPortal.PortalProcessDialogKey(UIKeyEventArgs e)
            {
                return true;
            }
            void IUserEventPortal.PortalMouseDown(UIMouseEventArgs e)
            {

                //get free hit chain***
                //hit test 
                HitChain hitPointChain = new HitChain();
                hitPointChain.SetStartTestPoint(e.X, e.Y);
                this.renderE.HitTestCore(hitPointChain);
                //then invoke
                int hitCount = hitPointChain.Count;

                RenderElement hitElement = hitPointChain.TopMostElement;
                if (hitCount > 0)
                {
                    //use events
                    if (!e.CancelBubbling)
                    {
                        ForEachEventListenerBubbleUp(e, hitPointChain, (listener) =>
                        {
                            listener.ListenMouseDown(e);
                            //-------------------------------------------------------                          
                            return true;
                        });
                    }
                }
            }
            void IUserEventPortal.PortalMouseUp(UIMouseEventArgs e)
            {
                HitChain hitPointChain = new HitChain();
                hitPointChain.SetStartTestPoint(e.X, e.Y);

                renderE.HitTestCore(hitPointChain);
                //then invoke
                int hitCount = hitPointChain.Count;

                RenderElement hitElement = hitPointChain.TopMostElement;
                if (hitCount > 0)
                {
                    //use events
                    if (!e.CancelBubbling)
                    {
                        ForEachEventListenerBubbleUp(e, hitPointChain, (listener) =>
                        {
                            listener.ListenMouseUp(e);
                            //-------------------------------------------------------                          
                            return true;
                        });
                    }
                }
            }

            void IUserEventPortal.PortalMouseWheel(UIMouseEventArgs e)
            {
                HitChain hitPointChain = new HitChain();
                hitPointChain.SetStartTestPoint(e.X, e.Y);
                renderE.HitTestCore(hitPointChain);
                //then invoke
                int hitCount = hitPointChain.Count;

                RenderElement hitElement = hitPointChain.TopMostElement;
                if (hitCount > 0)
                {
                    //use events
                    if (!e.CancelBubbling)
                    {
                        ForEachEventListenerBubbleUp(e, hitPointChain, (listener) =>
                        {
                            listener.ListenMouseWheel(e);
                            //-------------------------------------------------------                          
                            return true;
                        });
                    }
                }
            }

            void IUserEventPortal.PortalGotFocus(UIFocusEventArgs e)
            {

            }

            void IUserEventPortal.PortalLostFocus(UIFocusEventArgs e)
            {

            }
            void IUserEventPortal.PortalMouseMove(UIMouseEventArgs e)
            {
                HitChain hitPointChain = new HitChain();
                hitPointChain.SetStartTestPoint(e.X, e.Y);
                renderE.HitTestCore(hitPointChain);
                //then invoke
                int hitCount = hitPointChain.Count;

                RenderElement hitElement = hitPointChain.TopMostElement;
                if (hitCount > 0)
                {
                    //use events
                    if (!e.CancelBubbling)
                    {
                        ForEachEventListenerBubbleUp(e, hitPointChain, (listener) =>
                        {
                            listener.ListenMouseMove(e);
                            //-------------------------------------------------------                          
                            return true;
                        });
                    }
                }
            }

            //===================================================================
            delegate bool EventPortalAction(IUserEventPortal evPortal);
            delegate bool EventListenerAction(IEventListener listener);
            static void ForEachEventListenerBubbleUp(UIEventArgs e, HitChain hitPointChain, EventListenerAction listenerAction)
            {
                LayoutFarm.RenderBoxes.HitInfo hitInfo;
                for (int i = hitPointChain.Count - 1; i >= 0; --i)
                {

                    hitInfo = hitPointChain.GetHitInfo(i);
                    IEventListener listener = hitInfo.hitElement.GetController() as IEventListener;
                    if (listener != null)
                    {
                        var hitPoint = hitInfo.point;
                        e.SetLocation(hitPoint.X, hitPoint.Y);
                        e.CurrentContextElement = listener;

                        if (listenerAction(listener))
                        {
                            return;
                        }
                    }
                }
            }
        }

    }



}