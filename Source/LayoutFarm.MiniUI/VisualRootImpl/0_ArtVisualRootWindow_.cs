//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;


using LayoutFarm.Presentation;



namespace LayoutFarm.Presentation
{

    partial class ArtVisualWindowImpl
    {


        System.Timers.Timer centralAnimationClock;


        ArtVisualElement currentMouseActiveElement = null;

        ArtVisualElement currentDragingElement = null;



        int globalXOfCurrentUI = 0;
        int globalYOfCurrentUI = 0;



        int currentXDistanceFromDragPoint = 0;
        int currentYDistanceFromDragPoint = 0;


        bool isInRenderPhase = false;


        readonly ArtHitPointChain hitPointChain = new ArtHitPointChain();



        LinkedList<ArtVisualRootTimerTask> rootTimerTasks = new LinkedList<ArtVisualRootTimerTask>();
        System.Timers.Timer rootTasksTimer;

        ArtUIHoverMonitorTask hoverMonitoringTask;


        public event EventHandler<ArtInvalidatedEventArgs> CanvasInvalidatedEvent;
        public event EventHandler<ArtCaretEventArgs> CanvasCaretEvent;
        public event EventHandler<ArtCursorEventArgs> CursorStyleEventHandler;
        public event EventHandler CanvasForcePaintMe;
        public event EventHandler CurrentFocusElementChanged;




        int msgChainVersion;



        public void ChangeVisualRootSize(int width, int height)
        {
            VisualElementArgs vinv = this.GetVInv();
            this.ChangeRootElementSize(width, height, vinv);
            this.FreeVInv(vinv);
        }
        public void Dispose()
        {

        }

        LinkedList<LinkedListNode<ArtVisualRootTimerTask>> tobeRemoveTasks = new LinkedList<LinkedListNode<ArtVisualRootTimerTask>>();












        void SetCaretVisible(bool visible)
        {
            if (CanvasCaretEvent != null)
            {

                var e = eventStock.GetFreeCaretEventArgs();
                e.Visible = visible;
                CanvasCaretEvent.Invoke(this, e);
                eventStock.ReleaseEventArgs(e);
            }
        }

        public override ArtVisualElement CurrentKeyboardFocusedElement
        {
            get
            {

                if (currentKeyboardFocusedElement != this)
                {
                    return currentKeyboardFocusedElement;
                }
                else
                {
                    return null;
                }
            }
            set
            {

                if (value != null && !(value.Focusable))
                {
                    return;
                }
                if (currentKeyboardFocusedElement != null)
                {
                    if (currentKeyboardFocusedElement == value)
                    {
                        return;
                    }


                    ArtFocusEventArgs focusEventArg = eventStock.GetFreeFocusEventArgs(value, currentKeyboardFocusedElement);
                    focusEventArg.SetWinRoot(this);
                    var script = currentKeyboardFocusedElement.GetController();
                    if (script != null)
                    {
                    }


                    if (currentKeyboardFocusedElement.IsTextEditContainer)
                    {
                        SetCaretVisible(false);
                        VisualElementArgs vinv = this.GetVInv();
                        currentKeyboardFocusedElement.InvalidateGraphic(vinv);
                        this.FreeVInv(vinv);
                    }
                    eventStock.ReleaseEventArgs(focusEventArg);
                }
                currentKeyboardFocusedElement = value;
                if (currentKeyboardFocusedElement != null)
                {
                    ArtFocusEventArgs focusEventArg = eventStock.GetFreeFocusEventArgs(value, currentKeyboardFocusedElement);
                    focusEventArg.SetWinRoot(this);
                    Point globalLocation = value.GetGlobalLocation();
                    globalXOfCurrentUI = globalLocation.X;
                    globalYOfCurrentUI = globalLocation.Y;
                    focusEventArg.SetWinRoot(this);

                    IEventDispatcher ui = value.GetController() as IEventDispatcher;
                    if (ui != null)
                    {

                    }
                    eventStock.ReleaseEventArgs(focusEventArg);
                    if (currentKeyboardFocusedElement.IsTextEditContainer)
                    {

                        SetCaretVisible(true);
                    }
                    else
                    {
                        SetCaretVisible(false);
                    }
                    if (CurrentFocusElementChanged != null)
                    {
                        CurrentFocusElementChanged.Invoke(this, EventArgs.Empty);
                    }
                }
                else
                {
                    globalXOfCurrentUI = 0;
                    globalYOfCurrentUI = 0;
                }
            }
        }


