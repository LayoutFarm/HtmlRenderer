//BSD, 2014-present, WinterDev 
//ArthurHub, Jose Manuel Menendez Poo

using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
using LayoutFarm.UI;
using LayoutFarm.UI.ForImplementator;

namespace LayoutFarm.HtmlBoxes
{
    /// <summary>
    /// control Html input logic 
    /// </summary>
    public class HtmlInputEventAdapter
    {
        HtmlVisualRoot _htmlVisualRoot;
        CssBoxHitChain _latestMouseDownChain = null;
        //-----------------------------------------------
        DateTime _lastimeMouseUp;
        IUIEventListener _currentMouseDown;
        int _mousedownX;
        int _mousedownY;
        CssBox _mouseDownStartAt;
        //-----------------------------------------------

        ITextService _textService;
        bool _isBinded;
        int _lastDomLayoutVersion;
        const int DOUBLE_CLICK_SENSE = 150;//ms 
        Stack<CssBoxHitChain> _hitChainPools = new Stack<CssBoxHitChain>();
        UIMouseLostFocusEventArgs _mouseLostFocus = new UIMouseLostFocusEventArgs();

        public HtmlInputEventAdapter()
        {
        }
        public void Bind(HtmlVisualRoot htmlVisualRoot)
        {
            _htmlVisualRoot = htmlVisualRoot;
            _isBinded = htmlVisualRoot != null;
            if (_isBinded)
            {
                _textService = htmlVisualRoot.GetTextService();
            }
        }
        public void Unbind()
        {
            _htmlVisualRoot = null;
            _isBinded = false;
        }
        static void SetEventOrigin(UIEventArgs e, CssBoxHitChain hitChain)
        {
            int count = hitChain.Count;
            if (count > 0)
            {
                e.SetExactHitObject(hitChain.GetHitInfo(count - 1).hitObject);
            }
        }
        public void MouseDown(UIMouseDownEventArgs e, CssBox startAt)
        {
            if (!_isBinded) return;
            if (startAt == null) return;
            //---------------------------------------------------- 
            if (!e.Shift)
            {
                ClearPreviousSelection();
            }

            if (_latestMouseDownChain != null)
            {
                ReleaseHitChain(_latestMouseDownChain);
                _latestMouseDownChain = null;
            }
            _lastDomLayoutVersion = _htmlVisualRoot.LayoutVersion;
            //----------------------------------------------------
            int x = e.X;
            int y = e.Y;
            _mouseDownStartAt = startAt;
            _mousedownX = x;
            _mousedownY = y;
            CssBoxHitChain hitChain = GetFreeHitChain();
#if DEBUG
            hitChain.debugEventPhase = CssBoxHitChain.dbugEventPhase.MouseDown;
#endif
            hitChain.SetRootGlobalPosition(x, y);
            //1. hittest 
            BoxHitUtils.HitTest(startAt, x, y, hitChain);
            //2. propagate events
            SetEventOrigin(e, hitChain);
            ForEachOnlyEventPortalBubbleUp(e, hitChain, portal =>
            {
                portal.PortalMouseDown(e);
                return true;
            });
            if (!e.CancelBubbling)
            {
                IUIEventListener prevMouseDownElement = _currentMouseDown;
                _currentMouseDown = null; //clear                 
                e.SetCurrentContextElement(null);
                ForEachEventListenerBubbleUp(e, hitChain, () =>
                {
                    //TODO: check accept keyboard
                    _currentMouseDown = e.CurrentContextElement;
                    e.CurrentContextElement.ListenMouseDown(e);
                    if (prevMouseDownElement != null &&
                        prevMouseDownElement != _currentMouseDown)
                    {
                        prevMouseDownElement.ListenLostMouseFocus(_mouseLostFocus);
                    }

                    return e.CancelBubbling;
                });
            }
            //----------------------------------
            //save mousedown hitchain
            _latestMouseDownChain = hitChain;
        }
        public void MouseDown(UIMouseDownEventArgs e)
        {
            this.MouseDown(e, _htmlVisualRoot.RootCssBox);
        }
        public void MouseMove(UIMouseMoveEventArgs e, CssBox startAt)
        {
            if (!_isBinded) return;
            if (startAt == null) return;
            //-----------------------------------------
            int x = e.X;
            int y = e.Y;


            if (e.IsDragging && _latestMouseDownChain != null)
            {
                //dragging *** , if changed
                if (_mousedownX != x || _mousedownY != y)
                {

                    //handle mouse drag
                    CssBoxHitChain hitChain = GetFreeHitChain();
#if DEBUG
                    hitChain.debugEventPhase = CssBoxHitChain.dbugEventPhase.MouseMove;
#endif
                    hitChain.SetRootGlobalPosition(x, y);
                    BoxHitUtils.HitTest(startAt, x, y, hitChain);
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
                        ClearPreviousSelection();
                        if (_latestMouseDownChain.Count > 0 && hitChain.Count > 0)
                        {
                            if (_htmlVisualRoot.LayoutVersion != _lastDomLayoutVersion)
                            {
                                //the dom has been changed so...
                                //need to evaluate hitchain at mousedown position again
                                int lastRootGlobalX = _latestMouseDownChain.RootGlobalX;
                                int lastRootGlobalY = _latestMouseDownChain.RootGlobalY;
                                _latestMouseDownChain.Clear();
                                _latestMouseDownChain.SetRootGlobalPosition(lastRootGlobalX, lastRootGlobalY);
                                BoxHitUtils.HitTest(_mouseDownStartAt, lastRootGlobalX, lastRootGlobalY, _latestMouseDownChain);
                            }

                            //create selection range 
                            var newSelectionRange = new SelectionRange(
                                  _latestMouseDownChain,
                                   hitChain,
                                    _textService);
                            if (newSelectionRange.IsValid)
                            {
                                _htmlVisualRoot.SetSelection(newSelectionRange);
                            }
                            else
                            {
                                _htmlVisualRoot.SetSelection(null);
                            }
                        }
                        else
                        {
                            _htmlVisualRoot.SetSelection(null);
                        }

                        ForEachEventListenerBubbleUp(e, hitChain, () =>
                        {
                            e.CurrentContextElement.ListenMouseMove(e);
                            return true;
                        });
                    }

                    //---------------------------------------------------------
                    ReleaseHitChain(hitChain);
                }
            }
            else
            {
                //mouse move  
                //---------------------------------------------------------
                CssBoxHitChain hitChain = GetFreeHitChain();
#if DEBUG
                hitChain.debugEventPhase = CssBoxHitChain.dbugEventPhase.MouseMove;
#endif
                hitChain.SetRootGlobalPosition(x, y);
                BoxHitUtils.HitTest(startAt, x, y, hitChain);
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

                var cssbox = e.ExactHitObject as HtmlBoxes.CssBox;
                if (cssbox != null)
                {
                    switch (cssbox.CursorName)
                    {
                        case Css.CssCursorName.IBeam:
                            if (e.MouseCursorStyle != MouseCursorStyle.IBeam)
                            {
                                e.MouseCursorStyle = MouseCursorStyle.IBeam;
                            }
                            break;
                        case Css.CssCursorName.Hand:
                        case Css.CssCursorName.Pointer:
                            if (e.MouseCursorStyle != MouseCursorStyle.Pointer)
                            {
                                e.MouseCursorStyle = MouseCursorStyle.Pointer;
                            }
                            break;
                        case Css.CssCursorName.Default:
                            if (e.MouseCursorStyle != MouseCursorStyle.Default)
                            {
                                e.MouseCursorStyle = MouseCursorStyle.Default;
                            }
                            break;
                    }
                }
                else
                {
                    var cssspan = e.ExactHitObject as HtmlBoxes.CssTextRun;
                    if (cssspan != null)
                    {
                        cssbox = cssspan.OwnerBox;
                        switch (cssbox.CursorName)
                        {
                            default:
                                e.MouseCursorStyle = MouseCursorStyle.IBeam;
                                break;
                            case Css.CssCursorName.Hand:
                            case Css.CssCursorName.Pointer:
                                if (e.MouseCursorStyle != MouseCursorStyle.Pointer)
                                {
                                    e.MouseCursorStyle = MouseCursorStyle.Pointer;
                                }
                                break;
                        }
                    }
                }
                ReleaseHitChain(hitChain);
            }
        }
        public void MouseMove(UIMouseMoveEventArgs e)
        {
            if (!_isBinded) return;
            //---------------------------------------------------- 
            this.MouseMove(e, _htmlVisualRoot.RootCssBox);
        }
        public void MouseUp(UIMouseUpEventArgs e, CssBox startAt)
        {
            if (!_isBinded) return;
            if (startAt == null) return;
            //---------------------------------------------------- 

            DateTime snapMouseUpTime = DateTime.Now;
            TimeSpan timediff = snapMouseUpTime - _lastimeMouseUp;
            bool isAlsoDoubleClick = timediff.Milliseconds < DOUBLE_CLICK_SENSE;
            _lastimeMouseUp = snapMouseUpTime;
            //----------------------------------------- 
            CssBoxHitChain hitChain = GetFreeHitChain();
#if DEBUG
            hitChain.debugEventPhase = CssBoxHitChain.dbugEventPhase.MouseUp;
#endif
            hitChain.SetRootGlobalPosition(e.X, e.Y);
            //1. prob hit chain only 
            BoxHitUtils.HitTest(startAt, e.X, e.Y, hitChain);
            SetEventOrigin(e, hitChain);
            //2. invoke css event and script event   
            ForEachOnlyEventPortalBubbleUp(e, hitChain, (portal) =>
            {
                portal.PortalMouseUp(e);
                return true;
            });
            if (!e.CancelBubbling)
            {
                ForEachEventListenerBubbleUp(e, hitChain, () =>
                {
                    e.CurrentContextElement.ListenMouseUp(e);
                    return e.CancelBubbling;
                });
            }

            if (!e.IsCanceled)
            {
                //--------------------
                //click or double click
                //--------------------
                if (isAlsoDoubleClick)
                {
                    ForEachEventListenerBubbleUp(e, hitChain, () =>
                    {
                        e.CurrentContextElement.ListenMouseDoubleClick(e);
                        return e.CancelBubbling;
                    });
                }
                else
                {
                    ForEachEventListenerBubbleUp(e, hitChain, () =>
                    {
                        e.CurrentContextElement.ListenMouseClick(e);
                        return e.CancelBubbling;
                    });
                }
            }

            ReleaseHitChain(hitChain);
            if (_latestMouseDownChain != null)
            {
                _latestMouseDownChain.Clear();
                //Console.WriteLine(dbugNN++);
                _latestMouseDownChain = null;
            }
        }
        //int dbugNN = 0;
        public void MouseUp(UIMouseUpEventArgs e)
        {
            MouseUp(e, _htmlVisualRoot.RootCssBox);
        }
        public void MouseWheel(UIMouseWheelEventArgs e)
        {
            this.MouseWheel(e, _htmlVisualRoot.RootCssBox);
        }
        public void MouseWheel(UIMouseWheelEventArgs e, CssBox startAt)
        {
        }
        void ClearPreviousSelection()
        {
            _htmlVisualRoot.ClearPreviousSelection();
        }
        public void KeyDown(UIKeyEventArgs e)
        {
            this.KeyDown(e, _htmlVisualRoot.RootCssBox);
        }

