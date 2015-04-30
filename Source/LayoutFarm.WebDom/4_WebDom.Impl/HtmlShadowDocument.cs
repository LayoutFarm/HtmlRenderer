// 2015,2014 ,BSD, WinterDev  
using System;
using System.Collections.Generic;
using LayoutFarm.HtmlBoxes;
using LayoutFarm.Css;

namespace LayoutFarm.WebDom.Impl
{

    class HtmlShadowDocument : HtmlDocument
    {
        //this is not document fragment ***
        HtmlDocument primaryHtmlDoc;
        public HtmlShadowDocument(HtmlDocument primaryHtmlDoc)
            : base(primaryHtmlDoc.UniqueStringTable)
        {
            //share string table with primary html doc
            this.primaryHtmlDoc = primaryHtmlDoc;
        } 
    }
}