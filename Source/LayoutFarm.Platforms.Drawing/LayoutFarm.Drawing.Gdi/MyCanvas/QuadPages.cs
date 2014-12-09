//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Drawing2D;

namespace LayoutFarm.Drawing
{

    public class QuadPages
    {

        internal MyCanvas pageA;
        internal MyCanvas pageB;
        internal MyCanvas pageC;
        internal MyCanvas pageD;
        CanvasCollection physicalCanvasCollection;

        public QuadPages(int cachedPageNum, int eachCachedPageWidth, int eachCachedPageHeight)
        {

            physicalCanvasCollection = new CanvasCollection(cachedPageNum, eachCachedPageWidth, eachCachedPageHeight);

        }
        public int EachPageWidth
        {
            get
            {
                return physicalCanvasCollection.EachPageWidth;
            }
        }
        public int EachPageHeight
        {
            get
            {
                return physicalCanvasCollection.EachPageHeight;
            }
        }
        public void Dispose()
        {
            if (physicalCanvasCollection != null)
            {
                physicalCanvasCollection.Dispose();
                physicalCanvasCollection = null;
            }
        }
        public void CanvasInvalidate(Rectangle rect)
        {
            Rect r = Rect.CreateFromRect(rect);
            if (pageA != null && pageA.IntersectsWith(r))
            {
                pageA.Invalidate(r);
            }
            if (pageB != null && pageB.IntersectsWith(r))
            {
                pageB.Invalidate(r);
            }
            if (pageC != null && pageC.IntersectsWith(r))
            {
                pageC.Invalidate(r);
            }
            if (pageD != null && pageD.IntersectsWith(r))
            {
                pageD.Invalidate(r);
            }

        }
        public bool IsValid
        {
            get
            {
                if (pageA != null)
                {
                    if (!pageA.IsContentReady)
                    {
                        return false;
                    }
                    if (pageB != null && !pageB.IsContentReady)
                    {
                        return false;
                    }
                    if (pageC != null && !pageC.IsContentReady)
                    {
                        return false;
                    }
                    if (pageD != null && !pageD.IsContentReady)
                    {
                        return false;
                    }


                }
                return true;
            }
        }

        int render_parts = 0;

        public const int PAGE_A = 0;
        public const int PAGE_AB = 1;
        public const int PAGE_AC = 2;
        public const int PAGE_ABCD = 3;

        public void RenderToOutputWindowFullMode(
        ITopWindowRenderBox rootElement,
        IntPtr destOutputHdc,
        int viewportX, int viewportY, int viewportWidth, int viewportHeight)
        {


            int render_part = PAGE_A;

            if (pageA != null && !pageA.IsContentReady)
            {
                UpdateAllArea(pageA, rootElement);
            }
            if (pageB != null)
            {
                render_part |= PAGE_AB;
                if (!pageB.IsContentReady)
                {
                    UpdateAllArea(pageB, rootElement);
                }
            }
            if (pageC != null)
            {
                render_part |= PAGE_AC;
                if (!pageC.IsContentReady)
                {
                    UpdateAllArea(pageC, rootElement);
                }
            }
            if (pageD != null)
            {
                render_part |= PAGE_ABCD;
                if (!pageD.IsContentReady)
                {
                    UpdateAllArea(pageD, rootElement);
                }
            }


            switch (render_part)
            {
                case PAGE_A:
                    {
                        pageA.RenderTo(destOutputHdc, viewportX - pageA.Left,
                        viewportY - pageA.Top,
                            new Rectangle(0, 0,
                            viewportWidth,
                            viewportHeight));
                    } break;
                case PAGE_AB:
                    {
                        int remainingHeightOfPageA = pageA.Bottom - viewportY;

                        pageA.RenderTo(destOutputHdc,
                            viewportX - pageA.Left, viewportY - pageA.Top,
                            new Rectangle(0, 0, viewportWidth, remainingHeightOfPageA));
                        pageB.RenderTo(destOutputHdc, 0, 0,
                            new Rectangle(0, remainingHeightOfPageA, viewportWidth, viewportHeight - remainingHeightOfPageA));

                    } break;
                case PAGE_AC:
                    {
                        int remainingWidthOfPageA = pageA.Right - viewportX;

                        pageA.RenderTo(destOutputHdc,
                            viewportX - pageA.Left, viewportY - pageA.Top,
                            new Rectangle(0, 0, remainingWidthOfPageA, viewportHeight));
                        pageC.RenderTo(destOutputHdc, 0, 0,
                            new Rectangle(0, remainingWidthOfPageA, viewportWidth - remainingWidthOfPageA, viewportHeight));

                    } break;
                case PAGE_ABCD:
                    {
                        int remainingHeightOfPageA = pageA.Bottom - viewportY;

                        pageA.RenderTo(destOutputHdc,
                            viewportX - pageA.Left, viewportY - pageA.Top,
                            new Rectangle(0, 0, viewportWidth, remainingHeightOfPageA));
                        pageB.RenderTo(destOutputHdc, 0, 0,
                            new Rectangle(0, remainingHeightOfPageA, viewportWidth, viewportHeight - remainingHeightOfPageA));



                    } break;
            }
        }

