//BSD, 2014-present, WinterDev 

namespace LayoutFarm.WebDom.Impl
{
    class HtmlShadowDocument : HtmlDocument
    {
        //this is not document fragment ***
        HtmlDocument _primaryHtmlDoc;
        public HtmlShadowDocument(HtmlDocument primaryHtmlDoc)
            : base(primaryHtmlDoc.UniqueStringTable)
        {
            //share string table with primary html doc
            _primaryHtmlDoc = primaryHtmlDoc;
        }
    }
}