//MIT, 2015-present, WinterDev  

using System.Collections.Generic;
 
namespace LayoutFarm.WebDom.Impl
{
    partial class HtmlElement : IHtmlElement
    {
        Dictionary<string, string> _userDataDic;

        void IHtmlElement.setData(string key, string value)
        {
            if (key == null) return;

            if (_userDataDic == null)
            {
                _userDataDic = new Dictionary<string, string>();
            }

            if (value == null)
            {
                //remove the key
                _userDataDic.Remove(key);
            }
            else
            {
                _userDataDic[key] = value; //replace?
            }

        }
        string IHtmlElement.getData(string key)
        {
            if (_userDataDic == null) return null;
            //
            _userDataDic.TryGetValue(key, out string value);
            return value;
        }
        void IHtmlElement.setAttribute(string attrName, string value)
        {
            this.SetAttribute(attrName, value);
        }
        string IHtmlElement.getAttribute(string attrName)
        {
            return "";
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
            get => this.GetInnerHtml();
            set => this.SetInnerHtml(value);
        }

        string IHtmlElement.id => this.AttrElementId;

    }
}