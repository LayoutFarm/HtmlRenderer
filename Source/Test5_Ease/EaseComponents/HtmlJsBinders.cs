using LayoutFarm;
using System;
using NativeV8;
using VroomJs;
namespace LayoutFarm.WebDom.Wrap
{
    [JsType]
    public class HtmlDocument
    {
        WebDocument webdoc;
        public HtmlDocument() { }
        public HtmlDocument(WebDocument webdoc)
        {
            this.webdoc = webdoc;
        }

        [JsMethod]
        public HtmlElement getElementById(string id)
        {

            var found = webdoc.GetElementById(id);
            if (found == null)
            {
                return null;
            }
            else
            {
                return new HtmlElement(found);
            }
        }
    }

    [JsType]
    public class HtmlElement
    {
        WebDom.DomElement domE;
        public HtmlElement(WebDom.DomElement domE)
        {
            this.domE = domE;
        }
    }

}