        public void KeyDown(UIKeyEventArgs e, CssBox startAt)
        {
            //send focus to current input element

        }
        public void MouseLeaveFromViewport() { }

        public void KeyPress(UIKeyEventArgs e)
        {
            this.KeyPress(e, _htmlVisualRoot.RootCssBox);
        }
        public void KeyPress(UIKeyEventArgs e, CssBox startAt)
        {
            //send focus to current input element

        }
        public bool ProcessDialogKey(UIKeyEventArgs e)
        {
            return this.ProcessDialogKey(e, _htmlVisualRoot.RootCssBox);
        }
        public bool ProcessDialogKey(UIKeyEventArgs e, CssBox startAt)
        {
            //send focus to current input element
            return false;
        }
        public void KeyUp(UIKeyEventArgs e)
        {
            this.KeyUp(e, _htmlVisualRoot.RootCssBox);
        }
        public void KeyUp(UIKeyEventArgs e, CssBox startAt)
        {
        }

        static void ForEachOnlyEventPortalBubbleUp(UIEventArgs e, CssBoxHitChain hitPointChain, System.Func<IEventPortal, bool> eventPortalAction)
        {
            //only listener that need tunnel down 
            for (int i = hitPointChain.Count - 1; i >= 0; --i)
            {
                //propagate up 
                var hitInfo = hitPointChain.GetHitInfo(i);
                IEventPortal controller;
                switch (hitInfo.hitObjectKind)
                {
                    default:
                        {
                            continue;
                        }
                    case HitObjectKind.Run:
                        {
                            CssRun run = (CssRun)hitInfo.hitObject;
                            controller = CssBox.UnsafeGetController(run.OwnerBox) as IEventPortal;
                        }
                        break;
                    case HitObjectKind.CssBox:
                        {
                            CssBox box = (CssBox)hitInfo.hitObject;
                            controller = CssBox.UnsafeGetController(box) as IEventPortal;
                        }
                        break;
                }

                //---------------------
                if (controller != null)
                {
                    e.SetLocation(hitInfo.localX, hitInfo.localY);
                    if (eventPortalAction(controller))
                    {
                        return;
                    }
                }
            }
        }

