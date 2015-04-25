//2015 MIT, WinterDev  
using System;
using System.Collections.Generic;
using LayoutFarm.HtmlBoxes;
using LayoutFarm.Scripting;

namespace LayoutFarm.WebDom
{
    public interface IHtmlDocument
    {
        IHtmlElement getElementById(string id);
        ITextNode createTextNode(object nodeContent);
        IHtmlElement createElement(string nodeName);
    }
    public interface IHtmlElement
    {
        void setAttribute(string attrName, string value);
        void appendChild(DomNode childNode);
        void attachEventListener(string eventName, HtmlEventHandler handler);
        void detachEventListener(string eventName, HtmlEventHandler handler);
        string innerHTML { get; set; }
    }
    public interface ITextNode
    {
    }
}