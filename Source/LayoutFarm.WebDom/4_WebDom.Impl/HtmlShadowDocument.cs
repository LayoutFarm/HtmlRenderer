//BSD, 2014-2017, WinterDev 

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