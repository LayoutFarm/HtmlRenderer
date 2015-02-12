// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;
using LayoutFarm.WebDom;
using LayoutFarm.WebDom.Extension;
using LayoutFarm.UI;
using LayoutFarm.RenderBoxes;
using LayoutFarm.CustomWidgets;
namespace LayoutFarm.HtmlWidgets
{
    //tab page similar to listview

    public class TabPageContainer : LightHtmlWidgetBase
    {
        DomElement pnode;
        DomElement titleBar;
        DomElement contentBar;
        Color backColor = Color.LightGray;
        List<TabPage> tabPageCollection = new List<TabPage>();

        TabPage currentPage;
        int currentSelectedIndex;

        public TabPageContainer(int width, int height)
            : base(width, height)
        {
            //UICollection plainLayer = new UICollection(this);
            ////panel for listview items
            //this.panel = new Panel(width, height);
            //this.panel.PanelLayoutKind = PanelLayoutKind.VerticalStack;

            //panel.BackColor = Color.LightGray;
            //plainLayer.AddUI(panel);
            //this.layers.Add(plainLayer);
        }

        public List<TabPage> TabPageList
        {
            get { return this.tabPageCollection; }
        }
        protected override WebDom.DomElement GetPresentationDomNode(WebDom.DomElement hostNode)
        {
            if (pnode != null) return pnode;
            //------------------------------
            var ownerdoc = hostNode.OwnerDocument;
            pnode = ownerdoc.CreateElement("div");
            pnode.SetAttribute("style", "font:10pt tahoma");
            //------------------------------

            titleBar = ownerdoc.CreateElement("div");
            titleBar.AddTextContent("hello tabPage");

            pnode.AddChild(titleBar);

            contentBar = ownerdoc.CreateElement("div");
            pnode.AddChild(contentBar);
            //------------------------------
            return pnode;
        }
        public void AddItem(TabPage tabPage)
        {
            //1. store in collection
            tabPageCollection.Add(tabPage);
            if (pnode != null)
            {
                if (currentPage == null)
                {
                    currentPage = tabPage;
                }
                //add tab 
                this.titleBar.AddChild(tabPage.GetPresentationNode(pnode));
            }
            //ui.Owner = this;
            ////show only one page per time
            //if (currentPage == null)
            //{
            //    currentPage = ui;
            //    panel.AddChildBox(ui);
            //}
        }
        public void RemoveItem(TabPage p)
        {
            //p.Owner = null;
            //tabPageCollection.Remove(p);
            //panel.RemoveChildBox(p);
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
                    //if (currentPage != null)
                    //{
                    //    this.panel.RemoveChildBox(currentPage);
                    //}
                    //this.currentPage = selectednedSelectedPage;
                    //this.panel.AddChildBox(currentPage);
                }
            }
        }

    }


    public class TabPage
    {
        DomElement domNode;
        public TabPage()
        {

        }
        public string PageTitle
        {
            get;
            set;
        }
        public DomElement GetPresentationNode(WebDom.DomElement hostNode)
        {
            if (domNode != null) return domNode;
            //create dom node
            var ownerdoc = hostNode.OwnerDocument;
            this.domNode = ownerdoc.CreateElement("div");


            domNode.AddChild("span", span =>
            {
                if (PageTitle == null)
                {
                    span.AddTextContent("");
                }
                else
                {
                    span.AddTextContent(this.PageTitle);
                }
            });

            return domNode;
        }
    }
}