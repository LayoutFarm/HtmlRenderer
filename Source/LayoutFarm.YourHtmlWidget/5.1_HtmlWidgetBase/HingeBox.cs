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
        DomElement _floatPartDomElement;
        DomElement _presentationNode;
        Color _backColor = Color.LightGray;
        bool _isOpen;
        HingeFloatPartStyle _floatPartStyle;
        DomElement _div_floatingPart;
        DomElement _div_landingPoint;
        DomElement _div_glassCover;
        DomElement _span_textLabel;

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
            //---------------------------------------
            _div_glassCover = htmldoc.CreateElement("div");
            _div_glassCover.SetAttribute("style", "position:absolute;width:100%;height:100%;");
            _div_glassCover.AddChild(_div_floatingPart);
            _div_glassCover.AttachMouseDownEvent(e =>
            {
                //when click on cover glass
                CloseHinge();
            });
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
        public bool NeedUpdateDom { get; set; }
        void ItemSelected(LayoutFarm.UI.UIEventArgs e)
        {
            //some item is selected
            if (e.SourceHitElement is DomElement)
            {
                DomElement domElem = (DomElement)e.SourceHitElement;
                if (domElem.Tag != null)
                {
                    //selected value
                    _span_textLabel.ClearAllElements();
                    _span_textLabel.AddTextContent(domElem.Tag.ToString());
                    NeedUpdateDom = true;
                }
            }
            e.StopPropagation();
            CloseHinge();
        }
        public void AddItem(DomElement item)
        {
            if (_items == null)
            {
                _items = new List<DomElement>();
            }
            item.AttachMouseDownEvent(ItemSelected);

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
        public DomElement GetItem(int index) => _items[index];
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
            if (_presentationNode != null)
            {
                return _presentationNode;
            }
            //-------------------
            _presentationNode = htmldoc.CreateElement("div");
            _presentationNode.AddChild("div", div =>
            {
                div.SetAttribute("style", "font:10pt tahoma;");
               
                div.AddChild("img", img =>
                {
                    //init 
                    img.SetAttribute("src", WidgetResList.arrow_close);
                    img.AttachMouseDownEvent(e =>
                    {
                        if (this.IsOpen)
                        {
                            img.SetAttribute("src", WidgetResList.arrow_close);
                            this.CloseHinge();
                        }
                        else
                        {
                            img.SetAttribute("src", WidgetResList.arrow_open);
                            this.OpenHinge();
                        }

                        //----------------------------- 
                        e.StopPropagation();
                    });
                });

                div.AddChild("span", span1 =>
                {
                    _span_textLabel = span1;
                    span1.SetAttribute("style", "background-color:white;width:50px;height:20px;");
                    span1.AddTextContent("");
                });
            });

            _div_landingPoint = _presentationNode.AddChild("div", div =>
            {
                div.SetAttribute("style", "display:block");
            });
            //-------------------

            _floatPartDomElement = this.CreateFloatPartDom(htmldoc);
            return _presentationNode;
        }
        //
        public bool IsOpen => _isOpen;
        //
        public HingeFloatPartStyle FloatPartStyle
        {
            get => _floatPartStyle;
            set => _floatPartStyle = value;
        }
        //
        public void OpenHinge()
        {
            if (_isOpen) return;
            //----------------------
            _isOpen = true;
            switch (_floatPartStyle)
            {
                default:
                case HingeFloatPartStyle.Popup:
                    {

                        LayoutFarm.Composers.HtmlDocument htmldoc = _presentationNode.OwnerDocument as HtmlDocument;
                        var floatPartE = _floatPartDomElement as WebDom.Impl.HtmlElement;
                        var landPartE = _presentationNode as WebDom.Impl.HtmlElement;

                        //add the floating part to root node**
                        htmldoc.RootNode.AddChild(_div_glassCover);
                        //find location relate to the landing point 
                        _div_landingPoint.GetGlobalLocationRelativeToRoot(out int x, out int y);
                        //and set its location 
                        floatPartE.SetLocation(x, y);
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
            if (!_isOpen) return;
            //-------------------------------------
            _isOpen = false;
            if (_floatPartDomElement == null)
            {
                return;
            }
            //-------------------------------------
            switch (_floatPartStyle)
            {
                default:
                    {
                    }
                    break;
                case HingeFloatPartStyle.Popup:
                    {
                        if (_floatPartDomElement != null && _floatPartDomElement.ParentNode != null)
                        {
                            ((IHtmlElement)_div_glassCover.ParentNode).removeChild(_div_glassCover);
                        }
                    }
                    break;
                case HingeFloatPartStyle.Embeded:
                    {
                    }
                    break;
            }
        }

    }
}
