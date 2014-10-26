//BSD 2014,WinterDev
//ArthurHub

using System;
using System.Collections.Generic;
using LayoutFarm.Drawing;
using HtmlRenderer.Boxes;
using HtmlRenderer.WebDom;
using LayoutFarm;
using LayoutFarm.UI;
namespace HtmlRenderer.Composers
{

    /// <summary>
    /// control Html input logic 
    /// </summary>
    public class HtmlInputEventAdapter
    {
        DateTime lastimeMouseUp;
        //-----------------------------------------------
        HtmlIsland _htmlIsland;
        CssBoxHitChain _latestHitChain = null;
        int _mousedownX;
        int _mousedownY;
        bool _isMouseDown;
        IFonts ifonts;
        bool _isBinded;
        const int DOUBLE_CLICK_SENSE = 150;//ms


        Stack<CssBoxHitChain> hitChainPools = new Stack<CssBoxHitChain>();

        public HtmlInputEventAdapter()
        {

        }

        
        public void Bind(HtmlIsland htmlIsland, IFonts ifonts)
        {
            this.ifonts = ifonts;
            if (htmlIsland != null)
            {
                this._htmlIsland = htmlIsland;
            }
            _isBinded = true;
        }
        public void Unbind()
        {
            this._htmlIsland = null;
            this._isBinded = false;
        }
        public void MouseDown(UIMouseEventArgs e)
        {
            if (!_isBinded)
            {
                return;
            }

            //find top root 
            var rootbox = _htmlIsland.GetRootCssBox();
            if (rootbox == null)
            {
                return;
            }
            //---------------------------------------------------- 
            ClearPreviousSelection();
            if (_latestHitChain != null)
            {
                ReleaseHitChain(_latestHitChain);
                _latestHitChain = null;
            }

            //----------------------------------------------------
            int x = e.X;
            int y = e.Y;

            _mousedownX = x;
            _mousedownY = y;
            this._isMouseDown = true;

            CssBoxHitChain hitChain = GetFreeHitChain();
            hitChain.SetRootGlobalPosition(x, y);
            //1. prob hit chain only
            BoxUtils.HitTest(rootbox, x, y, hitChain);
            //2. invoke css event and script event     
            e.EventName = UIEventName.MouseDown;
            PropagateEventOnBubblingPhase(hitChain, (hit, controller) =>
            {
                e.CurrentContextElement = controller;
                controller.ListenMouseDown(e);
                return true;
            });
            //----------------------------------
            //save mousedown hitchain
            this._latestHitChain = hitChain;             
        }        
        public void MouseMove(UIMouseEventArgs e)
        {
            if (!_isBinded)
            {
                return;
            }
            CssBox rootbox = _htmlIsland.GetRootCssBox();
            if (rootbox == null)
            {
                return;
            }
            //-----------------------------------------
            int x = e.X;
            int y = e.Y;
            if (this._isMouseDown)
            {
                //dragging *** 
                if (this._mousedownX != x || this._mousedownY != y)
                {
                    //handle mouse drag
                    CssBoxHitChain hitChain = GetFreeHitChain();
                    hitChain.SetRootGlobalPosition(x, y);
                    BoxUtils.HitTest(rootbox, x, y, hitChain);
                    ClearPreviousSelection();

                    if (hitChain.Count > 0)
                    {
                        this._htmlIsland.SetSelection(new SelectionRange(
                            _latestHitChain,
                            hitChain,
                            this.ifonts));
                    }
                    else
                    {
                        this._htmlIsland.SetSelection(null);
                    }
                    ReleaseHitChain(hitChain);
                     
                }
            }
            else
            {
                //mouse move  
            }
        }
        public void MouseUp(UIMouseEventArgs e)
        {

            if (!_isBinded)
            {
                return;
            }
            var rootbox = _htmlIsland.GetRootCssBox();
            if (rootbox == null)
            {
                return;
            }

            //--------------------------------------------
            DateTime snapMouseUpTime = DateTime.Now;
            TimeSpan timediff = snapMouseUpTime - lastimeMouseUp;
            bool isAlsoDoubleClick = timediff.Milliseconds < DOUBLE_CLICK_SENSE;
            this.lastimeMouseUp = snapMouseUpTime;
            //--------------------------------------------

            this._isMouseDown = false;
            //-----------------------------------------

            CssBoxHitChain hitChain = GetFreeHitChain();

            hitChain.SetRootGlobalPosition(e.X, e.Y);
            //1. prob hit chain only 
            BoxUtils.HitTest(rootbox, e.X, e.Y, hitChain);
            //2. invoke css event and script event  
            e.EventName = UIEventName.MouseUp;
            PropagateEventOnBubblingPhase(hitChain, (hit, controller) =>
            {
                e.CurrentContextElement = controller;
                controller.ListenMouseUp(e);
                return true;
            });

            hitChain.Clear();

            ReleaseHitChain(hitChain);
            this._latestHitChain.Clear();
            this._latestHitChain = null;
        }
        public void MouseLeave(UIMouseEventArgs e)
        {

        }
        public void MouseDoubleClick(UIMouseEventArgs e)
        {


        }
        public void MouseWheel(UIMouseEventArgs e)
        {

        }
        void ClearPreviousSelection()
        {
            this._htmlIsland.ClearPreviousSelection();
        }
        public void KeyDown(UIKeyEventArgs e)
        {
            //send focus to current input element

        }
        public void KeyPress(UIKeyEventArgs e)
        {
            //send focus to current input element

        }
        public bool ProcessDialogKey(UIKeyEventArgs e)
        {
            //send focus to current input element
            return false;
        }
        public void KeyUp(UIKeyEventArgs e)
        {
        }


