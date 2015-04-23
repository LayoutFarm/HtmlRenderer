//BSD  2014 ,WinterDev  
//using LayoutFarm.HtmlBoxes;
using System.Collections.Generic;

namespace LayoutFarm.WebDom
{

    public abstract class WebDocument
    {
        UniqueStringTable uniqueStringTable;
        Dictionary<string, DomElement> registerElementsById;

        public WebDocument(UniqueStringTable uniqueStringTable)
        {
            this.uniqueStringTable = uniqueStringTable;
            this.DocumentState = WebDom.DocumentState.Init;
        }
        public abstract DomElement RootNode
        {
            get;
        }
        public abstract int DomUpdateVersion { get; set; }
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
        public abstract DomElement CreateShadowRootElement();

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
            if (registerElementsById == null) this.registerElementsById = new Dictionary<string, DomElement>();

            //replace exisitng if exists *** 
            registerElementsById[element.AttrElementId] = element;
        }
        public DomElement GetElementById(string elementId)
        {
            if (registerElementsById == null) return null;
            DomElement found;
            registerElementsById.TryGetValue(elementId, out found);
            return found;
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
        public UniqueStringTable UniqueStringTable
        {
            get { return this.uniqueStringTable; }
        }
        public virtual bool IsDocFragment { get { return false; } }
    }

    public enum DocumentState
    {
        Init,
        Building,
        Idle,
        ChangedAfterIdle
    }

    

}