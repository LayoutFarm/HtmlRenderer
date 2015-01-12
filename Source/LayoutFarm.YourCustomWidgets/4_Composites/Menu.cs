// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;

using LayoutFarm.UI;
using LayoutFarm.RenderBoxes;
namespace LayoutFarm.CustomWidgets
{

    public class MenuItem : UIBox
    {

        CustomRenderBox primElement;//background 
        Color backColor = Color.LightGray;
        bool isOpen;
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
        protected override RenderElement CurrentPrimaryRenderElement
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
                var layers = new VisualLayerCollection();
                var layer0 = new PlainLayer(renderE);
                layers.AddLayer(layer0);
                renderE.Layers = layers;

                //layer0

                //layer0 
                //int layerCount = this.layers.Count;
                //for (int m = 0; m < layerCount; ++m)
                //{
                //    UICollection plain = (UICollection)this.layers[m];
                //    var groundLayer = new VisualPlainLayer(renderE);
                //    renderE.Layers.AddLayer(groundLayer);

                //    //---------------------------------
                //    int j = plain.Count;
                //    for (int i = 0; i < j; ++i)
                //    {
                //        groundLayer.AddUI(plain.GetElement(i));
                //    }
                //}
                //---------------------------------
                if (this.landPart != null)
                {
                    layer0.AddChild(this.landPart.GetPrimaryRenderElement(rootgfx));
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
        }
        protected override void OnDragBegin(UIMouseEventArgs e)
        {
            if (this.DragStart != null)
            {
                this.DragStart(this, e);
            }
            base.OnDragBegin(e);
        }
        protected override void OnDragEnd(UIMouseEventArgs e)
        {
            if (this.DragStop != null)
            {
                this.DragStop(this, e);
            }
            base.OnDragEnd(e);
        }
        protected override void OnMouseUp(UIMouseEventArgs e)
        {
            if (this.MouseUp != null)
            {
                MouseUp(this, e);
            }
            base.OnMouseUp(e);
        }
        protected override void OnDragging(UIMouseEventArgs e)
        {
            if (this.Dragging != null)
            {
                Dragging(this, e);
            }
            base.OnDragging(e);
        }
        //----------------------------------------------------

        public event EventHandler<UIMouseEventArgs> MouseDown;
        public event EventHandler<UIMouseEventArgs> MouseUp;

        public event EventHandler<UIMouseEventArgs> Dragging;
        public event EventHandler<UIMouseEventArgs> DragStart;
        public event EventHandler<UIMouseEventArgs> DragStop;

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
                        var visualPlainLayer = primElement.Layers.GetLayer(0) as PlainLayer;
                        if (visualPlainLayer != null)
                        {
                            visualPlainLayer.AddChild(value.GetPrimaryRenderElement(primElement.Root));
                        }

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
                            this.floatPartRenderElement = this.floatPart.GetPrimaryRenderElement(primElement.Root) as CustomRenderBox;
                            topRenderBox.AddChild(floatPartRenderElement);
                            //temp here
                            topRenderBox.InvalidateGraphics();
                        }

                    } break;
                case HingeFloatPartStyle.Embeded:
                    {

                    } break;
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
                    } break;
                case HingeFloatPartStyle.Popup:
                    {


                        var topRenderBox = primElement.GetTopWindowRenderBox();
                        if (topRenderBox != null)
                        {
                            if (this.floatPartRenderElement != null)
                            {
                                topRenderBox.Layer0.RemoveChild(floatPartRenderElement);
                            }

                        }

                    } break;
                case HingeFloatPartStyle.Embeded:
                    {
                    } break;

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


        public void AddSubMenuItem(MenuItem childItem)
        {
            if (childItems == null)
            {
                childItems = new List<MenuItem>();
            }
            this.childItems.Add(childItem);
            floatPart.AddChildBox(childItem);
        }
    }

    public class MenuBox : Panel
    {
        bool showing;
        TopWindowRenderBox topWindow;
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
                    var plainLayer = topWindow.Layer0;
                    plainLayer.RemoveChild(this.myRenderE);
                }
            }
        }

    }
}