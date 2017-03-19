//Apache2, 2014-2017, WinterDev

using System;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;
using LayoutFarm.UI;
namespace LayoutFarm.CustomWidgets
{
    public class MenuItem : UIBox
    {
        CustomRenderBox primElement;//background 
        Color backColor = Color.LightGray;
        bool thisMenuOpened;
        //1. land part
        UIBox landPart;
        //2. float part   
        MenuBox floatPart;
        CustomRenderBox floatPartRenderElement;
        HingeFloatPartStyle floatPartStyle;
        List<MenuItem> childItems;
        public MenuItem(int width, int height)
            : base(width, height)
        {
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
        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {
            if (primElement == null)
            {
                var renderE = new CustomRenderBox(rootgfx, this.Width, this.Height);
                renderE.SetLocation(this.Left, this.Top);
                renderE.BackColor = backColor;
                renderE.HasSpecificSize = true;
                renderE.SetController(this);
                //------------------------------------------------
                //create visual layer                 

                if (this.landPart != null)
                {
                    renderE.AddChild(this.landPart);
                }
                if (this.floatPart != null)
                {
                }

                //---------------------------------
                primElement = renderE;
            }
            return primElement;
        }
        //----------------------------------------------------
        protected override void OnMouseDown(UIMouseEventArgs e)
        {
            if (this.MouseDown != null)
            {
                this.MouseDown(this, e);
            }
            base.OnMouseDown(e);
        }
        protected override void OnMouseUp(UIMouseEventArgs e)
        {
            if (this.MouseUp != null)
            {
                MouseUp(this, e);
            }
            base.OnMouseUp(e);
        }

        //---------------------------------------------------- 
        public event EventHandler<UIMouseEventArgs> MouseDown;
        public event EventHandler<UIMouseEventArgs> MouseUp;
        //----------------------------------------------------  
        public UIBox LandPart
        {
            get { return this.landPart; }
            set
            {
                this.landPart = value;
                if (value != null)
                {
                    //if new value not null
                    //check existing land part
                    if (this.landPart != null)
                    {
                        //remove existing landpart
                    }

                    if (primElement != null)
                    {
                        //add 
                        primElement.AddChild(value);
                    }
                }
                else
                {
                    if (this.landPart != null)
                    {
                        //remove existing landpart

                    }
                }
            }
        }
        public MenuBox FloatPart
        {
            get { return this.floatPart; }
            set
            {
                this.floatPart = value;
                if (value != null)
                {
                    //attach float part 
                }
            }
        }
        //---------------------------------------------------- 
        public bool IsOpened
        {
            get { return this.thisMenuOpened; }
        }
        public void Open()
        {
            if (thisMenuOpened) return;
            this.thisMenuOpened = true;
            //-----------------------------------
            if (this.primElement == null) return;
            if (floatPart == null) return;
            switch (floatPartStyle)
            {
                default:
                case HingeFloatPartStyle.Popup:
                    {
                        //add float part to top window layer
                        var topRenderBox = primElement.GetTopWindowRenderBox();
                        if (topRenderBox != null)
                        {
                            Point globalLocation = primElement.GetGlobalLocation();
                            floatPart.SetLocation(globalLocation.X, globalLocation.Y + primElement.Height);
                            this.floatPartRenderElement = this.floatPart.GetPrimaryRenderElement(primElement.Root) as CustomRenderBox;
                            topRenderBox.AddChild(floatPartRenderElement);
                            //temp here

                        }
                    }
                    break;
                case HingeFloatPartStyle.Embeded:
                    {
                    }
                    break;
            }
        }
        public void Close()
        {
            if (!thisMenuOpened) return;
            this.thisMenuOpened = false;
            if (this.primElement == null) return;
            if (floatPart == null) return;
            switch (floatPartStyle)
            {
                default:
                    {
                    }
                    break;
                case HingeFloatPartStyle.Popup:
                    {
                        var topRenderBox = primElement.GetTopWindowRenderBox();
                        if (topRenderBox != null)
                        {
                            if (this.floatPartRenderElement != null)
                            {
                                topRenderBox.RemoveChild(floatPartRenderElement);
                            }
                        }
                    }
                    break;
                case HingeFloatPartStyle.Embeded:
                    {
                    }
                    break;
            }
        }
        public void MaintenanceParentOpenState()
        {
            if (this.ParentMenuItem != null)
            {
                this.ParentMenuItem.MaintenceOpenState = true;
                this.ParentMenuItem.MaintenanceParentOpenState();
            }
        }
        public void UnmaintenanceParentOpenState()
        {
            if (this.ParentMenuItem != null)
            {
                this.ParentMenuItem.MaintenceOpenState = false;
                this.ParentMenuItem.MaintenanceParentOpenState();
            }
        }
        public bool MaintenceOpenState
        {
            get;
            private set;
        }
        public void CloseRecursiveUp()
        {
            this.Close();
            if (this.ParentMenuItem != null &&
               !this.ParentMenuItem.MaintenceOpenState)
            {
                this.ParentMenuItem.CloseRecursiveUp();
            }
        }
        public MenuItem ParentMenuItem
        {
            get;
            private set;
        }
        public HingeFloatPartStyle FloatPartStyle
        {
            get { return this.floatPartStyle; }
            set
            {
                this.floatPartStyle = value;
            }
        }
        public void AddSubMenuItem(MenuItem childItem)
        {
            if (childItems == null)
            {
                childItems = new List<MenuItem>();
            }
            this.childItems.Add(childItem);
            floatPart.AddChild(childItem);
            childItem.ParentMenuItem = this;
        }

        public override void Walk(UIVisitor visitor)
        {
            visitor.BeginElement(this, "menuitem");
            this.Describe(visitor);
            visitor.EndElement();
        }
    }


    public class MenuBox : EaseBox
    {
        bool showing;
        RenderBoxBase topWindow;
        RenderElement myRenderE;
        public MenuBox(int w, int h)
            : base(w, h)
        {
        }
        public void ShowMenu(RootGraphic rootgfx)
        {
            //add to topmost box 
            if (!showing)
            {
                this.topWindow = rootgfx.TopWindowRenderBox;
                if (topWindow != null)
                {
                    topWindow.AddChild(this.myRenderE = this.GetPrimaryRenderElement(topWindow.Root));
                }
                showing = true;
            }
        }
        public void HideMenu()
        {
            if (showing)
            {
                //remove from top 
                showing = false;
                if (this.topWindow != null && this.myRenderE != null)
                {
                    topWindow.RemoveChild(this.myRenderE);
                }
            }
        }

        public override void Walk(UIVisitor visitor)
        {
            visitor.BeginElement(this, "menubox");
            this.Describe(visitor);
            visitor.EndElement();
        }
    }
}