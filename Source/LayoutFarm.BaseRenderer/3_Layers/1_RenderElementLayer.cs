//Apache2, 2014-2017, WinterDev

using System.Collections.Generic;
using PixelFarm.Drawing;
namespace LayoutFarm.RenderBoxes
{
    public abstract class RenderElementLayer
    {
#if DEBUG
        public int dbug_layer_id;
        static int dbug_layer_id_count = 0;
        public int dbug_InvalidateCount = 0;
        public int dbug_ValidateCount = 0;
#endif

        protected int layerFlags;
        protected const int IS_LAYER_HIDDEN = 1 << (14 - 1);
        protected const int IS_GROUND_LAYER = 1 << (15 - 1);
        protected const int MAY_HAS_OTHER_OVERLAP_CHILD = 1 << (16 - 1);
        protected const int DOUBLE_BACKCANVAS_WIDTH = 1 << (18 - 1);
        protected const int DOUBLE_BACKCANVAS_HEIGHT = 1 << (19 - 1);
        protected const int CONTENT_DRAWING = 1 << (22 - 1);
        protected const int ARRANGEMENT_VALID = 1 << (23 - 1);
        protected const int HAS_CALCULATE_SIZE = 1 << (24 - 1);
        protected const int FLOWLAYER_HAS_MULTILINE = 1 << (25 - 1);
        protected RenderElement owner;
        int postCalculateContentWidth;
        int postCalculateContentHeight;
        public RenderElementLayer(RenderElement owner)
        {
            this.owner = owner;
#if DEBUG
            this.dbug_layer_id = dbug_layer_id_count;
            ++dbug_layer_id_count;
#endif
        }

        public RootGraphic Root
        {
            get { return this.owner.Root; }
        }
        public abstract void Clear();
        public bool Visible
        {
            get
            {
                return (layerFlags & IS_LAYER_HIDDEN) == 0;
            }
            set
            {
                layerFlags = value ?
                    layerFlags & ~IS_LAYER_HIDDEN :
                    layerFlags | IS_LAYER_HIDDEN;
            }
        }

        public Size PostCalculateContentSize
        {
            get
            {
                return new Size(postCalculateContentWidth, postCalculateContentHeight);
            }
        }
        protected void OwnerInvalidateGraphic()
        {
            if (this.owner != null)
            {
                this.owner.InvalidateGraphics();
            }
        }


        protected void BeginDrawingChildContent()
        {
            layerFlags |= CONTENT_DRAWING;
        }
        protected void FinishDrawingChildContent()
        {
            layerFlags &= ~CONTENT_DRAWING;
        }

        public bool DoubleBackCanvasWidth
        {
            get
            {
                return (layerFlags & DOUBLE_BACKCANVAS_WIDTH) != 0;
            }
            set
            {
                layerFlags = value ?
                    layerFlags | DOUBLE_BACKCANVAS_WIDTH :
                    layerFlags & ~DOUBLE_BACKCANVAS_WIDTH;
            }
        }

        public bool DoubleBackCanvasHeight
        {
            get
            {
                return (layerFlags & DOUBLE_BACKCANVAS_HEIGHT) != 0;
            }
            set
            {
                layerFlags = value ?
                    layerFlags | DOUBLE_BACKCANVAS_HEIGHT :
                    layerFlags & ~DOUBLE_BACKCANVAS_HEIGHT;
            }
        }
        protected void SetDoubleCanvas(bool useWithWidth, bool useWithHeight)
        {
            DoubleBackCanvasWidth = useWithWidth;
            DoubleBackCanvasHeight = useWithHeight;
        }
        protected void SetPostCalculateLayerContentSize(int width, int height)
        {
            ValidateCalculateContentSize();
            postCalculateContentWidth = width;
            postCalculateContentHeight = height;
        }

        protected void SetPostCalculateLayerContentSize(Size s)
        {
            ValidateCalculateContentSize();
            postCalculateContentWidth = s.Width;
            postCalculateContentHeight = s.Height;
        }