        public override ArtVisualElement CurrentDraggingElement
        {
            get
            {
                return currentDragingElement;
            }
            set
            {
                if (currentDragingElement != null
    && currentDragingElement != value)
                {
                    if (value != null)
                    {
                        currentDragingElement = value;
                    }
                    else
                    {

                    }
                }
                else if (currentDragingElement == null)
                {
                    if (value != null)
                    {
                        currentDragingElement = value;
                    }
                }
            }
        }

        internal ArtVisualElement CurrentMouseFocusedElement
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


        public void ClearAllResources()
        {
            if (centralAnimationClock != null)
            {
                centralAnimationClock.Stop();
            }
            CurrentKeyboardFocusedElement = null;
            ClearAllChildren();
            hitPointChain.ClearAll();



        }

        public new void OnDoubleClick(ArtMouseEventArgs e)
        {

            ArtVisualElement hitElement = HitTestCoreWithPrevChainHint(e.X, e.Y, HitEventName.DblClick);
            if (currentMouseActiveElement != null)
            {
                e.TranslateCanvasOrigin(globalXOfCurrentUI, globalYOfCurrentUI);
                e.Location = hitPointChain.CurrentHitPoint;
                e.SourceVisualElement = currentMouseActiveElement;

                IEventDispatcher ui = currentMouseActiveElement.GetController() as IEventDispatcher;
                if (ui != null)
                {
                }
                e.TranslateCanvasOriginBack();


            }
            hitPointChain.SwapHitChain();
        }



        public new void OnMouseWheel(ArtMouseEventArgs e)
        {

            if (currentMouseActiveElement != null)
            {
                IEventDispatcher ui = currentMouseActiveElement.GetController() as IEventDispatcher;
                if (ui != null)
                {
                    ui.DispatchMouseEvent(UIMouseEventName.MouseWheel, e);
                }
            }
        }
        public void OnMouseDown(ArtMouseEventArgs e)
        {

#if DEBUG

            if (this.visualroot.dbugEnableGraphicInvalidateTrace)
            {
                this.visualroot.dbugGraphicInvalidateTracer.WriteInfo("================");
                this.visualroot.dbugGraphicInvalidateTracer.WriteInfo("MOUSEDOWN");
                this.visualroot.dbugGraphicInvalidateTracer.WriteInfo("================");
            }

#endif


            msgChainVersion = 1; int local_msgVersion = 1;
            ArtVisualElement hitElement = HitTestCoreWithPrevChainHint(e.X, e.Y, HitEventName.MouseDown);
            if (hitElement == this || hitElement == null)
            {
                hitPointChain.SwapHitChain(); return;
            }
            disableGraphicOutputFlush = true;

            e.TranslateCanvasOrigin(globalXOfCurrentUI, globalYOfCurrentUI);
            e.Location = hitPointChain.CurrentHitPoint;
            e.SourceVisualElement = hitElement;


            currentMouseActiveElement = hitElement;


            IEventDispatcher ui = hitElement.GetController() as IEventDispatcher;
            if (ui != null)
            {
                ui.DispatchMouseEvent(UIMouseEventName.MouseDown, e);
            }
            e.TranslateCanvasOriginBack();
#if DEBUG
            VisualRoot visualroot = this.dbugVRoot;
            if (visualroot.dbug_RecordHitChain)
            {
                visualroot.dbug_rootHitChainMsg.Clear();
                int i = 0;
                foreach (ArtHitPointChain.HitPair hp in hitPointChain.HitPairIter)
                {

                    ArtVisualElement ve = hp.elem;
                    ve.dbug_WriteOwnerLayerInfo(visualroot, i);
                    ve.dbug_WriteOwnerLineInfo(visualroot, i);

                    string hit_info = new string('.', i) + " [" + i + "] "
                        + "(" + hp.point.X + "," + hp.point.Y + ") "
                        + ve.dbug_FullElementDescription();
                    visualroot.dbug_rootHitChainMsg.AddLast(new dbugLayoutMsg(ve, hit_info));
                    i++;
                }
            }
#endif
            hitPointChain.SwapHitChain();
            if (hitElement.ParentVisualElement == null)
            {
                currentMouseActiveElement = null;
                return;
            }

            if (local_msgVersion != msgChainVersion)
            {
                return;
            }


            if (hitElement.Focusable)
            {
                VisualElementArgs vinv = e.GetVisualInvalidateCanvasArgs();
                hitElement.Focus(vinv);
                e.FreeVisualInvalidateCanvasArgs(vinv);
            }
            disableGraphicOutputFlush = false;
            FlushGraphicUpdate();

#if DEBUG
            visualroot.dbugHitTracker.Write("stop-mousedown");
            visualroot.dbugHitTracker.Play = false;
#endif

        }
        ArtVisualElement HitTestCoreWithPrevChainHint(int x, int y, HitEventName hitEvent)
        {
            hitPointChain.SetVisualRootStartTestPoint(x, y);
            ArtVisualElement commonElement = hitPointChain.HitTestOnPrevChain(); if (commonElement == null)
            {
                commonElement = this;
            }
            commonElement.HitTestCore(hitPointChain); return hitPointChain.CurrentHitElement;
        }

