//Apache2, 2014-2017, WinterDev

using System;
using PixelFarm.Drawing;
using LayoutFarm.RenderBoxes;
namespace LayoutFarm
{
    public interface IRenderElement
    {
        void DrawToThisCanvas(Canvas canvas, Rectangle updateArea);
#if DEBUG
        void dbugShowRenderPart(Canvas canvas, Rectangle r);
#endif
    }


    public abstract partial class RootGraphic : IRootGraphics
    {
        public delegate void PaintToOutputWindowDelegate();
        protected PaintToOutputWindowDelegate paintToOutputWindowHandler;
        CanvasInvalidateDelegate canvasInvalidateDelegate;
        Rectangle accumulateInvalidRect;
        bool hasAccumRect;
        public RootGraphic(int width, int heigth)
        {
            this.Width = width;
            this.Height = heigth;
        }

        public abstract IFonts IFonts { get; }
        public abstract RequestFont DefaultTextEditFontInfo
        {
            get;
        }
        public abstract RenderBoxBase TopWindowRenderBox
        {
            get;
        }
        public int Width
        {
            get;
            internal set;
        }
        public int Height
        {
            get;
            internal set;
        }
        public abstract void CloseWinRoot();
        //-------------------------------------------------------------------------


        public abstract void ClearRenderRequests();
        public abstract void AddToLayoutQueue(RenderElement renderElement);
        public abstract void AddToElementUpdateQueue(object requestBy);
        public abstract void SetCurrentKeyboardFocus(RenderElement renderElement);
        //TODO: review this !
        public abstract void AddToUpdateQueue(object toupdateObj);
        public bool LayoutQueueClearing
        {
            get;
            set;
        }

        //--------------------------------------------------------------------------
        //timers
        public abstract bool GfxTimerEnabled { get; set; }
        public abstract GraphicsTimerTask SubscribeGraphicsIntervalTask(
            object uniqueName,
            TaskIntervalPlan planName,
            int intervalMs,
            EventHandler<GraphicsTimerTaskEventArgs> tickhandler);
        public abstract void RemoveIntervalTask(object uniqueName);
        //--------------------------------------------------------------------------
#if DEBUG

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
        //--------------------------------------------------------------------------

        public abstract void PrepareRender();
        public void FlushAccumGraphics()
        {
            if (!this.hasAccumRect)
            {
                return;
            }

            this.canvasInvalidateDelegate(accumulateInvalidRect);
            this.paintToOutputWindowHandler();
            hasAccumRect = false;
        }
        public void SetPaintDelegates(CanvasInvalidateDelegate canvasPaintToOutput, PaintToOutputWindowDelegate paintToOutputHandler)
        {
            this.canvasInvalidateDelegate = canvasPaintToOutput;
            this.paintToOutputWindowHandler = paintToOutputHandler;
        }

#if DEBUG
        void dbugWriteStopGfxBubbleUp(RenderElement fromElement, ref int dbug_ncount, int nleftOnStack, string state_str)
        {
            RootGraphic dbugMyroot = this;
            if (dbugMyroot.dbugEnableGraphicInvalidateTrace && dbugMyroot.dbugGraphicInvalidateTracer != null)
            {
                if (this.dbugNeedContentArrangement || this.dbugNeedReCalculateContentSize)
                {
                    state_str = "!!" + state_str;
                }
                dbugMyroot.dbugGraphicInvalidateTracer.WriteInfo(state_str, fromElement);
                while (dbug_ncount > nleftOnStack)
                {
                    dbugMyroot.dbugGraphicInvalidateTracer.PopElement();
                    dbug_ncount--;
                }
            }
        }
#endif


