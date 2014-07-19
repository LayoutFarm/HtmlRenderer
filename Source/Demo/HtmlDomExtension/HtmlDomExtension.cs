using System;
using HtmlRenderer.Composers;
using HtmlRenderer.WebDom;

namespace HtmlRenderer.Demo
{

    public delegate void Decorate(HtmlElement h);

    public static class BridgeHtmlExtension
    {
        //level 1
        public static HtmlElement AddChild(this HtmlElement elem, string elementName)
        {
            var newchild = elem.OwnerDocument.CreateElement(elementName);
            elem.AddChild(newchild);
            return newchild;
        }
        public static HtmlElement AddChild(this HtmlElement elem, string elementName, out HtmlElement elemExit)
        {
            var newchild = elem.OwnerDocument.CreateElement(elementName);
            elem.AddChild(newchild);
            elemExit = newchild;
            return newchild;
        }
        public static HtmlElement AddChild(this HtmlElement elem, string elementName, Decorate d)
        {
            var newchild = elem.OwnerDocument.CreateElement(elementName);
            elem.AddChild(newchild);
            if (d != null)
            {
                d(newchild);
            }

            return newchild;
        }


        public static void AddTextContent(this HtmlElement elem, string text)
        {
            var newTextNode = elem.OwnerDocument.CreateTextNode(text.ToCharArray());
            elem.AddChild(newTextNode);
        }
        //------------------------------------------------------------------------------

        //level 2
        public static void AttachMouseDownEvent(this HtmlElement elem, HtmlEventHandler hdl)
        {
            elem.AttachEvent(EventName.MouseDown, hdl);
        }
        public static void AttachMouseUpEvent(this HtmlElement elem, HtmlEventHandler hdl)
        {
            elem.AttachEvent(EventName.MouseUp, hdl);
        }
    }
}