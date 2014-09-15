//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using System.Diagnostics;



using LayoutFarm;


namespace LayoutFarm
{

    partial class MyTopWindowRenderBox
    {

        RenderElement currentKeyboardFocusedElement = null;

        InternalRect accumulateInvalidRect = null;

        Stack<VisualDrawingChain> renderingChainStock = new Stack<VisualDrawingChain>();

        Rectangle flushRect;

        public override void InvalidateGraphicArea(RenderElement fromElement, ref Rectangle elementClientRect)
        {
            var rect = InternalRect.CreateFromRect(elementClientRect);
            InvalidateGraphicArea(fromElement, rect);
            InternalRect.FreeInternalRect(rect);
        }

        int GraphicUpdateSuspendCount
        {
            get { return this.visualroot.graphicUpdateBlockCount; }
            set { this.visualroot.graphicUpdateBlockCount = value; }
        }
        bool DisableGraphicOutputFlush
        {
            get { return this.visualroot.disableGraphicOutputFlush; }
            set { this.visualroot.disableGraphicOutputFlush = value; }
        }
        void InvalidateGraphicArea(RenderElement fromElement, InternalRect elementClientRect)
        {
            if (isInRenderPhase)
            {
                return;
            }
            int globalX = 0;
            int globalY = 0;
            bool isBubbleUp = false;

            RenderElement startVisualElement = fromElement;

#if DEBUG

            RootGraphic dbugMyroot = this.dbugVRoot;
            if (dbugMyroot.dbugEnableGraphicInvalidateTrace && dbugMyroot.dbugGraphicInvalidateTracer != null)
            {

                dbugMyroot.dbugGraphicInvalidateTracer.WriteInfo(">> :" + elementClientRect.ToRectangle().ToString()
                    , startVisualElement);
            }
            int dbug_ncount = 0;
#endif
            do
            {
#if DEBUG

                if (!fromElement.Visible)
                {

                    if (dbugMyroot.dbugEnableGraphicInvalidateTrace &&
                        dbugMyroot.dbugGraphicInvalidateTracer != null)
                    {

                        string state_str = "EARLY-RET: ";
                        if (this.dbugNeedContentArrangement || this.dbugNeedReCalculateContentSize)
                        {
                            state_str = "!!" + state_str;
                        }
                        dbugMyroot.dbugGraphicInvalidateTracer.WriteInfo(state_str, fromElement);
                        while (dbug_ncount > 0)
                        {
                            dbugMyroot.dbugGraphicInvalidateTracer.PopElement();
                            dbug_ncount--;
                        }
                    }
                    return;
                }
                else if (fromElement.IsInvalidateGraphicBlocked)
                {
                    if (dbugMyroot.dbugEnableGraphicInvalidateTrace &&
                            dbugMyroot.dbugGraphicInvalidateTracer != null)
                    {
                        string state_str = "BLOCKED2: ";
                        if (this.dbugNeedContentArrangement || this.dbugNeedReCalculateContentSize)
                        {
                            state_str = "!!" + state_str;
                        }
                        dbugMyroot.dbugGraphicInvalidateTracer.WriteInfo(state_str, fromElement); while (dbug_ncount > 0)
                        {
                            dbugMyroot.dbugGraphicInvalidateTracer.PopElement();
                            dbug_ncount--;
                        }
                    }
                    return;
                }
#else 
                if (!fromElement.Visible || fromElement.IsInvalidateGraphicBlocked)
                {
                    return;
                }
#endif


#if DEBUG
                if (dbugMyroot.dbugEnableGraphicInvalidateTrace &&
                    dbugMyroot.dbugGraphicInvalidateTracer != null)
                {
                    dbug_ncount++;
                    dbugMyroot.dbugGraphicInvalidateTracer.PushVisualElement(fromElement);
                    string state_str = ">> ";
                    if (this.dbugNeedContentArrangement || this.dbugNeedReCalculateContentSize)
                    {
                        state_str = "!!" + state_str;
                    }
                    dbugMyroot.dbugGraphicInvalidateTracer.WriteInfo(state_str, fromElement);
                }
#endif


                globalX += fromElement.X;
                globalY += fromElement.Y;


                if (fromElement.MayHasViewport && isBubbleUp)
                {

                    RenderBoxBase container = (RenderBoxBase)fromElement;
                    elementClientRect.Offset(globalX, globalY);
                    if (fromElement.HasDoubleScrollableSurface)
                    {
                        //container.VisualScrollableSurface.WindowRootNotifyInvalidArea(elementClientRect);
                    }
                    Rectangle elementRect = fromElement.BoundRect;
                    elementRect.Offset(container.ViewportX, container.ViewportY);
                    elementClientRect.Intersect(elementRect);
                    globalX = -container.ViewportX;
                    globalY = -container.ViewportY;
                }

                if (fromElement.IsTopWindow)
                {
                    break;
                }
                else
                {

#if DEBUG
                    if (fromElement.ParentVisualElement == null)
                    {


                        if (dbugMyroot.dbugEnableGraphicInvalidateTrace &&
                            dbugMyroot.dbugGraphicInvalidateTracer != null)
                        {
                            string state_str = "BLOCKED3: ";
                            if (this.dbugNeedContentArrangement || this.dbugNeedReCalculateContentSize)
                            {
                                state_str = "!!" + state_str;
                            }
                            dbugMyroot.dbugGraphicInvalidateTracer.WriteInfo(state_str, fromElement); while (dbug_ncount > 0)
                            {
                                dbugMyroot.dbugGraphicInvalidateTracer.PopElement();
                                dbug_ncount--;
                            }
                        }
                    }
#endif

                    fromElement = fromElement.ParentVisualElement;
                    if (fromElement == null)
                    {
                        return;
                    }
                }
                isBubbleUp = true;
            } while (true);

#if DEBUG
            if (dbugMyroot.dbugEnableGraphicInvalidateTrace
             && dbugMyroot.dbugGraphicInvalidateTracer != null)
            {
                while (dbug_ncount > 0)
                {
                    dbugMyroot.dbugGraphicInvalidateTracer.PopElement();
                    dbug_ncount--;
                }
            }
#endif

            //----------------------------------------
            elementClientRect.Offset(globalX, globalY);
            Rectangle rootGlobalArea = elementClientRect.ToRectangle();


            if (elementClientRect._top > this.Height
                || elementClientRect._left > this.Width
                || elementClientRect._bottom < 0
                || elementClientRect._right < 0)
            {
#if DEBUG
                if (dbugMyroot.dbugEnableGraphicInvalidateTrace &&
                    dbugMyroot.dbugGraphicInvalidateTracer != null)
                {
                    dbugMyroot.dbugGraphicInvalidateTracer.WriteInfo("ZERO-EEX");
                    dbugMyroot.dbugGraphicInvalidateTracer.WriteInfo("\r\n");
                }
#endif
                return;
            }

            if (!DisableGraphicOutputFlush)
            {
                if (accumulateInvalidRect != null)
                {

                    accumulateInvalidRect.MergeRect(rootGlobalArea);
#if DEBUG
                    if (dbugMyroot.dbugEnableGraphicInvalidateTrace &&
                        dbugMyroot.dbugGraphicInvalidateTracer != null)
                    {
                        string state_str = "SUDDEN_1: ";
                        if (this.dbugNeedContentArrangement || this.dbugNeedReCalculateContentSize)
                        {
                            state_str = "!!" + state_str;
                        }
                        dbugMyroot.dbugGraphicInvalidateTracer.WriteInfo(state_str + accumulateInvalidRect.ToRectangle());
                        dbugMyroot.dbugGraphicInvalidateTracer.WriteInfo("\r\n");
                    }
#endif

                    if (CanvasInvalidatedEvent != null)
                    {

                        UIInvalidateEventArgs e = this.eventStock.GetFreeCanvasInvalidatedEventArgs();
                        InternalRect internalRect = InternalRect.CreateFromRect(rootGlobalArea);
                        e.InvalidArea = internalRect;
                        CanvasInvalidatedEvent(this, e);
                        InternalRect.FreeInternalRect(internalRect);
                        eventStock.ReleaseEventArgs(e);
                    }



                    this.flushRect = accumulateInvalidRect.ToRectangle();


                    InternalRect.FreeInternalRect(accumulateInvalidRect);
                    accumulateInvalidRect = null;
                }
                else
                {
                    if (CanvasInvalidatedEvent != null)
                    {

                        UIInvalidateEventArgs e = this.eventStock.GetFreeCanvasInvalidatedEventArgs();
                        InternalRect internalRect = InternalRect.CreateFromRect(rootGlobalArea);
                        e.InvalidArea = internalRect;
                        CanvasInvalidatedEvent(this, e);
                        InternalRect.FreeInternalRect(internalRect);

                        eventStock.ReleaseEventArgs(e);
#if DEBUG
                        if (dbugMyroot.dbugEnableGraphicInvalidateTrace &&
                        dbugMyroot.dbugGraphicInvalidateTracer != null)
                        {

                            string state_str = "SUDDEN_2: ";
                            if (this.dbugNeedContentArrangement || this.dbugNeedReCalculateContentSize)
                            {
                                state_str = "!!" + state_str;
                            }
                            dbugMyroot.dbugGraphicInvalidateTracer.WriteInfo(state_str +
                                   internalRect.ToString() + " " +
                                   startVisualElement.dbug_FullElementDescription());
                            dbugMyroot.dbugGraphicInvalidateTracer.WriteInfo("\r\n");
                        }
#endif
                    }

                }
            }
            else
            {
                if (accumulateInvalidRect == null)
                {
                    accumulateInvalidRect = InternalRect.CreateFromRect(rootGlobalArea);
                }
                else
                {
                    accumulateInvalidRect.MergeRect(rootGlobalArea);
                }


#if DEBUG
                if (dbugMyroot.dbugEnableGraphicInvalidateTrace &&
                    dbugMyroot.dbugGraphicInvalidateTracer != null)
                {
                    string state_str = "ACC: ";
                    if (this.dbugNeedContentArrangement || this.dbugNeedReCalculateContentSize)
                    {
                        state_str = "!!" + state_str;
                    }
                    dbugMyroot.dbugGraphicInvalidateTracer.WriteInfo("ACC: " + accumulateInvalidRect.ToString());
                    dbugMyroot.dbugGraphicInvalidateTracer.WriteInfo("\r\n");
                }
#endif
            }
        }

