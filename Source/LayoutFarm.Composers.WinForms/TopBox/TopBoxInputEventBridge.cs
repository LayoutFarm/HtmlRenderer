//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using LayoutFarm.Drawing;

namespace LayoutFarm
{

    class TopBoxInputEventBridge
    {

        CanvasEventsStock eventStock = new CanvasEventsStock();
        RenderElement currentMouseActiveElement = null;
        RenderElement currentDragingElement = null;

        int kbFocusGlobalX = 0;
        int kbFocusGlobalY = 0;

        int currentXDistanceFromDragPoint = 0;
        int currentYDistanceFromDragPoint = 0;

        readonly MyHitChain hitPointChain = new MyHitChain();

        UIHoverMonitorTask hoverMonitoringTask;
        public event EventHandler CurrentFocusElementChanged;

        int msgChainVersion;
        MyTopWindowRenderBox topwin;
        IEventListener currentKeyboardFocusedElement;

        RootGraphic rootGraphic;
        public TopBoxInputEventBridge()
        {


        }
        public void Bind(MyTopWindowRenderBox topwin)
        {
            this.topwin = topwin;
            this.rootGraphic = topwin.Root;
            this.hoverMonitoringTask = new UIHoverMonitorTask(this.topwin, OnMouseHover);
#if DEBUG
            hitPointChain.dbugHitTracker = this.rootGraphic.dbugHitTracker;
#endif
        }


