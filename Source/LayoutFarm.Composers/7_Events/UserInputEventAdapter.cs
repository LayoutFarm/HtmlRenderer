// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;
using LayoutFarm.RenderBoxes;
namespace LayoutFarm.UI
{

    partial class UserInputEventAdapter
    {

        //current hit chain        
        HitChain _previousChain = new HitChain();
        Stack<HitChain> hitChainStack = new Stack<HitChain>();

        UIHoverMonitorTask hoverMonitoringTask;
        int msgChainVersion;


        IEventListener currentKbFocusElem;
        IEventListener currentMouseActiveElement;
        IEventListener currentMouseDown;

        DateTime lastTimeMouseUp;
        const int DOUBLE_CLICK_SENSE = 150;//ms

        MyRootGraphic rootgfx;

        internal UserInputEventAdapter(MyRootGraphic rootgfx)
        {

            this.rootgfx = rootgfx;
            this.hoverMonitoringTask = new UIHoverMonitorTask(OnMouseHover);
#if DEBUG
            this._previousChain.dbugHitTracker = this.dbugRootGraphic.dbugHitTracker;
#endif
        }


        public void EnableGraphicsTimer()
        {
            this.rootgfx.GfxTimerEnabled = true;
        }
        public void DisableGraphicsTimer()
        {
            this.rootgfx.GfxTimerEnabled = false;
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
        void PrepareRenderAndFlushAccumGraphics()
        {
            this.rootgfx.PrepareRender();
            this.rootgfx.FlushAccumGraphics();
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
                    currentKbFocusElem.ListenLostKeyboardFocus(null);
                }
                //2. keyboard focus
                currentKbFocusElem = value;
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
                    if (elem != null && elem.VisibleAndHasParent)
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
                commonElement = this.rootgfx.TopWindowRenderBox;
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
                //------------------------------
                //1. origin object 
                SetEventOrigin(e, hitPointChain);
                //------------------------------ 
                var prevMouseDownElement = this.currentMouseDown;

                //portal                
                ForEachOnlyEventPortalBubbleUp(e, hitPointChain, (portal) =>
                {
                    portal.PortalMouseDown(e);
                    //*****
                    this.currentMouseDown = e.CurrentContextElement;


                    return true;
                });

                //------------------------------
                //use events
                if (!e.CancelBubbling)
                {

                    e.CurrentContextElement = this.currentMouseDown = null; //clear 
                    ForEachEventListenerBubbleUp(e, hitPointChain, (listener) =>
                    {
                        this.currentMouseDown = e.CurrentContextElement;
                        listener.ListenMouseDown(e);
                        //------------------------------------------------------- 
                        bool cancelMouseBubbling = e.CancelBubbling;
                        if (prevMouseDownElement != null &&
                            prevMouseDownElement != listener)
                        {
                            prevMouseDownElement.ListenLostMouseFocus(e);
                            prevMouseDownElement = null;//clear
                        }
                        //------------------------------------------------------- 
                        if (!cancelMouseBubbling && currentMouseDown.BypassAllMouseEvents)
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    });
                }

                if (prevMouseDownElement != currentMouseDown &&
                    prevMouseDownElement != null)
                {
                    prevMouseDownElement.ListenLostMouseFocus(e);
                    prevMouseDownElement = null;
                }


            }
            //---------------------------------------------------------------

#if DEBUG
            RootGraphic visualroot = this.dbugRootGraphic;

            if (visualroot.dbug_RecordHitChain)
            {
                visualroot.dbug_rootHitChainMsg.Clear();


                HitInfo hitInfo;
                for (int tt = hitPointChain.Count - 1; tt >= 0; --tt)
                {
                    hitInfo = hitPointChain.GetHitInfo(tt);
                    RenderElement ve = hitInfo.hitElement;
                    if (ve != null)
                    {
                        ve.dbug_WriteOwnerLayerInfo(visualroot, tt);
                        ve.dbug_WriteOwnerLineInfo(visualroot, tt);

                        string hit_info = new string('.', tt) + " [" + tt + "] "
                            + "(" + hitInfo.point.X + "," + hitInfo.point.Y + ") "
                            + ve.dbug_FullElementDescription();
                        visualroot.dbug_rootHitChainMsg.AddLast(new dbugLayoutMsg(ve, hit_info));
                    }
                }
            }
#endif
            SwapHitChain(hitPointChain);
            this.PrepareRenderAndFlushAccumGraphics();

            if (local_msgVersion != msgChainVersion)
            {
                return;
            }
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

