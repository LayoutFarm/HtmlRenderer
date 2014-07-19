//BSD  2014 ,WinterDev  
using HtmlRenderer.Boxes;
using System.Collections.Generic;

namespace HtmlRenderer.WebDom
{

    public abstract class HtmlDocument
    {
        UniqueStringTable uniqueStringTable;
        Dictionary<string, HtmlElement> registerElementsById = new Dictionary<string, HtmlElement>();
        public HtmlDocument(UniqueStringTable uniqueStringTable)
        {
            this.uniqueStringTable = uniqueStringTable;
            this.DocumentState = WebDom.DocumentState.Init;
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

        public HtmlElement CreateElement(string localName)
        {
            return this.CreateElement(null, localName);
        }

        public HtmlComment CreateComent()
        {
            return new HtmlComment(this);
        }
        public HtmlProcessInstructionNode CreateProcessInstructionNode(int nameIndex)
        {
            return new HtmlProcessInstructionNode(this, nameIndex);
        }

        public abstract HtmlTextNode CreateTextNode(char[] strBufferForElement);

        public HtmlCDataNode CreateCDataNode()
        {
            return new HtmlCDataNode(this);
        }

        //-------------------------------------------------------
        internal void RegisterElementById(HtmlElement element)
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
        BuildingElement,
        Layout,
        Render,
        Idle,
        ChangedAfterIdle
    }

}