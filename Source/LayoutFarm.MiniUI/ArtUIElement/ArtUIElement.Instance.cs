using System;
using System.Collections.Generic;
using System.Drawing;
 
namespace LayoutFarm.Presentation
{
    public enum UIKeyEventName
    {
        KeyDown,
        KeyUp,
        KeyPress,
        ProcessDialogKey
    }
    public enum UIMouseEventName
    {
        Click,
        DoubleClick,
        MouseDown,
        MouseMove,
        MouseUp,
        MouseEnter,
        MouseLeave,
        MouseHover,


    }
    public enum UIDragEventName
    {
        DragStart,
        DragStop,
        Dragging
    }
    public interface IVisualElementUI
    {
        void DispatchKeyPressEvent(ArtKeyPressEventArgs args);
        void DispatchKeyEvent(UIKeyEventName keyEventName, ArtKeyEventArgs e);
        bool DispatchProcessDialogKey(ArtKeyEventArgs args);
        void DispatchMouseEvent(UIMouseEventName mouseEventName, ArtMouseEventArgs e);
        void DispatchDragEvent(UIDragEventName dragEventName, ArtDragEventArgs e);
    }


                public abstract partial class ArtUIElement : IVisualElementUI
    {

                                
                                
                                ArtVisualElement primaryVisualElement; 
        public ArtUIElement()
        {   
        }

                                                                                                        
                                                public void FreeVInv(VisualElementArgs vinv)
        {
        }



        protected virtual void OnLostFocus(ArtFocusEventArgs e)
        {

        }
        protected virtual void OnLostMouseFocus(ArtFocusEventArgs2 e)
        {

        }
        protected virtual void OnGotFocus(ArtFocusEventArgs e)
        {
        }

                                        public void SetPrimaryVisualElement(ArtVisualElement visualElement)
        {
            this.primaryVisualElement = visualElement;
        }



        public ArtVisualElement PrimaryVisualElement
        {
            get
            {
                return primaryVisualElement;
            }
        }





        void IVisualElementUI.DispatchKeyPressEvent(ArtKeyPressEventArgs args)
        {
            OnKeyPress(args);
        }
        void IVisualElementUI.DispatchKeyEvent(UIKeyEventName keyEventName, ArtKeyEventArgs args)
        {
            switch (keyEventName)
            {
                case UIKeyEventName.KeyDown:
                    {
                        OnKeyDown(args);
                    } break;
                case UIKeyEventName.KeyUp:
                    {
                        OnKeyUp(args);
                    } break;
            }
        }
        bool IVisualElementUI.DispatchProcessDialogKey(ArtKeyEventArgs e)
        {
            return OnProcessDialogKey(e);
        }
        void IVisualElementUI.DispatchMouseEvent(UIMouseEventName evName, ArtMouseEventArgs e)
        {
            switch (evName)
            {
                case UIMouseEventName.Click:
                    {

                    } break;
                case UIMouseEventName.DoubleClick:
                    {
                        OnDoubleClick(e);
                    } break;
                case UIMouseEventName.MouseDown:
                    {
                        OnMouseDown(e);
                    } break;
                case UIMouseEventName.MouseMove:
                    {
                        OnMouseMove(e);
                    } break;
                case UIMouseEventName.MouseUp:
                    {
                        OnMouseUp(e);
                    } break;

            }
        }
        void IVisualElementUI.DispatchDragEvent(UIDragEventName evName, ArtDragEventArgs e)
        {
            switch (evName)
            {
                case UIDragEventName.Dragging:
                    {
                        OnDragging(e);
                    } break;
                case UIDragEventName.DragStart:
                    {
                        OnDragStart(e);
                    } break;
                case UIDragEventName.DragStop:
                    {
                        OnDragStop(e);
                    } break;
            }
        }

#if DEBUG

        object dbugTagObject;
        public object dbugTag
        {
            get
            {
                return this.dbugTagObject;
            }
            set
            {
                this.dbugTagObject = value;
            }
        }
#endif


    }
}