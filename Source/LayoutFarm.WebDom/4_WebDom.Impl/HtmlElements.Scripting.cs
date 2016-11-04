//MIT, 2015-2016, WinterDev  

using LayoutFarm.Scripting;
namespace LayoutFarm.WebDom.Impl
{
    partial class HtmlElement : IHtmlElement
    {
        [JsMethod]
        void IHtmlElement.setAttribute(string attrName, string value)
        {
            this.SetAttribute(attrName, value);
        }
        [JsMethod]
        void IHtmlElement.appendChild(INode childNode)
        {
            this.AddChild((DomNode)childNode);
        }
        [JsMethod]
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
        [JsMethod]
        void IHtmlElement.detachEventListener(string eventName, HtmlEventHandler handler)
        {
        }
        [JsMethod]
        void IHtmlElement.removeChild(DomNode domNode)
        {
            this.RemoveChild(domNode);
        }
        [JsMethod]
        void IHtmlElement.getGlobalLocation(out int x, out int y)
        {
            this.GetGlobalLocation(out x, out y);
        }
        [JsMethod]
        void IHtmlElement.clear()
        {
            this.ClearAllElements();
        }
        [JsProperty]
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
        [JsProperty]
        string IHtmlElement.id
        {
            get { return this.AttrElementId; }
        }
    }
}