        public abstract bool HitTestCore(HitChain hitChain);
        public abstract void TopDownReCalculateContentSize();
        public abstract void TopDownReArrangeContent();
        public abstract IEnumerable<RenderElement> GetRenderElementIter();
        public abstract IEnumerable<RenderElement> GetRenderElementReverseIter();
        public abstract void DrawChildContent(Canvas canvasPage, Rectangle updateArea);
        protected void ValidateArrangement()
        {
#if DEBUG
            this.dbug_ValidateCount++;
#endif

            layerFlags |= ARRANGEMENT_VALID;
        }

        bool NeedReArrangeContent
        {
            get
            {
                return (layerFlags & ARRANGEMENT_VALID) == 0;
            }
        }
        void ValidateCalculateContentSize()
        {
            this.layerFlags |= HAS_CALCULATE_SIZE;
        }

#if DEBUG
        public RootGraphic dbugVRoot
        {
            get
            {
                return LayoutFarm.RootGraphic.dbugCurrentGlobalVRoot;
            }
        }
        public string dbugLayerState
        {
            get
            {
                if (NeedReArrangeContent)
                {
                    return "[A]";
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        public abstract void dbug_DumpElementProps(dbugLayoutMsgWriter writer);
        static dbugVisualLayoutTracer dbugGetLayoutTracer()
        {
            RootGraphic visualroot = RootGraphic.dbugCurrentGlobalVRoot;
            if (visualroot == null || !visualroot.dbug_IsRecordLayoutTraceEnable)
            {
                return null;
            }
            else
            {
                return visualroot.dbug_GetLastestVisualLayoutTracer();
            }
        }
        protected static void vinv_dbug_EnterLayerReCalculateContent(RenderElementLayer layer)
        {
            var debugVisualLay = dbugGetLayoutTracer();
            if (debugVisualLay == null) return;
            debugVisualLay.PushLayerElement(layer);
            debugVisualLay.WriteInfo("..>L_RECAL_TOPDOWN :" + layer.ToString());
        }
        protected static void vinv_dbug_ExitLayerReCalculateContent()
        {
            var debugVisualLay = dbugGetLayoutTracer();
            if (debugVisualLay == null) return;
            RenderElementLayer layer = (RenderElementLayer)debugVisualLay.PeekElement();
            debugVisualLay.WriteInfo("<..L_RECAL_TOPDOWN  :" + layer.ToString());
            debugVisualLay.PopLayerElement();
        }
        protected static void vinv_dbug_BeginSetElementBound(RenderElement ve)
        {
            var debugVisualLay = dbugGetLayoutTracer();
            if (debugVisualLay == null) return;
            debugVisualLay.BeginNewContext();
            debugVisualLay.WriteInfo(dbugVisitorMessage.WITH_0.text, ve);
        }
        protected static void vinv_dbug_EndSetElementBound(RenderElement ve)
        {
            var debugVisualLay = dbugGetLayoutTracer();
            if (debugVisualLay == null) return;
            debugVisualLay.WriteInfo(dbugVisitorMessage.WITH_1.text, ve);
            debugVisualLay.EndCurrentContext();
        }
        protected static void vinv_dbug_EnterLayerReArrangeContent(RenderElementLayer layer)
        {
            var debugVisualLay = dbugGetLayoutTracer();
            if (debugVisualLay == null) return;
            debugVisualLay.PushLayerElement(layer);
            debugVisualLay.WriteInfo("..>LAYER_ARR :" + layer.ToString());
        }
        protected static void vinv_dbug_ExitLayerReArrangeContent()
        {
            var debugVisualLay = dbugGetLayoutTracer();
            if (debugVisualLay == null) return;
            RenderElementLayer layer = (RenderElementLayer)debugVisualLay.PeekElement();
            debugVisualLay.WriteInfo("<..LAYER_ARR :" + layer.ToString());
            debugVisualLay.PopLayerElement();
        }
        protected static void vinv_dbug_WriteInfo(dbugVisitorMessage msg, object o)
        {
            var debugVisualLay = dbugGetLayoutTracer();
            if (debugVisualLay == null) return;
            debugVisualLay.WriteInfo(msg.text);
        }
#endif
        public RenderElement OwnerRenderElement
        {
            get { return this.owner; }
        }
    }
}