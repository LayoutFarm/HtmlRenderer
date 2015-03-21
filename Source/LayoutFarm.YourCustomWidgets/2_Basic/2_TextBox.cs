// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;

using LayoutFarm;
using LayoutFarm.UI;
using LayoutFarm.Text;
using LayoutFarm.Composers;
namespace LayoutFarm.CustomWidgets
{

    public class TextBox : UIBox, IBoxElement, IUserEventPortal
    {
        TextSurfaceEventListener textSurfaceListener;
        TextEditRenderBox visualTextEdit;
        bool _multiline;
        TextSpanSytle defaultSpanStyle;

        public TextBox(int width, int height, bool multiline)
            : base(width, height)
        {
            this._multiline = multiline;

        }
        public void ClearText()
        {
            if (visualTextEdit != null)
            {
                this.visualTextEdit.ClearAllChildren();
            }
        }
        public TextSpanSytle DefaultSpanStyle
        {
            get { return this.defaultSpanStyle; }
            set
            {
                this.defaultSpanStyle = value;
                if (visualTextEdit != null)
                {
                    visualTextEdit.CurrentTextSpanStyle = value;
                }
            }
        }
        public override bool AcceptKeyboardFocus
        {
            get
            {
                return true;
            }
        }
        public void Focus()
        {
            //request keyboard focus
            visualTextEdit.Focus();
        }

        protected override bool HasReadyRenderElement
        {
            get { return this.visualTextEdit != null; }
        }
        public override RenderElement CurrentPrimaryRenderElement
        {
            get { return this.visualTextEdit; }
        }
        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {
            if (visualTextEdit == null)
            {
                var tbox = new TextEditRenderBox(rootgfx, this.Width, this.Height, _multiline);
                tbox.SetLocation(this.Left, this.Top);
                tbox.HasSpecificSize = true;
                if (this.defaultSpanStyle == null)
                {
                    this.defaultSpanStyle = new TextSpanSytle();
                    this.defaultSpanStyle.FontInfo = rootgfx.DefaultTextEditFontInfo;
                    
                    tbox.CurrentTextSpanStyle = this.defaultSpanStyle;
                }
                else
                {
                    tbox.CurrentTextSpanStyle = this.defaultSpanStyle;
                }
                tbox.SetController(this);
                RegisterNativeEvent(
                  1 << UIEventIdentifier.NE_MOUSE_DOWN
                  | 1 << UIEventIdentifier.NE_LOST_FOCUS
                  | 1 << UIEventIdentifier.NE_SIZE_CHANGED
                  );
                if (this.textSurfaceListener != null)
                {
                    tbox.TextSurfaceListener = textSurfaceListener;
                }
                this.visualTextEdit = tbox;
            }
            return visualTextEdit;
        }
        //----------------------------------------------------------------
        public TextSurfaceEventListener TextEventListener
        {
            get { return this.textSurfaceListener; }
            set
            {
                this.textSurfaceListener = value;

                if (this.visualTextEdit != null)
                {
                    this.visualTextEdit.TextSurfaceListener = value;
                }

            }
        }
        public EditableTextSpan CurrentTextSpan
        {
            get
            {

                return this.visualTextEdit.CurrentTextRun;
            }
        }
        public void ReplaceCurrentTextRunContent(int nBackspaces, string newstr)
        {
            if (visualTextEdit != null)
            {

                visualTextEdit.ReplaceCurrentTextRunContent(nBackspaces, newstr);
            }
        }
        public void ReplaceCurrentLineTextRuns(IEnumerable<EditableTextSpan> textRuns)
        {
            if (visualTextEdit != null)
            {
                visualTextEdit.ReplaceCurrentLineTextRuns(textRuns);
            }
        }
        public void CopyCurrentLine(StringBuilder stbuilder)
        {
            visualTextEdit.CopyCurrentLine(stbuilder);
        }
        //----------------------------------------------------------------

