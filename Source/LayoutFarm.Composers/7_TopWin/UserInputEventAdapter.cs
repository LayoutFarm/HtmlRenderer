//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using LayoutFarm.Drawing;

namespace LayoutFarm.UI
{

    partial class UserInputEventAdapter
    {

        //current hit chain        
        HitChain _previousChain = new HitChain();
        Stack<HitChain> hitChainStack = new Stack<HitChain>();

        UIHoverMonitorTask hoverMonitoringTask;
        int msgChainVersion;
        TopWindowRenderBox topwin;
        IEventListener currentKbFocusElem;
        IEventListener currentMouseActiveElement;
        DateTime lastTimeMouseUp;
        const int DOUBLE_CLICK_SENSE = 150;//ms

        RootGraphic rootgfx;
        public UserInputEventAdapter()
        {

        }
        public void Bind(TopWindowRenderBox topwin)
        {

            this.topwin = topwin;
            this.rootgfx = topwin.Root;
            this.hoverMonitoringTask = new UIHoverMonitorTask(OnMouseHover);

#if DEBUG
            this._previousChain.dbugHitTracker = this.dbugRootGraphic.dbugHitTracker;
#endif
        }

#if DEBUG
        RootGraphic dbugRootGraphic
        {
            get { return rootgfx; }
        }
#endif

        HitChain GetFreeHitChain()
        {
            if (hitChainStack.Count > 0)
            {
                return hitChainStack.Pop();
            }
            else
            {

#if DEBUG
                var hitChain = new HitChain();
                hitChain.dbugHitTracker = this.dbugRootGraphic.dbugHitTracker;
                return hitChain;
#else
                return new HitChain();
#endif

            }
        }
        void RelaseHitChain(HitChain hitChain)
        {
            hitChain.ClearAll();
            this.hitChainStack.Push(hitChain);
        }
        void SwapHitChain(HitChain hitChain)
        {
            RelaseHitChain(this._previousChain);
            this._previousChain = hitChain;
        }

        RootGraphic MyRootGraphic
        {
            get { return rootgfx; }
        }
        //---------------------------------------------------------------------
        bool DisableGraphicOutputFlush
        {
            get { return this.MyRootGraphic.DisableGraphicOutputFlush; }
            set { this.MyRootGraphic.DisableGraphicOutputFlush = value; }
        }
        void FlushAccumGraphicUpdate()
        {
            this.MyRootGraphic.FlushAccumGraphicUpdate(this.topwin);
        }

        public IEventListener CurrentKeyboardFocusedElement
        {
            get
            {

                return this.currentKbFocusElem;
            }
            set
            {
                //1. lost keyboard focus
                if (this.currentKbFocusElem != null && this.currentKbFocusElem != value)
                {

                }
                //2. keyboard focus
                currentKbFocusElem = value;

                //1. send lost focus to existing ui

                //if (currentKeyboardFocusedElement != null)
                //{
                //    if (currentKeyboardFocusedElement == value)
                //    {
                //        return;
                //    }

                //    UIFocusEventArgs focusEventArg = eventStock.GetFreeFocusEventArgs(value, currentKeyboardFocusedElement);
                //    focusEventArg.SetWinRoot(topwin);
                //    eventStock.ReleaseEventArgs(focusEventArg);
                //}
                //currentKeyboardFocusedElement = value;
                //if (currentKeyboardFocusedElement != null)
                //{
                //    UIFocusEventArgs focusEventArg = eventStock.GetFreeFocusEventArgs(value, currentKeyboardFocusedElement);
                //    focusEventArg.SetWinRoot(topwin);
                //    Point globalLocation = value.GetGlobalLocation();
                //    kbFocusGlobalX = globalLocation.X;
                //    kbFocusGlobalY = globalLocation.Y;
                //    focusEventArg.SetWinRoot(topwin);
                //    eventStock.ReleaseEventArgs(focusEventArg);
                //    if (CurrentFocusElementChanged != null)
                //    {
                //        CurrentFocusElementChanged.Invoke(this, EventArgs.Empty);
                //    }
                //}
                //else
                //{
                //    kbFocusGlobalX = 0;
                //    kbFocusGlobalY = 0;
                //}
            }
        }
        static void SetEventOrigin(UIEventArgs e, HitChain hitChain)
        {
            int count = hitChain.Count;
            if (count > 0)
            {
                var hitInfo = hitChain.GetHitInfo(count - 1);
                e.SourceHitElement = hitInfo.hitElement;
            }
        }
        void ClearAllFocus()
        {
            CurrentKeyboardFocusedElement = null;
        }

