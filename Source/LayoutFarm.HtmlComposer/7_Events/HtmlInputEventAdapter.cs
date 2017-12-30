//BSD, 2014-2017, WinterDev
//ArthurHub  , Jose Manuel Menendez Poo

using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
using LayoutFarm.UI;
namespace LayoutFarm.HtmlBoxes
{
    /// <summary>
    /// control Html input logic 
    /// </summary>
    public class HtmlInputEventAdapter
    {
        HtmlContainer _htmlContainer;
        CssBoxHitChain _latestMouseDownChain = null;
        //-----------------------------------------------
        DateTime lastimeMouseUp;
        IEventListener currentMouseDown;
        int _mousedownX;
        int _mousedownY;
        CssBox _mouseDownStartAt;
        //-----------------------------------------------

        ITextService ifonts;
        bool _isBinded;
        int lastDomLayoutVersion;
        const int DOUBLE_CLICK_SENSE = 150;//ms 
        Stack<CssBoxHitChain> hitChainPools = new Stack<CssBoxHitChain>();


        public HtmlInputEventAdapter()
        {

            //this.ifonts = TextServices.IFonts;
        }
        public void Bind(HtmlContainer htmlCont)
        {
            this._htmlContainer = htmlCont;
            _isBinded = htmlCont != null;
            this.ifonts = htmlCont.GetTextService();// ((RootGraphic)htmlCont.RootCssBox.GetInternalRootGfx()).IFonts;
        }
        public void Unbind()
        {
            this._htmlContainer = null;
            this._isBinded = false;
        }
        static void SetEventOrigin(UIEventArgs e, CssBoxHitChain hitChain)
        {
            int count = hitChain.Count;
            if (count > 0)
            {
                var hitInfo = hitChain.GetHitInfo(count - 1);
                e.ExactHitObject = hitInfo.hitObject;
            }
        }
        public void MouseDown(UIMouseEventArgs e, CssBox startAt)
        {
            if (!_isBinded) return;
            if (startAt == null) return;
            //---------------------------------------------------- 
            ClearPreviousSelection();
            if (_latestMouseDownChain != null)
            {
                ReleaseHitChain(_latestMouseDownChain);
                _latestMouseDownChain = null;
            }
            this.lastDomLayoutVersion = this._htmlContainer.LayoutVersion;
            //----------------------------------------------------
            int x = e.X;
            int y = e.Y;
            this._mouseDownStartAt = startAt;
            this._mousedownX = x;
            this._mousedownY = y;
            CssBoxHitChain hitChain = GetFreeHitChain();
            hitChain.SetRootGlobalPosition(x, y);
            //1. hittest 
            BoxHitUtils.HitTest(startAt, x, y, hitChain);
            //2. propagate events
            SetEventOrigin(e, hitChain);
            ForEachOnlyEventPortalBubbleUp(e, hitChain, (portal) =>
            {
                portal.PortalMouseDown(e);
                return true;
            });
            if (!e.CancelBubbling)
            {
                var prevMouseDownElement = this.currentMouseDown;
                e.CurrentContextElement = this.currentMouseDown = null; //clear
                ForEachEventListenerBubbleUp(e, hitChain, () =>
                {
                    //TODO: check accept keyboard
                    this.currentMouseDown = e.CurrentContextElement;
                    e.CurrentContextElement.ListenMouseDown(e);
                    if (prevMouseDownElement != null &&
                        prevMouseDownElement != currentMouseDown)
                    {
                        prevMouseDownElement.ListenLostMouseFocus(e);
                    }

                    return e.CancelBubbling;
                });
            }
            //----------------------------------
            //save mousedown hitchain
            this._latestMouseDownChain = hitChain;
        }
        public void MouseDown(UIMouseEventArgs e)
        {
            this.MouseDown(e, _htmlContainer.RootCssBox);
        }
        public void MouseMove(UIMouseEventArgs e, CssBox startAt)
        {
            if (!_isBinded) return;
            if (startAt == null) return;
            //-----------------------------------------
            int x = e.X;
            int y = e.Y;
            if (e.IsDragging && _latestMouseDownChain != null)
            {
                //dragging *** , if changed
                if (this._mousedownX != x || this._mousedownY != y)
                {
                    //handle mouse drag
                    CssBoxHitChain hitChain = GetFreeHitChain();
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
                            if (this._htmlContainer.LayoutVersion != this.lastDomLayoutVersion)
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
                                   this.ifonts);
                            if (newSelectionRange.IsValid)
                            {
                                this._htmlContainer.SetSelection(newSelectionRange);
                            }
                            else
                            {
                                this._htmlContainer.SetSelection(null);
                            }
                        }
                        else
                        {
                            this._htmlContainer.SetSelection(null);
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
                ReleaseHitChain(hitChain);
            }
        }
        public void MouseMove(UIMouseEventArgs e)
        {
            if (!_isBinded) return;
            //---------------------------------------------------- 
            this.MouseMove(e, this._htmlContainer.RootCssBox);
        }
        public void MouseUp(UIMouseEventArgs e, CssBox startAt)
        {
            if (!_isBinded) return;
            if (startAt == null) return;
            //---------------------------------------------------- 

            DateTime snapMouseUpTime = DateTime.Now;
            TimeSpan timediff = snapMouseUpTime - lastimeMouseUp;
            bool isAlsoDoubleClick = timediff.Milliseconds < DOUBLE_CLICK_SENSE;
            this.lastimeMouseUp = snapMouseUpTime;
            //----------------------------------------- 
            CssBoxHitChain hitChain = GetFreeHitChain();
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
            if (this._latestMouseDownChain != null)
            {
                this._latestMouseDownChain.Clear();
                //Console.WriteLine(dbugNN++);
                this._latestMouseDownChain = null;
            }
        }
        //int dbugNN = 0;
        public void MouseUp(UIMouseEventArgs e)
        {
            MouseUp(e, this._htmlContainer.RootCssBox);
        }
        public void MouseWheel(UIMouseEventArgs e)
        {
            this.MouseWheel(e, this._htmlContainer.RootCssBox);
        }
        public void MouseWheel(UIMouseEventArgs e, CssBox startAt)
        {
        }
        void ClearPreviousSelection()
        {
            this._htmlContainer.ClearPreviousSelection();
        }
        public void KeyDown(UIKeyEventArgs e)
        {
            this.KeyDown(e, this._htmlContainer.RootCssBox);
        }