        void FlushAccumGraphicUpdate()
        {
            if (accumulateInvalidRect != null)
            {

                if (CanvasInvalidatedEvent != null)
                {

                    UIInvalidateEventArgs e = this.eventStock.GetFreeCanvasInvalidatedEventArgs();
                    e.InvalidArea = accumulateInvalidRect;
#if DEBUG
                    if (this.visualroot.dbugEnableGraphicInvalidateTrace)
                    {

                    }
#endif
                    //--------------------------------
                    //invoke actual platform-specific rendering process
                    CanvasInvalidatedEvent(this, e);
                    //--------------------------------

                    eventStock.ReleaseEventArgs(e);
                }

                flushRect = accumulateInvalidRect.ToRectangle();

                InternalRect.FreeInternalRect(accumulateInvalidRect);
                accumulateInvalidRect = null;

            }
            GraphicUpdateSuspendCount = 0;
        }


        public bool IsCurrentElementUseCaret
        {
            get
            {
                return currentKeyboardFocusedElement != null && currentKeyboardFocusedElement.NeedSystemCaret;

            }
        }

        public override void RootBeginGraphicUpdate()
        {
            GraphicUpdateSuspendCount++;
            DisableGraphicOutputFlush = true;
        }
        public override void RootEndGraphicUpdate()
        {
            GraphicUpdateSuspendCount--;
            if (GraphicUpdateSuspendCount <= 0)
            {
                DisableGraphicOutputFlush = false;
                FlushAccumGraphicUpdate();
                GraphicUpdateSuspendCount = 0;
            }
        }
        public void ClearNotificationSizeChangeList()
        {
            if (tobeNotifySizeChangedList.Count > 0)
            {
                int sizeChangeCount = tobeNotifySizeChangedList.Count;
                for (int i = 0; i < sizeChangeCount; ++i)
                {
                    ToNotifySizeChangedEvent item = tobeNotifySizeChangedList[i];
                    UISizeChangedEventArgs sizeChangedEventArg = UISizeChangedEventArgs.GetFreeOne(
                        null, item.xdiff, item.ydiff, item.affectedSideFlags);



                    UISizeChangedEventArgs.ReleaseOne(sizeChangedEventArg);
                }
                tobeNotifySizeChangedList.Clear();
            }
        }

        public void BeginRenderPhase()
        {
            isInRenderPhase = true;
#if DEBUG
            RootGraphic myroot = this.dbugVRoot;
            myroot.dbug_rootDrawingMsg.Clear();
            myroot.dbug_drawLevel = 0;
#endif
        }
        public void EndRenderPhase()
        {
            isInRenderPhase = false;
        }


    }
}