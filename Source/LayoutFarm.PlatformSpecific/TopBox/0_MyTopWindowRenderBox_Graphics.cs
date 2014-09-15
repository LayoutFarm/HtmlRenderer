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
        Stack<VisualDrawingChain> renderingChainStock = new Stack<VisualDrawingChain>(); 

        int GraphicUpdateSuspendCount
        {
            get { return this.visualroot.GraphicUpdateBlockCount; }
            set { this.visualroot.GraphicUpdateBlockCount = value; }
        }
        bool LayoutQueueClearing
        {
            get { return this.visualroot.LayoutQueueClearing; }
            set { this.visualroot.LayoutQueueClearing = value; }
        }
        bool DisableGraphicOutputFlush
        {
            get { return this.visualroot.DisableGraphicOutputFlush; }
            set { this.visualroot.DisableGraphicOutputFlush = value; }
        }
        public override void FlushGraphic(Rectangle rect)
        {
            UIInvalidateEventArgs e = this.eventStock.GetFreeCanvasInvalidatedEventArgs(); 
            e.InvalidArea = rect;
            CanvasInvalidatedEvent(this, e); 
            eventStock.ReleaseEventArgs(e);
        }
        void FlushAccumGraphicUpdate()
        {
            this.visualroot.FlushAccumGraphicUpdate(this); 
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
            this.visualroot.IsInRenderPhase = true;
#if DEBUG
            RootGraphic myroot = this.dbugVRoot;
            myroot.dbug_rootDrawingMsg.Clear();
            myroot.dbug_drawLevel = 0;
#endif
        }
        public void EndRenderPhase()
        {
            this.visualroot.IsInRenderPhase = false;
        }

    }
}