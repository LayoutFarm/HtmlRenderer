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

    public class Panel : EaseBox
    {
        PanelLayoutKind panelLayoutKind;
        PanelStretch panelChildStretch;

        CustomRenderBox primElement;
        Color backColor = Color.LightGray;
        int viewportX;
        int viewportY;

        UICollection uiList;
        bool needContentLayout;

        public Panel(int width, int height)
            : base(width, height)
        {
            uiList = new UICollection(this);

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
        //public Color BackColor
        //{
        //    get { return this.backColor; }
        //    set
        //    {
        //        this.backColor = value;
        //        if (HasReadyRenderElement)
        //        {
        //            this.primElement.BackColor = value;
        //        }
        //    }
        //}
        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {
            if (primElement == null)
            {

                var renderE = new CustomRenderBox(rootgfx, this.Width, this.Height);
                renderE.HasSpecificHeight = this.HasSpecificHeight;
                renderE.HasSpecificWidth = this.HasSpecificWidth;
                renderE.SetController(this);
#if DEBUG
                //if (dbugBreakMe)
                //{
                //    renderE.dbugBreak = true;
                //}
#endif
                renderE.SetLocation(this.Left, this.Top);
                renderE.BackColor = backColor;
                renderE.HasSpecificSize = true;
                renderE.SetViewport(this.viewportX, this.viewportY);
                //------------------------------------------------
                //create visual layer
                PlainLayer plan0 = new PlainLayer(renderE);
                int childCount = this.uiList.Count;
                for (int m = 0; m < childCount; ++m)
                {
                    plan0.AddChild(uiList.GetElement(m).GetPrimaryRenderElement(rootgfx));
                }
                renderE.Layer = plan0;

                //renderE.Layers = new VisualLayerCollection();
                //int layerCount = this.layers.Count;
                //for (int m = 0; m < layerCount; ++m)
                //{
                //    UICollection plain = (UICollection)this.layers[m];
                //    var groundLayer = new PlainLayer(renderE);
                //    renderE.Layers.AddLayer(groundLayer);
                //    //---------------------------------
                //    int j = plain.Count;
                //    for (int i = 0; i < j; ++i)
                //    {
                //        groundLayer.AddUI(plain.GetElement(i));
                //    }
                //}


                SetPrimaryRenderElement(renderE);
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
            this.uiList.AddUI(ui);


            //UICollection layer0 = (UICollection)this.layers[0];
            //layer0.AddUI(ui);
            if (this.HasReadyRenderElement)
            {
                PlainLayer plain1 = this.primElement.Layer as PlainLayer;
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
            this.uiList.RemoveUI(ui);
            if (this.HasReadyRenderElement)
            {
                PlainLayer plain1 = this.primElement.Layer as PlainLayer;
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
            //UICollection layer0 = (UICollection)this.layers[0];
            //layer0.Clear();
            this.uiList.Clear();
            if (this.HasReadyRenderElement)
            {
                PlainLayer plain1 = this.primElement.Layer as PlainLayer;
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

            //if (this.dbugBreakMe)
            //{
            //}

            this.InvalidateGraphics();
            //temp : arrange as vertical stack***
            switch (this.PanelLayoutKind)
            {
                case CustomWidgets.PanelLayoutKind.VerticalStack:
                    {

                        int count = this.uiList.Count;
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
                        int maxRight = 0;
                        for (int i = 0; i < count; ++i)
                        {
                            var element = this.uiList.GetElement(i) as UIBox;
                            if (element != null)
                            {
                                //if (element.dbugBreakMe)
                                //{

                                //}
                                element.PerformContentLayout();

                                int elemH = element.HasSpecificHeight ?
                                    element.Height :
                                    element.DesiredHeight;
                                int elemW = element.HasSpecificWidth ?
                                    element.Width :
                                    element.DesiredWidth;
                                element.SetBounds(0, ypos, element.Width, elemH);
                                ypos += element.Height;



                                int tmp_right = element.DesiredWidth + element.Left;
                                if (tmp_right > maxRight)
                                {
                                    maxRight = tmp_right;
                                }
                            }
                        }
                        this.desiredWidth = maxRight;
                        this.desiredHeight = ypos;
                    } break;
                case CustomWidgets.PanelLayoutKind.HorizontalStack:
                    {

                        int count = this.uiList.Count;
                        int xpos = 0;

                        int maxBottom = 0;

                        for (int i = 0; i < count; ++i)
                        {
                            var element = this.uiList.GetElement(i) as UIBox;
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

                        this.desiredWidth = xpos;
                        this.desiredHeight = maxBottom;
                    } break;
                default:
                    {

                        int count = this.uiList.Count;
                        int maxRight = 0;
                        int maxBottom = 0;

                        for (int i = 0; i < count; ++i)
                        {
                            var element = this.uiList.GetElement(i) as UIBox;
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
                            this.desiredWidth = maxRight;
                        }
                        if (!this.HasSpecificHeight)
                        {
                            this.desiredHeight = maxBottom;
                        }
                    } break;
            }
            //------------------------------------------------
            base.RaiseLayoutFinished();
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