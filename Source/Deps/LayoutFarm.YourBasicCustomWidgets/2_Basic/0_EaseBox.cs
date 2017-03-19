//Apache2, 2014-2017, WinterDev

using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
using LayoutFarm.UI;
namespace LayoutFarm.CustomWidgets
{
    public abstract class EaseBox : UIBox
    {
        BoxContentLayoutKind panelLayoutKind;
        bool needContentLayout;
        bool draggable;
        bool dropable;
        CustomRenderBox primElement;
        Color backColor = Color.LightGray;
        int viewportX;
        int viewportY;
        int desiredHeight;
        int desiredWidth;
        UICollection uiList;

        public event EventHandler<UIMouseEventArgs> MouseDown;
        public event EventHandler<UIMouseEventArgs> MouseMove;
        public event EventHandler<UIMouseEventArgs> MouseUp;
        public event EventHandler<UIMouseEventArgs> MouseDoubleClick;
        public event EventHandler<UIMouseEventArgs> MouseLeave;
        public event EventHandler<UIMouseEventArgs> MouseDrag;
        public event EventHandler<UIMouseEventArgs> LostMouseFocus;
        public event EventHandler<UIGuestTalkEventArgs> DragOver;
        //--------------------------------------------------------

        public event EventHandler<UIKeyEventArgs> KeyDown;

        public EaseBox(int width, int height)
            : base(width, height)
        {
            this.desiredHeight = height;
            this.desiredWidth = width;
        }

        protected override bool HasReadyRenderElement
        {
            get { return this.primElement != null; }
        }
        public override RenderElement CurrentPrimaryRenderElement
        {
            get { return this.primElement; }
        }
        public Color BackColor
        {
            get { return this.backColor; }
            set
            {
                this.backColor = value;
                if (HasReadyRenderElement)
                {
                    this.primElement.BackColor = value;
                }
            }
        }

        protected void SetPrimaryRenderElement(CustomRenderBox primElement)
        {
            this.primElement = primElement;
        }
        protected virtual void BuildChildrenRenderElement(RenderElement parent)
        {
            parent.HasSpecificHeight = this.HasSpecificHeight;
            parent.HasSpecificWidth = this.HasSpecificWidth;
            parent.SetController(this);
            parent.SetVisible(this.Visible);
#if DEBUG
            //if (dbugBreakMe)
            //{
            //    renderE.dbugBreak = true;
            //}
#endif
            parent.SetLocation(this.Left, this.Top);
            if (parent is CustomRenderBox)
            {
                ((CustomRenderBox)parent).BackColor = backColor;
            }

            parent.HasSpecificSize = true;
            parent.SetViewport(this.ViewportX, this.ViewportY);
            //------------------------------------------------


            //create visual layer 
            int childCount = this.ChildCount;
            for (int m = 0; m < childCount; ++m)
            {
                parent.AddChild(this.GetChild(m));
            }
            //set primary render element
            //---------------------------------

        }
        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {
            if (primElement == null)
            {
                var renderE = new CustomRenderBox(rootgfx, this.Width, this.Height);
                BuildChildrenRenderElement(renderE);
                this.primElement = renderE;
            }
            return primElement;
        }
        //----------------------------------------------------

        public bool AcceptKeyboardFocus
        {
            get;
            set;
        }
        protected override void OnDoubleClick(UIMouseEventArgs e)
        {
            if (this.MouseDoubleClick != null)
            {
                MouseDoubleClick(this, e);
            }
            if (this.AcceptKeyboardFocus)
            {
                this.Focus();
            }
        }
        protected override void OnMouseDown(UIMouseEventArgs e)
        {
            if (this.MouseDown != null)
            {
                this.MouseDown(this, e);
            }

            if (this.AcceptKeyboardFocus)
            {
                this.Focus();
            }
        }
        protected override void OnMouseMove(UIMouseEventArgs e)
        {
            if (e.IsDragging)
            {
                if (this.MouseDrag != null)
                {
                    this.MouseDrag(this, e);
                }
            }
            else
            {
                if (this.MouseMove != null)
                {
                    this.MouseMove(this, e);
                }
            }
        }
        protected override void OnMouseLeave(UIMouseEventArgs e)
        {
            if (this.MouseLeave != null)
            {
                this.MouseLeave(this, e);
            }
        }

