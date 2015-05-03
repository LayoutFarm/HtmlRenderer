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
    public class NewListView : NewHtmlWidgetBase
    {
        //composite           
        Color backColor = Color.LightGray;
        List<UICollection> layers = new List<UICollection>(1);
        List<ListItem> items = new List<ListItem>();
        int selectedIndex = -1;//default = no selection
        WebDom.DomElement pnode;

        public NewListView(int w, int h)
            : base(w, h)
        {
        }
        public override WebDom.DomElement GetPresentationDomNode(WebDom.Impl.HtmlDocument htmldoc)
        {
            if (pnode != null) return pnode;
            //--------------------------------
            pnode = htmldoc.CreateElement("div");
            pnode.SetAttribute("style", "font:10pt tahoma");
            int j = items.Count;
            if (j > 0)
            {
                for (int i = 0; i < j; ++i)
                {
                    //itemnode
                    pnode.AddChild(items[i].GetPresentationNode(pnode));
                }
            }
            return pnode;
        }
        public void AddItem(ListItem ui)
        {
            items.Add(ui);
            if (pnode != null)
            {
                pnode.AddChild(ui.GetPresentationNode(pnode));
            }
        }
        public int ItemCount
        {
            get { return this.items.Count; }
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
        public void RemoveAt(int index)
        {
            var item = items[index];
            items.RemoveAt(index);

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


    }
   

}