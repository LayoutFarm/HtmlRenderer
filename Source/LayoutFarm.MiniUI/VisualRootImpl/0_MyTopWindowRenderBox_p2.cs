//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;


using LayoutFarm;



namespace LayoutFarm
{

    partial class MyTopWindowRenderBox
    {


        System.Timers.Timer centralAnimationClock;


        RenderElement currentMouseActiveElement = null;

        RenderElement currentDragingElement = null;



        int globalXOfCurrentUI = 0;
        int globalYOfCurrentUI = 0;



        int currentXDistanceFromDragPoint = 0;
        int currentYDistanceFromDragPoint = 0;


        bool isInRenderPhase = false;


        readonly HitPointChain hitPointChain = new HitPointChain();



        LinkedList<VisualRootTimerTask> rootTimerTasks = new LinkedList<VisualRootTimerTask>();
        System.Timers.Timer rootTasksTimer;

        UIHoverMonitorTask hoverMonitoringTask;


        public event EventHandler<UIInvalidateEventArgs> CanvasInvalidatedEvent;
        public event EventHandler<UICaretEventArgs> CanvasCaretEvent;
        public event EventHandler<UICursorEventArgs> CursorStyleEventHandler;
        public event EventHandler CanvasForcePaintMe;
        public event EventHandler CurrentFocusElementChanged;
        int msgChainVersion;


        public void ChangeVisualRootSize(int width, int height)
        {
            
            this.ChangeRootElementSize(width, height);
           
        }
        public void Dispose()
        {

        }

        LinkedList<LinkedListNode<VisualRootTimerTask>> tobeRemoveTasks = new LinkedList<LinkedListNode<VisualRootTimerTask>>();



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

        public override RenderElement CurrentKeyboardFocusedElement
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


                    UIFocusEventArgs focusEventArg = eventStock.GetFreeFocusEventArgs(value, currentKeyboardFocusedElement);
                    focusEventArg.SetWinRoot(this);
                    var script = currentKeyboardFocusedElement.GetController();
                    if (script != null)
                    {
                    }