        public void OnMouseMove(ArtMouseEventArgs e)
        {
#if DEBUG

#endif
            ArtVisualElement hitElement = HitTestCoreWithPrevChainHint(e.X, e.Y, HitEventName.MouseMove);

            hoverMonitoringTask.Reset(); hoverMonitoringTask.SetEnable(true, this);

            if (hitElement != currentMouseActiveElement)
            {
                disableGraphicOutputFlush = true;
                {
                    if (ArtVisualElement.IsTestableElement(currentMouseActiveElement))
                    {
                        Point prevElementGlobalLocation = currentMouseActiveElement.GetGlobalLocation();
                        e.TranslateCanvasOrigin(prevElementGlobalLocation); e.Location = hitPointChain.PrevHitPoint; e.SourceVisualElement = currentMouseActiveElement;
                        IEventDispatcher ui = currentMouseActiveElement.GetController() as IEventDispatcher;
                        if (ui != null)
                        {
                            ui.DispatchMouseEvent(UIMouseEventName.MouseLeave, e);
                        }

                        e.TranslateCanvasOriginBack(); currentMouseActiveElement = null;
                    }


                    if (ArtVisualElement.IsTestableElement(hitElement))
                    {

                        currentMouseActiveElement = hitElement;


                        e.TranslateCanvasOrigin(hitPointChain.LastestElementGlobalX, hitPointChain.LastestElementGlobalY);
                        e.Location = hitPointChain.CurrentHitPoint; e.SourceVisualElement = hitElement;

                        IEventDispatcher ui = hitElement.GetController() as IEventDispatcher;
                        if (ui != null)
                        {
                            ui.DispatchMouseEvent(UIMouseEventName.MouseEnter, e);
                        }

                        e.TranslateCanvasOriginBack();

                    }
                }
                disableGraphicOutputFlush = false;
                FlushGraphicUpdate();
            }
            else if (hitElement != null)
            {
                disableGraphicOutputFlush = true;
                {
                    e.TranslateCanvasOrigin(hitPointChain.LastestElementGlobalX, hitPointChain.LastestElementGlobalY);
                    e.Location = hitPointChain.CurrentHitPoint; e.SourceVisualElement = hitElement;

                    IEventDispatcher ui = hitElement.GetController() as IEventDispatcher;
                    if (ui != null)
                    {
                        ui.DispatchMouseEvent(UIMouseEventName.MouseMove, e);
                    }

                    e.TranslateCanvasOriginBack();
                }
                disableGraphicOutputFlush = false;
                FlushGraphicUpdate();
            }

            hitPointChain.SwapHitChain();
        }
        void OnMouseHover(object sender, EventArgs e)
        {
            ArtVisualElement hitElement = HitTestCoreWithPrevChainHint(hitPointChain.LastestRootX, hitPointChain.LastestRootY, HitEventName.MouseHover);
            if (hitElement != null && ArtVisualElement.IsTestableElement(hitElement))
            {
                disableGraphicOutputFlush = true;
                Point hitElementGlobalLocation = hitElement.GetGlobalLocation();

                ArtMouseEventArgs e2 = new ArtMouseEventArgs();

                e2.Location = hitPointChain.CurrentHitPoint; e2.SourceVisualElement = hitElement;
                IEventDispatcher ui = hitElement.GetController() as IEventDispatcher;
                if (ui != null)
                {
                    ui.DispatchMouseEvent(UIMouseEventName.MouseHover, e2);
                }

                disableGraphicOutputFlush = false;
                FlushGraphicUpdate();
            }
            hitPointChain.SwapHitChain();

            hoverMonitoringTask.SetEnable(false, this);
        }