        static void UpdateAllArea(MyCanvas mycanvas, ITopWindowRenderBox rootElement)
        {

            mycanvas.OffsetCanvasOrigin(-mycanvas.Left, -mycanvas.Top);
            Rect rect = Rect.CreateFromRect(mycanvas.Rect);
            rootElement.DrawToThisPage(mycanvas, rect);

#if DEBUG
            rootElement.dbugShowRenderPart(mycanvas, rect);
#endif
#if DEBUG
#endif

            mycanvas.IsContentReady = true;
            mycanvas.OffsetCanvasOrigin(mycanvas.Left, mycanvas.Top);
        }

        static void UpdateInvalidArea(MyCanvas mycanvas, ITopWindowRenderBox rootElement, IVisualDrawingChain renderingChain)
        {

            renderingChain.UpdateInvalidArea(mycanvas, rootElement);

        }
        static void UpdateInvalidArea(MyCanvas mycanvas, ITopWindowRenderBox rootElement)
        {
#if DEBUG
#endif
            mycanvas.OffsetCanvasOrigin(-mycanvas.Left, -mycanvas.Top);
            Rect rect = mycanvas.InvalidateArea;
            rootElement.DrawToThisPage(mycanvas, rect);
#if DEBUG
            rootElement.dbugShowRenderPart(mycanvas, rect);
#endif

            mycanvas.IsContentReady = true;
            mycanvas.OffsetCanvasOrigin(mycanvas.Left, mycanvas.Top);
        }


