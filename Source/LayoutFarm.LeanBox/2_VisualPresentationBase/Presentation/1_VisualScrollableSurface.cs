using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using System.Drawing;

using LayoutFarm.Presentation;

namespace LayoutFarm.Presentation
{


                public sealed class VisualScrollableSurface
    {

                
                                QuadPages quadPages;

                                        public event EventHandler<ScrollSurfaceRequestEventArgs> VScrollRequest;
        public event EventHandler<ScrollSurfaceRequestEventArgs> HScrollRequest;
                public event EventHandler<ArtScrollEventArgs> VScrollChanged;
        public event EventHandler<ArtScrollEventArgs> HScrollChanged;
        
                        int viewport_h_smallChange = 0;
        int viewport_h_largeChange = 0;

        int viewport_v_smallChange = 0;
        int viewport_v_largeChange = 0;


                                ArtVisualContainerBase ownerVisualElement;


        bool scrollableFullMode;


        public VisualScrollableSurface(ArtVisualContainerBase ownerVisualElement, int width, int height)
        {

            this.ownerVisualElement = ownerVisualElement;
            FullModeUpdate = true;            
            quadPages = new QuadPages(
                4,                 width,                height);
                        

            EvaluateScrollBar();

            FullModeUpdate = false;         }
                                int viewportX
        {
            get
            {
                return this.ownerVisualElement.ViewportX;
            } 
        }
                                int viewportY
        {
            get
            {
                return this.ownerVisualElement.ViewportY;
            } 
        }
        public bool FullModeUpdate
        {
            get
            {
                return this.scrollableFullMode;
            }
            set
            {
                this.scrollableFullMode = value;
            }
        }
        public bool HasHScrollChanged
        {
            get
            {
                return this.HScrollChanged != null;
            }
        }
        public bool HasVScrollChanged
        {
            get
            {
                return this.VScrollChanged != null;
            }
        }
        public void RaiseProperEvents(ArtScrollEventArgs hScrollEventArgs, ArtScrollEventArgs vScrollEventArgs)
        {
            if (this.VScrollChanged != null && vScrollEventArgs != null)
            {
                VScrollChanged.Invoke(ownerVisualElement, vScrollEventArgs);
            }
            if (this.HScrollChanged != null && hScrollEventArgs != null)
            {
                HScrollChanged.Invoke(ownerVisualElement, hScrollEventArgs);
            }
        }

                                public int Width
        {
            get
            {
                return this.ownerVisualElement.Width;
            }
        }
                                public int Height
        {
            get
            {
                return this.ownerVisualElement.Height;
            }
        }
#if DEBUG
        public void dbug_DrawRuler(ArtCanvas myInternalCanvas, int x)
        {
            if (dbugVRoot.dbug_ShowNativeScrollableElementDrawToThisPage)
            {
                int internalCx = myInternalCanvas.Left;
                int internalCy = myInternalCanvas.Top;
                myInternalCanvas.OffsetCanvasOrigin(-internalCx, -internalCy);
                myInternalCanvas.PushTextColor(Color.LightGray);
                myInternalCanvas.dbug_DrawRuler(x);
                myInternalCanvas.PopTextColor();
                myInternalCanvas.OffsetCanvasOrigin(internalCx, internalCy);
            }
        }
#endif