        protected override void OnMouseLeave(UIMouseEventArgs e)
        {
            e.MouseCursorStyle = MouseCursorStyle.Arrow;

        }
        protected override void OnDoubleClick(UIMouseEventArgs e)
        {
            visualTextEdit.OnDoubleClick(e);
            e.CancelBubbling = true;
        }
        protected override void OnKeyPress(UIKeyEventArgs e)
        {
            visualTextEdit.OnKeyPress(e);
            e.CancelBubbling = true;
        }
        protected override void OnKeyDown(UIKeyEventArgs e)
        {
            visualTextEdit.OnKeyDown(e);
            e.CancelBubbling = true;
        }
        protected override void OnKeyUp(UIKeyEventArgs e)
        {
            visualTextEdit.OnKeyUp(e);
            e.CancelBubbling = true;
        }
        protected override bool OnProcessDialogKey(UIKeyEventArgs e)
        {
            if (visualTextEdit.OnProcessDialogKey(e))
            {
                e.CancelBubbling = true;
                return true;
            }
            return false;
        }
        protected override void OnMouseDown(UIMouseEventArgs e)
        {
            this.Focus();

            e.MouseCursorStyle = MouseCursorStyle.IBeam;
            e.CancelBubbling = true;
            e.CurrentContextElement = this;
            visualTextEdit.OnMouseDown(e);
        }
        protected override void OnMouseMove(UIMouseEventArgs e)
        {
            if (e.IsDragging)
            {
                visualTextEdit.OnDrag(e);
                e.CancelBubbling = true;
                e.MouseCursorStyle = MouseCursorStyle.IBeam;
            }

            //base.OnMouseMove(e);
        }
        protected override void OnMouseUp(UIMouseEventArgs e)
        {
            if (e.IsDragging)
            {
                visualTextEdit.OnDragEnd(e);
            }
            else
            {
                visualTextEdit.OnMouseUp(e);
            }
            e.MouseCursorStyle = MouseCursorStyle.Default;
            e.CancelBubbling = true;
        }

#if DEBUG
        //int dbugMouseDragBegin = 0;
        //int dbugMouseDragging = 0;
        //int dbugMouseDragEnd = 0;
#endif
        //protected override void OnDragBegin(UIMouseEventArgs e)
        //{
        //    dbugMouseDragBegin++;
        //    this.isMouseDown = this.isDragging = true;
        //    visualTextEdit.OnDragBegin(e);
        //    e.CancelBubbling = true;
        //    e.MouseCursorStyle = MouseCursorStyle.IBeam;
        //}
        //protected override void OnDragging(UIMouseEventArgs e)
        //{
        //    dbugMouseDragging++;

        //    visualTextEdit.OnDrag(e);
        //    e.CancelBubbling = true;
        //    e.MouseCursorStyle = MouseCursorStyle.IBeam;
        //}
        //protected override void OnDragEnd(UIMouseEventArgs e)
        //{
        //    dbugMouseDragEnd++;
        //    visualTextEdit.OnDragEnd(e);
        //    this.isMouseDown = this.isDragging = false;
        //    e.MouseCursorStyle = MouseCursorStyle.Default;
        //    e.CancelBubbling = true;
        //}
        //------------------------------------------------------


        void IUserEventPortal.PortalKeyPress(UIKeyEventArgs e)
        {
            this.OnKeyPress(e);
        }
        void IUserEventPortal.PortalKeyDown(UIKeyEventArgs e)
        {
            this.OnKeyDown(e);
        }
        void IUserEventPortal.PortalKeyUp(UIKeyEventArgs e)
        {
            this.OnKeyUp(e);
        }
        bool IUserEventPortal.PortalProcessDialogKey(UIKeyEventArgs e)
        {
            return this.OnProcessDialogKey(e);
        }
        void IUserEventPortal.PortalMouseDown(UIMouseEventArgs e)
        {
            this.OnMouseDown(e);
        }
        void IUserEventPortal.PortalMouseMove(UIMouseEventArgs e)
        {
            this.OnMouseMove(e);

        }
        void IUserEventPortal.PortalMouseUp(UIMouseEventArgs e)
        {
            this.OnMouseUp(e);

        }

        void IUserEventPortal.PortalMouseWheel(UIMouseEventArgs e)
        {

        }

        void IUserEventPortal.PortalGotFocus(UIFocusEventArgs e)
        {

        }

        void IUserEventPortal.PortalLostFocus(UIFocusEventArgs e)
        {

        }
        void IBoxElement.ChangeElementSize(int w, int h)
        {
            this.SetSize(w, h);
        }
        int IBoxElement.MinHeight
        {
            get
            {
                //todo: use mimimum current font height
                return 17;
            }
        }
    }
}