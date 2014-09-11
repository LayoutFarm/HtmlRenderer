//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
namespace LayoutFarm.Presentation
{


#if DEBUG
    [System.Diagnostics.DebuggerDisplay("RenderBoxBase {dbugGetCssBoxInfo}")]
#endif
    public abstract partial class RenderBoxBase : RenderElement
    {

        int myviewportX;
        int myviewportY;

        List<VisualLayer> otherLayers;

        public RenderBoxBase(int width, int height, ElementNature nature)
            : base(width, height, nature)
        {
        }

        public override void CustomDrawToThisPage(CanvasBase canvasPage, InternalRect updateArea)
        {
            //-------------------
            //if (this.IsScrollable && !this.HasDoubleScrollableSurface)
            //{
            canvasPage.OffsetCanvasOrigin(-myviewportX, -myviewportY);
            updateArea.Offset(myviewportX, myviewportY);
            //} 

            this.BoxDrawContent(canvasPage, updateArea);

            //if (this.IsScrollable && !this.HasDoubleScrollableSurface)
            //{
            canvasPage.OffsetCanvasOrigin(myviewportX, myviewportY);
            updateArea.Offset(-myviewportX, -myviewportY);
            //}
            //-------------------
        }
        public void InvalidateContentArrangementFromContainerSizeChanged()
        {
            this.MarkInvalidContentArrangement();
            foreach (VisualLayer layer in this.GetAllLayerBottomToTopIter())
            {
                layer.InvalidateContentArrangementFromContainerSizeChanged();
            }
        }
        protected virtual void BoxDrawContent(CanvasBase canvasPage, InternalRect updateArea)
        {
            //sample ***
            //1. draw background
            RenderElementHelper.DrawBackground(this, canvasPage, updateArea.Width, updateArea.Height, Color.White);
            //2. draw each layer
            
            VisualLayer groundLayer = this.GetGroundLayer();
            if (groundLayer != null)
            {
                groundLayer.DrawChildContent(canvasPage, updateArea);
            }
            if (otherLayers != null)
            {
                int j = otherLayers.Count;
                for (int i = 0; i < j; i++)
                {
#if DEBUG
                    debug_RecordLayerInfo(otherLayers[i]);
#endif
                    otherLayers[i].DrawChildContent(canvasPage, updateArea);
                }
            }

        }
        protected virtual void DrawSubGround(CanvasBase canvasPage, InternalRect updateArea)
        {

        }

        public void PrepareOriginalChildContentDrawingChain(VisualDrawingChain chain)
        {
            if (otherLayers != null)
            {
                int j = otherLayers.Count;
                for (int i = 0; i < j; i++)
                {
                    if (otherLayers[i].PrepareDrawingChain(chain))
                    {
                        return;
                    }
                }
            }
            VisualLayer groundLayer = this.GetGroundLayer();
            if (groundLayer != null)
            {
                groundLayer.PrepareDrawingChain(chain);
            }
        }
        public virtual void ChildrenHitTestCore(HitPointChain artHitResult)
        {

            if (otherLayers != null)
            {
                for (int i = otherLayers.Count - 1; i > -1; --i)
                {
                    if (otherLayers[i].HitTestCore(artHitResult))
                    {
                        return;
                    }
                }
            }
            VisualLayer groundLayer = this.GetGroundLayer();
            if (groundLayer != null)
            {
                groundLayer.HitTestCore(artHitResult);
            }
        }



        protected static void InnerDoTopDownReCalculateContentSize(RenderBoxBase containerBase, VisualElementArgs vinv)
        {
            containerBase.TopDownReCalculateContentSize(vinv);
        }
        protected static void InnerTopDownReArrangeContentIfNeed(RenderBoxBase containerBase, VisualElementArgs vinv)
        {
            containerBase.TopDownReArrangeContentIfNeed(vinv);
        }
        public override sealed void TopDownReCalculateContentSize(VisualElementArgs vinv)
        {

            if (!vinv.ForceReArrange && this.HasCalculatedSize)
            {
                return;
            }
#if DEBUG
            vinv.dbug_EnterTopDownReCalculateContent(this);
#endif
            int cHeight = this.Height;
            int cWidth = this.Width;
            if (otherLayers != null)
            {
                for (int i = otherLayers.Count - 1; i > -1; --i)
                {
                    otherLayers[i].TopDownReCalculateContentSize(vinv);
                }
            }
            VisualLayer groundLayer = this.GetGroundLayer();
            Size ground_contentSize = Size.Empty;
            if (groundLayer != null)
            {
                groundLayer.TopDownReCalculateContentSize(vinv);
                ground_contentSize = groundLayer.PostCalculateContentSize;
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
                case LY_HAS_SPC_HEIGHT:
                    {
                        finalHeight = cHeight;
                    } break;
                case LY_HAS_SPC_WIDTH:
                    {
                        finalWidth = cWidth;
                    } break;
                case LY_HAS_SPC_SIZE:
                    {
                        finalWidth = cWidth;
                        finalHeight = cHeight;
                    } break;
            }


            SetCalculatedDesiredSize(this, finalWidth, finalHeight);
#if DEBUG
            vinv.dbug_ExitTopDownReCalculateContent(this);
#endif

        }