                                                void UpdateInternalCanvas(ArtCanvas internalCanvas, InternalRect updateArea)
        {

                                                                        int logicalCanvasX = internalCanvas.Left;
            int logicalCanvasY = internalCanvas.Top;

            internalCanvas.OffsetCanvasOrigin(-logicalCanvasX, -logicalCanvasY);

            if (internalCanvas.PushClipAreaForNativeScrollableElement(updateArea))             {

                                                internalCanvas.ClearSurface();

#if DEBUG
                                if (dbugVRoot.dbug_RecordDrawingChain)
                {
                                        dbugVRoot.dbug_AddDrawElement(this.ownerVisualElement, internalCanvas, "?pageName" + " { --update_inv_area--");
                }
#endif

                
                                                ownerVisualElement.ContainerDrawOriginalContent(internalCanvas, updateArea);
                
#if DEBUG

                if (dbugVRoot.dbug_ShowNativeScrollableElementUpdateArea)
                {
                                        internalCanvas.dbug_DrawCrossRect(Color.Magenta, updateArea.ToRectangle());
                                    }
                if (dbugVRoot.dbug_RecordDrawingChain)
                {                       dbugVRoot.dbug_AddDrawElement(this.ownerVisualElement, internalCanvas, "?pageName" + " }--update_inv_area--");
                }
#endif
            }
            internalCanvas.PopClipArea();            internalCanvas.OffsetCanvasOrigin(logicalCanvasX, logicalCanvasY);
        }

#if DEBUG
        VisualRoot dbugVRoot
        {
            get
            {

                return VisualRoot.dbugCurrentGlobalVRoot;
            }
        }
#endif
                                public void ConfirmSizeChanged()
        {

            RefreshSnapshotCanvas();
            EvaluateScrollBar();
        }

                                        public void QuadPagesCalculateCanvas()
        {
            quadPages.CalculateCanvasPages(viewportX, viewportY, this.Width, this.Height);

                    }



                                public Size OwnerInnerContentSize
        {
            get
            {
                                return ownerVisualElement.InnerContentSize;

            }
        }

#if DEBUG

        public string dbug_FullElementDescription()
        {
                        return " SCR-INNER:" + OwnerInnerContentSize.ToString();
        }

#endif

                                        void EvaluateScrollBar()
        {
                        System.Drawing.Size innerContentSize = this.OwnerInnerContentSize;
                        
                        viewport_v_largeChange = 32;             viewport_v_smallChange = viewport_v_largeChange / 4;

            viewport_h_largeChange = Width / 2;
            viewport_h_smallChange = viewport_h_largeChange / 4;

            
            QuadPagesCalculateCanvas();


            if (VScrollRequest != null)            {
                if (innerContentSize.Height <= Height)
                {
                    
                    VScrollRequest.Invoke(this, new ScrollSurfaceRequestEventArgs(false));
                }
                else
                {    
                    VScrollRequest.Invoke(this, new ScrollSurfaceRequestEventArgs(true));
                }
            }
            if (HScrollRequest != null)            {
                if (innerContentSize.Width <= Width)
                {
                    
                    HScrollRequest.Invoke(this, new ScrollSurfaceRequestEventArgs(false));
                }
                else
                {
                                        HScrollRequest.Invoke(this, new ScrollSurfaceRequestEventArgs(true));
                }
            }
        }

        public int VerticalSmallChange
        {
            get
            {
                return viewport_v_smallChange;
            }
        }
        public int VerticalLargeChange
        {
            get
            {
                return viewport_v_largeChange;
            }
        }
        public int HorizontalLargeChange
        {
            get
            {
                return viewport_h_largeChange;
            }
        }
        public int HorizontalSmallChange
        {
            get
            {
                return viewport_h_smallChange;
            }
        }


