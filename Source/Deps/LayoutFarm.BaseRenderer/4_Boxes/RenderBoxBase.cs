//Apache2, 2014-2017, WinterDev

using PixelFarm.Drawing;
using LayoutFarm.RenderBoxes;
namespace LayoutFarm
{
#if DEBUG
    [System.Diagnostics.DebuggerDisplay("RenderBoxBase {dbugGetCssBoxInfo}")]
#endif
    public abstract class RenderBoxBase : RenderElement
    {
        int myviewportX;
        int myviewportY;
        PlainLayer defaultLayer;
        public RenderBoxBase(RootGraphic rootgfx, int width, int height)
            : base(rootgfx, width, height)
        {
            this.MayHasViewport = true;
            this.MayHasChild = true;
        }


        public bool UseAsFloatWindow { get; set; }
        public override void SetViewport(int viewportX, int viewportY)
        {
            this.myviewportX = viewportX;
            this.myviewportY = viewportY;
            this.InvalidateGraphics();
        }
        public override int ViewportX
        {
            get
            {
                return this.myviewportX;
            }
        }
        public override int ViewportY
        {
            get
            {
                return this.myviewportY;
            }
        }


        public sealed override void CustomDrawToThisCanvas(Canvas canvas, Rectangle updateArea)
        {
            canvas.OffsetCanvasOrigin(-myviewportX, -myviewportY);
            updateArea.Offset(myviewportX, myviewportY);
            this.DrawBoxContent(canvas, updateArea);
            canvas.OffsetCanvasOrigin(myviewportX, myviewportY);
            updateArea.Offset(-myviewportX, -myviewportY);
        }

        public override void ChildrenHitTestCore(HitChain hitChain)
        {
            if (this.defaultLayer != null)
            {
                defaultLayer.HitTestCore(hitChain);
#if DEBUG
                debug_RecordLayerInfo(defaultLayer);
#endif
            }
        }

        public override sealed void TopDownReCalculateContentSize()
        {
            if (!ForceReArrange && this.HasCalculatedSize)
            {
                return;
            }
#if DEBUG
            dbug_EnterTopDownReCalculateContent(this);
#endif
            int cHeight = this.Height;
            int cWidth = this.Width;
            Size ground_contentSize = Size.Empty;
            if (defaultLayer != null)
            {
                defaultLayer.TopDownReCalculateContentSize();
                ground_contentSize = defaultLayer.PostCalculateContentSize;
            }
            int finalWidth = ground_contentSize.Width;
            if (finalWidth == 0)
            {
                finalWidth = this.Width;
            }
            int finalHeight = ground_contentSize.Height;
            if (finalHeight == 0)
            {
                finalHeight = this.Height;
            }
            switch (GetLayoutSpecificDimensionType(this))
            {
                case RenderElementConst.LY_HAS_SPC_HEIGHT:
                    {
                        finalHeight = cHeight;
                    }
                    break;
                case RenderElementConst.LY_HAS_SPC_WIDTH:
                    {
                        finalWidth = cWidth;
                    }
                    break;
                case RenderElementConst.LY_HAS_SPC_SIZE:
                    {
                        finalWidth = cWidth;
                        finalHeight = cHeight;
                    }
                    break;
            }


            SetCalculatedSize(this, finalWidth, finalHeight);
#if DEBUG
            dbug_ExitTopDownReCalculateContent(this);
#endif

        }
        public override void ResetRootGraphics(RootGraphic rootgfx)
        {
            if (this.Root != rootgfx)
            {
                DirectSetRootGraphics(this, rootgfx);
                if (this.defaultLayer != null)
                {
                    foreach (var r in defaultLayer.GetRenderElementIter())
                    {
                        r.ResetRootGraphics(rootgfx);
                    }
                }
            }
        }
        public override void AddChild(RenderElement renderE)
        {
            if (this.defaultLayer == null)
            {
                this.defaultLayer = new PlainLayer(this);
            }
            this.defaultLayer.AddChild(renderE);
        }

        public override void RemoveChild(RenderElement renderE)
        {
            if (this.defaultLayer != null)
            {
                this.defaultLayer.RemoveChild(renderE);
            }
        }
        public override void ClearAllChildren()
        {
            if (this.defaultLayer != null)
            {
                this.defaultLayer.Clear();
            }
        }



        public override RenderElement FindUnderlyingSiblingAtPoint(Point point)
        {
            if (this.MyParentLink != null)
            {
                return this.MyParentLink.FindOverlapedChildElementAtPoint(this, point);
            }

            return null;
        }

        public override Size InnerContentSize
        {
            get
            {
                if (this.defaultLayer != null)
                {
                    Size s1 = defaultLayer.PostCalculateContentSize;
                    if (s1.Width < this.Width)
                    {
                        s1.Width = this.Width;
                    }
                    if (s1.Height < this.Height)
                    {
                        s1.Height = this.Height;
                    }
                    return s1;
                }
                else
                {
                    return this.Size;
                }
            }
        }



        protected abstract void DrawBoxContent(Canvas canvas, Rectangle updateArea);
        protected bool HasDefaultLayer
        {
            get { return this.defaultLayer != null; }
        }
        protected void DrawDefaultLayer(Canvas canvas, ref Rectangle updateArea)
        {
            if (this.defaultLayer != null)
            {
#if DEBUG
                if (!debugBreaK1)
                {
                    debugBreaK1 = true;
                }
#endif
                defaultLayer.DrawChildContent(canvas, updateArea);
            }
        }

#if DEBUG
        public static bool debugBreaK1;
        //-----------------------------------------------------------------
        public void dbugForceTopDownReArrangeContent()
        {
            dbug_EnterReArrangeContent(this);
            dbug_topDownReArrContentPass++;
            this.dbug_BeginArr++;
            debug_PushTopDownElement(this);
            this.MarkValidContentArrangement();
            IsInTopDownReArrangePhase = true;
            if (this.defaultLayer != null)
            {
                this.defaultLayer.TopDownReArrangeContent();
            }

            // BoxEvaluateScrollBar();


            this.dbug_FinishArr++;
            debug_PopTopDownElement(this);
            dbug_ExitReArrangeContent();
        }
        public void dbugTopDownReArrangeContentIfNeed()
        {
            bool isIncr = false;
            if (!ForceReArrange && !this.NeedContentArrangement)
            {
                if (!this.FirstArrangementPass)
                {
                    this.FirstArrangementPass = true;
                    dbug_WriteInfo(dbugVisitorMessage.PASS_FIRST_ARR);
                }
                else
                {
                    isIncr = true;
                    this.dbugVRoot.dbugNotNeedArrCount++;
                    this.dbugVRoot.dbugNotNeedArrCountEpisode++;
                    dbug_WriteInfo(dbugVisitorMessage.NOT_NEED_ARR);
                    this.dbugVRoot.dbugNotNeedArrCount--;
                }
                return;
            }

            dbugForceTopDownReArrangeContent();
            if (isIncr)
            {
                this.dbugVRoot.dbugNotNeedArrCount--;
            }
        }
        public override void dbug_DumpVisualProps(dbugLayoutMsgWriter writer)
        {
            base.dbug_DumpVisualProps(writer);
            writer.EnterNewLevel();
            writer.LeaveCurrentLevel();
        }
        void debug_RecordLayerInfo(RenderElementLayer layer)
        {
            RootGraphic visualroot = RootGraphic.dbugCurrentGlobalVRoot;
            if (visualroot.dbug_RecordDrawingChain)
            {
                visualroot.dbug_AddDrawLayer(layer);
            }
        }
        static int dbug_topDownReArrContentPass = 0;
#endif

    }
}
