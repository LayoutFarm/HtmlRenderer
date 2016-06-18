// 2015,2014 ,Apache2, WinterDev

using System;
namespace PixelFarm.Drawing.WinGdi
{
    public class QuadPages
    {
        internal MyScreenCanvas pageA;
        internal MyScreenCanvas pageB;
        internal MyScreenCanvas pageC;
        internal MyScreenCanvas pageD;
        CanvasCollection physicalCanvasCollection;
        public QuadPages(GraphicsPlatform gfxPlatform,
            int cachedPageNum,
            int eachCachedPageWidth,
            int eachCachedPageHeight)
        {
            physicalCanvasCollection = new CanvasCollection(
                gfxPlatform,
                cachedPageNum, eachCachedPageWidth, eachCachedPageHeight);
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
            Rectangle r = rect;
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
        internal const int PAGE_A = 0;
        internal const int PAGE_AB = 1;
        internal const int PAGE_AC = 2;
        internal const int PAGE_ABCD = 3;
        public void RenderToOutputWindowFullMode(
            IRenderElement topWindowRenderBox,
            IntPtr destOutputHdc,
            int viewportX, int viewportY, int viewportWidth, int viewportHeight)
        {
            int render_part = PAGE_A;
            if (pageA != null && !pageA.IsContentReady)
            {
                UpdateAllArea(pageA, topWindowRenderBox);
            }
            if (pageB != null)
            {
                render_part |= PAGE_AB;
                if (!pageB.IsContentReady)
                {
                    UpdateAllArea(pageB, topWindowRenderBox);
                }
            }
            if (pageC != null)
            {
                render_part |= PAGE_AC;
                if (!pageC.IsContentReady)
                {
                    UpdateAllArea(pageC, topWindowRenderBox);
                }
            }
            if (pageD != null)
            {
                render_part |= PAGE_ABCD;
                if (!pageD.IsContentReady)
                {
                    UpdateAllArea(pageD, topWindowRenderBox);
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
                    }
                    break;
                case PAGE_AB:
                    {
                        int remainingHeightOfPageA = pageA.Bottom - viewportY;
                        pageA.RenderTo(destOutputHdc,
                            viewportX - pageA.Left, viewportY - pageA.Top,
                            new Rectangle(0, 0, viewportWidth, remainingHeightOfPageA));
                        pageB.RenderTo(destOutputHdc, 0, 0,
                            new Rectangle(0, remainingHeightOfPageA, viewportWidth, viewportHeight - remainingHeightOfPageA));
                    }
                    break;
                case PAGE_AC:
                    {
                        int remainingWidthOfPageA = pageA.Right - viewportX;
                        pageA.RenderTo(destOutputHdc,
                            viewportX - pageA.Left, viewportY - pageA.Top,
                            new Rectangle(0, 0, remainingWidthOfPageA, viewportHeight));
                        pageC.RenderTo(destOutputHdc, 0, 0,
                            new Rectangle(0, remainingWidthOfPageA, viewportWidth - remainingWidthOfPageA, viewportHeight));
                    }
                    break;
                case PAGE_ABCD:
                    {
                        int remainingHeightOfPageA = pageA.Bottom - viewportY;
                        pageA.RenderTo(destOutputHdc,
                            viewportX - pageA.Left, viewportY - pageA.Top,
                            new Rectangle(0, 0, viewportWidth, remainingHeightOfPageA));
                        pageB.RenderTo(destOutputHdc, 0, 0,
                            new Rectangle(0, remainingHeightOfPageA, viewportWidth, viewportHeight - remainingHeightOfPageA));
                    }
                    break;
            }
        }

        static void UpdateAllArea(MyScreenCanvas mycanvas, IRenderElement topWindowRenderBox)
        {
            mycanvas.OffsetCanvasOrigin(-mycanvas.Left, -mycanvas.Top);
            Rectangle rect = mycanvas.Rect;
            topWindowRenderBox.DrawToThisCanvas(mycanvas, rect);
#if DEBUG
            topWindowRenderBox.dbugShowRenderPart(mycanvas, rect);
#endif

            mycanvas.IsContentReady = true;
            mycanvas.OffsetCanvasOrigin(mycanvas.Left, mycanvas.Top);
        }


        static void UpdateInvalidArea(MyScreenCanvas mycanvas, IRenderElement rootElement)
        {
            mycanvas.OffsetCanvasOrigin(-mycanvas.Left, -mycanvas.Top);
            Rectangle rect = mycanvas.InvalidateArea;
            rootElement.DrawToThisCanvas(mycanvas, rect);
#if DEBUG
            rootElement.dbugShowRenderPart(mycanvas, rect);
#endif

            mycanvas.IsContentReady = true;
            mycanvas.OffsetCanvasOrigin(mycanvas.Left, mycanvas.Top);
        }


        public void RenderToOutputWindowPartialMode(
            IRenderElement renderE,
            IntPtr destOutputHdc,
            int viewportX, int viewportY,
            int viewportWidth, int viewportHeight)
        {
            switch (render_parts)
            {
                case PAGE_A:
                    {
                        if (!pageA.IsContentReady)
                        {
                            UpdateInvalidArea(pageA, renderE);
                        }
                    }
                    break;
                case PAGE_AB:
                    {
                        if (!pageA.IsContentReady)
                        {
                            UpdateInvalidArea(pageA, renderE);
                        }
                        if (!pageB.IsContentReady)
                        {
                            UpdateInvalidArea(pageB, renderE);
                        }
                    }
                    break;
                case PAGE_AC:
                    {
                        if (!pageA.IsContentReady)
                        {
                            UpdateInvalidArea(pageA, renderE);
                        }
                        if (!pageC.IsContentReady)
                        {
                            UpdateInvalidArea(pageC, renderE);
                        }
                    }
                    break;
                case PAGE_ABCD:
                    {
                        if (!pageA.IsContentReady)
                        {
                            UpdateInvalidArea(pageA, renderE);
                        }
                        if (!pageB.IsContentReady)
                        {
                            UpdateInvalidArea(pageB, renderE);
                        }
                        if (!pageC.IsContentReady)
                        {
                            UpdateInvalidArea(pageC, renderE);
                        }
                        if (!pageD.IsContentReady)
                        {
                            UpdateInvalidArea(pageD, renderE);
                        }
                    }
                    break;
            }
            //----------------------------------------------------------------------------------------------------------
            switch (render_parts)
            {
                case PAGE_A:
                    {
                        Rectangle invalidateArea = pageA.InvalidateArea;
                        ////Console.WriteLine("update2:" + invalidateArea.ToString());


                        pageA.RenderTo(destOutputHdc, invalidateArea.Left - pageA.Left, invalidateArea.Top - pageA.Top,
                            new Rectangle(invalidateArea.Left -
                                viewportX, invalidateArea.Top - viewportY,
                                invalidateArea.Width, invalidateArea.Height));
                        pageA.ResetInvalidateArea();
                    }
                    break;
                case PAGE_AB:
                    {
                        int remainingHeightOfPageA = pageA.Top + pageA.Height - viewportY;
                        pageA.RenderTo(destOutputHdc, viewportX - pageA.Left, viewportY - pageA.Top,
                            new Rectangle(0, 0, viewportWidth, remainingHeightOfPageA));
                        pageB.RenderTo(destOutputHdc, 0, 0,
                            new Rectangle(0, remainingHeightOfPageA, viewportWidth, viewportHeight - remainingHeightOfPageA));
                    }
                    break;
                case PAGE_AC:
                    {
                    }
                    break;
                case PAGE_ABCD:
                    {
                    }
                    break;
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
    }
}