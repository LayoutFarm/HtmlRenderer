//MIT, 2015-present, WinterDev  

using LayoutFarm.Scripting;
namespace LayoutFarm.WebDom.Impl
{
    partial class HtmlDocument : IHtmlDocument
    {

        IHtmlElement IHtmlDocument.getElementById(string id)
        {
            return this.GetElementById(id) as HtmlElement;
        }


        ITextNode IHtmlDocument.createTextNode(string nodeContent)
        {
            return this.CreateTextNode(nodeContent.ToCharArray());
        }

        IHtmlElement IHtmlDocument.createElement(string nodeName)
        {
            return (HtmlElement)this.CreateElement(nodeName);
        }


        IHtmlElement IHtmlDocument.rootNode
        {
            get { return (IHtmlElement)this.RootNode; }
        }

        IHtmlDocument IHtmlDocument.createDocumentFragment()
        {
            return (IHtmlDocument)this.CreateDocumentFragment();
        }

        IHtmlElement IHtmlDocument.createShadowRootElement()
        {
            return (IHtmlElement)this.CreateShadowRootElement();
        }
    }
}