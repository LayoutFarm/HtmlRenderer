//BSD, 2014-present, WinterDev 

namespace LayoutFarm.WebDom.Impl
{
    public class HtmlDocumentFragment : HtmlDocument
    {
        HtmlDocument _primaryHtmlDoc;
        internal HtmlDocumentFragment(HtmlDocument primaryHtmlDoc)
            : base(primaryHtmlDoc.UniqueStringTable)
        {
            //share string table with primary html doc
            _primaryHtmlDoc = primaryHtmlDoc;
        }
        public override bool IsDocFragment => true;
    }
}