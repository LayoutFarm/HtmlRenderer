//Apache2, 2014-2017, WinterDev

using System;
using PixelFarm.Drawing;
using LayoutFarm.RenderBoxes;
namespace LayoutFarm
{
    
    public abstract partial class RenderElement : IRenderElement
    {
        RootGraphic rootGfx;
        IParentLink parentLink;
        object controller;
        int propFlags;
        public RenderElement(RootGraphic rootGfx, int width, int height)
        {
            this.b_width = width;
            this.b_height = height;
            this.rootGfx = rootGfx;
#if DEBUG
            dbug_totalObjectId++;
            dbug_obj_id = dbug_totalObjectId;
            //if (dbug_obj_id == 57)
            //{

            //}
            //if(this.dbug_obj_id ==6)
            //{
            //}
            //this.dbug_SetFixedElementCode(this.GetType().Name);
#endif
        }

        public abstract void ResetRootGraphics(RootGraphic rootgfx);
        protected static void DirectSetRootGraphics(RenderElement r, RootGraphic rootgfx)
        {
            r.rootGfx = rootgfx;
        }
        public RootGraphic Root
        {
            get { return this.rootGfx; }
        }

        public RenderElement GetTopWindowRenderBox()
        {
            if (parentLink == null) { return null; }

            return this.rootGfx.TopWindowRenderBox;
        }

        //==============================================================
        //controller-listener
        public object GetController()
        {
            return controller;
        }
        public void SetController(object controller)
        {
            this.controller = controller;
        }
        public bool TransparentForAllEvents
        {
            get
            {
                return (propFlags & RenderElementConst.TRANSPARENT_FOR_ALL_EVENTS) != 0;
            }
            set
            {
                propFlags = value ?
                       propFlags | RenderElementConst.TRANSPARENT_FOR_ALL_EVENTS :
                       propFlags & ~RenderElementConst.TRANSPARENT_FOR_ALL_EVENTS;
            }
        }

        //==============================================================
        //parent/child ...
        public bool HasParent
        {
            get
            {
                return this.parentLink != null;
            }
        }
        public virtual void ClearAllChildren()
        {
        }
        public virtual void AddChild(RenderElement renderE)
        {
        }
        public virtual void RemoveChild(RenderElement renderE)
        {
        }

        protected bool HasParentLink
        {
            get { return this.parentLink != null; }
        }
        public RenderElement ParentRenderElement
        {
            get
            {
                if (parentLink == null)
                {
                    return null;
                }
                return parentLink.ParentRenderElement;
            }
        }

        public static void RemoveParentLink(RenderElement childElement)
        {
            childElement.parentLink = null;
        }
        public static void SetParentLink(RenderElement childElement, IParentLink parentLink)
        {
            childElement.parentLink = parentLink;
#if DEBUG
            if (childElement.ParentRenderElement == childElement)
            {
                //error!
                throw new NotSupportedException();
            }
#endif
        }
        public bool MayHasChild
        {
            get { return (propFlags & RenderElementConst.MAY_HAS_CHILD) != 0; }
            protected set
            {
                propFlags = value ?
                      propFlags | RenderElementConst.MAY_HAS_CHILD :
                      propFlags & ~RenderElementConst.MAY_HAS_CHILD;
            }
        }
        public bool MayHasViewport
        {
            get { return (propFlags & RenderElementConst.MAY_HAS_VIEWPORT) != 0; }
            protected set
            {
                propFlags = value ?
                      propFlags | RenderElementConst.MAY_HAS_VIEWPORT :
                      propFlags & ~RenderElementConst.MAY_HAS_VIEWPORT;
            }
        }
        public virtual RenderElement FindUnderlyingSiblingAtPoint(Point point)
        {
            return null;
        }

        public virtual void ChildrenHitTestCore(HitChain hitChain)
        {
        }
        //==============================================================
        public bool Visible
        {
            get
            {
                return ((propFlags & RenderElementConst.HIDDEN) == 0);
            }
        }
        public void SetVisible(bool value)
        {
            //check if visible change? 
            if (this.Visible != value)
            {
                propFlags = value ?
                    propFlags & ~RenderElementConst.HIDDEN :
                    propFlags | RenderElementConst.HIDDEN;
                if (parentLink != null)
                {
                    this.InvalidateGraphicBounds(this.RectBounds);
                }
            }
        }

