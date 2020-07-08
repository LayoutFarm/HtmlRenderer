//BSD, 2014-present, WinterDev  

using System.Collections.Generic;
namespace LayoutFarm.WebDom
{
    public abstract class WebDocument
    {
        readonly UniqueStringTable _uniqueStringTable;
        Dictionary<string, DomElement> _registerElementsById;
        public WebDocument(UniqueStringTable uniqueStringTable)
        {
            _uniqueStringTable = uniqueStringTable;
            this.DocumentState = WebDom.DocumentState.Init;
        }
        public abstract DomElement RootNode { get; }
        public abstract int DomUpdateVersion { get; }
        public abstract void IncDomVersion();

        public int AddStringIfNotExists(string uniqueString) => _uniqueStringTable.AddStringIfNotExist(uniqueString);

        public string GetString(int index) => _uniqueStringTable.GetString(index);

        public int FindStringIndex(string uniqueString) => _uniqueStringTable.GetStringIndex(uniqueString);

        public DomAttribute CreateAttribute(string localName) => new DomAttribute(this,
                0,
                _uniqueStringTable.AddStringIfNotExist(localName));

        public DomAttribute CreateAttribute(string localName, string value) => new DomAttribute(this, 0, _uniqueStringTable.AddStringIfNotExist(localName)) { Value = value };

        public DomAttribute CreateAttributeWithPrefix(string prefix, string localName)
        {
            return new DomAttribute(this,
                _uniqueStringTable.AddStringIfNotExist(prefix),
                _uniqueStringTable.AddStringIfNotExist(localName));
        }

        public abstract DomElement CreateElement(string prefix, string localName);
        public abstract DomNode CreateDocumentNodeElement();
        public abstract DomElement CreateShadowRootElement();
        public DomElement CreateElement(string localName) => this.CreateElement(null, localName);

        public DomComment CreateComent() => new DomComment(this);

        public DomProcessInstructionNode CreateProcessInstructionNode(int nameIndex) => new DomProcessInstructionNode(this, nameIndex);

        public abstract DomTextNode CreateTextNode(char[] strBufferForElement);

        public DomCDataNode CreateCDataNode() => new DomCDataNode(this);

        //-------------------------------------------------------
        internal void RegisterElementById(DomElement element)
        {
            if (_registerElementsById == null) _registerElementsById = new Dictionary<string, DomElement>();
            //replace exisitng if exists *** 
            _registerElementsById[element.AttrElementId] = element;
        }
        public DomElement GetElementById(string elementId)
        {
            if (_registerElementsById == null) return null;
            _registerElementsById.TryGetValue(elementId, out DomElement found);
            return found;
        }
        //-------------------------------------------------------
        public DocumentState DocumentState { get; private set; }

        public void SetDocumentState(DocumentState docstate)
        {
            this.DocumentState = docstate;
        }
        public UniqueStringTable UniqueStringTable => _uniqueStringTable;

        public virtual bool IsDocFragment => false;
    }

    public enum DocumentState
    {
        Init,
        Building,
        Idle,
        ChangedAfterIdle
    }
}