        public void RenderToOutputWindowPartialMode(
            ITopWindowRenderBox rootElement,
            IntPtr destOutputHdc,
            int viewportX, int viewportY,
            int viewportWidth, int viewportHeight)
        {

            //temp ****
            IVisualDrawingChain renderChain = null;
            switch (render_parts)
            {
                case PAGE_A:
                    {

                        if (!pageA.IsContentReady)
                        {
                            if (renderChain != null)
                            {
                                UpdateInvalidArea(pageA, rootElement, renderChain);
                            }
                            else
                            {
                                UpdateInvalidArea(pageA, rootElement);
                            }
                        }
                    } break;
                case PAGE_AB:
                    {
                        if (!pageA.IsContentReady)
                        {
                            if (renderChain != null)
                            {
                                UpdateInvalidArea(pageA, rootElement, renderChain);
                            }
                            else
                            {
                                UpdateInvalidArea(pageA, rootElement);
                            }
                        }
                        if (!pageB.IsContentReady)
                        {
                            if (renderChain != null)
                            {
                                UpdateInvalidArea(pageB, rootElement, renderChain);
                            }
                            else
                            {
                                UpdateInvalidArea(pageB, rootElement);
                            }
                        }
                    } break;
                case PAGE_AC:
                    {
                        if (!pageA.IsContentReady)
                        {
                            if (renderChain != null)
                            {
                                UpdateInvalidArea(pageA, rootElement, renderChain);
                            }
                            else
                            {
                                UpdateInvalidArea(pageA, rootElement);

                            }
                        }
                        if (!pageC.IsContentReady)
                        {
                            if (renderChain != null)
                            {
                                UpdateInvalidArea(pageC, rootElement, renderChain);
                            }
                            else
                            {
                                UpdateInvalidArea(pageC, rootElement);
                            }
                        }
                    } break;
                case PAGE_ABCD:
                    {
                        if (!pageA.IsContentReady)
                        {
                            if (renderChain != null)
                            {
                                UpdateInvalidArea(pageA, rootElement, renderChain);
                            }
                            else
                            {
                                UpdateInvalidArea(pageA, rootElement);
                            }
                        }
                        if (!pageB.IsContentReady)
                        {
                            if (renderChain != null)
                            {
                                UpdateInvalidArea(pageB, rootElement, renderChain);
                            }
                            else
                            {
                                UpdateInvalidArea(pageB, rootElement);
                            }
                        }
                        if (!pageC.IsContentReady)
                        {
                            if (renderChain != null)
                            {
                                UpdateInvalidArea(pageC, rootElement, renderChain);
                            }
                            else
                            {
                                UpdateInvalidArea(pageC, rootElement);
                            }
                        }
                        if (!pageD.IsContentReady)
                        {
                            if (renderChain != null)
                            {
                                UpdateInvalidArea(pageD, rootElement, renderChain);
                            }
                            else
                            {
                                UpdateInvalidArea(pageD, rootElement);
                            }
                        }
                    } break;
            }



            switch (render_parts)
            {
                case PAGE_A:
                    {

                        Rect invalidateArea = pageA.InvalidateArea;
                        pageA.RenderTo(destOutputHdc, invalidateArea._left - pageA.Left, invalidateArea._top - pageA.Top,
                            new Rectangle(invalidateArea._left -
                                viewportX, invalidateArea._top - viewportY,
                                invalidateArea.Width, invalidateArea.Height));

                    } break;
                case PAGE_AB:
                    {

                        int remainingHeightOfPageA = pageA.Top + pageA.Height - viewportY;

                        pageA.RenderTo(destOutputHdc, viewportX - pageA.Left, viewportY - pageA.Top,
                            new Rectangle(0, 0, viewportWidth, remainingHeightOfPageA));
                        pageB.RenderTo(destOutputHdc, 0, 0,
                            new Rectangle(0, remainingHeightOfPageA, viewportWidth, viewportHeight - remainingHeightOfPageA));
                    } break;
                case PAGE_AC:
                    {
                    } break;
                case PAGE_ABCD:
                    {
                    } break;

            }
        } 
        public void CalculateCanvasPages(int viewportX, int viewportY, int viewportWidth, int viewportHeight)
        {
            int firstVerticalPageNum = viewportY / physicalCanvasCollection.EachPageHeight;
            int firstHorizontalPageNum = viewportX / physicalCanvasCollection.EachPageWidth;

            render_parts = PAGE_A;

            if (pageA == null)
            {
                pageA = physicalCanvasCollection.GetCanvasPage(firstHorizontalPageNum, firstVerticalPageNum);

            }
            else
            {
                if (!pageA.IsPageNumber(firstHorizontalPageNum, firstVerticalPageNum))
                {
                    physicalCanvasCollection.ReleasePage(pageA);
                    pageA = physicalCanvasCollection.GetCanvasPage(firstHorizontalPageNum, firstVerticalPageNum);
                }
            }

            if (pageA.Right >= viewportX + viewportWidth)
            {
                if (pageC != null)
                {
                    physicalCanvasCollection.ReleasePage(pageC);
                    pageC = null;
                }
            }
            else
            {
                if (pageC == null)
                {
                    pageC = physicalCanvasCollection.GetCanvasPage(firstHorizontalPageNum + 1, firstVerticalPageNum);
                }
                else
                {
                    if (!pageC.IsPageNumber(firstHorizontalPageNum + 1, firstVerticalPageNum))
                    {
                        physicalCanvasCollection.ReleasePage(pageC);
                        pageC = physicalCanvasCollection.GetCanvasPage(firstHorizontalPageNum + 1, firstVerticalPageNum);
                    }
                }
                render_parts |= PAGE_AC;
            }

            if (pageA.Bottom >= viewportY + viewportHeight)
            {
                if (pageB != null)
                {
                    physicalCanvasCollection.ReleasePage(pageB);
                    pageB = null;
                }
            }
            else
            {
                render_parts |= PAGE_AB;
                if (pageB == null)
                {
                    pageB = physicalCanvasCollection.GetCanvasPage(firstHorizontalPageNum, firstVerticalPageNum + 1);
                }
                else
                {
                    if (!pageB.IsPageNumber(firstHorizontalPageNum, firstVerticalPageNum + 1))
                    {
                        physicalCanvasCollection.ReleasePage(pageB);
                        pageB = physicalCanvasCollection.GetCanvasPage(firstHorizontalPageNum, firstVerticalPageNum + 1);
                    }
                }

                if (pageC != null)
                {

                    render_parts = PAGE_ABCD;
                    if (pageD == null)
                    {
                        pageD = physicalCanvasCollection.GetCanvasPage(firstHorizontalPageNum + 1, firstVerticalPageNum + 1);
                    }
                    else
                    {
                        if (!pageD.IsPageNumber(firstHorizontalPageNum + 1, firstVerticalPageNum + 1))
                        {
                            physicalCanvasCollection.ReleasePage(pageD);
                            pageD = physicalCanvasCollection.GetCanvasPage(firstHorizontalPageNum + 1, firstVerticalPageNum + 1);
                        }

                    }
                }
                else
                {
                    if (pageD != null)
                    {
                        physicalCanvasCollection.ReleasePage(pageD);
                        pageD = null;
                    }
                }
            }
        }