        public bool IsBlockElement
        {
            get
            {
                return ((propFlags & RenderElementConst.IS_BLOCK_ELEMENT) == RenderElementConst.IS_BLOCK_ELEMENT);
            }
            set
            {
                propFlags = value ?
                     propFlags | RenderElementConst.IS_BLOCK_ELEMENT :
                     propFlags & ~RenderElementConst.IS_BLOCK_ELEMENT;
            }
        }

        public bool IsTopWindow
        {
            get
            {
                return (this.propFlags & RenderElementConst.IS_TOP_RENDERBOX) != 0;
            }
            set
            {
                propFlags = value ?
                      propFlags | RenderElementConst.IS_TOP_RENDERBOX :
                      propFlags & ~RenderElementConst.IS_TOP_RENDERBOX;
            }
        }

        internal bool HasDoubleScrollableSurface
        {
            get
            {
                return (this.propFlags & RenderElementConst.HAS_DOUBLE_SCROLL_SURFACE) != 0;
            }
            set
            {
                propFlags = value ?
                      propFlags | RenderElementConst.HAS_DOUBLE_SCROLL_SURFACE :
                      propFlags & ~RenderElementConst.HAS_DOUBLE_SCROLL_SURFACE;
            }
        }

        internal bool HasSolidBackground
        {
            get
            {
                return (propFlags & RenderElementConst.HAS_TRANSPARENT_BG) != 0;
            }
            set
            {
                propFlags = value ?
                       propFlags | RenderElementConst.HAS_TRANSPARENT_BG :
                       propFlags & ~RenderElementConst.HAS_TRANSPARENT_BG;
            }
        }

        public bool VisibleAndHasParent
        {
            get { return ((this.propFlags & RenderElementConst.HIDDEN) == 0) && (this.parentLink != null); }
        }

        //==============================================================
        //hit test

        public bool HitTestCore(HitChain hitChain)
        {
            if ((propFlags & RenderElementConst.HIDDEN) != 0)
            {
                return false;
            }

            int testX;
            int testY;
            hitChain.GetTestPoint(out testX, out testY);
            if ((testY >= b_top && testY <= (b_top + b_height)
            && (testX >= b_left && testX <= (b_left + b_width))))
            {
                if (this.MayHasViewport)
                {
                    hitChain.OffsetTestPoint(
                        -b_left + this.ViewportX,
                        -b_top + this.ViewportY);
                }
                else
                {
                    hitChain.OffsetTestPoint(-b_left, -b_top);
                }

                hitChain.AddHitObject(this);
                if (this.MayHasChild)
                {
                    this.ChildrenHitTestCore(hitChain);
                }

                if (this.MayHasViewport)
                {
                    hitChain.OffsetTestPoint(
                            b_left - this.ViewportX,
                            b_top - this.ViewportY);
                }
                else
                {
                    hitChain.OffsetTestPoint(b_left, b_top);
                }

                if ((propFlags & RenderElementConst.TRANSPARENT_FOR_ALL_EVENTS) != 0 &&
                    hitChain.TopMostElement == this)
                {
                    hitChain.RemoveCurrentHit();
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        //==============================================================
        //render...
        public abstract void CustomDrawToThisCanvas(Canvas canvas, Rectangle updateArea);
        public void DrawToThisCanvas(Canvas canvas, Rectangle updateArea)
        {
            if ((propFlags & RenderElementConst.HIDDEN) == RenderElementConst.HIDDEN)
            {
                return;
            }
#if DEBUG
            dbugVRoot.dbug_drawLevel++;
#endif

            if (canvas.PushClipAreaRect(b_width, b_height, ref updateArea))
            {
#if DEBUG
                if (dbugVRoot.dbug_RecordDrawingChain)
                {
                    dbugVRoot.dbug_AddDrawElement(this, canvas);
                }
#endif
                //------------------------------------------ 
                this.CustomDrawToThisCanvas(canvas, updateArea);
                //------------------------------------------
                propFlags |= RenderElementConst.IS_GRAPHIC_VALID;
#if DEBUG
                debug_RecordPostDrawInfo(canvas);
#endif
            }
            else
            {
            }
            canvas.PopClipAreaRect();
#if DEBUG
            dbugVRoot.dbug_drawLevel--;
#endif
        }

        //==============================================================
        //set location and size , not bubble***

        public static void DirectSetSize(RenderElement visualElement, int width, int height)
        {
            visualElement.b_width = width;
            visualElement.b_height = height;
        }
        public static void DirectSetLocation(RenderElement visualElement, int x, int y)
        {
            visualElement.b_left = x;
            visualElement.b_top = y;
        }
    }
}