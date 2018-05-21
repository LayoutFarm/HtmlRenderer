//Apache2, 2014-2018, WinterDev
//MS-PL, Apache2 some parts derived from github.com/vvvv/svg 

using PixelFarm.Drawing;
using LayoutFarm.UI;
namespace LayoutFarm.Svg
{
    partial class SvgRootEventPortal : IEventPortal
    {
        void IEventPortal.PortalMouseDown(UIMouseEventArgs e)
        {
            //find hit svg graphics....
            SvgHitChain hitChain = GetFreeHitChain();
            hitChain.SetRootGlobalPosition(e.X, e.Y);
            //1. hit test
            HitTestCore(this.SvgRoot.SvgSpec, hitChain, e.X, e.Y);
            SetEventOrigin(e, hitChain);
            //2. propagate event  portal
            ForEachOnlyEventPortalBubbleUp(e, hitChain, (portal) =>
            {
                portal.PortalMouseDown(e);
                return true;
            });
            if (!e.CancelBubbling)
            {
                //2. propagate events
                ForEachSvgElementBubbleUp(e, hitChain, () =>
                {
                    //-------
                    //temp test only
                    //-------
                    var svgElement = e.ExactHitObject as SvgElement;
                    if (svgElement is SvgRect)
                    {
                        ((SvgRect)svgElement).FillColor = Color.White;
                    }
                    return true;
                });
            }

            e.CancelBubbling = true;
            ReleaseHitChain(hitChain);
        }
        void IEventPortal.PortalMouseUp(UIMouseEventArgs e)
        {
            //find hit svg graphics....
            SvgHitChain hitChain = GetFreeHitChain();
            hitChain.SetRootGlobalPosition(e.X, e.Y);
            //1. hit test
            HitTestCore(this.SvgRoot.SvgSpec, hitChain, e.X, e.Y);
            SetEventOrigin(e, hitChain);
            //2. propagate event  portal
            ForEachOnlyEventPortalBubbleUp(e, hitChain, (portal) =>
            {
                portal.PortalMouseUp(e);
                return true;
            });
            if (!e.CancelBubbling)
            {
                //2. propagate events
                ForEachEventListenerBubbleUp(e, hitChain, () =>
                {
                    e.CurrentContextElement.ListenMouseUp(e);
                    return true;
                });
            }

            e.CancelBubbling = true;
            ReleaseHitChain(hitChain);
        }
        void IEventPortal.PortalMouseMove(UIMouseEventArgs e)
        {
            int x = e.X;
            int y = e.Y;
            if (e.IsDragging)
            {
                //dragging *** , if changed

                //handle mouse drag
                SvgHitChain hitChain = GetFreeHitChain();
                hitChain.SetRootGlobalPosition(x, y);
                HitTestCore(this.SvgRoot.SvgSpec, hitChain, e.X, e.Y);
                SetEventOrigin(e, hitChain);
                //---------------------------------------------------------
                //propagate mouse drag 
                ForEachOnlyEventPortalBubbleUp(e, hitChain, (portal) =>
                {
                    portal.PortalMouseMove(e);
                    return true;
                });
                //---------------------------------------------------------  
                if (!e.CancelBubbling)
                {
                    //clear previous svg selection 
                    ClearPreviousSelection();
                    //if (hitChain.Count > 0)
                    //{
                    //    //create selection range 
                    //    this._htmlContainer.SetSelection(new SelectionRange(
                    //        _latestMouseDownChain,
                    //        hitChain,
                    //        this.ifonts));
                    //}
                    //else
                    //{
                    //    this._htmlContainer.SetSelection(null);
                    //}


                    ForEachEventListenerBubbleUp(e, hitChain, () =>
                    {
                        e.CurrentContextElement.ListenMouseMove(e);
                        return true;
                    });
                }


                //---------------------------------------------------------
                ReleaseHitChain(hitChain);
            }
            else
            {
                //mouse move  
                //---------------------------------------------------------
                SvgHitChain hitChain = GetFreeHitChain();
                hitChain.SetRootGlobalPosition(x, y);
                HitTestCore(this.SvgRoot.SvgSpec, hitChain, e.X, e.Y);
                SetEventOrigin(e, hitChain);
                //---------------------------------------------------------

                ForEachOnlyEventPortalBubbleUp(e, hitChain, (portal) =>
                {
                    portal.PortalMouseMove(e);
                    return true;
                });
                //---------------------------------------------------------
                if (!e.CancelBubbling)
                {
                    ForEachEventListenerBubbleUp(e, hitChain, () =>
                    {
                        e.CurrentContextElement.ListenMouseMove(e);
                        return true;
                    });
                }
                ReleaseHitChain(hitChain);
            }
        }
        void IEventPortal.PortalMouseWheel(UIMouseEventArgs e)
        {
            //this.OnMouseWheel(e);
        }

        //------------------------------------------------------------
        void IEventPortal.PortalKeyUp(UIKeyEventArgs e)
        {
            //this.OnKeyUp(e);
        }
        void IEventPortal.PortalKeyDown(UIKeyEventArgs e)
        {
            //this.OnKeyDown(e);
        }
        void IEventPortal.PortalKeyPress(UIKeyEventArgs e)
        {
            // this.OnKeyPress(e);
        }
        bool IEventPortal.PortalProcessDialogKey(UIKeyEventArgs e)
        {
            return false;
            //return this.OnProcessDialogKey(e);
        }
        //------------------------------------------------------------
        void IEventPortal.PortalGotFocus(UIFocusEventArgs e)
        {
            //this.OnGotFocus(e);
        }
        void IEventPortal.PortalLostFocus(UIFocusEventArgs e)
        {
            //this.OnLostFocus(e);
        }
    }
}