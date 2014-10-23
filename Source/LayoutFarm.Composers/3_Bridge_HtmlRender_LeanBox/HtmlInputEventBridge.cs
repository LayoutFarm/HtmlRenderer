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

    public class HtmlInputEventBridge
    {

        //-----------------------------------------------
        HtmlIsland _htmlIsland;
        CssBoxHitChain _latestMouseDownHitChain = null;
        int _mousedownX;
        int _mousedownY;
        bool _isMouseDown;
        IFonts ifonts;
        bool _isBinded;

        public HtmlInputEventBridge()
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

            if (_latestMouseDownHitChain != null)
            {
                _latestMouseDownHitChain.Clear();
                _latestMouseDownHitChain = null;
            }
            //----------------------------------------------------
            int x = e.X;
            int y = e.Y;

            _mousedownX = x;
            _mousedownY = y;
            this._isMouseDown = true;

            CssBoxHitChain hitChain = new CssBoxHitChain();
            _latestMouseDownHitChain = hitChain;
            hitChain.SetRootGlobalPosition(x, y);
            //1. prob hit chain only
            BoxUtils.HitTest(rootbox, x, y, hitChain);
            //2. invoke css event and script event     
            e.EventName = UIEventName.MouseDown;
            PropagateEventOnBubblingPhase(hitChain, e);
        }

        public void MouseMove(UIMouseEventArgs e)
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
            //-----------------------------------------
            int x = e.X;
            int y = e.Y;
            if (this._isMouseDown)
            {
                //dragging
                if (this._mousedownX != x || this._mousedownY != y)
                {
                    //handle mouse drag
                    CssBoxHitChain hitChain = new CssBoxHitChain();
                    hitChain.SetRootGlobalPosition(x, y);
                    BoxUtils.HitTest(rootbox, x, y, hitChain);
                    ClearPreviousSelection();
                    if (hitChain.Count > 0)
                    {
                        this._htmlIsland.SetSelection(new SelectionRange(
                            _latestMouseDownHitChain,
                            hitChain,
                            this.ifonts));
                    }
                    else
                    {
                        this._htmlIsland.SetSelection(null);
                    }
                    hitChain.Clear();
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
            this._isMouseDown = false;
            var rootbox = _htmlIsland.GetRootCssBox();
            if (rootbox == null)
            {
                return;
            }
            //-----------------------------------------

            CssBoxHitChain hitChain = new CssBoxHitChain();
            hitChain.SetRootGlobalPosition(e.X, e.Y);
            //1. prob hit chain only

            BoxUtils.HitTest(rootbox, e.X, e.Y, hitChain);

            //2. invoke css event and script event 
            var hitInfo = hitChain.GetLastHit();

            PropagateEventOnBubblingPhase(hitChain, e);
            hitChain.Clear();
            //ReleaseHitChainhitChain);          

            if (_latestMouseDownHitChain != null)
            {
                //ReleaseHitChain(_latestMouseDownHitChain);
                _latestMouseDownHitChain.Clear();
                _latestMouseDownHitChain = null;
            }


        }
        public void MouseLeave()
        {
        }
        public void MouseDoubleClick(int x, int y, int button)
        {
        }
        public void MouseWheel(int x, int y, int button, int delta)
        {

        }
        void ClearPreviousSelection()
        {
            this._htmlIsland.ClearPreviousSelection();
        }
        public void KeyDown(char keyChar)
        {
            //send focus to current input element
        }
        public void KeyUp()
        {
        }

        static void PropagateEventOnBubblingPhase(CssBoxHitChain hitChain, UIEventArgs eventArgs)
        {

            for (int i = hitChain.Count - 1; i >= 0; --i)
            {
                //propagate up 
                var hitInfo = hitChain.GetHitInfo(i);
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
                    eventArgs.CurrentContextElement = controller;
                    eventArgs.Location = new Point(hitInfo.localX, hitInfo.localY);
                    //---------------------------------
                    //dispatch 
                    switch (eventArgs.EventName)
                    {
                        case UIEventName.MouseDown:
                            {                                 
                                controller.ListenMouseEvent(UIMouseEventName.MouseDown, (UIMouseEventArgs)eventArgs);
                            } break;
                        case UIEventName.MouseUp:
                            {   
                                controller.ListenMouseEvent(UIMouseEventName.MouseUp, (UIMouseEventArgs)eventArgs);
                            } break;
                    }

                    if (eventArgs.IsCanceled)
                    {
                        break;
                    }
                }
                //---------------------
            }


        }
        //----------------------------------- 
        //Queue<CssBoxHitChain> hitChainPools = new Queue<CssBoxHitChain>();
        //CssBoxHitChain GetFreeHitChain()
        //{ 
        //    if (hitChainPools.Count == 0)
        //    {
        //        return new CssBoxHitChain();
        //    }
        //    else
        //    {
        //        return hitChainPools.Dequeue();
        //    }
        //}
        //void ReleaseHitChain(CssBoxHitChain hitChain)
        //{
        //    hitChain.Clear();
        //    hitChainPools.Enqueue(hitChain);
        //} 
        //-----------------------------------

    }
}