            SetEventOrigin(e, hitPointChain);
            //-------------------------------------------------------
            ForEachOnlyEventPortalBubbleUp(e, hitPointChain, (portal) =>
            {
                portal.PortalMouseMove(e);
                return true;
            });
            //-------------------------------------------------------  
            if (!e.CancelBubbling)
            {
                bool foundSomeHit = false;
                ForEachEventListenerBubbleUp(e, hitPointChain, (listener) =>
                {
                    foundSomeHit = true;
                    bool isFirstMouseEnter = false;
                    if (currentMouseActiveElement != null &&
                        currentMouseActiveElement != listener)
                    {
                        currentMouseActiveElement.ListenMouseLeave(e);
                        isFirstMouseEnter = true;
                    }
                    if (!e.IsCanceled)
                    {
                        currentMouseActiveElement = listener;
                        e.IsFirstMouseEnter = isFirstMouseEnter;
                        currentMouseActiveElement.ListenMouseMove(e);
                        e.IsFirstMouseEnter = false;
                    }
                    return true;//stop
                });

                if (!foundSomeHit && currentMouseActiveElement != null)
                {
                    currentMouseActiveElement.ListenMouseLeave(e);
                    if (!e.IsCanceled)
                    {
                        currentMouseActiveElement = null;
                    }
                }
            }


            SwapHitChain(hitPointChain);
            this.PrepareRenderAndFlushAccumGraphics();

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
                SetEventOrigin(e, hitPointChain);
                //--------------------------------------------------------------- 
                ForEachOnlyEventPortalBubbleUp(e, hitPointChain, (portal) =>
                {
                    portal.PortalMouseUp(e);
                    return true;
                });
                //---------------------------------------------------------------
                if (!e.CancelBubbling)
                {
                    ForEachEventListenerBubbleUp(e, hitPointChain, (listener) =>
                    {
                        listener.ListenMouseUp(e);
                        return true;
                    });
                }
                //---------------------------------------------------------------
                if (!e.CancelBubbling)
                {
                    if (isAlsoDoubleClick)
                    {
                        ForEachEventListenerBubbleUp(e, hitPointChain, (listener) =>
                        {
                            listener.ListenMouseDoubleClick(e);
                            return true;
                        });
                    }
                    else
                    {

                        ForEachEventListenerBubbleUp(e, hitPointChain, (listener) =>
                        {
                            listener.ListenMouseClick(e);
                            return true;
                        });
                    }

                }
            }
            SwapHitChain(hitPointChain);
            this.PrepareRenderAndFlushAccumGraphics();
        }
        protected void OnKeyDown(UIKeyEventArgs e)
        {
            if (currentKbFocusElem != null)
            {
                e.SourceHitElement = currentKbFocusElem;
                currentKbFocusElem.ListenKeyDown(e);

                this.PrepareRenderAndFlushAccumGraphics();
            }
        }
        protected void OnKeyUp(UIKeyEventArgs e)
        {
            if (currentKbFocusElem != null)
            {
                e.SourceHitElement = currentKbFocusElem;
                currentKbFocusElem.ListenKeyUp(e);
                this.PrepareRenderAndFlushAccumGraphics();
            }
        }
        protected void OnKeyPress(UIKeyEventArgs e)
        {

            if (currentKbFocusElem != null)
            {
                e.SourceHitElement = currentKbFocusElem;
                currentKbFocusElem.ListenKeyPress(e);
                this.PrepareRenderAndFlushAccumGraphics();
            }
        }
        protected bool OnProcessDialogKey(UIKeyEventArgs e)
        {
            bool result = false;
            if (currentKbFocusElem != null)
            {
                e.SourceHitElement = currentKbFocusElem;
                result = currentKbFocusElem.ListenProcessDialogKey(e);
                if (result)
                {
                    this.PrepareRenderAndFlushAccumGraphics();
                }
            }
            return result;
        }

        //===================================================================
        delegate bool EventPortalAction(IUserEventPortal evPortal);
        delegate bool EventListenerAction(IEventListener listener);

        static void ForEachOnlyEventPortalBubbleUp(UIEventArgs e, HitChain hitPointChain, EventPortalAction eventPortalAction)
        {
            for (int i = hitPointChain.Count - 1; i >= 0; --i)
            {
                HitInfo hitPoint = hitPointChain.GetHitInfo(i);
                object currentHitElement = hitPoint.hitElement.GetController();
                IUserEventPortal eventPortal = currentHitElement as IUserEventPortal;
                if (eventPortal != null)
                {

                    var ppp = hitPoint.point;
                    e.CurrentContextElement = currentHitElement as IEventListener;
                    e.SetLocation(ppp.X, ppp.Y);
                    if (eventPortalAction(eventPortal))
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