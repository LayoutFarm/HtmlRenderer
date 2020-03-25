//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
using LayoutFarm.UI;
using LayoutFarm.Composers;

namespace LayoutFarm.HtmlWidgets
{
    public class ListView : HtmlWidgetBase
    {
        //composite           
        Color _backColor = KnownColors.LightGray;
        List<UICollection> _layers = new List<UICollection>(1);
        List<ListItem> _items = new List<ListItem>();
        int _selectedIndex = -1;//default = no selection
        HtmlElement _pnode;
        public ListView(int w, int h)
            : base(w, h)
        {
        }
        public override HtmlElement GetPresentationDomNode(Composers.HtmlElement orgDomElem)
        {
            if (_pnode != null) return _pnode;
            //--------------------------------
            _pnode = orgDomElem.OwnerHtmlDoc.CreateHtmlDiv();
            _pnode.SetStyleAttribute("font:10pt tahoma;overflow:scroll;height:300px;");
            int j = _items.Count;
            if (j > 0)
            {
                for (int i = 0; i < j; ++i)
                {
                    //itemnode
                    _pnode.AddChild(_items[i].GetPresentationNode(_pnode));
                }
            }
            return _pnode;
        }

        public void AddItem(ListItem ui)
        {
            _items.Add(ui);
            if (_pnode != null)
            {
                _pnode.AddChild(ui.GetPresentationNode(_pnode));
            }
        }
        //
        public int ItemCount => _items.Count;
        //
        public ListItem GetItem(int index)
        {
            if (index < 0)
            {
                return null;
            }
            else
            {
                return _items[index];
            }
        }
        public void Remove(ListItem item)
        {
            _items.Remove(item);
        }
        public void RemoveAt(int index)
        {
            var item = _items[index];
            _items.RemoveAt(index);
        }
        public void ClearItems()
        {
            _selectedIndex = -1;
            _items.Clear();
        }
        //----------------------------------------------------

        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                if (value < this.ItemCount)
                {
                    if (_selectedIndex != value)
                    {
                        //1. current item
                        if (_selectedIndex > -1)
                        {
                            //switch back
                            GetItem(_selectedIndex).BackColor = KnownColors.LightGray;
                        }

                        _selectedIndex = value;
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
    public class ListItem
    {
        HtmlElement _pnode;
        HtmlElement _textSpanNode;
        int width;
        int height;
        public ListItem(int width, int height)
        {
            this.width = width;
            this.height = height;
            Text = "";
        }
        public Color BackColor { get; set; }
        public string Text { get; set; }
        public HtmlElement GetPresentationNode(WebDom.DomElement hostNode)
        {
            if (_pnode != null) return _pnode;
            //------------------------------

            var ownerdoc = (HtmlDocument)hostNode.OwnerDocument;
            _pnode = ownerdoc.CreateHtmlDiv();
            // pnode.SetAttribute("style", "font:10pt tahoma");
            _textSpanNode = ownerdoc.CreateHtmlSpan();
            _textSpanNode.AddChild(ownerdoc.CreateTextNode(Text.ToCharArray()));
            _pnode.AddChild(_textSpanNode);
            return _pnode;
        }
    }
}