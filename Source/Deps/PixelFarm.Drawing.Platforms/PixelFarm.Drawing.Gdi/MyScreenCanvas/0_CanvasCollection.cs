// 2015,2014 ,Apache2, WinterDev

using System.Collections.Generic;
namespace PixelFarm.Drawing.WinGdi
{
    class CanvasCollection
    {
        List<MyScreenCanvas> cachePages;
        int numOfCachePages;
        int eachPageWidth;
        int eachPageHeight;
        GraphicsPlatform gfxPlatform;
        public CanvasCollection(GraphicsPlatform gfxPlatform, int numOfCachePages, int eachPageWidth, int eachPageHeight)
        {
            this.gfxPlatform = gfxPlatform;
            if (eachPageWidth < 1)
            {
                eachPageWidth = 1;
            }
            if (eachPageHeight < 1)
            {
                eachPageHeight = 1;
            }
            cachePages = new List<MyScreenCanvas>(numOfCachePages);
            this.eachPageWidth = eachPageWidth;
            this.eachPageHeight = eachPageHeight;
            this.numOfCachePages = numOfCachePages;
        }
        public int EachPageWidth
        {
            get
            {
                return eachPageWidth;
            }
        }
        public int EachPageHeight
        {
            get
            {
                return eachPageHeight;
            }
        }
        public void ResizeAllPages(int width, int height)
        {
            if (eachPageWidth != width || EachPageHeight != height)
            {
                this.eachPageWidth = width;
                this.eachPageHeight = height;
                for (int i = cachePages.Count - 1; i > -1; i--)
                {
                    cachePages[i].DimensionInvalid = true;
                }
            }
        }
        public MyScreenCanvas GetCanvasPage(int hPageNum, int vPageNum)
        {
            int j = cachePages.Count;
            for (int i = j - 1; i > -1; i--)
            {
                MyScreenCanvas page = cachePages[i];
                if (page.IsPageNumber(hPageNum, vPageNum))
                {
                    cachePages.RemoveAt(i);
                    if (page.DimensionInvalid)
                    {
                        page.Reset(hPageNum, vPageNum, eachPageWidth, eachPageHeight);
                        page.IsUnused = false;
                    }
                    return page;
                }
            }

            if (j >= numOfCachePages)
            {
                MyScreenCanvas page = cachePages[0];
                cachePages.RemoveAt(0);
                page.IsUnused = false;
                if (page.DimensionInvalid)
                {
                    page.Reset(hPageNum, vPageNum, eachPageWidth, eachPageHeight);
                }
                else
                {
                    page.Reuse(hPageNum, vPageNum);
                }

                Rectangle rect = page.Rect;
                page.Invalidate(rect);
                return page;
            }
            else
            {
                return new MyScreenCanvas(gfxPlatform,
                    hPageNum,
                    vPageNum,
                    hPageNum * eachPageWidth,
                    eachPageHeight * vPageNum,
                    eachPageWidth,
                    eachPageHeight);
            }
        }
        public void ReleasePage(MyScreenCanvas page)
        {
            page.IsUnused = true;
            cachePages.Add(page);
        }
        public void Dispose()
        {
            foreach (MyScreenCanvas canvas in cachePages)
            {
                canvas.CloseCanvas();
            }
            cachePages.Clear();
        }
    }
}