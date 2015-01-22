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
    //tab page similar to listview

    public class TabPageContainer : UIBox
    {
        //composite          
        CustomRenderBox primElement;//background
        Color backColor = Color.LightGray;
        int viewportX, viewportY;
        List<UICollection> layers = new List<UICollection>(1);
        List<TabPage> tabPageCollection = new List<TabPage>();

        TabPage currentPage;
        Panel panel;
        int currentSelectedIndex;

        public TabPageContainer(int width, int height)
            : base(width, height)
        {
            UICollection plainLayer = new UICollection(this);
            //panel for listview items
            this.panel = new Panel(width, height);
            this.panel.PanelLayoutKind = PanelLayoutKind.VerticalStack;
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

        protected override void OnContentLayout()
        {
            panel.PerformContentLayout();
        }

        public override bool NeedContentLayout
        {
            get
            {
                return this.panel.NeedContentLayout;
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
                this.panel.SetViewport(x, y);
            }
        }


        //----------------------------------------------------
        public List<TabPage> TabPageList
        {
            get { return this.tabPageCollection; }
        }
        public void AddItem(TabPage ui)
        {
            tabPageCollection.Add(ui);
            ui.Owner = this;

            //show only one page per time
            if (currentPage == null)
            {
                currentPage = ui;
                panel.AddChildBox(ui);
            }
        }
        public void RemoveItem(TabPage p)
        {
            p.Owner = null;
            tabPageCollection.Remove(p);
            panel.RemoveChildBox(p);
        }
        public void ClearPages()
        {
            //TODO: implement this
        }
        public int SelectedIndex
        {
            get { return this.currentSelectedIndex; }
            set
            {
                if (value > -1 && value < this.tabPageCollection.Count
                    && this.currentSelectedIndex != value)
                {
                    this.currentSelectedIndex = value;
                    TabPage selectednedSelectedPage = this.tabPageCollection[value];
                    if (currentPage != null)
                    {
                        this.panel.RemoveChildBox(currentPage);
                    }
                    this.currentPage = selectednedSelectedPage;
                    this.panel.AddChildBox(currentPage);
                }
            }
        }
    }


    public class TabPage : UIBox
    {
        CustomRenderBox primElement;
        Color backColor;
        TabPageContainer owner;
        public TabPage(int width, int height)
            : base(width, height)
        {

        }
        public TabPageContainer Owner
        {
            get { return this.owner; }
            internal set { this.owner = value; }

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