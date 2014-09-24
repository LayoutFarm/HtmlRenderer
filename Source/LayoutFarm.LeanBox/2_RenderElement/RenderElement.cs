//2014 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using LayoutFarm.Drawing;

namespace LayoutFarm
{


    public abstract partial class RenderElement
    {

        IParentLink visualParentLink;
        //special 
        bool isWindowRoot;
        bool mayHasChild;
        bool mayHasViewport;

        RootGraphic rootGfx;
        public RenderElement(RootGraphic rootGfx, int width, int height)
        {
            this.b_width = width;
            this.b_Height = height;
            this.rootGfx = rootGfx;
#if DEBUG
            dbug_totalObjectId++;
            dbug_obj_id = dbug_totalObjectId;
            this.dbug_SetFixedElementCode(this.GetType().Name);
#endif
        }
        public RootGraphic Root
        {
            get { return this.rootGfx; }
        }
        

        public bool IsFreeElement
        {
            get
            {
                return visualParentLink == null;
            }
        }
        public virtual void ClearAllChildren()
        {

        }
        public IParentLink ParentLink
        {
            get
            {
                return visualParentLink;
            }
        }


        public static void RemoveParentLink(RenderElement visual)
        {
            visual.visualParentLink = null;
        }
        public virtual RenderElement ParentVisualElement
        {
            get
            {
                if (visualParentLink == null)
                {
                    return null;
                }
                return visualParentLink.ParentVisualElement;
            }
        }


        public bool Visible
        {

            get
            {
                return ((uiFlags & HIDDEN) == 0);
            }

        }


        public void SetVisible(bool value)
        {

            if (visualParentLink == null)
            {

                if (value)
                {
                    uiFlags &= ~HIDDEN;
                }
                else
                {
                    uiFlags |= HIDDEN;
                }
            }
            else
            {

                InvalidateGraphic(); 
                if (value)
                {
                    uiFlags &= ~HIDDEN;
                }
                else
                {
                    uiFlags |= HIDDEN;
                }
                InvalidateGraphic();
            }

        }

        public static bool IsTestableElement(RenderElement ui)
        {
            return (ui != null) && ((ui.uiFlags & HIDDEN) == 0) && (ui.visualParentLink != null);
        }

        public bool Focusable
        {
            get
            {
                return (uiFlags & NOT_ACCEPT_FOCUS) == 0;
            }
            set
            {
                if (value)
                {
                    uiFlags &= ~NOT_ACCEPT_FOCUS;
                }
                else
                {
                    uiFlags |= NOT_ACCEPT_FOCUS;
                }
            }
        }


        int uiFlags;


        const int IS_TRANSLUCENT_BG = 1 << (1 - 1);
        const int SCROLLABLE_FULL_MODE = 1 << (2 - 1);
        const int TRANSPARENT_FOR_ALL_EVENTS = 1 << (3 - 1);
        const int HIDDEN = 1 << (4 - 1);
        const int IS_GRAPHIC_VALID = 1 << (5 - 1);
        const int IS_DRAG_OVERRED = 1 << (6 - 1);
        const int IS_IN_ANIMATION_MODE = 1 << (7 - 1);

        const int LISTEN_DRAG_EVENT = 1 << (9 - 1);
        const int ANIMATION_WAITING_FOR_NORMAL_MODE = 1 << (10 - 1);
        const int IS_BLOCK_ELEMENT = 1 << (11 - 1);
        const int HAS_OUTER_BOUND_EFFECT = 1 << (12 - 1);
        const int NOT_ACCEPT_FOCUS = 1 << (13 - 1);
        const int IS_LINE_BREAK = 1 << (14 - 1);
        const int IS_STRECHABLE = 1 << (15 - 1);

        const int HAS_DOUBLE_SCROLL_SURFACE = 1 << (22 - 1);
        const int HAS_DRAG_BROADCASTABLE = 1 << (23 - 1);

        const int IS_IN_RENDER_CHAIN = 1 << (24 - 1);
        const int IS_SCROLLABLE = 1 << (25 - 1);
        const int IS_PAGE_WINDOW = 1 << (26 - 1);
        const int FIRST_ARR_PASS = 1 << (27 - 1);
        const int HAS_SUB_GROUND = 1 << (28 - 1);
        const int IS_FLOATING_WINDOW = 1 << (29 - 1);



#if DEBUG
        public override string ToString()
        {

            return string.Empty;
        }
#endif