                                                public void WindowRootNotifyInvalidArea(InternalRect clientRect)
        {
                                                            FullModeUpdate = false;            quadPages.CanvasInvalidate(clientRect);

        }
                                                void DirectDrawToThisPage(ArtCanvas destPage, InternalRect updateArea)
        {
                                                
                                    InternalRect internalRect = InternalRect.CreateFromWH(this.Width, this.Height);
            if (destPage.PushClipAreaForNativeScrollableElement(internalRect))
            {
                                destPage.ClearSurface();
                                ownerVisualElement.ContainerDrawOriginalContent(destPage, updateArea);

            }
            destPage.PopClipArea();
            InternalRect.FreeInternalRect(internalRect);
            
        }
                                        void UpdateInternalCanvasFullMode(InternalRect internalUpdateArea)
        {
                                                                                                            switch (quadPages.RenderParts)
            {
                case QuadPages.PAGE_A:
                    {
                        if (!quadPages.pageA.IsContentUpdated)
                        {
                                                        InternalRect pageARect = InternalRect.CreateFromRect(quadPages.pageA.Rect);
                            UpdateInternalCanvas(quadPages.pageA, pageARect);
                            InternalRect.FreeInternalRect(pageARect);
#if DEBUG
                            dbug_DrawRuler(quadPages.pageA, 0);
#endif
                        }
                    } break;
                case QuadPages.PAGE_AB:
                    {
                        if (!quadPages.pageA.IsContentUpdated)
                        {

                                                        InternalRect pageARect = InternalRect.CreateFromRect(quadPages.pageA.Rect);
                            UpdateInternalCanvas(quadPages.pageA, pageARect);
                            InternalRect.FreeInternalRect(pageARect);
#if DEBUG
                            dbug_DrawRuler(quadPages.pageA, 0);
#endif
                        }
                        if (!quadPages.pageB.IsContentUpdated)
                        {
                            
                            InternalRect pageBRect = InternalRect.CreateFromRect(quadPages.pageB.Rect);
                            UpdateInternalCanvas(quadPages.pageB, pageBRect);
                            InternalRect.FreeInternalRect(pageBRect);
#if DEBUG
                            dbug_DrawRuler(quadPages.pageB, 10);
#endif
                        }

                    } break;
                case QuadPages.PAGE_AC:
                    {
                        if (!quadPages.pageA.IsContentUpdated)
                        {

                                                        InternalRect pageARect = InternalRect.CreateFromRect(quadPages.pageA.Rect);
                            UpdateInternalCanvas(quadPages.pageA, pageARect);
                            InternalRect.FreeInternalRect(pageARect);
#if DEBUG
                            dbug_DrawRuler(quadPages.pageA, 0);
#endif
                        }
                        if (!quadPages.pageC.IsContentUpdated)
                        {
                                                                                    InternalRect pageCRect = InternalRect.CreateFromRect(quadPages.pageC.Rect);
                            UpdateInternalCanvas(quadPages.pageC, pageCRect);
                            InternalRect.FreeInternalRect(pageCRect);
#if DEBUG
                            dbug_DrawRuler(quadPages.pageC, 0);
#endif
                        }
                    } break;
                case QuadPages.PAGE_ABCD:
                    {
                        if (!quadPages.pageA.IsContentUpdated)
                        {
                                                        InternalRect pageARect = InternalRect.CreateFromRect(quadPages.pageA.Rect);
                            UpdateInternalCanvas(quadPages.pageA, pageARect);
                            InternalRect.FreeInternalRect(pageARect);
#if DEBUG
                            dbug_DrawRuler(quadPages.pageA, 0);
#endif
                        }
                        if (!quadPages.pageB.IsContentUpdated)
                        {
                                                        InternalRect pageBRect = InternalRect.CreateFromRect(quadPages.pageB.Rect);
                            UpdateInternalCanvas(quadPages.pageB, pageBRect);
                            InternalRect.FreeInternalRect(pageBRect);
#if DEBUG
                            dbug_DrawRuler(quadPages.pageB, 10);
#endif
                        }
                        if (!quadPages.pageC.IsContentUpdated)
                        {
                                                                                    InternalRect pageCRect = InternalRect.CreateFromRect(quadPages.pageC.Rect);
                            UpdateInternalCanvas(quadPages.pageC, pageCRect);
                            InternalRect.FreeInternalRect(pageCRect);
#if DEBUG
                            dbug_DrawRuler(quadPages.pageC, 0);
#endif
                        }
                        if (!quadPages.pageD.IsContentUpdated)
                        {
                                                        InternalRect pageDRect = InternalRect.CreateFromRect(quadPages.pageD.Rect);
                            UpdateInternalCanvas(quadPages.pageD, pageDRect);
#if DEBUG
                            dbug_DrawRuler(quadPages.pageC, 10);
#endif
                        }
                    } break;
            }

        }


