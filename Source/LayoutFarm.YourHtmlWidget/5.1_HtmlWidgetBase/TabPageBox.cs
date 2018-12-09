//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
using LayoutFarm.WebDom;
using LayoutFarm.WebDom.Extension;
using LayoutFarm.UI;
namespace LayoutFarm.HtmlWidgets
{
    //tab page similar to listview
    public class TabPage
    {
        DomElement _titleNode;
        DomElement _contentNode;
        UIElement _contentUI;
        public TabPage()
        {
        }
        public string PageTitle
        {
            get;
            set;
        }
        internal TabPageContainer OwnerContainer
        {
            get;
            set;
        }
        public DomElement GetTitleNode(DomElement hostNode)
        {
            //-------------------------------------
            if (_titleNode != null) return _titleNode;
            //create dom node
            var ownerdoc = hostNode.OwnerDocument;
            _titleNode = ownerdoc.CreateElement("div");
            _titleNode.SetAttribute("style", "display:inline");
            _titleNode.AddChild("span", span =>
            {
                if (PageTitle == null)
                {
                    span.AddTextContent("");
                }
                else
                {
                    span.AddTextContent(this.PageTitle);
                }
                span.AttachMouseDownEvent(e =>
                {
                    if (this.OwnerContainer != null)
                    {
                        this.OwnerContainer.ChildNotifyTabMouseDown(this);
                    }
                });
            });
            ////mouse down on title
            //titleNode.AttachMouseDownEvent(e =>
            //{


            //});
            //-------------------------------------
            return _titleNode;
        }
        public DomElement GetPageBody(DomElement hostNode)
        {
            if (_contentNode != null) return _contentNode;
            WebDocument ownerdoc = hostNode.OwnerDocument;
            _contentNode = ownerdoc.CreateElement("div");
            if (_contentUI != null)
            {
                //add content ui to the body of page
                //creat html wrapper for this ...        

                //  throw new NotImplementedException();
                //reimpl here again
                LayoutFarm.Composers.HtmlDocument htmldoc = (LayoutFarm.Composers.HtmlDocument)ownerdoc;
                var wrapperElement = htmldoc.CreateWrapperElement("x",
                 (RootGraphic rootgfx, out RenderElement renderE, out object controller) =>
                 {
                     renderE = _contentUI.GetPrimaryRenderElement(rootgfx);
                     controller = _contentUI;

                 });
                _contentNode.AddChild(wrapperElement);

            }
            return _contentNode;
        }
        public UIElement ContentUI
        {
            get => _contentUI;
            set
            {
                _contentUI = value;
                //add ui to content node if 
                if (_contentNode != null)
                {
                }
            }
        }
    }
    public class TabPageContainer : HtmlWidgetBase
    {
        DomElement _pnode;
        DomElement _titleBar;
        DomElement _contentNode;
        DomElement _tabTitleList;
        Color _backColor = Color.LightGray;
        List<TabPage> _tabPageCollection = new List<TabPage>();
        TabPage _currentPage;
        int _currentSelectedIndex;
        public TabPageContainer(int width, int height)
            : base(width, height)
        {
        }
        //
        public List<TabPage> TabPageList => _tabPageCollection;
        //
        public override WebDom.DomElement GetPresentationDomNode(WebDom.Impl.HtmlDocument htmldoc)
        {
            if (_pnode != null) return _pnode;
            //------------------------------

            _pnode = htmldoc.CreateElement("div");
            _pnode.SetAttribute("style", "font:10pt tahoma");
            //------------------------------ 
            _titleBar = htmldoc.CreateElement("div");
            _titleBar.AddTextContent("hello tabPage");
            _pnode.AddChild(_titleBar);
            //------------------------------ 
            _tabTitleList = htmldoc.CreateElement("div");
            _pnode.AddChild(_tabTitleList);
            //------------------------------ 
            _contentNode = htmldoc.CreateElement("div");
            _pnode.AddChild(_contentNode);
            //------------------------------
            return _pnode;
        }
        public void AddItem(TabPage tabPage)
        {
            //1. store in collection

            _tabPageCollection.Add(tabPage);
            tabPage.OwnerContainer = this;
            if (_pnode != null)
            {
                if (_currentPage == null)
                {
                    _currentPage = tabPage;
                    //add tab button into list
                    _tabTitleList.AddChild(tabPage.GetTitleNode(_pnode));
                    //add page body
                    _contentNode.AddChild(tabPage.GetPageBody(_pnode));
                }
                else
                {
                    //add tab button into list
                    _tabTitleList.AddChild(tabPage.GetTitleNode(_pnode));
                }
            }
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
            get { return _currentSelectedIndex; }
            set
            {
                if (value > -1 && value < _tabPageCollection.Count
                    && _currentSelectedIndex != value)
                {
                    _currentSelectedIndex = value;
                    TabPage selectednedSelectedPage = _tabPageCollection[value];
                    //if (currentPage != null)
                    //{
                    //    this.panel.RemoveChildBox(currentPage);
                    //}
                    //this.currentPage = selectednedSelectedPage;
                    //this.panel.AddChildBox(currentPage);
                }
            }
        }
        //------------------------
        internal void ChildNotifyTabMouseDown(TabPage childPage)
        {
            //change content ***
            _contentNode.ClearAllElements();
            _contentNode.AddChild(childPage.GetPageBody(_contentNode));
        }
    }
}