//MIT, 2015-2018, WinterDev   
using LayoutFarm.Scripting;

namespace LayoutFarm.WebDom
{
    public interface INode
    {
    }

    [JsType]
    public interface IHtmlDocument
    {
        //TODO: 
        //current version we not support overload
        //resolve this
        IHtmlElement rootNode { get; }
        IHtmlElement getElementById(string id);
        ITextNode createTextNode(string nodeContent);
        IHtmlElement createElement(string nodeName);
        IHtmlDocument createDocumentFragment();
        IHtmlElement createShadowRootElement();
    }
    [JsType]
    public interface IHtmlElement : INode
    {
        void setAttribute(string attrName, string value);
        void appendChild(INode childNode);
        void attachEventListener(string eventName, HtmlEventHandler handler);
        void detachEventListener(string eventName, HtmlEventHandler handler);
        string innerHTML { get; set; }
        string id { get; }
        void clear();
        //----------------------
        void removeChild(DomNode childNode);
        void getGlobalLocation(out int x, out int y);
    }
     [JsType]
    public interface ITextNode : INode
    {
    }
}