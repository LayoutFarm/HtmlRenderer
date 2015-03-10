//2015 MIT, WinterDev 

using System;
using System.Collections.Generic;

using PixelFarm.Drawing;
using LayoutFarm.HtmlBoxes;
using LayoutFarm.Composers;

using LayoutFarm.Scripting;
namespace LayoutFarm.WebDom
{
    public interface IHtmlElement
    {
        //string innerHtml { get; set; } 
        void setAttribute(string attrName, string value);
        void appendChild(DomNode childNode);
        void attachEventListener(string eventName, HtmlEventHandler handler);
        void detachEventListener(string eventName, HtmlEventHandler handler);
        string innerHTML { get; set; }
    }

    partial class HtmlElement : IHtmlElement
    {
        [JsMethod]
        void IHtmlElement.setAttribute(string attrName, string value)
        {
            this.SetAttribute(attrName, value);
        }
        [JsMethod]
        void IHtmlElement.appendChild(DomNode childNode)
        {
            this.AddChild(childNode);
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
                        } break;
                    case "mouseup":
                        {
                            this.AttachEvent(UI.UIEventName.MouseUp, eventHandler);
                        } break;
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

    }

}