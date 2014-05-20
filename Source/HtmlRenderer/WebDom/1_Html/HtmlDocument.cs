//BSD  2014 ,WinterCore 
using System;
using System.Text;
using System.Collections;

namespace HtmlRenderer.WebDom
{

    public class HtmlDocument
    {

        UniqueStringTable uniqueStringTable = HtmlPredefineNames.CreateUniqueStringTableClone();
        HtmlRootNode rootNode;
        public HtmlDocument()
        {
            rootNode = new HtmlRootNode(this);
        }

        public HtmlRootNode RootNode
        {
            get
            {
                return rootNode;
            }
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

        public HtmlElement CreateElement(string prefix, string localName)
        {
            if (localName.EndsWith("\r\n"))
            {
            }
            return new HtmlElement(this,
                uniqueStringTable.AddStringIfNotExist(prefix),
                uniqueStringTable.AddStringIfNotExist(localName));
        }

        public HtmlComment CreateComent()
        {
            return new HtmlComment(this);
        }
        public HtmlProcessInstructionNode CreateProcessInstructionNode(int nameIndex)
        {
            return new HtmlProcessInstructionNode(this, nameIndex);
        }
        public HtmlTextNode CreateTextNode(string text)
        {
            return new HtmlTextNode(this, text);
        }
        public HtmlTextNode CreateTextNode(char[] strBufferForElement)
        {
            return new HtmlTextNode(this, strBufferForElement);
        }
        public HtmlCDataNode CreateCDataNode()
        {
            return new HtmlCDataNode(this);
        }
    }
}