        protected override void OnMouseUp(UIMouseEventArgs e)
        {
            if (this.MouseUp != null)
            {
                MouseUp(this, e);
            }
        }
        protected override void OnLostMouseFocus(UIMouseEventArgs e)
        {
            if (this.LostMouseFocus != null)
            {
                this.LostMouseFocus(this, e);
            }
        }

        public bool Draggable
        {
            get { return this.draggable; }
            set
            {
                this.draggable = value;
            }
        }
        public bool Droppable
        {
            get { return this.dropable; }
            set
            {
                this.dropable = value;
            }
        }

        public void RemoveSelf()
        {
            var parentBox = this.CurrentPrimaryRenderElement.ParentRenderElement as LayoutFarm.RenderElement;
            if (parentBox != null)
            {
                parentBox.RemoveChild(this.CurrentPrimaryRenderElement);
            }
            this.InvalidateOuterGraphics();
        }
        //----------------------------------------------------
        public override int ViewportX
        {
            get { return this.viewportX; }
        }
        public override int ViewportY
        {
            get { return this.viewportY; }
        }
        public override void SetViewport(int x, int y)
        {
            this.viewportX = x;
            this.viewportY = y;
            if (this.HasReadyRenderElement)
            {
                primElement.SetViewport(viewportX, viewportY);
            }
        }
        protected override void OnMouseWheel(UIMouseEventArgs e)
        {
            //vertical scroll
            if (this.desiredHeight > this.Height)
            {
                if (e.Delta < 0)
                {
                    //down
                    this.viewportY += 20;
                    if (viewportY > desiredHeight - this.Height)
                    {
                        this.viewportY = desiredHeight - this.Height;
                    }
                }
                else
                {
                    //up
                    this.viewportY -= 20;
                    if (viewportY < 0)
                    {
                        viewportY = 0;
                    }
                }
                this.primElement.SetViewport(viewportX, viewportY);
                this.InvalidateGraphics();
            }
        }
        //-------------------
        protected override bool OnProcessDialogKey(UIKeyEventArgs e)
        {
            if (KeyDown != null)
            {
                KeyDown(this, e);
            }
            //return true if you want to stop event bubble to other 
            if (e.CancelBubbling)
            {
                return true;
            }
            else
            {
                return base.OnProcessDialogKey(e);
            }
        }
        protected override void OnKeyDown(UIKeyEventArgs e)
        {
            base.OnKeyDown(e);
        }
        protected override void OnKeyPress(UIKeyEventArgs e)
        {
            base.OnKeyPress(e);
        }
        protected override void OnKeyUp(UIKeyEventArgs e)
        {
            base.OnKeyUp(e);
        }
        //-------------------
        public override int DesiredWidth
        {
            get
            {
                return this.desiredWidth;
            }
        }
        public override int DesiredHeight
        {
            get
            {
                return this.desiredHeight;
            }
        }
        protected void SetDesiredSize(int w, int h)
        {
            this.desiredWidth = w;
            this.desiredHeight = h;
        }

        //----------------------------------------------------
        public IEnumerable<UIElement> GetChildIter()
        {
            if (uiList != null)
            {
                int j = uiList.Count;
                for (int i = 0; i < j; ++i)
                {
                    yield return uiList.GetElement(i);
                }
            }
        }
        public void AddChild(UIElement ui)
        {
            if (this.uiList == null)
            {
                this.uiList = new UICollection(this);
            }

            needContentLayout = true;
            this.uiList.AddUI(ui);
            if (this.HasReadyRenderElement)
            {
                primElement.AddChild(ui);
                if (this.panelLayoutKind != BoxContentLayoutKind.Absolute)
                {
                    this.InvalidateLayout();
                }
            }

            if (ui.NeedContentLayout)
            {
                ui.InvalidateLayout();
            }
        }
        public void RemoveChild(UIElement ui)
        {
            needContentLayout = true;
            this.uiList.RemoveUI(ui);
            if (this.HasReadyRenderElement)
            {
                if (this.ContentLayoutKind != BoxContentLayoutKind.Absolute)
                {
                    this.InvalidateLayout();
                }
                this.primElement.RemoveChild(ui.CurrentPrimaryRenderElement);
            }
        }
        public void ClearChildren()
        {
            needContentLayout = true;
            if (this.uiList != null)
            {
                this.uiList.Clear();
            }
            if (this.HasReadyRenderElement)
            {
                primElement.ClearAllChildren();
                if (this.panelLayoutKind != BoxContentLayoutKind.Absolute)
                {
                    this.InvalidateLayout();
                }
            }
        }

