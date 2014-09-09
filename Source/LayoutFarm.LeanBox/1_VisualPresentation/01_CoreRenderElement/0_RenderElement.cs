//2014 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace LayoutFarm.Presentation
{

    public abstract partial class RenderElement
    {

        IParentLink visualParentLink;
        object controller;

        public RenderElement(int width, 
            int height, 
            ElementNature nature)
        {

            this.b_width = width;
            this.b_Height = height;
            SetVisualElementNature(this, nature);
#if DEBUG
            dbug_totalObjectId++;
            dbug_obj_id = dbug_totalObjectId;
            this.dbug_SetFixedElementCode(this.GetType().Name);
#endif
        }
        //=========================================
        public object GetController()
        {
            return controller;
        } 
        public void SetController(object ui)
        {
            this.controller = ui;
        }
        //=========================================

        public VisualElementArgs GetVInv()
        {

            if (this.IsWindowRoot)
            {

                return new VisualElementArgs((RootWindowRenderBox)this);

            }
            else
            {
                RootWindowRenderBox winroot = this.InternalGetWinRootElement();
                if (winroot != null)
                {
                    return new VisualElementArgs((RootWindowRenderBox)winroot);

                }
                else
                {
                    return new VisualElementArgs(null as RootWindowRenderBox);
                }
            }
        }
        public void FreeVInv(VisualElementArgs vinv)
        {

        }

        public RootWindowRenderBox WinRoot
        {
            get
            {
                return this.InternalGetWinRootElement();
            }
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
        protected void RegisterNativeEvent(int registerEventFlags)
        {
            oneBitNativeEventFlags |= registerEventFlags;

        }
        public void RemoveSelf(VisualElementArgs vinv)
        {

            if (visualParentLink == null)
            {
                return;
            }
            if (vinv != null)
            {
                this.InvalidateGraphic(vinv);
            }
            visualParentLink.Unlink(this);
            visualParentLink = null;
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


        public void SetVisible(bool value, VisualElementArgs vinv)
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

                InvalidateGraphic(vinv);
                if (value)
                {
                    uiFlags &= ~HIDDEN;
                }
                else
                {
                    uiFlags |= HIDDEN;
                }
                InvalidateGraphic(vinv);

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
        int oneBitNativeEventFlags = 0;
        int uiCombineFlags;


        const int IS_TRANSLUCENT_BG = 1 << (1 - 1);
        const int SCROLLABLE_FULL_MODE = 1 << (2 - 1); const int TRANSPARENT_FOR_ALL_EVENTS = 1 << (3 - 1);
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
        const int USE_ANIMATOR = 1 << (30 - 1);



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
                    this.ParentVisualElement == this.InternalGetWinRootElement();
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


        public bool HasSubGroundLayer
        {

            get
            {
                return (uiFlags & HAS_SUB_GROUND) != 0;
            }
            set
            {
                if (value)
                {
                    uiFlags |= HAS_SUB_GROUND;
                }
                else
                {
                    uiFlags &= ~HAS_SUB_GROUND;
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


        public ElementNature ElementNature
        {
            get
            {
                return (ElementNature)(uiCombineFlags & 0xF);
            }
        }
        static void SetVisualElementNature(RenderElement target, ElementNature visualNature)
        {
            target.uiCombineFlags = (target.uiCombineFlags & ~0xF) | (int)visualNature;
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
            if (lineLinkedNode == null)
            {
            }
            childElement.visualParentLink = lineLinkedNode;
        }
        public static void SetVisualElementAsChildOfSimpleContainer(RenderElement childElement, IParentLink lineLinkedNode)
        {

            childElement.visualParentLink = lineLinkedNode;
        }


    }
}