        delegate bool EventPortalAction(HitInfo hitInfo, IUserEventPortal evPortal);
        delegate bool EventListenerAction(HitInfo hitInfo, IEventListener listener);

        static void ForEachOnlyEventPortalBubbleUp(CssBoxHitChain hitPointChain, EventPortalAction eventPortalAction)
        {
            //only listener that need tunnel down 
            for (int i = hitPointChain.Count - 1; i >= 0; --i)
            {
                //propagate up 
                HitInfo hitInfo = hitPointChain.GetHitInfo(i);
                IUserEventPortal controller = null;
                switch (hitInfo.hitObjectKind)
                {
                    default:
                        {
                            continue;
                        }
                    case HitObjectKind.Run:
                        {
                            CssRun run = (CssRun)hitInfo.hitObject;
                            controller = CssBox.UnsafeGetController(run.OwnerBox) as IUserEventPortal;

                        } break;
                    case HitObjectKind.CssBox:
                        {
                            CssBox box = (CssBox)hitInfo.hitObject;
                            controller = CssBox.UnsafeGetController(box) as IUserEventPortal;
                        } break;
                }

                //---------------------
                if (controller != null)
                {
                    //found controller
                    if (eventPortalAction(hitInfo, controller))
                    {
                        return;
                    }
                }
            }
        }



        static void PropagateEventOnBubblingPhase(CssBoxHitChain hitChain, EventListenerAction listenerAction)
        {

            for (int i = hitChain.Count - 1; i >= 0; --i)
            {
                //propagate up 
                HitInfo hitInfo = hitChain.GetHitInfo(i);
                IEventListener controller = null;
                switch (hitInfo.hitObjectKind)
                {
                    default:
                        {
                            continue;
                        }
                    case HitObjectKind.Run:
                        {
                            CssRun run = (CssRun)hitInfo.hitObject;
                            controller = CssBox.UnsafeGetController(run.OwnerBox) as IEventListener;

                        } break;
                    case HitObjectKind.CssBox:
                        {
                            CssBox box = (CssBox)hitInfo.hitObject;
                            controller = CssBox.UnsafeGetController(box) as IEventListener;
                        } break;
                }

                //---------------------
                if (controller != null)
                {
                    //found controller
                    if (listenerAction(hitInfo, controller))
                    {
                        return;
                    }
                }
            }
        }        
        CssBoxHitChain GetFreeHitChain()
        {
            if (hitChainPools.Count > 0)
            {
                return hitChainPools.Pop();
            }
            else
            {
                return new CssBoxHitChain();
            }
        }
        void ReleaseHitChain(CssBoxHitChain hitChain)
        {
            hitChain.Clear();
            this.hitChainPools.Push(hitChain);
        }
      
    }
}