        static void ForEachEventListenerBubbleUp(UIEventArgs e, CssBoxHitChain hitChain, System.Func<bool> listenerAction)
        {
            for (int i = hitChain.Count - 1; i >= 0; --i)
            {
                //propagate up 
                var hitInfo = hitChain.GetHitInfo(i);
                IUIEventListener controller = null;
                switch (hitInfo.hitObjectKind)
                {
                    default:
                        {
                            continue;
                        }
                    case HitObjectKind.Run:
                        {
                            CssRun run = (CssRun)hitInfo.hitObject;
                            controller = CssBox.UnsafeGetController(run.OwnerBox) as IUIEventListener;
                        }
                        break;
                    case HitObjectKind.CssBox:
                        {
                            CssBox box = (CssBox)hitInfo.hitObject;
                            controller = CssBox.UnsafeGetController(box) as IUIEventListener;
                        }
                        break;
                }

                //---------------------
                if (controller != null)
                {
                    //found controller
                    if (e.SourceHitElement == null)
                    {
                        e.SetSourceHitObject(controller);
                    }

                    e.SetCurrentContextElement(controller);
                    e.SetLocation(hitInfo.localX, hitInfo.localY);
                    if (listenerAction())
                    {
                        return;
                    }
                }
            }
        }


        CssBoxHitChain GetFreeHitChain()
        {
            if (_hitChainPools.Count > 0)
            {
                return _hitChainPools.Pop();
            }
            else
            {
                return new CssBoxHitChain();
            }
        }
        void ReleaseHitChain(CssBoxHitChain hitChain)
        {
            hitChain.Clear();
            _hitChainPools.Push(hitChain);
        }
    }
}