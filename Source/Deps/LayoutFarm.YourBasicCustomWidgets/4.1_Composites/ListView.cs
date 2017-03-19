//Apache2, 2014-2017, WinterDev

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;
using LayoutFarm.UI;
namespace LayoutFarm.CustomWidgets
{


    public class ListView : UIBox
    {


        public delegate void ListItemMouseHandler(object sender, UIMouseEventArgs e);
        public delegate void ListItemKeyboardHandler(object sender, UIKeyEventArgs e);

        //composite          
        CustomRenderBox primElement;//background
        Color backColor = Color.LightGray;
        int viewportX, viewportY;
        UICollection uiList;
        List<ListItem> items = new List<ListItem>();
        int selectedIndex = -1;//default = no selection
        ListItem selectedItem = null;
        SimpleBox panel;

        public event ListItemMouseHandler ListItemMouseEvent;
        public event ListItemKeyboardHandler ListItemKeyboardEvent;

        public ListView(int width, int height)
            : base(width, height)
        {
            uiList = new UICollection(this);
            //panel for listview items
            //
            var simpleBox = new SimpleBox(width, height);
            simpleBox.ContentLayoutKind = BoxContentLayoutKind.VerticalStack;
            simpleBox.BackColor = Color.LightGray;
            simpleBox.MouseDown += panel_MouseDown;
            simpleBox.MouseDoubleClick += panel_MouseDoubleClick;
            simpleBox.AcceptKeyboardFocus = true;
            simpleBox.KeyDown += simpleBox_KeyDown;

            this.panel = simpleBox;
            uiList.AddUI(panel);
        }

        void simpleBox_KeyDown(object sender, UIKeyEventArgs e)
        {
            if (selectedItem != null && ListItemKeyboardEvent != null)
            {
                e.UIEventName = UIEventName.KeyDown;
                ListItemKeyboardEvent(this, e);
            }
            //switch (e.KeyCode)
            //{
            //    case UIKeys.Down:
            //        {
            //            e.CancelBubbling = true;
            //            SelectedIndex++;
            //        } break;
            //    case UIKeys.Up:
            //        {
            //            e.CancelBubbling = true;
            //            SelectedIndex--;
            //        } break;
            //    case UIKeys.Enter:
            //        {
            //            //accept selected item?

            //            if (selectedItem != null && ListItemKeyboardEvent != null)
            //            {
            //                ListItemKeyboardEvent(this, e);
            //            }
            //        }
            //        break;
            //    case UIKeys.Escape:
            //        //
            //        break;
            //}
        }
        void panel_MouseDoubleClick(object sender, UIMouseEventArgs e)
        {
            //raise event mouse double click
            var src = e.SourceHitElement as ListItem;
            if (src != null && ListItemMouseEvent != null)
            {
                e.UIEventName = UIEventName.DblClick;
                ListItemMouseEvent(this, e);
            }
        }
        void panel_MouseDown(object sender, UIMouseEventArgs e)
        {
            //check what item is selected
            var src = e.SourceHitElement as ListItem;
            if (src != null)
            {
                //make this as current selected list item
                //find index ?
                //TODO: review, for faster find list item index method
                int found = -1;
                for (int i = items.Count - 1; i >= 0; --i)
                {
                    if (items[i] == src)
                    {
                        found = i;
                        break;
                    }
                }
                if (found > -1)
                {
                    SelectedIndex = found;
                }
                if (ListItemMouseEvent != null)
                {
                    e.UIEventName = UIEventName.MouseDown;
                    ListItemMouseEvent(this, e);
                }
            }
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
                for (int m = 0; m < uiCount; ++m)
                {
                    renderE.AddChild(uiList.GetElement(m));
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
                    if (value < 0)
                    {
                        value = -1;
                    }
                    //-----------------------------
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
                            this.selectedItem = null;
                        }
                        else
                        {
                            //highlight selection item
                            this.selectedItem = GetItem(value);
                            selectedItem.BackColor = Color.Yellow;
                        }
                    }
                }
                else
                {
                    //do nothing 
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
        public void ScrollToSelectedItem()
        {
            //EnsureSelectedItemVisible();
            if (this.selectedIndex > -1)
            {
                //find the item height
                int topPos = selectedItem.Top;
                SetViewport(this.viewportX, topPos);
            }
        }
        public void EnsureSelectedItemVisible()
        {
            if (selectedIndex > -1)
            {
                //check if selected item is visible
                //if not bring them into view
                int topPos = selectedItem.Top;
                if (this.viewportY + ViewportHeight < topPos)
                {
                    SetViewport(this.viewportX, topPos - (ViewportHeight / 2));
                }
            }

        }

        //----------------------------------------------------

        public event EventHandler<UIMouseEventArgs> MouseDown;
        public event EventHandler<UIMouseEventArgs> MouseUp;
        public override void Walk(UIVisitor visitor)
        {
            visitor.BeginElement(this, "listview");
            this.Describe(visitor);
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
            //this.RegisterNativeEvent(1 << UIEventIdentifier.NE_MOUSE_DOWN);
            this.TransparentAllMouseEvents = true;
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
                element.SetController(this);

                listItemText = new CustomTextRun(rootgfx, 200, this.Height);
                element.AddChild(listItemText);
                listItemText.TransparentForAllEvents = true;
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

        public override void Walk(UIVisitor visitor)
        {
            visitor.BeginElement(this, "listitem");
            this.Describe(visitor);
            visitor.EndElement();
        }
    }
}