        public void OnDragStart(ArtDragEventArgs e)
        {

#if DEBUG
            if (this.visualroot.dbugEnableGraphicInvalidateTrace)
            {
                this.visualroot.dbugGraphicInvalidateTracer.WriteInfo("================");
                this.visualroot.dbugGraphicInvalidateTracer.WriteInfo("START_DRAG");
                this.visualroot.dbugGraphicInvalidateTracer.WriteInfo("================");
            }
#endif


            currentXDistanceFromDragPoint = 0;
            currentYDistanceFromDragPoint = 0;
            currentDragingElement = HitTestCoreWithPrevChainHint(
hitPointChain.LastestRootX,
hitPointChain.LastestRootY,
HitEventName.DragStart);

            if (currentDragingElement != null && currentDragingElement != this)
            {
                disableGraphicOutputFlush = true;
                Point globalLocation = currentDragingElement.GetGlobalLocation();
                e.TranslateCanvasOrigin(globalLocation);
                e.Location = hitPointChain.CurrentHitPoint;
                e.DragingElement = currentDragingElement;
                e.SourceVisualElement = currentDragingElement;
                IEventDispatcher ui = currentDragingElement.GetController() as IEventDispatcher;
                if (ui != null)
                {
                    ui.DispatchDragEvent(UIDragEventName.DragStart, e);
                }
                e.TranslateCanvasOriginBack();
                disableGraphicOutputFlush = false;
                FlushGraphicUpdate();
                hitPointChain.ClearDragHitElements();
            }
            hitPointChain.SwapHitChain();


        }

