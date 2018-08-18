//Apache2, 2014-present, WinterDev

using System.Collections.Generic;

using PixelFarm.Drawing;
using LayoutFarm.Composers;
using LayoutFarm.WebDom;
using LayoutFarm.WebDom.Extension;
namespace LayoutFarm.HtmlWidgets
{
    public enum HingeFloatPartStyle
    {
        Popup,
        Embeded
    }

    public class HingeBox : HtmlWidgetBase
    {
        DomElement floatPartDomElement;
        DomElement presentationNode;
        Color backColor = Color.LightGray;
        bool isOpen;
        HingeFloatPartStyle floatPartStyle;
        DomElement _div_floatingPart;
        List<DomElement> _items;
        public HingeBox(int w, int h)
            : base(w, h)
        {

        }
        DomElement CreateFloatPartDom(WebDom.Impl.HtmlDocument htmldoc)
        {
            //create land part 
            _div_floatingPart = htmldoc.CreateElement("div");
            _div_floatingPart.SetAttribute("style", "background-color:white;position:absolute;left:0px;top:0px;width:300px;height:500px;");
            if (_items != null)
            {
                int j = _items.Count;
                for (int i = 0; i < j; ++i)
                {
                    _div_floatingPart.AddChild(_items[i]);
                }
            }
            return _div_floatingPart;
        }
        //--------------
        public void ClearItems()
        {
            if (_items != null)
            {
                _items.Clear();
            }
            if (_div_floatingPart != null)
            {
                _div_floatingPart.ClearAllElements();
            }
        }
        public void AddItem(DomElement item)
        {
            if (_items == null)
            {
                _items = new List<DomElement>();
            }
            _items.Add(item);
            //
            //
            if (_div_floatingPart != null)
            {
                _div_floatingPart.AddChild(item);
            }

        }
        public void RemoveItem(int index)
        {
            DomElement elem = _items[index];
            _items.RemoveAt(index);

            if (_div_floatingPart != null)
            {
                _div_floatingPart.RemoveChild(elem);
            }
        }
        public DomElement GetItem(int index)
        {
            return _items[index];
        }
        public int ItemCount
        {
            get
            {
                if (_items == null) return 0;
                return _items.Count;
            }

        }
        public override DomElement GetPresentationDomNode(WebDom.Impl.HtmlDocument htmldoc)
        {
            if (presentationNode != null)
            {
                return presentationNode;
            }
            //-------------------
            presentationNode = htmldoc.CreateElement("div");
            presentationNode.AddChild("div", div =>
            {
                div.SetAttribute("style", "font:10pt tahoma;");
                div.AddChild("img", img =>
                {
                    //init 
                    img.SetAttribute("src", "arrow_close.png");
                    img.AttachMouseDownEvent(e =>
                    {
                        if (this.IsOpen)
                        {
                            img.SetAttribute("src", "arrow_close.png");
                            this.CloseHinge();
                        }
                        else
                        {
                            img.SetAttribute("src", "arrow_open.png");
                            this.OpenHinge();
                        }

                        //----------------------------- 
                        e.StopPropagation();
                    });
                });
            });
            //-------------------

            this.floatPartDomElement = this.CreateFloatPartDom(htmldoc);
            return presentationNode;
        }

        public bool IsOpen
        {
            get { return this.isOpen; }
        }


        public void OpenHinge()
        {
            if (isOpen) return;
            //----------------------
            this.isOpen = true;
            switch (floatPartStyle)
            {
                default:
                case HingeFloatPartStyle.Popup:
                    {
                        var htmldoc = this.presentationNode.OwnerDocument as HtmlDocument;
                        var floatPartE = this.floatPartDomElement as WebDom.Impl.HtmlElement;
                        var landPartE = this.presentationNode as WebDom.Impl.HtmlElement;
                        htmldoc.RootNode.AddChild(this.floatPartDomElement);

                        this.presentationNode.GetGlobalLocationRelativeToRoot(out int x, out int y);
                        float actualHeight = landPartE.GetActualHeightIndirect();
                        floatPartE.SetLocation(x, (int)(y + actualHeight));
                    }
                    break;
                case HingeFloatPartStyle.Embeded:
                    {
                    }
                    break;
            }
        }
        public void CloseHinge()
        {
            if (!isOpen) return;
            //-------------------------------------
            this.isOpen = false;
            if (floatPartDomElement == null)
            {
                return;
            }
            //-------------------------------------
            switch (floatPartStyle)
            {
                default:
                    {
                    }
                    break;
                case HingeFloatPartStyle.Popup:
                    {
                        if (this.floatPartDomElement != null && this.floatPartDomElement.ParentNode != null)
                        {
                            ((IHtmlElement)this.floatPartDomElement.ParentNode).removeChild(this.floatPartDomElement);
                        }
                    }
                    break;
                case HingeFloatPartStyle.Embeded:
                    {
                    }
                    break;
            }
        }

        public HingeFloatPartStyle FloatPartStyle
        {
            get { return this.floatPartStyle; }
            set
            {
                this.floatPartStyle = value;
            }
        }
    }
}
