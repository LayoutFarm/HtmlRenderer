//2015 MIT, WinterDev  
using System;
using System.Collections.Generic;
using LayoutFarm.HtmlBoxes;
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
        ITextNode IHtmlDocument.createTextNode(object nodeContent)
        {
            return (HtmlTextNode)this.CreateTextNode(nodeContent.ToString().ToCharArray());
        }
        [JsMethod]
        IHtmlElement IHtmlDocument.createElement(string nodeName)
        {
            return (HtmlElement)this.CreateElement(nodeName);
        }
    }

}