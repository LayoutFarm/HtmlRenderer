// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;
using LayoutFarm.RenderBoxes;
namespace LayoutFarm
{


    public abstract partial class RenderElement
    {

        bool mayHasChild;
        bool mayHasViewport;
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
            //this.dbug_SetFixedElementCode(this.GetType().Name);
#endif
        }
        public RootGraphic Root
        {
            get { return this.rootGfx; }
        }

        public object GetController()
        {
            return controller;
        }
        public void SetController(object controller)
        {
            this.controller = controller;
        }

        public bool IsFreeElement
        {
            get
            {
                return parentLink == null;
            }
        }
        public virtual void ClearAllChildren()
        {

        }
        public IParentLink ParentLink
        {
            get
            {
                return parentLink;
            }
        }
#if DEBUG
        public RenderElement dbugParentVisualElement
        {
            get { return this.ParentVisualElement; }
        }
#endif

        public virtual RenderElement ParentVisualElement
        {
            get
            {
                if (parentLink == null)
                {
                    return null;
                }
                return parentLink.ParentVisualElement;
            }
        }

        public bool Visible
        {
            get
            {
                return ((propFlags & RenderElementConst.HIDDEN) == 0);
            }

        }
        public void SetVisible(bool value)
        {

            if (parentLink == null)
            {


                if (value)
                {
                    propFlags &= ~RenderElementConst.HIDDEN;
                }
                else
                {
                    propFlags |= RenderElementConst.HIDDEN;
                }
            }
            else
            {

                InvalidateGraphic();
                if (value)
                {
                    propFlags &= ~RenderElementConst.HIDDEN;
                }
                else
                {
                    propFlags |= RenderElementConst.HIDDEN;
                }
                InvalidateGraphic();
            }

        }


       

#if DEBUG
        public override string ToString()
        {

            return string.Empty;
        }
#endif

        public bool IsInRenderChain
        {
            get
            {
                return (propFlags & RenderElementConst.IS_IN_RENDER_CHAIN) != 0;
            }
            set
            {
                propFlags = value ?
                   propFlags | RenderElementConst.IS_IN_RENDER_CHAIN :
                   propFlags & ~RenderElementConst.FIRST_ARR_PASS;

            }
        }

        public bool FirstArrangementPass
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
        public bool IsDragedOver
        {
            get
            {
                return (propFlags & RenderElementConst.IS_DRAG_OVERRED) != 0;
            }
            set
            {
                propFlags = value ?
                     propFlags | RenderElementConst.IS_DRAG_OVERRED :
                     propFlags & ~RenderElementConst.IS_DRAG_OVERRED;
            }
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
        public virtual void ChildrenHitTestCore(HitChain hitChain)
        {
        }
        
        public bool MayHasChild
        {
            get { return this.mayHasChild; }
            protected set { this.mayHasChild = value; }
        }

        public bool MayHasViewport
        {
            get { return this.mayHasViewport; }
            protected set { this.mayHasViewport = value; }
        }

        public int ViewportBottom
        {
            get
            {
                return this.Bottom + this.ViewportY;
            }
        }
        public int ViewportRight
        {
            get
            {
                return this.Right + this.ViewportX;
            }
        }
        public virtual int ViewportY
        {
            get
            {
                return 0;
            }

        }
        public virtual int ViewportX
        {
            get
            {
                return 0;
            }
        }
        public virtual RenderElement FindOverlapedChildElementAtPoint(RenderElement afterThisChild, Point point)
        {
            return null;
        }
        public static void RemoveParentLink(RenderElement childElement)
        {
            childElement.parentLink = null;
        }
        public static void SetParentLink(RenderElement childElement, IParentLink lineLinkedNode)
        {
            childElement.parentLink = lineLinkedNode;
        }
        public bool HasOwner
        {
            get
            {
                return this.parentLink != null;
            }
        }
        public RenderElement GetOwnerRenderElement()
        {
            if (this.parentLink != null)
            {
                return parentLink.ParentVisualElement as RenderBoxes.RenderBoxBase;
            }
            return null;
        }
    }
}