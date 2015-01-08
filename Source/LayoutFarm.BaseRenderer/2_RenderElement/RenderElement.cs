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
        bool isWindowRoot;
        bool mayHasChild;
        bool mayHasViewport;
        RootGraphic rootGfx;
        IParentLink parentLink;
        object controller;
        int uiFlags;

        public RenderElement(RootGraphic rootGfx, int width, int height)
        {
            this.b_width = width;
            this.b_Height = height;
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
                return ((uiFlags & RenderElementConst.HIDDEN) == 0);
            }

        }
        public void SetVisible(bool value)
        {

            if (parentLink == null)
            {


                if (value)
                {
                    uiFlags &= ~RenderElementConst.HIDDEN;
                }
                else
                {
                    uiFlags |= RenderElementConst.HIDDEN;
                }
            }
            else
            {

                InvalidateGraphic();
                if (value)
                {
                    uiFlags &= ~RenderElementConst.HIDDEN;
                }
                else
                {
                    uiFlags |= RenderElementConst.HIDDEN;
                }
                InvalidateGraphic();
            }

        }


        public bool Focusable
        {
            get
            {
                return (uiFlags & RenderElementConst.NOT_ACCEPT_FOCUS) == 0;
            }
            set
            {
                if (value)
                {
                    uiFlags &= ~RenderElementConst.NOT_ACCEPT_FOCUS;
                }
                else
                {
                    uiFlags |= RenderElementConst.NOT_ACCEPT_FOCUS;
                }
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
                return (uiFlags & RenderElementConst.IS_IN_RENDER_CHAIN) != 0;
            }
            set
            {
                uiFlags = value ?
                   uiFlags | RenderElementConst.IS_IN_RENDER_CHAIN :
                   uiFlags & ~RenderElementConst.FIRST_ARR_PASS;

            }
        }

        public bool FirstArrangementPass
        {

            get
            {
                return (uiFlags & RenderElementConst.FIRST_ARR_PASS) != 0;
            }
            set
            {
                uiFlags = value ?
                   uiFlags | RenderElementConst.FIRST_ARR_PASS :
                   uiFlags & ~RenderElementConst.FIRST_ARR_PASS;
            }
        }

        public bool IsBlockElement
        {
            get
            {
                return ((uiFlags & RenderElementConst.IS_BLOCK_ELEMENT) == RenderElementConst.IS_BLOCK_ELEMENT);
            }
            set
            {
                uiFlags = value ?
                     uiFlags | RenderElementConst.IS_BLOCK_ELEMENT :
                     uiFlags & ~RenderElementConst.IS_BLOCK_ELEMENT;
            }
        }
        public bool IsDragedOver
        {
            get
            {
                return (uiFlags & RenderElementConst.IS_DRAG_OVERRED) != 0;
            }
            set
            {
                uiFlags = value ?
                     uiFlags | RenderElementConst.IS_DRAG_OVERRED :
                     uiFlags & ~RenderElementConst.IS_DRAG_OVERRED;
            }
        }

        public bool ListeningDragEvent
        {
            get
            {
                return (uiFlags & RenderElementConst.LISTEN_DRAG_EVENT) != 0;
            }
            set
            {
                uiFlags = value ?
                       uiFlags | RenderElementConst.LISTEN_DRAG_EVENT :
                       uiFlags & ~RenderElementConst.LISTEN_DRAG_EVENT;
            }
        }
        public bool TransparentForAllEvents
        {
            get
            {
                return (uiFlags & RenderElementConst.TRANSPARENT_FOR_ALL_EVENTS) != 0;
            }
            set
            {
                uiFlags = value ?
                       uiFlags | RenderElementConst.TRANSPARENT_FOR_ALL_EVENTS :
                       uiFlags & ~RenderElementConst.TRANSPARENT_FOR_ALL_EVENTS;

            }
        }
        public virtual void ChildrenHitTestCore(HitChain hitChain)
        {
        }
        internal static void SetIsWindowRoot(RenderElement e, bool isWinRoot)
        {
            e.isWindowRoot = isWinRoot;
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