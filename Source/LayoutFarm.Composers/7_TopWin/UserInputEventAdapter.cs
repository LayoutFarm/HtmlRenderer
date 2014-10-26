//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using LayoutFarm.Drawing;

namespace LayoutFarm.UI
{

    partial class UserInputEventAdapter
    {

        readonly MyHitChain hitPointChain = new MyHitChain();
        UIHoverMonitorTask hoverMonitoringTask;

        int msgChainVersion;
        TopWindowRenderBox topwin;
        IEventListener currentKbFocusElem;
        IEventListener currentMouseActiveElement;

        public UserInputEventAdapter()
        {
        }
        public void Bind(TopWindowRenderBox topwin)
        {
            this.topwin = topwin;
            this.hoverMonitoringTask = new UIHoverMonitorTask(OnMouseHover);
#if DEBUG
            hitPointChain.dbugHitTracker = this.dbugRootGraphic.dbugHitTracker;
#endif
        }

#if DEBUG
        RootGraphic dbugRootGraphic
        {
            get { return topwin.Root; }
        }
#endif


        RootGraphic MyRootGraphic
        {
            get { return topwin.Root; }
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

        void ClearAllFocus()
        {
            CurrentKeyboardFocusedElement = null;

        }
        void HitTestCoreWithPrevChainHint(int x, int y)
        {
            hitPointChain.SetStartTestPoint(x, y);
            RenderElement commonElement = hitPointChain.HitTestOnPrevChain();
            if (commonElement == null)
            {
                commonElement = this.topwin;
            }
            commonElement.HitTestCore(hitPointChain);
        }
        //--------------------------------------------------
        protected void OnDoubleClick(UIMouseEventArgs e)
        {

            HitTestCoreWithPrevChainHint(e.X, e.Y);
            ForEachEventListenerBubbleUp(this.hitPointChain, (hit, listener) =>
            {
                //on double click 
                return true;
            });

            hitPointChain.SwapHitChain();
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

            HitTestCoreWithPrevChainHint(e.X, e.Y);
            int hitCount = this.hitPointChain.Count;

            RenderElement hitElement = this.hitPointChain.CurrentHitElement;
            if (hitCount > 0)
            {
                DisableGraphicOutputFlush = true;
                //------------------------------
                //1. for some built-in event
                ForEachOnlyEventPortalBubbleUp(this.hitPointChain, (hitInfo, listener) =>
                {
                    e.CurrentContextElement = listener;
                    listener.PortalMouseDown(e);

                    var curContextElement = e.CurrentContextElement as IEventListener;
                    if (curContextElement != null && curContextElement.AcceptKeyboardFocus)
                    {
                        this.CurrentKeyboardFocusedElement = curContextElement;
                    }
                    return true;
                });
                //------------------------------
                //use events
                if (!e.CancelBubbling)
                {
                    ForEachEventListenerBubbleUp(this.hitPointChain, (hitobj, listener) =>
                    {
                        e.Location = hitobj.point;
                        e.SourceHitElement = hitobj.hitElement;
                        e.CurrentContextElement = listener;

                        listener.ListenMouseDown(e);

                        var curContextElement = e.CurrentContextElement as IEventListener;
                        if (curContextElement != null && curContextElement.AcceptKeyboardFocus)
                        {
                            this.CurrentKeyboardFocusedElement = curContextElement;
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
            hitPointChain.SwapHitChain();

            //if (hitElement.ParentLink == null)
            //{
            //    currentMouseActiveElement = null;
            //    return;
            //}

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

            HitTestCoreWithPrevChainHint(e.X, e.Y);
            //-------------------------------------------------------
            //when mousemove -> reset hover!
            hoverMonitoringTask.Reset();
            hoverMonitoringTask.Enabled = true;
            //-------------------------------------------------------
            DisableGraphicOutputFlush = true;
            this.isDragging = e.IsDragging;

            ForEachOnlyEventPortalBubbleUp(this.hitPointChain, (hitInfo, listener) =>
            {
                e.Location = hitInfo.point;
                e.CurrentContextElement = listener;
                listener.PortalMouseMove(e);

                return true;
            });
            //-------------------------------------------------------  
            if (!e.CancelBubbling)
            {
                ForEachEventListenerBubbleUp(this.hitPointChain, (hitobj, listener) =>
                {
                    e.Location = hitobj.point;
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
            }

            DisableGraphicOutputFlush = false;
            hitPointChain.SwapHitChain();
        }

        protected void OnGotFocus(UIFocusEventArgs e)
        {


        }
        protected void OnLostFocus(UIFocusEventArgs e)
        {

        }
        protected void OnMouseUp(UIMouseEventArgs e)
        {

#if DEBUG

            if (this.dbugRootGraphic.dbugEnableGraphicInvalidateTrace)
            {
                this.dbugRootGraphic.dbugGraphicInvalidateTracer.WriteInfo("================");
                this.dbugRootGraphic.dbugGraphicInvalidateTracer.WriteInfo("MOUSEUP");
                this.dbugRootGraphic.dbugGraphicInvalidateTracer.WriteInfo("================");
            }
#endif

            HitTestCoreWithPrevChainHint(e.X, e.Y);
            int hitCount = this.hitPointChain.Count;

            if (hitCount > 0)
            {

                DisableGraphicOutputFlush = true;
                //---------------------------------------------------------------

                ForEachOnlyEventPortalBubbleUp(this.hitPointChain, (hitInfo, listener) =>
                {
                    e.CurrentContextElement = listener;
                    listener.PortalMouseUp(e);
                    var curContextElement = e.CurrentContextElement as IEventListener;

                    if (curContextElement != null && curContextElement.AcceptKeyboardFocus)
                    {
                        this.CurrentKeyboardFocusedElement = curContextElement;
                    }
                    return true;
                });
                //---------------------------------------------------------------

                ForEachEventListenerBubbleUp(this.hitPointChain, (hitobj, listener) =>
                {
                    e.Location = hitobj.point;
                    e.SourceHitElement = hitobj.hitElement; //not correct!
                    e.CurrentContextElement = hitobj.hitElement;


                    listener.ListenMouseUp(e);
                    var curContextElement = e.CurrentContextElement as IEventListener;
                    if (curContextElement != null && curContextElement.AcceptKeyboardFocus)
                    {
                        this.CurrentKeyboardFocusedElement = curContextElement;
                    }
                    return true;
                });
                //---------------------------------------------------------------
                DisableGraphicOutputFlush = false;
                FlushAccumGraphicUpdate();
            }

            hitPointChain.SwapHitChain();
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
        delegate bool EventPortalAction(HitPoint hitInfo, IUserEventPortal evPortal);
        delegate bool EventListenerAction(HitPoint hitInfo, IEventListener listener);

        static void ForEachOnlyEventPortalBubbleUp(MyHitChain hitPointChain, EventPortalAction eventPortalAction)
        {
            //only listener that need tunnel down 
            for (int i = hitPointChain.Count - 1; i >= 0; --i)
            {
                HitPoint hitPoint = hitPointChain.GetHitPoint(i);
                IUserEventPortal eventPortal = hitPoint.hitElement.GetController() as IUserEventPortal;
                if (eventPortal != null &&
                    eventPortalAction(hitPoint, eventPortal))
                {
                    return;
                }
            }
        }
        static void ForEachEventListenerBubbleUp(MyHitChain hitPointChain, EventListenerAction listenerAction)
        {

            for (int i = hitPointChain.Count - 1; i >= 0; --i)
            {
                HitPoint hitPoint = hitPointChain.GetHitPoint(i);
                IEventListener listener = hitPoint.hitElement.GetController() as IEventListener;
                if (listener != null &&
                    listenerAction(hitPoint, listener))
                {
                    return;
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