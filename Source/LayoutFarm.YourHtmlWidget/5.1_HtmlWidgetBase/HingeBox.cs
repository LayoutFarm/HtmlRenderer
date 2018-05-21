//Apache2, 2014-2018, WinterDev

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
        public HingeBox(int w, int h)
            : base(w, h)
        {
        }
        DomElement CreateFloatPartDom(WebDom.Impl.HtmlDocument htmldoc)
        {
            //create land part 
            var div = htmldoc.CreateElement("div");
            div.SetAttribute("style", "position:absolute;left:0px;top:0px;width:300px;height:500px;");
            for (int i = 0; i < 10; ++i)
            {
                div.AddChild("div", div2 =>
                {
                    div2.AddTextContent("HELLO!" + i);
                });
            }
            return div;
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
                    img.SetAttribute("src", "../Test3_MixHtml/Demo/arrow_close.png");
                    img.AttachMouseDownEvent(e =>
                    {
                        //img.SetAttribute("src", this.IsOpen ?
                        //    "../../Demo/arrow_open.png" :
                        //    "../../Demo/arrow_close.png");
                        ////------------------------------

                        if (this.IsOpen)
                        {
                            img.SetAttribute("src", "../Test3_MixHtml/Demo/arrow_close.png");
                            this.CloseHinge();
                        }
                        else
                        {
                            img.SetAttribute("src", "../Test3_MixHtml/Demo/arrow_open.png");
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
                        int x, y;
                        this.presentationNode.GetGlobalLocation(out x, out y);
                        floatPartE.SetLocation(x, y + (int)landPartE.ActualHeight);
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
            this.isOpen = false;
            if (floatPartDomElement == null)
            {
                return;
            }

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