        public void OnDrag(ArtDragEventArgs e)
        {

#if DEBUG
            this.dbugVRoot.dbugEventIsDragging = true;
#endif

            if (currentDragingElement == null)
            {

                return;
            }
            else
            {
            }




            currentXDistanceFromDragPoint += e.XDiff; currentYDistanceFromDragPoint += e.YDiff;

            if (currentDragingElement.IsTextEditContainer)
            {

                disableGraphicOutputFlush = true;
                Point globalLoca = currentDragingElement.GetGlobalLocation();
                e.TranslateCanvasOrigin(globalLoca);
                Point dragPoint = hitPointChain.PrevHitPoint;
                dragPoint.Offset(currentXDistanceFromDragPoint, currentYDistanceFromDragPoint);
                e.Location = dragPoint;
                e.SourceVisualElement = currentDragingElement;
                IEventDispatcher ui = currentDragingElement.GetController() as IEventDispatcher;
                if (ui != null)
                {

                    ui.DispatchDragEvent(UIDragEventName.Dragging, e);
                }

                e.TranslateCanvasOriginBack();
                disableGraphicOutputFlush = false;

            }
            else
            {

                disableGraphicOutputFlush = true;

                Point globalDragingElementLocation = currentDragingElement.GetGlobalLocation();
                e.TranslateCanvasOrigin(globalDragingElementLocation);
                e.SourceVisualElement = currentDragingElement;
                Point dragPoint = hitPointChain.PrevHitPoint;
                dragPoint.Offset(currentXDistanceFromDragPoint, currentYDistanceFromDragPoint);
                e.Location = dragPoint;
                e.DragingElement = currentDragingElement;



                IEventDispatcher ui = currentDragingElement.GetController() as IEventDispatcher;
                if (ui != null)
                {

                    ui.DispatchDragEvent(UIDragEventName.Dragging, e);
                }
                e.TranslateCanvasOriginBack();

                if (currentDragingElement.HasDragBroadcastable)
                {
                    BroadcastDragHitEvents(e);
                }
            }
            FlushGraphicUpdate();
        }


