//MIT, 2015-2016, WinterDev  

using LayoutFarm.Scripting;
namespace LayoutFarm.WebDom.Impl
{
    partial class HtmlDocument : IHtmlDocument
    {
        [JsMethod]
        IHtmlElement IHtmlDocument.getElementById(string id)
        {
            return this.GetElementById(id) as HtmlElement;
        }

        [JsMethod]
        ITextNode IHtmlDocument.createTextNode(string nodeContent)
        {
            return this.CreateTextNode(nodeContent.ToCharArray());
        }
        [JsMethod]
        IHtmlElement IHtmlDocument.createElement(string nodeName)
        {
            return (HtmlElement)this.CreateElement(nodeName);
        }

        [JsProperty]
        IHtmlElement IHtmlDocument.rootNode
        {
            get { return (IHtmlElement)this.RootNode; }
        }
        [JsMethod]
        IHtmlDocument IHtmlDocument.createDocumentFragment()
        {
            return (IHtmlDocument)this.CreateDocumentFragment();
        }
        [JsMethod]
        IHtmlElement IHtmlDocument.createShadowRootElement()
        {
            return (IHtmlElement)this.CreateShadowRootElement();
        }
    }
}