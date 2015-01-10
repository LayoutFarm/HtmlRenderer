// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;

namespace LayoutFarm.RenderBoxes
{

    public class VisualDrawingChain : IVisualDrawingChain
    {
        public List<RenderElement> selectedVisualElements = new List<RenderElement>();
        public List<bool> containAllAreaTestResults = new List<bool>();


        Rectangle currentClipRect;

        int useCount = 0;
        public VisualDrawingChain(Rectangle flushRect)
        {
            currentClipRect = flushRect;
            useCount = 1;
        }
        public void ResetFlushRect(Rectangle flushRect)
        {
            useCount = 1;
            currentClipRect = flushRect;
        }
        public int UpdateAreaX
        {
            get
            {
                return this.currentClipRect.X;
            }

        }
        public int UpdateAreaY
        {
            get
            {
                return this.currentClipRect.Y;
            }
        }
        public int UpdateAreaRight
        {
            get
            {
                return this.currentClipRect.Right;
            }
        }
        public int UpdateAreaBottom
        {
            get
            {
                return this.currentClipRect.Bottom;
            }
        }
        public Rectangle CurrentClipRect
        {
            get
            {
                return currentClipRect;
            }
        }
        public void AddVisualElement(RenderElement ve, bool containAllArea)
        {

            ve.IsInRenderChain = true;
            selectedVisualElements.Add(ve);
            containAllAreaTestResults.Add(containAllArea);
        }

        public void OffsetCanvasOrigin(int dx, int dy)
        {
            currentClipRect.Offset(-dx, -dy);
        }

        public int UsedCount
        {
            get
            {
                return this.useCount;
            }
            set
            {
                this.useCount = value;
            }
        }
        public void ClearForReuse()
        {

            useCount = 0;
            for (int i = selectedVisualElements.Count - 1; i > -1; --i)
            {
                selectedVisualElements[i].IsInRenderChain = false;
            }
            this.selectedVisualElements.Clear();
            containAllAreaTestResults.Clear();
        }
        public void OffsetCanvasOriginX(int dx)
        {
            currentClipRect.Offset(-dx, 0);
        }
        public void OffsetCanvasOriginY(int dy)
        {
            currentClipRect.Offset(0, -dy);
        }

        void IVisualDrawingChain.UpdateInvalidArea(Canvas mycanvas, ITopWindowRenderBox rootElement)
        {
            List<RenderElement> selectedVisualElements = this.selectedVisualElements;
            List<bool> containAllAreaTestResults = this.containAllAreaTestResults;


            int j = containAllAreaTestResults.Count;

            mycanvas.OffsetCanvasOrigin(-mycanvas.Left, -mycanvas.Top);
            Rectangle rect = mycanvas.InvalidateArea;

            for (int i = j - 1; i > -1; --i)
            {

                if (containAllAreaTestResults[i])
                {
                    RenderElement ve = selectedVisualElements[i];
                    if (!ve.IsInRenderChain)
                    {
                        continue;
                    }
                    if (!ve.HasSolidBackground)
                    {
                        continue;
                    }

                    Point globalLocation = ve.GetGlobalLocation();

                    mycanvas.OffsetCanvasOrigin(globalLocation.X, globalLocation.Y);

                    rect.Offset(-globalLocation.X, -globalLocation.Y);

                    ve.DrawToThisCanvas(mycanvas, rect);
#if DEBUG
                    rootElement.dbugShowRenderPart(mycanvas, rect);
#endif

#if DEBUG

#endif
                    mycanvas.IsContentReady = true;
                    rect.Offset(globalLocation.X, globalLocation.Y);
                    mycanvas.OffsetCanvasOrigin(-globalLocation.X, -globalLocation.Y);

                    ve.IsInRenderChain = false;

                    break;
                }
            }
            mycanvas.OffsetCanvasOrigin(mycanvas.Left, mycanvas.Top);
            for (int i = j - 1; i > -1; --i)
            {
                selectedVisualElements[i].IsInRenderChain = true;
            }
        }
    }
}