        public int ChildCount
        {
            get
            {
                if (this.uiList != null)
                {
                    return this.uiList.Count;
                }
                return 0;
            }
        }
        public UIElement GetChild(int index)
        {
            return uiList.GetElement(index);
        }
        public override bool NeedContentLayout
        {
            get
            {
                return this.needContentLayout;
            }
        }
        public BoxContentLayoutKind ContentLayoutKind
        {
            get { return this.panelLayoutKind; }
            set
            {
                this.panelLayoutKind = value;
            }
        }
        protected override void OnContentLayout()
        {
            this.PerformContentLayout();
        }
        public override void PerformContentLayout()
        {
            this.InvalidateGraphics();
            //temp : arrange as vertical stack***
            switch (this.ContentLayoutKind)
            {
                case CustomWidgets.BoxContentLayoutKind.VerticalStack:
                    {
                        int count = this.ChildCount;
                        int ypos = 0;
                        int maxRight = 0;
                        for (int i = 0; i < count; ++i)
                        {
                            var element = this.GetChild(i) as UIBox;
                            if (element != null)
                            {
                                //if (element.dbugBreakMe)
                                //{

                                //}
                                element.PerformContentLayout();
                                //int elemH = element.HasSpecificHeight ?
                                //    element.Height :
                                //    element.DesiredHeight;
                                //int elemW = element.HasSpecificWidth ?
                                //    element.Width :
                                //    element.DesiredWidth;
                                //element.SetBounds(0, ypos, element.Width, elemH);
                                element.SetBounds(0, ypos, element.Width, element.Height);
                                ypos += element.Height;
                                int tmp_right = element.DesiredWidth + element.Left;
                                if (tmp_right > maxRight)
                                {
                                    maxRight = tmp_right;
                                }
                            }
                        }

                        this.SetDesiredSize(maxRight, ypos);
                    }
                    break;
                case CustomWidgets.BoxContentLayoutKind.HorizontalStack:
                    {
                        int count = this.ChildCount;
                        int xpos = 0;
                        int maxBottom = 0;
                        for (int i = 0; i < count; ++i)
                        {
                            var element = this.GetChild(i) as UIBox;
                            if (element != null)
                            {
                                element.PerformContentLayout();
                                element.SetBounds(xpos, 0, element.DesiredWidth, element.DesiredHeight);
                                xpos += element.DesiredWidth;
                                int tmp_bottom = element.DesiredHeight + element.Top;
                                if (tmp_bottom > maxBottom)
                                {
                                    maxBottom = tmp_bottom;
                                }
                            }
                        }

                        this.SetDesiredSize(xpos, maxBottom);
                    }
                    break;
                default:
                    {
                        int count = this.ChildCount;
                        int maxRight = 0;
                        int maxBottom = 0;
                        for (int i = 0; i < count; ++i)
                        {
                            var element = this.GetChild(i) as UIBox;
                            if (element != null)
                            {
                                element.PerformContentLayout();
                                int tmp_right = element.DesiredWidth + element.Left;
                                if (tmp_right > maxRight)
                                {
                                    maxRight = tmp_right;
                                }
                                int tmp_bottom = element.DesiredHeight + element.Top;
                                if (tmp_bottom > maxBottom)
                                {
                                    maxBottom = tmp_bottom;
                                }
                            }
                        }

                        if (!this.HasSpecificWidth)
                        {
                            this.SetDesiredSize(maxRight, this.DesiredHeight);
                        }
                        if (!this.HasSpecificHeight)
                        {
                            this.SetDesiredSize(this.DesiredWidth, maxBottom);
                        }
                    }
                    break;
            }
            //------------------------------------------------
            base.RaiseLayoutFinished();
        }
        protected override void Describe(UIVisitor visitor)
        {
            //describe base properties
            base.Describe(visitor);
            //describe child content
            if (uiList != null)
            {
                int j = this.uiList.Count;
                for (int i = 0; i < j; ++i)
                {
                    uiList.GetElement(i).Walk(visitor);
                }
            }
        }

        protected override void OnGuestTalk(UIGuestTalkEventArgs e)
        {
            if (this.DragOver != null)
            {
                this.DragOver(this, e);
            }
            base.OnGuestTalk(e);
        }
    }
}