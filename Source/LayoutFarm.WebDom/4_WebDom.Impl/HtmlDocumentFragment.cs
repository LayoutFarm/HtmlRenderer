//BSD, 2014-2016, WinterDev   

namespace LayoutFarm.WebDom.Impl
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