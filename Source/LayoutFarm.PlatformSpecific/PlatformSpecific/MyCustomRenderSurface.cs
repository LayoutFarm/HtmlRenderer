//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;



namespace LayoutFarm.Presentation
{
    public class MyCustomRenderSurface : CustomRenderSurface
    {


        QuadPages quadPages;

        //int viewport_h_smallChange = 0;
        //int viewport_h_largeChange = 0; 
        //int viewport_v_smallChange = 0;
        //int viewport_v_largeChange = 0;

        RenderBoxBase ownerVisualElement;
        bool scrollableFullMode;
        public MyCustomRenderSurface(RenderBoxBase ownerVisualElement, int width, int height)
            : base(ownerVisualElement)
        {

            this.ownerVisualElement = ownerVisualElement;
            FullModeUpdate = true;
            quadPages = new QuadPages(4, width, height);


            EvaluateScrollBar();

            FullModeUpdate = false;
        }

        public override bool FullModeUpdate
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



        public override int Width
        {
            get
            {
                return this.ownerVisualElement.Width;
            }
        }
        public override int Height
        {
            get
            {
                return this.ownerVisualElement.Height;
            }
        }
        public override void ConfirmSizeChanged()
        {

            RefreshSnapshotCanvas();
            EvaluateScrollBar();
        }

        public override void QuadPagesCalculateCanvas()
        {
            quadPages.CalculateCanvasPages(viewportX, viewportY, this.Width, this.Height);

        }
        public override Size OwnerInnerContentSize
        {
            get
            {

                return new Size(ownerVisualElement.ElementDesiredWidth, ownerVisualElement.ElementDesiredHeight);

            }
        }
        //public override int VerticalSmallChange
        //{
        //    get
        //    {
        //        return viewport_v_smallChange;
        //    }
        //}
        //public override int VerticalLargeChange
        //{
        //    get
        //    {
        //        return viewport_v_largeChange;
        //    }
        //}
        //public override int HorizontalLargeChange
        //{
        //    get
        //    {
        //        return viewport_h_largeChange;
        //    }
        //}
        //public override int HorizontalSmallChange
        //{
        //    get
        //    {
        //        return viewport_h_smallChange;
        //    }
        //}


        public override void WindowRootNotifyInvalidArea(InternalRect clientRect)
        {
            FullModeUpdate = false; quadPages.CanvasInvalidate(clientRect);

        }
        public override void DrawToThisPage(CanvasBase destPage, InternalRect updateArea)
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
            if (this.FullModeUpdate)
            {
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

        //------------------------------------

        void UpdateInternalCanvas(CanvasBase internalCanvas, InternalRect updateArea)
        {

            int logicalCanvasX = internalCanvas.Left;
            int logicalCanvasY = internalCanvas.Top;

            internalCanvas.OffsetCanvasOrigin(-logicalCanvasX, -logicalCanvasY);

            if (internalCanvas.PushClipAreaForNativeScrollableElement(updateArea))
            {

                internalCanvas.ClearSurface();

#if DEBUG
                if (dbugVRoot.dbug_RecordDrawingChain)
                {
                    dbugVRoot.dbug_AddDrawElement(this.ownerVisualElement, internalCanvas, "?pageName" + " { --update_inv_area--");
                }
#endif

                ownerVisualElement.CustomDrawToThisPage(internalCanvas, updateArea);
                

#if DEBUG

                if (dbugVRoot.dbug_ShowNativeScrollableElementUpdateArea)
                {
                    internalCanvas.dbug_DrawCrossRect(Color.Magenta, updateArea.ToRectangle());
                }
                if (dbugVRoot.dbug_RecordDrawingChain)
                {
                    dbugVRoot.dbug_AddDrawElement(this.ownerVisualElement, internalCanvas, "?pageName" + " }--update_inv_area--");
                }
#endif
            }
            internalCanvas.PopClipArea(); internalCanvas.OffsetCanvasOrigin(logicalCanvasX, logicalCanvasY);
        }
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

        void EvaluateScrollBar()
        {
            System.Drawing.Size innerContentSize = this.OwnerInnerContentSize;

            //viewport_v_largeChange = 32;
            //viewport_v_smallChange = viewport_v_largeChange / 4;

            //viewport_h_largeChange = Width / 2;
            //viewport_h_smallChange = viewport_h_largeChange / 4;


            QuadPagesCalculateCanvas();

            //this.RaiseScrollChangedEvents(innerContentSize.Height > this.Height, innerContentSize.Width > this.Width);

        }


        void DirectDrawToThisPage(CanvasBase destPage, InternalRect updateArea)
        {

            InternalRect internalRect = InternalRect.CreateFromWH(this.Width, this.Height);
            if (destPage.PushClipAreaForNativeScrollableElement(internalRect))
            {
                destPage.ClearSurface();
                ownerVisualElement.CustomDrawToThisPage(destPage, updateArea);

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





        void RefreshSnapshotCanvas()
        {

            int newBackCanvasWidth = this.Width;
            int newBackCanvasHeight = this.Height;

            //static int screenWidth = 1024;
            //static int screenHeight = 800;

            int initW = 1024;
            int initH = 800;

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
#if DEBUG
        public void dbug_DrawRuler(CanvasBase myInternalCanvas, int x)
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

        dbugRootElement dbugVRoot
        {
            get
            {

                return dbugRootElement.dbugCurrentGlobalVRoot;
            }
        }
        public string dbug_FullElementDescription()
        {
            return " SCR-INNER:" + OwnerInnerContentSize.ToString();
        }
#endif
    }


}