//BSD 2014,WinterDev
//ArthurHub

using System;
using System.Collections.Generic;
using LayoutFarm.Drawing;
using HtmlRenderer.Boxes;
using HtmlRenderer.WebDom;
using LayoutFarm;

namespace HtmlRenderer.Composers
{

    public class InputEventBridge
    {

        //-----------------------------------------------
        RootVisualBox _container;
        BoxHitChain _latestMouseDownHitChain = null;
        int _mousedownX;
        int _mousedownY;
        bool _isMouseDown;
        //----------------------------------------------- 
        SelectionRange _currentSelectionRange = null;

        bool _isBinded;
        public InputEventBridge()
        {
        }
        public void Bind(RootVisualBox container)
        {
            if (container != null)
            {
                this._container = container;
            }

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

             
            UIMouseEventArgs mouseDownE = new UIMouseEventArgs();
            mouseDownE.EventName = UIEventName.MouseDown;
            PropagateEventOnBubblingPhase(hitChain, mouseDownE);

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
                            hitChain, CurrentGraphicPlatform.P.SampleIGraphics);

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
        static void PropagateEventOnCapturingPhase(BoxHitChain hitChain, UIEventArgs eventArgs)
        {
            //TODO: consider implement capture phase

        }
        static void PropagateEventOnBubblingPhase(BoxHitChain hitChain, UIEventArgs eventArgs)
        {

            for (int i = hitChain.Count - 1; i >= 0; --i)
            {
                //propagate up 
                var hitInfo = hitChain.GetHitInfo(i);
                LayoutFarm.IEventListener controller = null;

                switch (hitInfo.hitObjectKind)
                {
                    default:
                        {
                            continue;
                        }
                    case HitObjectKind.Run:
                        {
                            CssRun run = (CssRun)hitInfo.hitObject;
                            controller = CssBox.UnsafeGetController(run.OwnerBox) as LayoutFarm.IEventListener;

                        } break;
                    case HitObjectKind.CssBox:
                        {
                            CssBox box = (CssBox)hitInfo.hitObject;
                            controller = CssBox.UnsafeGetController(box) as LayoutFarm.IEventListener;
                        } break;
                }

                //---------------------
                if (controller != null)
                {
                     
                    eventArgs.SetLocation(hitInfo.localX, hitInfo.localY);
                    //---------------------------------
                    //dispatch 


                    switch (eventArgs.EventName)
                    {
                        case UIEventName.MouseDown:
                            {
                                UIMouseEventArgs mouseE = new UIMouseEventArgs();
                                controller.ListenMouseEvent(UIMouseEventName.MouseDown, mouseE);
                            } break;
                        case UIEventName.MouseUp:
                            {
                                UIMouseEventArgs mouseE = new UIMouseEventArgs();
                                controller.ListenMouseEvent(UIMouseEventName.MouseUp, mouseE);
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

    }
}