                    if (currentKeyboardFocusedElement.NeedSystemCaret)
                    {
                        SetCaretVisible(false);
                       
                        currentKeyboardFocusedElement.InvalidateGraphic();
                         
                    }
                    eventStock.ReleaseEventArgs(focusEventArg);
                }
                currentKeyboardFocusedElement = value;
                if (currentKeyboardFocusedElement != null)
                {
                    UIFocusEventArgs focusEventArg = eventStock.GetFreeFocusEventArgs(value, currentKeyboardFocusedElement);
                    focusEventArg.SetWinRoot(this);
                    Point globalLocation = value.GetGlobalLocation();
                    globalXOfCurrentUI = globalLocation.X;
                    globalYOfCurrentUI = globalLocation.Y;
                    focusEventArg.SetWinRoot(this);

                    IEventListener ui = value.GetController() as IEventListener;
                    if (ui != null)
                    {

                    }
                    eventStock.ReleaseEventArgs(focusEventArg);
                    if (currentKeyboardFocusedElement.NeedSystemCaret)
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


        public override RenderElement CurrentDraggingElement
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

        public new void OnDoubleClick(UIMouseEventArgs e)
        {

            RenderElement hitElement = HitTestCoreWithPrevChainHint(e.X, e.Y, HitEventName.DblClick);
            if (currentMouseActiveElement != null)
            {
                e.TranslateCanvasOrigin(globalXOfCurrentUI, globalYOfCurrentUI);
                e.Location = hitPointChain.CurrentHitPoint;
                e.SourceVisualElement = currentMouseActiveElement;

                IEventListener ui = currentMouseActiveElement.GetController() as IEventListener;
                if (ui != null)
                {
                }
                e.TranslateCanvasOriginBack();


            }
            hitPointChain.SwapHitChain();
        }



        public new void OnMouseWheel(UIMouseEventArgs e)
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

            if (this.visualroot.dbugEnableGraphicInvalidateTrace)
            {
                this.visualroot.dbugGraphicInvalidateTracer.WriteInfo("================");
                this.visualroot.dbugGraphicInvalidateTracer.WriteInfo("MOUSEDOWN");
                this.visualroot.dbugGraphicInvalidateTracer.WriteInfo("================");
            }

#endif


            msgChainVersion = 1; int local_msgVersion = 1;
            RenderElement hitElement = HitTestCoreWithPrevChainHint(e.X, e.Y, HitEventName.MouseDown);
            if (hitElement == this || hitElement == null)
            {
                hitPointChain.SwapHitChain(); return;
            }
            disableGraphicOutputFlush = true;

            e.TranslateCanvasOrigin(globalXOfCurrentUI, globalYOfCurrentUI);
            e.Location = hitPointChain.CurrentHitPoint;
            e.SourceVisualElement = hitElement;


            currentMouseActiveElement = hitElement;


            IEventListener ui = hitElement.GetController() as IEventListener;
            if (ui != null)
            {
                ui.ListenMouseEvent(UIMouseEventName.MouseDown, e);
            }
            e.TranslateCanvasOriginBack();
#if DEBUG
            RootGraphic visualroot = this.dbugVRoot;
            if (visualroot.dbug_RecordHitChain)
            {
                visualroot.dbug_rootHitChainMsg.Clear();
                int i = 0;
                foreach (HitPointChain.HitPair hp in hitPointChain.HitPairIter)
                {

                    RenderElement ve = hp.elem;
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
                 
                //hitElement.Focus();
                hitElement.WinRoot.CurrentKeyboardFocusedElement = hitElement;
                
            }
            disableGraphicOutputFlush = false;
            FlushGraphicUpdate();

#if DEBUG
            visualroot.dbugHitTracker.Write("stop-mousedown");
            visualroot.dbugHitTracker.Play = false;
#endif

        }
        RenderElement HitTestCoreWithPrevChainHint(int x, int y, HitEventName hitEvent)
        {
            hitPointChain.SetVisualRootStartTestPoint(x, y);
            RenderElement commonElement = hitPointChain.HitTestOnPrevChain(); if (commonElement == null)
            {
                commonElement = this;
            }
            commonElement.HitTestCore(hitPointChain); return hitPointChain.CurrentHitElement;
        }

        public void OnMouseMove(UIMouseEventArgs e)
        {
#if DEBUG

#endif
            RenderElement hitElement = HitTestCoreWithPrevChainHint(e.X, e.Y, HitEventName.MouseMove);

            hoverMonitoringTask.Reset(); hoverMonitoringTask.SetEnable(true, this);

            if (hitElement != currentMouseActiveElement)
            {
                disableGraphicOutputFlush = true;
                {
                    if (RenderElement.IsTestableElement(currentMouseActiveElement))
                    {
                        Point prevElementGlobalLocation = currentMouseActiveElement.GetGlobalLocation();
                        e.TranslateCanvasOrigin(prevElementGlobalLocation); e.Location = hitPointChain.PrevHitPoint; e.SourceVisualElement = currentMouseActiveElement;
                        IEventListener ui = currentMouseActiveElement.GetController() as IEventListener;
                        if (ui != null)
                        {
                            ui.ListenMouseEvent(UIMouseEventName.MouseLeave, e);
                        }

                        e.TranslateCanvasOriginBack(); currentMouseActiveElement = null;
                    }


                    if (RenderElement.IsTestableElement(hitElement))
                    {

                        currentMouseActiveElement = hitElement;


                        e.TranslateCanvasOrigin(hitPointChain.LastestElementGlobalX, hitPointChain.LastestElementGlobalY);
                        e.Location = hitPointChain.CurrentHitPoint; e.SourceVisualElement = hitElement;

                        IEventListener ui = hitElement.GetController() as IEventListener;
                        if (ui != null)
                        {
                            ui.ListenMouseEvent(UIMouseEventName.MouseEnter, e);
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

                    IEventListener ui = hitElement.GetController() as IEventListener;
                    if (ui != null)
                    {
                        ui.ListenMouseEvent(UIMouseEventName.MouseMove, e);
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
            RenderElement hitElement = HitTestCoreWithPrevChainHint(hitPointChain.LastestRootX, hitPointChain.LastestRootY, HitEventName.MouseHover);
            if (hitElement != null && RenderElement.IsTestableElement(hitElement))
            {
                disableGraphicOutputFlush = true;
                Point hitElementGlobalLocation = hitElement.GetGlobalLocation();

                UIMouseEventArgs e2 = new UIMouseEventArgs();

                e2.Location = hitPointChain.CurrentHitPoint; e2.SourceVisualElement = hitElement;
                IEventListener ui = hitElement.GetController() as IEventListener;
                if (ui != null)
                {
                    ui.ListenMouseEvent(UIMouseEventName.MouseHover, e2);
                }

                disableGraphicOutputFlush = false;
                FlushGraphicUpdate();
            }
            hitPointChain.SwapHitChain();

            hoverMonitoringTask.SetEnable(false, this);
        }

        public void OnDragStart(UIDragEventArgs e)
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
                IEventListener ui = currentDragingElement.GetController() as IEventListener;
                if (ui != null)
                {
                    ui.ListenDragEvent(UIDragEventName.DragStart, e);
                }
                e.TranslateCanvasOriginBack();
                disableGraphicOutputFlush = false;
                FlushGraphicUpdate();
                hitPointChain.ClearDragHitElements();
            }
            hitPointChain.SwapHitChain();


        }

        public void OnDrag(UIDragEventArgs e)
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
            
            //--------------
            currentXDistanceFromDragPoint += e.XDiff; currentYDistanceFromDragPoint += e.YDiff;

            if (currentDragingElement.NeedSystemCaret)
            {

                disableGraphicOutputFlush = true;
                Point globalLoca = currentDragingElement.GetGlobalLocation();
                e.TranslateCanvasOrigin(globalLoca);
                Point dragPoint = hitPointChain.PrevHitPoint;
                dragPoint.Offset(currentXDistanceFromDragPoint, currentYDistanceFromDragPoint);
                e.Location = dragPoint;
                e.SourceVisualElement = currentDragingElement;
                IEventListener ui = currentDragingElement.GetController() as IEventListener;
                if (ui != null)
                {

                    ui.ListenDragEvent(UIDragEventName.Dragging, e);
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



                IEventListener ui = currentDragingElement.GetController() as IEventListener;
                if (ui != null)
                {

                    ui.ListenDragEvent(UIDragEventName.Dragging, e);
                }
                e.TranslateCanvasOriginBack();

                if (currentDragingElement.HasDragBroadcastable)
                {
                    BroadcastDragHitEvents(e);
                }
            }
            FlushGraphicUpdate();
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
            var script = currentDragingElement.GetController() as IEventListener;
            if (script != null)
            {
                script.ListenDragEvent(UIDragEventName.DragStop, e);
            }

            e.TranslateCanvasOriginBack();

            if (currentMouseActiveElement != null)
            {
                SetCaretVisible(currentMouseActiveElement.NeedSystemCaret);
                 
            }

            UIDragEventArgs d_eventArg = UIDragEventArgs.GetFreeDragEventArgs();
            if (hitPointChain.DragHitElementCount > 0)
            {
                foreach (RenderElement elem in hitPointChain.GetDragHitElementIter())
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
            UIDragEventArgs.ReleaseEventArgs(d_eventArg);


            currentDragingElement = null;
            disableGraphicOutputFlush = false;
            FlushGraphicUpdate();

        }
        public void OnGotFocus(UIFocusEventArgs e)
        {

            if (currentMouseActiveElement != null)
            {
                if (currentMouseActiveElement.NeedSystemCaret)
                {
                    SetCaretVisible(true);
                }
            }

        }
        public void OnLostFocus(UIFocusEventArgs e)
        {

        }
        public void OnMouseUp(UIMouseEventArgs e)
        {

#if DEBUG

            if (this.visualroot.dbugEnableGraphicInvalidateTrace)
            {
                this.visualroot.dbugGraphicInvalidateTracer.WriteInfo("================");
                this.visualroot.dbugGraphicInvalidateTracer.WriteInfo("MOUSEUP");
                this.visualroot.dbugGraphicInvalidateTracer.WriteInfo("================");
            }

#endif

            RenderElement hitElement = HitTestCoreWithPrevChainHint(e.X, e.Y, HitEventName.MouseUp);
            if (hitElement != null)
            {
                disableGraphicOutputFlush = true;

                Point globalLocation = hitElement.GetGlobalLocation();
                e.TranslateCanvasOrigin(globalLocation);
                e.Location = hitPointChain.CurrentHitPoint;

                e.SourceVisualElement = hitElement;
                IEventListener ui = hitElement.GetController() as IEventListener;
                if (ui != null)
                {
                    ui.ListenMouseEvent(UIMouseEventName.MouseUp, e);
                }
                e.TranslateCanvasOriginBack();

                disableGraphicOutputFlush = false;

                if (hitElement.Focusable)
                { 
                    //hitElement.Focus();
                    hitElement.WinRoot.CurrentKeyboardFocusedElement = hitElement; 
                } 
                FlushGraphicUpdate();
            }

            hitPointChain.SwapHitChain();
        }
        public new void OnKeyDown(UIKeyEventArgs e)
        {
            var visualroot = this.MyVisualRoot;
            e.IsShiftKeyDown = e.Shift;
            e.IsAltKeyDown = e.Alt;
            e.IsCtrlKeyDown = e.Control;

            if (currentKeyboardFocusedElement != null)
            {

                e.TranslateCanvasOrigin(globalXOfCurrentUI, globalYOfCurrentUI);
                e.SourceVisualElement = currentKeyboardFocusedElement;
                IEventListener ui = currentKeyboardFocusedElement.GetController() as IEventListener;
                if (ui != null)
                {
                    ui.ListenKeyEvent(UIKeyEventName.KeyDown, e);
                }
                e.TranslateCanvasOriginBack();
            }
        }
        public new void OnKeyUp(UIKeyEventArgs e)
        {



            var visualroot = this.MyVisualRoot;
            e.IsShiftKeyDown = e.Shift;
            e.IsAltKeyDown = e.Alt;
            e.IsCtrlKeyDown = e.Control;

            if (currentKeyboardFocusedElement != null)
            {
                e.TranslateCanvasOrigin(globalXOfCurrentUI, globalYOfCurrentUI);
                e.SourceVisualElement = currentKeyboardFocusedElement;

                IEventListener ui = currentKeyboardFocusedElement.GetController() as IEventListener;
                if (ui != null)
                {
                    ui.ListenKeyEvent(UIKeyEventName.KeyUp, e);
                }

                e.TranslateCanvasOriginBack();
            }


        }
        public void OnKeyPress(UIKeyPressEventArgs e)
        {

            if (currentKeyboardFocusedElement != null)
            {

                e.TranslateCanvasOrigin(globalXOfCurrentUI, globalYOfCurrentUI);
                e.SourceVisualElement = currentKeyboardFocusedElement;
                IEventListener ui = currentKeyboardFocusedElement.GetController() as IEventListener;
                if (ui != null)
                {
                    ui.ListenKeyPressEvent(e);
                }
                e.TranslateCanvasOriginBack();
            }
        }

        public bool OnProcessDialogKey(UIKeyEventArgs e)
        {

            bool result = false;
            if (currentKeyboardFocusedElement != null)
            {
                e.TranslateCanvasOrigin(globalXOfCurrentUI, globalYOfCurrentUI);

                e.SourceVisualElement = currentKeyboardFocusedElement;


                IEventListener ui = currentKeyboardFocusedElement.GetController() as IEventListener;
                if (ui != null)
                {
                    result = ui.ListenProcessDialogKey(e);
                }


                if (result && currentKeyboardFocusedElement != null)
                {
                    
                    currentKeyboardFocusedElement.InvalidateGraphic();
                     
                }
                e.TranslateCanvasOriginBack();
            }

            return result;
        }
     
        //public Point CaretPosition
        //{
        //    get
        //    {
        //        RenderElement currentElem = this.currentKeyboardFocusedElement;
        //        if (currentElem != null && currentElem.IsTextEditContainer)
        //        {

        //            Point elementCaretPosition = ((MultiLayerRenderBox)currentElem).CaretPosition;

        //            bool caretOutOfScope = false;
        //            if (elementCaretPosition.X >= currentElem.Right)
        //            {
        //                caretOutOfScope = true;
        //            }
        //            if (elementCaretPosition.Y >= currentElem.Bottom)
        //            {
        //                caretOutOfScope = true;
        //            } 
        //            if (!caretOutOfScope)
        //            {
        //                elementCaretPosition.Offset(currentElem.GetGlobalLocation());
        //                return elementCaretPosition;
        //            }
        //            else
        //            {

        //                return new Point(-10, -10);
        //            }

        //        }
        //        else
        //        {
        //            return new Point(-10, -10);
        //        }
        //    }
        //}

    }
}