        public int RenderParts
        {
            get
            {
                return render_parts;
            }
        }
        public void ResizeAllPages(int newWidth, int newHeight)
        {
            physicalCanvasCollection.Dispose(); physicalCanvasCollection.ResizeAllPages(newWidth, newHeight);
            render_parts = 0;

            if (pageA != null)
            {
                pageA.IsUnused = true;
                pageA = null;
            }
            if (pageB != null)
            {
                pageB.IsUnused = true;
                pageB = null;
            }
            if (pageC != null)
            {
                pageC.IsUnused = true;
                pageC = null;
            }
            if (pageD != null)
            {
                pageD.IsUnused = true;
                pageD = null;
            }


        }

        public void TransferDataFromSourceCanvas(
            Rect logicalArea,
            int viewportX, int viewportY,
            int viewportWidth, int viewportHeight, Canvas destPage)
        {

            Rectangle clipRect = Rectangle.Intersect(
            new Rectangle(0, 0, viewportWidth, viewportHeight),
            destPage.CurrentClipRect);

            switch (render_parts)
            {
                case PAGE_A:
                    {
                        TransferDataFromSourceCanvas(
                            pageA, logicalArea,
                            clipRect,
                            destPage);
                    } break;
                case PAGE_AB:
                    {
                        int remainHeightOfPageA = pageA.Bottom - viewportY;
                        TransferDataFromSourceCanvas(
                            pageA, logicalArea,
                            Rectangle.Intersect(
                                    new Rectangle(0, 0, viewportWidth, remainHeightOfPageA),
                                    clipRect), destPage);

                        TransferDataFromSourceCanvas(
                            pageB, logicalArea,
                            Rectangle.Intersect(
                                    new Rectangle(0, remainHeightOfPageA, viewportWidth, viewportHeight - remainHeightOfPageA),
                                    clipRect), destPage);
                    } break;
                case PAGE_AC:
                    {
                        int remainWidthOfPageA = pageA.Right - viewportX;
                        TransferDataFromSourceCanvas(pageA, logicalArea,
                               Rectangle.Intersect(
                                    new Rectangle(0, 0, remainWidthOfPageA, viewportHeight),
                                    clipRect), destPage);

                        TransferDataFromSourceCanvas(
                            pageC, logicalArea,
                                Rectangle.Intersect(
                                    new Rectangle(remainWidthOfPageA, 0, viewportWidth - remainWidthOfPageA, viewportHeight),
                                    clipRect), destPage);

                    } break;
                case PAGE_ABCD:
                    {
                        int remainHeightOfPageA = pageA.Bottom - viewportY;
                        TransferDataFromSourceCanvas(
                            pageA, logicalArea,
                                Rectangle.Intersect(
                                new Rectangle(0, 0, viewportWidth, remainHeightOfPageA),
                                clipRect), destPage);

                        TransferDataFromSourceCanvas(
                            pageB, logicalArea,
                                                     Rectangle.Intersect(
                               new Rectangle(0, remainHeightOfPageA, viewportWidth, viewportHeight - remainHeightOfPageA),
                               clipRect), destPage);

                        int remainWidthOfPageA = pageA.Right - viewportX;
                        TransferDataFromSourceCanvas(
                            pageC, logicalArea,
                                                     Rectangle.Intersect(
                               new Rectangle(remainWidthOfPageA, 0, viewportWidth - remainWidthOfPageA, viewportHeight),
                               clipRect), destPage);

                        TransferDataFromSourceCanvas(pageD, logicalArea,
                            Rectangle.Intersect(
                                    new Rectangle(remainWidthOfPageA, remainHeightOfPageA,
                                    viewportWidth - remainWidthOfPageA, viewportHeight - remainHeightOfPageA),
                                    clipRect), destPage);


                    } break;
            }


        }
        void TransferDataFromSourceCanvas(
                Canvas sourceCanvas,
                Rect logicalSourceArea, Rectangle physicalUpdateArea, Canvas destPage)
        {

            Rectangle logicalClip = Rectangle.Intersect(logicalSourceArea.ToRectangle(), sourceCanvas.Rect);
            if (logicalClip.Width > 0 && logicalClip.Height > 0)
            {
                destPage.CopyFrom(sourceCanvas, logicalClip.Left,
                    logicalClip.Top,
                    physicalUpdateArea);

            }
        }

#if DEBUG
        void dbug_PrintBitbltInfo(Canvas srcCanvas, string srcCanvasName, int srcX, int srcY, Canvas destCanvas, Rectangle destRect)
        {
            srcX -= srcCanvas.Left;
            srcY -= srcCanvas.Top;
        }
#endif

    }
}