        static RenderElement HitTestOnPreviousChain(HitChain hitPointChain, HitChain previousChain, int x, int y)
        {

            if (previousChain.Count > 0)
            {

                previousChain.SetStartTestPoint(x, y);
                //test on prev chain top to bottom
                int j = previousChain.Count;
                for (int i = 0; i < j; ++i)
                {
                    HitInfo hitInfo = previousChain.GetHitInfo(i);
                    RenderElement elem = hitInfo.hitElement;
                    if (elem != null && elem.IsTestable)
                    {
                        if (elem.Contains(hitInfo.point))
                        {
                            RenderElement foundOverlapChild = elem.FindOverlapedChildElementAtPoint(elem, hitInfo.point);
                            if (foundOverlapChild == null)
                            {
                                Point leftTop = elem.Location;
                                hitPointChain.OffsetTestPoint(leftTop.X, leftTop.Y);
                                hitPointChain.AddHitObject(elem);
                                //add to chain
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
            //---------------------------------
            if (hitPointChain.Count > 0)
            {
                return hitPointChain.GetHitInfo(hitPointChain.Count - 1).hitElement;
            }
            else
            {
                return null;
            }
        }

        void HitTestCoreWithPrevChainHint(HitChain hitPointChain, HitChain previousChain, int x, int y)
        {
            //---------------------------------
            //test on previous chain first , find common element 
            hitPointChain.ClearAll();
            hitPointChain.SetStartTestPoint(x, y);
            RenderElement commonElement = HitTestOnPreviousChain(hitPointChain, previousChain, x, y);
            if (commonElement == null)
            {
                commonElement = this.topwin;
            }
            commonElement.HitTestCore(hitPointChain);
        }

        protected void OnMouseWheel(UIMouseEventArgs e)
        {
            //only on mouse active element
            if (currentMouseActiveElement != null)
            {
                currentMouseActiveElement.ListenMouseWheel(e);
            }
        }


        protected void OnMouseDown(UIMouseEventArgs e)
        {

#if DEBUG
            if (this.dbugRootGraphic.dbugEnableGraphicInvalidateTrace)
            {
                this.dbugRootGraphic.dbugGraphicInvalidateTracer.WriteInfo("================");
                this.dbugRootGraphic.dbugGraphicInvalidateTracer.WriteInfo("MOUSEDOWN");
                this.dbugRootGraphic.dbugGraphicInvalidateTracer.WriteInfo("================");
            }
#endif
            msgChainVersion = 1;
            int local_msgVersion = 1;

            HitChain hitPointChain = GetFreeHitChain();
            HitTestCoreWithPrevChainHint(hitPointChain, this._previousChain, e.X, e.Y);

            int hitCount = hitPointChain.Count;

            RenderElement hitElement = hitPointChain.TopMostElement;
            if (hitCount > 0)
            {
                DisableGraphicOutputFlush = true;
                //------------------------------
                //1. origin object 
                SetEventOrigin(e, hitPointChain);
                //------------------------------
                //portal
                ForEachOnlyEventPortalBubbleUp(e, hitPointChain, (hitInfo, portal) =>
                {
                    portal.PortalMouseDown(e);
                    if (e.CurrentContextElement.AcceptKeyboardFocus)
                    {
                        this.CurrentKeyboardFocusedElement = e.CurrentContextElement;
                    }
                    return true;
                });

                //------------------------------
                //use events
                if (!e.CancelBubbling)
                {

                    ForEachEventListenerBubbleUp(e, hitPointChain, (listener) =>
                    {
                        listener.ListenMouseDown(e);
                        if (e.CurrentContextElement.AcceptKeyboardFocus)
                        {
                            this.CurrentKeyboardFocusedElement = e.CurrentContextElement;
                        }
                        return true;
                    });
                }
            }
            //---------------------------------------------------------------

#if DEBUG
            RootGraphic visualroot = this.dbugRootGraphic;
            if (visualroot.dbug_RecordHitChain)
            {
                visualroot.dbug_rootHitChainMsg.Clear();
                int i = 0;
                //foreach (HitPoint hp in hitPointChain.dbugGetHitPairIter())
                //{

                //    RenderElement ve = hp.hitObject as RenderElement;
                //    if (ve != null)
                //    {
                //        ve.dbug_WriteOwnerLayerInfo(visualroot, i);
                //        ve.dbug_WriteOwnerLineInfo(visualroot, i);

                //        string hit_info = new string('.', i) + " [" + i + "] "
                //            + "(" + hp.point.X + "," + hp.point.Y + ") "
                //            + ve.dbug_FullElementDescription();
                //        visualroot.dbug_rootHitChainMsg.AddLast(new dbugLayoutMsg(ve, hit_info));
                //    }
                //    i++;
                //}
            }
#endif


            SwapHitChain(hitPointChain);

            if (local_msgVersion != msgChainVersion)
            {
                return;
            }
            //if (hitElement.Focusable)
            //{
            //    this.CurrentKeyboardFocusedElement = hitElement.GetController() as IEventListener;
            //}
            DisableGraphicOutputFlush = false;
            FlushAccumGraphicUpdate();

#if DEBUG
            visualroot.dbugHitTracker.Write("stop-mousedown");
            visualroot.dbugHitTracker.Play = false;
#endif
        }
        protected void OnMouseMove(UIMouseEventArgs e)
        {

            HitChain hitPointChain = GetFreeHitChain();
            HitTestCoreWithPrevChainHint(hitPointChain, this._previousChain, e.X, e.Y);
            //-------------------------------------------------------
            //when mousemove -> reset hover!
            hoverMonitoringTask.Reset();
            hoverMonitoringTask.Enabled = true;
            //-------------------------------------------------------
            DisableGraphicOutputFlush = true;
            this.isDragging = e.IsDragging;
            SetEventOrigin(e, hitPointChain);
            //-------------------------------------------------------
            ForEachOnlyEventPortalBubbleUp(e, hitPointChain, (hitInfo, portal) =>
            {
                portal.PortalMouseMove(e);
                return true;
            });
            //-------------------------------------------------------  
            if (!e.CancelBubbling)
            {
                bool passloop = false;

                ForEachEventListenerBubbleUp(e, hitPointChain, (listener) =>
                {
                    passloop = true; 

                    if (currentMouseActiveElement != null &&
                        currentMouseActiveElement != listener)
                    {
                        currentMouseActiveElement.ListenMouseLeave(e);
                        currentMouseActiveElement = null;
                    }

                    if (currentMouseActiveElement == listener)
                    {
                        e.JustEnter = false;
                        currentMouseActiveElement.ListenMouseMove(e);
                    }
                    else
                    {
                        currentMouseActiveElement = listener;
                        e.JustEnter = true;
                        currentMouseActiveElement.ListenMouseMove(e);
                    }

                    return true;//stop
                });

                if (!passloop && currentMouseActiveElement != null)
                {
                    currentMouseActiveElement.ListenMouseLeave(e);
                    if (!isDragging)
                    {
                        currentMouseActiveElement = null;
                    }
                }
            }

            DisableGraphicOutputFlush = false;
            SwapHitChain(hitPointChain);

        }

        protected void OnGotFocus(UIFocusEventArgs e)
        {


        }
        protected void OnLostFocus(UIFocusEventArgs e)
        {

        }
        protected void OnMouseUp(UIMouseEventArgs e)
        {


            DateTime snapMouseUpTime = DateTime.Now;
            TimeSpan timediff = snapMouseUpTime - lastTimeMouseUp;
            bool isAlsoDoubleClick = timediff.Milliseconds < DOUBLE_CLICK_SENSE;
            this.lastTimeMouseUp = snapMouseUpTime;

            //--------------------------------------------
#if DEBUG

            if (this.dbugRootGraphic.dbugEnableGraphicInvalidateTrace)
            {
                this.dbugRootGraphic.dbugGraphicInvalidateTracer.WriteInfo("================");
                this.dbugRootGraphic.dbugGraphicInvalidateTracer.WriteInfo("MOUSEUP");
                this.dbugRootGraphic.dbugGraphicInvalidateTracer.WriteInfo("================");
            }
#endif

            HitChain hitPointChain = GetFreeHitChain();

            HitTestCoreWithPrevChainHint(hitPointChain, this._previousChain, e.X, e.Y);
            int hitCount = hitPointChain.Count;

            if (hitCount > 0)
            {

                DisableGraphicOutputFlush = true;
                SetEventOrigin(e, hitPointChain);
                //--------------------------------------------------------------- 
                ForEachOnlyEventPortalBubbleUp(e, hitPointChain, (hitInfo, portal) =>
                {
                    portal.PortalMouseUp(e);
                    if (e.CurrentContextElement.AcceptKeyboardFocus)
                    {
                        this.CurrentKeyboardFocusedElement = e.CurrentContextElement;
                    }
                    return true;
                });

                //---------------------------------------------------------------
                if (!e.CancelBubbling)
                {

                    ForEachEventListenerBubbleUp(e, hitPointChain, (listener) =>
                    {
                        listener.ListenMouseUp(e);
                        if (e.CurrentContextElement.AcceptKeyboardFocus)
                        {
                            this.CurrentKeyboardFocusedElement = e.CurrentContextElement;
                        }
                        return true;
                    });
                }

                if (!e.CancelBubbling)
                {
                    if (isAlsoDoubleClick)
                    {

                        ForEachEventListenerBubbleUp(e, hitPointChain, (listener) =>
                        {
                            listener.ListenMouseDoubleClick(e); //double click 
                            if (e.CurrentContextElement.AcceptKeyboardFocus)
                            {
                                this.CurrentKeyboardFocusedElement = e.CurrentContextElement;
                            }
                            return true;
                        });
                    }
                    else
                    {

                        ForEachEventListenerBubbleUp(e, hitPointChain, (listener) =>
                        {
                            listener.ListenMouseClick(e);
                            if (e.CurrentContextElement.AcceptKeyboardFocus)
                            {
                                this.CurrentKeyboardFocusedElement = e.CurrentContextElement;
                            }
                            return true;
                        });
                    }

                }

                //---------------------------------------------------------------
                DisableGraphicOutputFlush = false;
                FlushAccumGraphicUpdate();
            }
            SwapHitChain(hitPointChain);
        }
        protected void OnKeyDown(UIKeyEventArgs e)
        {
            if (currentKbFocusElem != null)
            {
                e.SourceHitElement = currentKbFocusElem;
                currentKbFocusElem.ListenKeyDown(e);
            }
        }
        protected void OnKeyUp(UIKeyEventArgs e)
        {
            if (currentKbFocusElem != null)
            {
                e.SourceHitElement = currentKbFocusElem;
                currentKbFocusElem.ListenKeyUp(e);
            }
        }
        protected void OnKeyPress(UIKeyEventArgs e)
        {

            if (currentKbFocusElem != null)
            {
                e.SourceHitElement = currentKbFocusElem;
                currentKbFocusElem.ListenKeyPress(e);
            }
        }
        protected bool OnProcessDialogKey(UIKeyEventArgs e)
        {
            bool result = false;
            if (currentKbFocusElem != null)
            {
                e.SourceHitElement = currentKbFocusElem;
                result = currentKbFocusElem.ListenProcessDialogKey(e);
            }
            return result;
        }

        //===================================================================
        delegate bool EventPortalAction(HitInfo hitInfo, IUserEventPortal evPortal);
        delegate bool EventListenerAction(IEventListener listener);

        static void ForEachOnlyEventPortalBubbleUp(UIEventArgs e, HitChain hitPointChain, EventPortalAction eventPortalAction)
        {
            for (int i = hitPointChain.Count - 1; i >= 0; --i)
            {
                HitInfo hitPoint = hitPointChain.GetHitInfo(i);
                IUserEventPortal eventPortal = hitPoint.hitElement.GetController() as IUserEventPortal;
                if (eventPortal != null)
                {
                    e.Location = hitPoint.point;
                    if (eventPortalAction(hitPoint, eventPortal))
                    {
                        return;
                    }
                }
            }
        }
        static void ForEachEventListenerBubbleUp(UIEventArgs e, HitChain hitPointChain, EventListenerAction listenerAction)
        {
            HitInfo hitInfo;
            for (int i = hitPointChain.Count - 1; i >= 0; --i)
            {
                hitInfo = hitPointChain.GetHitInfo(i);
                IEventListener listener = hitInfo.hitElement.GetController() as IEventListener;
                if (listener != null)
                {
                    e.Location = hitInfo.point;
                    e.CurrentContextElement = listener;

                    if (listenerAction(listener))
                    {
                        return;
                    }
                }
            }
        }

        //--------------------------------------------------------------------
        protected void OnMouseHover(object sender, EventArgs e)
        {
            return;
            //HitTestCoreWithPrevChainHint(hitPointChain.LastestRootX, hitPointChain.LastestRootY);
            //RenderElement hitElement = this.hitPointChain.CurrentHitElement as RenderElement;
            //if (hitElement != null && hitElement.IsTestable)
            //{
            //    DisableGraphicOutputFlush = true;
            //    Point hitElementGlobalLocation = hitElement.GetGlobalLocation();

            //    UIMouseEventArgs e2 = new UIMouseEventArgs();
            //    e2.WinTop = this.topwin;
            //    e2.Location = hitPointChain.CurrentHitPoint;
            //    e2.SourceHitElement = hitElement;
            //    IEventListener ui = hitElement.GetController() as IEventListener;
            //    if (ui != null)
            //    {
            //        ui.ListenMouseEvent(UIMouseEventName.MouseHover, e2);
            //    }

            //    DisableGraphicOutputFlush = false;
            //    FlushAccumGraphicUpdate();
            //}
            //hitPointChain.SwapHitChain();
            //hoverMonitoringTask.SetEnable(false, this.topwin);
        }
        //        public override void OnDragStart(UIMouseEventArgs e)
        //        {

        //#if DEBUG
        //            if (this.dbugRootGraphic.dbugEnableGraphicInvalidateTrace)
        //            {
        //                this.dbugRootGraphic.dbugGraphicInvalidateTracer.WriteInfo("================");
        //                this.dbugRootGraphic.dbugGraphicInvalidateTracer.WriteInfo("START_DRAG");
        //                this.dbugRootGraphic.dbugGraphicInvalidateTracer.WriteInfo("================");
        //            }
        //#endif

        //            HitTestCoreWithPrevChainHint(
        //              hitPointChain.LastestRootX,
        //              hitPointChain.LastestRootY);

        //            DisableGraphicOutputFlush = true;
        //            this.currentDragElem = null;

        //            //-----------------------------------------------------------------------

        //            ForEachEventListenerPreviewBubbleUp(this.hitPointChain, (hitobj, listener) =>
        //            {
        //                listener.PortalMouseMove(e);
        //                return true;
        //            });

        //            //-----------------------------------------------------------------------

        //            ForEachEventListenerBubbleUp(this.hitPointChain, (hit, listener) =>
        //            {
        //                currentDragElem = listener;
        //                listener.ListenDragEvent(UIDragEventName.DragStart, e);
        //                return true;
        //            });
        //            DisableGraphicOutputFlush = false;
        //            FlushAccumGraphicUpdate();

        //            hitPointChain.SwapHitChain();
        //        }
        //        public override void OnDrag(UIMouseEventArgs e)
        //        {
        //            if (currentDragElem == null)
        //            {
        //                return;
        //            }

        //#if DEBUG
        //            this.dbugRootGraphic.dbugEventIsDragging = true;
        //#endif

        //            //if (currentDragingElement == null)
        //            //{

        //            //    return;
        //            //}
        //            //else
        //            //{
        //            //}

        //            //--------------

        //            DisableGraphicOutputFlush = true;

        //            currentDragElem.ListenDragEvent(UIDragEventName.Dragging, e);

        //            DisableGraphicOutputFlush = false;
        //            FlushAccumGraphicUpdate();

        //            //Point globalDragingElementLocation = currentDragingElement.GetGlobalLocation();
        //            //e.TranslateCanvasOrigin(globalDragingElementLocation);
        //            //e.SourceHitElement = currentDragingElement;
        //            //Point dragPoint = hitPointChain.PrevHitPoint;
        //            //dragPoint.Offset(currentXDistanceFromDragPoint, currentYDistanceFromDragPoint);
        //            //e.Location = dragPoint;
        //            //e.DragingElement = currentDragingElement;

        //            //IEventListener ui = currentDragingElement.GetController() as IEventListener;
        //            //if (ui != null)
        //            //{
        //            //    ui.ListenDragEvent(UIDragEventName.Dragging, e);
        //            //}
        //            //e.TranslateCanvasOriginBack();


        //        }


        //        public override void OnDragStop(UIMouseEventArgs e)
        //        {

        //            if (currentDragElem == null)
        //            {
        //                return;
        //            }
        //#if DEBUG
        //            this.dbugRootGraphic.dbugEventIsDragging = false;
        //#endif

        //            DisableGraphicOutputFlush = true;

        //            currentDragElem.ListenDragEvent(UIDragEventName.DragStop, e);

        //            DisableGraphicOutputFlush = false;
        //            FlushAccumGraphicUpdate();

        //            //if (currentDragingElement == null)
        //            //{
        //            //    return;
        //            //}

        //            //DisableGraphicOutputFlush = true;

        //            //Point globalDragingElementLocation = currentDragingElement.GetGlobalLocation();
        //            //e.TranslateCanvasOrigin(globalDragingElementLocation);

        //            //Point dragPoint = hitPointChain.PrevHitPoint;
        //            //dragPoint.Offset(currentXDistanceFromDragPoint, currentYDistanceFromDragPoint);
        //            //e.Location = dragPoint;

        //            //e.SourceHitElement = currentDragingElement;
        //            //var script = currentDragingElement.GetController() as IEventListener;
        //            //if (script != null)
        //            //{
        //            //    script.ListenDragEvent(UIDragEventName.DragStop, e);
        //            //}

        //            //e.TranslateCanvasOriginBack();

        //            //UIMouseEventArgs d_eventArg = new UIMouseEventArgs();
        //            //if (hitPointChain.DragHitElementCount > 0)
        //            //{
        //            //    ForEachDraggingObjects(this.hitPointChain, (hitobj, listener) =>
        //            //    {
        //            //        //d_eventArg.TranslateCanvasOrigin(globalLocation);
        //            //        //d_eventArg.SourceHitElement = elem;
        //            //        //d_eventArg.DragingElement = currentDragingElement;

        //            //        //var script2 = elem.GetController();
        //            //        //if (script2 != null)
        //            //        //{
        //            //        //}

        //            //        //d_eventArg.TranslateCanvasOriginBack();
        //            //        return true;
        //            //    });
        //            //    //foreach (RenderElement elem in hitPointChain.GetDragHitElementIter())
        //            //    //{
        //            //    //    Point globalLocation = elem.GetGlobalLocation();
        //            //    //    d_eventArg.TranslateCanvasOrigin(globalLocation);
        //            //    //    d_eventArg.SourceHitElement = elem;
        //            //    //    d_eventArg.DragingElement = currentDragingElement;

        //            //    //    var script2 = elem.GetController();
        //            //    //    if (script2 != null)
        //            //    //    {
        //            //    //    }

        //            //    //    d_eventArg.TranslateCanvasOriginBack();
        //            //    //}
        //            //} 
        //            DisableGraphicOutputFlush = false;
        //            FlushAccumGraphicUpdate();
        //        }


        void BroadcastDragHitEvents(UIMouseEventArgs e)
        {


            //Point globalDragingElementLocation = currentDragingElement.GetGlobalLocation();
            //Rectangle dragRect = currentDragingElement.GetGlobalRect();

            //VisualDrawingChain drawingChain = this.WinRootPrepareRenderingChain(dragRect);

            //List<RenderElement> selVisualElements = drawingChain.selectedVisualElements;
            //int j = selVisualElements.Count;
            //LinkedList<RenderElement> underlyingElements = new LinkedList<RenderElement>();
            //for (int i = j - 1; i > -1; --i)
            //{

            //    if (selVisualElements[i].ListeningDragEvent)
            //    {
            //        underlyingElements.AddLast(selVisualElements[i]);
            //    }
            //}

            //if (underlyingElements.Count > 0)
            //{
            //    foreach (RenderElement underlyingUI in underlyingElements)
            //    {

            //        if (underlyingUI.IsDragedOver)
            //        {   
            //            hitPointChain.RemoveDragHitElement(underlyingUI);
            //            underlyingUI.IsDragedOver = false;
            //        }
            //    }
            //}
            //UIMouseEventArgs d_eventArg = UIMouseEventArgs.GetFreeDragEventArgs();

            //if (hitPointChain.DragHitElementCount > 0)
            //{
            //    foreach (RenderElement elem in hitPointChain.GetDragHitElementIter())
            //    {
            //        Point globalLocation = elem.GetGlobalLocation();
            //        d_eventArg.TranslateCanvasOrigin(globalLocation);
            //        d_eventArg.SourceVisualElement = elem;
            //        var script = elem.GetController();
            //        if (script != null)
            //        {
            //        }
            //        d_eventArg.TranslateCanvasOriginBack();
            //    }
            //}
            //hitPointChain.ClearDragHitElements();

            //foreach (RenderElement underlyingUI in underlyingElements)
            //{

            //    hitPointChain.AddDragHitElement(underlyingUI);
            //    if (underlyingUI.IsDragedOver)
            //    {
            //        Point globalLocation = underlyingUI.GetGlobalLocation();
            //        d_eventArg.TranslateCanvasOrigin(globalLocation);
            //        d_eventArg.SourceVisualElement = underlyingUI;

            //        var script = underlyingUI.GetController();
            //        if (script != null)
            //        {
            //        }

            //        d_eventArg.TranslateCanvasOriginBack();
            //    }
            //    else
            //    {
            //        underlyingUI.IsDragedOver = true;
            //        Point globalLocation = underlyingUI.GetGlobalLocation();
            //        d_eventArg.TranslateCanvasOrigin(globalLocation);
            //        d_eventArg.SourceVisualElement = underlyingUI;

            //        var script = underlyingUI.GetController();
            //        if (script != null)
            //        {
            //        }

            //        d_eventArg.TranslateCanvasOriginBack();
            //    }
            //}
            //UIMouseEventArgs.ReleaseEventArgs(d_eventArg);
        }

    }


}