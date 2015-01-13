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
    public class ListView : UIBox
    {
        //composite          
        CustomRenderBox primElement;//background

        Color backColor = Color.LightGray;
        int viewportX, viewportY;
        List<UICollection> layers = new List<UICollection>(1);

        int latestItemY; 
        Panel panel;
        public ListView(int width, int height)
            : base(width, height)
        {
            UICollection plainLayer = new UICollection(this);
            //panel for listview items
            this.panel = new Panel(width, height);
            panel.BackColor = Color.LightGray;
            plainLayer.AddUI(panel);
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
                 
                renderE.SetLocation(this.Left, this.Top);
                renderE.BackColor = backColor;
                renderE.SetController(this);
                renderE.HasSpecificSize = true;

                //------------------------------------------------
                //create visual layer
                renderE.Layers = new VisualLayerCollection();
                int layerCount = this.layers.Count;
                for (int m = 0; m < layerCount; ++m)
                {
                    UICollection plain = (UICollection)this.layers[m];
                    var groundLayer = new PlainLayer(renderE);
                    renderE.Layers.AddLayer(groundLayer);
                    renderE.SetViewport(this.viewportX, this.viewportY);
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


        public void AddItem(ListItem ui)
        {
            //append last?
            //not correct if we remove above item
            //TODO: use automatic arrange layer 
            ui.SetLocation(0, latestItemY);
            latestItemY += ui.Height;
            panel.AddChildBox(ui);

        }
        //----------------------------------------------------
        protected override void OnMouseDown(UIMouseEventArgs e)
        {
            if (this.MouseDown != null)
            {
                this.MouseDown(this, e);
            }
        } 
        protected override void OnMouseUp(UIMouseEventArgs e)
        {
            if (this.MouseUp != null)
            {
                MouseUp(this, e);
            }
            base.OnMouseUp(e);
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
                this.panel.SetViewport(x, y);                 
            }
        }
        //----------------------------------------------------

        public event EventHandler<UIMouseEventArgs> MouseDown;
        public event EventHandler<UIMouseEventArgs> MouseUp;

        
    }


    public class ListItem : UIBox
    {
        CustomRenderBox primElement;
        Color backColor;
        public ListItem(int width, int height)
            : base(width, height)
        {
        }
        protected override RenderElement CurrentPrimaryRenderElement
        {
            get { return this.primElement; }
        }
        protected override bool HasReadyRenderElement
        {
            get { return primElement != null; }
        }
        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {
            if (primElement == null)
            {
                var element = new CustomRenderBox(rootgfx, this.Width, this.Height);
                element.SetLocation(this.Left, this.Top);
                element.BackColor = this.backColor;
                this.primElement = element;
            }
            return primElement;
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
    }

}