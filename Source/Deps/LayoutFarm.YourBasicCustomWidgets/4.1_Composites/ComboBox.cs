//Apache2, 2014-2017, WinterDev

using System;
using PixelFarm.Drawing;
using LayoutFarm.UI;
namespace LayoutFarm.CustomWidgets
{
    public class ComboBox : UIBox
    {
        CustomRenderBox primElement;//background 
        Color backColor = Color.LightGray;
        bool isOpen;
        //1. land part
        UIBox landPart;
        //2. float part   
        UIBox floatPart;
        RenderElement floatPartRenderElement;
        HingeFloatPartStyle floatPartStyle;
        public ComboBox(int width, int height)
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
                renderE.SetController(this);
                renderE.HasSpecificSize = true;
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
        public UIBox FloatPart
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
        public bool IsOpen
        {
            get { return this.isOpen; }
        }
        //---------------------------------------------------- 


        public void OpenHinge()
        {
            if (isOpen) return;
            this.isOpen = true;
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
                            this.floatPartRenderElement = this.floatPart.GetPrimaryRenderElement(primElement.Root);
                            topRenderBox.AddChild(floatPartRenderElement);
                        }
                    }
                    break;
                case HingeFloatPartStyle.Embeded:
                    {
                    }
                    break;
            }
        }
        public void CloseHinge()
        {
            if (!isOpen) return;
            this.isOpen = false;
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
                            topRenderBox.RemoveChild(floatPartRenderElement);
                        }
                    }
                    break;
                case HingeFloatPartStyle.Embeded:
                    {
                    }
                    break;
            }
        }

        public HingeFloatPartStyle FloatPartStyle
        {
            get { return this.floatPartStyle; }
            set
            {
                this.floatPartStyle = value;
            }
        }
        public override void Walk(UIVisitor visitor)
        {
            visitor.BeginElement(this, "combobox");
            this.Describe(visitor);
            visitor.EndElement();
        }
    }
}