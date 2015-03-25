// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;

namespace LayoutFarm.RenderBoxes
{


#if DEBUG
    [System.Diagnostics.DebuggerDisplay("RenderBoxBase {dbugGetCssBoxInfo}")]
#endif
    public abstract class RenderBoxBase : RenderElement
    {

        int myviewportX;
        int myviewportY;

        PlainLayer layer0;
        public RenderBoxBase(RootGraphic rootgfx, int width, int height)
            : base(rootgfx, width, height)
        {
            this.MayHasViewport = true;
            this.MayHasChild = true;
        }
        protected abstract void DrawContent(Canvas canvas, Rectangle updateArea);



        public void SetViewport(int viewportX, int viewportY)
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

            this.DrawContent(canvas, updateArea);

            canvas.OffsetCanvasOrigin(myviewportX, myviewportY);
            updateArea.Offset(-myviewportX, -myviewportY);
        }

        public override void ChildrenHitTestCore(HitChain hitChain)
        {
            if (this.layer0 != null)
            {
                layer0.HitTestCore(hitChain);
#if DEBUG
                debug_RecordLayerInfo(layer0);
#endif
            }
        }

        public void InvalidateContentArrangementFromContainerSizeChanged()
        {
            this.MarkInvalidContentArrangement();
            //foreach (VisualLayer layer in this.GetAllLayerBottomToTopIter())
            //{
            //    layer.InvalidateContentArrangementFromContainerSizeChanged();
            //}
        }
        protected static void InnerDoTopDownReCalculateContentSize(RenderBoxBase containerBase)
        {
            containerBase.TopDownReCalculateContentSize();
        }
        protected static void InnerTopDownReArrangeContentIfNeed(RenderBoxBase containerBase)
        {
            containerBase.TopDownReArrangeContentIfNeed();
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
            if (layer0 != null)
            {
                layer0.TopDownReCalculateContentSize();
                ground_contentSize = layer0.PostCalculateContentSize;

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
                    } break;
                case RenderElementConst.LY_HAS_SPC_WIDTH:
                    {
                        finalWidth = cWidth;
                    } break;
                case RenderElementConst.LY_HAS_SPC_SIZE:
                    {
                        finalWidth = cWidth;
                        finalHeight = cHeight;
                    } break;
            }


            SetCalculatedDesiredSize(this, finalWidth, finalHeight);
#if DEBUG
            dbug_ExitTopDownReCalculateContent(this);
#endif

        }

        protected bool HasDefaultLayer
        {
            get { return this.layer0 != null; }
        }
        protected void DrawDefaultLayer(Canvas canvas, ref Rectangle updateArea)
        {
            if (this.layer0 != null)
            {
                layer0.DrawChildContent(canvas, updateArea);
            }
        }
        public PlainLayer GetDefaultLayer()
        {
            if (this.layer0 == null)
            {
                return this.layer0 = new PlainLayer(this);
            }
            return this.layer0;
        }
        public virtual void AddChild(RenderElement renderE)
        {
            if (this.layer0 == null)
            {
                this.layer0 = new PlainLayer(this);
            }
            this.layer0.AddChild(renderE);
        }
        public virtual void RemoveChild(RenderElement renderE)
        {
            if (this.layer0 != null)
            {
                this.layer0.RemoveChild(renderE);
            }
        }
        //-------------------------------------------------------------------------- 
        public override void ClearAllChildren()
        {
            if (this.layer0 != null)
            {
                this.layer0.Clear();
            }
        }
        //-----------------------------------------------------------------
        public void ForceTopDownReArrangeContent()
        {

#if DEBUG
            dbug_EnterReArrangeContent(this);
            dbug_topDownReArrContentPass++;
            this.dbug_BeginArr++;
            debug_PushTopDownElement(this);
#endif

            this.MarkValidContentArrangement();

            IsInTopDownReArrangePhase = true;

            if (this.layer0 != null)
            {
                this.layer0.TopDownReArrangeContent();
            }

            // BoxEvaluateScrollBar();

#if DEBUG
            this.dbug_FinishArr++;
            debug_PopTopDownElement(this);
            dbug_ExitReArrangeContent();
#endif
        }
        public void TopDownReArrangeContentIfNeed()
        {
#if DEBUG
            bool isIncr = false;
#endif

            if (!ForceReArrange && !this.NeedContentArrangement)
            {
                if (!this.FirstArrangementPass)
                {
                    this.FirstArrangementPass = true;
#if DEBUG
                    dbug_WriteInfo(dbugVisitorMessage.PASS_FIRST_ARR);
#endif

                }
                else
                {
#if DEBUG
                    isIncr = true;
                    this.dbugVRoot.dbugNotNeedArrCount++;
                    this.dbugVRoot.dbugNotNeedArrCountEpisode++;
                    dbug_WriteInfo(dbugVisitorMessage.NOT_NEED_ARR);
                    this.dbugVRoot.dbugNotNeedArrCount--;
#endif
                }
                return;
            }

            ForceTopDownReArrangeContent();


#if DEBUG
            if (isIncr)
            {
                this.dbugVRoot.dbugNotNeedArrCount--;
            }
#endif
        }

        public override RenderElement FindOverlapedChildElementAtPoint(RenderElement afterThisChild, Point point)
        {
#if DEBUG
            if (afterThisChild.dbugParentVisualElement != this)
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
        public Size InnerContentSize
        {
            get
            {
                if (this.layer0 != null)
                {
                    Size s1 = layer0.PostCalculateContentSize;
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


        //--------------------------------------------
#if DEBUG
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
