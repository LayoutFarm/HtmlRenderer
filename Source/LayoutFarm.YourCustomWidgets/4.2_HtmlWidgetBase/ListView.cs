// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;

using LayoutFarm.UI;
using LayoutFarm.RenderBoxes;
using LayoutFarm.CustomWidgets;
using LayoutFarm.HtmlBoxes;

namespace LayoutFarm.HtmlWidgets
{
    public class ListView : LightHtmlWidgetBase
    {
        //composite           
        Color backColor = Color.LightGray;
        int viewportX, viewportY;
        List<UICollection> layers = new List<UICollection>(1);
        List<ListItem> items = new List<ListItem>();
        int selectedIndex = -1;//default = no selection

        public ListView(HtmlHost htmlhost, int w, int h)
            : base(htmlhost, w, h)
        {

        } 
        //----------------------------------------------------
        protected override Composers.FragmentHtmlDocument CreatePresentationDom()
        {
            throw new NotImplementedException();
        }
        public void AddItem(ListItem ui)
        {
            items.Add(ui);

        }
        public int ItemCount
        {
            get { return this.items.Count; }
        }
        public void RemoveAt(int index)
        {
            var item = items[index];
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

        }
        public void ClearItems()
        {
            this.selectedIndex = -1;
            this.items.Clear();
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
        public int ViewportX
        {
            get { return this.viewportX; }
        }
        public int ViewportY
        {
            get { return this.viewportY; }
        }

        //public override int ViewportX
        //{
        //    get { return this.viewportX; }

        //}
        //public override int ViewportY
        //{
        //    get { return this.viewportY; }

        //}
        public void SetViewport(int x, int y)
        {
            this.viewportX = x;
            this.viewportY = y;
            //set viewport 
            //if (this.HasReadyRenderElement)
            //{
            //    this.panel.SetViewport(x, y);
            //}
        }

    }
    public class ListItem
    {

        CustomTextRun listItemText;

        string itemText;
        Color backColor;
        int width;
        int height;
        public ListItem(int width, int height)
        {
            this.width = width;
            this.height = height;
        }
        public Color BackColor
        {
            get { return this.backColor; }
            set
            {
                this.backColor = value;
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
            //add content to list item

        }
    }

}