        public void KeyDown(UIKeyEventArgs e, CssBox startAt)
        {
            //send focus to current input element

        }

        public void KeyPress(UIKeyEventArgs e)
        {
            this.KeyPress(e, this._htmlContainer.RootCssBox);
        }
        public void KeyPress(UIKeyEventArgs e, CssBox startAt)
        {
            //send focus to current input element

        }
        public bool ProcessDialogKey(UIKeyEventArgs e)
        {
            return this.ProcessDialogKey(e, this._htmlContainer.RootCssBox);
        }
        public bool ProcessDialogKey(UIKeyEventArgs e, CssBox startAt)
        {
            //send focus to current input element
            return false;
        }
        public void KeyUp(UIKeyEventArgs e)
        {
            this.KeyUp(e, this._htmlContainer.RootCssBox);
        }
        public void KeyUp(UIKeyEventArgs e, CssBox startAt)
        {
        }

        static void ForEachOnlyEventPortalBubbleUp(UIEventArgs e, CssBoxHitChain hitPointChain, EventPortalAction eventPortalAction)
        {
            //only listener that need tunnel down 
            for (int i = hitPointChain.Count - 1; i >= 0; --i)
            {
                //propagate up 
                var hitInfo = hitPointChain.GetHitInfo(i);
                IEventPortal controller = null;
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

        static void ForEachEventListenerBubbleUp(UIEventArgs e, CssBoxHitChain hitChain, EventListenerAction listenerAction)
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
                        }
                        break;
                    case HitObjectKind.CssBox:
                        {
                            CssBox box = (CssBox)hitInfo.hitObject;
                            controller = CssBox.UnsafeGetController(box) as IEventListener;
                        }
                        break;
                }

                //---------------------
                if (controller != null)
                {
                    //found controller
                    if (e.SourceHitElement == null)
                    {
                        e.SourceHitElement = controller;
                    }

                    e.CurrentContextElement = controller;
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