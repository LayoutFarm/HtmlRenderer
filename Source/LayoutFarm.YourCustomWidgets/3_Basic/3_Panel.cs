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
    public enum PanelLayoutKind
    {
        Absolute,
        VerticalStack,
        HorizontalStack
    }
    public enum PanelStretch
    {
        None,
        Horizontal,
        Vertical,
        Both,
    }
    public class Panel : UIBox
    {

        public event EventHandler<EventArgs> LayoutFinished;

        PanelLayoutKind panelLayoutKind;
        PanelStretch panelChildStretch;

        CustomRenderBox primElement;
        Color backColor = Color.LightGray;
        int viewportX;
        int viewportY;

        List<UICollection> layers = new List<UICollection>(1);
        bool needContentLayout;

        public Panel(int width, int height)
            : base(width, height)
        {
            UICollection plainLayer = new UICollection(this);
            this.layers.Add(plainLayer);
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
                renderE.SetController(this);

                renderE.SetLocation(this.Left, this.Top);
                renderE.BackColor = backColor;
                renderE.HasSpecificSize = true;
                renderE.SetViewport(this.viewportX, this.viewportY);
                //------------------------------------------------
                //create visual layer
                renderE.Layers = new VisualLayerCollection();
                int layerCount = this.layers.Count;
                for (int m = 0; m < layerCount; ++m)
                {
                    UICollection plain = (UICollection)this.layers[m];
                    var groundLayer = new PlainLayer(renderE);
                    renderE.Layers.AddLayer(groundLayer);

                    //---------------------------------
                    int j = plain.Count;
                    for (int i = 0; i < j; ++i)
                    {
                        groundLayer.AddUI(plain.GetElement(i));
                    }
                }

                //---------------------------------
                primElement = renderE;
            }
            return primElement;
        }
        public override bool NeedContentLayout
        {
            get
            {
                return this.needContentLayout;
            }
        }
        public PanelLayoutKind PanelLayoutKind
        {
            get { return this.panelLayoutKind; }
            set
            {
                this.panelLayoutKind = value;
            }
        }
        public void AddChildBox(UIElement ui)
        {
            needContentLayout = true;
            UICollection layer0 = (UICollection)this.layers[0];
            layer0.AddUI(ui);
            if (this.HasReadyRenderElement)
            {
                PlainLayer plain1 = this.primElement.Layers.Layer0 as PlainLayer;
                plain1.AddUI(ui);
                if (this.panelLayoutKind != PanelLayoutKind.Absolute)
                {
                    this.InvalidateLayout();
                }
            }

            if (ui.NeedContentLayout)
            {
                ui.InvalidateLayout();
            }

        }
        public void RemoveChildBox(UIElement ui)
        {
            needContentLayout = true;
            UICollection layer0 = (UICollection)this.layers[0];
            layer0.RemoveUI(ui);
            if (this.HasReadyRenderElement)
            {
                PlainLayer plain1 = this.primElement.Layers.Layer0 as PlainLayer;
                if (this.panelLayoutKind != PanelLayoutKind.Absolute)
                {
                    this.InvalidateLayout();
                }

                plain1.RemoveUI(ui);
            }
        }
        public void ClearItems()
        {
            needContentLayout = true;
            UICollection layer0 = (UICollection)this.layers[0];
            layer0.Clear();
            if (this.HasReadyRenderElement)
            {
                PlainLayer plain1 = this.primElement.Layers.Layer0 as PlainLayer;
                plain1.Clear();
                if (this.panelLayoutKind != PanelLayoutKind.Absolute)
                {
                    this.InvalidateLayout();
                }
            }

        }

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
        protected override void OnContentLayout()
        {
            this.PerformContentLayout();
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
        public override void PerformContentLayout()
        {
            this.InvalidateGraphics();
            //temp : arrange as vertical stack***
            switch (this.PanelLayoutKind)
            {
                case CustomWidgets.PanelLayoutKind.VerticalStack:
                    {
                        UICollection layer0 = (UICollection)this.layers[0];
                        int count = layer0.Count;
                        int ypos = 0;

                        //todo: implement stretching ...
                        //switch (this.panelChildStretch)
                        //{
                        //    case PanelStretch.Horizontal:
                        //        {
                        //        } break;
                        //    case PanelStretch.Vertical:
                        //        {
                        //        } break;
                        //    case PanelStretch.Both:
                        //        {
                        //        } break;
                        //}

                        for (int i = 0; i < count; ++i)
                        {
                            var element = layer0.GetElement(i) as UIBox;
                            if (element != null)
                            {
                                element.PerformContentLayout();
                                element.SetBounds(0, ypos, element.Width, element.DesiredHeight);
                                ypos += element.DesiredHeight;
                            }
                        }
                        this.desiredHeight = ypos;
                    } break;
                case CustomWidgets.PanelLayoutKind.HorizontalStack:
                    {
                        UICollection layer0 = (UICollection)this.layers[0];
                        int count = layer0.Count;
                        int xpos = 0;
                        int d_h = 0;
                        for (int i = 0; i < count; ++i)
                        {
                            var element = layer0.GetElement(i) as UIBox;
                            if (element != null)
                            {
                                element.PerformContentLayout();
                                element.SetBounds(xpos, 0, element.DesiredWidth, element.DesiredHeight);
                                xpos += element.DesiredWidth;
                                if (d_h < element.DesiredHeight)
                                {
                                    d_h = element.DesiredHeight;
                                }
                            }
                        }

                        this.desiredWidth = xpos;
                        this.desiredHeight = d_h;


                    } break;
                default:
                    {
                    } break;
            }
            //------------------------------------------------
            if (this.LayoutFinished != null)
            {
                this.LayoutFinished(this, EventArgs.Empty);
            }
        }
        public override int DesiredHeight
        {
            get
            {
                return this.desiredHeight;
            }
        }
        public override int DesiredWidth
        {
            get
            {
                return this.desiredWidth;
            }
        }
        //temp***
        int desiredHeight;
        int desiredWidth;
    }



}