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
        UICollection uiList;
        List<ListItem> items = new List<ListItem>();
        int selectedIndex = -1;//default = no selection
        SimpleBox panel;
        public ListView(int width, int height)
            : base(width, height)
        {
            uiList = new UICollection(this);
            //panel for listview items
            this.panel = new SimpleBox(width, height);
            this.panel.PanelLayoutKind = BoxContentLayoutKind.VerticalStack;
            panel.BackColor = Color.LightGray;
            uiList.AddUI(panel);
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

                int uiCount = this.uiList.Count;
                PlainLayer plainLayer = renderE.GetDefaultLayer();

                for (int m = 0; m < uiCount; ++m)
                {
                    plainLayer.AddUI(uiList.GetElement(m));

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
            panel.AddChild(ui);
        }
        public int ItemCount
        {
            get { return this.items.Count; }
        }
        public void RemoveAt(int index)
        {
            var item = items[index];
            panel.RemoveChild(item);
            items.RemoveAt(index);

        }
        public ListItem GetItem(int index)
        {
            if (index < 0)
            {
                return null;
            }
            else
            {
                return items[index];
            }
        }
        public void Remove(ListItem item)
        {
            items.Remove(item);
            panel.RemoveChild(item);
        }
        public void ClearItems()
        {
            this.selectedIndex = -1;
            this.items.Clear();
            this.panel.ClearChildren();
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

        public override void Walk(UIVisitor visitor)
        {
            visitor.BeginElement(this, "listview");
            this.DescribeDimension(visitor);
            visitor.EndElement();
        }
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
        public override RenderElement CurrentPrimaryRenderElement
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


                listItemText = new CustomTextRun(rootgfx, 200, this.Height);
                element.AddChild(listItemText);

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
        //----------------- 
        public void AddChild(RenderElement renderE)
        {
            primElement.AddChild(renderE);
        }


        public override void Walk(UIVisitor visitor)
        {
            visitor.BeginElement(this, "listitem");
            this.DescribeDimension(visitor);
            visitor.EndElement();
        }
    }

}