                                        void UpdateInternalCanvasPartialMode(InternalRect internalUpdateArea)
        {


                                                                                                
            switch (quadPages.RenderParts)
            {
                case QuadPages.PAGE_A:
                    {
                        if (!quadPages.pageA.IsContentUpdated)
                        {

                            
                                                        UpdateInternalCanvas(quadPages.pageA, internalUpdateArea);
#if DEBUG
                            dbug_DrawRuler(quadPages.pageA, 0);
#endif
                        }
                    } break;
                case QuadPages.PAGE_AB:
                    {
                        if (!quadPages.pageA.IsContentUpdated)
                        {
                                                        UpdateInternalCanvas(quadPages.pageA, internalUpdateArea);
#if DEBUG
                            dbug_DrawRuler(quadPages.pageA, 0);
#endif
                        }
                        if (!quadPages.pageB.IsContentUpdated)
                        {
                                                                                    UpdateInternalCanvas(quadPages.pageB, internalUpdateArea);
#if DEBUG
                            dbug_DrawRuler(quadPages.pageB, 10);
#endif
                        }

                    } break;
                case QuadPages.PAGE_AC:
                    {
                        if (!quadPages.pageA.IsContentUpdated)
                        {
                                                        UpdateInternalCanvas(quadPages.pageA, internalUpdateArea);
#if DEBUG
                            dbug_DrawRuler(quadPages.pageA, 0);
#endif
                        }
                        if (!quadPages.pageC.IsContentUpdated)
                        {
                                                                                    UpdateInternalCanvas(quadPages.pageC, internalUpdateArea);
#if DEBUG
                            dbug_DrawRuler(quadPages.pageC, 0);
#endif
                        }
                    } break;
                case QuadPages.PAGE_ABCD:
                    {
                        if (!quadPages.pageA.IsContentUpdated)
                        {
                                                        UpdateInternalCanvas(quadPages.pageA, internalUpdateArea);
#if DEBUG
                            dbug_DrawRuler(quadPages.pageA, 0);
#endif
                        }
                        if (!quadPages.pageB.IsContentUpdated)
                        {
                                                                                    UpdateInternalCanvas(quadPages.pageB, internalUpdateArea);
#if DEBUG
                            dbug_DrawRuler(quadPages.pageB, 10);
#endif
                        }
                        if (!quadPages.pageC.IsContentUpdated)
                        {
                                                                                    UpdateInternalCanvas(quadPages.pageC, internalUpdateArea);
#if DEBUG
                            dbug_DrawRuler(quadPages.pageC, 0);
#endif
                        }
                        if (!quadPages.pageD.IsContentUpdated)
                        {
                                                                                    UpdateInternalCanvas(quadPages.pageD, internalUpdateArea);
#if DEBUG
                            dbug_DrawRuler(quadPages.pageC, 10);
#endif
                        }
                    } break;
            }


                                    
                                                            
                                                            

                                                            
                        


        }



                                                public void DrawToThisPage(ArtCanvas destPage, InternalRect updateArea)
        {
                                                                                                            if (quadPages.pageA == null)
            {
                return;
            }
                                                            if (destPage.IsFromPrinter)
            {
                DirectDrawToThisPage(destPage, updateArea);
                return;
            }
                                                                        
            InternalRect logicalArea = InternalRect.CreateFromRect(destPage.CurrentClipRect);

                        logicalArea.Offset(viewportX, viewportY); 
            if (this.FullModeUpdate)             {
                UpdateInternalCanvasFullMode(logicalArea);
            }
            else
            {


                UpdateInternalCanvasPartialMode(logicalArea);
            }
                        quadPages.TransferDataFromSourceCanvas(
                logicalArea,
                viewportX,
                viewportY,
                this.Width,
                this.Height,
                destPage);
            
            InternalRect.FreeInternalRect(logicalArea);

        }


                                void RefreshSnapshotCanvas()
        {

                        int newBackCanvasWidth = this.Width;
                        int newBackCanvasHeight = this.Height;


            int initW = VisualRoot.ScreenWidth;
            int initH = VisualRoot.ScreenHeight;

            if (newBackCanvasWidth < initW / 3)
            {
                initW /= 2;
            }
            if (newBackCanvasHeight < initH / 3)
            {
                initH /= 2;
            }


                        if ((quadPages.EachPageWidth < initW) || (quadPages.EachPageHeight < initH))
            {

                quadPages.ResizeAllPages(initW, initH);
                                QuadPagesCalculateCanvas();
                FullModeUpdate = false;
                            }
            
        }

    }


}