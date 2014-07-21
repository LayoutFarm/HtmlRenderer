//BSD 2014,WinterDev
//ArthurHub

using System;
using System.Collections.Generic;
using System.Drawing;
using HtmlRenderer.Drawing;
using HtmlRenderer.Boxes;
using HtmlRenderer.WebDom;

namespace HtmlRenderer.Composers 
{

    public class HtmlInputEventBridge
    {

        //-----------------------------------------------
        RootVisualBox _container;
        BoxHitChain _latestMouseDownHitChain = null;
        int _mousedownX;
        int _mousedownY;
        bool _isMouseDown;
        //----------------------------------------------- 
        SelectionRange _currentSelectionRange = null; 
        IGraphics sampleGraphics; 
        bool _isBinded; 
        public HtmlInputEventBridge()
        {
        }
        public void Bind(RootVisualBox container)
        {
            if (container != null)
            {
                this._container = container;
            }
            this.sampleGraphics = container.GetSampleGraphics();
            _isBinded = true;
        }
        public void Unbind()
        {
            this._container = null;
            this._isBinded = false;
        }

        public void MouseDown(int x, int y, int button)
        {
            if (!_isBinded)
            {
                return;
            }
            var rootbox = _container.GetRootCssBox();
            if (rootbox == null)
            {
                return;
            }
            //---------------------------------------------------- 
            ClearPreviousSelection();

            if (_latestMouseDownHitChain != null)
            {
                ReleaseHitChain(_latestMouseDownHitChain);
                _latestMouseDownHitChain = null;
            }
            //----------------------------------------------------

            _mousedownX = x;
            _mousedownY = y;
            this._isMouseDown = true;

            BoxHitChain hitChain = GetFreeHitChain();
            _latestMouseDownHitChain = hitChain;

            hitChain.SetRootGlobalPosition(x, y);
            //1. prob hit chain only
            BoxUtils.HitTest(rootbox, x, y, hitChain);
            //2. invoke css event and script event  


            HtmlEventArgs eventArgs = new HtmlEventArgs(EventName.MouseDown);
            PropagateEventOnBubblingPhase(hitChain, eventArgs);

        }
        public void MouseMove(int x, int y, int button)
        {
            if (!_isBinded)
            {
                return;
            }
            var rootbox = _container.GetRootCssBox();
            if (rootbox == null)
            {
                return;
            }
            //-----------------------------------------
            if (this._isMouseDown)
            {
                //dragging
                if (this._mousedownX != x || this._mousedownY != y)
                {
                    //handle mouse drag
                    BoxHitChain hitChain = GetFreeHitChain();

                    hitChain.SetRootGlobalPosition(x, y);
                    BoxUtils.HitTest(rootbox, x, y, hitChain);
                    ClearPreviousSelection();
                    if (hitChain.Count > 0)
                    {
                        _currentSelectionRange = new SelectionRange(
                            _latestMouseDownHitChain,
                            hitChain, sampleGraphics);

                    }
                    else
                    {
                        _currentSelectionRange = null;
                    }
                    ReleaseHitChain(hitChain);
                }
            }
            else
            {
                //mouse move 


            }
        }
        public void MouseUp(int x, int y, int button)
        {
            if (!_isBinded)
            {
                return;
            }
            this._isMouseDown = false;
            var rootbox = _container.GetRootCssBox();
            if (rootbox == null)
            {
                return;
            }
            //-----------------------------------------

            BoxHitChain hitChain = GetFreeHitChain();
            hitChain.SetRootGlobalPosition(x, y);
            //1. prob hit chain only
            BoxUtils.HitTest(rootbox, x, y, hitChain);

            //2. invoke css event and script event 
            var hitInfo = hitChain.GetLastHit();

            ReleaseHitChain(hitChain);
            if (_latestMouseDownHitChain != null)
            {
                ReleaseHitChain(_latestMouseDownHitChain);
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
            //TODO: check mouse botton
            if (_currentSelectionRange != null)
            {
                _currentSelectionRange.ClearSelectionStatus();
                _currentSelectionRange = null;
            }
        }

        public void KeyDown()
        {
        }
        public void KeyUp()
        {
        }
        //-----------------------------------


        Queue<BoxHitChain> hitChainPools = new Queue<BoxHitChain>();
        BoxHitChain GetFreeHitChain()
        {

            if (hitChainPools.Count == 0)
            {
                return new BoxHitChain();
            }
            else
            {
                return hitChainPools.Dequeue();
            }
        }
        void ReleaseHitChain(BoxHitChain hitChain)
        {
            hitChain.Clear();
            hitChainPools.Enqueue(hitChain);
        }

        //-----------------------------------
        static void PropagateEventOnCapturingPhase(BoxHitChain hitChain, HtmlEventArgs eventArgs)
        {
            //TODO: consider implement capture phase

        }
        static void PropagateEventOnBubblingPhase(BoxHitChain hitChain, HtmlEventArgs eventArgs)
        {

            for (int i = hitChain.Count - 1; i >= 0; --i)
            {
                //propagate up 
                var hitInfo = hitChain.GetHitInfo(i);
                HtmlElement controller = null;

                switch (hitInfo.hitObjectKind)
                {
                    default:
                        {
                            continue;
                        }
                    case HitObjectKind.Run:
                        {
                            CssRun run = (CssRun)hitInfo.hitObject;
                            controller = CssBox.UnsafeGetController(run.OwnerBox) as HtmlElement;
                        } break;
                    case HitObjectKind.CssBox:
                        {
                            CssBox box = (CssBox)hitInfo.hitObject;
                            controller = CssBox.UnsafeGetController(box) as HtmlElement;
                        } break;
                }

                //---------------------
                if (controller != null)
                {
                    controller.DispatchEvent(eventArgs);
                    if (eventArgs.IsCanceled)
                    {
                        break;
                    }
                }
                //---------------------
            }


        }

    }
}