        public void InvalidateGraphicArea(RenderElement fromElement, ref Rectangle elemClientRect)
        {
            //total bounds = total bounds at level

            if (this.IsInRenderPhase) { return; }
            //--------------------------------------            
            //bubble up ,find global rect coord
            //and then merge to accumulate rect
            //int globalX = 0;
            //int globalY = 0;
            Point globalPoint = new Point();
            bool isBubbleUp = false;
#if DEBUG
            int dbug_ncount = 0;
            dbugWriteStopGfxBubbleUp(fromElement, ref dbug_ncount, dbug_ncount, ">> :" + elemClientRect.ToString());
#endif
            do
            {
                if (!fromElement.Visible)
                {
#if DEBUG
                    dbugWriteStopGfxBubbleUp(fromElement, ref dbug_ncount, 0, "EARLY-RET: ");
#endif
                    return;
                }
                else if (fromElement.BlockGraphicUpdateBubble)
                {
#if DEBUG
                    dbugWriteStopGfxBubbleUp(fromElement, ref dbug_ncount, 0, "BLOCKED2: ");
#endif
                    return;
                }
                //--------------------------------------------------------------------- 


#if DEBUG
                dbugWriteStopGfxBubbleUp(fromElement, ref dbug_ncount, dbug_ncount, ">> ");
#endif

                globalPoint.Offset(fromElement.X, fromElement.Y);
                //globalX += fromElement.BubbleUpX;
                //globalY += fromElement.BubbleUpY;


                if (fromElement.MayHasViewport && isBubbleUp)
                {
                    //elemClientRect.Offset(globalX, globalY);
                    elemClientRect.Offset(globalPoint);
                    if (fromElement.HasDoubleScrollableSurface)
                    {
                        //container.VisualScrollableSurface.WindowRootNotifyInvalidArea(elementClientRect);
                    }

                    Rectangle elementRect = fromElement.RectBounds;
                    elementRect.Offset(fromElement.ViewportX, fromElement.ViewportY);
                    elemClientRect.Intersect(elementRect);
                    globalPoint.X = -fromElement.ViewportX;
                    globalPoint.Y = -fromElement.ViewportY;
                    //globalX = -fromElement.ViewportX;
                    //globalY = -fromElement.ViewportY;
                }

                if (fromElement.IsTopWindow)
                {
                    break;
                }
                else
                {
#if DEBUG
                    if (fromElement.dbugParentVisualElement == null)
                    {
                        dbugWriteStopGfxBubbleUp(fromElement, ref dbug_ncount, 0, "BLOCKED3: ");
                    }
#endif

                    var parentLink = fromElement.MyParentLink;
                    if (parentLink == null)
                    {
                        return;
                    }
                    parentLink.AdjustLocation(ref globalPoint);
                    //move up
                    fromElement = parentLink.ParentRenderElement;// fromElement.ParentRenderElement;
                    if (fromElement == null)
                    {
                        return;
                    }
                }

                isBubbleUp = true;
            } while (true);
#if DEBUG
            var dbugMyroot = this;
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
            //elemClientRect.Offset(globalX, globalY);
            elemClientRect.Offset(globalPoint);
            if (elemClientRect.Top > this.Height
                || elemClientRect.Left > this.Width
                || elemClientRect.Bottom < 0
                || elemClientRect.Right < 0)
            {
                //no intersect with  

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
            //--------------------------------------------------------------------------------------------------
            if (!hasAccumRect)
            {
                accumulateInvalidRect = elemClientRect;
                hasAccumRect = true;
            }
            else
            {
                accumulateInvalidRect = Rectangle.Union(accumulateInvalidRect, elemClientRect);
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
        public bool IsInRenderPhase
        {
            get;
            set;
        }
        //--------------------------------------------- 
        //carets ...
        public abstract void CaretStartBlink();
        public abstract void CaretStopBlink();
        public bool CaretHandleRegistered { get; set; }
        //---------------------------------------------

        /// <summary>
        /// create new root graphics based on the same platform
        /// </summary>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <returns></returns>
        public abstract RootGraphic CreateNewOne(int w, int h);
        //---------------------------------------------



        //static TextBreakGenDel s_textBreakGen;
        //public static PixelFarm.Drawing.Text.TextBreaker GetTextBreaker(string locale)
        //{
        //    return s_textBreakGen(locale);
        //}
        //public static void SetTextBreakerGenerator(TextBreakGenDel textBreakGen)
        //{
        //    s_textBreakGen = textBreakGen;
        //}

        //public delegate PixelFarm.Drawing.Text.TextBreaker TextBreakGenDel(string locale);
    }



}