        public void ForceTopDownReArrangeContent(VisualElementArgs vinv)
        {

#if DEBUG
            vinv.dbug_EnterReArrangeContent(this);
            dbug_topDownReArrContentPass++;
            this.dbug_BeginArr++;
            vinv.debug_PushTopDownElement(this);
#endif
            this.MarkValidContentArrangement();
            vinv.IsInTopDownReArrangePhase = true;

            VisualLayer groundLayer = this.GetGroundLayer();
            if (groundLayer != null)
            {
                groundLayer.TopDownReArrangeContent(vinv);
            }
            if (otherLayers != null)
            {
                for (int i = otherLayers.Count - 1; i > -1; --i)
                {
                    otherLayers[i].TopDownReArrangeContent(vinv);
                }
            }

            // BoxEvaluateScrollBar();

#if DEBUG
            this.dbug_FinishArr++;
            vinv.debug_PopTopDownElement(this);
            vinv.dbug_ExitReArrangeContent();
#endif
        }

        public void TopDownReArrangeContentIfNeed(VisualElementArgs vinv)
        {
#if DEBUG
            bool isIncr = false;
#endif

            if (!vinv.ForceReArrange && !this.NeedContentArrangement)
            {
                if (!this.FirstArrangementPass)
                {
                    this.FirstArrangementPass = true;
#if DEBUG
                    vinv.dbug_WriteInfo(dbugVisitorMessage.PASS_FIRST_ARR);
#endif

                }
                else
                {
#if DEBUG
                    isIncr = true;
                    this.dbugVRoot.dbugNotNeedArrCount++;
                    this.dbugVRoot.dbugNotNeedArrCountEpisode++;
                    vinv.dbug_WriteInfo(dbugVisitorMessage.NOT_NEED_ARR);
                    this.dbugVRoot.dbugNotNeedArrCount--;
#endif
                }
                return;
            }

            ForceTopDownReArrangeContent(vinv);


#if DEBUG
            if (isIncr)
            {
                this.dbugVRoot.dbugNotNeedArrCount--;
            }
#endif
        }

        //-------------------------------------------------------------------------------
        public abstract override void ClearAllChildren();
        protected void ClearAllChildrenInOtherLayers()
        {
            if (otherLayers != null)
            {
                for (int i = otherLayers.Count - 1; i > -1; --i)
                {
                    otherLayers[i].Clear();
                }
            }
        }
        protected abstract bool HasGroundLayer();
        protected virtual void GroundLayerAddChild(RenderElement child)
        {

#if DEBUG
            throw new NotSupportedException("item error");
#endif

        }
        public IEnumerable<VisualLayer> GetAllLayerBottomToTopIter()
        {

            if (this.HasGroundLayer())
            {
                yield return GetGroundLayer();
            }
            if (otherLayers != null)
            {
                int j = otherLayers.Count;
                for (int i = 0; i < j; ++i)
                {
                    if (otherLayers[i] != null)
                    {
                        yield return otherLayers[i];
                    }
                }
            }
        }
        protected bool HasOtherLayers()
        {
            return otherLayers != null;
        }
        protected abstract VisualLayer GetGroundLayer();
        //-------------------------------------------------------------------------------

        public override RenderElement FindOverlapedChildElementAtPoint(RenderElement afterThisChild, Point point)
        {
#if DEBUG
            if (afterThisChild.ParentVisualElement != this)
            {
                throw new Exception("not a parent-child relation");
            }
#endif

            if (afterThisChild.ParentLink.MayHasOverlapChild)
            {

                return afterThisChild.ParentLink.FindOverlapedChildElementAtPoint(afterThisChild, point);

            }

            return null;
        }


        public void AddChild(RenderElement child)
        {
#if DEBUG
            if (this == child)
            {
                throw new NotSupportedException();
            }
#endif
            GroundLayerAddChild(child);
        }
        public Size InnerContentSize
        {
            get
            {
                VisualLayer groundLayer = GetGroundLayer();
                if (groundLayer == null)
                {
                    return this.Size;
                }
                else
                {
                    Size s1 = groundLayer.PostCalculateContentSize;
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
            }
        }

        public int ViewportBottom
        {
            get
            {
                return this.Bottom + myviewportY;
            }
        }
        public int ViewportRight
        {
            get
            {
                return this.Right + this.myviewportX;
            }
        }
        public int ViewportY
        {
            get
            {
                return this.myviewportY;
            }
            set
            {
                this.myviewportY = value;
            }
        }
        public int ViewportX
        {
            get
            {

                return this.myviewportX;
            }
            set
            {
                this.myviewportX = value;
            }
        }
        public int ClientTop
        {
            get
            {
                return 0;
            }
        }
        public int ClientLeft
        {
            get
            {
                return 0;
            }
        }

        //--------------------------------------------
#if DEBUG
        public override void dbug_DumpVisualProps(dbugLayoutMsgWriter writer)
        {
            base.dbug_DumpVisualProps(writer);
            writer.EnterNewLevel();



            VisualLayer layer = this.GetGroundLayer();
            if (layer != null)
            {
                layer.dbug_DumpElementProps(writer);
            }

            writer.LeaveCurrentLevel();
        }
        void debug_RecordLayerInfo(VisualLayer layer)
        {
            dbugRootElement visualroot = dbugRootElement.dbugCurrentGlobalVRoot;
            if (visualroot.dbug_RecordDrawingChain)
            {
                visualroot.dbug_AddDrawLayer(layer);
            }
        }
        static int dbug_topDownReArrContentPass = 0;
#endif

    }

}