        public IEventListener CurrentKeyboardFocusedElement
        {
            get
            {

                return this.currentKeyboardFocusedElement;
            }
            set
            {


                currentKeyboardFocusedElement = value;


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

        internal RenderElement CurrentMouseFocusedElement
        {
            get
            {
                return currentMouseActiveElement;
            }
        }
        public void ClearAllFocus()
        {
            CurrentKeyboardFocusedElement = null;
            this.currentDragingElement = null;
        }
        void HitTestCoreWithPrevChainHint(int x, int y, UIEventName hitEvent)
        {
            hitPointChain.SetStartTestPoint(x, y);
            RenderElement commonElement = hitPointChain.HitTestOnPrevChain();
            if (commonElement == null)
            {
                commonElement = this.topwin;
            }
            commonElement.HitTestCore(hitPointChain);

        }
        bool DisableGraphicOutputFlush
        {
            get { return this.rootGraphic.DisableGraphicOutputFlush; }
            set { this.rootGraphic.DisableGraphicOutputFlush = value; }
        }
        void FlushAccumGraphicUpdate()
        {
            this.rootGraphic.FlushAccumGraphicUpdate(this.topwin);
        }


        //--------------------------------------------------
        public void OnDoubleClick(UIMouseEventArgs e)
        {

            HitTestCoreWithPrevChainHint(e.X, e.Y, UIEventName.DblClick);
            RenderElement hitElement = this.hitPointChain.CurrentHitElement;
            if (currentMouseActiveElement != null)
            {
                e.TranslateCanvasOrigin(kbFocusGlobalX, kbFocusGlobalY);
                e.Location = hitPointChain.CurrentHitPoint;
                e.SourceHitElement = currentMouseActiveElement;

                IEventListener ui = currentMouseActiveElement.GetController() as IEventListener;
                if (ui != null)
                {
                }
                e.TranslateCanvasOriginBack();
            }
            hitPointChain.SwapHitChain();
        }
        public void OnMouseWheel(UIMouseEventArgs e)
        {
            if (currentMouseActiveElement != null)
            {
                IEventListener ui = currentMouseActiveElement.GetController() as IEventListener;
                if (ui != null)
                {
                    ui.ListenMouseEvent(UIMouseEventName.MouseWheel, e);
                }
            }
        }

        public void OnMouseDown(UIMouseEventArgs e)
        {

#if DEBUG
            if (this.rootGraphic.dbugEnableGraphicInvalidateTrace)
            {
                this.rootGraphic.dbugGraphicInvalidateTracer.WriteInfo("================");
                this.rootGraphic.dbugGraphicInvalidateTracer.WriteInfo("MOUSEDOWN");
                this.rootGraphic.dbugGraphicInvalidateTracer.WriteInfo("================");
            }
#endif
            msgChainVersion = 1;
            int local_msgVersion = 1;

            HitTestCoreWithPrevChainHint(e.X, e.Y, UIEventName.MouseDown);
            int hitCount = this.hitPointChain.Count;

            RenderElement hitElement = this.hitPointChain.CurrentHitElement;
            if (hitCount > 0)
            {
                DisableGraphicOutputFlush = true;

                e.TranslateCanvasOrigin(kbFocusGlobalX, kbFocusGlobalY);
                e.Location = hitPointChain.CurrentHitPoint;
                e.SourceHitElement = hitElement;

                currentMouseActiveElement = hitElement;
                //---------------------------------------------------------------
                //propagate : bubble up model *** 
                bool isOk = false;
                for (int i = this.hitPointChain.Count - 1; i >= 0 && !isOk; --i)
                {

                    HitPoint hitPoint = hitPointChain.GetHitPoint(i);
                    RenderElement hitElem = hitPoint.elem;
                    if (hitElem is RenderElement)
                    {
                        IEventListener listener = hitElem.GetController() as IEventListener;
                        listener.ListenMouseEvent(UIMouseEventName.MouseDown, e);
                        //may propagate next or not 
                        hitElement = hitElem;
                        currentMouseActiveElement = hitElement;
                        break;
                    }
                    else
                    {
                        var boxChain = (HtmlRenderer.Boxes.CssBoxHitChain)hitPoint.externalObject;
                        //loop
                        for (int n = boxChain.Count - 1; n >= 0; --n)
                        {
                            var hit2 = boxChain.GetHitInfo(n);
                            var box2 = hit2.hitObject as HtmlRenderer.Boxes.CssBox;
                            var controller2 = HtmlRenderer.Boxes.CssBox.UnsafeGetController(box2);
                            if (box2 != null)
                            {
                                var box2EvListener = controller2 as IEventListener;
                                if (box2EvListener != null)
                                {

                                    e.Location = new Point(hit2.localX, hit2.localY);
                                    e.SourceHitElement = box2;
                                    box2EvListener.ListenMouseEvent(UIMouseEventName.MouseDown, e);
                                    //hitElement = box2;                                
                                    if (box2.AcceptKeyboardFocus)
                                    {
                                        this.CurrentKeyboardFocusedElement = box2EvListener;
                                    } 
                                    isOk = true;
                                    break; //break loop for
                                }
                            }
                        }
                    }
                }
                //---------------------------------------------------------------
                //if (this.hitPointChain.TailObject != null)
                //{
                //    //if has tail
                //    HtmlRenderer.Boxes.CssBoxHitChain boxChain = (HtmlRenderer.Boxes.CssBoxHitChain)this.hitPointChain.TailObject;
                //    for (int n = boxChain.Count - 1; n >= 0; --n)
                //    {
                //        var hit2 = boxChain.GetHitInfo(n);
                //        var box2 = hit2.hitObject as HtmlRenderer.Boxes.CssBox;
                //        var controller2 = HtmlRenderer.Boxes.CssBox.UnsafeGetController(box2);
                //        if (box2 != null)
                //        {
                //            var box2EvListener = controller2 as IEventListener;
                //            if (box2EvListener != null)
                //            {

                //                e.Location = new Point(hit2.localX, hit2.localY);
                //                e.SourceHitElement = box2;
                //                box2EvListener.ListenMouseEvent(UIMouseEventName.MouseDown, e);
                //                //hitElement = box2;                                
                //                if (box2.AcceptKeyboardFocus)
                //                {
                //                    this.CurrentKeyboardFocusedElement = box2EvListener;
                //                }
                //                //currentMouseActiveElement = new CssBoxHitWrapper(box2); 
                //                //if (box2.AcceptKeyboardFocus)
                //                //{
                //                //    hitElement = new HtmlRenderer.Boxes.CssBoxHitElement(box2);
                //                //}
                //                //currentMouseActiveElement = new HtmlRenderer.Boxes.CssBoxHitElement(box2); 
                //                isOk = true;
                //                break; //break loop for
                //            }
                //        }
                //    }

                //}
                //if (!isOk)
                //{
                //    for (int i = this.hitPointChain.Count - 1; i >= 0; --i)
                //    {

                //        HitPoint hitPoint = hitPointChain.GetHitPoint(i);
                //        RenderElement hitElem = hitPoint.elem;
                //        if (hitElem is RenderElement)
                //        {
                //            IEventListener listener = hitElem.GetController() as IEventListener;
                //            listener.ListenMouseEvent(UIMouseEventName.MouseDown, e);
                //            //may propagate next or not 
                //            hitElement = hitElem;
                //            currentMouseActiveElement = hitElement;
                //            break;
                //        }

                //    }
                //}
                //---------------------------------------------------------------

            }
            //---------------------------------------------------------------
            e.TranslateCanvasOriginBack();
#if DEBUG
            RootGraphic visualroot = this.rootGraphic;
            if (visualroot.dbug_RecordHitChain)
            {
                visualroot.dbug_rootHitChainMsg.Clear();
                int i = 0;
                foreach (HitPoint hp in hitPointChain.dbugGetHitPairIter())
                {

                    RenderElement ve = hp.elem;
                    if (ve != null)
                    {
                        ve.dbug_WriteOwnerLayerInfo(visualroot, i);
                        ve.dbug_WriteOwnerLineInfo(visualroot, i);

                        string hit_info = new string('.', i) + " [" + i + "] "
                            + "(" + hp.point.X + "," + hp.point.Y + ") "
                            + ve.dbug_FullElementDescription();
                        visualroot.dbug_rootHitChainMsg.AddLast(new dbugLayoutMsg(ve, hit_info));
                    }
                    i++;
                }
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



        public void OnMouseMove(UIMouseEventArgs e)
        {

            HitTestCoreWithPrevChainHint(e.X, e.Y, UIEventName.MouseMove);
            var hitElement = this.hitPointChain.CurrentHitElement;
            hoverMonitoringTask.Reset();
            hoverMonitoringTask.SetEnable(true, this.topwin);

            if (hitElement != currentMouseActiveElement)
            {
                DisableGraphicOutputFlush = true;
                {
                    if (currentMouseActiveElement != null &&
                        currentMouseActiveElement.IsTestable)
                    {
                        Point prevElementGlobalLocation = currentMouseActiveElement.GetGlobalLocation();
                        e.TranslateCanvasOrigin(prevElementGlobalLocation);
                        e.Location = hitPointChain.PrevHitPoint;
                        e.SourceHitElement = currentMouseActiveElement;

                        IEventListener ui = currentMouseActiveElement.GetController() as IEventListener;
                        if (ui != null)
                        {
                            ui.ListenMouseEvent(UIMouseEventName.MouseLeave, e);
                        }

                        e.TranslateCanvasOriginBack();
                        currentMouseActiveElement = null;
                    }

                    if (hitElement != null && hitElement.IsTestable)
                    {

                        currentMouseActiveElement = hitElement;

                        e.TranslateCanvasOrigin(hitPointChain.LastestElementGlobalX, hitPointChain.LastestElementGlobalY);
                        e.Location = hitPointChain.CurrentHitPoint;
                        e.SourceHitElement = hitElement;

                        IEventListener ui = hitElement.GetController() as IEventListener;
                        if (ui != null)
                        {
                            ui.ListenMouseEvent(UIMouseEventName.MouseEnter, e);
                        }

                        e.TranslateCanvasOriginBack();

                    }
                }
                DisableGraphicOutputFlush = false;
                FlushAccumGraphicUpdate();
            }
            else if (hitElement != null)
            {
                DisableGraphicOutputFlush = true;
                {
                    e.TranslateCanvasOrigin(hitPointChain.LastestElementGlobalX, hitPointChain.LastestElementGlobalY);
                    e.Location = hitPointChain.CurrentHitPoint;
                    e.SourceHitElement = hitElement;

                    IEventListener ui = hitElement.GetController() as IEventListener;
                    if (ui != null)
                    {
                        ui.ListenMouseEvent(UIMouseEventName.MouseMove, e);
                    }

                    e.TranslateCanvasOriginBack();
                }
                DisableGraphicOutputFlush = false;
                FlushAccumGraphicUpdate();
            }

            hitPointChain.SwapHitChain();
        }
        void OnMouseHover(object sender, EventArgs e)
        {
            return;
            HitTestCoreWithPrevChainHint(hitPointChain.LastestRootX,
                 hitPointChain.LastestRootY,
                 UIEventName.MouseHover);
            RenderElement hitElement = this.hitPointChain.CurrentHitElement;
            if (hitElement != null && hitElement.IsTestable)
            {
                DisableGraphicOutputFlush = true;
                Point hitElementGlobalLocation = hitElement.GetGlobalLocation();

                UIMouseEventArgs e2 = new UIMouseEventArgs();
                e2.WinTop = this.topwin;
                e2.Location = hitPointChain.CurrentHitPoint;
                e2.SourceHitElement = hitElement;
                IEventListener ui = hitElement.GetController() as IEventListener;
                if (ui != null)
                {
                    ui.ListenMouseEvent(UIMouseEventName.MouseHover, e2);
                }

                DisableGraphicOutputFlush = false;
                FlushAccumGraphicUpdate();
            }
            hitPointChain.SwapHitChain();
            hoverMonitoringTask.SetEnable(false, this.topwin);
        }
        public void OnDragStart(UIDragEventArgs e)
        {

#if DEBUG
            if (this.rootGraphic.dbugEnableGraphicInvalidateTrace)
            {
                this.rootGraphic.dbugGraphicInvalidateTracer.WriteInfo("================");
                this.rootGraphic.dbugGraphicInvalidateTracer.WriteInfo("START_DRAG");
                this.rootGraphic.dbugGraphicInvalidateTracer.WriteInfo("================");
            }
#endif


            currentXDistanceFromDragPoint = 0;
            currentYDistanceFromDragPoint = 0;

            HitTestCoreWithPrevChainHint(
              hitPointChain.LastestRootX,
              hitPointChain.LastestRootY,
              UIEventName.DragStart);

            currentDragingElement = this.hitPointChain.CurrentHitElement;

            if (currentDragingElement != null &&
                currentDragingElement != this.topwin)
            {
                DisableGraphicOutputFlush = true;
                Point globalLocation = currentDragingElement.GetGlobalLocation();
                e.TranslateCanvasOrigin(globalLocation);
                e.Location = hitPointChain.CurrentHitPoint;
                e.DragingElement = currentDragingElement;
                e.SourceHitElement = currentDragingElement;



                IEventListener ui = currentDragingElement.GetController() as IEventListener;
                if (ui != null)
                {
                    ui.ListenDragEvent(UIDragEventName.DragStart, e);
                }
                e.TranslateCanvasOriginBack();
                DisableGraphicOutputFlush = false;
                FlushAccumGraphicUpdate();
                hitPointChain.ClearDragHitElements();
            }
            hitPointChain.SwapHitChain();
        }
        public void OnDrag(UIDragEventArgs e)
        {

#if DEBUG
            this.rootGraphic.dbugEventIsDragging = true;
#endif

            if (currentDragingElement == null)
            {

                return;
            }
            else
            {
            }

            //--------------
            currentXDistanceFromDragPoint += e.XDiff;
            currentYDistanceFromDragPoint += e.YDiff;


            DisableGraphicOutputFlush = true;

            Point globalDragingElementLocation = currentDragingElement.GetGlobalLocation();
            e.TranslateCanvasOrigin(globalDragingElementLocation);
            e.SourceHitElement = currentDragingElement;
            Point dragPoint = hitPointChain.PrevHitPoint;
            dragPoint.Offset(currentXDistanceFromDragPoint, currentYDistanceFromDragPoint);
            e.Location = dragPoint;
            e.DragingElement = currentDragingElement;

            IEventListener ui = currentDragingElement.GetController() as IEventListener;
            if (ui != null)
            {
                ui.ListenDragEvent(UIDragEventName.Dragging, e);
            }
            e.TranslateCanvasOriginBack();

            //if (currentDragingElement.HasDragBroadcastable)
            //{
            //    BroadcastDragHitEvents(e);
            //}

            FlushAccumGraphicUpdate();
        }


        void BroadcastDragHitEvents(UIDragEventArgs e)
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
            //UIDragEventArgs d_eventArg = UIDragEventArgs.GetFreeDragEventArgs();

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
            //UIDragEventArgs.ReleaseEventArgs(d_eventArg);
        }
        public void OnDragStop(UIDragEventArgs e)
        {


#if DEBUG
            this.rootGraphic.dbugEventIsDragging = false;
#endif
            if (currentDragingElement == null)
            {
                return;
            }

            DisableGraphicOutputFlush = true;

            Point globalDragingElementLocation = currentDragingElement.GetGlobalLocation();
            e.TranslateCanvasOrigin(globalDragingElementLocation);

            Point dragPoint = hitPointChain.PrevHitPoint;
            dragPoint.Offset(currentXDistanceFromDragPoint, currentYDistanceFromDragPoint);
            e.Location = dragPoint;

            e.SourceHitElement = currentDragingElement;
            var script = currentDragingElement.GetController() as IEventListener;
            if (script != null)
            {
                script.ListenDragEvent(UIDragEventName.DragStop, e);
            }

            e.TranslateCanvasOriginBack();

            UIDragEventArgs d_eventArg = new UIDragEventArgs();
            if (hitPointChain.DragHitElementCount > 0)
            {
                foreach (RenderElement elem in hitPointChain.GetDragHitElementIter())
                {



                    Point globalLocation = elem.GetGlobalLocation();
                    d_eventArg.TranslateCanvasOrigin(globalLocation);
                    d_eventArg.SourceHitElement = elem;
                    d_eventArg.DragingElement = currentDragingElement;

                    var script2 = elem.GetController();
                    if (script2 != null)
                    {
                    }

                    d_eventArg.TranslateCanvasOriginBack();
                }
            }

            hitPointChain.ClearDragHitElements();


            currentDragingElement = null;
            DisableGraphicOutputFlush = false;
            FlushAccumGraphicUpdate();

        }
        public void OnGotFocus(UIFocusEventArgs e)
        {

            if (currentMouseActiveElement != null)
            {

            }

        }
        public void OnLostFocus(UIFocusEventArgs e)
        {

        }
        public void OnMouseUp(UIMouseEventArgs e)
        {

#if DEBUG

            if (this.rootGraphic.dbugEnableGraphicInvalidateTrace)
            {
                this.rootGraphic.dbugGraphicInvalidateTracer.WriteInfo("================");
                this.rootGraphic.dbugGraphicInvalidateTracer.WriteInfo("MOUSEUP");
                this.rootGraphic.dbugGraphicInvalidateTracer.WriteInfo("================");
            }
#endif

            HitTestCoreWithPrevChainHint(e.X, e.Y, UIEventName.MouseUp);
            int hitCount = this.hitPointChain.Count;
            if (hitCount > 0)
            {
                RenderElement hitElement = this.hitPointChain.CurrentHitElement;
                DisableGraphicOutputFlush = true;

                //Point globalLocation = hitElement.GetGlobalLocation();
                //e.TranslateCanvasOrigin(globalLocation);
                e.Location = hitPointChain.CurrentHitPoint;
                e.SourceHitElement = hitElement;
                //---------------------------------------------------------------
                //propagate : bubble up model *** 
                bool isOk = false;

                for (int i = this.hitPointChain.Count - 1; i >= 0 && !isOk; --i)
                {

                    HitPoint hitPoint = hitPointChain.GetHitPoint(i);
                    RenderElement hitElem = hitPoint.elem;
                    if (hitElem is RenderElement)
                    {
                        IEventListener listener = hitElem.GetController() as IEventListener;
                        listener.ListenMouseEvent(UIMouseEventName.MouseDown, e);
                        //may propagate next or not 
                        hitElement = hitElem;
                        currentMouseActiveElement = hitElement;
                        break;
                    }
                    else
                    {
                        var boxChain = (HtmlRenderer.Boxes.CssBoxHitChain)hitPoint.externalObject;
                        //loop
                        for (int n = boxChain.Count - 1; n >= 0; --n)
                        {
                            var hit2 = boxChain.GetHitInfo(n);
                            var box2 = hit2.hitObject as HtmlRenderer.Boxes.CssBox;
                            var controller2 = HtmlRenderer.Boxes.CssBox.UnsafeGetController(box2);
                            if (box2 != null)
                            {
                                var box2EvListener = controller2 as IEventListener;
                                if (box2EvListener != null)
                                {

                                    e.Location = new Point(hit2.localX, hit2.localY);
                                    e.SourceHitElement = box2;
                                    box2EvListener.ListenMouseEvent(UIMouseEventName.MouseDown, e);
                                    //hitElement = box2;                                
                                    if (box2.AcceptKeyboardFocus)
                                    {
                                        this.CurrentKeyboardFocusedElement = box2EvListener;
                                    }
                                    isOk = true;
                                    break; //break loop for
                                }
                            }
                        }
                    }
                }
                //--------------------------------------------------------------- 
                e.TranslateCanvasOriginBack();
                DisableGraphicOutputFlush = false;
                //if (hitElement.Focusable)
                //{
                //    this.CurrentKeyboardFocusedElement = hitElement.HitObject as RenderElement;
                //}
                FlushAccumGraphicUpdate();
            }

            hitPointChain.SwapHitChain();
        }
        public void OnKeyDown(UIKeyEventArgs e)
        {
            var visualroot = this.rootGraphic;
            e.IsShiftKeyDown = e.Shift;
            e.IsAltKeyDown = e.Alt;
            e.IsCtrlKeyDown = e.Control;

            if (currentKeyboardFocusedElement != null)
            {

                e.TranslateCanvasOrigin(kbFocusGlobalX, kbFocusGlobalY);
                e.SourceHitElement = currentKeyboardFocusedElement;


                currentKeyboardFocusedElement.ListenKeyEvent(UIKeyEventName.KeyDown, e);

                e.TranslateCanvasOriginBack();
            }
        }
        public void OnKeyUp(UIKeyEventArgs e)
        {
            var visualroot = this.rootGraphic;
            e.IsShiftKeyDown = e.Shift;
            e.IsAltKeyDown = e.Alt;
            e.IsCtrlKeyDown = e.Control;

            if (currentKeyboardFocusedElement != null)
            {
                e.TranslateCanvasOrigin(kbFocusGlobalX, kbFocusGlobalY);
                e.SourceHitElement = currentKeyboardFocusedElement;


                currentKeyboardFocusedElement.ListenKeyEvent(UIKeyEventName.KeyUp, e);


                e.TranslateCanvasOriginBack();
            }
        }
        public void OnKeyPress(UIKeyPressEventArgs e)
        {

            if (currentKeyboardFocusedElement != null)
            {

                e.TranslateCanvasOrigin(kbFocusGlobalX, kbFocusGlobalY);
                e.SourceHitElement = currentKeyboardFocusedElement;

                currentKeyboardFocusedElement.ListenKeyPressEvent(e);

                e.TranslateCanvasOriginBack();
            }
        }

        public bool OnProcessDialogKey(UIKeyEventArgs e)
        {

            bool result = false;
            if (currentKeyboardFocusedElement != null)
            {
                e.TranslateCanvasOrigin(kbFocusGlobalX, kbFocusGlobalY);
                e.SourceHitElement = currentKeyboardFocusedElement;

                result = currentKeyboardFocusedElement.ListenProcessDialogKey(e);


                if (result && currentKeyboardFocusedElement != null)
                {

                    //currentKeyboardFocusedElement.InvalidateGraphic();

                }
                e.TranslateCanvasOriginBack();
            }

            return result;
        }

    }


}