        void BroadcastDragHitEvents(ArtDragEventArgs e)
        {


            Point globalDragingElementLocation = currentDragingElement.GetGlobalLocation();
            Rectangle dragRect = currentDragingElement.GetGlobalRect();
            VisualDrawingChain drawingChain = this.WinRootPrepareRenderingChain(dragRect);
            List<ArtVisualElement> selVisualElements = drawingChain.selectedVisualElements;
            int j = selVisualElements.Count;
            LinkedList<ArtVisualElement> underlyingElements = ArtUILinkListPool.GetFreeLinkedList();
            for (int i = j - 1; i > -1; --i)
            {

                if (selVisualElements[i].ListeningDragEvent)
                {
                    underlyingElements.AddLast(selVisualElements[i]);
                }
            }

            if (underlyingElements.Count > 0)
            {
                foreach (ArtVisualElement underlyingUI in underlyingElements)
                {

                    if (underlyingUI.IsDragedOver)
                    {

                        hitPointChain.RemoveDragHitElement(underlyingUI);
                        underlyingUI.IsDragedOver = false;
                    }
                }
            }
            ArtDragEventArgs d_eventArg = ArtDragEventArgs.GetFreeDragEventArgs();

            if (hitPointChain.DragHitElementCount > 0)
            {
                foreach (ArtVisualElement elem in hitPointChain.GetDragHitElementIter())
                {
                    Point globalLocation = elem.GetGlobalLocation();
                    d_eventArg.TranslateCanvasOrigin(globalLocation);
                    d_eventArg.SourceVisualElement = elem;
                    var script = elem.GetController();
                    if (script != null)
                    {
                    }
                    d_eventArg.TranslateCanvasOriginBack();
                }
            }
            hitPointChain.ClearDragHitElements();

            foreach (ArtVisualElement underlyingUI in underlyingElements)
            {

                hitPointChain.AddDragHitElement(underlyingUI);
                if (underlyingUI.IsDragedOver)
                {
                    Point globalLocation = underlyingUI.GetGlobalLocation();
                    d_eventArg.TranslateCanvasOrigin(globalLocation);
                    d_eventArg.SourceVisualElement = underlyingUI;

                    var script = underlyingUI.GetController();
                    if (script != null)
                    {
                    }

                    d_eventArg.TranslateCanvasOriginBack();
                }
                else
                {
                    underlyingUI.IsDragedOver = true;
                    Point globalLocation = underlyingUI.GetGlobalLocation();
                    d_eventArg.TranslateCanvasOrigin(globalLocation);
                    d_eventArg.SourceVisualElement = underlyingUI;

                    var script = underlyingUI.GetController();
                    if (script != null)
                    {
                    }

                    d_eventArg.TranslateCanvasOriginBack();
                }
            }
            ArtDragEventArgs.ReleaseEventArgs(d_eventArg);

            ArtUILinkListPool.Release(underlyingElements);
        }
        public new void OnDragStop(ArtDragEventArgs e)
        {


#if DEBUG
            this.dbugVRoot.dbugEventIsDragging = false;
#endif
            if (currentDragingElement == null)
            {
                return;
            }

            disableGraphicOutputFlush = true;
            Point globalDragingElementLocation = currentDragingElement.GetGlobalLocation();
            e.TranslateCanvasOrigin(globalDragingElementLocation);

            Point dragPoint = hitPointChain.PrevHitPoint;
            dragPoint.Offset(currentXDistanceFromDragPoint, currentYDistanceFromDragPoint);
            e.Location = dragPoint;

            e.SourceVisualElement = currentDragingElement;
            var script = currentDragingElement.GetController() as IEventDispatcher;
            if (script != null)
            {
                script.DispatchDragEvent(UIDragEventName.DragStop, e);
            }

            e.TranslateCanvasOriginBack();

            if (currentMouseActiveElement != null)
            {
                if (currentMouseActiveElement.IsTextEditContainer)
                {
                    SetCaretVisible(true);
                }
                else
                {
                    SetCaretVisible(false);
                }
            }

            ArtDragEventArgs d_eventArg = ArtDragEventArgs.GetFreeDragEventArgs();
            if (hitPointChain.DragHitElementCount > 0)
            {
                foreach (ArtVisualElement elem in hitPointChain.GetDragHitElementIter())
                {
                    Point globalLocation = elem.GetGlobalLocation();
                    d_eventArg.TranslateCanvasOrigin(globalLocation);
                    d_eventArg.SourceVisualElement = elem;
                    d_eventArg.DragingElement = currentDragingElement;

                    var script2 = elem.GetController();
                    if (script2 != null)
                    {
                    }

                    d_eventArg.TranslateCanvasOriginBack();
                }
            }

            hitPointChain.ClearDragHitElements();
            ArtDragEventArgs.ReleaseEventArgs(d_eventArg);


            currentDragingElement = null;
            disableGraphicOutputFlush = false;
            FlushGraphicUpdate();

        }
        public void OnGotFocus(ArtFocusEventArgs e)
        {

            if (currentMouseActiveElement != null)
            {
                if (currentMouseActiveElement.IsTextEditContainer)
                {
                    SetCaretVisible(true);
                }
            }

        }
        public void OnLostFocus(ArtFocusEventArgs e)
        {

        }
        public void OnMouseUp(ArtMouseEventArgs e)
        {

#if DEBUG

            if (this.visualroot.dbugEnableGraphicInvalidateTrace)
            {
                this.visualroot.dbugGraphicInvalidateTracer.WriteInfo("================");
                this.visualroot.dbugGraphicInvalidateTracer.WriteInfo("MOUSEUP");
                this.visualroot.dbugGraphicInvalidateTracer.WriteInfo("================");
            }

#endif

            ArtVisualElement hitElement = HitTestCoreWithPrevChainHint(e.X, e.Y, HitEventName.MouseUp);
            if (hitElement != null)
            {
                disableGraphicOutputFlush = true;

                Point globalLocation = hitElement.GetGlobalLocation();
                e.TranslateCanvasOrigin(globalLocation);
                e.Location = hitPointChain.CurrentHitPoint;

                e.SourceVisualElement = hitElement;
                IEventDispatcher ui = hitElement.GetController() as IEventDispatcher;
                if (ui != null)
                {
                    ui.DispatchMouseEvent(UIMouseEventName.MouseUp, e);
                }
                e.TranslateCanvasOriginBack();

                disableGraphicOutputFlush = false;

                if (hitElement.Focusable)
                {
                    VisualElementArgs vinv = e.GetVisualInvalidateCanvasArgs();
                    hitElement.Focus(vinv);
                    e.FreeVisualInvalidateCanvasArgs(vinv);
                }


                FlushGraphicUpdate();
            }

            hitPointChain.SwapHitChain();
        }
        public new void OnKeyDown(ArtKeyEventArgs e)
        {
            var visualroot = this.MyVisualRoot;
            e.IsShiftKeyDown = e.Shift;
            e.IsAltKeyDown = e.Alt;
            e.IsCtrlKeyDown = e.Control;

            if (currentKeyboardFocusedElement != null)
            {

                e.TranslateCanvasOrigin(globalXOfCurrentUI, globalYOfCurrentUI);
                e.SourceVisualElement = currentKeyboardFocusedElement;
                IEventDispatcher ui = currentKeyboardFocusedElement.GetController() as IEventDispatcher;
                if (ui != null)
                {
                    ui.DispatchKeyEvent(UIKeyEventName.KeyDown, e);
                }
                e.TranslateCanvasOriginBack();
            }
        }
        public new void OnKeyUp(ArtKeyEventArgs e)
        {



            var visualroot = this.MyVisualRoot;
            e.IsShiftKeyDown = e.Shift;
            e.IsAltKeyDown = e.Alt;
            e.IsCtrlKeyDown = e.Control;

            if (currentKeyboardFocusedElement != null)
            {
                e.TranslateCanvasOrigin(globalXOfCurrentUI, globalYOfCurrentUI);
                e.SourceVisualElement = currentKeyboardFocusedElement;

                IEventDispatcher ui = currentKeyboardFocusedElement.GetController() as IEventDispatcher;
                if (ui != null)
                {
                    ui.DispatchKeyEvent(UIKeyEventName.KeyUp, e);
                }

                e.TranslateCanvasOriginBack();
            }


        }
        public void OnKeyPress(ArtKeyPressEventArgs e)
        {

            if (currentKeyboardFocusedElement != null)
            {

                e.TranslateCanvasOrigin(globalXOfCurrentUI, globalYOfCurrentUI);
                e.SourceVisualElement = currentKeyboardFocusedElement;
                IEventDispatcher ui = currentKeyboardFocusedElement.GetController() as IEventDispatcher;
                if (ui != null)
                {
                    ui.DispatchKeyPressEvent(e);
                }
                e.TranslateCanvasOriginBack();
            }
        }

