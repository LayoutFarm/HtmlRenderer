//Apache2, 2014-2017, WinterDev

namespace LayoutFarm
{
    partial class RenderElement
    {
        public virtual void TopDownReCalculateContentSize()
        {
            MarkHasValidCalculateSize();
        }
        protected static void SetCalculatedSize(RenderBoxBase v, int w, int h)
        {
            v.b_width = w;
            v.b_height = h;
            v.MarkHasValidCalculateSize();
        }
        internal static int GetLayoutSpecificDimensionType(RenderElement visualElement)
        {
            return visualElement.uiLayoutFlags & 0x3;
        }

        public bool HasCalculatedSize
        {
            get
            {
                return ((uiLayoutFlags & RenderElementConst.LAY_HAS_CALCULATED_SIZE) != 0);
            }
        }
        protected void MarkHasValidCalculateSize()
        {
            uiLayoutFlags |= RenderElementConst.LAY_HAS_CALCULATED_SIZE;
#if DEBUG
            this.dbug_ValidateRecalculateSizeEpisode++;
#endif
        }
        public bool IsInLayoutQueue
        {
            get
            {
                return (uiLayoutFlags & RenderElementConst.LY_IN_LAYOUT_QUEUE) != 0;
            }
            set
            {
                uiLayoutFlags = value ?
                      uiLayoutFlags | RenderElementConst.LY_IN_LAYOUT_QUEUE :
                      uiLayoutFlags & ~RenderElementConst.LY_IN_LAYOUT_QUEUE;
            }
        }

        public bool NeedReCalculateContentSize
        {
            get
            {
                return (uiLayoutFlags & RenderElementConst.LAY_HAS_CALCULATED_SIZE) == 0;
            }
        }

        //        internal void MarkInvalidContentArrangement()
        //        {
        //            uiLayoutFlags &= ~RenderElementConst.LY_HAS_ARRANGED_CONTENT;
        //#if DEBUG

        //            this.dbug_InvalidateContentArrEpisode++;
        //            dbug_totalInvalidateContentArrEpisode++;
        //#endif
        //        }
        //        public void MarkInvalidContentSize()
        //        {
        //            uiLayoutFlags &= ~RenderElementConst.LAY_HAS_CALCULATED_SIZE;
        //#if DEBUG
        //            this.dbug_InvalidateRecalculateSizeEpisode++;
        //#endif
        //        }
        public void MarkValidContentArrangement()
        {
#if DEBUG
            this.dbug_ValidateContentArrEpisode++;
#endif

            uiLayoutFlags |= RenderElementConst.LY_HAS_ARRANGED_CONTENT;
        }
        public bool NeedContentArrangement
        {
            get
            {
                return (uiLayoutFlags & RenderElementConst.LY_HAS_ARRANGED_CONTENT) == 0;
            }
        }
        internal bool FirstArrangementPass
        {
            get
            {
                return (propFlags & RenderElementConst.FIRST_ARR_PASS) != 0;
            }
            set
            {
                propFlags = value ?
                   propFlags | RenderElementConst.FIRST_ARR_PASS :
                   propFlags & ~RenderElementConst.FIRST_ARR_PASS;
            }
        }
    }
}