//MIT, 2015-present, WinterDev  

using LayoutFarm.Scripting;
namespace LayoutFarm.WebDom.Impl
{
    partial class HtmlElement : IHtmlElement
    {

        void IHtmlElement.setAttribute(string attrName, string value)
        {
            this.SetAttribute(attrName, value);
        }

        void IHtmlElement.appendChild(INode childNode)
        {
            this.AddChild((DomNode)childNode);
        }
        void IHtmlElement.attachEventListener(string eventName, HtmlEventHandler handler)
        {
            //todo : reimplement event mapping here
            var eventHandler = handler as HtmlEventHandler;
            if (eventHandler != null)
            {
                switch (eventName)
                {
                    case "mousedown":
                        {
                            this.AttachEvent(UI.UIEventName.MouseDown, eventHandler);
                        }
                        break;
                    case "mouseup":
                        {
                            this.AttachEvent(UI.UIEventName.MouseUp, eventHandler);
                        }
                        break;
                }
            }
            else
            {
            }
        }

        void IHtmlElement.detachEventListener(string eventName, HtmlEventHandler handler)
        {
        }

        void IHtmlElement.removeChild(DomNode domNode)
        {
            this.RemoveChild(domNode);
        }

        void IHtmlElement.getGlobalLocation(out int x, out int y)
        {
            this.GetGlobalLocation(out x, out y);
        }

        void IHtmlElement.clear()
        {
            this.ClearAllElements();
        }

        string IHtmlElement.innerHTML
        {
            get
            {
                return this.GetInnerHtml();
            }
            set
            {
                this.SetInnerHtml(value);
            }
        }

        string IHtmlElement.id
        {
            get { return this.AttrElementId; }
        }
    }
}