        public bool OnProcessDialogKey(ArtKeyEventArgs e)
        {

            bool result = false;
            if (currentKeyboardFocusedElement != null)
            {
                e.TranslateCanvasOrigin(globalXOfCurrentUI, globalYOfCurrentUI);

                e.SourceVisualElement = currentKeyboardFocusedElement;


                IEventDispatcher ui = currentKeyboardFocusedElement.GetController() as IEventDispatcher;
                if (ui != null)
                {
                    result = ui.DispatchProcessDialogKey(e);
                }


                if (result && currentKeyboardFocusedElement != null)
                {
                    VisualElementArgs vinv = this.GetVInv();
                    currentKeyboardFocusedElement.InvalidateGraphic(vinv);
                    this.FreeVInv(vinv);
                }
                e.TranslateCanvasOriginBack();
            }

            return result;
        }





        public Point CaretPosition
        {
            get
            {
                ArtVisualElement currentElem = this.currentKeyboardFocusedElement;
                if (currentElem != null && currentElem.IsTextEditContainer)
                {

                    Point elementCaretPosition = ((ArtVisualContainerBase)currentElem).CaretPosition;

                    bool caretOutOfScope = false;
                    if (elementCaretPosition.X >= currentElem.Right)
                    {
                        caretOutOfScope = true;
                    }
                    if (elementCaretPosition.Y >= currentElem.Bottom)
                    {
                        caretOutOfScope = true;
                    }


                    if (!caretOutOfScope)
                    {
                        elementCaretPosition.Offset(currentElem.GetGlobalLocation());
                        return elementCaretPosition;
                    }
                    else
                    {

                        return new Point(-10, -10);
                    }

                }
                else
                {
                    return new Point(-10, -10);
                }
            }
        }

    }
}
