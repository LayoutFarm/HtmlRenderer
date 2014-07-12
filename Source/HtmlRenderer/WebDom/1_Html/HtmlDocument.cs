//BSD  2014 ,WinterDev  
using HtmlRenderer.Boxes;
using HtmlRenderer.Composers;
namespace HtmlRenderer.WebDom
{



    public abstract class HtmlDocument
    {
        UniqueStringTable uniqueStringTable;//     
        public HtmlDocument(UniqueStringTable uniqueStringTable)
        {
            this.uniqueStringTable = uniqueStringTable;
        }
        public abstract HtmlElement RootNode
        {
            get;
        }

        public int AddStringIfNotExists(string uniqueString)
        {
            return uniqueStringTable.AddStringIfNotExist(uniqueString);
        }
        public string GetString(int index)
        {
            return uniqueStringTable.GetString(index);
        }
        public int FindStringIndex(string uniqueString)
        {
            return uniqueStringTable.GetStringIndex(uniqueString);
        }


        public HtmlAttribute CreateAttribute(string prefix, string localName)
        {
            return new HtmlAttribute(this,
                uniqueStringTable.AddStringIfNotExist(prefix),
                uniqueStringTable.AddStringIfNotExist(localName));
        }
        public abstract HtmlElement CreateElement(string prefix, string localName);
        public HtmlComment CreateComent()
        {
            return new HtmlComment(this);
        }
        public HtmlProcessInstructionNode CreateProcessInstructionNode(int nameIndex)
        {
            return new HtmlProcessInstructionNode(this, nameIndex);
        }

        public HtmlTextNode CreateTextNode(char[] strBufferForElement)
        {
            return new BridgeHtmlTextNode(this, strBufferForElement);
        }
        public HtmlCDataNode CreateCDataNode()
        {
            return new HtmlCDataNode(this);
        }
    }
}