        public bool CanbeFloatingWindow
        {
            get
            {
                return (uiFlags & IS_FLOATING_WINDOW) != 0;
            }
            set
            {
                if (value)
                {
                    uiFlags |= IS_FLOATING_WINDOW;
                }
                else
                {
                    uiFlags &= ~IS_FLOATING_WINDOW;
                }
            }
        }


        public bool HasDragBroadcastable
        {
            get
            {
                return (uiFlags & HAS_DRAG_BROADCASTABLE) != 0;
            }
            set
            {
                if (value)
                {
                    uiFlags |= HAS_DRAG_BROADCASTABLE;
                }
                else
                {
                    uiFlags &= ~HAS_DRAG_BROADCASTABLE;
                }
            }
        }

        public bool IsInRenderChain
        {
            get
            {
                return (uiFlags & IS_IN_RENDER_CHAIN) != 0;
            }
            set
            {
                if (value)
                {
                    uiFlags |= IS_IN_RENDER_CHAIN;
                }
                else
                {
                    uiFlags &= ~IS_IN_RENDER_CHAIN;
                }
            }
        }

        public bool ActAsFloatingWindow
        {
            get
            {
                return this.CanbeFloatingWindow &&
                    this.ParentVisualElement == this.GetTopWindowRenderBox();
            }
        }

        public bool IsPageWindow
        {
            get
            {
                return (uiFlags & IS_PAGE_WINDOW) != 0;
            }
            set
            {
                if (value)
                {
                    uiFlags |= IS_PAGE_WINDOW;
                }
                else
                {
                    uiFlags &= ~IS_PAGE_WINDOW;
                }
            }
        }
        public bool FirstArrangementPass
        {

            get
            {
                return (uiFlags & FIRST_ARR_PASS) != 0;
            }
            set
            {
                if (value)
                {
                    uiFlags |= FIRST_ARR_PASS;
                }
                else
                {
                    uiFlags &= ~FIRST_ARR_PASS;
                }
            }
        }

        public bool IsBlockElement
        {
            get
            {
                return ((uiFlags & IS_BLOCK_ELEMENT) == IS_BLOCK_ELEMENT);
            }
            set
            {
                if (value)
                {
                    uiFlags |= IS_BLOCK_ELEMENT;
                }
                else
                {
                    uiFlags &= ~IS_BLOCK_ELEMENT;
                }
            }
        }



        public bool IsDragedOver
        {
            get
            {
                return (uiFlags & IS_DRAG_OVERRED) != 0;
            }
            set
            {
                if (value)
                {
                    uiFlags |= IS_DRAG_OVERRED;//
                }
                else
                {
                    uiFlags &= ~IS_DRAG_OVERRED;
                }
            }
        }

        public bool ListeningDragEvent
        {
            get
            {
                return (uiFlags & LISTEN_DRAG_EVENT) != 0;
            }
            set
            {
                if (value)
                {
                    if (!ListeningDragEvent)
                    {
                        uiFlags |= LISTEN_DRAG_EVENT;
                    }
                }
                else
                {
                    if (ListeningDragEvent)
                    {
                        uiFlags &= ~LISTEN_DRAG_EVENT;

                    }
                }
            }
        }

        internal static void SetIsWindowRoot(RenderElement e, bool isWinRoot)
        {
            e.isWindowRoot = isWinRoot;
        }
        public bool MayHasChild
        {
            get { return this.mayHasChild; }
        }
        internal static void SetMayHasChild(RenderElement e, bool mayHasChild)
        {
            e.mayHasChild = mayHasChild;
        }
        public bool MayHasViewport
        {
            get { return this.mayHasViewport; }
        }
        internal static void SetMayHasViewport(RenderElement e, bool mayHasViewport)
        {
            e.mayHasViewport = mayHasViewport;
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

        public static void ClearVisualElementInternalLinkedNode(RenderElement childElement)
        {

            childElement.visualParentLink = null;
        }

        public static void SetVisualElementAsChildOfOther(RenderElement childElement, IParentLink lineLinkedNode)
        {

            childElement.visualParentLink = lineLinkedNode;
        }
        public static void SetVisualElementAsChildOfSimpleContainer(RenderElement childElement, IParentLink lineLinkedNode)
        {

            childElement.visualParentLink = lineLinkedNode;
        }

        public bool HasOwner
        {
            get
            {
                return this.visualParentLink != null;
            }
        }
        public RenderElement GetOwnerRenderElement()
        {
            if (this.visualParentLink != null)
            {
                return visualParentLink.ParentVisualElement as RenderBoxBase;
            }
            return null;
        }
    }
}