// 2015,2014 ,BSD, WinterDev  
using System;
using System.Collections.Generic;

using PixelFarm.Drawing;
using LayoutFarm.HtmlBoxes;
using LayoutFarm.Composers;
using LayoutFarm.Css;

namespace LayoutFarm.WebDom
{

    class HtmlSharedDocument : HtmlDocument
    {
        //this is not document fragment ***
        HtmlDocument primaryHtmlDoc;
        internal HtmlSharedDocument(HtmlDocument primaryHtmlDoc)
            : base(primaryHtmlDoc.UniqueStringTable)
        {
            //share string table with primary html doc
            this.primaryHtmlDoc = primaryHtmlDoc;
        }
       
    }
}