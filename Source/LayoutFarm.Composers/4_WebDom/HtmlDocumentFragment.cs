// 2015,2014 ,BSD, WinterDev  
using System;
using System.Collections.Generic; 
using LayoutFarm.HtmlBoxes;
using LayoutFarm.Composers;
using LayoutFarm.Css;

namespace LayoutFarm.WebDom
{  
    public class HtmlDocumentFragment : HtmlDocument
    {   
        HtmlDocument primaryHtmlDoc;
        internal HtmlDocumentFragment(HtmlDocument primaryHtmlDoc)
            : base(primaryHtmlDoc.UniqueStringTable)
        {
            //share string table with primary html doc
            this.primaryHtmlDoc = primaryHtmlDoc;
        }
        public override bool IsDocFragment
        {
            get
            {
                return true;
            }
        }
    }
}