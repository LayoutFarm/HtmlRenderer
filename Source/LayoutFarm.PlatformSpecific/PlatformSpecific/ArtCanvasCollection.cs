//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
 
namespace LayoutFarm.Presentation
{

    class ArtCanvasCollection
    {
        List<ArtCanvas> cachePages;
        int numOfCachePages;
        int eachPageWidth;
        int eachPageHeight;

        public ArtCanvasCollection(int numOfCachePages, int eachPageWidth, int eachPageHeight)
        {
            if (eachPageWidth < 1)
            {
                eachPageWidth = 1;
            }
            if (eachPageHeight < 1)
            {
                eachPageHeight = 1;
            }
            cachePages = new List<ArtCanvas>(numOfCachePages);
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
        public ArtCanvas GetCanvasPage(int hPageNum, int vPageNum)
        {
            int j = cachePages.Count; for (int i = j - 1; i > -1; i--)
            {
                ArtCanvas page = cachePages[i];
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
                ArtCanvas page = cachePages[0];
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

                InternalRect rect = InternalRect.CreateFromRect(page.Rect);
                page.Invalidate(rect); InternalRect.FreeInternalRect(rect);
                return page;
            }
            else
            {
                return new ArtCanvasImpl(hPageNum, vPageNum, hPageNum * eachPageWidth, eachPageHeight * vPageNum, eachPageWidth, eachPageHeight);

            }
        }
        public void ReleasePage(ArtCanvas page)
        {
            page.IsUnused = true;
            cachePages.Add(page);
        }
        public void Dispose()
        {
            foreach (ArtCanvas canvasPage in cachePages)
            {
                canvasPage.ReleaseUnManagedResource();
            }
            cachePages.Clear();
        }

    }

}