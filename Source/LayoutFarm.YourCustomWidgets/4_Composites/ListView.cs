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
        List<ListItem> items = new List<ListItem>();
        int selectedIndex = -1;//default = no selection
        Panel panel;
        public ListView(int width, int height)
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
                renderE.SetVisible(this.Visible);
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
        //----------------------------------------------------
        public void AddItem(ListItem ui)
        {
            items.Add(ui);
            panel.AddChildBox(ui);
        }
        public int ItemCount
        {
            get { return this.items.Count; }
        }
        public void RemoveAt(int index)
        {
            var item = items[index];
            panel.RemoveChildBox(item);
            items.RemoveAt(index);

        }
        public ListItem GetItem(int index)
        {
            return items[index];
        }
        public void Remove(ListItem item)
        {
            items.Remove(item);
            panel.RemoveChildBox(item);
        }
        public void ClearItems()
        {
            this.selectedIndex = -1;
            this.items.Clear();
            this.panel.ClearItems();
        }
        //----------------------------------------------------

        public int SelectedIndex
        {
            get { return this.selectedIndex; }
            set
            {
                if (value < this.ItemCount)
                {

                    if (this.selectedIndex != value)
                    {
                        //1. current item
                        if (selectedIndex > -1)
                        {
                            //switch back
                            GetItem(this.selectedIndex).BackColor = Color.LightGray;
                        }

                        this.selectedIndex = value;
                        if (value == -1)
                        {
                            //no selection
                        }
                        else
                        {
                            //highlight selection item
                            GetItem(this.SelectedIndex).BackColor = Color.Yellow;
                        }
                    }
                }
                else
                {
                    throw new Exception("out of range");
                }
            }
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
        CustomContainerRenderBox primElement;
        CustomTextRun listItemText;

        string itemText;
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
                //1.
                var element = new CustomContainerRenderBox(rootgfx, this.Width, this.Height);

                element.SetLocation(this.Left, this.Top);
                element.BackColor = this.backColor;


                listItemText = new CustomTextRun(rootgfx, this.Width, this.Height);
                element.AddChildBox(listItemText);

                if (this.itemText != null)
                {
                    listItemText.Text = this.itemText;
                }

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
        public string Text
        {
            get { return this.itemText; }
            set
            {
                this.itemText = value;
                if (listItemText != null)
                {
                    listItemText.Text = value;
                }
            }
        }


    }

}