// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;
using LayoutFarm.RenderBoxes;

namespace LayoutFarm
{
    public abstract partial class RootGraphic
    {
        Rectangle flushRect;
        Rectangle accumulateInvalidRect;
        bool hasAccumRect;

        public RootGraphic(int width, int heigth)
        {
            this.Width = width;
            this.Height = heigth;
        }

        public abstract GraphicsPlatform P { get; }

        public IFonts SampleIFonts { get { return this.P.SampleIFonts; } }

        public abstract void CaretStartBlink();
        public abstract void CaretStopBlink();

        public abstract void ClearRenderRequests(TopWindowRenderBox topwin);

        public int GraphicUpdateBlockCount
        {
            get;
            set;
        }
        public int Width
        {
            get;
            set;
        }
        public int Height
        {
            get;
            set;
        }
        public bool DisableGraphicOutputFlush
        {
            get;
            set;
        }
        public bool LayoutQueueClearing
        {
            get;
            set;
        }
        public bool IsInRenderPhase
        {
            get;
            set;
        }
        public abstract void CloseWinRoot();
        public void BeginGraphicUpdate()
        {
            GraphicUpdateBlockCount++;
            DisableGraphicOutputFlush = true;
        }
        public void EndGraphicUpdate(TopWindowRenderBox topbox)
        {
            GraphicUpdateBlockCount--;
            if (GraphicUpdateBlockCount <= 0)
            {
                DisableGraphicOutputFlush = false;
                FlushAccumGraphicUpdate(topbox);
                GraphicUpdateBlockCount = 0;
            }
        }

        public abstract GraphicsTimerTask RequestGraphicsIntervalTask(
            object uniqueName,
            TaskIntervalPlan planName,
            int intervalMs,
            EventHandler<GraphicsTimerTaskEventArgs> tickhandler);

        public abstract void RemoveIntervalTask(object uniqueName);

#if DEBUG
        RootGraphic dbugVRoot
        {
            get { return this; }
        }
        bool dbugNeedContentArrangement
        {
            get;
            set;
        }
        bool dbugNeedReCalculateContentSize
        {
            get;
            set;
        }
#endif
        public void InvalidateGraphicArea(RenderElement fromElement,
            ref Rectangle elementClientRect,
            out TopWindowRenderBox wintop)
        {

            if (IsInRenderPhase)
            {
                wintop = null;
                return;
            }


            InvalidateGraphicArea(fromElement, elementClientRect, out wintop);
        }
        public void FlushAccumGraphicUpdate(TopWindowRenderBox topbox)
        {
            if (hasAccumRect)
            {
                topbox.FlushGraphic(accumulateInvalidRect);
                hasAccumRect = false;
            }
            this.GraphicUpdateBlockCount = 0;
        }
        void InvalidateGraphicArea(RenderElement fromElement,
            Rectangle elementClientRect,
            out TopWindowRenderBox wintop)
        {
            if (this.IsInRenderPhase)
            {
                wintop = null;
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

                dbugMyroot.dbugGraphicInvalidateTracer.WriteInfo(">> :" + elementClientRect.ToString(),
                    startVisualElement);
            }
            int dbug_ncount = 0;

#endif
            wintop = null;

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


                globalX += fromElement.BubbleUpX;
                globalY += fromElement.BubbleUpY;


                if (fromElement.MayHasViewport && isBubbleUp)
                {


                    elementClientRect.Offset(globalX, globalY);
                    if (fromElement.HasDoubleScrollableSurface)
                    {
                        //container.VisualScrollableSurface.WindowRootNotifyInvalidArea(elementClientRect);
                    }
                    Rectangle elementRect = fromElement.BoundRect;
                    elementRect.Offset(fromElement.ViewportX, fromElement.ViewportY);
                    elementClientRect.Intersect(elementRect);
                    globalX = -fromElement.ViewportX;
                    globalY = -fromElement.ViewportY;
                }

                if (fromElement.IsTopWindow)
                {
                    wintop = (TopWindowRenderBox)fromElement;
                    break;
                }
                else
                {

#if DEBUG
                    if (fromElement.dbugParentVisualElement == null)
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
            Rectangle rootGlobalArea = elementClientRect;


            if (elementClientRect.Top > this.Height
                || elementClientRect.Left > this.Width
                || elementClientRect.Bottom < 0
                || elementClientRect.Right < 0)
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



            if (!this.DisableGraphicOutputFlush)
            {
                if (hasAccumRect)
                {
                                         
                    accumulateInvalidRect = Rectangle.Union(accumulateInvalidRect, rootGlobalArea);
#if DEBUG
                    if (dbugMyroot.dbugEnableGraphicInvalidateTrace &&
                        dbugMyroot.dbugGraphicInvalidateTracer != null)
                    {
                        string state_str = "SUDDEN_1: ";
                        if (this.dbugNeedContentArrangement || this.dbugNeedReCalculateContentSize)
                        {
                            state_str = "!!" + state_str;
                        }
                        dbugMyroot.dbugGraphicInvalidateTracer.WriteInfo(state_str + accumulateInvalidRect);
                        dbugMyroot.dbugGraphicInvalidateTracer.WriteInfo("\r\n");
                    }
#endif


                    wintop.FlushGraphic(rootGlobalArea);
                    this.flushRect = accumulateInvalidRect;


                    hasAccumRect = false;
                }
                else
                {

                    wintop.FlushGraphic(rootGlobalArea);

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
                               rootGlobalArea.ToString() + " " +
                               startVisualElement.dbug_FullElementDescription());
                        dbugMyroot.dbugGraphicInvalidateTracer.WriteInfo("\r\n");
                    }
#endif
                }
            }
            else
            {
                if (!hasAccumRect)
                {
                    accumulateInvalidRect = rootGlobalArea;
                    hasAccumRect = true;
                }
                else
                {
                    accumulateInvalidRect = Rectangle.Union(accumulateInvalidRect, rootGlobalArea);
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

    }
}