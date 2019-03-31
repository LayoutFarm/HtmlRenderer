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

        HtmlElement _presentationNode;
        Color _backColor = Color.LightGray;
        bool _isOpen;
        HingeFloatPartStyle _floatPartStyle;

        HtmlElement _div_floatingPart;
        HtmlElement _div_floatingPart_shadow;

        HtmlElement _div_landingPoint;
        HtmlElement _div_glassCover;
        HtmlElement _span_textLabel;

        List<DomElement> _items;

        const int SHADOW_OFFSET = 5;
        public HingeBox(int w, int h)
            : base(w, h)
        {

        }
        void CreateFloatPartDom(HtmlDocument htmldoc)
        {

            _div_glassCover = htmldoc.CreateHtmlDiv();
            _div_glassCover.SetStyleAttribute("position:absolute;width:100%;height:100%;");

            //---------------------------------------
            //create shadow element for floating part
            _div_floatingPart_shadow = htmldoc.CreateHtmlDiv();
            _div_floatingPart_shadow.SetStyleAttribute("background-color:rgba(0,0,0,0.2);position:absolute;left:0px;top:0px;width:300px;height:500px;");
            _div_glassCover.AddChild(_div_floatingPart_shadow);
            //---------------------------------------
            _div_floatingPart = htmldoc.CreateHtmlDiv();
            _div_floatingPart.SetStyleAttribute("background-color:white;position:absolute;left:0px;top:0px;width:300px;height:500px;");
            if (_items != null)
            {
                int j = _items.Count;
                for (int i = 0; i < j; ++i)
                {
                    _div_floatingPart.AddChild(_items[i]);
                }

                _div_glassCover.AddChild(_div_floatingPart);
                _div_glassCover.AttachMouseDownEvent(e =>
                {
                    //when click on cover glass
                    CloseHinge();
                });
            }
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
            WebDom.Impl.HtmlElement srcElem = e.SourceHitElement as WebDom.Impl.HtmlElement;
            if (srcElem != null)
            {
                var domElem = e.SourceHitElement as WebDom.Impl.HtmlElement;
                if (domElem != null)
                {
                    //selected value
                    _span_textLabel.ClearAllElements();
                    _span_textLabel.AddTextContent(domElem.GetInnerText());
                }
                else
                {
#if DEBUG
                    _span_textLabel.ClearAllElements();
                    _span_textLabel.AddTextContent("???");
#endif
                }
                NeedUpdateDom = true;
                //}
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

        public override HtmlElement GetPresentationDomNode(Composers.HtmlElement orgDomElem)
        {
            if (_presentationNode != null)
            {
                return _presentationNode;
            }
            //-------------------

            _presentationNode = orgDomElem.OwnerHtmlDoc.CreateHtmlDiv();
            _presentationNode.AddHtmlDivElement(div =>
            {
                div.SetStyleAttribute("font:10pt tahoma;");
                div.AddHtmlImageElement(img =>
                {
                    //init  
                    img.SetImageSource(WidgetResList.arrow_close);
                    img.AttachMouseDownEvent(e =>
                    {
                        if (this.IsOpen)
                        {
                            img.SetImageSource(WidgetResList.arrow_close);
                            this.CloseHinge();
                        }
                        else
                        {
                            img.SetImageSource(WidgetResList.arrow_open);
                            this.OpenHinge();
                        }

                        //----------------------------- 
                        e.StopPropagation();
                    });
                });

                div.AddHtmlSpanElement(span1 =>
                {
                    _span_textLabel = span1;
                    span1.SetStyleAttribute("background-color:white;width:50px;height:20px;");
                    span1.AddTextContent("");
                });
            });

            _div_landingPoint = _presentationNode.AddHtmlDivElement(div =>
            {
                div.SetStyleAttribute("display:block");
            });

            //-------------------

            CreateFloatPartDom(orgDomElem.OwnerHtmlDoc);
            return _presentationNode;
        }

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

                        HtmlDocument htmldoc = _presentationNode.OwnerHtmlDoc;

                        //add the floating part to root node**
                        htmldoc.RootNode.AddChild(_div_glassCover);
                        //find location relate to the landing point 
                        _div_landingPoint.GetGlobalLocationRelativeToRoot(out int x, out int y);
                        //and set its location 

                        _div_floatingPart_shadow.SetLocation(x + SHADOW_OFFSET, y + SHADOW_OFFSET);
                        _div_floatingPart.SetLocation(x, y);
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
            if (_div_floatingPart == null)
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
                        if (_div_floatingPart != null && _div_floatingPart.ParentNode != null)
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
