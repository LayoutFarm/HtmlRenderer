//BSD  2014 ,WinterDev  
using HtmlRenderer.Boxes;
using System.Collections.Generic;

namespace HtmlRenderer.WebDom
{

    public abstract class WebDocument
    {
        UniqueStringTable uniqueStringTable;
        Dictionary<string, DomElement> registerElementsById = new Dictionary<string, DomElement>();
        public WebDocument(UniqueStringTable uniqueStringTable)
        {
            this.uniqueStringTable = uniqueStringTable;
            this.DocumentState = WebDom.DocumentState.Init;
        }
        public abstract DomElement RootNode
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

        public DomAttribute CreateAttribute(string prefix, string localName)
        {
            return new DomAttribute(this,
                uniqueStringTable.AddStringIfNotExist(prefix),
                uniqueStringTable.AddStringIfNotExist(localName));
        }
        public abstract DomElement CreateElement(string prefix, string localName);

        public DomElement CreateElement(string localName)
        {
            return this.CreateElement(null, localName);
        }

        public DomComment CreateComent()
        {
            return new DomComment(this);
        }
        public DomProcessInstructionNode CreateProcessInstructionNode(int nameIndex)
        {
            return new DomProcessInstructionNode(this, nameIndex);
        }

        public abstract DomTextNode CreateTextNode(char[] strBufferForElement);

        public DomCDataNode CreateCDataNode()
        {
            return new DomCDataNode(this);
        }
        //-------------------------------------------------------
        internal void RegisterElementById(DomElement element)
        {
            //replace exisitng if exists *** 
            registerElementsById[element.AttrElementId] = element;
        }
        //-------------------------------------------------------
        public DocumentState DocumentState
        {
            get;
            private set;
        }
        public void SetDocumentState(DocumentState docstate)
        {
            this.DocumentState = docstate;
        }
    }

    public enum DocumentState
    {
        Init,
        Building,
        Idle,
        ChangedAfterIdle
    }

    public class WebDocumentFragment
    {
        WebDocument owner;
        public WebDocumentFragment(WebDocument owner)
        {
            this.owner = owner;
        }
    }

}