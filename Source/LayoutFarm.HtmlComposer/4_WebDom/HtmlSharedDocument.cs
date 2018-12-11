//BSD, 2014-present, WinterDev 

namespace LayoutFarm.Composers
{
    class HtmlSharedDocument : HtmlDocument
    {
        //this is not document fragment ***
        HtmlDocument _primaryHtmlDoc;
        internal HtmlSharedDocument(HtmlDocument primaryHtmlDoc)
            : base(primaryHtmlDoc.Host, primaryHtmlDoc.UniqueStringTable)
        {
            //share string table with primary html doc
            _primaryHtmlDoc = primaryHtmlDoc;
        }
    }
}