// 2015,2014 ,BSD, WinterDev 
//ArthurHub

using System;
using System.Collections.Generic;

using PixelFarm.Drawing;
using LayoutFarm.HtmlBoxes;
using LayoutFarm.Composers;

using LayoutFarm.Scripting;
namespace LayoutFarm.WebDom
{
    //only for scripting
    public interface IHtmlDocument
    {   
        HtmlElement getElementById(string id);
    }

    partial class HtmlDocument : IHtmlDocument
    {
        [JsMethod]
        HtmlElement IHtmlDocument.getElementById(string id)
        {
            return this.GetElementById(id) as HtmlElement;
        }
    }
}