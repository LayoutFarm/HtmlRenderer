//BSD, 2014-present, WinterDev 

namespace LayoutFarm.Composers
{
    class HtmlShadowDocument : HtmlDocument
    {
        //this is not document fragment ***
        HtmlDocument _primaryHtmlDoc;
        internal HtmlShadowDocument(HtmlBoxes.HtmlHost host, HtmlDocument primaryHtmlDoc)
            : base(host, primaryHtmlDoc.UniqueStringTable)
        {
            //share string table with primary html doc
            _primaryHtmlDoc = primaryHtmlDoc;
        }
    }
}