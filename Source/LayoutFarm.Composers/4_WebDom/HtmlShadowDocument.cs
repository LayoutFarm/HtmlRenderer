// 2015,2014 ,BSD, WinterDev  
using System;
using System.Collections.Generic;
using LayoutFarm.HtmlBoxes;

namespace LayoutFarm.WebDom
{

    class HtmlShadowDocument : LayoutFarm.WebDom.Impl.HtmlShadowDocument
    {
        //this is not document fragment ***
        HtmlDocument primaryHtmlDoc;
        internal HtmlShadowDocument(HtmlDocument primaryHtmlDoc)
            : base(primaryHtmlDoc)
        {
            //share string table with primary html doc
            this.primaryHtmlDoc = primaryHtmlDoc;
        }

    }
}